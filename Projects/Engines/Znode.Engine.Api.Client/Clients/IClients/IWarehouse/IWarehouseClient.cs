using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IWarehouseClient : IBaseClient
    {
        /// <summary>
        /// Gets the list of warehouse.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with warehouse list.</param>
        /// <param name="filters">Filters to be applied on warehouse list.</param>
        /// <param name="sorts">Sorting to be applied on warehouse list.</param>
        /// <param name="pageIndex">Start page index of warehouse list.</param>
        /// <param name="pageSize">Page size of warehouse list.</param>
        /// <returns>Returns warehouse list.</returns>
        WarehouseListModel GetWarehouseList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Create warehouse.
        /// </summary>
        /// <param name="warehouseModel">Warehouse Model.</param>
        /// <returns>Returns created warehouse Model.</returns>
        WarehouseModel CreateWarehouse(WarehouseModel warehouseModel);

        /// <summary>
        /// Get warehouse on the basis of warehouse id.
        /// </summary>
        /// <param name="warehouseId">WarehouseId to get warehouse details.</param>
        /// <returns>Returns WarehouseModel.</returns>
        WarehouseModel GetWarehouse(int warehouseId);

        /// <summary>
        /// Update warehouse data.
        /// </summary>
        /// <param name="warehouseModel">Warehouse model to update.</param>
        /// <returns>Returns updated warehouse model.</returns>
        WarehouseModel UpdateWarehouse(WarehouseModel warehouseModel);

        /// <summary>
        /// Delete warehouse.
        /// </summary>
        /// <param name="warehouseId">Warehouse Id.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool DeleteWarehouse(ParameterModel warehouseId);

        #region Associate inventory
        /// <summary>
        /// Get associated inventory list for warehouse.
        /// </summary>
        /// <param name="expands">Expands for inventory.</param>
        /// <param name="filters">Filters for inventory.</param>
        /// <param name="sorts">Sorts for for inventory.</param>
        /// <param name="pageIndex">Index of page.</param>
        /// <param name="pageSize">Size of page.</param>
        /// <returns>Returns list of associated inventory.</returns>
        InventoryWarehouseMapperListModel GetAssociatedInventoryList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);
        #endregion
    }
}
