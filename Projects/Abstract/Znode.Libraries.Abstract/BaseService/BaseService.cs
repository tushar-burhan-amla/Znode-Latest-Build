using System;
using System.Web;

using Znode.Libraries.Framework.Business;

namespace Znode.Libraries.Abstract.Services
{
    public abstract class  BaseService : ZnodeBusinessBase
    {
        #region Properties
        protected int PortalId
        {
            get
            {
                string domainName = GetPortalDomainName() ?? HttpContext.Current.Request.ServerVariables.Get("HTTP_HOST").Trim();
                return Convert.ToInt32(ZnodeConfigManager.GetSiteConfig(domainName)?.PortalId);
            }
        }

        #endregion

        //Get PortalId from SiteConfig
        public static int GetPortalId()
        {
            string domainName = HttpContext.Current.Request.ServerVariables.Get("HTTP_HOST")?.Trim() ?? GetPortalDomainName();
            return ZnodeConfigManager.GetSiteConfig(domainName)?.PortalId ?? 0;
        }

        //Get Domain Name by Request Headers
        public static string GetPortalDomainName()
        {
            const string headerDomainName = "Znode-DomainName";
            var headers = HttpContext.Current.Request.Headers;
            string domainName = headers[headerDomainName];
            return domainName;
        }

    }
}
