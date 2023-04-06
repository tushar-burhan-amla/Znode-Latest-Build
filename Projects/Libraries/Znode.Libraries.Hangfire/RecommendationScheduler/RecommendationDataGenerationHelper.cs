using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Libraries.Framework.Business;

namespace Znode.Libraries.Hangfire
{
    public class RecommendationDataGenerationHelper : BaseScheduler, ISchedulerProviders
    {
        #region Protected properties and fields
        protected override string SchedulerName => "RecommendationDataGenerationHelper";
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

                bool.TryParse(args[2], out bool isBuildPartial);

                CallRecommendationDataGenerationAPI(Convert.ToInt32(args[1]), isBuildPartial, args[3], args[4], args[5]);
            }
        }

        //To call recommendation data generation API.
        private void CallRecommendationDataGenerationAPI(int portalId, bool isBuildPartial, string apiDomainURL, string userId, string token)
        {
            //Request model.
            RecommendationDataGenerationRequestModel recommendationRequest = new RecommendationDataGenerationRequestModel() { PortalId = portalId, IsBuildPartial = isBuildPartial };
            var dataBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(recommendationRequest));
            string requestPath = $"{apiDomainURL}/recommendation/GenerateRecommendationData";            
            string jsonString = string.Empty;
            string message = string.Empty;
            ZnodeLogging.LogMessage(requestPath, LoggingComponent, TraceLevel.Info);

            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(requestPath);
                request.Method = "POST";
                request.ContentType = "application/json";
                request.ContentLength = dataBytes.Length;
                request.Headers.Add($"{ UserHeader }: { userId }");
                request.Headers.Add($"{ AuthorizationHeader }: Basic { token }");

                if (!string.IsNullOrEmpty(TokenValue) && TokenValue != "0")
                    request.Headers.Add($"{ TokenHeader }: { TokenValue }");

                request.Timeout = base.RequestTimeout;

                using (var reqStream = request.GetRequestStream())
                {
                    reqStream.Write(dataBytes, 0, dataBytes.Length);
                }

                ZnodeLogging.LogMessage("Recommendation data generation API will be called.", LoggingComponent, TraceLevel.Info);
                using (HttpWebResponse responce = (HttpWebResponse)request.GetResponse())
                {
                    Stream datastream = responce.GetResponseStream();
                    ZnodeLogging.LogMessage("Got Response Stream.", LoggingComponent, TraceLevel.Info);
                    StreamReader reader = new StreamReader(datastream);
                    ZnodeLogging.LogMessage("read Response Stream.", LoggingComponent, TraceLevel.Info);
                    jsonString = reader.ReadToEnd();
                    reader.Close();
                    datastream.Close();

                    RecommendationDataGenerationResponse result = new RecommendationDataGenerationResponse();
                    result = JsonConvert.DeserializeObject<RecommendationDataGenerationResponse>(jsonString);
                    RecommendationGeneratedDataModel model = result?.recommendationGeneratedData;
                    ZnodeLogging.LogMessage("Recommendation API called successfully.", LoggingComponent, TraceLevel.Info);

                    //Scheduler activity log.
                    if (model.IsDataGenerationStarted)
                        ZnodeLogging.LogMessage($"Scheduler for recommendation data generation for portal Id {portalId} started successfully at {DateTime.Now} ", LoggingComponent, TraceLevel.Info);
                    else
                        ZnodeLogging.LogMessage($"Failed to create recommendation data for portal Id {portalId}", LoggingComponent, TraceLevel.Error);
                }
            }
            catch (WebException webException)
            {
                if (CheckTokenIsInvalid(webException))
                {
                    TokenValue = GetToken(apiDomainURL, token);
                    CallRecommendationDataGenerationAPI(portalId, isBuildPartial, apiDomainURL, userId, token);
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
