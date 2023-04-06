using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Cache
{
    public class WebStorePortalCache : BaseCache, IWebStorePortalCache
    {
        #region Private Variables
        private readonly IPortalService _portalService;
        #endregion

        #region Constructor
        public WebStorePortalCache(IPortalService portalService)
        {
            _portalService = portalService;
        }
        #endregion

        #region Public Methods
        //Get Portal information by PortalId.
        public virtual string GetPortal(int portalId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get data from WebStorePortal Service.
                WebStorePortalModel portalModel = _portalService.WebStoreGetPortal(portalId, Expands);
                if (!Equals(portalModel, null))
                {
                    //Create Response and insert in to Cache.
                    WebStorePortalResponse response = new WebStorePortalResponse { Portal = portalModel };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        
        public virtual string GetPortal(int portalId, int localeId, ApplicationTypesEnum applicationType, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get data from WebStorePortal Service.
                WebStorePortalModel portalModel = _portalService.WebStoreGetPortal(portalId, localeId, applicationType, Expands);
                if (!Equals(portalModel, null))
                {
                    //Create Response and insert in to Cache.
                    WebStorePortalResponse response = new WebStorePortalResponse { Portal = portalModel };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetPortal(string domainName, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get data from WebStorePortal Service.
                WebStorePortalModel portalModel = _portalService.WebStoreGetPortal(domainName, Expands);
                if (!Equals(portalModel, null))
                {
                    //Create Response and insert in to Cache.
                    WebStorePortalResponse response = new WebStorePortalResponse { Portal = portalModel };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion
    }
}