namespace Znode.Engine.Api.Client.Endpoints
{
    public class WebSiteEndpoint : BaseEndpoint
    {
        //Get Portal list.
        public static string GetPortalList() => $"{ApiRoot}/websitelogo/portallist";

        //Get WebSite Logo Endpoint
        public static string GetLogo(int portalId) => $"{ApiRoot}/websitelogo/{portalId}";

        //Save Web Site Logo Endpoint
        public static string SaveWebSiteLogo() => $"{ApiRoot}/websitelogo/savewebsitelogo";

        //Get Portal Product Page List Endpoint.
        public static string GetPortalProductPage(int portalId) => $"{ApiRoot}/website/getportalproductpagelist/{portalId}";

        //Assign new PDP template to product type Endpoint.
        public static string UpdatePortalProductPage() => $"{ApiRoot}/website/updateportalproductpage";

        // Publish CMS configuration
        public static string Publish(int portalId) => $"{ApiRoot}/publish/{portalId}";

        public static string Publish(int portalId, string targetPublishState = null, string publishContent = null, bool takeFromDraftFirst = false) => $"{ApiRoot}/PublishWithPreview/{portalId}/{targetPublishState}/{publishContent}/{takeFromDraftFirst}";

        //Get widget id by its code.
        public static string GetWidgetIdByCode(string widgetCode) => $"{ApiRoot}/website/{widgetCode}";

        public static string GetAssociatedCatalogId(int portalId) => $"{ApiRoot}/getassociatedcatalog/{portalId}";
    }
}
