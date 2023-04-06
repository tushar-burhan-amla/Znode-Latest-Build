namespace Znode.Engine.Api.Client.Endpoints
{
    public class PortalUnitEndpoint : BaseEndpoint
    {
        //Get PortalUnit Endpoint
        public static string Get(int portalId) => $"{ApiRoot}/portalunit/Get/{portalId}";

        //Update PortalUnit Endpoint
        public static string Update() => $"{ApiRoot}/portalunit/Update";
    }
}
