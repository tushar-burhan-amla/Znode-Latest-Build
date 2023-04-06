using Newtonsoft.Json;

using System;
using System.Configuration;
using System.Linq;
using System.Web;

using Znode.Libraries.Abstract.Models.Responses;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Framework.Business;

namespace Znode.Libraries.Abstract.Api
{
    public class DomainConfig
    {
        #region Private Variables
        private readonly IZnodeRepository<ZnodeDomain> _domainRepository;
        private readonly IZnodeRepository<ZnodePortal> _portalRepository;
        #endregion

        public DomainConfig()
        {
            _domainRepository = new ZnodeRepository<ZnodeDomain>();
            _portalRepository = new ZnodeRepository<ZnodePortal>();
        }

        #region Public Methods
        /// <summary>
        /// Get domain by route uri for domain configuration on application startup
        /// </summary>
        /// <param name="routeUri">routeUri</param>
        /// <returns>ZnodeDomain</returns>
        public virtual ZnodeDomain GetDomain(string routeUri)
        {
            //Read domain from cache
            ZnodeDomain data = (ZnodeDomain)HttpRuntime.Cache.Get(routeUri);

            if (Equals(data, null))
            {
                ZnodeDomain domain = GetDomainFromRepo(routeUri);
                if (!Equals(domain, null))
                    HttpRuntime.Cache.Insert(routeUri, domain, null, System.Web.Caching.Cache.NoAbsoluteExpiration, TimeSpan.FromSeconds(20));
                else
                    domain = new ZnodeDomain();
                return domain;
            }
            return data;
        }

        /// <summary>
        /// Get portal by portal id for site config on application startup
        /// </summary>
        /// <param name="portalId">portalId</param>
        /// <returns>ZnodePortal</returns>
        public ZnodePortal GetPortal(int portalId)
       => _portalRepository.GetById(portalId);




        public void SetSiteConfig()
        {
            var domainName = GetDomainNameFromAuthHeader();

            //Set Domain Config & Site Config details based on domain name.
            SetDomainAndSiteConfigDetails(domainName);

            //Set the API Domain & Site Configuration.
            SetAPISiteConfig();
        }
       

        #endregion

        #region Protected Methods
        /// <summary>
        /// Read domain from table
        /// </summary>
        /// <param name="domainName">domainName</param>
        /// <returns>ZnodeDomain</returns>
        protected ZnodeDomain GetDomainFromRepo(string domainName)
          => _domainRepository.Table.Where(x => Equals(x.DomainName, domainName.ToLower()))?.FirstOrDefault();
        #endregion

        #region Private Methods
        private string GetDomainNameFromAuthHeader()
        {
            var headers = HttpContext.Current.Request.Headers;

            const string domainHeader = "Znode-DomainName";

            string domain = string.IsNullOrEmpty(headers[domainHeader]) ? HttpContext.Current.Request.ServerVariables.Get("HTTP_HOST").Trim() : headers[domainHeader].ToString();

            return domain;
        }
        private void SetDomainAndSiteConfigDetails(string domainName)
        {
            if (!String.IsNullOrEmpty(domainName))
            {
                if (!ZnodeConfigManager.CheckSiteConfigCache(domainName) || ZnodeConfigManager.GetDomainConfig(domainName) == null)
                {
                   
                    var domainConfig = GetDomain(domainName);

                    if (Equals(domainConfig, null) || !domainConfig.IsActive)
                    {
                        ZnodeLogging.LogMessage($"Domain {domainName} has not been configured to work with Znode.", string.Empty, System.Diagnostics.TraceLevel.Warning);

                        // The URL was not found in our config, send out a 404 error
                        HttpContext.Current.Response.StatusCode = 404;

                        BaseResponse responseBody = new BaseResponse
                        {
                            ErrorCode = 8001,
                            ErrorMessage = "This domain has not been configured to work with Znode.",
                            HasError = true
                        };
                        string _response = JsonConvert.SerializeObject(responseBody, Newtonsoft.Json.Formatting.Indented);
                        HttpContext.Current.Response.Write(_response);
                        HttpContext.Current.ApplicationInstance.CompleteRequest();
                        return;
                    }

                    ZnodeConfigManager.SetDomainConfig(domainName, domainConfig);

                    //Set below call from Portal Cache.
                    var _portal = GetPortal(domainConfig.PortalId);

                    ZnodeConfigManager.SetSiteConfig(domainName, _portal);
                }
            }
        }

        private void SetAPISiteConfig()
        {
            var domainName = GetAPIDomainName();

            //Set Domain Config & Site Config details based on domain name.
            SetDomainAndSiteConfigDetails(domainName);
        }

        private string GetAPIDomainName()
           => Convert.ToBoolean(ValidateAuthHeader) ? HttpContext.Current.Request.ServerVariables.Get("HTTP_HOST").Trim() : string.Empty;
        private static string ValidateAuthHeader
        {
            get
            {
                return Convert.ToString(ConfigurationManager.AppSettings["ValidateAuthHeader"]);
            }
        }
        #endregion

    }
}
