
namespace Znode.Engine.Api.Client.Endpoints
{
    public class MaintenanceEndpoint : BaseEndpoint
    {
        //To delete published data of all catalog, store,cms and elastic search
        public static string PurgeAllPublishedData() => $"{ApiRoot}/maintenance/purgeallpublisheddata";
    }
}
