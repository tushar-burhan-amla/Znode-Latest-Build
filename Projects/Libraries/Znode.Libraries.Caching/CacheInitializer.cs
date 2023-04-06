using Znode.Libraries.Caching.ElasticSearch;

namespace Znode.Libraries.Caching
{
    public static class CacheInitializer
    {

        public static void EnsureInitialized()
        {
            ElasticSearchHelper.CreateIndexIfNotExists(CacheFrameworkSettings.GetCacheEventIndexName());
            ElasticSearchHelper.CreateIndexIfNotExists(CacheFrameworkSettings.GetCacheMetricsIndexName());
        }
    }
}
