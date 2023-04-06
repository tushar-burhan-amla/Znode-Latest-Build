namespace Znode.Engine.Api.Client.Endpoints
{
    public class PortalProfileEndpoint : BaseEndpoint
    {
        //Get PortalProfile List Endpoint
        public static string List() => $"{ApiRoot}/portalprofile/list";

        //Get PortalProfile Endpoint
        public static string Get(int portalProfileId) => $"{ApiRoot}/portalprofile/{portalProfileId}";

        //Create PortalProfile Endpoint
        public static string Create() => $"{ApiRoot}/portalprofile";

        //Update PortalProfile Endpoint
        public static string Update() => $"{ApiRoot}/portalprofile/update";

        //Delete PortalProfile Endpoint
        public static string Delete() => $"{ApiRoot}/portalprofile/delete";
        
    }
}
