using System.Collections.Generic;
using Znode.Libraries.Caching.Events;
using Znode.Libraries.ECommerce.Utilities;
using Znode.WebStore.Caching.Core;

namespace Znode.WebStore.Caching.Evictors
{
    internal class BannerSliderPublishEvent_Evictor : BaseWebStoreEvictor<BannerSliderPublishEvent>
    {
        protected override void Setup(BannerSliderPublishEvent cacheEvent)
        {

        }

        protected override void EvictNonDictionaryCacheData(BannerSliderPublishEvent cacheEvent)
        {
            ClearHtmlCache();
        }

        protected override List<string> EvictSpecificDictionaryCacheKeys(BannerSliderPublishEvent cacheEvent)
        {
            return new List<string>();
        }

        protected override bool IsDictionaryItemStale(BannerSliderPublishEvent cacheEvent, string key)
        {
            // TODO - improve this so only necessary banner sliders with given ID are removed from cache
            bool isStale = key.StartsWith(CachedKeys.SliderBannerKey_);
            return isStale;
        }

        protected override void Teardown(BannerSliderPublishEvent cacheEvent)
        {

        }
    }
}
