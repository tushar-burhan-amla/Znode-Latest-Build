using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IInventoryClient : IBaseClient
    {
        #region SKU Inventory

        /// <summary>
        /// Get SKU Inventory list.
        /// </summary>
        /// <param name="expands">Expands for associated item.</param>
        /// <param name="filters">Filters for associated item.</param>
        /// <param name="sorts">Sorts for associated item.</param>
        /// <param name="pageIndex">Page index</param>
        /// <param name="pageSize">Page size</param>
        /// <returns>Inventory SKU List.</returns>
        InventorySKUListModel GetSKUInventoryList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get SKU Inventory by InventoryId.
        /// </summary>
        /// <param name="inventoryId">InventoryId.</param>
        /// <returns>Inventory SKU Model</returns>
        InventorySKUModel GetSKUInventory(int inventoryId);

        /// <summary>
        /// Add SKU Inventory. 
        /// </summary>
        /// <param name="inventorySKUModel">Inventory SKU Model to add SKU Inventory.</param>
        /// <returns>Added SKU Inventory.</returns>
        InventorySKUModel AddSKUInventory(InventorySKUModel inventorySKUModel);

        /// <summary>
        /// Update SKU Inventory.
        /// </summary>
        /// <param name="inventorySKUModel">InventorySKUModel to update existing SKU inventory.</param>
        /// <returns>Returns updated SKU Inventory.</returns>
        InventorySKUModel UpdateSKUInventory(InventorySKUModel inventorySKUModel);

        /// <summary>
        /// Delete existing SKU inventory.
        /// </summary>
        /// <param name="inventoryId">InventoryId to delete SKU inventory.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool DeleteSKUInventory(string inventoryId);

        #endregion

        #region Digital Asset
        /// <summary>
        /// Get downloadable product key list.
        /// </summary>
        /// <param name="expands">Expands for downloadable product  </param>
        /// <param name="filters">Filters for downloadable product</param>
        /// <param name="sorts">Sorts for downloadable product</param>
        /// <param name="pageIndex">pageIndex</param>
        /// <param name="pageSize">pageSize</param>
        /// <returns>DownloadableProductKeyListModel</returns>
        DownloadableProductKeyListModel GetDownloadableProductKeys(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Add downloadable product key.
        /// </summary>
        /// <param name="downloadableProductKeyModel">DownloadableProductKeyModel to create Keys</param>
        /// <returns>downloadableProductKeyModel</returns>
        DownloadableProductKeyModel AddDownloadableProductKey(DownloadableProductKeyModel downloadableProductKeyModel);

        /// <summary>
        /// Delete Downloadable Product Keys
        /// </summary>
        /// <param name="PimDownloadableProductKeyId">PimDownloadableProductKeyId to delete product key</param>
        /// <returns>true or false</returns>
        bool DeleteDownloadableProductKeys(string PimDownloadableProductKeyId);

        /// <summary>
        /// Update Downloadable Product Key
        /// </summary>
        /// <param name="downloadableProductKeyModel">DownloadableProductKeyModel to update Keys</param>
        /// <returns>Update downloadable product Keys</returns>
        bool UpdateDownloadableProductKey(DownloadableProductKeyModel downloadableProductKeyModel);

        #endregion
    }
}
