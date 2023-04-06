namespace Znode.Engine.Api.Client.Endpoints
{
    public class InventoryEndpoint : BaseEndpoint
    {
        #region SKU inventory.
        //Add SKU inventory endpoint.
        public static string AddSKUInventory() => $"{ApiRoot}/skuinventory/create";

        //Get SKU inventory endpoint.
        public static string GetSKUInventory(int inventoryId) => $"{ApiRoot}/skuinventory/get/{inventoryId}";

        //SKU inventory List endpoint.
        public static string SKUInventoryList() => $"{ApiRoot}/skuinventory/list";

        //Update SKU inventory endpoint.
        public static string UpdateSKUInventory() => $"{ApiRoot}/skuinventory/update";

        //Delete SKU inventory endpoint.
        public static string DeleteSKUInventory() => $"{ApiRoot}/skuinventory/delete";
        #endregion

        #region Digital Asset
        //Downloadable product key List endpoint
        public static string GetDownloadableProductKeyList() => $"{ApiRoot}/inventory/downloadableproductkeylist";

        //Add downloadable product keys endpoint
        public static string AddDownloadableProductKeys() => $"{ApiRoot}/inventory/adddownloadableproductkeys";

        //Update Downloadable Product Key.
        public static string UpdateDownloadableProductKey() => $"{ApiRoot}/inventory/updatedownloadableproductkey";

        //Delete Downloadable Product Keys endpoint
        public static string DeleteDownloadableProductKeys() => $"{ApiRoot}/inventory/deletedownloadableproductkeys";
        #endregion
    }
}
