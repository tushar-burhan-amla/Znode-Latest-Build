namespace Znode.Libraries.Caching.Events
{
    public class StaleApiRoutesEvent : BaseCacheEvent
    {
        public string[] RouteTemplateKeys { get; set; }
    }
}
