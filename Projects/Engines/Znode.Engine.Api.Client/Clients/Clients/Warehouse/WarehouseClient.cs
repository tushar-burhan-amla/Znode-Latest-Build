using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public class WarehouseClient: BaseClient, IWarehouseClient
    {
        #region Warehouse

        // Gets the list of warehouse.
        public virtual WarehouseListModel GetWarehouseList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = WarehouseEndpoint.GetWarehouseList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            WarehouseListResponse response = GetResourceFromEndpoint<WarehouseListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            WarehouseListModel list = new WarehouseListModel { WarehouseList = response?.WarehouseList };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Method to create warehouse.
        public virtual WarehouseModel CreateWarehouse(WarehouseModel warehouseModel)
        {
            //Get Endpoint.
            string endpoint = WarehouseEndpoint.CreateWarehouse();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            WarehouseResponse response = PostResourceToEndpoint<WarehouseResponse>(endpoint, JsonConvert.SerializeObject(warehouseModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.Warehouse;
        }

        //Get warehouse by warehouseId
        public virtual WarehouseModel GetWarehouse(int warehouseId)
        {
            string endpoint = WarehouseEndpoint.GetWarehouse(warehouseId);

            ApiStatus status = new ApiStatus();
            WarehouseResponse response = GetResourceFromEndpoint<WarehouseResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.Warehouse;
        }

        //Method to update warehouse.
        public virtual WarehouseModel UpdateWarehouse(WarehouseModel warehouseModel)
        {
            string endpoint = WarehouseEndpoint.UpdateWarehouse();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            WarehouseResponse response = PutResourceToEndpoint<WarehouseResponse>(endpoint, JsonConvert.SerializeObject(warehouseModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.Warehouse;
        }

        //Method to delete warehouse
        public virtual bool DeleteWarehouse(ParameterModel warehouseId)
        {
            string endpoint = WarehouseEndpoint.DeleteWarehouse();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(warehouseId), status);

            //check the status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        } 
        #endregion

        #region Associate inventory
        //Method to get associated inventory list.
        public virtual InventoryWarehouseMapperListModel GetAssociatedInventoryList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = WarehouseEndpoint.GetAssociatedInventoryList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            InventoryWarehouseMapperListResponse response = GetResourceFromEndpoint<InventoryWarehouseMapperListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            InventoryWarehouseMapperListModel inventoryWarehouseMapperListModel = new InventoryWarehouseMapperListModel { InventoryWarehouseMapperList = response?.InventoryWarehouseMapperList };
            inventoryWarehouseMapperListModel.MapPagingDataFromResponse(response);

            return inventoryWarehouseMapperListModel;
        }
        #endregion
    }
}
