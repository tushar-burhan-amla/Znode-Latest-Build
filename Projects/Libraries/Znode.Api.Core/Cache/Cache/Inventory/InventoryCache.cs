using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Api.Cache
{
    public class InventoryCache : BaseCache, IInventoryCache
    {
        #region Private Variable
        private readonly IInventoryService _service;
        #endregion

        #region Constructor
        public InventoryCache(IInventoryService inventoryService)
        {
            _service = inventoryService;
        }
        #endregion

        #region Public Methods

        #region SKU Inventory
        //Method to get sku inventory by inventory id.
        public virtual string GetSKUInventory(int inventoryId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                InventorySKUModel inventorySKU = _service.GetSKUInventory(inventoryId);
                if (IsNotNull(inventorySKU))
                {
                    InventorySKUResponse response = new InventorySKUResponse { InventorySKU = inventorySKU };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Method to get sku inventory list.
        public virtual string GetSKUInventoryList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                InventorySKUListModel list = _service.GetSKUInventoryList(Expands, Filters, Sorts, Page);
                if (list?.InventorySKUList?.Count > 0)
                {
                    InventorySKUListResponse response = new InventorySKUListResponse { InventorySKUList = list.InventorySKUList };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion

        #region Digital asset
        //Get downloadable product key list From Cache.
        public virtual string GetDownloadableProductKeyList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                DownloadableProductKeyListModel list = _service.GetDownloadableProductKeyList(Expands, Filters, Sorts, Page);
                if (list?.DownloadableProductKeys?.Count > 0)
                {
                    DownloadableProductKeyListResponse response = new DownloadableProductKeyListResponse { DownloadableProductKeys = list.DownloadableProductKeys };
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