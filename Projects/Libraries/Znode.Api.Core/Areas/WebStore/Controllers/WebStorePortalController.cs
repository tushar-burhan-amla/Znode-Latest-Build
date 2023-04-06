using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Controllers;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Api.Areas.WebStore.Controllers
{
    public class WebStorePortalController : BaseController
    {
        #region Private Variables
        private readonly IWebStorePortalCache _portalCache;
        #endregion

        #region Constructor
        public WebStorePortalController(IPortalService portalService)
        {
            _portalCache = new WebStorePortalCache(portalService);
        }
        #endregion

        /// <summary>
        /// Get portal information for a portal by its id.
        /// </summary>
        /// <param name="portalId">Id of portal to get portal information.</param>
        /// <returns>Returns portal information by portal id.</returns>
        [ResponseType(typeof(WebStorePortalResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetPortal(int portalId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _portalCache.GetPortal(portalId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<string>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                WebStorePortalResponse portalResponse = new WebStorePortalResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(portalResponse);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                WebStorePortalResponse portalResponse = new WebStorePortalResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(portalResponse);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get portal information for a portal by its id.
        /// </summary>
        /// <param name="portalId">Id of portal to get portal information.</param>
        /// <param name="localeId">Locale Id of the portal.</param>
        /// <param name="applicationType">Application Type of the portal which has to be picked.</param>
        /// <returns>Returns portal information by portal id.</returns>
        [ResponseType(typeof(WebStorePortalResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetPortalFromApplicationType(int portalId, int localeId, ApplicationTypesEnum applicationType)
        {
            HttpResponseMessage response;
            try
            {
                string data = _portalCache.GetPortal(portalId, localeId, applicationType, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<string>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                WebStorePortalResponse portalResponse = new WebStorePortalResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(portalResponse);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                WebStorePortalResponse portalResponse = new WebStorePortalResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(portalResponse);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Error);
            }
            return response;
        }


        /// <summary>
        /// Get portal information for a portal by its domainName.
        /// </summary>
        /// <param name="domainName">Id of portal to get portal information.</param>
        /// <returns>Returns portal information by domainName.</returns>
        [ResponseType(typeof(WebStorePortalResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetPortalByDomain(string domainName)
        {
            HttpResponseMessage response;
            try
            {
                string data = _portalCache.GetPortal(domainName, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<string>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                WebStorePortalResponse portalResponse = new WebStorePortalResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(portalResponse);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                WebStorePortalResponse portalResponse = new WebStorePortalResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(portalResponse);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Error);
            }
            return response;
        }
    }
}