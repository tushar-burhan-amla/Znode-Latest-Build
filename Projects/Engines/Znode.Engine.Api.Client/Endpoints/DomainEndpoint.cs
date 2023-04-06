namespace Znode.Engine.Api.Client.Endpoints
{
    public class DomainEndpoint : BaseEndpoint
    {
        //Get Domain List Endpoint
        public static string List() => $"{ApiRoot}/domain";

        //Get Domain Endpoint
        public static string Get(int domainId) => $"{ApiRoot}/domain/{domainId}";

        //Create Domain Endpoint
        public static string Create() => $"{ApiRoot}/domain";

        //Update Domain Endpoint
        public static string Update() => $"{ApiRoot}/domain";

        //Delete Domain Endpoint
        public static string Delete() => $"{ApiRoot}/domain/delete";

        //Enable disable domain basis of IsActive status.
        public static string EnableDisableDomain() => $"{ApiRoot}/domain/enabledisabledomain";
    }
}

