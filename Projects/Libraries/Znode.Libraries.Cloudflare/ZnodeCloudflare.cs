using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Libraries.Caching.Events;
using Znode.Libraries.Cloudflare.API;
using Znode.Libraries.Cloudflare.Helper;
using Znode.Libraries.Framework.Business;

namespace Znode.Libraries.Cloudflare
{
    public class ZnodeCloudflare : IZnodeCloudflare
    {
        private readonly DataHelper dataHelper;
        readonly ICloudflareRequest request;

        public ZnodeCloudflare(ICloudflareRequest cloudflareRequest, DataHelper data)
        {
            dataHelper = data;
            request = cloudflareRequest;
        }

        /// <summary>
        /// Purge everything from cloudflare for provided domain ids
        /// </summary>
        /// <param name="DomainIds"></param>
        /// <returns>List<CloudflareErrorResponseModel></returns>
        public List<CloudflareErrorResponseModel> PurgeEverything(string domainIds)
        {
            try
            {
                PurgeResponseModel purgeResponse;
                List<CloudflareErrorResponseModel> cloudflareResponseModel = new List<CloudflareErrorResponseModel>();
                List<DomainAndZoneModel> domainList = dataHelper.GetActiveDomainsAndZone(domainIds);
                if (domainList == null)
                {
                    cloudflareResponseModel.Add(new CloudflareErrorResponseModel() { DomainId = 0, Status = false, ErrorMessage = "Domain list is empty" });
                    return cloudflareResponseModel;
                }

                foreach (DomainAndZoneModel domains in domainList)
                {
                    try
                    {
                        ClearCacheCloudFlareHelper.EnqueueEviction(new CloudflarePurgeEverythingEvent()
                        {
                            Comment = $"From PurgeEverything with Zone Id '{domains.zoneId}'.",
                            ZoneId = domains.zoneId
                        });
                        ZnodeLogging.LogActivity(ZnodeCloudflareConstant.CloudflarePurgeAllActivityTypeId, "Full purge successful for Domain Id : " + domains.DomainId);
                    }
                    catch (Exception ex)
                    {
                        ZnodeLogging.LogActivity(ZnodeCloudflareConstant.CloudflarePurgeAllActivityTypeId, "Full purge failed for Domain Id : " + domains.DomainId);
                        ZnodeLogging.LogMessage("Znode.Libraries.Cloudflare.ZnodeCloudflare -> PurgeEverything -> domain" + domains.DomainName + " error -" + ex, "Cloudflare", TraceLevel.Error);
                    }
                }
                return cloudflareResponseModel;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Znode.Libraries.Cloudflare.ZnodeCloudflare.PurgeEverything error -" + ex, "Cloudflare", TraceLevel.Error);
                throw new ZnodeCloudflareException(null, ex.Message);
            }
        }

        /// <summary>
        /// Purge everything from cloudflare for provided Portal Id
        /// </summary>
        /// <param name="portalId"></param>
        /// <returns>List<CloudflareErrorResponseModel></returns>
        public List<CloudflareErrorResponseModel> PurgeEverythingByPortalId(string portalId)
        {
            try
            {
                if (string.IsNullOrEmpty(portalId)) return new List<CloudflareErrorResponseModel>();

                return PurgeEverything(dataHelper.GetCloudflareEnableDomainsByPortalId(portalId));
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Znode.Libraries.Cloudflare.ZnodeCloudflare.PurgeEverythingByPortalId error -" + ex, "Cloudflare", TraceLevel.Error);
                throw new ZnodeCloudflareException(null, ex.Message);
            }
        }

