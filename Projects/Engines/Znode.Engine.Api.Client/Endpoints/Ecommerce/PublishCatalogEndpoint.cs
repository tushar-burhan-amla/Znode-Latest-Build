using Znode.Libraries.ECommerce.Utilities;
namespace Znode.Engine.Api.Client.Endpoints
{
    public class PublishCatalogEndpoint : BaseEndpoint
    {
        //Get publish Catalog list 
        public static string GetPublishCatalogList() => $"{ApiRoot}/publishcatalog/list";

        //Get publish Catalog 
        public static string GetPublishCatalog(int publishCatalogId, int? localeId)
            => HelperUtility.IsNotNull(localeId) ? $"{ApiRoot}/publishcatalog/get/{publishCatalogId}/{localeId}"
            : $"{ApiRoot}/publishcatalog/get/{publishCatalogId}";

        //Get publish catelog excluding assigned ids.
        public static string GetUnAssignedPublishCatelogList(string assignedIds) => $"{ApiRoot}/publishcatalog/unassignedlist/{assignedIds}";

    }
}
