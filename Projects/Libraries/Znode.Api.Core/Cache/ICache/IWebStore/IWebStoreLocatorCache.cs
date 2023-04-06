namespace Znode.Engine.Api.Cache
{
    public interface IWebStoreLocatorCache
    {
        /// <summary>
        /// Get a list of Store Locator.
        /// </summary>              
        /// /// <param name="routeUri">URI to route.</param>       
        /// /// <param name="routeTemplate">Template to route.</param>       
        /// <returns>Response in string format.</returns>
        string GetWebStoreLocatorList(string routeUri, string routeTemplate);
    }
}