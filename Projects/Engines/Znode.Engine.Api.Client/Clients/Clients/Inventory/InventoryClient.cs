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
    public class InventoryClient : BaseClient, IInventoryClient
    {
        #region SKU Inventory
        //Method to get sku inventory list.
        public virtual InventorySKUListModel GetSKUInventoryList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = InventoryEndpoint.SKUInventoryList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            InventorySKUListResponse response = GetResourceFromEndpoint<InventorySKUListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            InventorySKUListModel inventorySKUListModel = new InventorySKUListModel { InventorySKUList = response?.InventorySKUList };
            inventorySKUListModel.MapPagingDataFromResponse(response);

            return inventorySKUListModel;
        }

        //Method to create sku inventory.
        public virtual InventorySKUModel AddSKUInventory(InventorySKUModel inventorySKUModel)
        {
            string endpoint = InventoryEndpoint.AddSKUInventory();

            ApiStatus status = new ApiStatus();
            InventorySKUResponse response = PostResourceToEndpoint<InventorySKUResponse>(endpoint, JsonConvert.SerializeObject(inventorySKUModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);
            return response?.InventorySKU;
        }

        //Method to get sku inventory on the basis of inventory id.
        public virtual InventorySKUModel GetSKUInventory(int inventoryId)
        {
            string endpoint = InventoryEndpoint.GetSKUInventory(inventoryId);

            ApiStatus status = new ApiStatus();
            InventorySKUResponse response = GetResourceFromEndpoint<InventorySKUResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response?.InventorySKU;
        }

        //Method to update sku inventory.
        public virtual InventorySKUModel UpdateSKUInventory(InventorySKUModel inventorySKUModel)
        {
            string endpoint = InventoryEndpoint.UpdateSKUInventory();

            ApiStatus status = new ApiStatus();
            InventorySKUResponse response = PutResourceToEndpoint<InventorySKUResponse>(endpoint, JsonConvert.SerializeObject(inventorySKUModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);
            return response?.InventorySKU;
        }

        //Method to delete sku inventory.
        public virtual bool DeleteSKUInventory(string inventoryId)
        {
            string endpoint = InventoryEndpoint.DeleteSKUInventory();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(new ParameterModel() { Ids = inventoryId }), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }
        #endregion

        #region Digital Asset 
        //Get downloadable product keys associated to product.
        public virtual DownloadableProductKeyListModel GetDownloadableProductKeys(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = InventoryEndpoint.GetDownloadableProductKeyList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            DownloadableProductKeyListResponse response = GetResourceFromEndpoint<DownloadableProductKeyListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            DownloadableProductKeyListModel downloadableProductKeyListModel = new DownloadableProductKeyListModel { DownloadableProductKeys = response?.DownloadableProductKeys };
            downloadableProductKeyListModel.MapPagingDataFromResponse(response);

            return downloadableProductKeyListModel;
        }

        //Add downloadable product keys.
        public virtual DownloadableProductKeyModel AddDownloadableProductKey(DownloadableProductKeyModel downloadableProductKeyModel)
        {
            //Get Endpoint.
            string endpoint = InventoryEndpoint.AddDownloadableProductKeys();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            DownloadableProductKeyResponse response = PostResourceToEndpoint<DownloadableProductKeyResponse>(endpoint, JsonConvert.SerializeObject(downloadableProductKeyModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);
            return response?.DownloadableProductKey;
        }

        //Delete Downloadable Product Keys.
        public virtual bool DeleteDownloadableProductKeys(string PimDownloadableProductKeyId)
        {
            string endpoint = InventoryEndpoint.DeleteDownloadableProductKeys();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(new ParameterModel() { Ids = PimDownloadableProductKeyId.ToString() }), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        //Update Downloadable Product Key.
        public virtual bool UpdateDownloadableProductKey(DownloadableProductKeyModel downloadableProductKeyModel)
        {
            string endpoint = InventoryEndpoint.UpdateDownloadableProductKey();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PutResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(downloadableProductKeyModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        #endregion
    }
}
