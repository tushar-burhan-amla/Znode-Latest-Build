using System.Collections.Generic;
using Znode.Api.Caching.Core;
using Znode.Libraries.Caching.Events;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Api.Caching.Evictors
{
    internal class LoggingSettingsUpdateEvent_Evictor : BaseApiEvictor<LoggingSettingsUpdateEvent>
    {
        protected override void Setup(LoggingSettingsUpdateEvent cacheEvent)
        {

        }

        protected override void EvictNonDictionaryCacheData(LoggingSettingsUpdateEvent cacheEvent)
        {
            ZnodeCacheDependencyManager.Remove(CachedKeys.DefaultLoggingConfigCache); // TODO - check if this can be removed
        }

        protected override List<string> EvictSpecificDictionaryCacheKeys(LoggingSettingsUpdateEvent cacheEvent)
        {
            // TODO - Investigate why DefaultLoggingConfigCache item is already removed from local API node's cache
            // by this time this event is processed.
            return new List<string> { CachedKeys.DefaultLoggingConfigCache };
        }

        protected override bool IsDictionaryItemStale(LoggingSettingsUpdateEvent cacheEvent, string key)
        {
            return false;
        }

        protected override void Teardown(LoggingSettingsUpdateEvent cacheEvent)
        {

        }
    }
}
