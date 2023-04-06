namespace Znode.Engine.Api.Client.Endpoints.SavedCart
{
    public class SavedCartEndpoint : BaseEndpoint
    {
        public static string CreateSavedCart() => $"{ApiRoot}/savedcart/createsavedcart";

        public static string EditSaveCart() => $"{ApiRoot}/savedcart/editsavecart";

        public static string AddProductToCartForSaveCart(int omsTemplateId, int userId, int portalId) => $"{ApiRoot}/savedcart/addproducttocartforsavecart/{omsTemplateId}/{userId}/{portalId}";

        public static string EditSaveCartName(string templateName, int templateId) => $"{ApiRoot}/savedcart/EditSaveCartName/{templateName}/{templateId}";
    }
}
