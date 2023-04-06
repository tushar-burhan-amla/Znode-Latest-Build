namespace Znode.Engine.Api.Client.Endpoints
{
    public class LicenseEndpoint : BaseEndpoint
    {
        public static string GetLicenseInformation() => $"{ApiRoot}/license/getlicenseinformation";
    }
}
