using System;
using System.Collections.Generic;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Maps
{
    public class InventoryViewModelMap
    {
        public static List<InventoryWarehouseMapperModel> ToAssociateInventoryListModel(int warehouseId, string inventoryListIds)
        {
            List<InventoryWarehouseMapperModel> listModel = new List<InventoryWarehouseMapperModel>();

            if (!string.IsNullOrEmpty(inventoryListIds))
                foreach (string inventoryListId in inventoryListIds.Split(','))
                    listModel.Add(new InventoryWarehouseMapperModel { WarehouseId = warehouseId, InventoryListId = Convert.ToInt32(inventoryListId) });
            
            return listModel;
        }

        //Map DownloadableProductKeyViewModel to DownloadableProductKeyListModel.
        public static DownloadableProductKeyModel ToDownloadableProductKeyListModel(DownloadableProductKeyViewModel model)
        {
            if (HelperUtility.IsNotNull(model?.DownloadableProductKeyList))
            {
                int rowIndexId = model.rowIndexId = 0;
                DownloadableProductKeyModel listModel = new DownloadableProductKeyModel { DownloadableProductKeyList = new List<DownloadableProductKeyModel>() };
                listModel.SKU = model.SKU;
                foreach (DownloadableProductKeyViewModel item in model.DownloadableProductKeyList)
                {
                    listModel.DownloadableProductKeyList.Add(new DownloadableProductKeyModel { DownloadableProductKey = item.DownloadableProductKey, DownloadableProductURL = item.DownloadableProductURL, SKU = model.SKU, rowIndexId = rowIndexId });
                    rowIndexId++;
                }

                return listModel;
            }
            return new DownloadableProductKeyModel();
        }
    }
}