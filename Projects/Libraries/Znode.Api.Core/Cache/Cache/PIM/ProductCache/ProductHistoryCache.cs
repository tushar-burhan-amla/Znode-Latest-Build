using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;

namespace Znode.Engine.Api.Cache
{
    //Product History Cache.
    public class ProductHistoryCache : BaseCache, IProductHistoryCache
    {
        #region Private Variables
        private readonly IProductHistoryService _service;
        #endregion

        #region Public Constructor
        public ProductHistoryCache(IProductHistoryService productHistoryService)
        {
            _service = productHistoryService;
        }
        #endregion

        #region Public Methods
        //Get Product History list.
        public string GetProductHistoryList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (Equals(data, null))
            {
                ProductHistoryListModel list = _service.GetProductHistoryList(Expands, Filters, Sorts, Page);
                if (list?.ProductHistoryList?.Count > 0)
                {
                    //Create response.
                    ProductHistoryListResponse response = new ProductHistoryListResponse { ProductHistoryList = list.ProductHistoryList };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Gets product history by ID.
        public string GetProductHistory(int id, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (Equals(data, null))
            {
                ProductHistoryModel productHistory = _service.GetProductHistoryById(id, Expands);
                if (!Equals(productHistory, null))
                {
                    //Create response.
                    ProductHistoryResponse response = new ProductHistoryResponse { ProductHistory = productHistory };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion
    }
}