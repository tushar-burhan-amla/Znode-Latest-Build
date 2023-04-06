using System.Collections.Generic;
using System.Linq;
using Znode.Api.Caching.Core;
using Znode.Libraries.Caching.Events;

namespace Znode.Api.Caching.Evictors
{
    internal class StaleApiRoutesEvent_Evictor : BaseApiEvictor<StaleApiRoutesEvent>
    {
        private IEnumerable<string> routePrefixes;

        protected override void Setup(StaleApiRoutesEvent cacheEvent)
        {
            routePrefixes = cacheEvent.RouteTemplateKeys.Select(k => GetRouteFromTemplateKey(k));
        }

        protected override void EvictNonDictionaryCacheData(StaleApiRoutesEvent cacheEvent)
        {

        }

        protected override List<string> EvictSpecificDictionaryCacheKeys(StaleApiRoutesEvent cacheEvent)
        {
            return new List<string>();
        }

        protected override bool IsDictionaryItemStale(StaleApiRoutesEvent cacheEvent, string key)
        {
            return routePrefixes.Any(prefix => key.Contains(prefix));
        }

        protected override void Teardown(StaleApiRoutesEvent cacheEvent)
        {

        }
    }
}
