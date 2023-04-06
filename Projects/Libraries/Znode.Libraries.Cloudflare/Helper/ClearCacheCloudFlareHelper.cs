using Znode.Libraries.Caching;

namespace Znode.Libraries.Cloudflare.Helper
{
    /// <summary>
    /// Helper class that provides methods to trigger cache eviction.
    /// </summary>
    public static class ClearCacheCloudFlareHelper
    {
        /// <summary>
        /// Writes the cache events to elasticsearch.
        /// </summary>
        private static ICacheEventWriter cacheEventWriter = new ElasticsearchCacheEventWriter();

        /// <summary>
        /// Enqueue eviction event (writes event to elasticsearch).
        /// </summary>
        public static void EnqueueEviction(BaseCacheEvent cacheEvent)
        {
            cacheEventWriter.WriteEvent(cacheEvent);
        }
    }
}
