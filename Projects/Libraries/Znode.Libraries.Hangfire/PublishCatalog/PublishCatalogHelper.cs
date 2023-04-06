using Newtonsoft.Json;

using System;
using System.Diagnostics;
using System.IO;
using System.Net;

using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Libraries.Framework.Business;

namespace Znode.Libraries.Hangfire
{
    public class PublishCatalogHelper : BaseScheduler, ISchedulerProviders
    {
        #region Protected properties and fields
        protected override string SchedulerName => "PublishCatalogHelper";
        private string TokenValue = string.Empty;
        #endregion

        public void InvokeMethod(ERPTaskSchedulerModel model)
        {
            if (!string.IsNullOrEmpty(model.ExeParameters))
            {
                var args = model.ExeParameters.Split(',');
                if (args.Length > 6)
                    TokenValue = args[6];
                // args[7] contains request timeout value.
                if (args.Length > 7 && !string.IsNullOrEmpty(args[7]))
                    base.RequestTimeout = int.Parse(args[7]);

                CallPublishCatalogAPI(Convert.ToInt32(args[1]), args[2], args[3], args[4], "PRODUCTION", args[5]);
            }
        }

        private void CallPublishCatalogAPI(int catalogId, string catalogName, string apiDomainURL, string userId, string revisionType, string token)
        {
            string requestPath = $"{apiDomainURL}/catalog/publish/{catalogId}/{revisionType}";
            ZnodeLogging.LogMessage(requestPath, LoggingComponent, TraceLevel.Info);
            string jsonString = string.Empty;
            string message = string.Empty;
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(requestPath);
                request.Method = "GET";
                request.ContentType = "application/x-www-form-url-encoded";

                request.Headers.Add($"{ UserHeader }: { userId }");
                request.Headers.Add($"{ AuthorizationHeader }: Basic { token }");
                if (!string.IsNullOrEmpty(TokenValue) && TokenValue != "0")
                    request.Headers.Add($"{ TokenHeader }: { TokenValue }");

                request.Timeout = base.RequestTimeout;
                ZnodeLogging.LogMessage("SyncCallApi method Called and Set request parameter.", LoggingComponent, TraceLevel.Info);
                using (HttpWebResponse responce = (HttpWebResponse)request.GetResponse())
                {
                    Stream datastream = responce.GetResponseStream();
                    ZnodeLogging.LogMessage("Got Response Stream.", LoggingComponent, TraceLevel.Info);
                    StreamReader reader = new StreamReader(datastream);
                    ZnodeLogging.LogMessage("read Response Stream.", LoggingComponent, TraceLevel.Info);
                    jsonString = reader.ReadToEnd();
                    reader.Close();
                    datastream.Close();
                    PublishedResponse result = new PublishedResponse();
                    result = JsonConvert.DeserializeObject<PublishedResponse>(jsonString);
                    PublishedModel model = result.PublishedModel;
                    ZnodeLogging.LogMessage("API Call Successfully.", LoggingComponent, TraceLevel.Info);
                    //Call Send Scheduler Activity Log Method with "No Error" and scheduler status as true
                    ZnodeLogging.LogMessage("Sending Scheduler Activity Log.", LoggingComponent, TraceLevel.Info);
                    if (model.IsPublished)
                        ZnodeLogging.LogMessage($"Scheduler for catalog {catalogName} started successfully at {DateTime.Now}", LoggingComponent, TraceLevel.Info);
                    else
                        ZnodeLogging.LogMessage($"Failed to publish catalog {catalogName} because { model.ErrorMessage}", LoggingComponent, TraceLevel.Error);
                }
            }
            catch (WebException webException)
            {
                if (CheckTokenIsInvalid(webException))
                {
                    TokenValue = GetToken(apiDomainURL, token);
                    CallPublishCatalogAPI(catalogId, catalogName, apiDomainURL, userId, revisionType, token);
                }
                else
                    ZnodeLogging.LogMessage(webException, LoggingComponent, TraceLevel.Error);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, LoggingComponent, TraceLevel.Error);
            }
        }

    }
}
