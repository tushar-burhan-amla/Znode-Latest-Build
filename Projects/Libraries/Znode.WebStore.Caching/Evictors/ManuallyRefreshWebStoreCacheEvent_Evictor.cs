using System.Collections.Generic;
using Znode.Libraries.Caching.Events;
using Znode.WebStore.Caching.Core;

namespace Znode.WebStore.Caching.Evictors
{
    internal class ManuallyRefreshWebStoreCacheEvent_Evictor : BaseWebStoreEvictor<ManuallyRefreshWebStoreCacheEvent>
    {
        protected override void Setup(ManuallyRefreshWebStoreCacheEvent cacheEvent)
        {

        }

        protected override void EvictNonDictionaryCacheData(ManuallyRefreshWebStoreCacheEvent cacheEvent)
        {
            ClearHtmlCache();
        }

        protected override List<string> EvictSpecificDictionaryCacheKeys(ManuallyRefreshWebStoreCacheEvent cacheEvent)
        {
            return new List<string>();
        }

        protected override bool IsDictionaryItemStale(ManuallyRefreshWebStoreCacheEvent cacheEvent, string key)
        {
            return true;
        }

        protected override void Teardown(ManuallyRefreshWebStoreCacheEvent cacheEvent)
        {

        }
    }
}