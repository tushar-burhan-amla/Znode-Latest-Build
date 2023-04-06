namespace Znode.Engine.Api.Client.Endpoints
{
    public class HelperEndpoint : BaseEndpoint
    {
        //Check CodeFieldExists  
        public static string CheckCodeExists(string service, string methodName) => $"{ApiRoot}/helper/isCodeExists/{service}/{methodName}";
    }
}
