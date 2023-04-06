using DevTrends.MvcDonutCaching;
using Znode.Libraries.Caching;
using Znode.WebStore.Caching.Helpers;

namespace Znode.WebStore.Caching.Core
{
    public abstract class BaseWebStoreEvictor<T> : BaseCacheEventEvictor<T> where T : BaseCacheEvent, new()
    {
        // Convenience methods/properties for assisting with HTML cache eviction ???
        protected void ClearHtmlCache()
        {
            OutputCacheManager cacheManager = new OutputCacheManager();
            cacheManager.RemoveItems();

            OutputCacheHelper.ClearOutputCache();                       
        }

        protected override string GetCacheStorageType()
        {
            return "WebStore";
        }
    }
}
