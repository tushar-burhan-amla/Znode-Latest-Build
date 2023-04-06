namespace Znode.Engine.Api.Cache
{
    interface IBrandCache
    {
        /// <summary>
        /// Get a list of all brands.
        /// </summary>
        /// <param name="routeUri">URI to route</param>
        /// <param name="routeTemplate">Template of route</param>
        /// <returns>Response in string format</returns>
        string GetBrands(string routeUri, string routeTemplate);

        /// <summary>
        /// Get brand using brandId.
        /// </summary>
        /// <param name="brandId">BrandId use to retrieve brand</param>
        /// <param name="localeId">locale Id </param>
        /// <param name="routeUri">URI to route</param>
        /// <param name="routeTemplate">Template of route</param>
        /// <returns>Response in string format</returns>
        string GetBrand(int brandId, int localeId , string routeUri, string routeTemplate);

        /// <summary>
        /// Get brand code list.
        /// </summary>
        /// <param name="routeUri">URI to route</param>
        /// <param name="routeTemplate">Template of route</param>
        /// <param name="attributeCode">Attribute code </param>
        /// <returns></returns>
        string GetBrandCodeList(string attributeCode, string routeUri, string routeTemplate);

        /// <summary>
        /// Get the list of all portals.
        /// </summary>
        /// <param name="routeUri">URI to route.</param>
        /// <param name="routeTemplate">Template of route.</param>
        /// <returns>list of portal in string format by serializing it.</returns>
        string GetBrandPortalList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get the list of all brands associate/unAssociate with portal.
        /// </summary>
        /// <param name="routeUri">URI to route.</param>
        /// <param name="routeTemplate">Template of route.</param>
        /// <returns>list of portal in string format by serializing it.</returns>
        string GetPortalBrandList(string routeUri, string routeTemplate);


    }
}
