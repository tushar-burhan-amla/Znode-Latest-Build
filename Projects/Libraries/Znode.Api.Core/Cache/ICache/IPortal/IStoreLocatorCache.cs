namespace Znode.Engine.Api.Cache
{
    public interface IStoreLocatorCache
    {   
        /// <summary>
        /// Get list of store for location.
        /// </summary>
        /// <param name="storeId">id to get store data.</param>
        /// <param name="routeUri">Route URI</param>
        /// <param name="routeTemplate">Route Template.</param>
        /// <returns>string of data.</returns>
        string GetStoreLocator(int storeId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get list of store for location.
        /// </summary>
        /// <param name="storeLocationCode">code of store location.</param>
        /// <param name="routeUri">Route URI</param>
        /// <param name="routeTemplate">Route Template.</param>
        /// <returns>string of data.</returns>
        string GetStoreLocator(string storeLocationCode , string routeUri, string routeTemplate);

        /// <summary>
        /// Get store list for location.
        /// </summary>
        /// <param name="routeUri">Route URI</param>
        /// <param name="routeTemplate">Route Template.</param>
        /// <returns>string of data.</returns>
        string GetStoreLocatorList(string routeUri, string routeTemplate);
    }
}
