namespace Znode.Engine.Api.Client.Endpoints
{
    public class CatalogHistoryEndpoint : BaseEndpoint
    {
        public static string GetCatalogHistories() => $"{ApiRoot}/cataloghistory/list";

        public static string GetCatalogHistory(int id) => $"{ApiRoot}/cataloghistory/{id}";

        public static string CreateCatalogHistory() => $"{ApiRoot}/cataloghistory";

        public static string UpdateCatalogHistory() => $"{ApiRoot}/cataloghistory/update";

        public static string DeleteCatalogHistory(int id) => $"{ApiRoot}/cataloghistory/delete/{id}";
    }
}
