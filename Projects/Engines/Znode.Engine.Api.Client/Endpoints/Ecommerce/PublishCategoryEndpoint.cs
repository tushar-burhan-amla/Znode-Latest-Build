namespace Znode.Engine.Api.Client.Endpoints
{
    public class PublishCategoryEndpoint : BaseEndpoint
    {
        //Get publish Categories list 
        public static string GetPublishCategoryList() => $"{ApiRoot}/publishcategory/list";

        //Get publish Category 
        public static string GetPublishCategory(int publishCategoryId) => $"{ApiRoot}/publishcategory/get/{publishCategoryId}";

        //Get publish category excluding assigned ids.
        public static string GetUnAssignedPublishCategoryList(string assignedIds) => $"{ApiRoot}/publishcategory/unassignedlist/{assignedIds}";

    }
}
