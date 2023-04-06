using System.Collections.Generic;
using Znode.Libraries.Caching.Events;
using Znode.WebStore.Caching.Core;

namespace Znode.WebStore.Caching.Evictors
{
    internal class StaleWebStoreKeyEvent_Evictor : BaseWebStoreEvictor<StaleWebStoreKeyEvent>
    {
        protected override void Setup(StaleWebStoreKeyEvent cacheEvent)
        {

        }

        protected override void EvictNonDictionaryCacheData(StaleWebStoreKeyEvent cacheEvent)
        {
            ClearHtmlCache();
        }

        protected override List<string> EvictSpecificDictionaryCacheKeys(StaleWebStoreKeyEvent cacheEvent)
        {
            return new List<string>();
        }

        protected override bool IsDictionaryItemStale(StaleWebStoreKeyEvent cacheEvent, string key)
        {
            return key.Contains(cacheEvent.Key);
        }

        protected override void Teardown(StaleWebStoreKeyEvent cacheEvent)
        {

        }
    }
}
