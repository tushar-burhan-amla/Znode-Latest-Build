namespace Znode.Libraries.Cloudflare.API.Endpoints
{
    public class CloudflareEndpoints
    {
        public static string CloudflareRoot => ZnodeCloudflareSetting.CloudflareUrl;

        //Set Cloudflare URL 
        public static string CloudflareEndpoint(string zoneId) => $"{CloudflareRoot}/zones/{zoneId}/purge_cache";
    }
}
