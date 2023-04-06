using System.Collections.Generic;
using Znode.Libraries.Caching.Events;
using Znode.WebStore.Caching.Core;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.WebStore.Caching.Evictors
{
    internal class GlobalSettingCacheEvent_Evictor : BaseWebStoreEvictor<GlobalSettingCacheEvent>
    {
        protected override void Setup(GlobalSettingCacheEvent cacheEvent)
        {

        }

        protected override void EvictNonDictionaryCacheData(GlobalSettingCacheEvent cacheEvent)
        {

        }

        protected override List<string> EvictSpecificDictionaryCacheKeys(GlobalSettingCacheEvent cacheEvent)
        {
            return new List<string> { CachedKeys.DefaultGlobalConfigCache };
        }

        protected override bool IsDictionaryItemStale(GlobalSettingCacheEvent cacheEvent, string key)
        {
            return false;
        }

        protected override void Teardown(GlobalSettingCacheEvent cacheEvent)
        {

        }
    }
}