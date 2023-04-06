using System;

namespace Znode.Libraries.Caching
{
    public interface ICacheEventEvictor<T> where T : BaseCacheEvent
    {
        
        void HandleCacheEvent(T cacheEvent);

        Type GetCacheEventType();
    }
}
