using Znode.Libraries.Caching.Events.Models;

namespace Znode.Libraries.Caching.Events
{
    public class PortalUpdateEvent : BaseCacheEvent
    {
        public PortalUpdateEventEntry[] PortalUpdateEventEntries { get; set; }
    }
}