        /// <summary>
        /// Purge cache from cloudflare for provided Portal Ids and URLs
        /// </summary>
        /// <param name="portalId"></param>
        /// <param name="urlsWithoutDomain"></param>
        /// <returns>List<CloudflareErrorResponseModel></returns>
        public bool PurgeUrlContentsByPortalId(List<int> portalId, List<string> urlsWithoutDomain)
        {
            try
            {
                List<DomainModel> domainModels = dataHelper.GetCloudflareEnableDomains().Where(x => portalId.Contains(x.PortalId)).ToList();
                List<URLandZoneId> uRLandZones = new List<URLandZoneId>();
                foreach (DomainModel domainModel in domainModels)
                {
                    foreach (string url in urlsWithoutDomain)
                    {
                        uRLandZones.Add(new URLandZoneId() { URL = string.Concat(domainModel.DomainName, "/", url),zoneId = domainModel.CloudflareZoneId});
                    }
                }

                return PurgeURLs(uRLandZones);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Znode.Libraries.Cloudflare.ZnodeCloudflare.PurgeEverythingByPortalId error -" + ex, "Cloudflare", TraceLevel.Error);
                throw new ZnodeCloudflareException(null, ex.Message);
            }
        }

        /// <summary>
        /// Purge everything from cloudflare for provided domain names
        /// </summary>
        /// <param name="domainIds"></param>
        /// <returns></returns>
        public bool PurgeByHosts(string domainIds)
        {
            try
            {
                PurgeResponseModel purgeResponse = new PurgeResponseModel();
                List<DomainAndZoneModel> domainList = dataHelper.GetActiveDomainsAndZone(domainIds);
                if (domainList == null)
                {
                    throw new ZnodeCloudflareException(null, "No domain id available");
                }

                foreach (DomainAndZoneModel domains in domainList)
                {
                    HostListModel hostList = new HostListModel() { Hosts = new List<string>() { domains.DomainName } };
                    ClearCacheCloudFlareHelper.EnqueueEviction(new CloudflarePurgeByHostNameEvent()
                    {
                        Comment = $"From PurgeByHosts with Zone Id '{domains.zoneId}'.",
                        ZoneId = domains.zoneId,
                        Hosts = hostList
                    });
                    ZnodeLogging.LogActivity(ZnodeCloudflareConstant.CloudflarePurgeCustomActivityTypeId, "Purge successful for host : " + domains.DomainName);
                }
                return purgeResponse.Success;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Znode.Libraries.Cloudflare.ZnodeCloudflare.PurgeByHosts error -" + ex, "Cloudflare", TraceLevel.Error);
                throw new ZnodeCloudflareException(null, ex.Message);
            }
        }

        /// <summary>
        /// Purge cache from Cloudflare for specified URLs
        /// </summary>
        /// <param name="URLs"></param>
        /// <returns></returns>
        public bool PurgeURLs(List<URLandZoneId> URLs)
        {
            try
            {
                PurgeResponseModel purgeResponse = new PurgeResponseModel();
                if (URLs == null || URLs.Count < 0) return false;
                foreach (URLandZoneId uRLandZone in URLs)
                {
                    FilesListModel fileList = new FilesListModel() { Files = new List<string>() { uRLandZone.URL } };
                    ClearCacheCloudFlareHelper.EnqueueEviction(new CloudflarePurgeByURLsEvent()
                    {
                        Comment = $"From PurgeByHosts with Zone Id '{uRLandZone.zoneId}'.",
                        ZoneId = uRLandZone.zoneId,
                        Files = fileList
                    });
                    ZnodeLogging.LogActivity(ZnodeCloudflareConstant.CloudflarePurgeCustomActivityTypeId, "Purge successful for URL : " + uRLandZone.URL);
                }
                return purgeResponse.Success;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Znode.Libraries.Cloudflare.ZnodeCloudflare.PurgeURLs error -" + ex, "Cloudflare", TraceLevel.Error);
                throw new ZnodeCloudflareException(null, ex.Message);
            }
        }

        /// <summary>
        /// To get domain list which portal have cloudflare enabled.
        /// </summary>
        /// <returns></returns>
        public List<DomainModel> GetCloudflareEnableDomains()
        {
            try
            {
                return dataHelper.GetCloudflareEnableDomains();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Znode.Libraries.Cloudflare.ZnodeCloudflare -> GetCloudflareEnableDomains error -" + ex, "Cloudflare", TraceLevel.Error);
                return new List<DomainModel>();
            }
        }
    }
}
