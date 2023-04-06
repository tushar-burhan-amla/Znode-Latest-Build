using System.Collections.Generic;
using Znode.Libraries.Caching.Events;
using Znode.WebStore.Caching.Core;

namespace Znode.WebStore.Caching.Evictors
{
    internal class CustomerReviewUpdateEvent_Evictor : BaseWebStoreEvictor<CustomerReviewUpdateEvent>
    {
        protected override void Setup(CustomerReviewUpdateEvent cacheEvent)
        {

        }

        protected override void EvictNonDictionaryCacheData(CustomerReviewUpdateEvent cacheEvent)
        {
            ClearHtmlCache();
        }

        protected override List<string> EvictSpecificDictionaryCacheKeys(CustomerReviewUpdateEvent cacheEvent)
        {
            return new List<string>();
        }

        protected override bool IsDictionaryItemStale(CustomerReviewUpdateEvent cacheEvent, string key)
        {
            return false;
        }

        protected override void Teardown(CustomerReviewUpdateEvent cacheEvent)
        {

        }
    }
}
