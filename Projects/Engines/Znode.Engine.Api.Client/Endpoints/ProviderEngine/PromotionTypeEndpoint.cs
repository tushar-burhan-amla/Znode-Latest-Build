namespace Znode.Engine.Api.Client.Endpoints
{
    public class PromotionTypeEndpoint : BaseEndpoint
    {
        //Get Promotion Type List Endpoint
        public static string List() => $"{ApiRoot}/promotiontype/list";

        //Get PromotionType Endpoint
        public static string Get(int promotionTypeId) => $"{ApiRoot}/promotiontype/get/{promotionTypeId}";

        //Create PromotionType Endpoint
        public static string Create() => $"{ApiRoot}/promotiontype/create";

        //Update PromotionType Endpoint
        public static string Update() => $"{ApiRoot}/promotiontype/update";

        //Delete PromotionType Endpoint
        public static string Delete() => $"{ApiRoot}/promotiontype/delete";

        //Get PromotionType List which are Not In Database Endpoint
        public static string GetAllPromotionTypesNotInDatabase() => $"{ApiRoot}/promotiontype/getallpromotiontypesnotindatabase";

        //Enable/Disable bulky promotion types Endpoint.
        public static string BulkEnableDisablePromotionTypes(bool isEnable) => $"{ApiRoot}/promotiontype/bulkenabledisablepromotiontypes/{isEnable}";
    }
}
