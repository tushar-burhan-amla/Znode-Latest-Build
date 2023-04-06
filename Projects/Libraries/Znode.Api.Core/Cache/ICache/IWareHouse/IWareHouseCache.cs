namespace Znode.Engine.Api.Cache
{
    public interface IWarehouseCache
    {
        #region Warehouse

        /// <summary>
        /// Get warehouse list.
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns warehouse list.</returns>
        string GetWarehouseList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get warehouse on the basis of warehouse id.
        /// </summary>
        /// <param name="warehouseId">Warehouse id.</param>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns warehouse.</returns>
        string GetWarehouse(int warehouseId, string routeUri, string routeTemplate); 
        #endregion

        #region Associate inventory

        /// <summary>
        /// Get associated inventory list for warehouse.
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns list of associated inventory.</returns>
        string GetAssociatedInventoryList(string routeUri, string routeTemplate);

        #endregion

    }
}