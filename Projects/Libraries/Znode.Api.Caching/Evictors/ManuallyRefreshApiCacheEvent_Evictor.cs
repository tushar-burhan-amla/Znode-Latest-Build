using System.Collections.Generic;
using Znode.Api.Caching.Core;
using Znode.Libraries.Caching.Events;

namespace Znode.Api.Caching.Evictors
{
    internal class ManuallyRefreshApiCacheEvent_Evictor : BaseApiEvictor<ManuallyRefreshApiCacheEvent>
    {
        protected override void Setup(ManuallyRefreshApiCacheEvent cacheEvent)
        {

        }

        protected override void EvictNonDictionaryCacheData(ManuallyRefreshApiCacheEvent cacheEvent)
        {

        }

        protected override List<string> EvictSpecificDictionaryCacheKeys(ManuallyRefreshApiCacheEvent cacheEvent)
        {
            return new List<string>();
        }

        protected override bool IsDictionaryItemStale(ManuallyRefreshApiCacheEvent cacheEvent, string key)
        {
            return true;
        }

        protected override void Teardown(ManuallyRefreshApiCacheEvent cacheEvent)
        {

        }
    }
}
