namespace Znode.Engine.Api.Client.Endpoints
{
    public class DefaultGlobalConfigEndpoint : BaseEndpoint
    {
        //Get default config list endpoint
        public static string GetDefaultConfigList() => $"{ApiRoot}/defaultglobalconfig";
        //Get default Logging setting list endpoint
        public static string GetLoggingConfigList() => $"{ApiRoot}/defaultLoggingconfig";

    }
}
