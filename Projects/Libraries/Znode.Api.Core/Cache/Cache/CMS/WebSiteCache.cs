using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Api.Cache
{
    public class WebSiteCache : BaseCache, IWebSiteCache
    {
        #region Private Variable
        private readonly IWebSiteService _service;
        #endregion

        #region Constructor
        public WebSiteCache(IWebSiteService webSiteService)
        {
            _service = webSiteService;
        }
        #endregion

        #region Public Methods

        //Get Portal List
        public virtual string GetPortalList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                PortalListModel list = _service.GetPortalList(Filters, Sorts, Page);
                if (list?.PortalList?.Count > 0)
                {
                    PortalListResponse response = new PortalListResponse { PortalList = list.PortalList };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                    return data;
                }
            }
            return data;
        }

        //Get the list of portal page product associated to selected store in website configuration.
        public virtual string GetPortalProductPageList(int portalId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                PortalProductPageModel portalProductPageModel = _service.GetPortalProductPageList(portalId);
                if (IsNotNull(portalProductPageModel))
                {
                    PortalProductPageResponse response = new PortalProductPageResponse { PortalProductPage = portalProductPageModel };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion
    }
}