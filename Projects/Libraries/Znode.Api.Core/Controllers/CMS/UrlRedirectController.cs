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
    public class UrlRedirectController : BaseController
    {
        #region Private Variables
        private readonly IUrlRedirectCache _cache;
        private readonly IUrlRedirectService _service;
        #endregion

        #region Constructor
        public UrlRedirectController(IUrlRedirectService service)
        {
            _service = service;
            _cache = new UrlRedirectCache(_service);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Create Url Redirect.
        /// </summary>
        /// <param name="model">Url Redirect Model.</param>
        /// <returns>Returns created Url Redirect.</returns>
        [ResponseType(typeof(UrlRedirectResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage Create([FromBody] UrlRedirectModel model)
        {
            HttpResponseMessage response;
            try
            {
                UrlRedirectModel urlRedirect = _service.Create(model);

                if (!Equals(urlRedirect, null))
                {
                    response = CreateCreatedResponse(new UrlRedirectResponse { UrlRedirect = urlRedirect });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(urlRedirect.CMSUrlRedirectId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new UrlRedirectResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new UrlRedirectResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get Url Redirect List.
        /// </summary>
        /// <returns>Returns Url Redirect List.</returns>
        [ResponseType(typeof(UrlRedirectResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetUrlRedirectList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetUrlRedirectlist(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<UrlRedirectResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new UrlRedirectResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new UrlRedirectResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get url redirect.
        /// </summary>
        /// <returns>Returns Url Redirect model.</returns>
        [ResponseType(typeof(UrlRedirectResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetUrlRedirect()
        {
            HttpResponseMessage response;

            try
            {
                //Get Url Redirect.
                string data = _cache.GetUrlRedirect(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<UrlRedirectResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new UrlRedirectResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new UrlRedirectResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Update url redirect.
        /// </summary>
        /// <param name="model">model to update.</param>
        /// <returns>Returns updated model.</returns>
        [ResponseType(typeof(UrlRedirectResponse))]
        [HttpPut, ValidateModel]
        public virtual HttpResponseMessage Update([FromBody] UrlRedirectModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Update Url Redirect.
                response = _service.Update(model) ? CreateCreatedResponse(new UrlRedirectResponse { UrlRedirect = model, ErrorCode = 0 }) : CreateInternalServerErrorResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(model.CMSUrlRedirectId)));
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new UrlRedirectResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new UrlRedirectResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Delete Url Redirect.
        /// </summary>
        /// <param name="urlRedirectIds">Url Redirect Id.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        [ResponseType(typeof(UrlRedirectResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage Delete([FromBody] ParameterModel urlRedirectIds)
        {
            HttpResponseMessage response;

            try
            {
                //Delete Url Redirect.
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.Delete(urlRedirectIds) });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new UrlRedirectResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new UrlRedirectResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }
        #endregion
    }
}
