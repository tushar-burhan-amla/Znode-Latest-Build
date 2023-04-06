namespace Znode.Engine.Api.Client.Endpoints
{
    public class AddressEndPoint : BaseEndpoint
    {
        public static string GetAddressList() => $"{ApiRoot}/address/list";
    }
}
