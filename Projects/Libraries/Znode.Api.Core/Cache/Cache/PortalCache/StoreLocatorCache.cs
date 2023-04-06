using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
namespace Znode.Engine.Api.Cache
{
    public class StoreLocatorCache : BaseCache, IStoreLocatorCache
    {
        #region Private Variable
        private readonly IStoreLocatorService _service;
        #endregion

        #region Constructor
        public StoreLocatorCache(IStoreLocatorService storeLocatorService)
        {
            _service = storeLocatorService;
        }
        #endregion

        #region Public Methods.
        //Get store list for location.
        public virtual string GetStoreLocatorList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //get store list.
                StoreLocatorListModel list = _service.GetStoreLocatorList(Expands, Filters, Sorts, Page);
                if (list?.StoreLocatorList?.Count > 0)
                {
                    StoreLocatorListResponse response = new StoreLocatorListResponse { StoreLocatorList = list?.StoreLocatorList };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                    return data;
                }
            }
            return data;
        }

        //Get store data by id.
        public virtual string GetStoreLocator(int storeId, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //get store details by id.
                StoreLocatorDataModel model = _service.GetStoreLocator(storeId, Expands);
                if (IsNotNull(model))
                {
                    StoreLocatorResponse response = new StoreLocatorResponse { storeLocatorModel = model };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        //Get store locator data by store Location code
        public virtual string GetStoreLocator(string storeLocationCode, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //get store details by store Location code
                StoreLocatorDataModel model = _service.GetStoreLocator(storeLocationCode, Expands);
                if (IsNotNull(model))
                {
                    StoreLocatorResponse response = new StoreLocatorResponse { storeLocatorModel = model };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion
    }
}