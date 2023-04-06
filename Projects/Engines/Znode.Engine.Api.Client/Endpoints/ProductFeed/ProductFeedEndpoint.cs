
namespace Znode.Engine.Api.Client.Endpoints
{
    public class ProductFeedEndpoint : BaseEndpoint
    {
        //Create product feed.
        public static string Create() => $"{ApiRoot}/productfeed";

        //Get product feed list endpoint
        public static string GetProductFeedList() => $"{ApiRoot}/productfeed/list";

        //Get product feed by product feed id endpoint.
        public static string GetProductFeedById(int productFeedId) => $"{ApiRoot}/productfeed/getproductfeed/{productFeedId}";

        //Update product feed endpoint.
        public static string Update() => $"{ApiRoot}/productfeed/update";

        //Endpoint to delete product feed by id.
        public static string DeleteProductFeed() => $"{ApiRoot}/productfeed/delete";

        //Get product feed master details.
        public static string GetProductFeedMasterDetails() => $"{ApiRoot}/productfeed/getproductfeedmasterdetails";

        //Get product feed by portal Id endpoint.
        public static string GetProductFeedByPortalId(int portalId) => $"{ApiRoot}/productfeed/getproductfeedbyportalid/{portalId}";

        //Check if the file name combination already exists.
        public static string FileNameCombinationAlreadyExist(int localeId, string fileName) => $"{ApiRoot}/productfeed/fileNamecombinationalreadyexist/{localeId}/{fileName}";
    }
}
