namespace Znode.Engine.Api.Client.Endpoints
{
    public class TokenEndpoint : BaseEndpoint
    {
        public static string GenerateToken(string key) => $"{ApiRoot}/token/generatetoken/{key}";

        public static string GenerateToken() => $"{ApiRoot}/token/generatetoken";
    }
}
