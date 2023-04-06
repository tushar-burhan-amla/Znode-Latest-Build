using System;
using System.Collections.Generic;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Api.Cache
{
    public class PublishProductCache : BaseCache, IPublishProductCache
    {
        #region Private Variable
        private readonly IPublishProductService _service;
        #endregion

        #region Constructor
        public PublishProductCache(IPublishProductService publishProductService)
        {
            _service = publishProductService;
        }
        #endregion

        #region Public Methods

        //Get list of Publish Product.
        public virtual string GetPublishProductList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (IsNull(data))
            {
                //Get Publish product list.
                PublishProductListModel list = _service.GetPublishProductList(Expands, Filters, Sorts, Page);

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

        //Get Publish Product by Publish Product id.
        public virtual string GetPublishProduct(int publishProductId, string routeUri, string routeTemplate)
        {
            //Get data from Cache
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Create Response
                PublishProductModel publishProduct = _service.GetPublishProduct(publishProductId, Filters, Expands);
                if (IsNotNull(publishProduct))
                {
                    PublishProductResponse response = new PublishProductResponse { PublishProduct = publishProduct };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get Parent Publish Product by Publish Product id.
        public virtual string GetParentProduct(int parentProductId, string routeUri, string routeTemplate)
        {
             //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get product details
                PublishProductModel publishProduct = _service.GetParentProduct(parentProductId, Filters, Expands);
                if (IsNotNull(publishProduct))
                {
                    //Create response.
                    PublishProductResponse response = new PublishProductResponse { PublishProduct = publishProduct };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }  
            }
            return data;
        }
        
        //Get only the brief details of a published product.
        public virtual string GetPublishProductBrief(int publishProductId, string routeUri, string routeTemplate)
		{
			//Get data from Cache
			string data = GetFromCache(routeUri);
			if (string.IsNullOrEmpty(data))
			{
				//Create Response
				PublishProductModel publishProduct = _service.GetPublishProductBrief(publishProductId, Filters, Expands);
				if (IsNotNull(publishProduct))
				{
					PublishProductResponse response = new PublishProductResponse { PublishProduct = publishProduct };
					data = InsertIntoCache(routeUri, routeTemplate, response);
				}
			}
			return data;
		}

        //Get only the details of a parent published product.
        public virtual string GetpublishParentProduct(int publishProductId, string routeUri, string routeTemplate)
        {
            //Get data from Cache
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Create Response
                PublishProductModel publishProduct = _service.GetpublishParentProduct(publishProductId, Filters, Expands);
                if (IsNotNull(publishProduct))
                {
                    PublishProductResponse response = new PublishProductResponse { PublishProduct = publishProduct };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get only the extended details of a published product.
        public virtual string GetExtendedPublishProductDetails(int publishProductId, string routeUri, string routeTemplate)
		{
			//Get data from Cache
			string data = GetFromCache(routeUri);
			if (string.IsNullOrEmpty(data))
			{
				//Create Response
				PublishProductModel result = _service.GetExtendedPublishProductDetails(publishProductId, Filters, Expands);
				PublishProductDTO publishProduct = AutoMapper.Mapper.Map<PublishProductDTO>(result);

				if (IsNotNull(publishProduct))
				{
					PublishProductDTOResponse response = new PublishProductDTOResponse { PublishProduct = publishProduct };
					data = InsertIntoCache(routeUri, routeTemplate, response);
				}
			}
			return data;
		}

		//Get Publish Product by Publish Product sku.
		public virtual string GetPublishProductBySKU(ParameterProductModel parameters, string routeUri, string routeTemplate)
        {
            //Get data from Cache
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Create Response
                PublishProductModel publishProduct = _service.GetPublishProductBySKU(parameters, Expands, Filters);
                if (IsNotNull(publishProduct))
                {
                    PublishProductResponse response = new PublishProductResponse { PublishProduct = publishProduct };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get list of Publish Product.
        public virtual string GetPublishProductList(ParameterKeyModel parameters, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (IsNull(data))
            {
                //Get Publish Product list.
                PublishProductListModel list = _service.GetPublishProductList(Expands, Filters, Sorts, Page, parameters);
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

        //get product inventory and price by sku
        public virtual string GetProductPriceAndInventory(ParameterInventoryPriceModel parameters, string routeUri, string routeTemplate)
        {
            // Get data from cache.
            string data = GetFromCache(routeUri);
            if (IsNull(data))
            {
                //Get Publish Product list.
                ProductInventoryPriceListModel list = _service.GetProductPriceAndInventory(parameters);
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

        //get product inventory or price by sku
        public virtual string GetPriceWithInventory(ParameterInventoryPriceListModel parameters, string routeUri, string routeTemplate)
        {
            // Get data from cache.
            string data = GetFromCache(routeUri);
            if (IsNull(data))
            {
                //Get Publish Product list.
                ProductInventoryPriceListModel list = _service.GetPriceWithInventory(parameters);
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


        //get bundle products.
        public virtual string GetBundleProducts(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                WebStoreBundleProductListModel list = _service.GetBundleProducts(Filters);
                if (list?.BundleProducts?.Count > 0)
                {
                    WebStoreBundleProductListResponse response = new WebStoreBundleProductListResponse { BundleProducts = list?.BundleProducts };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //get bundle products.
        public virtual string GetGroupProducts(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                WebStoreGroupProductListModel list = _service.GetGroupProducts(Filters);
                if (list?.GroupProducts?.Count > 0)
                {
                    WebStoreGroupProductListResponse response = new WebStoreGroupProductListResponse { GroupProducts = list?.GroupProducts };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }


        //Get product attributes by product id.
        public virtual string GetProductAttribute(int productId, ParameterProductModel model, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                ConfigurableAttributeListModel list = _service.GetProductAttribute(productId, model);
                if (list?.Attributes?.Count > 0)
                {
                    WebStoreConfigurableAttributeListResponse response = new WebStoreConfigurableAttributeListResponse { Attributes = list?.Attributes };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get configurable product.
        public virtual string GetConfigurableProduct(ParameterProductModel productAttributes, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                PublishProductModel product = _service.GetConfigurableProduct(productAttributes, Expands);
                if (IsNotNull(product))
                {
                    PublishProductResponse response = new PublishProductResponse { PublishProduct = product };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get list of Publish Product.
        public virtual string GetUnAssignedPublishProductList(ParameterModel assignedIds, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);

            if (IsNull(data))
            {
                //Get Publish Product list.
                PublishProductListModel list = _service.GetUnAssignedPublishProductList(assignedIds, Expands, Filters, Sorts, Page);

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

        public virtual string GetProductPrice(ParameterInventoryPriceModel productPriceModel, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);

            if (IsNull(data))
            {
                //Get Publish Product list.
                ProductInventoryPriceListModel list = _service.GetProductPrice(productPriceModel);

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

        //Get list of Publish Product.
        public virtual string GetPublishProductForSiteMap(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (IsNull(data))
            {
                //Get Publish product list.
                PublishProductListModel list = _service.GetPublishProductForSiteMap(Expands, Filters, Sorts, Page);

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

        //Get Publish Product Count
        public virtual string GetPublishProductCount(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (IsNull(data))
            {
                //Get Publish product list.
                int productCount = _service.GetPublishProductCount(Filters);

                //Create response.()
                StringResponse response = new StringResponse { Response = Convert.ToString(productCount) };
                data = InsertIntoCache(routeUri, routeTemplate, response);
            }
            return data;
        }

        //Get Published Product List.
        public virtual string GetPublishedProductsListData(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (IsNull(data))
            {
                //Get Publish product list.
                PublishProductListModel list = _service.GetPublishedProductsListData(Expands, Filters, Sorts, Page);

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

        //Get Publish Product by Publish Product id.
        public virtual string GetProductInventory(int publishProductId, string routeUri, string routeTemplate)
        {
            //Get data from Cache
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Create Response
                ProductInventoryDetailModel productInventory = _service.GetProductInventory(publishProductId, Filters, Expands);
                if (IsNotNull(productInventory))
                {
                    ProductInventoryDetailResponse response = new ProductInventoryDetailResponse { ProductInventory = productInventory };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        // Get associated configurable variants.
        public virtual string GetAssociatedConfigurableVariants(int productId, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get list of variants.
                List<WebStoreConfigurableProductModel> webStoreConfigurableProductlist = _service.GetAssociatedConfigurableVariants(productId);

                if (webStoreConfigurableProductlist?.Count > 0)
                {
                    //Create response.
                    PublishProductListResponse response = new PublishProductListResponse { ConfigurableProducts = webStoreConfigurableProductlist };

                    //apply pagination parameters.
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
    }
    #endregion
}
