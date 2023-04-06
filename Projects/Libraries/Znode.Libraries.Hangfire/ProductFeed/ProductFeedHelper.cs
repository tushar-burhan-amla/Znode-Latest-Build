using Newtonsoft.Json;

using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;

using Znode.Engine.Api.Models;
using Znode.Libraries.Framework.Business;

namespace Znode.Libraries.Hangfire
{
    public class ProductFeedHelper : BaseScheduler, ISchedulerProviders
    {
        #region Protected properties and fields
        protected override string SchedulerName => "ProductFeedScheduler";
        protected string TokenBasedAuthToken = string.Empty;
        #endregion

        public void InvokeMethod(ERPTaskSchedulerModel model)
        {
            try
            {
                if (!string.IsNullOrEmpty(model.ExeParameters))
                {
                    string[] args = model.ExeParameters.Split(',');
                    ProductFeedModel objmodel = GetModelFromArguments(args[1]);
                    if (!Equals(model, null))
                    {
                        CallProductFeedAPI(objmodel);
                    }
                }
            }          
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage($"Error in invoking Hangfire job for product feed", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Warning);
            }
        }

        private void CallProductFeedAPI(ProductFeedModel model)
        {
            string data = JsonConvert.SerializeObject(model);
            byte[] dataBytes = Encoding.UTF8.GetBytes(data);
            string jsonString = string.Empty;
            string message = string.Empty;
            string requestPath = $"{model.SuccessXMLGenerationMessage}/productfeed";
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(requestPath);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.Timeout = base.RequestTimeout;
                request.ContentLength = dataBytes.Length;
                request.Headers.Add($"{UserHeader}: {model.UserId}");
                request.Headers.Add($"{AuthorizationHeader}: {"Basic " + model.Token}");
                if (!string.IsNullOrEmpty(TokenBasedAuthToken) && TokenBasedAuthToken != "0")
                    request.Headers.Add($"{TokenHeader}: {TokenBasedAuthToken}");
                using (var reqStream = request.GetRequestStream())
                {
                    reqStream.Write(dataBytes, 0, dataBytes.Length);
                }
                using (HttpWebResponse responce = (HttpWebResponse)request.GetResponse())
                {
                    Stream datastream = responce.GetResponseStream();
                    StreamReader reader = new StreamReader(datastream);
                    jsonString = reader.ReadToEnd();
                    reader.Close();
                    datastream.Close();
                    ZnodeLogging.LogMessage($"{SchedulerName} API Call Successfully.", LoggingComponent, TraceLevel.Info);
                    ZnodeLogging.LogMessage($"Product feed data {jsonString}", LoggingComponent, TraceLevel.Info);
                }
            }
            catch (WebException webException)
            {
                if (CheckTokenIsInvalid(webException))
                {
                    TokenBasedAuthToken = GetToken(model.SuccessXMLGenerationMessage, model.Token);
                    CallProductFeedAPI(model);
                }
                else
                    ZnodeLogging.LogMessage(webException, LoggingComponent, TraceLevel.Error);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, LoggingComponent, TraceLevel.Error);
            }
        }

        private ProductFeedModel GetModelFromArguments(string arguments)
        {            
            if (string.IsNullOrEmpty(arguments))
                return null;

            ProductFeedModel model = new ProductFeedModel();
            string[] argsArray = arguments.Split('#');

            model.ProductFeedId = int.Parse(argsArray[0]);
            model.LocaleId = int.Parse(argsArray[1]);
            model.ProductFeedSiteMapTypeCode = Convert.ToString(argsArray[4]);
            model.ProductFeedTypeCode = Convert.ToString(argsArray[5]);
            model.Title = Convert.ToString(argsArray[6]);
            model.Link = Convert.ToString(argsArray[7]);
            model.Description = Convert.ToString(argsArray[8]);
            model.FileName = Convert.ToString(argsArray[9]);
            model.PortalId = int.Parse(argsArray[10]);
            model.SuccessXMLGenerationMessage = Convert.ToString(argsArray[11]);
            model.IsFromScheduler = true;
            model.UserId = int.Parse(argsArray[13]);
            model.Token = argsArray[14];
            if (argsArray.Length > 15)
                TokenBasedAuthToken = argsArray[15];
            // args[16] contains request timeout value.
            if (argsArray.Length > 16 && !string.IsNullOrEmpty(argsArray[16]))
                base.RequestTimeout = int.Parse(argsArray[16]);
            return model;
        }
    }
}
