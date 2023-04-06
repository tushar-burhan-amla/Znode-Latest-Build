using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;

using Znode.Engine.Api.Models;
using Znode.Libraries.Framework.Business;

namespace Znode.Libraries.Hangfire
{
    public class ERPTaskScheduler : BaseScheduler, ISchedulerProviders
    {
        #region Public Methods
        protected override string SchedulerName { get => "ERPTaskScheduler"; }

        public void InvokeMethod(ERPTaskSchedulerModel model)
        {
            try
            {
                if (string.IsNullOrEmpty(model.ExeParameters))
                    return;

                var args = model.ExeParameters.Split(',');
                ZnodeLogging.LogMessage($"Start the execution for {args[4]} Scheduler.", LoggingComponent, TraceLevel.Info);

                // args[8] contains request timeout value.
                if (args.Length > 8 && !string.IsNullOrEmpty(args[8]))
                {
                    base.RequestTimeout = int.Parse(args[8]);
                }

                ZnodeLogging.LogMessage("SyncCallApi method Calling.", LoggingComponent, TraceLevel.Info);
                SyncCallApi(args);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, LoggingComponent, TraceLevel.Error);
            }
        }
        #endregion

        #region Private Methods

        /// <summary>
        /// Connect to ERPTaskScheduler API 
        /// </summary>
        /// <param name="args">Parameter with task scheduler</param>
        /// <returns>Return responce by API controller</returns>
        private string SyncCallApi(string[] arguments)
        {
            string jsonString = string.Empty;
            string requestPath = string.Concat(arguments[3], "/ERPTaskScheduler/TriggerSchedulerTask/", arguments[1]);
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(requestPath);
                request.Method = "GET";
                request.ContentType = "application/x-www-form-url-encoded";

                SetRequestHeaders(arguments, request);

                ZnodeLogging.LogMessage("SyncCallApi method Called and Set request parameter.", LoggingComponent, TraceLevel.Info);
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream datastream = response.GetResponseStream();

                    ZnodeLogging.LogMessage($"{SchedulerName}: Got Response Stream.", LoggingComponent, TraceLevel.Info);
                    StreamReader reader = new StreamReader(datastream);
                    ZnodeLogging.LogMessage($"{SchedulerName}: Read Response Stream.", LoggingComponent, TraceLevel.Info);

                    jsonString = reader.ReadToEnd();
                    reader.Close();
                    datastream.Close();

                    ZnodeLogging.LogMessage($"{SchedulerName}: API Called Successfully.", LoggingComponent, TraceLevel.Info);

                    //Call Send Scheduler Activity Log Method with "No Error" and scheduler status as true
                    ZnodeLogging.LogMessage($"{SchedulerName}: Sending Scheduler Activity Log.", LoggingComponent, TraceLevel.Info);
                    SendSchedulerActivityLog("No Error", true, arguments[3], arguments[2], arguments[1], arguments[6], (arguments.Length > 7) ? arguments[7] : "");
                }
            }
            catch (WebException webException)
            {
                if (CheckTokenIsInvalid(webException))
                {
                    // args[6] contains authorization header value.
                    // args[3] contains domain url.
                    arguments[7] = GetToken(arguments[3], arguments[6]);
                    SyncCallApi(arguments);
                }
                else
                {
                    //Call Send Scheduler Activity Log Method with "Error" and scheduler status as false
                    SendSchedulerActivityLog(webException.Message, false, arguments[3], arguments[2], arguments[1], arguments[6], (arguments.Length > 7) ? arguments[7] : "");
                    ZnodeLogging.LogMessage(webException, LoggingComponent, TraceLevel.Error);
                }
            }
            catch (Exception ex)
            {
                //Call Send Scheduler Activity Log Method with "Error" and scheduler status as false
                SendSchedulerActivityLog(ex.Message, false, arguments[3], arguments[2], arguments[1], arguments[6], (arguments.Length > 7) ? arguments[7] : "");
                ZnodeLogging.LogMessage(ex, LoggingComponent, TraceLevel.Error);
            }
            return jsonString;
        }

        private void SetRequestHeaders(string[] arguments, HttpWebRequest request)
        {
            //If logged-in user id received in argument (It's 6th argument), It'll be used to invoke the call.
            if (arguments.Length > 5)
                request.Headers.Add($"{ UserHeader }: { arguments[5] }");

            if (arguments.Length > 6)
                request.Headers.Add($"{ AuthorizationHeader }: Basic { arguments[6] }");

            if (arguments.Length > 7 && !string.IsNullOrEmpty(arguments[7]) && arguments[7] != "0")
                request.Headers.Add($"{TokenHeader}: {arguments[7]}");

            if (arguments.Length > 8 && !string.IsNullOrEmpty(arguments[8]))
                base.RequestTimeout = int.Parse(arguments[8]);

            request.Timeout = base.RequestTimeout;
        }

        private bool SendSchedulerActivityLog(string errorMessage, bool schedulerStatus, string domailUrl, string portalId, string eRPTaskSchedulerId, string AuthorizationHeaderValue = "", string TokenBasedAuthTokenValue = "")
        {
            //Create static json string
            string data = string.Concat("{", '"', "ErrorMessage", '"', ":", '"', errorMessage, '"', ',', '"', "SchedulerStatus", '"', ":", '"', schedulerStatus, '"', ',', '"', "PortalId", '"', ":", '"', portalId, '"', ',', '"', "ERPTaskSchedulerId", '"', ":", '"', eRPTaskSchedulerId, '"', "}");
            //Create endpoint
            string endpoint = string.Concat(domailUrl, "/TouchPointConfiguration/SendSchedulerActivityLog");
            return PostResourceToEndpoint(endpoint, data, domailUrl, AuthorizationHeaderValue, TokenBasedAuthTokenValue);
        }

        private bool PostResourceToEndpoint(string endpoint, string data, string domailUrl = "", string AuthorizationHeaderValue = "", string TokenBasedAuthTokenValue = "")
        {
            ZnodeLogging.LogMessage("Posting Resource To Endpoint - " + endpoint, LoggingComponent, TraceLevel.Info);
            var dataBytes = Encoding.UTF8.GetBytes(data);
            var req = (HttpWebRequest)WebRequest.Create(endpoint);
            req.KeepAlive = false; // Prevents "server committed a protocol violation" error
            req.Method = "POST";
            req.ContentType = "application/json";
            req.ContentLength = dataBytes.Length;
            req.Headers.Add($"Authorization: {"Basic " + AuthorizationHeaderValue}");
            if (!string.IsNullOrEmpty(TokenBasedAuthTokenValue))
                req.Headers.Add($"Token: {TokenBasedAuthTokenValue}");

            using (var reqStream = req.GetRequestStream())
            {
                reqStream.Write(dataBytes, 0, dataBytes.Length);
            }
            ZnodeLogging.LogMessage("Read request stream for Email.", LoggingComponent, TraceLevel.Info);

            try
            {
                if (((HttpWebResponse)req.GetResponse()).StatusCode == HttpStatusCode.OK)
                {
                    ZnodeLogging.LogMessage("Email sent successfully.", LoggingComponent, TraceLevel.Info);
                    return true;
                }
                else
                {
                    ZnodeLogging.LogMessage("Failed to send email.", LoggingComponent, TraceLevel.Error);
                    return false;
                }
            }
            catch (WebException webException)
            {
                if (CheckTokenIsInvalid(webException))
                {
                    TokenBasedAuthTokenValue = GetToken(domailUrl, AuthorizationHeaderValue);
                    PostResourceToEndpoint(endpoint, data, domailUrl, AuthorizationHeaderValue, TokenBasedAuthTokenValue);
                }
                else
                    ZnodeLogging.LogMessage("Failed to send Email.", LoggingComponent, TraceLevel.Error);

                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, LoggingComponent, TraceLevel.Error);
                return false;
            }
        }

        #endregion
    }
}
