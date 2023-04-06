using System;
using Znode.Libraries.Caching.ElasticSearch;

namespace Znode.Libraries.Caching
{
    public class WrappedCacheEvent : Document, IWrappedCacheEvent
    {
        public Guid CacheEventId { get; set; }

        public string CacheEventAsJson { get; set; }

        public string CacheEventType { get; set; }

        public WrappedCacheEvent()
        {

        }
    }
}
