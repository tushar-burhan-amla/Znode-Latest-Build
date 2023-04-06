namespace Znode.Engine.Api.Cache
{
    public interface IPublishHistoryCache
    {
        /// <summary>
        /// Get Publish History List.
        /// </summary>
        /// <param name="routeUri">URI to route.</param>
        /// <param name="routeTemplate">Template of route.</param>
        /// <returns></returns>
        string GetPublishHistoryList(string routeUri, string routeTemplate);
    }
}
