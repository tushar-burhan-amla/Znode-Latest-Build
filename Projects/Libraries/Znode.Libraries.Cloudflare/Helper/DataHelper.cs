using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Data.Helpers;

namespace Znode.Libraries.Cloudflare
{
    public class DataHelper
    {
        private readonly IZnodeRepository<ZnodeDomain> _domainRepository;
        private readonly IZnodeRepository<ZnodeGlobalAttribute> _globalAttribute;
        private readonly IZnodeRepository<ZnodePortalGlobalAttributeValue> _portalGlobalAttributeValue;
        private readonly IZnodeRepository<ZnodePortalGlobalAttributeValueLocale> _portalGlobalAttributeValueLocale;
        private readonly IZnodeRepository<ZnodePortal> _portalRepository;

        public DataHelper()
        {
            IDBContext dbContext = HelperMethods.GetNewDBContext();
            _domainRepository = new ZnodeRepository<ZnodeDomain>(dbContext);
            _globalAttribute = new ZnodeRepository<ZnodeGlobalAttribute>(dbContext);
            _portalGlobalAttributeValue = new ZnodeRepository<ZnodePortalGlobalAttributeValue>(dbContext);
            _portalGlobalAttributeValueLocale = new ZnodeRepository<ZnodePortalGlobalAttributeValueLocale>(dbContext);
            _portalRepository = new ZnodeRepository<ZnodePortal>(dbContext);
        }

        #region Public Methods
        /// <summary>
        /// To get domain names of portals whose cloudflare is enabled
        /// </summary>
        /// <param name="domainIds">domain Ids whose domain name needs to retrive</param>
        /// <returns></returns>
        public List<DomainAndZoneModel> GetActiveDomainsAndZone(string domainIds)
        {
            try
            {
                List<CloudflareDomainModel> domains = GetActiveDomains();
                List<DomainAndZoneModel> domainList = domains.Where(x => domainIds.Split(',').ToList().Contains(Convert.ToString(x.DomainId))).Select(z => new DomainAndZoneModel() { DomainId = z.DomainId, DomainName = z.DomainName, zoneId = z.ZoneId }).ToList();
                return domainList;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Znode.Libraries.Cloudflare.DataHelper -> GetActiveDomainsAndZone error -" + ex, "Cloudflare", TraceLevel.Error);
                return new List<DomainAndZoneModel>();
            }
        }

        /// <summary>
        /// To get domain names of portals whose cloudflare is enabled
        /// </summary>
        /// <param name="domainIds">domain Ids whose domain name needs to retrive</param>
        /// <returns></returns>
        public List<string> GetActiveDomains(string domainIds)
        {
            try
            {
                List<CloudflareDomainModel> domains = GetActiveDomains();
                List<string> domainList = domains.Where(x => domainIds.Split(',').ToList().Contains(Convert.ToString(x.DomainId))).Select(z => z.DomainName).ToList();
                return domainList;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Znode.Libraries.Cloudflare.DataHelper -> GetActiveDomains error -" + ex, "Cloudflare", TraceLevel.Error);
                return new List<string>();
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
                List<CloudflareDomainModel> cloudflareDomains = GetActiveDomains();
                List<DomainModel> Domains = new List<DomainModel>();
                if (cloudflareDomains != null)
                {
                    Domains = cloudflareDomains.Select(x => new DomainModel { ApiKey = x.ApiKey, DomainId = x.DomainId, DomainName = x.DomainName, IsActive = x.IsActive, IsDefault = x.IsDefault, PortalId = x.PortalId, ApplicationType = x.ApplicationType, StoreName = x.StoreName, CloudflareZoneId = x.ZoneId }).ToList();
                }
                return Domains;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Znode.Libraries.Cloudflare.DataHelper -> GetCloudflareEnableDomains error -" + ex, "Cloudflare", TraceLevel.Error);
                return new List<DomainModel>();
            }
        }

        /// <summary>
        /// To get domain list which enabled cloudflare by portalId.
        /// </summary>
        /// <returns></returns>
        public string GetCloudflareEnableDomainsByPortalId(string portalId)
        {
            try
            {
                return string.Join(", ", GetCloudflareEnableDomains().Where(x => portalId.Split(',').ToList().Contains(Convert.ToString(x.PortalId))).Select(x => x.DomainId).ToArray());
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Znode.Libraries.Cloudflare.DataHelper -> GetCloudflareEnableDomains error -" + ex, "Cloudflare", TraceLevel.Error);
                return string.Empty;
            }
        }

