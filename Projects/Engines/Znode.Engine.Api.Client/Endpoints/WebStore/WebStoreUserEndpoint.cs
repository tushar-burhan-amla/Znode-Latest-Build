namespace Znode.Engine.Api.Client.Endpoints
{
    public class WebStoreUserEndpoint : BaseEndpoint
    {
        //Endpoint to get create address.
        public static string CreateUserAddress() => $"{ApiRoot}/webstoreaccount/createaccountaddress";
       
        //Endpoint to get update address.
        public static string UpdateUserAddress() => $"{ApiRoot}/webstoreaccount/updateaccountaddress";
        
        //Endpoint to get list of address for user.
        public static string GetUserAddressList() => $"{ApiRoot}/webstoreaccount/getuseraddresslist";

        //Endpoint to get address by address id.
        public static string GetAddress(int? addressId) => $"{ApiRoot}/webstoreaccount/getaddress/{addressId}";

        //Endpoint to delete address by address id.
        public static string DeleteAddress(int? addressId,int? userId) => $"{ApiRoot}/webstoreaccount/deleteaddress/{addressId}/{userId}";
        
    }
}
