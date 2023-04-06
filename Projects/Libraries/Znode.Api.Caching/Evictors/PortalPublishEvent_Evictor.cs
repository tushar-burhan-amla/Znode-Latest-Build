using System.Collections.Generic;
using Znode.Api.Caching.Core;
using Znode.Libraries.Caching.Events;

namespace Znode.Api.Caching.Evictors
{
    internal class PortalPublishEvent_Evictor : BaseApiEvictor<PortalPublishEvent>
    {
        protected override void Setup(PortalPublishEvent cacheEvent)
        {

        }

        protected override void EvictNonDictionaryCacheData(PortalPublishEvent cacheEvent)
        {

        }

        protected override List<string> EvictSpecificDictionaryCacheKeys(PortalPublishEvent cacheEvent)
        {
            return new List<string>();
        }

        protected override bool IsDictionaryItemStale(PortalPublishEvent cacheEvent, string key)
        {
            // TODO - Right now we clear everything in response to a portal publish event. We have additional
            // information on this event (portal domain name and portal id) which could be used to be more
            // selective and only evict what is necessary from the cache.
            return true;
        }

        protected override void Teardown(PortalPublishEvent cacheEvent)
        {

        }
    }
}
