using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;
namespace Znode.Engine.Api.Client
{
    public class PublishProductClient : BaseClient, IPublishProductClient
    {
        // Gets the list of Publish Product List.
        public virtual PublishProductListModel GetPublishProductList(ExpandCollection expands, FilterCollection filters, SortCollection sorts)
              => GetPublishProductList(expands, filters, sorts, null, null);

        // Gets the list of Publish Product List.
        public virtual PublishProductListModel GetPublishProductList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = PublishProductEndpoint.GetPublishProductList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            PublishProductListResponse response = GetResourceFromEndpoint<PublishProductListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            PublishProductListModel list = new PublishProductListModel { PublishProducts = response?.PublishProducts };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        // Gets the list of published products.
        public virtual PublishProductListModel GetPublishProductList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize, ParameterKeyModel parameters)
        {
            //Get Endpoint.
            string endpoint = PublishProductEndpoint.GetProductList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            PublishProductListResponse response = PostResourceToEndpoint<PublishProductListResponse>(endpoint, JsonConvert.SerializeObject(parameters), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            PublishProductListModel list = new PublishProductListModel { PublishProducts = response?.PublishProducts };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        // Get the Publish Product.
        public virtual PublishProductModel GetPublishProduct(int publishProductId, FilterCollection filters, ExpandCollection expands)
        {
            string endpoint = PublishProductEndpoint.GetPublishProduct(publishProductId);
            endpoint += BuildEndpointQueryString(expands, filters, null, null, null);

            ApiStatus status = new ApiStatus();
            PublishProductResponse response = GetResourceFromEndpoint<PublishProductResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.PublishProduct;
        }

        // Get only brief details of a Published Product.
        public virtual PublishProductModel GetPublishProductBrief(int publishProductId, FilterCollection filters, ExpandCollection expands)
        {
            string endpoint = PublishProductEndpoint.GetPublishProductBrief(publishProductId);
            endpoint += BuildEndpointQueryString(expands, filters, null, null, null);

            ApiStatus status = new ApiStatus();
            PublishProductResponse response = GetResourceFromEndpoint<PublishProductResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.PublishProduct;
        }
        // Get only brief details of a Parent Published Product.
        public virtual PublishProductModel GetpublishParentProduct(int publishProductId, FilterCollection filters, ExpandCollection expands)
        {
            string endpoint = PublishProductEndpoint.GetpublishParentProduct(publishProductId);
            endpoint += BuildEndpointQueryString(expands, filters, null, null, null);

            ApiStatus status = new ApiStatus();
            PublishProductResponse response = GetResourceFromEndpoint<PublishProductResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.PublishProduct;
        }
        // Get only the extended details of a Published Product.
        public virtual PublishProductDTO GetExtendedPublishProductDetails(int publishProductId, FilterCollection filters, ExpandCollection expands)
        {
            string endpoint = PublishProductEndpoint.GetExtendedPublishProductDetails(publishProductId);
            endpoint += BuildEndpointQueryString(expands, filters, null, null, null);

            ApiStatus status = new ApiStatus();
            PublishProductDTOResponse response = GetResourceFromEndpoint<PublishProductDTOResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.PublishProduct;
        }

        // Get the Publish Product by sku.
        public virtual PublishProductModel GetPublishProductBySKU(ParameterProductModel parameterModel, ExpandCollection expands, FilterCollection filters)
        {
            string endpoint = PublishProductEndpoint.GetPublishProductBySKU();
            endpoint += BuildEndpointQueryString(expands, filters, null, null, null);

            ApiStatus status = new ApiStatus();

            PublishProductResponse response = PostResourceToEndpoint<PublishProductResponse>(endpoint, JsonConvert.SerializeObject(parameterModel), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.PublishProduct;
        }

        //Get Product Price And Inventory.
        public virtual ProductInventoryPriceListModel GetProductPriceAndInventory(ParameterInventoryPriceModel parameterModel)
        {
            //Get Endpoint.
            string endpoint = PublishProductEndpoint.GetProductPriceAndInventory();
            ApiStatus status = new ApiStatus();

            ProductInventoryPriceListResponse response = PostResourceToEndpoint<ProductInventoryPriceListResponse>(endpoint, JsonConvert.SerializeObject(parameterModel), status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };

            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            ProductInventoryPriceListModel ProductList = new ProductInventoryPriceListModel { ProductList = response?.ProductList };

            return ProductList;
        }

        //Get Product Price And Inventory.
        public virtual ProductInventoryPriceListModel GetPriceWithInventory(ParameterInventoryPriceListModel parameterModel)
        {
            //Get Endpoint.
            string endpoint = PublishProductEndpoint.GetPriceWithInventory();
            ApiStatus status = new ApiStatus();

            ProductInventoryPriceListResponse response = PostResourceToEndpoint<ProductInventoryPriceListResponse>(endpoint, JsonConvert.SerializeObject(parameterModel), status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };

            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            ProductInventoryPriceListModel ProductList = new ProductInventoryPriceListModel { ProductList = response?.ProductList };

            return ProductList;
        }

        //Get bundle product by product id.
        public virtual WebStoreBundleProductListModel GetBundleProducts(FilterCollection filters)
        {
            string endpoint = PublishProductEndpoint.GetBundleProducts();
            endpoint += BuildEndpointQueryString(null, filters, null, null, null);
            ApiStatus status = new ApiStatus();
            WebStoreBundleProductListResponse response = GetResourceFromEndpoint<WebStoreBundleProductListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            WebStoreBundleProductListModel list = new WebStoreBundleProductListModel { BundleProducts = response?.BundleProducts };

            return list;
        }

        //Get Group Product list.
        public virtual WebStoreGroupProductListModel GetGroupProductList(FilterCollection filters)
        {
            string endpoint = PublishProductEndpoint.GetGroupProductList();
            endpoint += BuildEndpointQueryString(null, filters, null, null, null);
            ApiStatus status = new ApiStatus();
            WebStoreGroupProductListResponse response = GetResourceFromEndpoint<WebStoreGroupProductListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            WebStoreGroupProductListModel list = new WebStoreGroupProductListModel { GroupProducts = response?.GroupProducts };

            return list;
        }

        //Get product attribute by product id.
        public virtual ConfigurableAttributeListModel GetProductAttribute(int productId, ParameterProductModel parameterProductModel)
        {
            string endpoint = PublishProductEndpoint.GetProductAttribute(productId);

            ApiStatus status = new ApiStatus();
            WebStoreConfigurableAttributeListResponse response = PostResourceToEndpoint<WebStoreConfigurableAttributeListResponse>(endpoint, JsonConvert.SerializeObject(parameterProductModel), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            ConfigurableAttributeListModel list = new ConfigurableAttributeListModel { Attributes = response?.Attributes };

            return list;
        }

        //Get configurable product.
        public virtual PublishProductModel GetConfigurableProduct(ParameterProductModel parameterProductModel, ExpandCollection expands)
        {
            string endpoint = PublishProductEndpoint.GetConfigurableProduct();
            endpoint += BuildEndpointQueryString(expands);
            ApiStatus status = new ApiStatus();

            PublishProductResponse response = PostResourceToEndpoint<PublishProductResponse>(endpoint, JsonConvert.SerializeObject(parameterProductModel), status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.PublishProduct;
        }

        //Get Parent  product.
        public virtual PublishProductModel GetParentProduct(int parentProductId, FilterCollection filters, ExpandCollection expands)
        {
            string endpoint = PublishProductEndpoint.GetParentProduct(parentProductId);
            endpoint += BuildEndpointQueryString(expands, filters, null, null, null);

            ApiStatus status = new ApiStatus();
            PublishProductResponse response = GetResourceFromEndpoint<PublishProductResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.PublishProduct;
        }


        // Get publish product excluding assigned ids.
        public virtual PublishProductListModel GetUnAssignedPublishProductList(ParameterModel assignedIds, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = PublishProductEndpoint.GetUnAssignedPublishProductList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            PublishProductListResponse response = PostResourceToEndpoint<PublishProductListResponse>(endpoint, JsonConvert.SerializeObject(assignedIds), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            PublishProductListModel list = new PublishProductListModel { PublishProducts = response?.PublishProducts };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Send Compare Product Mail.
        public virtual bool SendComparedProductMail(ProductCompareModel productCompareModel)
        {
            //Get Endpoint.
            string endpoint = PublishProductEndpoint.SendComparedProductMail();
            //Get response
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(productCompareModel), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response.IsSuccess;
        }

        //Send Email To Friend.
        public virtual bool SendEmailToFriend(EmailAFriendListModel emailAFriendListModel)
        {
            //Get Endpoint.
            string endpoint = PublishProductEndpoint.SendMailToFriend();
            //Get response
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(emailAFriendListModel), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response.IsSuccess;
        }

        //Get price for products through ajax async call.
        public virtual ProductInventoryPriceListModel GetProductPrice(ParameterInventoryPriceModel productPrice)
        {
            //Get Endpoint.
            string endpoint = PublishProductEndpoint.GetProductPrice();
            //Get response
            ApiStatus status = new ApiStatus();
            ProductInventoryPriceListResponse response = PostResourceToEndpoint<ProductInventoryPriceListResponse>(endpoint, JsonConvert.SerializeObject(productPrice), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            ProductInventoryPriceListModel list = new ProductInventoryPriceListModel { ProductList = response?.ProductList };
            return list;
        }

        /// <summary>
        /// Get Active product for recent view API
        /// </summary>
        /// <param name="parentIds">Parent Ids</param>
        /// <param name="catalogId">Catalog Id</param>
        /// <param name="localeId">Locale Id</param>
        /// <param name="versionId">Version Id</param>
        /// <returns></returns>
        public List<RecentViewProductModel> GetActiveProducts(string parentIds, int catalogId, int localeId, int versionId)
        {
            string endpoint = PublishProductEndpoint.GetActiveProducts(parentIds, catalogId, localeId, versionId);

            ApiStatus status = new ApiStatus();
            RecentViewProductResponse response = GetResourceFromEndpoint<RecentViewProductResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.RecentViewProductModelCollection;
        }

        // Get Product inventory.
        public virtual ProductInventoryDetailModel GetProductInventory(int publishProductId, FilterCollection filters)
        {
            string endpoint = PublishProductEndpoint.GetProductInventory(publishProductId);
            endpoint += BuildEndpointQueryString(null, filters, null, null, null);

            ApiStatus status = new ApiStatus();
            ProductInventoryDetailResponse response = GetResourceFromEndpoint<ProductInventoryDetailResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.ProductInventory;
        }

        // Get associated configurable product variants.
        public virtual PublishProductListModel GetAssociatedConfigurableVariants(int productId)
        {
            //Get Endpoint.
            string endpoint = PublishProductEndpoint.GetAssociatedConfigurableVariants(productId);

            //Get response
            ApiStatus status = new ApiStatus();
            PublishProductListResponse response = GetResourceFromEndpoint<PublishProductListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            PublishProductListModel publishProductListModel = new PublishProductListModel { ConfigurableProducts = response?.ConfigurableProducts };

            return publishProductListModel;
        }

        // Submit request for stock notification.
        public virtual bool SubmitStockRequest(StockNotificationModel stockNotificationModel)
        {
            string endpoint = PublishProductEndpoint.SubmitStockRequest();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(stockNotificationModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return (response?.IsSuccess).GetValueOrDefault();
        }

        // Send stock notification of requested products.
        public virtual bool SendStockNotification(StockNotificationModel stockNotificationModel)
        {
            string endpoint = PublishProductEndpoint.SubmitStockRequest();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(stockNotificationModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return (response?.IsSuccess).GetValueOrDefault();
        }        

        // Get product details by sku.
        public virtual PublishProductModel GetProductDetailsBySKU(SearchRequestModel searchRequestModel, FilterCollection filters, ExpandCollection expandCollection, SortCollection sortCollection)
        {
            string endpoint = SearchEndpoint.GetProductDetailsBySKU();
            endpoint += BuildEndpointQueryString(expandCollection, filters, sortCollection, null, null);

            ApiStatus status = new ApiStatus();

            PublishProductResponse response = PostResourceToEndpoint<PublishProductResponse>(endpoint, JsonConvert.SerializeObject(searchRequestModel), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.PublishProduct;
        }

    }
}
