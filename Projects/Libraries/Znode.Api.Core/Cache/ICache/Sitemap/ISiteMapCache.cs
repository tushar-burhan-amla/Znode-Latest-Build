namespace Znode.Engine.Api.Cache
{
    public interface ISiteMapCache
    {
        /// <summary>
        /// Get category list from cache.
        /// </summary>
        /// <param name="includeAssociatedCategories"> 
        /// This parameter is used to include the child categorys.
        /// if includeAssociatedCategories  is true then all the categories of parent product
        /// is included otherwise only parent categorires will display.
        /// </param>
        /// <param name="routeUri">URI to route.</param>       
        /// <param name="routeTemplate">Template to route.</param>       
        /// <returns>Response in string format.</returns>
        string GetSiteMapCategoryList(bool includeAssociatedCategories, string routeUri, string routeTemplate);


        /// <summary>
        /// Get brand list from cache
        /// </summary>
        /// <param name="routeUri">URI to route.</param>       
        /// <param name="routeTemplate">Template to route.</param>       
        /// <returns>Response in string format.</returns>
        string GetSiteMapBrandList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get publish product list from cache
        /// </summary>
        /// <param name="routeUri">URI to route.</param>       
        /// <param name="routeTemplate">Template to route.</param>       
        /// <returns>Response in string format.</returns>
        string GetSiteMapProductList(string routeUri, string routeTemplate);
    }
}
