namespace Znode.Libraries.Caching.Events
{
    public class StaleWebStoreKeyEvent : BaseCacheEvent
    {
        public string Key { get; set; }
        public int[] PortalIds { get; set; }
    }
}
