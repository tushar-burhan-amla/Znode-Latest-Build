using Znode.Libraries.Caching;

namespace Znode.Cloudflare.Caching.Core
{
    public class CloudflareCacheEventPoller : BaseCacheEventPoller
    {
        public CloudflareCacheEventPoller(int delayTime) : base(delayTime)
        {

        }
    }
}
