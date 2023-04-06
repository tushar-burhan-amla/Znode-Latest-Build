using Znode.Libraries.Caching;

namespace Znode.Api.Caching.Core
{
    public class ApiCacheEventPoller : BaseCacheEventPoller
    {
        public ApiCacheEventPoller(int delayTime) : base(delayTime)
        {

        }
    }
}
