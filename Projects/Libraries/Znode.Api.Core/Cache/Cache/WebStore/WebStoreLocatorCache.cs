using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Cache
{
    public class WebStoreLocatorCache : BaseCache, IWebStoreLocatorCache
    {
        #region Private Variables
        private readonly IStoreLocatorService _service;
        #endregion

        #region Constructor
        public WebStoreLocatorCache(IStoreLocatorService storeLocatorService)
        {
            _service = storeLocatorService;
        }
        #endregion

        #region Public Methods

        //Get a list of Web Store.
        public virtual string GetWebStoreLocatorList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);

            if (HelperUtility.IsNull(data))
            {
                //Get Web Store list.
                WebStoreLocatorListModel list = _service.GetWebStoreLocatorList(Filters, Sorts);
                if (list?.StoreLocators?.Count > 0)
                {
                    //Create response.
                    WebStoreLocatorResponse response = new WebStoreLocatorResponse { StoreLocatorList = list.StoreLocators };

                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        #endregion
    }
}