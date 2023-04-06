using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

using Nest;
using Znode.Libraries.ElasticSearch;
using Znode.Libraries.Framework.Business;

namespace Znode.Libraries.Caching.ElasticSearch
{
    public static class ElasticSearchHelper
    {
        public static bool CreateIndexIfNotExists(string indexName)
        {
            var client = ElasticConnectionHelper.ElasticSearchClient;

            if (client.Indices.Exists(new IndexExistsRequest(Indices.Index(indexName))).Exists)
            {
                ZnodeLogging.LogMessage($"Determined that cache event index '{indexName}' already exists, not creating it.", "CACHE", TraceLevel.Info);
            }
            else
            {
                ZnodeLogging.LogMessage($"Determined that cache event index '{indexName}' does not exist, about to create it.", "CACHE", TraceLevel.Info);
                var response = client.Indices.Create(indexName);

                var isSuccessful = response.IsValid;
                if (isSuccessful)
                {
                    ZnodeLogging.LogMessage($"Index '{indexName}' successfully created.", "CACHE", TraceLevel.Info);
                }
                else
                {
                    ZnodeLogging.LogMessage($"Index '{indexName}' failed to create.", "CACHE", TraceLevel.Error);
                    return false;
                }
            }

            return true;
        }

        public static bool IndexDocuments(string indexName, IEnumerable<object> objects, string type)
        {
            var client = ElasticConnectionHelper.ElasticSearchClient;

            var bulkIndexResponse = client.Bulk(b => b
                .Index(indexName)
                .IndexMany(objects)
            );

            var isSuccessful = bulkIndexResponse.IsValid;
            if (!isSuccessful)
            {
                ZnodeLogging.LogMessage($"Failed to index objects to index '{indexName}'.", "CACHE", TraceLevel.Error);
            }
            else
            {
                ZnodeLogging.LogMessage($"Successfully indexed objects to index '{indexName}'.", "CACHE", TraceLevel.Info);
            }

            return isSuccessful;
        }

        public static List<T> QueryDocuments<T>(string indexName, DateTime minCreatedDateTime, DateTime maxCreatedDateTime) where T : Document
        {
            var client = ElasticConnectionHelper.ElasticSearchClient;

            var queryResponse = client
                .Search<T>(s => s.Index(indexName)
                .From(0)
                .Size(100)  // Note that if more than 100 cache events have been created since the last poll operation
                            // we will fail to read and process all of them. This should be more than enough, but if it
                            // ever would become a problem on a very high traffic site we need to add support to handle it.
                .Query(q =>
                    q.DateRange(r => r.Field(f => f.CreatedDateTime).GreaterThanOrEquals(minCreatedDateTime)) &&
                    q.DateRange(r => r.Field(f => f.CreatedDateTime).LessThanOrEquals(maxCreatedDateTime))));

            var isEventMissed = queryResponse.Total > queryResponse.Hits.Count;
            if (isEventMissed)
            {
                ZnodeLogging.LogMessage($"There were {queryResponse.Total} cache events " +
                                        $"available but only {queryResponse.Hits} will be handled.", 
                                        CacheFrameworkConstants.ZnodeLogComponent, TraceLevel.Error);
            }
            
            return queryResponse.Hits.Select(h => h.Source).ToList();
        }
    }
}
