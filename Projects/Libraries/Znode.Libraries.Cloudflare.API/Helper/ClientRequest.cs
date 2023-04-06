using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using Znode.Libraries.Framework.Business;

namespace Znode.Libraries.Cloudflare.API
{
    public class ClientRequest
    {
        public int RequestTimeout { get; set; } = 10000000;

        #region Public Methods
        /// <summary>
        /// Post resource data to an destinationURL.
        /// </summary>
        /// <typeparam name="T">The type of resource being created.</typeparam>
        /// <returns>The newly created resource.</returns>
        public T PostRequest<T, T1>(string destinationUrl, T1 data)
        {
            T result;
            try
            {
                result = WebResponse<T, T1>(destinationUrl, data, "POST");
                return result;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Znode.Libraries.Cloudflare.API.CloudflareHelper -> PostRequest error " + ex, "Cloudflare", TraceLevel.Error);
                throw new ZnodeCloudflareException(null, ex.Message);
            }
        }

        /// <summary>
        /// Put resource data to an destinationURL.
        /// </summary>
        /// <typeparam name="T">The type of resource being created.</typeparam>
        /// <param name="requestModel">request model to pass parameter to web request</param>
        /// <returns>The newly created resource.</returns>
        public T PutRequest<T, T1>(string destinationUrl, T1 data)
        {
            T result;
            try
            {
                result = WebResponse<T, T1>(destinationUrl, data, "PUT");
                return result;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Znode.Libraries.Cloudflare.API.CloudflareHelper -> PostRequest error " + ex, "Cloudflare", TraceLevel.Error);
                throw new ZnodeCloudflareException(null, ex.Message);
            }
        }

        /// <summary>
        /// Get resource data to an destinationURL.
        /// </summary>
        /// <typeparam name="T">The type of resource being created.</typeparam>
        /// <returns>The newly created resource.</returns>
        public T GetRequest<T, T1>(string destinationUrl, T1 data)
        {
            T result;
            try
            {
                result = WebResponse<T, T1>(destinationUrl, data, "GET");
                return result;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Znode.Libraries.Cloudflare.API.CloudflareHelper -> PostRequest error " + ex, "Cloudflare", TraceLevel.Error);
                throw new ZnodeCloudflareException(null, ex.Message);
            }
        }

        /// <summary>
        /// Delete resource data to an destinationURL.
        /// </summary>
        /// <typeparam name="T">The type of resource being created.</typeparam>
        /// <param name="requestModel">request model to pass parameter to web request</param>
        /// <returns>The newly created resource.</returns>
        public T DeleteRequest<T, T1>(string destinationUrl, T1 data)
        {
            T result;
            try
            {
                result = WebResponse<T, T1>(destinationUrl, data, "DELETE");
                return result;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Znode.Libraries.Cloudflare.API.CloudflareHelper -> PostRequest error " + ex, "Cloudflare", TraceLevel.Error);
                throw new ZnodeCloudflareException(null, ex.Message);
            }
        }
        #endregion

        #region Private methods
        private T WebResponse<T, T1>(string destinationUrl, T1 data, string requestType)
        {
            string serializeData = JsonConvert.SerializeObject(data);
            byte[] dataBytes = Encoding.UTF8.GetBytes(serializeData);
            HttpWebRequest req = (HttpWebRequest)WebRequest.Create(destinationUrl);
            req.KeepAlive = false; // Prevents "server committed a protocol violation" error
            req.Method = requestType;
            req.ContentType = "application/json";
            req.ContentLength = dataBytes.Length;
            req.Timeout = RequestTimeout;
            req.Headers.Add("X-Auth-Email", ZnodeCloudflareSetting.CloudflareEmailAccount);
            req.Headers.Add("X-Auth-Key", ZnodeCloudflareSetting.CloudflareApiKey);
            Stream requestStream = req.GetRequestStream();
            requestStream.Write(dataBytes, 0, dataBytes.Length);
            req.UserAgent = HttpContext.Current.Request.UserAgent;
            using (HttpWebResponse rsp = (HttpWebResponse)req.GetResponse())
            {
                if (rsp.StatusCode == HttpStatusCode.OK)
                {
                    using (System.IO.StreamReader sr = new System.IO.StreamReader(rsp.GetResponseStream()))
                    {
                        return JsonConvert.DeserializeObject<T>(sr.ReadToEnd());
                    }
                }

            }
            return default(T);
        }
        #endregion
    }
}
