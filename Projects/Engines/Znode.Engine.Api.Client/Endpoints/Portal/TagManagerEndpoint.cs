namespace Znode.Engine.Api.Client.Endpoints
{
    public class TagManagerEndpoint : BaseEndpoint
    {     
        //Get tag manager data for store by portal id.
        public static string GetTagManager(int portalId) => $"{ApiRoot}/tagmanager/get/{portalId}";

        //Save tag manager data for store.
        public static string SaveTagManager() => $"{ApiRoot}/tagmanager/save";
    }
}
