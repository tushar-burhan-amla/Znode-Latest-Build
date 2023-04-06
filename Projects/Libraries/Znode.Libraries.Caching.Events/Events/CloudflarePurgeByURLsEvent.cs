using Znode.Libraries.Cloudflare.API;

namespace Znode.Libraries.Caching.Events
{
    public class CloudflarePurgeByURLsEvent : BaseCacheEvent
    {
        public string ZoneId { get; set; }

        public FilesListModel Files { get; set; }
    }
}