        /// <summary>
        /// Insert cache refresh time into database after refreshing respective cache
        /// </summary>
        /// <param name="cacheType"></param>
        public void UpdateCloudflareCacheRefreshTime(string cacheType)
        {
            IZnodeRepository<ZnodeApplicationCache> _applicationCacheRepository = new ZnodeRepository<ZnodeApplicationCache>();
            ZnodeApplicationCache znodeApplicationCache = _applicationCacheRepository.Table.FirstOrDefault(x => x.ApplicationType == cacheType);
            if (HelperUtility.IsNotNull(znodeApplicationCache))
            {
                znodeApplicationCache.StartDate = DateTime.Now;
                ZnodeLogging.LogMessage(_applicationCacheRepository.Update(znodeApplicationCache) ? "Cache successfully updated." : "Failed to update cache.", string.Empty, TraceLevel.Info);
            }
        }
        #endregion Public Methods

        #region Private Method
        private List<CloudflareDomainModel> GetActiveDomains()
        {
            try
            {
                //To get the list of first active domain for specified portals having ApplicationType as WebStore or WebstorePreview and Cloudflare is enabled.
                List<CloudflareDomainModel> domains = (from domain in _domainRepository.Table
                                                       join portal in _portalRepository.Table on domain.PortalId equals portal.PortalId
                                                       join portalGlobalAttributeValue in _portalGlobalAttributeValue.Table on domain.PortalId equals portalGlobalAttributeValue.PortalId
                                                       join globalAttribute in _globalAttribute.Table on portalGlobalAttributeValue.GlobalAttributeId equals globalAttribute.GlobalAttributeId
                                                       join portalGlobalAttributeValueLocale in _portalGlobalAttributeValueLocale.Table on portalGlobalAttributeValue.PortalGlobalAttributeValueId equals portalGlobalAttributeValueLocale.PortalGlobalAttributeValueId
                                                       where portalGlobalAttributeValueLocale.AttributeValue == "true"
                                                       && domain.IsActive && globalAttribute.AttributeCode == ZnodeCloudflareConstant.IsCloudflareEnabled
                                                       && (domain.ApplicationType == ZnodeCloudflareConstant.WebStore || domain.ApplicationType == ZnodeCloudflareConstant.WebstorePreview)
                                                       select new CloudflareDomainModel
                                                       {
                                                           ApiKey = domain.ApiKey,
                                                           DomainId = domain.DomainId,
                                                           DomainName = domain.DomainName,
                                                           IsActive = domain.IsActive,
                                                           IsDefault = domain.IsDefault,
                                                           PortalId = domain.PortalId,
                                                           ApplicationType = domain.ApplicationType,
                                                           StoreName = portal.StoreName,
                                                           GlobalAttributeId = globalAttribute.GlobalAttributeId,
                                                           AttributeCode = globalAttribute.AttributeCode,
                                                           AttributeValue = portalGlobalAttributeValue.AttributeValue
                                                       }).ToList();

                //To get the list of first active domain and zoneId for specified portals.
                domains = (from domain in domains
                           join portalGlobalAttributeValue in _portalGlobalAttributeValue.Table on domain.PortalId equals portalGlobalAttributeValue.PortalId
                           join globalAttribute in _globalAttribute.Table on portalGlobalAttributeValue.GlobalAttributeId equals globalAttribute.GlobalAttributeId
                           join portalGlobalAttributeValueLocale in _portalGlobalAttributeValueLocale.Table on portalGlobalAttributeValue.PortalGlobalAttributeValueId equals portalGlobalAttributeValueLocale.PortalGlobalAttributeValueId
                           where domain.IsActive && globalAttribute.AttributeCode == ZnodeCloudflareConstant.CloudflareZoneId
                           select new CloudflareDomainModel
                           {
                               ApiKey = domain.ApiKey,
                               DomainId = domain.DomainId,
                               DomainName = domain.DomainName,
                               IsActive = domain.IsActive,
                               IsDefault = domain.IsDefault,
                               PortalId = domain.PortalId,
                               ApplicationType = domain.ApplicationType,
                               StoreName = domain.StoreName,
                               GlobalAttributeId = domain.GlobalAttributeId,
                               AttributeCode = domain.AttributeCode,
                               AttributeValue = domain.AttributeValue,
                               ZoneId = portalGlobalAttributeValueLocale.AttributeValue
                           }).ToList();

                return domains;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Znode.Libraries.Cloudflare.DataHelper -> GetActiveDomains error -" + ex, "Cloudflare", TraceLevel.Error);
                return new List<CloudflareDomainModel>();
            }
        }
        #endregion
    }
}
