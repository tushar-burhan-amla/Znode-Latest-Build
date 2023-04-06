using System.Collections.Generic;
using Znode.Engine.Api.Models;

namespace Znode.Libraries.Cloudflare
{
    public interface IZnodeCloudflare
    {
        /// <summary>
        /// Purge everything from cloudflare for provided domain ids
        /// </summary>
        /// <param name="domainIds"></param>
        /// <returns></returns>
        List<CloudflareErrorResponseModel> PurgeEverything(string domainIds);

        /// <summary>
        /// Purge everything from cloudflare for provided domain names
        /// </summary>
        /// <param name="domainIds"></param>
        /// <returns></returns>
        bool PurgeByHosts(string domainIds);

        /// <summary>
        /// Purge cache from Cloudflare for specified URLs
        /// </summary>
        /// <param name="URLs"></param>
        /// <returns></returns>
        bool PurgeURLs(List<URLandZoneId> URLs);

        /// <summary>
        /// Purge everything from cloudflare for provided Portal Id
        /// </summary>
        /// <param name="portalId"></param>
        /// <returns></returns>
        List<CloudflareErrorResponseModel> PurgeEverythingByPortalId(string portalId);

        /// <summary>
        /// To get domain list which portal have cloudflare enabled.
        /// </summary>
        /// <returns></returns>
        List<DomainModel> GetCloudflareEnableDomains();

        /// <summary>
        /// Purge cache from cloudflare for provided Portal Ids and URLs
        /// </summary>
        /// <returns></returns>
        bool PurgeUrlContentsByPortalId(List<int> portalId, List<string> urlsWithoutDomain);
    }
}
