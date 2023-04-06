using Newtonsoft.Json;
using System;
using System.IO;
using System.Net;
using System.Text;

namespace Znode.Engine.SearchIndexer
{
    public class Indexer
    {
        int portalId;
        string indexName;
        string logFilePath;
        string domainUrl;

        #region Main
        //args[0] - Portal ID.
        //args[1] - Index Name.
        //args[2] - Portal IndexId.
        //args[3] - create Index Monitor ID
        //args[4] - Is scheduler in use.
        //args[5] - Log file path.
        //args[6] - API Domain URL.
        //args[7] - User ID.
        //args[8] - Server Status ID.
        static void Main(string[] args)
        {
            Indexer indexer = new Indexer();

            int createIndexMonitorId = 0;

            indexer.portalId = Convert.ToInt32(args[0]);
            indexer.indexName = args[1];
            indexer.logFilePath = args[5];
            indexer.domainUrl = args[6];

            // If exe called from scheduler insert data required to create index.
            // else initialize create Index monitor ID from command line arguments.
            if (args[4] == "SchedulerInUse")
            {
                indexer.InsertCreateIndexData(args);
            }
            else
            {
                createIndexMonitorId = Convert.ToInt32(args[3]);
                //Call create index method
                indexer.CreateIndex(indexer.portalId, indexer.indexName, createIndexMonitorId, args[7],args[8]);
                Environment.Exit(0);
            }

            indexer.LogMessage(string.Join("|",args));
            Environment.Exit(0);
        }
        #endregion

        //Insert data required for creating index in database.(Portal index data and CreateIndexMonitor tables)
        //Input required PortalId, index Name.
        //Out param create index monitor ID.
        public void InsertCreateIndexData(string[] args)
        {
            string jsonString = string.Empty;
            string message = string.Empty;
            string requestPath = args[6] + "/search/insertcreateindexdata";
            try
            {
                PortalIndexModel portalIndex = new PortalIndexModel() { PortalId = portalId, IndexName = indexName, PortalIndexId = Convert.ToInt32(args[2]) };
                var dataBytes = Encoding.UTF8.GetBytes(JsonConvert.SerializeObject(portalIndex));
                var request = (HttpWebRequest)WebRequest.Create(requestPath);
                request.KeepAlive = false; // Prevents "server committed a protocol violation" error
                request.Method = "POST";
                request.ContentType = "application/json";
                request.ContentLength = dataBytes.Length;
                // 1 hour 
                request.Timeout = 3600000;
                using (var reqStream = request.GetRequestStream())
                {
                    reqStream.Write(dataBytes, 0, dataBytes.Length);
                }
                var result = GetResultFromResponse<PortalIndexModel>(request);

                //Log message
                LogMessage("Search index created by scheduler successfully.");
            }
            catch (Exception ex)
            {
                LogMessage($"Search index creation failed. Exception: {ex.Message}------API called {requestPath}");
            }
        }


        //Call search library to create Index
        //Input Index name, Portal Id, search index monitor ID.
        public void CreateIndex(int portalId, string indexName, int createIndexMonitorId, string userId,string serverStatusId)
        {
            string jsonString = string.Empty;
            string message = string.Empty;

            string requestPath = domainUrl + $"/search/createindex/{indexName}/{portalId}/{createIndexMonitorId}/{serverStatusId}";
            try
            {
                HttpWebRequest request = (HttpWebRequest)HttpWebRequest.Create(requestPath);
                request.Method = "GET";
                request.ContentType = "application/x-www-form-url-encoded";
                request.Headers.Add($"Znode-UserId: {userId}");
                using (HttpWebResponse response = (HttpWebResponse)request.GetResponse())
                {
                    Stream datastream = response.GetResponseStream();
                    StreamReader reader = new StreamReader(datastream);
                    jsonString = reader.ReadToEnd();
                    reader.Close();
                    datastream.Close();
                    LogMessage($"{DateTime.Now}-- API Call Success.------API called {requestPath} ");
                }
            }
            catch (Exception ex)
            {
                LogMessage($"{DateTime.Now}-- Failed: {ex.Message}");
            }
        }

        private void LogMessage(string message)
        {
            using (StreamWriter outfile = new StreamWriter(logFilePath))
            {
                outfile.Write(message);
            }
        }

        private PortalIndexModel GetResultFromResponse<PortalIndexModel>(HttpWebRequest request)
        {
            PortalIndexModel res;
            try
            {
                using (var rsp = (HttpWebResponse)request.GetResponse())
                {
                    Stream datastream = rsp.GetResponseStream();
                    StreamReader reader = new StreamReader(datastream);
                    string jsonString = reader.ReadToEnd();
                    res = JsonConvert.DeserializeObject<PortalIndexModel>(jsonString);
                }
            }
            catch (WebException ex)
            {
                using (var rsp = (HttpWebResponse)ex.Response)
                {
                    // This deserialization is used to get the error information
                    res = DeserializeResponseStream<PortalIndexModel>(rsp);
                    Console.WriteLine("Failed in get result from response");
                }
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
    }
}