using Znode.Libraries.Cloudflare.API;

namespace Znode.Libraries.Caching.Events
{
    public class CloudflarePurgeByHostNameEvent : BaseCacheEvent
    {
        public string ZoneId { get; set; }

        public HostListModel Hosts { get; set; }
    }
}
