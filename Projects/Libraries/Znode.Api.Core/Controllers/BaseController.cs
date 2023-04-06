using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Web;
using System.Web.Http;
using System.Web.Http.Controllers;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Api.Parser;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Api.Controllers
{
    public abstract class BaseController : ApiController
    {
        private string _domainName;
        private static QueryStringParser _queryStringParser;

        protected int PortalId
        {
            get { return ZnodeConfigManager.SiteConfig.PortalId; }
        }

        protected string PublishState
        {
            get { return HttpContext.Current.Request.Headers["Znode-PublishState"]; }
        }

        protected string RouteTemplate => ControllerContext.RouteData.Route.RouteTemplate;

        protected string RouteUri => GenerateRouteUri();

        protected static bool Indent
        {
            get
            {
                var indent = false;

                if (_queryStringParser.Indent.HasKeys() && !String.IsNullOrEmpty(_queryStringParser.Indent.Get("true")))
                    indent = true;

                return indent;
            }
        }

        protected override void Initialize(HttpControllerContext controllerContext)
        {
            bool validateAuthHeader = Convert.ToBoolean(ZnodeApiSettings.ValidateAuthHeader);
            if (validateAuthHeader)
            {
                bool headerOk = false;
                var authHeader = GetAuthHeader();

                if (HelperUtility.IsNotNull(authHeader))
                {
                    _domainName = HttpContext.Current.Request.Headers["Znode-DomainName"];
                    headerOk = CheckAuthHeader(authHeader[0], authHeader[1]);
                }

                if (!headerOk)
                {
                    HttpContext.Current.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
                    HttpContext.Current.Response.StatusDescription = "Domain name and key are either incorrect or missing from the request Authorization header.";
                    HttpContext.Current.Response.SuppressContent = true;
                    HttpContext.Current.ApplicationInstance.CompleteRequest();
                    throw new HttpResponseException(HttpStatusCode.Unauthorized);
                }
            }

            _queryStringParser = new QueryStringParser(controllerContext.Request.RequestUri.Query);
            base.Initialize(controllerContext);
        }

        /// <summary>
        /// Creates an OK response message, and writes given 'data' string directly into response.Content without any handling
        /// Instead of this method non-generic CreateOKResponse(string data); should be used.
        /// </summary>
        /// <typeparam name="T">Old implementation, now we don't need Type generic, but still </typeparam>
        /// <param name="data">Must be a JSON serialized string, anything else may fail silently or create issue in API client.</param>
        /// <returns>HttpResponseMessage having content object in StringContent format having UTF-8 encoding and application/json mimetype</returns>
        protected HttpResponseMessage CreateOKResponse<T>(string data)
        {
            return CreateOKResponse(data);
        }

        /// <summary>
        /// Creates an OK response message, and writes given 'data' string directly into response.Content without any handling
        /// </summary>
        /// <param name="data">Must be a JSON serialized string, anything else may fail silently or create issue in API client.</param>
        /// <returns>HttpResponseMessage having content object in StringContent format having UTF-8 encoding and application/json mimetype</returns>
        protected HttpResponseMessage CreateOKResponse(string data)
        {
            HttpResponseMessage response = Request.CreateResponse(HttpStatusCode.OK);
            response.Content = new StringContent(data, Encoding.UTF8, "application/json");
            return response;
        }

        /// <summary>
        /// Creates an OK response message, converts non-string types into JSON string before writing it into response.Content.
        /// and if a string is passed in 'data', it will write given 'data' string directly into response.Content without any handling
        /// Instead of this method non-generic CreateOKResponse(string data); should be used.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="data">If string, it must be an JSON serialized string, If any other object, it will be converted into JSON string.</param>
        /// <returns>HttpResponseMessage having content object in StringContent format having UTF-8 encoding and application/json mimetype</returns>
        protected HttpResponseMessage CreateOKResponse<T>(T data)
        {
            string d;
            if (typeof(T) == typeof(string))
            {
                d = data as string;
            }
            else
            {
                //We can directly use Response.CreateResponse(OK, data), but that will use WebAPI framework to serialize the object
                //Rather, we want to use our own JSON serializer, which creates a minified JSON, if requested.
                //JsonConvert uses global setting, setup in Global.asax.cs to minify the JSON
                d = ApiHelper.ToJson(data);
            }

            return CreateOKResponse(d);
        }

        protected HttpResponseMessage CreateOKResponse() => Request.CreateResponse(HttpStatusCode.OK);

        protected HttpResponseMessage CreateResponse<T>(Func<string> method, string componentName) where T : BaseResponse, new()
        {
            HttpResponseMessage response;

            try
            {
                string data = method();
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<T>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                TraceLevel level = (ex is ZnodeException) ? TraceLevel.Warning : TraceLevel.Error;
                T customResponse = Activator.CreateInstance<T>();
                customResponse.HasError = true;
                customResponse.ErrorMessage = ex.Message;
                ZnodeLogging.LogMessage(ex, componentName, level);
                response = CreateInternalServerErrorResponse(customResponse);
            }

            return response;
        }
        protected HttpResponseMessage CreateCreatedResponse<T>(T data) => Request.CreateResponse(HttpStatusCode.Created, data);

        protected HttpResponseMessage CreateInternalServerErrorResponse<T>(T data)
        {
            var basedata = data as BaseResponse;

            if (basedata != null)
            {
                var newEx = new Exception(basedata.ErrorCode + ":" + basedata.ErrorMessage);
            }

            return Request.CreateResponse(HttpStatusCode.InternalServerError, data);
        }

        protected HttpResponseMessage CreateInternalServerErrorResponse() => Request.CreateResponse(HttpStatusCode.InternalServerError);

        protected HttpResponseMessage CreateNotFoundResponse() => Request.CreateResponse(HttpStatusCode.NotFound);

        protected HttpResponseMessage CreateNoContentResponse() => Request.CreateResponse(HttpStatusCode.NoContent);

        protected HttpResponseMessage CreateUnauthorizedResponse<T>(T data)
        {
            var basedata = data as BaseResponse;

            if (basedata != null)
            {
                var newEx = new Exception(basedata.ErrorCode + ":" + basedata.ErrorMessage);
            }
            return Request.CreateResponse(HttpStatusCode.Unauthorized, data);
        }

        private string[] GetAuthHeader()
        {
            var headers = HttpContext.Current.Request.Headers;
            var authValue = headers.AllKeys.Contains("Authorization") ? headers["Authorization"] : String.Empty;

            // If auth value doesn't exist, get out
            if (String.IsNullOrEmpty(authValue)) return null;

            // Strip off the "Basic "
            authValue = authValue.Remove(0, 6);

            // Decode it; if empty then get out
            var authValueDecoded = DecodeBase64(authValue);
            if (String.IsNullOrEmpty(authValueDecoded)) return null;

            // Now split it to get the domain info (index 0 = domain name, index 1 = domain key)
            return authValueDecoded.Split('|');
        }

        private bool CheckAuthHeader(string domainName, string domainKey)
        {
            // If either domain name or domain key are empty, get out
            if (String.IsNullOrEmpty(domainName) || String.IsNullOrEmpty(domainKey)) return false;

            // Get the configured key for the domain
            var configuredDomainKey = GetConfiguredDomainKey(domainName);

            // Now compare the two
            return String.Compare(domainKey, configuredDomainKey, StringComparison.InvariantCulture) == 0;
        }

        private string DecodeBase64(string encodedValue) => Encoding.UTF8.GetString(Convert.FromBase64String(encodedValue));


        private string GetConfiguredDomainKey(string domainName)
        {
            var domainConfig = ZnodeConfigManager.GetDomainConfig(domainName);
            return !Equals(domainConfig, null) ? domainConfig.ApiKey : String.Empty;
        }

        protected string GetUriLocation(string Param)
        {
            var uri = Request.RequestUri;
            var location = uri.Scheme + "://" + uri.Host + uri.AbsolutePath + "/" + Param;
            return location;
        }

        protected string GenerateRouteUri()
        {
            string uri = (!String.IsNullOrEmpty(_domainName) && _domainName.IndexOf(":").Equals(-1)) ? new UriBuilder(ControllerContext.Request.RequestUri.AbsoluteUri) { Host = _domainName }.Uri.ToString() : ControllerContext.Request.RequestUri.AbsoluteUri;
            return uri;
        }

        protected HttpResponseMessage CreateExceptionResponse(Exception exception, bool logException = true, ZnodeLogging.Components loggingComponent = ZnodeLogging.Components.Admin, TraceLevel traceLevel = TraceLevel.Error)
        {
            if (logException)
                ZnodeLogging.LogMessage(exception.ToString(), loggingComponent.ToString(), traceLevel);

            if (exception is ZnodeException)
            {
                ZnodeException znodeException = exception as ZnodeException;

                return Request.CreateResponse(znodeException.StatusCode, new BaseResponse
                {
                    ErrorCode = znodeException.ErrorCode,
                    ErrorMessage = znodeException.ErrorMessage,
                    HasError = true
                });
            }

            return Request.CreateResponse(HttpStatusCode.InternalServerError, new BaseResponse
            {
                ErrorMessage = exception.ToString(),
                HasError = true
            });
        }
    }
}