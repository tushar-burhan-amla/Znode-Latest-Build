using System.Collections.Generic;
using Znode.Cloudflare.Caching.Core;
using Znode.Libraries.Caching.Events;
using Znode.Libraries.Cloudflare.API;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Cloudflare.Caching.Evictors
{
    internal class CloudflarePurgeByHostNameEvent_Evictor : BaseCloudflareEvictor<CloudflarePurgeByHostNameEvent>
    {
        protected override void Setup(CloudflarePurgeByHostNameEvent cacheEvent)
        {

        }

        protected override void EvictNonDictionaryCacheData(CloudflarePurgeByHostNameEvent cacheEvent)
        {
            ICloudflareRequest cloudflareRequest = ZnodeDependencyResolver.GetService<ICloudflareRequest>();
            PurgeResponseModel purgeResponseModel = cloudflareRequest.PurgeByHostName(cacheEvent.Hosts, cacheEvent.ZoneId);
        }

        protected override List<string> EvictSpecificDictionaryCacheKeys(CloudflarePurgeByHostNameEvent cacheEvent)
        {
            return new List<string>();
        }

        protected override bool IsDictionaryItemStale(CloudflarePurgeByHostNameEvent cacheEvent, string key)
        {
            return false;
        }

        protected override void Teardown(CloudflarePurgeByHostNameEvent cacheEvent)
        {

        }
    }
}
