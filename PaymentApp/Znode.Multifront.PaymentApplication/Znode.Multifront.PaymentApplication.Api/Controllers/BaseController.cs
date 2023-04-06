using Newtonsoft.Json;
using System;
using System.Collections.Specialized;
using System.Configuration;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Formatting;
using System.Net.Http.Headers;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using System.Xml;
using System.Xml.Serialization;
using Znode.Multifront.PaymentApplication.Api.Helpers;
using Znode.Multifront.PaymentApplication.Helpers;

namespace Znode.Multifront.PaymentApplication.Api.Controllers
{
    public class BaseController : ApiController
    {
        private string _domainName;
        private static QueryStringParser _queryStringParser;
        private static JsonMediaTypeFormatter _jsonMediaTypeFormatter;
        private static XmlMediaTypeFormatter _xmlMediaTypeFormatter;

        protected string RouteTemplate
        {
            get { return ControllerContext.RouteData.Route.RouteTemplate; }
        }

        protected string RouteUri
        {
            get
            {
                if (!String.IsNullOrEmpty(_domainName) && _domainName.IndexOf(":").Equals(-1))
                    return new UriBuilder(ControllerContext.Request.RequestUri.AbsoluteUri) { Host = _domainName }.Uri.ToString();
                return ControllerContext.Request.RequestUri.AbsoluteUri;
            }
        }

        protected static bool Indent
        {
            get
            {
                var indent = false;

                if (_queryStringParser.Indent.HasKeys())
                    if (!String.IsNullOrEmpty(_queryStringParser.Indent.Get("true")))
                        indent = true;
                return indent;
            }
        }

        protected static MediaTypeFormatter MediaTypeFormatter
        {
            get
            {
                if (_queryStringParser.Format.HasKeys())
                    if (!String.IsNullOrEmpty(_queryStringParser.Format.Get("xml")))
                        // XML response format must be done with the XmlSerializer
                        return _xmlMediaTypeFormatter ?? (_xmlMediaTypeFormatter = new XmlMediaTypeFormatter { UseXmlSerializer = true, Indent = Indent });
                return _jsonMediaTypeFormatter ?? (_jsonMediaTypeFormatter = new JsonMediaTypeFormatter() { Indent = Indent });
                // JSON is the default response format
            }
        }

        protected MediaTypeHeaderValue MediaTypeHeaderValue
        {
            get
            {
                if (MediaTypeFormatter.GetType() == typeof(XmlMediaTypeFormatter))
                    return new MediaTypeHeaderValue("application/xml");
                // JSON is the default
                return new MediaTypeHeaderValue("application/json");
            }
        }

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            _queryStringParser = new QueryStringParser(controllerContext.Request.RequestUri.Query);
            base.Initialize(controllerContext);
        }


        protected HttpResponseMessage CreateOKResponse<T>(string data)
        {
            if (Indent)
            {
                // Indentation should only ever be used by humans when viewing a response in a browser,
                // so taking the performance hit (albeit a small one) with the deserialization is fine.
                var dataDeserialized = JsonConvert.DeserializeObject<T>(data);
                return Request.CreateResponse(HttpStatusCode.OK, dataDeserialized, MediaTypeFormatter);
            }
            // We use StringContent to skip the content negotiation and serialization step (performance improvement).
            var response = new HttpResponseMessage { StatusCode = HttpStatusCode.OK, Content = new StringContent(data) };
            response.Content.Headers.ContentType = MediaTypeHeaderValue;
            return response;
        }

        protected HttpResponseMessage CreateOKResponse<T>(T data) => Request.CreateResponse(HttpStatusCode.OK, data, MediaTypeFormatter);

        protected HttpResponseMessage CreateOKResponse() => Request.CreateResponse(HttpStatusCode.OK);

        protected HttpResponseMessage CreateCreatedResponse<T>(T data) => Request.CreateResponse(HttpStatusCode.Created, data, MediaTypeFormatter);

        protected HttpResponseMessage CreateInternalServerErrorResponse<T>(T data)
        {
            var basedata = data as BaseResponse;

            if (!Equals(basedata, null))
            {
                var newEx = new Exception($"{basedata.ErrorCode}:{basedata.ErrorMessage}");
            }

            return Request.CreateResponse(HttpStatusCode.InternalServerError, data, MediaTypeFormatter);
        }

        protected HttpResponseMessage CreateInternalServerErrorResponse() => Request.CreateResponse(HttpStatusCode.InternalServerError);

        protected HttpResponseMessage CreateNotFoundResponse() => Request.CreateResponse(HttpStatusCode.NotFound);

        protected HttpResponseMessage CreateNoContentResponse() => Request.CreateResponse(HttpStatusCode.NoContent);

