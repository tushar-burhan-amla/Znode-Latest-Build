using System.Collections.Generic;
using Znode.Api.Caching.Core;
using Znode.Libraries.Caching.Events;

namespace Znode.Api.Caching.Evictors
{
    internal class ManuallyClearAllPublishedDataEvent_Evictor : BaseApiEvictor<ManuallyClearAllPublishedDataEvent>
    {
        protected override void Setup(ManuallyClearAllPublishedDataEvent cacheEvent)
        {

        }

        protected override void EvictNonDictionaryCacheData(ManuallyClearAllPublishedDataEvent cacheEvent)
        {

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
