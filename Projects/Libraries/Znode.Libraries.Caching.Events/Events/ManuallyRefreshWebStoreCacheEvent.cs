namespace Znode.Libraries.Caching.Events
{
    public class ManuallyRefreshWebStoreCacheEvent : BaseCacheEvent
    {
        public int[] DomainIds { get; set; }
    }
}
