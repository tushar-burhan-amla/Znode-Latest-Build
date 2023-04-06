using System.Collections.Generic;
using Znode.Libraries.Caching.Events;
using Znode.WebStore.Caching.Core;

namespace Znode.WebStore.Caching.Evictors
{
    internal class ManuallyClearAllPublishedDataEvent_Evictor : BaseWebStoreEvictor<ManuallyClearAllPublishedDataEvent>
    {
        protected override void Setup(ManuallyClearAllPublishedDataEvent cacheEvent)
        {

        }

        protected override void EvictNonDictionaryCacheData(ManuallyClearAllPublishedDataEvent cacheEvent)
        {
            ClearHtmlCache();
        }

        protected override List<string> EvictSpecificDictionaryCacheKeys(ManuallyClearAllPublishedDataEvent cacheEvent)
        {
            return new List<string>();
        }

        protected override bool IsDictionaryItemStale(ManuallyClearAllPublishedDataEvent cacheEvent, string key)
        {
            return true;
        }

        protected override void Teardown(ManuallyClearAllPublishedDataEvent cacheEvent)
        {

        }
    }
}
