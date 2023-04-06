using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Controllers;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using Znode.Libraries.Framework.Business;
namespace Znode.Engine.Api.Areas.WebStore.Controllers
{
    public class WebStoreWidgetController : BaseController
    {
        #region Private Variables
        private readonly IWebStoreWidgetCache _cache;
        #endregion

        #region Constructor
        public WebStoreWidgetController(IWebStoreWidgetService service)
        {
            _cache = new WebStoreWidgetCache(service);
        }
        #endregion
        
        /// <summary>
        /// Get slider data.
        /// </summary>
        /// <param name="parameter">Web Store Widget Parameter Model.</param>
        /// <param name="key">Unique key for a widget.</param>
        /// <returns>Returns slider data.</returns>
        [ResponseType(typeof(ManageMessageResponse))]
        [HttpPut]
        public virtual HttpResponseMessage GetSlider([FromBody] WebStoreWidgetParameterModel parameter, string key)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetSlider(parameter, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<string>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new ManageMessageResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new ManageMessageResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Get product list widget data.
        /// </summary>
        /// <param name="parameter">Web Store Widget Parameter Model.</param>
        /// <param name="key">Unique key for a widget.</param>
        /// <returns>Returns product list widget data.</returns>
        [ResponseType(typeof(WebStoreWidgetProductListResponse))]
        [HttpPut]
        public virtual HttpResponseMessage GetProducts([FromBody] WebStoreWidgetParameterModel parameter, string key)
        {
            HttpResponseMessage response;
            try
            {
                //Get Product List Widget details.
                string data = _cache.GetProducts(parameter, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<string>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Warning);
                WebStoreWidgetProductListResponse data = new WebStoreWidgetProductListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Error);
                WebStoreWidgetProductListResponse data = new WebStoreWidgetProductListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Get link widget data.
        /// </summary>
        /// <param name="parameter">Web Store Widget Parameter Model.</param>
        /// <param name="key">Unique key for a widget.</param>
        /// <returns>Returns link widget data.</returns>
        [ResponseType(typeof(WebStoreLinkWidgetResponse))]
        [HttpPut]
        public virtual HttpResponseMessage GetLinkWidget([FromBody] WebStoreWidgetParameterModel parameter, string key)
        {
            HttpResponseMessage response;
            
            try
            {
                string data = _cache.GetLinkWidget(parameter, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<string>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new WebStoreLinkWidgetResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(),TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new WebStoreLinkWidgetResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Get categories list widget data.
        /// </summary>
        /// <param name="parameter">Web Store Widget Parameter Model.</param>
        /// <param name="key">Unique key for a widget.</param>
        /// <returns>Returns categories list widget data.</returns>
        [ResponseType(typeof(WebStoreWidgetCategoryListResponse))]
        [HttpPut]
        public virtual HttpResponseMessage GetCategories([FromBody] WebStoreWidgetParameterModel parameter, string key)
        {
            HttpResponseMessage response;
            try
            {
                //Get Category List Widget details.
                string data = _cache.GetCategories(parameter, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<string>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Warning);
                WebStoreWidgetCategoryListResponse data = new WebStoreWidgetCategoryListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Error);
                WebStoreWidgetCategoryListResponse data = new WebStoreWidgetCategoryListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Get link product list widget data.
        /// </summary>
        /// <param name="parameter">Web Store Widget Parameter Model.</param>
        /// <param name="key">Unique key for a widget.</param>
        /// <returns>Returns link product list widget data.</returns>
        [ResponseType(typeof(WebStoreLinkProductListResponse))]
        [HttpPut]
        public virtual HttpResponseMessage GetLinkProductList([FromBody] WebStoreWidgetParameterModel parameter, string key)
        {
            HttpResponseMessage response;
            try
            {
                //Get Link Product List Widget details.
                string data = _cache.GetLinkProducts(parameter, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<string>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Warning);
                WebStoreLinkProductListResponse data = new WebStoreLinkProductListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Error);
                WebStoreLinkProductListResponse data = new WebStoreLinkProductListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Get tag manager widget data.
        /// </summary>
        /// <param name="parameter">Web Store Widget Parameter Model.</param>
        /// <param name="key">Unique key for a widget.</param>
        /// <returns>Returns tag manager widget data.</returns>
        [ResponseType(typeof(WebStoreWidgetResponse))]
        [HttpPut]
        public virtual HttpResponseMessage GetTagManager([FromBody] WebStoreWidgetParameterModel parameter, string key)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetTagManager(parameter, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<string>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new ManageMessageResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new ManageMessageResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Get the Media Widget Details
        /// </summary>
        /// <param name="parameter">Web Store Widget Parameter Model.</param>
        /// <param name="key">Unique key for a widget.</param>
        /// <returns></returns>
        [ResponseType(typeof(WebStoreWidgetResponse))]
        [HttpPut]       
        public virtual HttpResponseMessage GetMediaWidgetDetails([FromBody] WebStoreWidgetParameterModel parameter, string key)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetMediaWidgetDetails(parameter, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<string>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new ManageMessageResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new ManageMessageResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Get brand list widget data.
        /// </summary>
        /// <param name="parameter">Web Store Widget Parameter Model.</param>
        /// <param name="key">Unique key for a widget.</param>
        /// <returns>Returns brand list widget data.</returns>
        [ResponseType(typeof(WebStoreWidgetBrandListResponse))]
        [HttpPut]
        public virtual HttpResponseMessage GetBrands([FromBody] WebStoreWidgetParameterModel parameter, string key)
        {
            HttpResponseMessage response;
            try
            {
                //Get brand List Widget details.
                string data = _cache.GetBrands(parameter, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<string>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Warning);
                WebStoreWidgetBrandListResponse data = new WebStoreWidgetBrandListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Error);
                WebStoreWidgetBrandListResponse data = new WebStoreWidgetBrandListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Get brand list widget data.
        /// </summary>
        /// <param name="parameter">Web Store Widget Parameter Model.</param>
        /// <param name="key">Unique key for a widget.</param>
        /// <returns>Returns brand list widget data.</returns>
        [ResponseType(typeof(WebStoreWidgetFormParameters))]
        [HttpPut]
        public virtual HttpResponseMessage GetFormConfigurationByCMSMappingId([FromBody] WebStoreWidgetFormParameters parameter)
        {
            HttpResponseMessage response;
            try
            {
                //Get brand List Widget details.
                string data = _cache.GetFormConfigurationByCMSMappingId(parameter, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<string>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new WebStoreWidgetFormResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Get Search widget data.
        /// </summary>
        /// <param name="parameter">search widget parameter model</param>
        /// <returns></returns>
        [ResponseType(typeof(WebStoreWidgetSearchResponse))]
        [HttpPut]
        public virtual HttpResponseMessage GetSearchWidgetData([FromBody] WebStoreSearchWidgetParameterModel parameter)
        {
            HttpResponseMessage response;
            try
            {
                //Get Search Widget details.
                string data = _cache.GetSearchWidgetData(parameter, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<string>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new WebStoreWidgetSearchResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new WebStoreWidgetSearchResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Get container data.
        /// </summary>
        /// <param name="parameter">Web Store Widget Parameter Model.</param>
        /// <returns>Returns container data.</returns>
        [ResponseType(typeof(ManageMessageResponse))]
        [HttpPut]
        public virtual HttpResponseMessage GetContainer([FromBody] WebStoreWidgetParameterModel parameter)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetContainer(parameter, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<string>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new ManageMessageResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new ManageMessageResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }
    }
}