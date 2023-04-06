using System.Collections.Generic;
using Znode.Api.Caching.Core;
using Znode.Libraries.Caching.Events;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Api.Caching.Evictors
{
    internal class BannerSliderPublishEvent_Evictor : BaseApiEvictor<BannerSliderPublishEvent>
    {
        private string routePrefix;

        protected override void Setup(BannerSliderPublishEvent cacheEvent)
        {
            routePrefix = GetRouteFromTemplateKey(CachedKeys.SliderBannerKey_);
        }

        protected override void EvictNonDictionaryCacheData(BannerSliderPublishEvent cacheEvent)
        {

        }

        protected override List<string> EvictSpecificDictionaryCacheKeys(BannerSliderPublishEvent cacheEvent)
        {
            return new List<string>();
        }

        protected override bool IsDictionaryItemStale(BannerSliderPublishEvent cacheEvent, string key)
        {
            // TODO - improve this so only necessary banner sliders with given ID are removed from cache
            bool isStale = key.Contains(routePrefix);
            return isStale;
        }

        protected override void Teardown(BannerSliderPublishEvent cacheEvent)
        {

        }
    }
}
