

namespace Znode.Engine.Api.Client.Endpoints
{
    public class PublishBrandEndpoint : BaseEndpoint
    {
        //Get Publish brand List.
        public static string GetPublishBrandList() => $"{ApiRoot}/publishbrand/getpublishbrandlist";

        //Get Publish brand.
        public static string GetPublishBrand(int brandId, int localeId, int portalId) => $"{ApiRoot}/publishbrand/getpublishbrand/{brandId}/{localeId}/{portalId}";
    }
}
