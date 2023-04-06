using System;
using Znode.Libraries.Caching.ElasticSearch;

namespace Znode.Libraries.Caching
{
    public class CacheEventMetric : Document, ICacheEventMetric
    {

        /// <summary>
        /// Id of the cache event that triggered this metric.
        /// </summary>
        public Guid CacheEventId { get; set; }

        /// <summary>
        /// Serialized JSON string of cached item's value. Might not be available if C# object
        /// was not serializable.
        /// </summary>
        public object CachedItemData { get; set; }

        /// <summary>
        /// True if the item being stored in the cache was a string. This means this item would be easy to
        /// store in a centralized cache such as Redis.
        /// </summary>
        public bool CachedItemDataIsString { get; set; }

        /// <summary>
        /// Key of the cached item.
        /// </summary>
        public string CachedItemKey { get; set; }

        /// <summary>
        /// For example: "API", "WebStore", "Cloudflare"
        /// </summary>
        public string CacheStorageType { get; set; }

        /// <summary>
        /// For example: "EVICTING"
        /// </summary>
        public string MetricType { get; set; }

        public CacheEventMetric(Guid cacheEventId)
        {
            this.CacheEventId = cacheEventId;
        }
    }
}
