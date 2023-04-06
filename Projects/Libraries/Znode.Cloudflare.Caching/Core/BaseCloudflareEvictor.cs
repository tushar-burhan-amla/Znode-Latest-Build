using Znode.Libraries.Caching;

namespace Znode.Cloudflare.Caching.Core
{
    internal abstract class BaseCloudflareEvictor<T> : BaseCacheEventEvictor<T> where T : BaseCacheEvent, new()
    {
        protected override string GetCacheStorageType()
        {
            return "Cloudflare";
        }
    }
}
