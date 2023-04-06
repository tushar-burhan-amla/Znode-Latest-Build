using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Agents
{
    public interface IInventoryAgent
    {
        #region SKU Inventory

        /// <summary>
        /// Add SKU Inventory.
        /// </summary>
        /// <param name="model">InventorySKUViewModel to create SKU Inventory.</param>
        /// <returns>Added SKU Inventory view model.</returns>
        InventorySKUViewModel AddSKUInventory(InventorySKUViewModel inventorySKUViewModel);

        /// <summary>
        /// Get SKUInventory list from database.
        /// </summary>
        /// <param name="expands">Expands collection.</param>
        /// <param name="filters">Filters collection.</param>
        /// <param name="sorts">Sort collection.</param>
        /// <param name="page">Page Number.</param>
        /// <returns>Returns InventorySKUViewListModel</returns>
        InventorySKUListViewModel GetSKUInventoryList(ExpandCollection expands = null, FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null);
  
        /// <summary>
        /// Get SKU Inventory by InventoryId.
        /// </summary>
        /// <param name="inventoryId">Get SKU Inventory on the basis of InventoryId.</param>
        /// <returns>Returns InventorySKUViewModel.</returns>
        InventorySKUViewModel GetSKUInventory(int inventoryId);

        /// <summary>
        /// Update SKU Inventory.
        /// </summary>
        /// <param name="inventorySKUViewModel">inventorySKUViewModel</param>
        /// <returns>Returns InventorySKUViewModel.</returns>
        InventorySKUViewModel UpdateSKUInventory(InventorySKUViewModel inventorySKUViewModel);

        /// <summary>
        /// Delete SKU Inventory on the basis of InventoryId.
        /// </summary>
        /// <param name="inventoryId">Inventory Id.</param>
        /// <returns>Returns True/False.</returns>
        bool DeleteSKUInventory(string inventoryId);

        /// <summary>
        /// Get warehouse list
        /// </summary>
        /// <returns>Returns warehouse list</returns>
        List<SelectListItem> GetWarehouseList();

        /// <summary>
        /// Sets filter for inventory list id.
        /// </summary>
        /// <param name="filters">Filter collection.</param>
        ///<param name="filterName">Name of filter.</param>
        ///<param name="id">filters id.</param>
        void SetFilters(FilterCollection filters, string filterName, int id);

        /// <summary>
        /// Get inventory by SKU from Product page.
        /// </summary>
        /// <param name="model"> FilterCollectionDataModel</param>
        /// <param name="SKU">Product SKU</param>
        /// <returns>InventorySKUListViewModel</returns>
        InventorySKUListViewModel GetInventoryBySKU(FilterCollectionDataModel model, string SKU);

        /// <summary>
        /// Get Inventory Object as model.
        /// </summary>
        /// <param name="sku"></param>
        /// <param name="productId"></param>
        /// <param name="isDownloadable"></param>
        /// <returns></returns>
        InventorySKUViewModel GetSKUInventoryBySKU(string sku, int productId, bool isDownloadable);

        /// <summary>
        /// Set import inventory details.
        /// </summary>
        /// <returns>InventorySKUViewModel</returns>
        InventorySKUViewModel SetImportInventoryDetails();

      
        #endregion

        #region Digital Asset
        /// <summary>
        /// Get all the downloadable product keys.
        /// </summary>
        /// <param name="productId">product id to get the downloadable product keys.</param>
        /// <param name="sku">sku to get the downloadable product keys.</param>
        /// <param name="expands">expands across downloadable product.</param>
        /// <param name="filters">filter list across downloadable product keys.</param>
        /// <param name="sortCollection">sort collection for downloadable product keys.</param>
        /// <param name="pageIndex">pageIndex for downloadable product keys record. </param>
        /// <param name="recordPerPage">paging downloadable product keys record per page.</param>
        /// <returns></returns>
        DownloadableProductKeyListViewModel GetDownloadableProductKeys(int productId, string sku, ExpandCollection expands = null, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Save/Add downloadable product keys.
        /// </summary>
        /// <param name="model">DownloadableProductViewModel to save downloadable product keys.</param>
        /// <param name="message">Error Message.</param>
        /// <returns>DownloadableProductKeyViewModel</returns>
        DownloadableProductKeyViewModel AddDownloadableProductKeys(DownloadableProductKeyViewModel model, out string message);

        /// <summary>
        /// Delete Downloadable Product Keys by PimDownloadableProductKeyId.
        /// </summary>
        /// <param name="PimDownloadableProductKeyId">PimDownloadableProductKeyId of Downloadable product key to be deleted.</param>
        /// <returns>Returns true if DeleteDownloadableProductKeys Deleted else returns false.</returns>
        bool DeleteDownloadableProductKeys(string pimDownloadableProductKeyId, out string message);

        /// <summary>
        /// Update Downloadable Product Key
        /// </summary>
        /// <param name="downloadableProductKeyViewModel">DownloadableProductViewModel to update downloadable product keys.</param>
        /// <returns>Returns true if keys updated successfully else return false.</returns>
        bool UpdateDownloadableProductKey(DownloadableProductKeyViewModel downloadableProductKeyViewModel, out string message);

        #endregion
    }
}
