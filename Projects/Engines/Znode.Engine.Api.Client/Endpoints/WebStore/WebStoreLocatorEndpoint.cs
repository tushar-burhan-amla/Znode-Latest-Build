namespace Znode.Engine.Api.Client.Endpoints
{
    public class WebStoreLocatorEndpoint : BaseEndpoint
    {
        //Endpoint to get details of store locator by postal code,state and city.
        public static string List() => $"{ApiRoot}/webstorelocator/list";
    }
}
