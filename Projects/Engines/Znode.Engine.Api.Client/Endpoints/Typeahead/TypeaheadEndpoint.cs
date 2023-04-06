namespace Znode.Engine.Api.Client.Endpoints
{
    public class TypeaheadEndpoint : BaseEndpoint
    {
        //Endpoint to get the list.
        public static string GetTypeaheadResponse() => $"{ApiRoot}/typeahead/gettypeaheadresponse";
    }
}
