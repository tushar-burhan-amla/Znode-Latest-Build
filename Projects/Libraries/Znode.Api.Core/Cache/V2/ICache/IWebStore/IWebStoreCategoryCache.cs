namespace Znode.Engine.Api.Cache
{
    public interface IWebStoreCategoryCacheV2 : IWebStoreCategoryCache
    {
        /// <summary>
        /// Get a list of Products based on Category.
        /// </summary>       
        /// /// <param name="routeUri">URI to route.</param>       
        /// /// <param name="routeTemplate">Template to route.</param>       
        /// <returns>Response in string format.</returns>
        string GetCategoryProducts(string routeUri, string routeTemplate);
    }
}