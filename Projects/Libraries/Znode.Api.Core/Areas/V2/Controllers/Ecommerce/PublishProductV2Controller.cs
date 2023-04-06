using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Api.Models.Responses.V2;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;

namespace Znode.Engine.Api.Controllers
{
    public class PublishProductV2Controller : BaseController
    {
        #region Private Variables

        private readonly IPublishProductCacheV2 _cache;

        #endregion

        #region Constructor
        public PublishProductV2Controller(IPublishProductServiceV2 service)
        {
            _cache = new PublishProductCacheV2(service);
        }
        #endregion

        /// <summary>
        /// Get list of published products by attribute
        /// </summary>
        /// <returns></returns>        
        [ResponseType(typeof(PublishProductListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetPublishProductsByAttribute()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetPublishProductsByAttribute(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PublishProductListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                PublishProductListResponse data = new PublishProductListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }

            return response;
        }

        /// <summary>
        /// Get price for products through ajax async call.
        /// </summary>
        /// <param name="productPrice">parameters to get product price</param>
        /// <returns></returns>
        [ResponseType(typeof(ProductInventoryPriceListResponse))]
        [HttpPost]
        public virtual HttpResponseMessage GetProductPrice([FromBody] ParameterInventoryPriceModel productPrice)
        {
            HttpResponseMessage response;
            try
            {
                if (!ModelState.IsValid)
                    throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidSku);

                //Get attribute by attribute id.
                string data = _cache.GetProductPriceV2(productPrice, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ProductInventoryPriceListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                ProductInventoryPriceListResponse data = new ProductInventoryPriceListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.InvalidData };
                response = CreateInternalServerErrorResponse(data);
            }

            return response;
        }

        /// <summary>
        /// Get  Publish Product by  Publish Product id.
        /// </summary>
        /// <param name="publishProductId"> Publish Product id to get  Publish Product details.</param>
        /// <returns></returns>
        [ResponseType(typeof(PublishProductResponseV2))]
        [HttpGet]
        public virtual HttpResponseMessage GetPublishProduct(int publishProductId)
        {
            HttpResponseMessage response;

            try
            {
                if (publishProductId < 0)
                    throw new ZnodeException(ErrorCodes.InvalidData,Admin_Resources.InvalidProductId);

                string data = _cache.GetPublishProductV2(publishProductId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PublishProductResponseV2>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new PublishProductResponseV2 { HasError = true, ErrorMessage = ex.Message });
            }

            return response;
        }
    }
}
