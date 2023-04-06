using System;
using System.Diagnostics;
using System.IO;
using System.Net;

using Znode.Engine.Api.Models;
using Znode.Libraries.Framework.Business;

namespace Znode.Libraries.Hangfire
{
    public class VoucherEmailHelper : BaseScheduler,ISchedulerProviders
    {
        #region MyRegion
        protected override string SchedulerName => "VoucherReminderEmail";
        private string TokenValue = string.Empty;
        #endregion

        public void InvokeMethod(ERPTaskSchedulerModel model)
        {
            if (!string.IsNullOrEmpty(model.ExeParameters))
            {
                var args = model.ExeParameters.Split(',');
                TokenValue = args[4];
                // args[7] contains request timeout value.
                if (args.Length > 7 && !string.IsNullOrEmpty(args[7]))
                    base.RequestTimeout = int.Parse(args[7]);

                CallVoucherEmailAPI(args[1], args[2], args[3]);
            }
        }

        //To call Voucher reminder email API.
        private void CallVoucherEmailAPI( string apiDomainURL, string userId, string token)
        {
            string requestPath = $"{apiDomainURL}/giftcard/sendvoucherexpirationreminderemail";
            string jsonString = string.Empty;
            ZnodeLogging.LogMessage(requestPath, LoggingComponent, TraceLevel.Info);

            try
            {
                try
                {
                    HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(requestPath);
                    request.Method = "GET";
                    request.ContentType = "application/json";
                    request.Headers.Add($"{ UserHeader }: { userId }");
                    request.Headers.Add($"{ AuthorizationHeader }: Basic { token }");

                    if (!string.IsNullOrEmpty(TokenValue) && TokenValue != "0")
                        request.Headers.Add($"{ TokenHeader }: { TokenValue }");

                    request.Timeout = base.RequestTimeout;
                    ZnodeLogging.LogMessage("Email schedular method Called and Set request parameter.", LoggingComponent, TraceLevel.Info);

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
                        CallVoucherEmailAPI(apiDomainURL, userId, token);
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

