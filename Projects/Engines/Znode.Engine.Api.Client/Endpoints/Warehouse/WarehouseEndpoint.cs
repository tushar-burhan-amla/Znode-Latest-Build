namespace Znode.Engine.Api.Client.Endpoints
{
    public class WarehouseEndpoint : BaseEndpoint
    {
        #region Warehouse
        //Get warehouse list Endpoint.
        public static string GetWarehouseList() => $"{ApiRoot}/warehouse/list";

        //Create warehouse Endpoint.
        public static string CreateWarehouse() => $"{ApiRoot}/warehouse/create";

        //Get warehouse on the basis of warehouse id Endpoint.
        public static string GetWarehouse(int warehouseId) => $"{ApiRoot}/warehouse/{warehouseId}";

        //Update warehouse Endpoint.
        public static string UpdateWarehouse() => $"{ApiRoot}/warehouse/update";

        //Delete warehouse Endpoint.
        public static string DeleteWarehouse() => $"{ApiRoot}/warehouse/delete"; 
        #endregion

        #region Associate inventories

        //Get assigned inventory list endpoint.
        public static string GetAssociatedInventoryList() => $"{ApiRoot}/warehouse/associatedinventory/list";

        #endregion

    }
}