        protected HttpResponseMessage CreateBadRequestResponse<T>(T data) => Request.CreateResponse(HttpStatusCode.BadRequest, data, MediaTypeFormatter);

        protected HttpResponseMessage CreateUnauthorizedResponse<T>(T data)
        {
            var basedata = data as BaseResponse;

            if (!Equals(basedata, null))
            {
                var newEx = new Exception($"{basedata.ErrorCode}:{basedata.ErrorMessage}");
                // Elmah.ErrorSignal.FromCurrentContext().Raise(newEx);
            }
            return Request.CreateResponse(HttpStatusCode.Unauthorized, data, MediaTypeFormatter);
        }

        protected FilterCollection Filters => _queryStringParser.Filters;

        protected NameValueCollection Sorts => _queryStringParser.Sorts;

        protected NameValueCollection Page => _queryStringParser.Page;

    }

    public class BaseResponse
    {
        public int? ErrorCode { get; set; }
        public string ErrorMessage { get; set; }
        public bool HasError { get; set; }

        public string ToJson() => JsonConvert.SerializeObject(this, Newtonsoft.Json.Formatting.Indented);

        public string ToXml()
        {
            var xml = String.Empty;

            var serializer = new XmlSerializer(GetType());
            var memoryStream = new MemoryStream();

            using (var tw = new XmlTextWriter(memoryStream, Encoding.UTF8) { Formatting = System.Xml.Formatting.Indented })
            {
                serializer.Serialize(tw, this);
                memoryStream = tw.BaseStream as MemoryStream;

                if (!Equals(memoryStream, null))
                {
                    xml = new UTF8Encoding().GetString(memoryStream.ToArray());
                    memoryStream.Dispose();
                }
            }
            return xml;
        }
    }

    public class QueryStringParser
    {
        private readonly string _queryString;

        public NameValueCollection Expands
        {
            get { return GetKeyValuePairs("expand"); }
        }

        public FilterCollection Filters
        {
            get { return GetTuples("filter"); }
        }

        public NameValueCollection Sorts
        {
            get { return GetKeyValuePairs("sort"); }
        }

        public NameValueCollection Page
        {
            get { return GetKeyValuePairs("page"); }
        }

        public NameValueCollection Format
        {
            get { return GetKeyValuePairs("format"); }
        }

        public NameValueCollection Indent
        {
            get { return GetKeyValuePairs("indent"); }
        }

        public NameValueCollection Cache
        {
            get { return GetKeyValuePairs("cache"); }
        }

        public QueryStringParser(string queryString)
        {
            _queryString = queryString.ToLower();
        }

        private NameValueCollection GetKeyValuePairs(string param)
        {
            var keyValuePairs = new NameValueCollection();
            var query = HttpUtility.ParseQueryString(_queryString);

            var uriItemSeparator = ConfigurationManager.AppSettings["UriItemSeparator"];
            var uriKeyValueSeparator = ConfigurationManager.AppSettings["UriKeyValueSeparator"];

            foreach (var key in query.AllKeys)
            {
                if (Equals(key.ToLower(), param))
                {
                    var value = query.Get(key);
                    var items = value.Split(uriItemSeparator.ToCharArray());

                    foreach (var item in items)
                    {
                        if (item.Contains(uriKeyValueSeparator))
                        {
                            var set = item.Split(uriKeyValueSeparator.ToCharArray());
                            keyValuePairs.Add(set[0].ToLower(), HttpUtility.HtmlDecode(set[1]));
                        }
                        else
                        {
                            // Just make the value the same as the key, for consistency of code in other places
                            keyValuePairs.Add(item.ToLower(), item.ToLower());
                        }
                    }

                    break;
                }
            }

            return keyValuePairs;
        }

        private FilterCollection GetTuples(string param)
        {
            FilterCollection filters = new FilterCollection();
            var query = HttpUtility.ParseQueryString(_queryString);

            var uriItemSeparator = ConfigurationManager.AppSettings["UriItemSeparator"];
            var uriKeyValueSeparator = ConfigurationManager.AppSettings["UriKeyValueSeparator"];

            foreach (var key in query.AllKeys)
            {
                if (Equals(key.ToLower(), param))
                {
                    var value = query.Get(key);
                    var items = value.Split(uriItemSeparator.ToCharArray());

                    foreach (var item in items)
                    {
                        if (item.Contains(uriKeyValueSeparator))
                        {
                            var tuple = item.Split(uriKeyValueSeparator.ToCharArray());
                            var filterKey = tuple[0].ToLower().Trim();
                            var filterOperator = tuple[1].ToLower().Trim();
                            var filterValue = tuple[2].ToLower().Trim();

                            filters.Add(new FilterTuple(filterKey, filterOperator, HttpUtility.HtmlDecode(filterValue)));
                        }
                    }
                    break;
                }
            }
            return filters;
        }
    }
}
