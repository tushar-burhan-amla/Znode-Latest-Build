using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IInventoryService
    {
        #region SKU Inventory

        /// <summary>
        /// Get SKU Inventory list.
        /// </summary>
        /// <param name="expands">Expands for Associated Item.</param>
        /// <param name="filters">Filters for Associated Item.</param>
        /// <param name="sorts">Sorts for Associated Item.</param>
        /// <param name="page">Page index.</param>
        /// <returns>InventorySKUListModel</returns>
        InventorySKUListModel GetSKUInventoryList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get SKU Inventory by InventoryId.
        /// </summary>
        /// <param name="inventoryId">InventoryId.</param>
        /// <returns>InventorySKUModel</returns>
        InventorySKUModel GetSKUInventory(int inventoryId);

        /// <summary>
        /// Create SKU Inventory. 
        /// </summary>
        /// <param name="inventorySKUModel">InventorySKUModel to add SKU Inventory.</param>
        /// <returns>Added SKU Inventory.</returns>
        InventorySKUModel AddSKUInventory(InventorySKUModel inventorySKUModel);

        /// <summary>
        /// Update SKU Inventory.
        /// </summary>
        /// <param name="inventorySKUModel">InventorySKUModel to update existing SKU Inventory.</param>
        /// <returns>Returns true if updated.</returns>
        bool UpdateSKUInventory(InventorySKUModel inventorySKUModel);

        /// <summary>
        /// Delete existing SKU Inventory.
        /// </summary>
        /// <param name="inventoryId">InventoryId to delete SKU Inventory.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool DeleteSKUInventory(ParameterModel inventoryId);

        #endregion

        #region Digital Asset
        /// <summary>
        /// Get downloadable product key list from database.
        /// </summary>
        /// <param name="expands">Expands collection.</param>
        /// <param name="filters">Filters collection.</param>
        /// <param name="sorts">Sort collection.</param>
        /// <param name="page">Page Number.</param>
        /// <returns>Returns DownloadableProductKeyListModel</returns>
        DownloadableProductKeyListModel GetDownloadableProductKeyList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Add downloadable product keys.
        /// </summary>
        /// <param name="downloadableProductKeyModel">DownloadableProductKeyModel to create keys</param>
        /// <returns>downloadableProductKeyModel</returns>
        DownloadableProductKeyModel AddDownloadableProductKeys(DownloadableProductKeyModel downloadableProductKeyModel);

        /// <summary>
        /// Delete Downloadable Product Keys.
        /// </summary>
        /// <param name="PimDownloadableProductKeyId">PimDownloadableProductKeyId Id.</param>
        /// <returns>Returns True/False.</returns>
        bool DeleteDownloadableProductKeys(ParameterModel pimDownloadableProductKeyId);

        /// <summary>
        /// Update Downloadable Product Key.
        /// </summary>
        /// <param name="downloadableProductKeyModel"></param>
        /// <returns>Returns True/False.</returns>
        bool UpdateDownloadableProductKey(DownloadableProductKeyModel downloadableProductKeyModel);
        #endregion
    }
}
