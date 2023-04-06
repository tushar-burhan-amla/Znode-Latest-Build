using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Agents
{
    public interface IWarehouseAgent
    {
        /// <summary>
        /// Gets the list of Warehouse.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with warehouse list.</param>
        /// <param name="filters">Filters to be applied on warehouse list.</param>
        /// <param name="sorts">Sorting to be applied on warehouse list.</param>
        /// <returns>Returns warehouse list.</returns>
        WarehouseListViewModel GetWarehouseList(ExpandCollection expands, FilterCollection filters, SortCollection sorts);

        /// <summary>
        /// Gets the list of Warehouse.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with warehouse list.</param>
        /// <param name="filters">Filters to be applied on warehouse list.</param>
        /// <param name="sorts">Sorting to be applied on warehouse list.</param>
        /// <param name="pageIndex">Start page index of warehouse list.</param>
        /// <param name="pageSize">Page size of warehouse list.</param>
        /// <returns>Returns warehouse list.</returns>
        WarehouseListViewModel GetWarehouseList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Create warehouse.
        /// </summary>
        /// <param name="warehouseViewModel">Warehouse View Model.</param>
        /// <returns>Returns created model.</returns>
        WarehouseViewModel Create(WarehouseViewModel warehouseViewModel);

        /// <summary>
        /// Get warehouse list by warehouse id.
        /// </summary>
        /// <param name="warehouseId">Warehouse list Id</param>
        /// <returns>Returns WarehouseViewModel.</returns>
        WarehouseViewModel GetWarehouse(int warehouseId);

        /// <summary>
        /// Update warehouse.
        /// </summary>
        /// <param name="warehouseViewModel">Warehouse view model to update.</param>
        /// <returns>Returns updated warehouse model.</returns>
        WarehouseViewModel Update(WarehouseViewModel warehouseViewModel);

        /// <summary>
        /// Delete warehouse.
        /// </summary>
        /// <param name="warehouseId">Warehouse Id.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool DeleteWarehouse(string warehouseId, out string errorMessage);

        /// <summary>
        /// Get country list
        /// </summary>
        /// <returns>Returns country list</returns>
        List<SelectListItem> GetCountries();

        /// <summary>
        /// Sets filter for warehouse id.
        /// </summary>
        /// <param name="filters">Filter collection.</param>
        /// <param name="warehouseId">warehouseId.</param>
        void SetFilters(FilterCollection filters, int warehouseId);


        #region Associate Inventory
        /// <summary>
        /// Get associated inventory list for warehouse.
        /// </summary>
        /// <param name="expands">Expands for inventory list.</param>
        /// <param name="filters">Filters for inventory list.</param>
        /// <param name="sorts">Sorts for for inventory list.</param>
        /// <param name="pageIndex">Index of page.</param>
        /// <param name="pageSize">Size of page.</param>
        /// <returns>Returns list of inventory</returns>
        InventoryWarehouseMapperListViewModel GetAssociatedInventoryList(ExpandCollection expands = null, FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Get SKU Inventory by InventoryId.
        /// </summary>
        /// <param name="inventoryId">Get SKU Inventory on the basis of InventoryId.</param>
        /// <param name="warehouseId">warehouseId</param>
        /// <returns>Returns InventorySKUViewModel.</returns>
        InventorySKUViewModel GetSKUInventory(int inventoryId, int warehouseId);

        /// <summary>
        /// Update SKU Inventory.
        /// </summary>
        /// <param name="model">InventorySKUViewModel having key and value from view.</param>
        /// <returns>Returns InventorySKUViewModel.</returns>
        InventorySKUViewModel UpdateSKUInventory(InventorySKUViewModel inventorySKUViewModel);

        /// <summary>
        /// Delete SKU Inventory on the basis of InventoryId.
        /// </summary>
        /// <param name="inventoryId">Inventory Id.</param>
        /// <returns>Returns True/False.</returns>
        bool DeleteSKUInventory(string inventoryId);
        #endregion

    }
}
