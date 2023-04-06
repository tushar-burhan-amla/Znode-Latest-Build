namespace Znode.Engine.Api.Cache
{
    interface IVendorCache
    {
        /// <summary>
        /// Get a list of all vendors.
        /// </summary>
        /// <param name="routeUri">URI to route</param>
        /// <param name="routeTemplate">Template of route</param>
        /// <returns>Response in string format</returns>
        string GetVendors(string routeUri, string routeTemplate);

        /// <summary>
        /// Get vendor using PimVendorId.
        /// </summary>
        /// <param name="PimVendorId">PimVendorId use to retrieve vendor</param>
        /// <param name="routeUri">URI to route</param>
        /// <param name="routeTemplate">Template of route</param>
        /// <returns>Response in string format</returns>
        string GetVendor(int PimVendorId, string routeUri, string routeTemplate);


        /// <summary>
        /// Get vendor code list.
        /// </summary>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <param name="attributeCode">Attribute code </param>
        /// <returns></returns>
        string GetVendorCodeList(string attributeCode, string routeUri, string routeTemplate);

    }
}