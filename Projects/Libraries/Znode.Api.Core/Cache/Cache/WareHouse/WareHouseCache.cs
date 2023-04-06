using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Api.Cache
{
    public class WarehouseCache : BaseCache, IWarehouseCache
    {
        #region Private Variable
        private readonly IWarehouseService _service;
        #endregion

        #region Constructor
        public WarehouseCache(IWarehouseService warehouseService)
        {
            _service = warehouseService;
        }
        #endregion

        #region Public Methods

        #region Warehouse
        //Get list of Warehouse.
        public virtual string GetWarehouseList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (IsNull(data))
            {
                //Get warehouse list.
                WarehouseListModel list = _service.GetWarehouseList(Expands, Filters, Sorts, Page);
                if (list?.WarehouseList?.Count > 0)
                {
                    //Create response.
                    WarehouseListResponse response = new WarehouseListResponse { WarehouseList = list.WarehouseList };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get warehouse by warehouse id.
        public virtual string GetWarehouse(int warehouseId, string routeUri, string routeTemplate)
        {
            //Get data from Cache
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Create Response
                WarehouseModel warehouseModel = _service.GetWarehouse(warehouseId);
                if (IsNotNull(warehouseModel))
                {
                    WarehouseResponse response = new WarehouseResponse { Warehouse = warehouseModel };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion

        #region Associate inventory
        //Get associated inventory list.
        public virtual string GetAssociatedInventoryList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                InventoryWarehouseMapperListModel list = _service.GetAssociatedInventoryList(Expands, Filters, Sorts, Page);
                if (list?.InventoryWarehouseMapperList?.Count > 0)
                {
                    InventoryWarehouseMapperListResponse response = new InventoryWarehouseMapperListResponse { InventoryWarehouseMapperList = list.InventoryWarehouseMapperList };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion
        #endregion
    }
}