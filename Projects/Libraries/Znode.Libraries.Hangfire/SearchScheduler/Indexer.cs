using Newtonsoft.Json;

using System;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;

using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Libraries.Hangfire
{
    public class Indexer : BaseScheduler, ISchedulerProviders
    {
        #region Private Variables
        private int catalogId;
        private string indexName;
        private string domainUrl;
        protected override string SchedulerName => "SearchScheduler";
        #endregion

        #region Public 

        public void InvokeMethod(ERPTaskSchedulerModel model)
        {
            if (!string.IsNullOrEmpty(model.ExeParameters))
            {
                    var args = model.ExeParameters.Split(',');
                    Indexer indexer = new Indexer
                    {
                        catalogId = Convert.ToInt32(args[1]),
                        indexName = args[2],
                        domainUrl = args[6]
                    };
                    if (!string.IsNullOrEmpty(args[args.Length-1]))
                        base.RequestTimeout = int.Parse(args[args.Length-1]);

                    //Generate the new token value as previous same token is used during the publish process. 
                    //So there might chance of token timeout elapsed issue if uses same token.
                    string newTokenValue = GetNewTokenValue(args);
                    ZnodeLogging.LogMessage($"The execution has been started with Arguments : {string.Join("|", args)} and Request Timeout : {base.RequestTimeout}", LoggingComponent, TraceLevel.Info);

                    // If exe called from scheduler insert data required to create index.
                    // else initialize create Index monitor ID from command line arguments.
                    if (args[5] == ZnodeConstant.SchedulerInUse)
                    {
                        if (!string.IsNullOrEmpty(newTokenValue))
                            args[11] = newTokenValue;

                        indexer.InsertCreateIndexData(args);
                    }
                    else
                    {
                        int createIndexMonitorId = Convert.ToInt32(args[4]);
                        //Call create index method                
                        indexer.CreateIndex(indexer.catalogId, indexer.indexName, args[9],Convert.ToBoolean(args[10]), Convert.ToBoolean(args[11]),args[12], createIndexMonitorId, args[7], args[8], args[13], newTokenValue, base.RequestTimeout);
                    }

                ZnodeLogging.LogMessage(string.Join("|", args), LoggingComponent, TraceLevel.Info);
            }
        }
        #endregion

        #region Private Methods

        //Insert data required for creating index in database.(Portal index data and CreateIndexMonitor tables)
        //Input required PortalId, index Name.
        //Out param create index monitor ID.
        private void InsertCreateIndexData(string[] args)
        {
            string jsonString = string.Empty;
            string message = string.Empty;
            string requestPath = args[6] + "/search/insertcreateindexdata";
            try
            {
                PortalIndexModel portalIndex = new PortalIndexModel() { PublishCatalogId = catalogId, IndexName = indexName, CatalogIndexId = Convert.ToInt32(args[3]) };
                var dataBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(portalIndex));
                var request = (HttpWebRequest)WebRequest.Create(requestPath);
                request.KeepAlive = false; // Prevents "server committed a protocol violation" error
                request.Method = "POST";
                request.ContentType = "application/json";
                request.ContentLength = dataBytes.Length;
                request.Headers.Add($"{UserHeader}: {args[7]}");
                request.Headers.Add($"{AuthorizationHeader}: {"Basic " + args[10]}");
                // args[11] contains token value.
                if ((args.Length > 11) && !string.IsNullOrEmpty(args[11]) && args[11] != "0")
                    request.Headers.Add($"{TokenHeader}: {args[11]}");

                if ((args.Length > 12) && !string.IsNullOrEmpty(args[12]))
                    base.RequestTimeout = int.Parse(args[12]);

                request.Timeout = base.RequestTimeout;
                using (var reqStream = request.GetRequestStream())
                {
                    reqStream.Write(dataBytes, 0, dataBytes.Length);
                }
                var result = GetResultFromResponse<PortalIndexModel>(request);

                //Log message
                ZnodeLogging.LogMessage("Search index created by scheduler successfully.", LoggingComponent, TraceLevel.Info);
            }
            catch (WebException webException)
            {
                ZnodeLogging.LogMessage(webException, LoggingComponent, TraceLevel.Error);

                if (CheckTokenIsInvalid(webException))
                {
                    ZnodeLogging.LogMessage(webException, LoggingComponent, TraceLevel.Error);
                    // args[10] contains authorization header value.
                    // args[6] contains domain url.
                    args[11] = GetToken(args[6], args[10]);
                    InsertCreateIndexData(args);
                }
                else
                    ZnodeLogging.LogMessage(webException, LoggingComponent, TraceLevel.Error);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, LoggingComponent, TraceLevel.Error);
            }
        }

        //Call search library to create Index
        //Input Index name, Portal Id, search index monitor ID.
        private void CreateIndex(int portalId, string indexName, string revisionType,bool isPreviewProductionEnabled, bool isPublishDraftProductsOnly,string newIndexName, int createIndexMonitorId, string userId, string serverStatusId, string headerAuthorization, string TokenValue = "", int requesttimeout = 600000)
        {
            string jsonString = string.Empty;
            string message = string.Empty;

            string requestPath = string.IsNullOrEmpty(newIndexName) ? domainUrl + $"/search/createindex/{indexName}/{revisionType}/{isPreviewProductionEnabled}/{isPublishDraftProductsOnly}/{portalId}/{createIndexMonitorId}/{serverStatusId}" 
                : domainUrl + $"/search/createindex/{indexName}/{revisionType}/{isPreviewProductionEnabled}/{isPublishDraftProductsOnly}/{portalId}/{createIndexMonitorId}/{serverStatusId}/{newIndexName}";
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(requestPath);
                request.Method = "GET";
                request.ContentType = "application/x-www-form-url-encoded";
                request.Headers.Add($"{UserHeader}: {userId}");
                request.Headers.Add($"{AuthorizationHeader}: {"Basic " + headerAuthorization}");
                if (!string.IsNullOrEmpty(TokenValue) && TokenValue != "0")
                    request.Headers.Add($"{TokenHeader}: {TokenValue}");

                request.Timeout = requesttimeout;
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream datastream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(datastream);
                    jsonString = reader.ReadToEnd();
                    reader.Close();
                    datastream.Close();
                    ZnodeLogging.LogMessage($"{DateTime.Now}-- API Call Success.------API called {requestPath} ", LoggingComponent, TraceLevel.Info);
                }
            }
            catch (WebException webException)
            {
                ZnodeLogging.LogMessage(webException, LoggingComponent, TraceLevel.Error);

                if (CheckTokenIsInvalid(webException))
                {
                    ZnodeLogging.LogMessage(webException, LoggingComponent, TraceLevel.Error);
                    TokenValue = GetToken(domainUrl, headerAuthorization);
                    CreateIndex(portalId, indexName, revisionType, isPreviewProductionEnabled,isPublishDraftProductsOnly, newIndexName, createIndexMonitorId, userId, serverStatusId, headerAuthorization, TokenValue, requesttimeout);
                }
                else
                {
                    PortalIndexModel res;
                    using (var rsp = (HttpWebResponse)webException.Response)
                    {
                        // This deserialization is used to get the error information
                        res = DeserializeResponseStream<PortalIndexModel>(rsp);
                        ZnodeLogging.LogMessage(webException, LoggingComponent, TraceLevel.Error);
                    }
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, LoggingComponent, TraceLevel.Error);
            }
        }


        private PortalIndexModel GetResultFromResponse<PortalIndexModel>(HttpWebRequest request)
        {
            PortalIndexModel res;
            using (var rsp = (HttpWebResponse)request.GetResponse())
            {
                Stream datastream = rsp.GetResponseStream();
                StreamReader reader = new StreamReader(datastream);
                string jsonString = reader.ReadToEnd();
                res = JsonConvert.DeserializeObject<PortalIndexModel>(jsonString);
            }

            return res;
        }

        private PortalIndexModel DeserializeResponseStream<PortalIndexModel>(WebResponse response)
        {
            if (response != null)
            {
                using (var body = response.GetResponseStream())
                {
                    if (body != null)
                    {
                        using (var stream = new StreamReader(body))
                        {
                            using (var jsonReader = new JsonTextReader(stream))
                            {
                                var jsonSerializer = new JsonSerializer();
                                try
                                {
                                    return jsonSerializer.Deserialize<PortalIndexModel>(jsonReader);
                                }
                                catch (JsonReaderException ex)
                                {
                                    throw new Exception(ex.Message);
                                }
                            }
                        }
                    }
                }
            }
            return default(PortalIndexModel);
        }

        //Generate the new token value as previous same token is used during the publish process. 
        //So there might chance of token timeout elapsed issue if uses same token.
        private string GetNewTokenValue(string[] args)
        {
            ZnodeLogging.LogMessage("Token Generation Process Started", LoggingComponent, TraceLevel.Info);

            return (args.Length > 11 && !string.IsNullOrEmpty(args[11]) && args[11] != "0") ? GetToken(args[6], args[10]) : "";
        }

        #endregion
    }
}
