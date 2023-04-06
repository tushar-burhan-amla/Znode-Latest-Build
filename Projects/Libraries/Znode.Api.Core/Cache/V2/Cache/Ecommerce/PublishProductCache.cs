using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Api.Models.Responses.V2;
using Znode.Engine.Api.Models.V2;
using Znode.Engine.Services;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Api.Cache
{
    public class PublishProductCacheV2 : PublishProductCache, IPublishProductCacheV2
    {
        #region Private Variable
        private readonly IPublishProductServiceV2 _service;
        #endregion

        #region Constructor
        public PublishProductCacheV2(IPublishProductServiceV2 publishProductService) : base(publishProductService)
        {
            _service = publishProductService;
        }
        #endregion

        //Get list of Publish Product by attributes.
        public virtual string GetPublishProductsByAttribute(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (IsNull(data))
            {
                //Get Publish product list.
                PublishProductListModel list = _service.GetPublishProductsByAttribute(Expands, Filters, Sorts, Page);

                if (list?.PublishProducts?.Count > 0)
                {
                    //Create response.
                    PublishProductListResponse response = new PublishProductListResponse { PublishProducts = list.PublishProducts };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetProductPriceV2(ParameterInventoryPriceModel productPriceModel, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);

            if (IsNull(data))
            {
                //Get Publish Product list.
                ProductInventoryPriceListModel list = _service.GetProductPriceV2(productPriceModel);

                if (list?.ProductList?.Count > 0)
                {
                    //Create response.
                    ProductInventoryPriceListResponse response = new ProductInventoryPriceListResponse { ProductList = list.ProductList };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get Publish Product by Publish Product id.
        public virtual string GetPublishProductV2(int publishProductId, string routeUri, string routeTemplate)
        {
            //Get data from Cache
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Create Response
                PublishProductModelV2 publishProduct = _service.GetPublishProductV2(publishProductId, Filters, Expands);
                if (IsNotNull(publishProduct))
                {
                    PublishProductResponseV2 response = new PublishProductResponseV2 { PublishProduct = publishProduct };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
    }
}
