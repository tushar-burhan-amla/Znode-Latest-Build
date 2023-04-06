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
    public class ProfileController : BaseController
    {
        #region Private Variables
        private readonly IProfileService _service;
        private readonly IProfileCache _cache;
        #endregion

        #region Default Constructor
        public ProfileController(IProfileService service)
        {
            _service = service;
            _cache = new ProfileCache(_service);
        }
        #endregion

        #region Public Methods
        #region Profile
        /// <summary>
        /// Create new profile.
        /// </summary>
        /// <param name="model">Profile model.</param>
        /// <returns>Returns created profile.</returns>
        [ResponseType(typeof(ProfileResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage CreateProfile([FromBody] ProfileModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Create profile.
                ProfileModel profile = _service.CreateProfile(model);
                if (!Equals(profile, null))
                {
                    response = CreateCreatedResponse(new ProfileResponse { Profile = profile });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(profile.ProfileId)));
                }
                else
                    response = CreateInternalServerErrorResponse();

            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Warning);
                ProfileResponse data = new ProfileResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
                ProfileResponse data = new ProfileResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }

            return response;

        }

        /// <summary>
        /// Update profile details.
        /// </summary>
        /// <param name="model">Profile model.</param>
        /// <returns>Returns updated profile.</returns>
        [ResponseType(typeof(ProfileResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage UpdateProfile([FromBody] ProfileModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Update profile.
                bool IsUpdated = _service.UpdateProfile(model);
                if (IsUpdated)
                {
                    response = CreateCreatedResponse(new ProfileResponse { Profile = model });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(model.ProfileId)));
                }
                else
                    response = CreateInternalServerErrorResponse();

            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Warning);
                ProfileResponse data = new ProfileResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
                ProfileResponse data = new ProfileResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }

            return response;

        }

        /// <summary>
        /// Gets list of profiles.
        /// </summary>
        /// <returns>Returns profile list.</returns>
        [ResponseType(typeof(ProfileListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetProfileList()
        {
            HttpResponseMessage response;
            try
            {
                //Get attributes.
                string data = _cache.GetProfiles(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ProfileListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
                ProfileListResponse data = new ProfileListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }

            return response;
        }

        /// <summary>
        /// Gets a profile by profile Id.
        /// </summary>
        /// <param name="profileId">The Id of the profile.</param>
        /// <returns>return profile.</returns>
        [ResponseType(typeof(ProfileResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetProfile(int profileId)
        {
            HttpResponseMessage response;
            try
            {
                //Get profile by profile id.
                string data = _cache.GetProfile(profileId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ProfileResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
                ProfileResponse data = new ProfileResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }

            return response;
        }

        /// <summary>
        /// Delete profile by profileId.
        /// </summary>
        /// <param name="profileId">Id of Profile.</param>
        /// <returns>return status.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage DeleteProfile([FromBody] ParameterModel profileId)
        {
            HttpResponseMessage response;
            try
            {
                //Delete profile.
                bool deleted = _service.DeleteProfile(profileId);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Warning);
                ProfileResponse data = new ProfileResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
                ProfileResponse data = new ProfileResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.AssociationDeleteError };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }
        #endregion

        #region Profile Catalog
        /// <summary>
        /// Gets list of profile catalog.
        /// </summary>
        /// <returns>Returns profile catalog list.</returns>
        [ResponseType(typeof(ProfileCatalogListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetProfileCatalogList()
        {
            HttpResponseMessage response;
            try
            {
                //Get profile catalogs.
                string data = _cache.GetProfileCatalogs(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ProfileCatalogListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
                ProfileCatalogListResponse data = new ProfileCatalogListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }

            return response;
        }

        /// <summary>
        /// Delete associated catalog to profile by profileId.
        /// </summary>
        /// <param name="profileId">Parameter Model</param>
        /// <returns>return Updated status after deleting corresponding Profile Catalog.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpGet]
        public virtual HttpResponseMessage DeleteAssociatedProfileCatalog(int profileId)
        {
            HttpResponseMessage response;
            try
            {
                //Delete profile.
                bool deleted = _service.DeleteAssociatedProfileCatalog(profileId);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Warning);
                ProfileResponse data = new ProfileResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
                ProfileResponse data = new ProfileResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Associate Catalog to profile.
        /// </summary>
        /// <param name="profileCatalogModel">ProfileCatalogModel.</param>
        /// <returns>Returns associated catalog to profile.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage AssociateCatalogToProfile([FromBody] ProfileCatalogModel profileCatalogModel)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateCreatedResponse(new TrueFalseResponse { IsSuccess = _service.AssociateCatalogToProfile(profileCatalogModel), ErrorCode = 0 });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }
        #endregion
        #endregion
    }
}