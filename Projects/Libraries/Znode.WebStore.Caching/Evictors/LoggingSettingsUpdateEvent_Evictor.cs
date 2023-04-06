using System.Collections.Generic;
using Znode.Libraries.Caching.Events;
using Znode.Libraries.ECommerce.Utilities;
using Znode.WebStore.Caching.Core;

namespace Znode.WebStore.Caching.Evictors
{
    internal class LoggingSettingsUpdateEvent_Evictor : BaseWebStoreEvictor<LoggingSettingsUpdateEvent>
    {
        protected override void Setup(LoggingSettingsUpdateEvent cacheEvent)
        {

        }

        protected override void EvictNonDictionaryCacheData(LoggingSettingsUpdateEvent cacheEvent)
        {

        }

        protected override List<string> EvictSpecificDictionaryCacheKeys(LoggingSettingsUpdateEvent cacheEvent)
        {
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
