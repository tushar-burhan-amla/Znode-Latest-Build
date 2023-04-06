using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;


namespace Znode.Engine.Api.Cache
{
    public class CatalogHistoryCache : BaseCache, ICatalogHistoryCache
    {
        #region Private Variables
        private readonly ICatalogHistoryService _service;
        #endregion

        #region Constructor
        public CatalogHistoryCache()
        {
            _service = GetService<ICatalogHistoryService>();
        }
        #endregion

        #region Public Methods
        //Get catalog history list.
        public virtual string GetCatalogHistoryList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (Equals(data, null))
            {
                CatalogHistoryListModel list = _service.GetCatalogHistoryList(Expands, Filters, Sorts, Page);
                if (list?.CatalogHistories?.Count > 0)
                {
                    //Create response.
                    CatalogHistoryListResponse response = new CatalogHistoryListResponse { CatalogHistories = list.CatalogHistories };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Gets catalog history by ID.
        public virtual string GetCatalogHistory(int id, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (Equals(data, null))
            {
                CatalogHistoryModel catalogHistory = _service.GetCatalogHistoryById(id, Expands);
                if (!Equals(catalogHistory, null))
                {
                    //Create response.
                    CatalogHistoryResponse response = new CatalogHistoryResponse { CatalogHistory = catalogHistory };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion
    }
}