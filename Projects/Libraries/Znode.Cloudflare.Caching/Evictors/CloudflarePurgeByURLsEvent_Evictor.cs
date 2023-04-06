using System.Collections.Generic;
using Znode.Cloudflare.Caching.Core;
using Znode.Libraries.Caching.Events;
using Znode.Libraries.Cloudflare.API;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Cloudflare.Caching.Evictors
{
    internal class CloudflarePurgeByURLsEvent_Evictor : BaseCloudflareEvictor<CloudflarePurgeByURLsEvent>
    {
        protected override void Setup(CloudflarePurgeByURLsEvent cacheEvent)
        {

        }

        protected override void EvictNonDictionaryCacheData(CloudflarePurgeByURLsEvent cacheEvent)
        {
            ICloudflareRequest cloudflareRequest = ZnodeDependencyResolver.GetService<ICloudflareRequest>();
            PurgeResponseModel purgeResponseModel = cloudflareRequest.PurgeByURLs(cacheEvent.Files, cacheEvent.ZoneId);
        }

        protected override List<string> EvictSpecificDictionaryCacheKeys(CloudflarePurgeByURLsEvent cacheEvent)
        {
            return new List<string>();
        }

        protected override bool IsDictionaryItemStale(CloudflarePurgeByURLsEvent cacheEvent, string key)
        {
            return false;
        }

        protected override void Teardown(CloudflarePurgeByURLsEvent cacheEvent)
        {

        }
    }
}
