namespace Znode.Engine.Api.Cache
{
    public interface IInventoryCache
    {
        #region SKU Inventory

        /// <summary>
        /// Get SKU Inventory from Cache By InventoryId.
        /// </summary>
        /// <param name="inventoryId">InventoryId.</param>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns SKU Inventory on the basis of InventoryId.</returns>
        string GetSKUInventory(int inventoryId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get SKU Inventory list.
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns list of SKU Inventory.</returns>
        string GetSKUInventoryList(string routeUri, string routeTemplate);

        #endregion

        #region Digital Asset
        /// <summary>
        /// Get downloadable product list.
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns list of downloadable product list.</returns>
        string GetDownloadableProductKeyList(string routeUri, string routeTemplate);
        #endregion
    }
}