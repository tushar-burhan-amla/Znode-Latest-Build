using Znode.Libraries.Caching;

namespace Znode.WebStore.Caching.Core
{
    public class WebStoreCacheEventPoller : BaseCacheEventPoller
    {
        public WebStoreCacheEventPoller(int delayTime) : base(delayTime)
        {

        }
    }
}
