namespace Znode.Engine.Api.Client.Endpoints
{
    public class SMTPEndpoint : BaseEndpoint
    {
        //Get SMTP Endpoint
        public static string Get(int portalId) => $"{ApiRoot}/smtp/Get/{portalId}";

        //Update SMTP Endpoint
        public static string Update() => $"{ApiRoot}/smtp/Update";

        public static string SendEmail() => $"{ApiRoot}/smtp/SendEmail";
    }
}
