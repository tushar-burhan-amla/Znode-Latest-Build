namespace Znode.Engine.Api.Client.Endpoints.SaveForLater
{
    public class SaveForLaterEndpoint : BaseEndpoint
    {
        //Create save for later
        public static string CreateSaveForLater() => $"{ApiRoot}/saveforlater/createcartforlater";

        //Get save for later cart.
        public static string GetSaveForLaterTemplate(int userId, string templateType) => $"{ApiRoot}/saveforlater/getcartforlater/{userId}/{templateType}";

        //Delete cart item endpoint.
        public static string DeleteCartItem() => $"{ApiRoot}/saveforlater/deletecartitem";

        //Delete all cart item endpoint.
        public static string DeleteAllCartItems(int omsTemplateId, bool isFromSavedCart = false) => $"{ApiRoot}/saveforlater/deleteallcartitems/{omsTemplateId}/{isFromSavedCart}";

        //Get cart template
        public static string GetAccountTemplate(int omsTemplateId) => $"{ApiRoot}/saveforlater/getcarttemplate/{omsTemplateId}";

    }
}
