using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
namespace Znode.Engine.Api.Cache
{
    public class WebStoreProductCache : BaseCache, IWebStoreProductCache
    {
        #region Private Variable
        private readonly IProductService _service;
        #endregion

        #region Constructor
        public WebStoreProductCache(IProductService productService)
        {
            _service = productService;
        }
        #endregion

        //Get webstore product list
        public virtual string ProductList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                WebStoreProductListModel list = _service.ProductList(Expands, Filters, Sorts, Page);
                if (list?.ProductList?.Count > 0)
                {
                    //Get response and insert it into cache.
                    WebStoreProductListResponse response = new WebStoreProductListResponse { ProductsList = list?.ProductList };
                    response.MapPagingDataFromModel(list);

                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetProduct(int productId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                WebStoreProductModel product = _service.GetProduct(productId, Expands);
                if (IsNotNull(product))
                {
                    WebStoreProductResponse response = new WebStoreProductResponse { Product = product };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetAssociatedProducts(ParameterModel productIDs, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                WebStoreProductListModel list = _service.GetAssociatedProducts(productIDs);
                if (list?.ProductList?.Count > 0)
                {
                    WebStoreProductListResponse response = new WebStoreProductListResponse { ProductsList = list?.ProductList };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get product list by sku
        public virtual string GetProductsBySkus(ParameterModel skus, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                WebStoreProductListModel list = _service.GetProductsBySkus(skus);
                if (list?.ProductList?.Count > 0)
                {
                    WebStoreProductListResponse response = new WebStoreProductListResponse { ProductsList = list?.ProductList };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get associated product highlights.
        public virtual string GetProductHighlights(ParameterProductModel model, int productId, int localeId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                HighlightListModel list = _service.GetProductHighlights(model, productId);
                if (list?.HighlightList?.Count > 0)
                {
                    HighlightListResponse response = new HighlightListResponse { Highlights = list?.HighlightList };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
    }
}