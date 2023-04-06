namespace Znode.Engine.Api.Client.Endpoints
{
    public class StoreLocatorEndpoint : BaseEndpoint
    {
        //Get store data for location
        public static string GetStoreLocator(int storeId) => $"{ApiRoot}/storelocator/get/{storeId}";

        //Get store data for location
        public static string GetStoreLocatorByCode(string storeLocationCode) => $"{ApiRoot}/storelocator/getStoreLocatorByCode/{storeLocationCode}";

        //Save store data for store location.
        public static string SaveStore() => $"{ApiRoot}/storelocator/create";

        // Get Store List for location
        public static string GetStoreLocatorList() => $"{ApiRoot}/storelocator/list";

        //Update an existing store data for store location.
        public static string Manage() => $"{ApiRoot}/storelocator/update";

        // Delete an existing store data.
        public static string DeleteStoreLocator(bool isDeleteByCode) => $"{ApiRoot}/storelocator/delete/{isDeleteByCode}";
    }
}
