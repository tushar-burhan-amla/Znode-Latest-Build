namespace Znode.Engine.Api.Client.Endpoints
{ 
    public class CategoryHistoryEndpoint : BaseEndpoint
    {
        public static string GetCategoryHistories() => $"{ApiRoot}/categoryhistory/list";

        public static string GetCategoryHistory(int id) => $"{ApiRoot}/categoryhistory/{id}";

        public static string CreateCategoryHistory() => $"{ApiRoot}/categoryhistory";

        public static string UpdateCategoryHistory() => $"{ApiRoot}/categoryhistory/update";

        public static string DeleteCategoryHistory(int id) => $"{ApiRoot}/categoryhistory/delete/{id}";
    }
}
