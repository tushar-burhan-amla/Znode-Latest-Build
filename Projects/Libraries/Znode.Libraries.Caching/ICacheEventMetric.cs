using System;
using Znode.Libraries.Caching.ElasticSearch;

namespace Znode.Libraries.Caching
{
    public interface ICacheEventMetric : IDocument
    {

        /// <summary>
        /// Guid of the cache event that this metric is created as a result of.
        /// </summary>
        Guid CacheEventId { get; }
        
        object CachedItemData { get; set; }

        bool CachedItemDataIsString { get; set; }

        string CachedItemKey { get; set; }

        string CacheStorageType { get; set; }

        /// <summary>
        /// Describes the type of metric gathered by this metric event.
        /// </summary>
        string MetricType { get; }
    }
}
