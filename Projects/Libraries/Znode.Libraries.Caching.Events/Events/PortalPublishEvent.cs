namespace Znode.Libraries.Caching.Events
{
    public class PortalPublishEvent : BaseCacheEvent
    {
        public int[] PortalIds { get; set; }
    }
}
