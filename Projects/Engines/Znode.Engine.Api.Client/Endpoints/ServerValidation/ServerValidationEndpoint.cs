namespace Znode.Engine.Api.Client.Endpoints
{
    public class ServerValidationEndpoint : BaseEndpoint
    {
        //Validate Server side endpoint
        public static string ValidateDataAtServerSide() => $"{ApiRoot}/valid";
    }
}
