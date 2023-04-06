using System.Collections.Generic;
using System.Linq;
using Znode.Api.Caching.Core;
using Znode.Libraries.Caching.Events;

namespace Znode.Api.Caching.Evictors
{
    internal class PortalUpdateEvent_Evictor : BaseApiEvictor<PortalUpdateEvent>
    {
        private IEnumerable<string> keysToEvict;

        protected override void Setup(PortalUpdateEvent cacheEvent)
        {
            keysToEvict = cacheEvent.PortalUpdateEventEntries.SelectMany(p => GetCachedItemsKeys(p.PortalId));
        }

        protected override void EvictNonDictionaryCacheData(PortalUpdateEvent cacheEvent)
        {

        }

        protected override List<string> EvictSpecificDictionaryCacheKeys(PortalUpdateEvent cacheEvent)
        {
            return cacheEvent.PortalUpdateEventEntries.Select(p => p.PortalDomainName).ToList();
        }

        protected override bool IsDictionaryItemStale(PortalUpdateEvent cacheEvent, string key)
        {
            return keysToEvict.Any(keyToEvict => key.Contains(keyToEvict));
        }

        protected override void Teardown(PortalUpdateEvent cacheEvent)
        {

        }

        private static List<string> GetCachedItemsKeys(int portalId)
        {
            List<string> cacheItemsToRemove = new List<string>();
            cacheItemsToRemove.Add("SliderBannerKey_" + portalId);
            cacheItemsToRemove.Add("ProductListKey_" + portalId);
            cacheItemsToRemove.Add("LinkKey_" + portalId);
            cacheItemsToRemove.Add("CategoryListKey_" + portalId);
            cacheItemsToRemove.Add("TextWidgetKey_" + portalId);
            cacheItemsToRemove.Add("TagManager_" + portalId);
            cacheItemsToRemove.Add("SpecialsNavigation" + portalId);
            cacheItemsToRemove.Add("BrandDropDownNavigation" + portalId);
            cacheItemsToRemove.Add("PriceNavigation" + portalId);
            cacheItemsToRemove.Add("BrandListKey_" + portalId);
            return cacheItemsToRemove;
        }
    }
}
