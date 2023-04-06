namespace Znode.Libraries.Cloudflare.API
{
    public interface ICloudflareRequest
    {
        /// <summary>
        /// Purge everything from cloudflare.
        /// </summary>
        /// <param name="zoneId">ZoneId which will send to Cloudflare URL</param>
        /// <returns>PurgeResponseModel</returns>
        PurgeResponseModel PurgeEverything(string zoneId);

        /// <summary>
        /// Purge cloudflare cache by Host Name.
        /// </summary>
        /// <param name="hostNames">List of hostnames which needs to be purge</param>
        /// <param name="zoneId">ZoneId which will send to Cloudflare URL</param>
        /// <returns>PurgeResponseModel</returns>
        PurgeResponseModel PurgeByHostName(HostListModel hostNames, string zoneId);

        /// <summary>
        /// Purge cloudflare cache by URLs.
        /// </summary>
        /// <param name="filesList">List of URLs which needs to be purge</param>
        /// <param name="zoneId">ZoneId which will send to Cloudflare URL</param>
        /// <returns>PurgeResponseModel</returns>
        PurgeResponseModel PurgeByURLs(FilesListModel filesList, string zoneId);
    }
}
