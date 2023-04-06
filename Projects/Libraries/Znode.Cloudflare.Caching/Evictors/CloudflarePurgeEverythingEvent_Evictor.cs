using System.Collections.Generic;
using Znode.Cloudflare.Caching.Core;
using Znode.Libraries.Caching.Events;
using Znode.Libraries.Cloudflare.API;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Cloudflare.Caching.Evictors
{
    internal class CloudflarePurgeEverythingEvent_Evictor : BaseCloudflareEvictor<CloudflarePurgeEverythingEvent>
    {
        protected override void Setup(CloudflarePurgeEverythingEvent cacheEvent)
        {

        }

        protected override void EvictNonDictionaryCacheData(CloudflarePurgeEverythingEvent cacheEvent)
        {
            ICloudflareRequest cloudflareRequest = ZnodeDependencyResolver.GetService<ICloudflareRequest>();
            PurgeResponseModel purgeResponseModel = cloudflareRequest.PurgeEverything(cacheEvent.ZoneId);
        }

        protected override List<string> EvictSpecificDictionaryCacheKeys(CloudflarePurgeEverythingEvent cacheEvent)
        {
            return new List<string>();
        }

        protected override bool IsDictionaryItemStale(CloudflarePurgeEverythingEvent cacheEvent, string key)
        {
            return false;
        }

        protected override void Teardown(CloudflarePurgeEverythingEvent cacheEvent)
        {

        }
    }
}
