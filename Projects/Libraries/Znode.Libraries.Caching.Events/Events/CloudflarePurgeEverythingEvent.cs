namespace Znode.Libraries.Caching.Events
{
    public class CloudflarePurgeEverythingEvent : BaseCacheEvent
    {
        public string ZoneId { get; set; }
    }
}
