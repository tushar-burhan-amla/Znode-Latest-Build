using System;
using System.Collections.Generic;
using System.Diagnostics;
using Newtonsoft.Json;
using Znode.Libraries.Caching.ElasticSearch;
using Znode.Libraries.Framework.Business;

namespace Znode.Libraries.Caching
{
    public class ElasticsearchCacheEventWriter : ICacheEventWriter
    {
        public void WriteEvent(BaseCacheEvent cacheEvent)
        {

            var cacheEvents = new List<BaseCacheEvent>();
            cacheEvents.Add(cacheEvent);
            var wrappedCacheEvents = new List<IWrappedCacheEvent>();

            foreach (var innerCacheEvent in cacheEvents)
            {
                string typeName = innerCacheEvent.GetType().Name;
                try
                {
                    var wrappedCacheEvent = new WrappedCacheEvent();
                    wrappedCacheEvent.CacheEventAsJson = JsonConvert.SerializeObject(innerCacheEvent);
                    wrappedCacheEvent.CacheEventType = typeName;
                    wrappedCacheEvent.CacheEventId = innerCacheEvent.CacheEventId;
                    wrappedCacheEvents.Add(wrappedCacheEvent);
                }
                catch (Exception e)
                {
                    ZnodeLogging.LogMessage($"Failed to serialize cache event of type '{typeName}'", CacheFrameworkConstants.ZnodeLogComponent, TraceLevel.Error, e);
                }
            }

            ElasticSearchHelper.IndexDocuments(CacheFrameworkSettings.GetCacheEventIndexName(), wrappedCacheEvents, typeof(WrappedCacheEvent).Name);
        }
    }
}
