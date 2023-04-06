using System;
using Znode.Libraries.Caching.ElasticSearch;

namespace Znode.Libraries.Caching
{
    public interface IWrappedCacheEvent : IDocument
    {
        /// <summary>
        /// Guid of the cache event.
        /// </summary>
        Guid CacheEventId { get; }

        string CacheEventAsJson { get; set; }

        string CacheEventType { get; set; }
    }
}
