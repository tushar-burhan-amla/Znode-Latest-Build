using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Cache
{
    public class CategoryHistoryCache : BaseCache, ICategoryHistoryCache
    {
        #region Private Variables
        private readonly ICategoryHistoryService _service;
        #endregion

        #region Constructor
        public CategoryHistoryCache()
        {
            _service = ZnodeDependencyResolver.GetService<ICategoryHistoryService>();
        }
        #endregion

        #region Public Methods
        //Get category history list.
        public virtual string GetCategoryHistoryList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (Equals(data, null))
            {
                CategoryHistoryListModel list = _service.GetCategoryHistoryList(Expands, Filters, Sorts, Page);
                if (list?.CategoryHistories?.Count > 0)
                {
                    //Create response.
                    CategoryHistoryListResponse response = new CategoryHistoryListResponse { CategoryHistories = list.CategoryHistories };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Gets category history by ID.
        public virtual string GetCategoryHistory(int id, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (Equals(data, null))
            {
                CategoryHistoryModel categoryHistory = _service.GetCategoryHistoryById(id, Expands);
                if (!Equals(categoryHistory, null))
                {
                    //Create response.
                    CategoryHistoryResponse response = new CategoryHistoryResponse { CategoryHistory = categoryHistory };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion
    }
}