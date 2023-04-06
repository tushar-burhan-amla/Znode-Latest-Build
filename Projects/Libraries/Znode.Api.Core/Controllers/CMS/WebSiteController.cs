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
    public class WebSiteController : BaseController
    {
        #region Private Variables
        private readonly IWebSiteCache _cache;
        private readonly IWebSiteService _service;
        #endregion

        #region Constructor
        public WebSiteController(IWebSiteService service)
        {
            _service = service;
            _cache = new WebSiteCache(_service);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Get the portal list.
        /// </summary>
        /// <returns>Returns list of portals.</returns>
        [ResponseType(typeof(PortalListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetPortalList()
        {
            HttpResponseMessage response;
            try
            {
                //Get Portal Details from Cache.
                string data = _cache.GetPortalList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PortalListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PortalListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get the website logo details.
        /// </summary>
        /// <param name="portalId">Portal id.</param>
        /// <returns>Returns the website logo details.</returns>
        [ResponseType(typeof(WebSiteLogoResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetWebSiteLogoDetails(int portalId)
        {
            HttpResponseMessage response;
            try
            {
                //Get the Web Site Logo Details.
                WebSiteLogoModel data = _service.GetWebSiteLogoDetails(portalId);
                // TODO: Add info logs
                response = HelperUtility.IsNotNull(data) ? CreateOKResponse(new WebSiteLogoResponse { WebSiteLogo = data }) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new WebSiteLogoResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new WebSiteLogoResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Save the website logo details.
        /// </summary>
        /// <param name="model">Website logo model to save.</param>
        /// <returns>Returns the website logo.</returns>
        [ResponseType(typeof(WebSiteLogoResponse))]
        [HttpPut]
        public virtual HttpResponseMessage SaveWebSiteLogo([FromBody] WebSiteLogoModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Save the Web Site Logo Details
                bool status = _service.SaveWebSiteLogo(model);
                response = CreateCreatedResponse(new WebSiteLogoResponse { WebSiteLogo = model });
                response.Headers.Add("Location", Convert.ToString(model.CMSThemeId));
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new WebSiteLogoResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new WebSiteLogoResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

      
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpGet]
        public virtual HttpResponseMessage PublishWithPreview([FromUri]PublishRequestModel publishRequestModel)
        {
            HttpResponseMessage response;

            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.PublishAsync(publishRequestModel.PortalId, publishRequestModel.TargetPublishState, publishRequestModel.PublishContent, publishRequestModel.TakeFromDraftFirst) });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new PublishedResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }

            return response;
        }

        #region Portal Product Page
        /// <summary>
        /// Get the list of portal page product associated to selected store in website configuration.
        /// </summary>
        /// <param name="portalId">Id of store to get portal page product.</param>
        /// <returns>Return portal product page.</returns>
        [ResponseType(typeof(PortalProductPageResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetPortalProductPageList(int portalId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetPortalProductPageList(portalId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PortalProductPageResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new PortalProductPageResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PortalProductPageResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;

        }

        /// <summary>
        /// Assign new PDP template to product type.
        /// </summary>
        /// <param name="model">ThemeModel</param>
        /// <returns>Returns true if update page successfully else returns false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPut]
        public virtual HttpResponseMessage UpdatePortalProductPage([FromBody] PortalProductPageModel model)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.UpdatePortalProductPage(model) });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new PortalProductPageResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PortalProductPageResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get widget id by its code.
        /// </summary>
        /// <returns>Returns widget id.</returns>
        [ResponseType(typeof(WebStoreWidgetResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetWidgetIdByCode(string widgetCode)
        {
            HttpResponseMessage response;
            try
            {
                int widgetId = _service.GetWidgetIdByCode(widgetCode);
                response = CreateOKResponse(new WebStoreWidgetResponse { CMSWidgetsId = widgetId });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new WebStoreWidgetResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get pim catalog id associated to supplied portal id.
        /// </summary>
        /// <returns>Returns widget id.</returns>
        [ResponseType(typeof(StringResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAssociatedCatalogId(int portalId)
        {
            HttpResponseMessage response;
            try
            {
                int pimCatalogId = _service.GetAssociatedCatalogId(portalId);
                response = CreateOKResponse(new StringResponse { Response = pimCatalogId.ToString() });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new StringResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }
        #endregion
        #endregion
    }
}