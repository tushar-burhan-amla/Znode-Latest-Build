namespace Znode.Engine.Api.Client.Endpoints
{
    public class ShippingTypeEndpoint : BaseEndpoint
    {
        //GetShipping Type List Endpoint
        public static string List() => $"{ApiRoot}/shippingtype/list";

        //GetShipping Type Endpoint
        public static string Get(int shippingTypeId) => $"{ApiRoot}/shippingtype/get/{shippingTypeId}";

        //CreateShipping Type Endpoint
        public static string Create() => $"{ApiRoot}/shippingtype/create";

        //UpdateShipping Type Endpoint
        public static string Update() => $"{ApiRoot}/shippingtype/update";

        //DeleteShipping Type Endpoint
        public static string Delete() => $"{ApiRoot}/shippingtype/delete";

        //GetShipping Type List which are Not In Database Endpoint
        public static string GetAllShippingTypesNotInDatabase() => $"{ApiRoot}/shippingtype/getallshippingtypesnotindatabase";

        //Enable/Disable bulky shipping types Endpoint.
        public static string BulkEnableDisableShippingTypes(bool isEnable) => $"{ApiRoot}/shippingtype/bulkenabledisableshippingtypes/{isEnable}";
    }
}
