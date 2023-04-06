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
    public class PortalCountryController : BaseController
    {
        #region Private Variables
        private readonly IPortalCountryCache _cache;
        private readonly IPortalCountryService _service;
        #endregion

        #region Constructor
        public PortalCountryController(IPortalCountryService service)
        {
            _service = service;
            _cache = new PortalCountryCache(_service);
        }
        #endregion

        #region Public Methods
        #region Country Association
        /// <summary>
        /// Get list of unassociate countries.
        /// </summary>
        /// <returns>Unassociate countries list.</returns>
        [ResponseType(typeof(CountryListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetUnAssociatedCountryList()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetUnAssociatedCountryList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<CountryListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new CountryListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get list of associate countries based on portal.
        /// </summary>
        /// <returns>list of associate countries.</returns>
        [ResponseType(typeof(CountryListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAssociatedCountryList()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetAssociatedCountryList(RouteUri, RouteTemplate);
                // TODO: Add info logs
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<CountryListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new CountryListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Remove associated countries to portal.
        /// </summary>
        /// <param name="portalCountries">portalCountries contains portalCountryId to unassociate countries.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage UnAssociateCountries([FromBody] ParameterModelForPortalCountries portalCountries)
        {
            HttpResponseMessage response;

            try
            {
                //Remove associated countries.
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.UnAssociateCountries(portalCountries) });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new BaseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new BaseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Associate countries to portal.
        /// </summary>
        /// <param name="model">model with country ids and portal id.</param>
        /// <returns></returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage AssociateCountries([FromBody] ParameterModelForPortalCountries model)
        {
            HttpResponseMessage response;

            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.AssociateCountries(model) });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new BaseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new BaseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }
        #endregion
        #endregion
    }
}
