using System.Collections.Generic;
using System.Linq;
using Znode.Libraries.Caching.Events;
using Znode.WebStore.Caching.Core;

namespace Znode.WebStore.Caching.Evictors
{
    internal class PortalUpdateEvent_Evictor : BaseWebStoreEvictor<PortalUpdateEvent>
    {
        private List<string> keyPrefixesToEvict;

        protected override void Setup(PortalUpdateEvent cacheEvent)
        {
            // The following code results in the Store's settings being evicted from the WebStore's cache:
            keyPrefixesToEvict = cacheEvent.PortalUpdateEventEntries.Select(p => $"{p.PortalDomainName}?LocaleId=")
                .ToList();

            // Note that the code was adapted from the following code, from before the cache framework re-architecture:
            //
            // ---------------------------------------------------------------------------------------------------------
            //                                Znode.WebStore.Core - HomeController.cs
            //
            // public virtual void ClearPartialCache(string key)
            // {
            //    if (key == "webstoreportal")
            //    {
            //        HttpRuntime.Cache.Remove(Request.Url.Authority + "?LocaleId=" + PortalAgent.LocaleId);
            //        return;
            //    }
            //    ...
            // }
            // ---------------------------------------------------------------------------------------------------------
            //
            // Note that we no longer know the LocaleId. If we knew the LocaleId we could be more efficient and evict 
            // the specific key, but, we don't know the LocaleId, so that's why we evict every key matching the prefix.
        }

        protected override void EvictNonDictionaryCacheData(PortalUpdateEvent cacheEvent)
        {
            ClearHtmlCache();
        }

        protected override List<string> EvictSpecificDictionaryCacheKeys(PortalUpdateEvent cacheEvent)
        {
            return new List<string>();
        }

        protected override bool IsDictionaryItemStale(PortalUpdateEvent cacheEvent, string key)
        {
            return keyPrefixesToEvict.Any(prefix => key.StartsWith(prefix));
        }

        protected override void Teardown(PortalUpdateEvent cacheEvent)
        {

        }
    }
}
