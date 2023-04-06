namespace Znode.Engine.Api.Cache
{
    public interface IWebStoreCategoryCache
    {
        /// <summary>
        /// Get a list of Store Locator details.
        /// </summary>       
        /// /// <param name="routeUri">URI to route.</param>       
        /// /// <param name="routeTemplate">Template to route.</param>       
        /// <returns>Response in string format.</returns>
        string GetCategoryDetails(string routeUri, string routeTemplate);
    }
}