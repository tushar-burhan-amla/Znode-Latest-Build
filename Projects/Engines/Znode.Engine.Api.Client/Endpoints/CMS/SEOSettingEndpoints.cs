namespace Znode.Engine.Api.Client.Endpoints
{
    public class SEOSettingEndpoints : BaseEndpoint
    {
        //Get EmailTemplate List Endpoint
        public static string List() => $"{ApiRoot}/seodetails/list";

        public static string CreatePortalSEOSettings() => $"{ApiRoot}/seo/createdefaultportalsetting";

        public static string GetPortalSEOSettings(int portalId) => $"{ApiRoot}/seo/defaultportalsetting/{portalId}";

        public static string UpdatePortalSEOSettings() => $"{ApiRoot}/seo/updateportalseosetting";

        public static string GetPublishedProducts() => $"{ApiRoot}/seo/getpublishedproducts";

        public static string GetPublishedCategories() => $"{ApiRoot}/seo/getpublishedcategories";

        public static string CreateSEODetails() => $"{ApiRoot}/seo/createseodetails";

        public static string UpdateSEODetails() => $"{ApiRoot}/seo/updateseodetails";

        public static string GetSEODetails(int? itemId, int seoTypeId, int localeId, int portalId) => $"{ApiRoot}/seo/getseodetails/{itemId}/{seoTypeId}/{localeId}/{portalId}";
        // Publish Seo details endpoint.
        public static string Publish(string seoCode, int portalId, int localeId, int seoTypeId) => $"{ApiRoot}/seo/publish/{seoCode}/{portalId}/{localeId}/{seoTypeId}";
        // Publish Seo details endpoint.
        public static string PublishWithPreview(string seoCode, int portalId, int localeId, int seoTypeId, string targetPublishState, bool takeFromDraftFirst) => $"{ApiRoot}/seo/publishwithpreview/{seoCode}/{portalId}/{localeId}/{seoTypeId}/{targetPublishState}/{takeFromDraftFirst}/";
        public static string GetPublishSEODetail(int itemId, string seoType, int localeId, int portalId,string seoCode) => $"{ApiRoot}/seo/getpublishseodetails/{itemId}/{seoType}/{localeId}/{portalId}/{seoCode}";

        public static string GetSEODetailsBySEOCode(string seoCode, int seoTypeId, int localeId, int portalId) => $"{ApiRoot}/seo/getseodetailsbyseocode/{seoCode}/{seoTypeId}/{localeId}/{portalId}";
        // Get Product List for SEO
        public static string GetProductsForSEO() => $"{ApiRoot}/seo/getproductsforseo";

        //category list for SEO endpoint
        public static string GetCategoryListForSEO() => $"{ApiRoot}/seo/getcategorylistforseo";

        public static string GetDefaultSEODetails(string seoCode, int seoTypeId, int localeId, int portalId, int itemId) => $"{ApiRoot}/seo/getdefaultseodetails/{seoCode}/{seoTypeId}/{localeId}/{portalId}/{itemId}";

        //Delete Seo endpoint.
        public static string SeoDelete(int seoTypeId, int portalId, string seoCode) => $"{ApiRoot}/seo/DeleteSeoDetail/{seoTypeId}/{portalId}/{seoCode}";

    }
}
