using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Configuration;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Api.Client
{
    public abstract class BaseClient : IBaseClient
    {
        private string _domainName;
        private string _domainKey;
        private const string ApiTokenKey = "ApiToken";
        private const string MinifiedJsonResponseFromAPIKey = "MinifiedJsonResponseFromAPI";

        public int UserId { get; set; }
        public bool RefreshCache { get; set; }
        public int LoginAs { get; set; }
        public string Custom1 { get; set; }
        public string Custom2 { get; set; }
        public string Custom3 { get; set; }
        public string Custom4 { get; set; }
        public string Custom5 { get; set; }

        //To hold value set by setter method of RequestTimeout property.
        private int _apiRequestTimeout = 0;

        /// <summary>
        /// API request timeout in milliseconds.
        /// </summary>
        public int RequestTimeout
        {
            get
            {
                if (_apiRequestTimeout > 0)
                    return _apiRequestTimeout;

                //If invalid value is present for ZnodeApiRequestTimeout key in the web.config file then default value will be used.
                int defaultApiRequestTimeout = 10000000;
                int configApiRequestTimeout;
                int.TryParse(ConfigurationManager.AppSettings["ZnodeApiRequestTimeout"], out configApiRequestTimeout);
                return configApiRequestTimeout > 0 ? configApiRequestTimeout : defaultApiRequestTimeout;
            }
            set
            {
                _apiRequestTimeout = value;
            }
        }

        public string AccountHeader => UserId > 0 ? $"Znode-UserId: {UserId}" : string.Empty;
        public string LoginAsHeader => LoginAs > 0 ? $"Znode-LoginAsUserId: {LoginAs}" : string.Empty;
        public string SalesRepAsHeader => UserId > 0 ? $"Znode-SaleRepAsUserId: {UserId}" : string.Empty;

        private string _DomainHeader;

        private string _PublishStateHeader;

        private string _localeId;

        private string _profileId;

        private string _impersonationCSRId;

        private string _portalId;

       private string _ipAddress;

        public bool IsGlobalAPIAuthorization { get; set; } = Convert.ToBoolean(ConfigurationManager.AppSettings["IsGlobalAPIAuthorization"]);

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

        public string PrivateKey
        {
            get
            {
                return "Znode-PrivateKey: " + ConfigurationManager.AppSettings["ZnodePrivateKey"];
            }
        }

        public string PublishStateHeader
        {
            get
            {
                return "Znode-PublishState: " + _PublishStateHeader;
            }

            set { _PublishStateHeader = value; }
        }

        public string LocaleHeader
        {
            get
            {
                return "Znode-Locale: " + _localeId;
            }

            set { _localeId = value; }
        }

        public string ProfileHeader
        {
            get
            {
                return "Znode-ProfileId: " + _profileId;
            }

            set { _profileId = value; }
        }

        //Get the IPAddress of the user.
        public string IPAddress
        {
            get
            {
                try
                {
                    System.Web.HttpContext context = System.Web.HttpContext.Current;
                    _ipAddress = context.Request.UserHostAddress;
                    return "User-IpAddress: " + _ipAddress;
                }
                catch
                {
                    // This catch is required, It observe that in the dev express report initialization few repeated calls occur.
                    // Without the catch it will raise the below mentioned exception.
                    // Error : Value does not fall within the expected range.
                    return "User-IpAddress: " + _ipAddress;
                }

            }
            set { _ipAddress = value; }
        }

        //Get the IPAddress of the user.
        public string MinifiedJsonResponseHeader
        {
            get
            {
                bool b = ConfigurationManager.AppSettings[MinifiedJsonResponseFromAPIKey].TryParseBoolean();
                return ZnodeHttpHeaders.GetHeaderFormattedString(ZnodeHttpHeaders.Header_MinifiedJsonResponse, b.ToString());
            }
        }

        public string Token
        {
            get
            {
                return GetToken();
            }
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
        public string ImpersonationHeader
        {
            get
            {
                if (Convert.ToInt32(_impersonationCSRId) > 0)
                {
                    return "Znode-ImpersonationCSRId: " + _impersonationCSRId;
                }
                else
                {
                    return null;
                }
            }

            set { _impersonationCSRId = value; }
        }
        public string ImpersonationPortalHeader
        {
            get
            {
                if (Convert.ToInt32(_portalId) > 0)
                {
                    return "Znode-ImpersonationPortalId: " + _portalId;
                }
                else
                {
                    return null;
                }
            }

            set { _portalId = value; }
        }
        public string PaymentAPIDomainName { get; set; } = Convert.ToString(ConfigurationManager.AppSettings["ZnodePaymentApiDomainName"]);

        public string PaymentAPIDomainKey { get; set; } = Convert.ToString(ConfigurationManager.AppSettings["ZnodePaymentApiDomainKey"]);

        public string UriItemSeparator => ZnodeApiSettings.ZnodeApiUriItemSeparator;

        public string UriKeyValueSeparator => ZnodeApiSettings.ZnodeApiUriKeyValueSeparator;

        public string CommaReplacer => ZnodeApiSettings.ZnodeCommaReplacer;

        public string GetAuthorizationHeader(string domainName, string domainKey, string endpoint = "")
        {
            if (endpoint.ToLower().Contains(ZnodeAdminSettings.PaymentApplicationUrl.ToLower()) && !string.IsNullOrEmpty(ZnodeAdminSettings.PaymentApplicationUrl))
                return $"Authorization: Basic {EncodeBase64($"{PaymentAPIDomainName}|{PaymentAPIDomainKey}")}";
            else
                return $"Authorization: Basic {EncodeBase64($"{domainName}|{domainKey}")}";
        }

        public string GetAuthorizationHeader(string domainName, string domainKey) => $"Basic {EncodeBase64($"{domainName}|{domainKey}")}";

        public void SetPublishStateExplicitly(ZnodePublishStatesEnum publishState)
         => PublishStateHeader = publishState.ToString();

        public void SetDomainHeaderExplicitly(string domainName)
            => DomainHeader = "Znode-DomainName: " + domainName;

        public void SetLocaleExplicitly(int localeId)
         => _localeId = Convert.ToString(localeId);

        public void SetCustomHeadersExplicitly(Dictionary<string, string> headers)
        {
            int? count = headers?.Count;
            if (count > 0)
            {
                for (int i = 0; i < count; i++)
                {
                    switch (i.ToString())
                    {
                        case "0":
                            Custom1 = $"{headers.ElementAt(i).Key}:{headers.ElementAt(i).Value}";
                            break;
                        case "1":
                            Custom2 = $"{headers.ElementAt(i).Key}:{headers.ElementAt(i).Value}";
                            break;
                        case "2":
                            Custom3 = $"{headers.ElementAt(i).Key}:{headers.ElementAt(i).Value}";
                            break;
                        case "3":
                            Custom4 = $"{headers.ElementAt(i).Key}:{headers.ElementAt(i).Value}";
                            break;
                        case "4":
                            Custom5 = $"{headers.ElementAt(i).Key}:{headers.ElementAt(i).Value}";
                            break;
                    }
                }
            }
            else
            {
                Custom1 = string.Empty;
                Custom2 = string.Empty;
                Custom3 = string.Empty;
                Custom4 = string.Empty;
                Custom5 = string.Empty;
            }
        }

        public void SetProfileIdExplicitly(int profileId)
       => this._profileId = Convert.ToString(profileId);

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
            if (RefreshCache)
                endpoint = BuildCacheRefreshQueryString(endpoint);
            BuildPublishStateQueryString(endpoint);

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(endpoint);
            req.KeepAlive = false; // Prevents "server committed a protocol violation" error
            req.Method = "GET";
            req.Timeout = RequestTimeout;


            //Set header for api request
            SetHeaders(req, endpoint);

            T result = GetResultFromResponse<T>(req, status, baseEndPoint, "GET");
            return result;
        }

        /// <summary>
        /// Gets a resource from an endpoint.
        /// </summary>
        /// <typeparam name="T">The type of resource to retrieve.</typeparam>
        /// <param name="endpoint">The endpoint where the resource resides.</param>
        /// <param name="status">The status of the API call; treat this as an out parameter.</param>
        /// <returns>The resource.</returns>
        public async Task<T> GetResourceFromEndpointAsync<T>(string endpoint, ApiStatus status) where T : BaseResponse
        {
            string baseEndPoint = endpoint;
            if (RefreshCache)
                endpoint = BuildCacheRefreshQueryString(endpoint);
            BuildPublishStateQueryString(endpoint);

            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(endpoint);
            req.KeepAlive = false; // Prevents "server committed a protocol violation" error
            req.Method = "GET";
            req.Timeout = RequestTimeout;


            //Set header for api request.
            SetHeaders(req, endpoint);

            T result = await GetResultFromResponseAsync<T>(req, status, baseEndPoint, "GET");

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
        public async Task<T> PutResourceToEndpointAsync<T>(string endpoint, string data, ApiStatus status) where T : BaseResponse
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
            SetHeaders(req, endpoint);

            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(dataBytes, 0, dataBytes.Length);
            }

            T result = await GetResultFromResponseAsync<T>(req, status, baseEndPoint, "PUT", data);
            return result;
        }

        /// <summary>
        /// Post resource data to an endpoint.
        /// </summary>
        /// <typeparam name="T">The type of resource being updated.</typeparam>
        /// <param name="endpoint">The endpoint where the resource resides.</param>
        /// <param name="data">The data for the resource.</param>
        /// <param name="status">The status of the API call; treat this as an out parameter.</param>
        /// <returns>The resource.</returns>
        public async Task<T> PostResourceToEndpointAsync<T>(string endpoint, string data, ApiStatus status) where T : BaseResponse
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
            SetHeaders(req, endpoint);

            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(dataBytes, 0, dataBytes.Length);
            }

            T result = await GetResultFromResponseAsync<T>(req, status, baseEndPoint, "POST", data);
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
            SetHeaders(req, endpoint);
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
            SetHeaders(req, endpoint);

            using (Stream reqStream = req.GetRequestStream())
            {
                reqStream.Write(dataBytes, 0, dataBytes.Length);
            }

            T result = GetResultFromResponse<T>(req, status, baseEndPoint, "PUT", data);
            return result;
        }

        /// <summary>
        /// Deletes a resource from an endpoint.
        /// </summary>
        /// <typeparam name="T">The type of resource being deleted.</typeparam>
        /// <param name="endpoint">The endpoint where the resource resides.</param>
        /// <param name="status">The status of the API call; treat this as an out parameter.</param>
        /// <returns>True if the resource was deleted; otherwise, false.</returns>
        public bool DeleteResourceFromEndpoint<T>(string endpoint, ApiStatus status) where T : BaseResponse
        {
            string baseEndPoint = endpoint;
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(endpoint);
            req.KeepAlive = false; // Prevents "server committed a protocol violation" error
            req.Method = "DELETE";
            req.Timeout = RequestTimeout;


            //Set header for api request
            SetHeaders(req, endpoint);
            try
            {
                using (HttpWebResponse rsp = (HttpWebResponse)req.GetResponse())
                {
                    // Be sure to set the status code
                    status.StatusCode = rsp.StatusCode;

                    if (rsp.StatusCode == HttpStatusCode.NoContent || rsp.StatusCode.Equals(HttpStatusCode.OK))
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
                        case ErrorCodes.InvalidElasticSearchConfiguration:
                        case ErrorCodes.InvalidSqlConfiguration:
                        case ErrorCodes.InvalidZnodeLicense:
                            ThrowMisconfigurationException(result.ErrorCode, result.ErrorMessage);
                            break;
                        case ErrorCodes.UnAuthorized:
                            {
                                RemoveTokenFromCache();
                                return DeleteResourceFromEndpoint<T>(baseEndPoint, status);
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


            SetHeaders(req, endpoint);
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
                        case ErrorCodes.InvalidElasticSearchConfiguration:
                        case ErrorCodes.InvalidSqlConfiguration:
                        case ErrorCodes.InvalidZnodeLicense:
                            ThrowMisconfigurationException(result.ErrorCode, result.ErrorMessage);
                            break;
                        case ErrorCodes.UnAuthorized:
                            {
                                RemoveTokenFromCache();
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
                        case ErrorCodes.InvalidElasticSearchConfiguration:
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

        private async Task<T> GetResultFromResponseAsync<T>(HttpWebRequest request, ApiStatus status, string endpoint = "", string methodType = "", string data = "") where T : BaseResponse
        {
            T result = null;

            try
            {
                using (HttpWebResponse rsp = (HttpWebResponse)await request.GetResponseAsync())
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
                    // This deserialization is used to get the error information
                    result = DeserializeResponseStream<T>(rsp);
                    switch (result.ErrorCode)
                    {
                        case ErrorCodes.WebAPIKeyNotFound:
                            ThrowApiKeyNotFoundException();
                            break;
                        case ErrorCodes.InvalidDomainConfiguration:
                        case ErrorCodes.InvalidElasticSearchConfiguration:
                        case ErrorCodes.InvalidSqlConfiguration:
                        case ErrorCodes.InvalidZnodeLicense:
                            ThrowMisconfigurationException(result.ErrorCode, result.ErrorMessage);
                            break;
                        case ErrorCodes.UnAuthorized:
                            result = await HandleAsyncUnAuthorizedRequest<T>(status, endpoint, methodType, data);
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

        public string BuildEndpointQueryString(ExpandCollection expands) => BuildEndpointQueryString(expands, null, null, null, null);

        public string BuildEndpointQueryString(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize) =>
          string.Concat(BuildExpandQueryString(expands), BuildFilterQueryString(filters), BuildSortQueryString(sorts), BuildPageQueryString(pageIndex, pageSize), BuildPublishStateQueryString());


        private string BuildExpandQueryString(ExpandCollection expands)
        {
            string queryString = "?expand=";

            if (expands != null)
            {
                foreach (string e in expands)
                    queryString += e + UriItemSeparator;

                queryString = queryString.TrimEnd(UriItemSeparator.ToCharArray());
            }

            return queryString;
        }

        private string BuildPublishStateQueryString()
        {
            string queryString = "&publishState=";

            if (!string.IsNullOrEmpty(_PublishStateHeader))
            {
                queryString += _PublishStateHeader;
            }

            return queryString;
        }

        private string BuildFilterQueryString(FilterCollection filters)
        {
            string queryString = "&filter=";

            if (filters != null)
            {
                foreach (FilterTuple f in filters)
                    queryString += $"{f.FilterName}{UriKeyValueSeparator}{f.FilterOperator }{UriKeyValueSeparator }{HttpUtility.UrlEncode(f.FilterValue?.Replace(",", CommaReplacer))}{UriItemSeparator }";

                queryString = queryString.TrimEnd(UriItemSeparator.ToCharArray());
            }

            return queryString;
        }

        private string BuildSortQueryString(SortCollection sorts)
        {
            string queryString = "&sort=";

            if (sorts != null)
            {
                foreach (KeyValuePair<string, string> s in sorts)
                    queryString += $"{ s.Key}{UriKeyValueSeparator}{s.Value}{UriItemSeparator}";

                queryString = queryString.TrimEnd(UriItemSeparator.ToCharArray());
            }

            return queryString;
        }

        private string BuildPageQueryString(int? pageIndex, int? pageSize)
        {
            string queryString = "&page=";

            if (pageIndex.HasValue && pageSize.HasValue)
            {
                queryString += $"index{UriKeyValueSeparator}{pageIndex.Value}";
                queryString += UriItemSeparator;
                queryString += $"size{ UriKeyValueSeparator} { pageSize.Value}";
            }

            return queryString;
        }

        private string BuildCacheRefreshQueryString(string endpoint) => endpoint.Contains('?') ? endpoint + "&cache=refresh" : endpoint + "?cache=refresh";

        protected string BuildPublishStateQueryString(string endpoint)
         => endpoint.Contains('?') ? endpoint + "&publishState=" + _PublishStateHeader : endpoint + "?publishState=" + _PublishStateHeader;

        protected string BuildLocaleQueryString(string endpoint)
         => endpoint.Contains('?') ? endpoint + "&locale=" + _localeId : endpoint + "?locale=" + _localeId;

        protected string BuildCustomEndpointQueryString(string endpoint, string key, string value)
         => endpoint.Contains('?') ? endpoint + $"&{key}=" + HttpUtility.UrlEncode(value) : endpoint + $"?{key}=" + HttpUtility.UrlEncode(value);

        private string EncodeBase64(string value) => Convert.ToBase64String(Encoding.UTF8.GetBytes(value));


        //Handle unauthorized request and again request with valid token.
        private T HandleUnAuthorizedRequest<T>(ApiStatus status, string endpoint = "", string methodType = "", string data = "") where T : BaseResponse
        {
            //Remove Expired token from cache.
            RemoveTokenFromCache();

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

        private Task<T> HandleAsyncUnAuthorizedRequest<T>(ApiStatus status, string endpoint, string methodType, string data) where T : BaseResponse
        {
            //Remove Expired token from cache.
            RemoveTokenFromCache();

            switch (methodType.ToLower())
            {
                case "get":
                    return GetResourceFromEndpointAsync<T>(endpoint, status);
                case "post":
                    return PostResourceToEndpointAsync<T>(endpoint, data, status);
                case "put":
                    return PutResourceToEndpointAsync<T>(endpoint, data, status);
            }
            return GetResourceFromEndpointAsync<T>(endpoint, status);
        }

        //Set headers for api request.
        private void SetHeaders(HttpWebRequest req, string endpoint = "")
        {
            SetAuthorizationHeader(req, endpoint);
            SetAccountHeader(req);
            SetLoginAsHeader(req);
            SetSaleRepAsHeader(req);
            SetDomainHeader(req);
            SetCustomHeaders(req);
            SetPublishStateHeader(req);
            SetLocaleHeader(req);
            SetProfileHeader(req);
            SetTokenHeader(req);
            SetImpersonationHeader(req);
            SetImpersonationPortalHeader(req);
            SetIpAddress(req);
            SetMinifiedJsonResponseHeader(req);
            SetPrivateKeyHeader(req);
        }

        private void SetPrivateKeyHeader(HttpWebRequest req)
        {
            if (!string.IsNullOrEmpty(PrivateKey))
                req.Headers.Add(PrivateKey);
        }
       
        private void SetCustomHeaders(HttpWebRequest req)
        {
            if (!string.IsNullOrEmpty(Custom1))
                req.Headers.Add(Custom1);
            if (!string.IsNullOrEmpty(Custom2))
                req.Headers.Add(Custom2);
            if (!string.IsNullOrEmpty(Custom3))
                req.Headers.Add(Custom3);
            if (!string.IsNullOrEmpty(Custom4))
                req.Headers.Add(Custom4);
            if (!string.IsNullOrEmpty(Custom5))
                req.Headers.Add(Custom5);
        }
        //Set Domain header
        private void SetDomainHeader(HttpWebRequest req)
        {
            if (!string.IsNullOrEmpty(DomainHeader))
                req.Headers.Add(DomainHeader);
        }

        //Set IpAddress of user.
        private void SetIpAddress(HttpWebRequest req)
        {
            if (!string.IsNullOrEmpty(IPAddress))
                req.Headers.Add(IPAddress);
        }

        //Sets the header to receive minified JSON response from API. (It returns JSON by removing null properties and default value properties)
        private void SetMinifiedJsonResponseHeader(HttpWebRequest req)
        {
            if (!string.IsNullOrEmpty(MinifiedJsonResponseHeader))
                req.Headers.Add(MinifiedJsonResponseHeader);
        }


        private void SetPublishStateHeader(HttpWebRequest req)
        {
            if (!string.IsNullOrEmpty(PublishStateHeader))
                req.Headers.Add(PublishStateHeader);
        }

        private void SetLocaleHeader(HttpWebRequest req)
        {
            if (!string.IsNullOrEmpty(LocaleHeader))
                req.Headers.Add(LocaleHeader);
        }

        private void SetProfileHeader(HttpWebRequest req)
        {
            if (!String.IsNullOrEmpty(ProfileHeader))
                req.Headers.Add(ProfileHeader);
        }

        //Set login as header
        private void SetLoginAsHeader(HttpWebRequest req)
        {
            if (!string.IsNullOrEmpty(LoginAsHeader))
                req.Headers.Add(LoginAsHeader);
        }

        private void SetSaleRepAsHeader(HttpWebRequest req)
        {
            if (!string.IsNullOrEmpty(SalesRepAsHeader))
                req.Headers.Add(SalesRepAsHeader);
        }

        //Set Account header
        private void SetAccountHeader(HttpWebRequest req)
        {
            if (!string.IsNullOrEmpty(AccountHeader))
                req.Headers.Add(AccountHeader);
        }

        //Set Authorization request.
        private void SetAuthorizationHeader(HttpWebRequest req, string endpoint) =>
            req.Headers.Add(GetAuthorizationHeader(DomainName, DomainKey, endpoint));

        //Set Token Header
        private void SetTokenHeader(HttpWebRequest req)
        {
            if (ZnodeApiSettings.EnableTokenBasedAuthorization)
                req.Headers.Add(GetAPITokenHeader());
        }

        // set impersonation header
        private void SetImpersonationHeader(HttpWebRequest req)
        {
            if (!String.IsNullOrEmpty(ImpersonationHeader))
                req.Headers.Add(ImpersonationHeader);
        }
        // set impersonation header
        private void SetImpersonationPortalHeader(HttpWebRequest req)
        {
            if (!String.IsNullOrEmpty(ImpersonationPortalHeader))
                req.Headers.Add(ImpersonationPortalHeader);
        }
        //Get api token header.
        private string GetAPITokenHeader() => $"Token:{GetToken()}";

        //Get Token for api.
        private string GetToken()
        {
            string token = (string)HttpContext.Current.Cache[ApiTokenKey];

            if (string.IsNullOrEmpty(token))
                return GenerateAndInsertTokenIntoCache();

            return token;
        }

        //Gerate token and insert token into cache.
        private string GenerateAndInsertTokenIntoCache()
        {
            string endpoint = TokenEndpoint.GenerateToken();
            //Get response.
            ApiStatus status = new ApiStatus();
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(endpoint);
            req.KeepAlive = false;
            req.Method = "GET";

            SetAuthorizationHeader(req, endpoint);
            SetAccountHeader(req);
            SetLoginAsHeader(req);
            SetSaleRepAsHeader(req);
            SetDomainHeader(req);

            StringResponse response = GetResultFromResponse<StringResponse>(req, status);

            string token = Convert.ToString(status?.StatusCode) == HttpStatusCode.NotFound.ToString() ? string.Empty : response.Response;

            if (!string.IsNullOrEmpty(token))
                HttpContext.Current.Cache.Insert(ApiTokenKey, token);

            return !string.IsNullOrEmpty(token) ? (string)HttpContext.Current.Cache[ApiTokenKey] : token;
        }

        //Remove Expired token from cache.
        private void RemoveTokenFromCache() =>
            HttpContext.Current.Cache.Remove(ApiTokenKey);

        private void ThrowApiKeyNotFoundException()
        {
            RemoveTokenFromCache();
            throw new ZnodeException(ErrorCodes.WebAPIKeyNotFound, "Web API Key Not Found");
        }

        private void ThrowMisconfigurationException(int? errorCode, string errorMessage)
        {
            RemoveTokenFromCache();
            switch (errorCode)
            {
                case ErrorCodes.InvalidDomainConfiguration:
                case ErrorCodes.InvalidSqlConfiguration:
                case ErrorCodes.InvalidZnodeLicense:
                case ErrorCodes.InvalidElasticSearchConfiguration:
                    throw new ZnodeException(errorCode, errorMessage);
                default:
                    break;
            }
        }
    }
}
