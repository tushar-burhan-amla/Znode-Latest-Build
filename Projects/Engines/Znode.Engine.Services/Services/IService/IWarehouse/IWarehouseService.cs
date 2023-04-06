using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IWarehouseService
    {
        #region Warehouse
        /// <summary>
        /// Gets the list of warehouse.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with warehouse list.</param>
        /// <param name="filters">Filters to be applied on warehouse list.</param>
        /// <param name="sorts">Sorting to be applied on warehouse list.</param>
        /// <param name="page">Page index.</param>
        /// <returns>Returns list of warehouse.</returns>
        WarehouseListModel GetWarehouseList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Create warehouse.
        /// </summary>
        /// <param name="warehouseModel">Warehouse Model.</param>
        /// <returns>Returns created warehouse Model.</returns>
        WarehouseModel CreateWarehouse(WarehouseModel warehouseModel);

        /// <summary>
        /// Get warehouse on the basis of warehouse id.
        /// </summary>
        /// <param name="warehouseId">WarehouseId.</param>
        /// <returns>Returns warehouse model.</returns>
        WarehouseModel GetWarehouse(int warehouseId);

        /// <summary>
        /// Update warehouse data.
        /// </summary>
        /// <param name="warehouseModel">Warehouse model to update.</param>
        /// <returns>Returns true if model updated successfully else return false.</returns>
        bool UpdateWarehouse(WarehouseModel warehouseModel);

        /// <summary>
        /// Delete warehouse.
        /// </summary>
        /// <param name="warehouseIds">Warehouse Id.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool DeleteWarehouse(ParameterModel warehouseIds); 
        #endregion

        #region Associate Inventory

        /// <summary>
        ///  Get associated inventory list for warehouse.
        /// </summary>
        /// <param name="expands">Expands for inventory warehouse.</param>
        /// <param name="filters">Filters for inventory warehouse.</param>
        /// <param name="sorts">Sorts for for inventory warehouse.</param>
        /// <param name="page">Page size.</param>
        /// <returns>Returns list of associated inventory.</returns>
        InventoryWarehouseMapperListModel GetAssociatedInventoryList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);
        #endregion
    }
}
