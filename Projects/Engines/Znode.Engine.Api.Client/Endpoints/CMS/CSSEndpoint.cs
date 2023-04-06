namespace Znode.Engine.Api.Client.Endpoints
{
    public class CSSEndpoint : BaseEndpoint
    {
        //Get CSS List Endpoint
        public static string List() => $"{ApiRoot}/css/list";

        //Get CSS Endpoint
        public static string Get(int cssId) => $"{ApiRoot}/css/{cssId}";

        //Create CSS Endpoint
        public static string Create() => $"{ApiRoot}/css";

        //Update CSS Endpoint
        public static string Update() => $"{ApiRoot}/css";

        //Delete CSS Endpoint
        public static string Delete() => $"{ApiRoot}/css/delete";

        //Get CSS list by theme Id Endpoint
        public static string GetCssListByThemeId(int cmsThemeId) => $"{ApiRoot}/css/getcsslistbythemeid/{cmsThemeId}";
    }
}
