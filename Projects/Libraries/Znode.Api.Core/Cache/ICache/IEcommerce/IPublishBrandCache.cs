namespace Znode.Engine.Api.Cache
{
    public interface IPublishBrandCache
    {
        
        /// <summary>
        /// Get a list of all brands.
        /// </summary>
        /// <param name="routeUri">URI to route</param>
        /// <param name="routeTemplate">Template of route</param>
        /// <returns>Response in string format</returns>
        string GetPublishBrandList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get brand using brandId,portal Id.
        /// </summary>
        /// <param name="brandId">BrandId use to retrieve brand</param>
        /// <param name="localeId">locale Id </param>
        /// <param name="portalId">portal Id  </param>
        /// <param name="routeUri">URI to route</param>
        /// <param name="routeTemplate">Template of route</param>
        /// <returns>Response in string format</returns>
        string GetPublishBrand(int brandId, int localeId, int portalId, string routeUri, string routeTemplate);
    }
}
