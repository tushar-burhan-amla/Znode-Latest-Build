using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Cache
{
    public class ProductFeedCache : BaseCache, IProductFeedCache
    {
        #region Private Variables
        private readonly IProductFeedService _service;
        #endregion

        #region Constructor
        public ProductFeedCache(IProductFeedService productFeedService)
        {
            _service = productFeedService;
        }
        #endregion
        #region Public Methods

        //Get a list of product feed.
        public virtual string GetProductFeedList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get product feed list
                ProductFeedListModel list = _service.GetProductFeedList(Expands, Filters, Sorts, Page);
                if (list?.ProductFeeds?.Count > 0)
                {
                    //Create response.
                    ProductFeedListResponse response = new ProductFeedListResponse { ProductFeeds = list.ProductFeeds };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get product feed by Id.
        public virtual string GetProductFeed(int productFeedId, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get ProductFeed by ProductFeed id.
                ProductFeedModel productFeed = _service.GetProductFeed(productFeedId, Expands);
                if (!Equals(productFeed, null))
                {
                    ProductFeedResponse response = new ProductFeedResponse { ProductFeed = productFeed };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get Payment Setting by Payment Setting id.
        public virtual string GetProductFeedByPortalId(string routeUri, string routeTemplate, int portalId)
        {
            //Get data from Cache
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Create Response
                ProductFeedListModel productFeedModel = _service.GetProductFeedByPortalId(portalId);
                if (HelperUtility.IsNotNull(productFeedModel))
                {
                    ProductFeedListResponse response = new ProductFeedListResponse { ProductFeeds = productFeedModel.ProductFeeds };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        #endregion
    }
}
