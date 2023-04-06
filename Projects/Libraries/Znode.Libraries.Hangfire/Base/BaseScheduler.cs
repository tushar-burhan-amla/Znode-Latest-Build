using Newtonsoft.Json;

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Web;

using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models.Responses;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Libraries.Hangfire
{
    public abstract class BaseScheduler
    {
        protected readonly string UriItemSeparator = ",";
        protected readonly string UriKeyValueSeparator = "~";

        protected const string UserHeader = ZnodeConstant.Znode_UserId;
        protected const string AuthorizationHeader = ZnodeConstant.Authorization;
        protected const string TokenHeader = ZnodeConstant.Token;

        protected int UnAuthorizedErrorCode = 32;
        protected int RequestTimeout = 600000;//10 min
        protected string TokenInvalidErrorMessage = "Token is expired";
        protected string LoggingComponent = ZnodeLogging.Components.ERP.ToString();

        protected abstract string SchedulerName { get; }

        #region Public Methods

        public bool CheckTokenIsInvalid(WebException ex)
        {
            bool isTokenInValid = false;
            if (ex?.Response != null)
            {
                using (WebResponse response = ex.Response)
                {
                    using (Stream data = response?.GetResponseStream())
                        if (data != null)
                        {
                            using (var reader = new StreamReader(data))
                            {
                                BaseResponse objBaseResponse = new BaseResponse();
                                objBaseResponse = JsonConvert.DeserializeObject<BaseResponse>(reader?.ReadToEnd());
                                if (objBaseResponse?.ErrorCode == UnAuthorizedErrorCode && objBaseResponse?.ErrorMessage == TokenInvalidErrorMessage)
                                {
                                    return isTokenInValid = true;
                                }
                            }
                        }
                }
            }
            return isTokenInValid;
        }

        public string GetToken(string ApiUrl, string AuthorizationHeader)
        {
            string token = string.Empty;
            StringResponse result = new StringResponse();
            string endpoint = $"{ApiUrl}/token/generatetoken";

            //Get response.
            ApiStatus status = new ApiStatus();
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(endpoint);
            req.KeepAlive = false;
            req.Method = "GET";
            req.Timeout = RequestTimeout;
            req.Headers.Add($"Authorization: Basic { AuthorizationHeader }");
            try
            {
                using (HttpWebResponse response = (HttpWebResponse)req.GetResponse())
                {
                    Stream datastream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(datastream);
                    string jsonString = reader.ReadToEnd();
                    result = JsonConvert.DeserializeObject<StringResponse>(jsonString);
                    token = result?.Response;
                    reader.Close();
                    datastream.Close();
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error);
            }
            return token;
        }
        #endregion

        #region Protected Methods
        //Build the query string for request
        protected virtual string BuildEndpointQueryString(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            // IMPORTANT: Expand always starts with ? while all the others start with &, which
            // means the expands must be added first when building the querystring parameters.

            string queryString = BuildExpandQueryString(expands);
            queryString += BuildFilterQueryString(filters);
            queryString += BuildSortQueryString(sorts);
            queryString += BuildPageQueryString(pageIndex, pageSize);

            return queryString;
        }

        //Generate query string for Expand
        protected virtual string BuildExpandQueryString(ExpandCollection expands)
        {
            string queryString = "?expand=";

            if (expands != null)
            {
                foreach (string e in expands)
                {
                    queryString += e + UriItemSeparator;
                }

                if (!string.IsNullOrEmpty(UriItemSeparator))
                {
                    queryString = queryString.TrimEnd(UriItemSeparator.ToCharArray());
                }
            }

            return queryString;
        }

        //Generate query string for Filter
        protected virtual string BuildFilterQueryString(FilterCollection filters)
        {
            string queryString = "&filter=";

            if (filters != null)
            {
                foreach (FilterTuple f in filters)
                {
                    queryString += $"{f.FilterName}{UriKeyValueSeparator}{f.FilterOperator }{UriKeyValueSeparator }{HttpUtility.UrlEncode(f.FilterValue)}{UriItemSeparator }";
                }

                if (!string.IsNullOrEmpty(UriItemSeparator))
                {
                    queryString = queryString.TrimEnd(UriItemSeparator.ToCharArray());
                }
            }

            return queryString;
        }

        //Generate query string for Sort
        protected virtual string BuildSortQueryString(SortCollection sorts)
        {
            string queryString = "&sort=";

            if (sorts != null)
            {
                foreach (KeyValuePair<string, string> s in sorts)
                {
                    queryString += $"{ s.Key}{UriKeyValueSeparator}{s.Value}{UriItemSeparator}";
                }

                if (!string.IsNullOrEmpty(UriItemSeparator))
                {
                    queryString = queryString.TrimEnd(UriItemSeparator.ToCharArray());
                }
            }

            return queryString;
        }

        //Generate query string for Pagination
        protected virtual string BuildPageQueryString(int? pageIndex, int? pageSize)
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
        #endregion
    }
}
