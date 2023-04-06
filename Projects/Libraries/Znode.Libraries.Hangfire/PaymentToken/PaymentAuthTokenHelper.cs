using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using Znode.Engine.Api.Models;
using Znode.Libraries.Framework.Business;

namespace Znode.Libraries.Hangfire.PaymentToken
{
    public class PaymentAuthTokenHelper : BaseScheduler, ISchedulerProviders
    {
        #region Schedular parameters
        protected override string SchedulerName => "PaymentAuthTokenHelper";
        private string TokenValue = string.Empty;
        #endregion

        public void InvokeMethod(ERPTaskSchedulerModel model)
        {
            if (!string.IsNullOrEmpty(model?.ExeParameters))
            {
                var args = model.ExeParameters.Split(',');
                TokenValue = args[4];
                try
                {
                    // Request timeout is contained in args[8].
                    if (args.Length > 7 && !string.IsNullOrEmpty(args[8]))
                        base.RequestTimeout = int.Parse(args[8]);
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, LoggingComponent, TraceLevel.Warning);
                }

                CallClearPaymentTokenAPI(args[1], args[3]);
            }
        }

        //To call delete expired payment token API.
        private void CallClearPaymentTokenAPI(string apiDomainURL, string token)
        {
            string requestPath = $"{apiDomainURL}/payment/deletepaymenttoken";
            string jsonString = string.Empty;
            ZnodeLogging.LogMessage(requestPath, LoggingComponent, TraceLevel.Info);

            try
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(requestPath);
                    request.Method = "DELETE";
                    request.ContentType = "application/json";
                    request.Headers.Add($"{ AuthorizationHeader }: Basic { token }");

                    if (!string.IsNullOrEmpty(TokenValue) && TokenValue != "0")
                        request.Headers.Add($"{ TokenHeader }: { TokenValue }");

                    request.Timeout = base.RequestTimeout;
                    ZnodeLogging.LogMessage("Clear payment token schedular method Called and Set request parameter.", LoggingComponent, TraceLevel.Info);

                    using (HttpWebResponse responce = (HttpWebResponse)request.GetResponse())
                    {
                        Stream datastream = responce.GetResponseStream();
                        ZnodeLogging.LogMessage("Got Response Stream.", LoggingComponent, TraceLevel.Info);
                        StreamReader reader = new StreamReader(datastream);
                        ZnodeLogging.LogMessage("read Response Stream.", LoggingComponent, TraceLevel.Info);
                        jsonString = reader.ReadToEnd();
                        reader.Close();
                        datastream.Close();
                        ZnodeLogging.LogMessage("API Call Successfully.", LoggingComponent, TraceLevel.Info);
                        //Call Send Scheduler Activity Log Method with "No Error" and scheduler status as true
                        ZnodeLogging.LogMessage("Sending Scheduler Activity Log.", LoggingComponent, TraceLevel.Info);

                    }
                }
                catch (WebException webException)
                {
                    if (CheckTokenIsInvalid(webException))
                    {
                        TokenValue = GetToken(apiDomainURL, token);
                        CallClearPaymentTokenAPI(apiDomainURL, token);
                    }
                    else
                        ZnodeLogging.LogMessage(webException, LoggingComponent, TraceLevel.Error);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, LoggingComponent, TraceLevel.Error);
            }
        }
    }
}

