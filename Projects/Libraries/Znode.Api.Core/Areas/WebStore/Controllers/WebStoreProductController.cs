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

namespace Znode.Engine.Api.Controllers
{
    public class WebStoreProductController : BaseController
    {
        #region Private Variables
        private readonly IWebStoreProductCache _cache;
        private readonly IProductService _service;
        #endregion

        #region Constructor
        public WebStoreProductController(IProductService service)
        {
            _service = service;
            _cache = new WebStoreProductCache(_service);
        }
        #endregion

        /// <summary>
        /// Get webstore product list.
        /// </summary>
        /// <returns>Returns list of products in webstore.</returns>
        [ResponseType(typeof(WebStoreProductListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage List()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.ProductList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<string>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                WebStoreProductListResponse data = new WebStoreProductListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                WebStoreProductListResponse data = new WebStoreProductListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Get product data by product id.
        /// </summary>
        /// <param name="productId">Product id.</param>
        /// <returns>Returns product model.</returns>
        [ResponseType(typeof(WebStoreProductResponse))]
        [HttpGet]
        public virtual HttpResponseMessage Get(int productId)
        {
            HttpResponseMessage response;

            try
            {
                //Get attribute by attribute id.
                string data = _cache.GetProduct(productId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<string>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Error);
                WebStoreProductResponse data = new WebStoreProductResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }

            return response;
        }

        /// <summary>
        /// Get list of associated products for product ids.
        /// </summary>
        /// <param name="productIDs">Ids of various products.</param>
        /// <returns>Returns list of associated products for those product ids.</returns>
        [ResponseType(typeof(WebStoreProductListResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage GetAssociatedProducts(ParameterModel productIDs)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetAssociatedProducts(productIDs, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<string>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                WebStoreProductListResponse data = new WebStoreProductListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                WebStoreProductListResponse data = new WebStoreProductListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Get product list by skus.
        /// </summary>
        /// <param name="skus">Parameter model with multiple SKUs.</param>
        /// <returns>Returns product list by skus.</returns>
        [ResponseType(typeof(WebStoreProductListResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage GetProductsBySkus(ParameterModel skus)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetProductsBySkus(skus, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<string>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                WebStoreProductListResponse data = new WebStoreProductListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                WebStoreProductListResponse data = new WebStoreProductListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        ///Get product highlights for a product on the basis of locale. 
        /// </summary>
        /// <param name="model">Model with highlight codes.</param>
        /// <param name="productId">Product id.</param>
        /// <param name="localeId">Locale id</param>
        /// <returns>HttpResponseMessage</returns>
        [ResponseType(typeof(HighlightListResponse))]
        [HttpPost]
        public virtual HttpResponseMessage GetProductHighlights(ParameterProductModel model, int productId,int localeId)
        {
            HttpResponseMessage response;

            try
            {
                //Get attribute by attribute id.
                string data = _cache.GetProductHighlights(model, productId, localeId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<string>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Error);
                HighlightListResponse data = new HighlightListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }

            return response;
        }

        /// <summary>
        /// Send mail for compare product feature.
        /// </summary>
        /// <param name="model">Product Compare Model.</param>
        /// <returns>Return true if mail sent successfully else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage SendComparedProductMail(ProductCompareModel model)
        {
            HttpResponseMessage response;

            try
            {
                //Get send product compare mail.
                bool result = _service.SendComparedProductMail(model);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = result });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(),TraceLevel.Warning);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(),TraceLevel.Error);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Send mail to a friend.
        /// </summary>
        /// <param name="model">Email A Friend List Model.</param>
        /// <returns>Return true if mail sent successfully else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage SendMailToFriend(EmailAFriendListModel model)
        {
            HttpResponseMessage response;

            try
            {
                //Get send mail to friend.
                bool result = _service.SendMailToFriend(model);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = result });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(),TraceLevel.Warning);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(),TraceLevel.Error);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }
    }
}