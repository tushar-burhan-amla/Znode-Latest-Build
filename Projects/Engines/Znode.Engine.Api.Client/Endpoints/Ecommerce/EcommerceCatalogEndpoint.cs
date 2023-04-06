namespace Znode.Engine.Api.Client.Endpoints
{
    public class EcommerceCatalogEndpoint : BaseEndpoint
    {
        //Get PublishCatalogList List Endpoint
        public static string GetPublishCatalogList() => $"{ApiRoot}/ecommercecatalog/getpublishcataloglist";

        //Get AssociatedPortalCatalog List Endpoint
        public static string GetAssociatedPortalCatalogByPortalId(int portalId) => $"{ApiRoot}/ecommercecatalog/getassociatedportalcatalogbyportalid/{portalId}";

        //Get Associated Catalogs as per PortalIds List Endpoint
        public static string GetAssociatedPortalCatalog() => $"{ApiRoot}/ecommercecatalog/getassociatedportalcatalog";

        //Get PortalCatalog Endpoint
        public static string GetPortalCatalog(int portalCatalogId) => $"{ApiRoot}/ecommercecatalog/getportalcatalog/{portalCatalogId}";

        //Update PortalCatalog Endpoint
        public static string UpdatePortalCatalog() => $"{ApiRoot}/ecommercecatalog/updateportalcatalog";

        //Get Catalog Tree structure
        public static string GetCatalogTree(int catalogId, int categoryId) => $"{ApiRoot}/ecommercecatalog/getcatalogtree/{catalogId}/{categoryId}";

        //Get Publish Catalog Details
        public static string GetPublishCatalogDetails(int publishCatalogId) => $"{ApiRoot}/ecommercecatalog/getpublishcatalogdetails/{publishCatalogId}";

        //Get Publish Category Details
        public static string GetPublishCategoryDetails(int publishCategoryId) => $"{ApiRoot}/ecommercecatalog/getpublishcategorydetails/{publishCategoryId}";

        //Get Publish Product Details
        public static string GetPublishProductDetails(int publishProductId, int portalId) => $"{ApiRoot}/ecommercecatalog/getpublishproductdetails/{publishProductId}/{portalId}";

    }
}
