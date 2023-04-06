namespace Znode.Engine.Api.Client.Endpoints
{
    public class CMSWidgetsEndpoint : BaseEndpoint
    {
        //Get CMS widgets List Endpoint
        public static string List() => $"{ApiRoot}/cmswidgets/list";

        //Get CMS widget by code Endpoint
        public static string GetWidgetByCodes() => $"{ApiRoot}/cmswidgets/getwidgetbycode";
    }
}
