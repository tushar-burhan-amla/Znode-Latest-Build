using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Cache
{
    public class WebStoreCategoryCache : BaseCache, IWebStoreCategoryCache
    {
        #region Private Variables
        private readonly ICategoryService _service;
        #endregion

        #region Constructor
        public WebStoreCategoryCache(ICategoryService categoryservice)
        {
            _service = categoryservice;
        }
        #endregion

        #region Public Methods

        //Get a list of Categories,SubCategories and Product list.
        public virtual string GetCategoryDetails(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (HelperUtility.IsNull(data))
            {
                //Get Categories,SubCategories and associated Product list
                WebStoreCategoryListModel list = _service.GetCategoryDetails(Expands, Filters, Sorts, Page);
                if (list?.Categories?.Count > 0)
                {
                    //Create response.
                    WebStoreCategoryListResponse response = new WebStoreCategoryListResponse { Categories = list.Categories };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        #endregion
    }
}