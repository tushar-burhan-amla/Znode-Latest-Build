namespace Znode.Engine.Api.Client.Endpoints
{
    public class ERPConnectorEndpoint : BaseEndpoint
    {
        //Get erpconnectorcontrols endpoint
        public static string GetERPConnectorControls() => $"{ApiRoot}/erpconnector/erpconnectorcontrols";

        //Save ERP Control Data in json file endpoint.
        public static string Create() => $"{ApiRoot}/erpconnector/createerpcontroldata";
    }
}
