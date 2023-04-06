using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Api.Controllers
{
    public class PortalProfileController : BaseController
    {
        #region Private Variables
        private readonly IPortalProfileCache _cache;
        private readonly IPortalProfileService _service;
        #endregion

        #region Constructor
        public PortalProfileController(IPortalProfileService service)
        {
            _service = service;
            _cache = new PortalProfileCache(_service);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Get the list of profiles associated to portal.
        /// </summary>
        /// <returns>Return List of profile associated to that portal.</returns>
        [ResponseType(typeof(PortalProfileListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage List()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetPortalProfiles(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PortalProfileListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                PortalProfileListResponse data = new PortalProfileListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Get profile associated to portal by portalProfile Id.
        /// </summary>
        /// <param name="portalProfileId">Id of portal profile.</param>
        /// <returns>Response with profile associated to portal by portalProfile Id.</returns>
        [ResponseType(typeof(PortalProfileResponse))]
        [HttpGet]
        public virtual HttpResponseMessage Get(int portalProfileId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetPortalProfile(portalProfileId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PortalProfileResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                PortalProfileResponse data = new PortalProfileResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Associate profile to portal
        /// </summary>
        /// <param name="model">PortalProfileModel</param>
        /// <returns>Response with successfully or unsuccessfully Associate profile to portal.</returns>
        [ResponseType(typeof(PortalProfileResponse))]
        [HttpPost]
        public virtual HttpResponseMessage Create([FromBody]PortalProfileModel model)
        {
            HttpResponseMessage response;
            try
            {
                PortalProfileModel portalProfile = _service.CreatePortalProfile(model);
                if (!Equals(portalProfile, null))
                {
                    response = CreateCreatedResponse(new PortalProfileResponse { PortalProfile = portalProfile });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(portalProfile.PortalProfileID)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (Exception ex)
            {
                PortalProfileResponse portalProfile = new PortalProfileResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(portalProfile);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Update portal profile details.
        /// </summary>
        /// <param name="model">PortalProfileModel</param>
        /// <returns>Return true if updated successfully else false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPut]
        public virtual HttpResponseMessage Update([FromBody]PortalProfileModel model)
        {
            HttpResponseMessage response;
            try
            {
                bool isSuccess = _service.UpdatePortalProfile(model);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = isSuccess });
            }
            catch (Exception ex)
            {
                PortalProfileResponse portalProfile = new PortalProfileResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(portalProfile);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Delete Portal profile association.
        /// </summary>
        /// <param name="portalProfileIds">Ids of portal profile to un-associate.</param>
        /// <returns>Return true if deleted successfully else false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage Delete(ParameterModel portalProfileIds)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.DeletePortalProfile(portalProfileIds) });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
            return response;
        }
        #endregion
    }
}
