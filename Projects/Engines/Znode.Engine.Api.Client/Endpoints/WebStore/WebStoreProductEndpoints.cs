namespace Znode.Engine.Api.Client.Endpoints
{
    public class WebStoreProductEndpoints : BaseEndpoint
    {
        //Get Product List
        public static string List() => $"{ApiRoot}/webstoreproducts/list";

        //Get Product by Id.
        public static string GetProduct(int productId) => $"{ApiRoot}/webstoreproducts/get/{productId}";

        //Get List Of Associated product
        public static string GetAssociatedProducts() => $"{ApiRoot}/webstoreproducts/getassociatedproducts";

        //Get associated product highlights.
        public static string GetProductHighlights(int productId, int localeId) => $"{ApiRoot}/webstoreproducts/getproducthighlights/{productId}/{localeId}";

    }
}
