namespace Znode.Engine.Api.Client.Endpoints
{
    public class UrlRedirectEndpoint : BaseEndpoint
    {
        //Create Url Redirect Endpoint.
        public static string Create() => $"{ApiRoot}/urlredirect/create";

        //Get Url Redirect List Endpoint.
        public static string GetUrlRedirectList() => $"{ApiRoot}/urlredirect/geturlredirectlist";

        //Get Url Redirect on the basis of Filter Endpoint.
        public static string GetUrlRedirect() => $"{ApiRoot}/urlredirect/geturlredirect";

        //Update  Url Redirect Endpoint.
        public static string Update() => $"{ApiRoot}/urlredirect/update";

        //Delete Url Redirect Endpoint.
        public static string Delete() => $"{ApiRoot}/urlredirect/delete";
    }
}
