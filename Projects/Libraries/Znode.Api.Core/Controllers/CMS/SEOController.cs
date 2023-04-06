using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Api.Controllers
{
    public class SEOController : BaseController
    {
        #region Private Variables
        private readonly ISEOCache _cache;
        private readonly ISEOService _service;
        #endregion

        public SEOController(ISEOService service)
        {
            _service = service;
            _cache = new SEOCache(_service);
        }

        // GET: SEO
        /// <summary>
        /// To get Portal SEO Setting.
        /// </summary>
        /// <param name="portalId">Portal Id for SEO setting.</param>
        /// <returns>Portal's SEO settings.</returns>   
        [ResponseType(typeof(PortalSEOSettingResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetPortalSEOSetting(int portalId)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetPortalSEOSetting(portalId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PortalSEOSettingResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new PortalSEOSettingResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PortalSEOSettingResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }

            return response;
        }

        ///<summary>
        ///Create portal SEO default settings.
        ///</summary>
        ///<param name="model">Portal default setting model.</param>
        /// <returns>Return new created portal default settings.</returns>     
        [ResponseType(typeof(PortalSEOSettingResponse))]
        [HttpPost]
        public virtual HttpResponseMessage CreatePortalDefaultSetting([FromBody] PortalSEOSettingModel model)
        {
            HttpResponseMessage response;

            try
            {
                var portalSeoSetting = _service.CreatePortalSEOSetting(model);
                if (!Equals(portalSeoSetting, null))
                {
                    response = CreateCreatedResponse(new PortalSEOSettingResponse { PortalSEOSetting = portalSeoSetting });

                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(portalSeoSetting.CMSPortalSEOSettingId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new PortalSEOSettingResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PortalSEOSettingResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Update an Exiting Portal's SEO Settings.
        /// </summary>
        /// <param name="model">Portal's SEO settings.</param>
        /// <returns>Return updated Portal SEO settings.</returns>
        [ResponseType(typeof(PortalSEOSettingResponse))]
        [HttpPut]
        public virtual HttpResponseMessage UpdatePortalSEOSetting([FromBody] PortalSEOSettingModel model)
        {
            HttpResponseMessage response;

            try
            {
                bool isPortalSEOSettingUpdated = _service.UpdatePortalSEOSetting(model);
                response = isPortalSEOSettingUpdated ? CreateCreatedResponse(new PortalSEOSettingResponse { PortalSEOSetting = model }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new PortalSEOSettingResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PortalSEOSettingResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Gets SEO details for specified SEO details ID.
        /// </summary>
        /// <param name="itemId">ID of Content page/Published product/published category.</param>
        /// <param name="seoTypeId">SEO Type Id</param>
        /// <param name="localeId">Locale Id</param>
        /// <param name="portalId">Portal Id</param>
        /// <returns>Return seo details.</returns>
        [ResponseType(typeof(SEODetailsResponse))]
        [HttpGet]
        [Obsolete]
        public virtual HttpResponseMessage GetSEODetails(int itemId, int seoTypeId, int localeId, int portalId)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetSEODetails(itemId, seoTypeId, localeId, portalId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<SEODetailsResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new PortalSEOSettingResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new SEODetailsResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Gets SEO details for specified SEO details ID.
        /// </summary>
        /// <param name="seoCode">SEOCode of Content page/Published product/published category.</param>
        /// <param name="seoTypeId">SEO Type Id</param>
        /// <param name="localeId">Locale Id</param>
        /// <param name="portalId">Portal Id</param>
        /// <returns>Return seo details.</returns>
        [ResponseType(typeof(SEODetailsResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetSEODetailsBySEOCode(string seoCode, int seoTypeId, int localeId, int portalId)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetSEODetailsBySEOCode(seoCode, seoTypeId, localeId, portalId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<SEODetailsResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new PortalSEOSettingResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new SEODetailsResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }

            return response;
        }


        /// <summary>
        /// Gets SEO details for specified SEO details ID.
        /// </summary>
        /// <param name="seoCode">SEOCode of Content page/Published product/published category.</param>
        /// <param name="seoTypeId">SEO Type Id</param>
        /// <param name="localeId">Locale Id</param>
        /// <param name="portalId">Portal Id</param>
        /// <returns>Return seo details.</returns>
        [ResponseType(typeof(SEODetailsResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetDefaultSEODetails(string seoCode, int seoTypeId, int localeId, int portalId, int itemId)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetDefaultSEODetails(seoCode, seoTypeId, localeId, portalId, itemId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<SEODetailsResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new PortalSEOSettingResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new SEODetailsResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Gets SEO details for specified SEO details ID.
        /// </summary>
        /// <param name="itemId">ID of Content page/Published product/published category.</param>
        /// <param name="seoType">SEO Type</param>
        /// <param name="localeId">Locale Id</param>
        /// <param name="portalId">Portal Id</param>
        /// <param name="seoCode"></param>
        /// <returns>Return seo details.</returns>
        [ResponseType(typeof(SEODetailsResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetPublishSEODetails(int itemId, string seoType, int localeId, int portalId, string seoCode)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetPublishSEODetails(itemId, seoType, localeId, portalId, seoCode, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<SEODetailsResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new SEODetailsResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }

            return response;
        }

      
        ///<summary>
        ///Create SEO details for product/category/content page.
        ///<param name="model">SEO details model.</param>
        /// <returns>Returns newly created SEO details model.</returns> 
        /// </summary>    
        [ResponseType(typeof(SEODetailsResponse))]
        [HttpPost]
        public virtual HttpResponseMessage CreateSEODetails([FromBody] SEODetailsModel model)
        {
            HttpResponseMessage response;

            try
            {
                model = _service.CreateSEODetails(model);
                if (HelperUtility.IsNotNull(model))
                {
                    response = CreateCreatedResponse(new SEODetailsResponse { SEODetail = model });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(model.CMSSEODetailId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new PortalSEOSettingResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PortalSEOSettingResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Update SEO details for product/category/content page.
        /// </summary>
        /// <param name="model">SEO details model.</param>
        /// <returns>Returns updated Portal SEO settings.</returns>
        [ResponseType(typeof(SEODetailsResponse))]
        [HttpPut]
        public virtual HttpResponseMessage UpdateSEODetails([FromBody] SEODetailsModel model)
        {
            HttpResponseMessage response;
            try
            {
                bool isSEODetailsUpdated = _service.UpdateSEODetails(model);
                response = isSEODetailsUpdated ? CreateCreatedResponse(new SEODetailsResponse { SEODetail = model }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new SEODetailsResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new SEODetailsResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

     

        /// <summary>
        /// Publish Seo
        /// </summary>
        /// <param name="seoCode">Seo code of Content page/Published product/published category.</param>
        /// <param name="portalId">Portal Id</param>
        /// <param name="localeId">Locale Id</param>
        /// <param name="seoTypeId">Seo Type Id</param>
        /// <param name="takeFromDraftFirst"></param>
        /// <param name="targetPublishState"></param>
        /// <returns></returns>
        public virtual HttpResponseMessage PublishWithPreview(string seoCode, int portalId, int localeId, int seoTypeId, string targetPublishState = null, bool takeFromDraftFirst = false)
        {
            HttpResponseMessage response;
            try
            {
                PublishedModel published = _service.Publish(seoCode, portalId, localeId, seoTypeId, targetPublishState, takeFromDraftFirst);
                response = !Equals(published, null) ? CreateOKResponse(new PublishedResponse { PublishedModel = published }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new PublishedResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new PublishedResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Gets a list of Seo Details.
        /// </summary>
        /// <returns>Return SEO details list.</returns>        
        [ResponseType(typeof(SEODetailsListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage List()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetSeoDetailsList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<SEODetailsListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new SEODetailsListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Gets list of Publish Categories.
        /// </summary>
        /// <returns>Returns list.</returns>
        [ResponseType(typeof(PublishCategoryListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetCategoryListForSEO()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetCategoryListForSEO(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PublishCategoryListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PublishCategoryListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }
        /// <summary>
        /// Gets list of Publish Products.
        /// </summary>
        /// <returns> Return list.</returns>
        [ResponseType(typeof(PublishProductListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetProductsForSEO()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetProductsForSEO(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PublishProductListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PublishProductListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Delete Seo Detail by using seoTypeId, portalId and seoCode.
        /// </summary>
        /// <param name="seoTypeId">seoType Id</param>
        /// <param name="portalId">portal Id</param>
        /// /// <param name="seoCode">seoCode</param>
        /// <returns>Returns true if record deleted successfully, else false.</returns>.
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpGet]
        public HttpResponseMessage DeleteSeoDetail(int seoTypeId, int portalId,  string seoCode)
        {
            HttpResponseMessage response;
            try
            {
                //Delete Seo.
                bool deleted = _service.DeleteSeo(seoTypeId, portalId, seoCode);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }
    }
}