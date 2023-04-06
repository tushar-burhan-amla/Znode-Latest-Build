using System;

namespace Znode.Libraries.Caching
{
    public interface IEvictorBroker
    {
        void RegisterEvictor(Type evictorType);
        void InvokeEvictor(BaseCacheEvent cacheEvent);
    }
}
