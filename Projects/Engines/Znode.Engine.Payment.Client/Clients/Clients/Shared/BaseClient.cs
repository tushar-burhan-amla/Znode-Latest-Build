using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Payment.Client
{
    public abstract class BaseClient : IBaseClient
    {
        public int RequestTimeout { get; set; } = 10000000;
       
        private string _domainName;
        private string _domainKey;
        private string _DomainHeader;

        public string DomainHeader
        {
            get
            {
                if (!string.IsNullOrEmpty(_DomainHeader))
                    return _DomainHeader;

                return "Znode-DomainName: " + HttpContext.Current.Request.Url.Authority.Trim();
            }

            set { _DomainHeader = value; }
        }
        public string DomainName
        {
            get
            {
                if (!string.IsNullOrEmpty(_domainName))
                    return _domainName;

                return !IsGlobalAPIAuthorization ? HttpContext.Current.Request.Url.Authority : ConfigurationManager.AppSettings["ZnodeApiDomainName"];
            }

            set { _domainName = value; }
        }

        public string PrivateKey
        {
            get
            {
                return "Znode-PrivateKey: " + ConfigurationManager.AppSettings["ZnodePrivateKey"];
            }
        }

        public string DomainKey
        {
            get
            {
                if (!string.IsNullOrEmpty(_domainKey))
                    return _domainKey;

                return !IsGlobalAPIAuthorization ? ConfigurationManager.AppSettings[HttpContext.Current.Request.Url.Authority] : ConfigurationManager.AppSettings["ZnodeApiDomainKey"];
            }

            set { _domainKey = value; }
        }

        public bool IsGlobalAPIAuthorization { get; set; } = Convert.ToBoolean(ConfigurationManager.AppSettings["IsGlobalAPIAuthorization"]);


        public string PaymentAPIDomainName { get; set; } = Convert.ToString(ConfigurationManager.AppSettings["ZnodePaymentApiDomainName"]);

        public string PaymentAPIDomainKey { get; set; } = Convert.ToString(ConfigurationManager.AppSettings["ZnodePaymentApiDomainKey"]);

        public string GetAuthorizationHeader()
        {
            return $"Authorization: Basic {EncodeBase64($"{PaymentAPIDomainName}|{PaymentAPIDomainKey}")}";
        }

        public void CheckStatusAndThrow<T>(ApiStatus status, HttpStatusCode expectedStatusCode) where T : ZnodeException, new() => CheckStatusAndThrow<T>(status, new Collection<HttpStatusCode> { expectedStatusCode });

        public void CheckStatusAndThrow<T>(ApiStatus status, Collection<HttpStatusCode> expectedStatusCodes) where T : ZnodeException, new()
        {
            T ex = (T)Activator.CreateInstance(typeof(T), status.ErrorCode, status.ErrorMessage, status.StatusCode);

            // If status has an error, throw exception and get out early
            if (status.HasError) throw ex;

            // Check if the status code is in the list of ones we expect

            bool found = expectedStatusCodes != null && expectedStatusCodes.Any(statusCode => status.StatusCode == statusCode);
            // If we didn't find our status code, throw the exception
            if (!found) throw ex;

        }

        /// <summary>
        /// Gets a resource from an endpoint.
        /// </summary>
        /// <typeparam name="T">The type of resource to retrieve.</typeparam>
        /// <param name="endpoint">The endpoint where the resource resides.</param>
        /// <param name="status">The status of the API call; treat this as an out parameter.</param>
        /// <returns>The resource.</returns>
        public T GetResourceFromEndpoint<T>(string endpoint, ApiStatus status) where T : BaseResponse
        {
            string baseEndPoint = endpoint;

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(endpoint);
            req.KeepAlive = false; // Prevents "server committed a protocol violation" error
            req.Method = "GET";
            req.Timeout = RequestTimeout;

            //Set header for api request
            SetHeaders(req);

            T result = GetResultFromResponse<T>(req, status, baseEndPoint, "GET");
            return result;
        }

        /// <summary>
        /// Delete a resource from an endpoint.
        /// </summary>
        /// <typeparam name="T">The type of resource to retrieve.</typeparam>
        /// <param name="endpoint">The endpoint where the resource resides.</param>
        /// <param name="status">The status of the API call; treat this as an out parameter.</param>
        /// <returns>The resource.</returns>
        public T DeleteResourceFromEndpoint<T>(string endpoint, ApiStatus status) where T : BaseResponse
        {
            string baseEndPoint = endpoint;

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(endpoint);
            req.KeepAlive = false; // Prevents "server committed a protocol violation" error
            req.Method = "DELETE";
            req.Timeout = RequestTimeout;

            //Set header for api request
            SetHeaders(req);

            T result = GetResultFromResponse<T>(req, status, baseEndPoint, "DELETE");
            return result;
        }

        /// <summary>
        /// Posts resource data to an endpoint, usually for creating a new resource.
        /// </summary>
        /// <typeparam name="T">The type of resource being created.</typeparam>
        /// <param name="endpoint">The endpoint that accepts posting resource data.</param>
        /// <param name="data">The data for the resource.</param>
        /// <param name="status">The status of the API call; treat this as an out parameter.</param>
        /// <returns>The newly created resource.</returns>
        public T PostResourceToEndpoint<T>(string endpoint, string data, ApiStatus status) where T : BaseResponse
        {
            string baseEndPoint = endpoint;
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(endpoint);
            req.KeepAlive = false; // Prevents "server committed a protocol violation" error
            req.Method = "POST";
            req.ContentType = "application/json";
            req.ContentLength = dataBytes.Length;
            req.Timeout = RequestTimeout;

            //Set header for api request
            SetHeaders(req);
            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(dataBytes, 0, dataBytes.Length);
            }

            T result = GetResultFromResponse<T>(req, status, baseEndPoint, "POST", data);
            return result;
        }

        /// <summary>
        /// Puts resource data to an endpoint, usually for updating an existing resource.
        /// </summary>
        /// <typeparam name="T">The type of resource being updated.</typeparam>
        /// <param name="endpoint">The endpoint where the resource resides.</param>
        /// <param name="data">The data for the resource.</param>
        /// <param name="status">The status of the API call; treat this as an out parameter.</param>
        /// <returns>The updated resource.</returns>
        public T PutResourceToEndpoint<T>(string endpoint, string data, ApiStatus status) where T : BaseResponse
        {
            string baseEndPoint = endpoint;
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(endpoint);

            req.KeepAlive = false; // Prevents "server committed a protocol violation" error
            req.Method = "PUT";
            req.ContentType = "application/json";
            req.ContentLength = dataBytes.Length;
            req.Timeout = RequestTimeout;

            //Set header for api request
            SetHeaders(req);

            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(dataBytes, 0, dataBytes.Length);
            }

            T result = GetResultFromResponse<T>(req, status, baseEndPoint, "PUT", data);
            return result;
        }

        /// <summary>
        /// Get Boolean a resource from an endpoint.
        /// </summary>
        /// <typeparam name="T">The type of resource being deleted.</typeparam>
        /// <param name="endpoint">The endpoint where the resource resides.</param>
        /// <param name="status">The status of the API call; treat this as an out parameter.</param>
        /// <returns>True if the get resource; otherwise, false.</returns>
        public bool GetBooleanResourceFromEndpoint<T>(string endpoint, ApiStatus status) where T : BaseResponse
        {
            string baseEndPoint = endpoint;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(endpoint);
            req.KeepAlive = false; // Prevents "server committed a protocol violation" error
            req.Method = "GET";
            //Set header for api request
            req.Timeout = RequestTimeout;

            SetHeaders(req);
            try
            {
                using (HttpWebResponse rsp = (HttpWebResponse)req.GetResponse())
                {
                    // Be sure to set the status code
                    status.StatusCode = rsp.StatusCode;

                    if (rsp.StatusCode == HttpStatusCode.NoContent)
                        return true;
                }
            }
            catch (WebException ex)
            {
                LogRequestResponseDetails(req, ex, status.StatusCode);
                using (HttpWebResponse rsp = (HttpWebResponse)ex.Response)
                {
                    // This deserialization is used to get the error information
                    T result = DeserializeResponseStream<T>(rsp);
                    switch (result.ErrorCode)
                    {
                        case ErrorCodes.WebAPIKeyNotFound:
                            ThrowApiKeyNotFoundException();
                            break;
                        case ErrorCodes.InvalidDomainConfiguration:
                        case ErrorCodes.InvalidSqlConfiguration:
                        case ErrorCodes.InvalidZnodeLicense:
                            ThrowMisconfigurationException(result.ErrorCode, result.ErrorMessage);
                            break;
                        case ErrorCodes.UnAuthorized:
                            {
                                return GetBooleanResourceFromEndpoint<T>(baseEndPoint, status);
                            }
                    }

                    UpdateApiStatus(result, rsp, status);
                }
            }
            catch (Exception ex)
            {
                LogRequestResponseDetails(req, ex, status.StatusCode);
            }

            return false;
        }

        private T GetResultFromResponse<T>(HttpWebRequest request, ApiStatus status, string endpoint = "", string methodType = "", string data = "") where T : BaseResponse
        {
            T result = null;

            try
            {
                request.UserAgent = HttpContext.Current.Request.UserAgent;
                using (HttpWebResponse rsp = (HttpWebResponse)request.GetResponse())
                {
                    // This deserialization gives back the populated resource
                    result = DeserializeResponseStream<T>(rsp);
                    UpdateApiStatus(result, rsp, status);
                }
            }
            catch (WebException ex)
            {
                LogRequestResponseDetails(request, ex, status.StatusCode);
                using (HttpWebResponse rsp = (HttpWebResponse)ex.Response)
                {
                    result = DeserializeResponseStream<T>(rsp);

                    switch (result.ErrorCode)
                    {
                        case ErrorCodes.WebAPIKeyNotFound:
                            ThrowApiKeyNotFoundException();
                            break;
                        case ErrorCodes.InvalidDomainConfiguration:
                        case ErrorCodes.InvalidSqlConfiguration:
                        case ErrorCodes.InvalidZnodeLicense:
                            ThrowMisconfigurationException(result.ErrorCode, result.ErrorMessage);
                            break;
                        case ErrorCodes.UnAuthorized:
                            result = HandleUnAuthorizedRequest<T>(status, endpoint, methodType, data);
                            break;
                        default:
                            UpdateApiStatus(result, rsp, status);
                            break;
                    }

                }
            }
            catch (Exception ex)
            {
                LogRequestResponseDetails(request, ex, status.StatusCode);
            }

            return result;
        }

        /// <summary>
        /// Log request-response details to BaseClient Component
        /// Request Details : URL, ReqHeaders
        /// Response Details: Response StatusCode 
        /// </summary>
        /// <param name="request">HttpWebRequest Object</param>
        /// <param name="ex">exception object</param>
        /// <param name="statusCode">status code</param>
        private void LogRequestResponseDetails(HttpWebRequest request, Exception ex, HttpStatusCode statusCode)
        {
            StringBuilder stringBuilder = new StringBuilder();
            stringBuilder.Append($"URL - {request.RequestUri}{Environment.NewLine}")
            .Append($"Req-Headers - { Environment.NewLine + GetHeaderDetails(request.Headers)}")
            .Append($"Response Status - {statusCode}{Environment.NewLine}").Append($"Error Message - {ex.Message}");

            ZnodeLogging.LogMessage(stringBuilder.ToString(), "BaseClient", TraceLevel.Error);
        }

        /// <summary>
        /// Get Request Header Details
        /// loop through Request Header collection and stores it's key-value pairwise detail in string
        /// </summary>
        /// <param name="collection">Request Header Collection</param>
        /// <returns>string - Reqest Header Value and it's key</returns>
        private string GetHeaderDetails(WebHeaderCollection collection)
        {
            string header = "Request Header Details - ";
            NameValueCollection headers = collection;
            for (int index = 0; index < headers.Count; index++)
            {
                //Skip Logging of Authorization Key
                if (headers.GetKey(index) == "Authorization")
                    continue;
                header = string.Concat(header, $"Key - {headers.GetKey(index)}, Value- {headers.Get(index)}{Environment.NewLine}");
            }
            return header;
        }

        private T DeserializeResponseStream<T>(WebResponse response) where T : BaseResponse
        {
            if (response != null)
            {
                using (Stream body = response.GetResponseStream())
                {
                    if (body != null)
                    {
                        using (StreamReader stream = new StreamReader(body))
                        {
                            using (JsonTextReader jsonReader = new JsonTextReader(stream))
                            {
                                JsonSerializer jsonSerializer = new JsonSerializer();
                                try
                                {
                                    return jsonSerializer.Deserialize<T>(jsonReader);
                                }
                                catch (JsonReaderException ex)
                                {
                                    ZnodeLogging.LogMessage(ex.Message, string.Empty, TraceLevel.Error);
                                    throw new ZnodeException(null, ex.Message);
                                }
                                catch (Exception ex)
                                {
                                    ZnodeLogging.LogMessage(ex.Message, string.Empty, TraceLevel.Error);
                                }
                            }
                        }
                    }
                }
            }

            return default(T);
        }

        private void UpdateApiStatus<T>(T result, HttpWebResponse response, ApiStatus status) where T : BaseResponse
        {
            if (status == null)
                status = new ApiStatus();

            if (result != null)
            {
                status.HasError = result.HasError;
                status.ErrorCode = result.ErrorCode;
                status.ErrorMessage = result.ErrorMessage;
            }

            if (response != null) status.StatusCode = response.StatusCode;

        }

        private string EncodeBase64(string value) => Convert.ToBase64String(Encoding.UTF8.GetBytes(value));

        //Handle unauthorized request and again request with valid token.
        private T HandleUnAuthorizedRequest<T>(ApiStatus status, string endpoint = "", string methodType = "", string data = "") where T : BaseResponse
        {
            switch (methodType.ToLower())
            {
                case "get":
                    return GetResourceFromEndpoint<T>(endpoint, status);
                case "post":
                    return PostResourceToEndpoint<T>(endpoint, data, status);
                case "put":
                    return PutResourceToEndpoint<T>(endpoint, data, status);
            }
            return GetResourceFromEndpoint<T>(endpoint, status);
        }

        private void SetHeaders(HttpWebRequest req)
        {
            SetAuthorizationHeader(req);
            SetDomainHeader(req);
            SetPrivateKeyHeader(req);
        }

        // To set private key header.
        private void SetPrivateKeyHeader(HttpWebRequest req)
        {
            if (!string.IsNullOrEmpty(PrivateKey))
                req.Headers.Add(PrivateKey);
        }

        //Set Domain header
        private void SetDomainHeader(HttpWebRequest req)
        {
            if (!string.IsNullOrEmpty(DomainHeader))
                req.Headers.Add(DomainHeader);
        }


        //Set Authorization request.
        private void SetAuthorizationHeader(HttpWebRequest req) =>
            req.Headers.Add(GetAuthorizationHeader());

        private void ThrowApiKeyNotFoundException()
        {
            throw new ZnodeException(ErrorCodes.WebAPIKeyNotFound, "Web API Key Not Found");
        }

        private void ThrowMisconfigurationException(int? errorCode, string errorMessage)
        {
            switch (errorCode)
            {
                case ErrorCodes.InvalidDomainConfiguration:
                case ErrorCodes.InvalidSqlConfiguration:
                case ErrorCodes.InvalidZnodeLicense:
                    throw new ZnodeException(errorCode, errorMessage);
                default:
                    break;
            }
        }
    }
}
