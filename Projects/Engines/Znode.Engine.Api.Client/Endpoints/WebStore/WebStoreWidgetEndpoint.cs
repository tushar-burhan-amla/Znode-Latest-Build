namespace Znode.Engine.Api.Client.Endpoints
{
    public class WebStoreWidgetEndpoint : BaseEndpoint
    {
        //Get widget slider details.
        public static string GetSlider(string key) => $"{ApiRoot}/webstorewidget/getslider/{key}";

        //Get widget product list details.
        public static string GetProducts(string key) => $"{ApiRoot}/webstorewidget/getproducts/{key}";

        //Get link widget details.
        public static string GetLinkWidget(string key) => $"{ApiRoot}/webstorewidget/getlinkwidget/{key}";

        //Get widget category list details.
        public static string GetCategories(string key) => $"{ApiRoot}/webstorewidget/getcategories/{key}";

        //Get widget product list details.
        public static string GetLinkProductList(string key) => $"{ApiRoot}/webstorewidget/getlinkproductlist/{key}";

        //get tag manager data
        public static string GetTagManager(string key) => $"{ApiRoot}/webstorewidget/gettagmanager/{key}";

        //get Media Widget Details
        public static string  GetMediaWidgetDetails(string key) => $"{ApiRoot}/webstorewidget/getmediawidgetdetails/{key}";

        //Get widget brand list details.
        public static string GetBrands(string key) => $"{ApiRoot}/webstorewidget/getbrands/{key}";

        // Get form builder attribute group list endpoint.
        public static string GetFormConfiguration() => $"{ApiRoot}/webstorewidget/getformconfigurationbycmsmappingid";

        //Get search widget products ad facets.
        public static string GetSearchWidgetData() => $"{ApiRoot}/webstorewidget/getsearchwidgetdata";

        //Get Available ECertificate balance
        public static string GetECertTotalBalance() => $"{ApiRoot}/ecert/GetAvailableECertBalance";

        //Get widget container details.
        public static string GetContainer() => $"{ApiRoot}/webstorewidget/getcontainer";
    }
}
