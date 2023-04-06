namespace Znode.Engine.Api.Client.Endpoints
{
    public class PublishProductEndpoint : BaseEndpoint
    {
        //Get Publish Product 
        public static string GetPublishProductList() => $"{ApiRoot}/publishproduct/list";

        //Get Publish product 
        public static string GetPublishProduct(int publishProductId) => $"{ApiRoot}/publishproduct/get/{publishProductId}";

        //Get only the brief details of a published product 
        public static string GetPublishProductBrief(int publishProductId) => $"{ApiRoot}/publishproduct/getproductbrief/{publishProductId}";

        //Get only the details of a parent published product 
        public static string GetpublishParentProduct(int publishProductId) => $"{ApiRoot}/publishproduct/getpublishparentproduct/{publishProductId}";

        //Get only the extended details of a published product 
        public static string GetExtendedPublishProductDetails(int publishProductId) => $"{ApiRoot}/publishproduct/getextendedproductdetails/{publishProductId}";

        //Gets the list of products according to parameter model.
        public static string GetProductList() => $"{ApiRoot}/publishproduct/getproductlist";

        //get product inventory and price.
        public static string GetProductPriceAndInventory() => $"{ApiRoot}/publishproduct/getproductpriceandinventory";

        //Get product details by product sku.
        public static string GetPublishProductBySKU() => $"{ApiRoot}/publishproduct/getproductbysku";

        //Get product details by product skus.
        public static string GetPriceWithInventory() => $"{ApiRoot}/publishproduct/getpricewithinventory";


        //Get Configurable product
        public static string GetConfigurableProduct() => $"{ApiRoot}/publishproduct/getconfigurableproduct";

        //Get Configurable Parent product
        public static string GetParentProduct(int parentProductId) => $"{ApiRoot}/publishproduct/GetParentProduct/{parentProductId}";


        //Get group product list.
        public static string GetGroupProductList() => $"{ApiRoot}/publishproduct/getgroupproducts";

        //Get bundle product list.
        public static string GetBundleProducts() => $"{ApiRoot}/publishproduct/getbundleproducts";

        //Get product attributes by product id.
        public static string GetProductAttribute(int productId) => $"{ApiRoot}/publishproduct/getproductattribute/{productId}";

        //Get publish product excluding assigned ids.
        public static string GetUnAssignedPublishProductList() => $"{ApiRoot}/publishproduct/unassignedlist";

        //Send Compare Product Mail.
        public static string SendComparedProductMail() => $"{ApiRoot}/webstoreproducts/sendcomparedproductmail";

        //Send Mail To friend.
        public static string SendMailToFriend() => $"{ApiRoot}/webstoreproducts/sendmailtofriend";

        //Get price for products through ajax async call.
        public static string GetProductPrice() => $"{ApiRoot}/publishproduct/getproductprice";

        //Get Publish Product 
        public static string GetPublishProductForSiteMap() => $"{ApiRoot}/publishproduct/GetPublishProductForSiteMap";

        public static string GetActiveProducts(string parentIds, int catalogId, int localeId, int versionId) => $"{ApiRoot}/product/getActiveProducts/{parentIds}/{catalogId}/{localeId}/{versionId}";

        //Get product inventory
        public static string GetProductInventory(int publishProductId) => $"{ApiRoot}/publishproduct/getinventory/{publishProductId}";

        //Gets the list of products according to parameter model.
        public static string GetQuickOrderProductList() => $"{ApiRoot}/quickorder/getquickorderproductlist";

        //This method return random quick order product basic details
        public static string GetDummyQuickOrderProductList() => $"{ApiRoot}/quickorder/getdummyquickorderproductlist";

        // Get associated configurable product variants.
        public static string GetAssociatedConfigurableVariants(int productId) => $"{ApiRoot}/publishproduct/getassociatedconfigurablevariants/{productId}";

        // Submit Stock Request.
        public static string SubmitStockRequest() => $"{ApiRoot}/publishproduct/submitstockrequest";

        // Send stock notification.
        public static string SendStockNotice() => $"{ApiRoot}/publishproduct/sendstocknotification";

    }
}
