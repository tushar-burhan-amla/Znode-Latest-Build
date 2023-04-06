using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Helper;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Api.Controllers.Ecommerce
{
    public class PublishProductController : BaseController
    {
        #region Private Variables

        private readonly IPublishProductCache _cache;
        private readonly IPublishProductService _publishProductService;
        #endregion

        #region Constructor
        public PublishProductController(IPublishProductService publishProductService)
        {        
            _cache = new PublishProductCache(publishProductService);
            _publishProductService = publishProductService;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets list of Publish Products.
        /// </summary>
        /// <returns> Return list.</returns>
        [ResponseType(typeof(PublishProductListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage List()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetPublishProductList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PublishProductListResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new PublishProductListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PublishProductListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Get  Publish Product by  Publish Product id.
        /// </summary>
        /// <param name="publishProductId"> Publish Product id to get  Publish Product details.</param>
        /// <returns>Returns publish product.</returns>
        [ResponseType(typeof(PublishProductResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetPublishProduct(int publishProductId)
        {
            HttpResponseMessage response;

            try
            {                
                string data = _cache.GetPublishProduct(publishProductId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PublishProductResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new PublishProductResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PublishProductResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Get parent product.
        /// </summary>
        /// <param name="parentProductId"> Parent Product Id to get  Publish Product details.</param>
        /// <returns>Returns publish product.</returns>
        [ResponseType(typeof(PublishProductResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetParentProduct(int parentProductId)
        {
                Func<string> method = () => _cache.GetParentProduct(parentProductId, RouteUri, RouteTemplate);
                return CreateResponse<PublishProductResponse>(method, ZnodeLogging.Components.PIM.ToString());  
        }

        /// <summary>
        /// Get only the brief details of a published product .
        /// </summary>
        /// <param name="publishProductId"> Publish Product id to get  Publish Product details.</param>
        /// <returns>Returns publish product.</returns>
        [ResponseType(typeof(PublishProductResponse))]
		[HttpGet]
		public virtual HttpResponseMessage GetPublishProductBrief(int publishProductId)
		{
			HttpResponseMessage response;

			try
			{
				string data = _cache.GetPublishProductBrief(publishProductId, RouteUri, RouteTemplate);
				response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PublishProductResponse>(data) : CreateNoContentResponse();
			}
			catch (ZnodeException ex)
			{
				response = CreateInternalServerErrorResponse(new PublishProductResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
				ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
			}
			catch (Exception ex)
			{
				response = CreateInternalServerErrorResponse(new PublishProductResponse { HasError = true, ErrorMessage = ex.Message });
				ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
			}

			return response;
		}

        /// <summary>
        /// Get only the details of a parent published product .
        /// </summary>
        /// <param name="publishProductId"> Publish Product id to get  Publish Product details.</param>
        /// <returns>Returns publish product.</returns>
        [ResponseType(typeof(PublishProductResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetpublishParentProduct(int publishProductId)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetpublishParentProduct(publishProductId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PublishProductResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new PublishProductResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PublishProductResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Get only the extended details of a published product .
        /// </summary>
        /// <param name="publishProductId"></param>
        /// <returns></returns>
        [ResponseType(typeof(PublishProductDTOResponse))]
		[HttpGet]
		public virtual HttpResponseMessage GetExtendedPublishProductDetails(int publishProductId)
		{
			HttpResponseMessage response;

			try
			{
				string data = _cache.GetExtendedPublishProductDetails(publishProductId, RouteUri, RouteTemplate);
				response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PublishProductDTOResponse>(data) : CreateNoContentResponse();
			}
			catch (ZnodeException ex)
			{
				response = CreateInternalServerErrorResponse(new PublishProductDTOResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
				ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
			}
			catch (Exception ex)
			{
				response = CreateInternalServerErrorResponse(new PublishProductDTOResponse { HasError = true, ErrorMessage = ex.Message });
				ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
			}

			return response;
		}

		/// <summary>
		/// Gets list of Publish Products.
		/// </summary>
		/// <returns>Return list.</returns>
		[ResponseType(typeof(PublishProductListResponse))]
        [HttpPost]
        public virtual HttpResponseMessage List([FromBody]ParameterKeyModel parameters)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetPublishProductList(parameters, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PublishProductListResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new PublishProductListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PublishProductListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Get Product Price And Inventory.
        /// </summary>
        /// <returns>Returns product price and inventory.</returns>
        [ResponseType(typeof(ProductInventoryPriceListResponse))]
        [HttpPost]
        public virtual HttpResponseMessage GetProductPriceAndInventory([FromBody]ParameterInventoryPriceModel parameters)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetProductPriceAndInventory(parameters, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ProductInventoryPriceListResponse>(data) : CreateNoContentResponse();
            }

            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PublishProductListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Get Product Price or Inventory.
        /// </summary>
        /// <returns>Returns product price or inventory.</returns>
        [ResponseType(typeof(ProductInventoryPriceListResponse))]
        [HttpPost]
        public virtual HttpResponseMessage GetPriceWithInventory([FromBody]ParameterInventoryPriceListModel parameters)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetPriceWithInventory(parameters, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ProductInventoryPriceListResponse>(data) : CreateNoContentResponse();
            }

            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PublishProductListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Get product by product sku.
        /// </summary>
        /// <param name="parameters">Model with product sku.</param>
        /// <returns>Return publish product.</returns>
        [ResponseType(typeof(PublishProductResponse))]
        [HttpPost]
        public virtual HttpResponseMessage GetPublishProductBySKU([FromBody]ParameterProductModel parameters)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetPublishProductBySKU(parameters, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PublishProductResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new PublishProductResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PublishProductResponse { HasError = true, ErrorMessage = ex.Message});
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// get bundle products.
        /// </summary>
        /// <returns>Returns bundle product.</returns>
        [ResponseType(typeof(WebStoreBundleProductListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetBundleProducts()
        {
            HttpResponseMessage response;

            try
            {
                //Get attribute by attribute id.
                string data = _cache.GetBundleProducts(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<WebStoreBundleProductListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new WebStoreBundleProductListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Get product attributes by product id.
        /// </summary>
        /// <param name="productId">Product id.</param>
        /// <param name="model">ParameterProductModel.</param>
        /// <returns>Returns product attribute list.</returns>
        /// <returns></returns>
        [ResponseType(typeof(WebStoreConfigurableAttributeListResponse))]
        [HttpPost]
        public virtual HttpResponseMessage GetProductAttribute(int productId, ParameterProductModel model)
        {
            HttpResponseMessage response;

            try
            {
                //Get attribute by attribute id.
                string data = _cache.GetProductAttribute(productId, model, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<WebStoreConfigurableAttributeListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new WebStoreConfigurableAttributeListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Get Configurable Product
        /// </summary>
        /// <param name="productAttributes">Model with attributes details.</param>
        /// <returns>Return publish product.</returns>
        [ResponseType(typeof(PublishProductResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage GetConfigurableProduct(ParameterProductModel productAttributes)
        {
            HttpResponseMessage response;
            try
            {
                string product = _cache.GetConfigurableProduct(productAttributes, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(product) ? CreateOKResponse<PublishProductResponse>(product) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PublishProductResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get products associated to main group product.
        /// </summary>
        /// <returns>Return product associated inventory list.</returns>
        [ResponseType(typeof(WebStoreGroupProductListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetGroupProducts()
        {
            HttpResponseMessage response;

            try
            {
                //Get attribute by attribute id.
                string data = _cache.GetGroupProducts(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<WebStoreGroupProductListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new WebStoreGroupProductListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }

            return response;
        }


        /// <summary>
        /// Get publish product excluding assigned ids.
        /// </summary>
        /// <param name="assignedIds">Already assigned Ids</param>
        /// <returns>Return publish product list.</returns>
        [ResponseType(typeof(PublishProductListResponse))]
        [HttpPost]
        public virtual HttpResponseMessage UnassignedList([FromBody] ParameterModel assignedIds)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetUnAssignedPublishProductList(assignedIds, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PublishProductListResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new PublishProductListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PublishProductListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Get price for products through ajax async call.
        /// </summary>
        /// <param name="productPrice">parameters to get product price</param>
        /// <returns>Return price list.</returns>
        [ResponseType(typeof(ProductInventoryPriceListResponse))]
        [HttpPost]
        public virtual HttpResponseMessage GetProductPrice([FromBody] ParameterInventoryPriceModel productPrice)
        {
            HttpResponseMessage response;
            try
            {
                //Get attribute by attribute id.
                string data = _cache.GetProductPrice(productPrice, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ProductInventoryPriceListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ProductInventoryPriceListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Gets list of Publish Products.
        /// </summary>
        /// <returns>Return publish product list.</returns>
        [ResponseType(typeof(PublishProductListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetPublishProductForSiteMap()
        {
            HttpResponseMessage response;
            try
            {
                //Get attribute by attribute id.
                string data = _cache.GetPublishProductForSiteMap(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PublishProductListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PublishProductListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Gets publish product count.
        /// </summary>
        /// <returns>Return publish product list.</returns>
        [ResponseType(typeof(StringResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetPublishProductCount()
        {
            HttpResponseMessage response;
            try
            {
                //Get attribute by attribute id.
                string data = _cache.GetPublishProductCount(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<StringResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new StringResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }
            return response;
        }


        /// <summary>
        /// Get published product list.
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(PublishProductListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetPublishedProductsListData()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetPublishedProductsListData(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PublishProductListResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new PublishProductListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PublishProductListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Get Product inventory.
        /// </summary>
        /// <param name="publishProductId"> Publish Product id to get  Publish Product details.</param>
        /// <returns>Returns publish product.</returns>
        [ResponseType(typeof(ProductInventoryDetailResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetProductInventory(int publishProductId)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetProductInventory(publishProductId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ProductInventoryDetailResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new ProductInventoryDetailResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ProductInventoryDetailResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Get associated configurable variants.
        /// </summary>
        /// <param name="ProductId">Product Id</param>
        /// <returns>Configurable Product variants.</returns>
        [ResponseType(typeof(PublishProductListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAssociatedConfigurableVariants(int ProductId)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetAssociatedConfigurableVariants(ProductId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PublishProductListResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new PublishProductListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PublishProductListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Submit stock request.
        /// </summary>
        /// <param name="stockNotificationModel">stockNotificationModel</param>
        /// <returns>status</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage SubmitStockRequest(StockNotificationModel stockNotificationModel)
        {
            HttpResponseMessage response;
            try
            {
                bool status = _publishProductService.SubmitStockRequest(stockNotificationModel);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = status });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Send stock notification.
        /// </summary>
        /// <returns>status</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpGet, ValidateModel]
        public virtual HttpResponseMessage SendStockNotification()
        {
            HttpResponseMessage response; 
            try
            {
                bool status = _publishProductService.SendStockNotification();   
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = status });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Error);
            }
            return response;
        }
        #endregion
    }
}