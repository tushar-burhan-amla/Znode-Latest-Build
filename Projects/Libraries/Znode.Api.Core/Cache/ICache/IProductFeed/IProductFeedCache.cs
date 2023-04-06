namespace Znode.Engine.Api.Cache
{
    public interface IProductFeedCache
    {
        /// <summary>
        /// Get a list of all product feed.
        /// </summary>
        /// <param name="routeUri">URI to route</param>
        /// <param name="routeTemplate">Template of route</param>
        /// <returns>Response in string format</returns>
        string GetProductFeedList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get product feed by id.
        /// </summary>
        /// <param name="productFeedId">productFeedId</param>
        /// <param name="routeUri">URI to route</param>
        /// <param name="routeTemplate">Template of route</param>
        /// <returns>Response in string format</returns>
        string GetProductFeed(int productFeedId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get product feed by id.
        /// </summary>
        /// <param name="routeUri">URI to route</param>
        /// <param name="routeTemplate">Template of route</param>
        /// <param name="portalId">portalId</param>
        /// <returns>Response in string format</returns>
        string GetProductFeedByPortalId(string routeUri, string routeTemplate, int portalId);

    }
}
