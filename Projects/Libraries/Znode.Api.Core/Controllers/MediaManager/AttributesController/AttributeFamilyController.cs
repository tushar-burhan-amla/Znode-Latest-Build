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
    public class AttributeFamilyController : BaseController
    {
        #region Private Variables
        private readonly IAttributeFamilyCache _cache;
        private readonly IAttributeFamilyService _service;
        #endregion

        #region Constructor
        public AttributeFamilyController(IAttributeFamilyService service)
        {
            _service = service;
            _cache = new AttributeFamilyCache(_service);
        }
        #endregion

        #region Public Action Methods
        /// <summary>
        /// List of Media Attribute Family.
        /// </summary>
        /// <returns>Returns list of media attribute family.</returns>
        [ResponseType(typeof(AttributeFamilyListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage List()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetAttributeFamilyList(RouteUri, RouteTemplate);
                response = !String.IsNullOrEmpty(data) ? CreateOKResponse<AttributeFamilyListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(),TraceLevel.Error);
                AttributeFamilyListResponse data = new AttributeFamilyListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Create Media Attribute Family.
        /// </summary>
        /// <param name="model">AttributeFamilyModel.</param>
        /// <returns>Returns created attribute family.</returns>
        [ResponseType(typeof(AttributeFamilyResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage Create([FromBody] AttributeFamilyModel model)
        {
            HttpResponseMessage response;
            try
            {
                AttributeFamilyModel attributeFamily = _service.Create(model);
                if (!Equals(attributeFamily, null))
                {
                    response = CreateCreatedResponse(new AttributeFamilyResponse { AttributeFamily = attributeFamily });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(attributeFamily.MediaAttributeFamilyId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(),TraceLevel.Warning);
                AttributeFamilyResponse data = new AttributeFamilyResponse { HasError = true, ErrorMessage = ex.ErrorMessage, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                AttributeFamilyResponse data = new AttributeFamilyResponse { HasError = true, ErrorMessage = ex.Message};
                response = CreateInternalServerErrorResponse(data);
            }

            return response;
        }

        /// <summary>
        /// Get assigned attribute group list.
        /// </summary>
        /// <returns>Returns list of assigned attribute groups.</returns>
        [ResponseType(typeof(AttributeGroupListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAssignedAttributeGroups()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetAssignedAttributeGroups(RouteUri, RouteTemplate);
                response = !String.IsNullOrEmpty(data) ? CreateOKResponse<AttributeGroupListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(),TraceLevel.Error);
                AttributeGroupListResponse data = new AttributeGroupListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.NotFound };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Associate attribute groups to attribute family.
        /// </summary>
        /// <param name="listModel">FamilyGroupAttributeListModel.</param>
        /// <returns>Returns true if associated successfully else return false.</returns>        
        [ResponseType(typeof(FamilyGroupAttributeResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage AssociateAttributeGroup([FromBody] FamilyGroupAttributeListModel listModel)
        {
            HttpResponseMessage response;
            try
            {
                bool status = _service.AssignAttributeGroups(listModel);
                response = status ? CreateNoContentResponse() : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(),TraceLevel.Warning);
                FamilyGroupAttributeResponse data = new FamilyGroupAttributeResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                FamilyGroupAttributeResponse data = new FamilyGroupAttributeResponse { HasError = true, ErrorMessage = ex.Message};
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Unassociate attribute group from attribute family.
        /// </summary>
        /// <param name="attributeFamilyId">Attribute Family Id from which attribute groups have to be unassociated.</param>
        /// <param name="attributeGroupId">Attribute Group Id which is to be unassociated.</param>
        /// <returns>Unassociate attribute group from attribute family in return</returns>
        [HttpDelete]
        public virtual HttpResponseMessage UnAssociateAttributeGroup(int attributeFamilyId, int attributeGroupId)
        {
            HttpResponseMessage response;
            try
            {
                response = _service.UnAssignAttributeGroups(attributeFamilyId, attributeGroupId) ? CreateNoContentResponse() : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(),TraceLevel.Warning);
                FamilyGroupAttributeResponse data = new FamilyGroupAttributeResponse { HasError = true, ErrorMessage = ex.ErrorMessage, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Warning);
                FamilyGroupAttributeResponse data = new FamilyGroupAttributeResponse { HasError = true, ErrorMessage = ex.Message};
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Delete attribute family.
        /// </summary>
        /// <param name="attributeFamilyId">Attribute Family Id.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage Delete([FromBody] ParameterModel attributeFamilyId)
        {
            HttpResponseMessage response;

            try
            {
                bool deleted = _service.DeleteAttributeFamily(attributeFamilyId);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Warning);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(),TraceLevel.Error);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.AssociationDeleteError };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Get attribute family on the basis of attribute family id.
        /// </summary>
        /// <param name="attributeFamilyId">Attribute Family Id.</param>
        /// <returns>Returns attribute family model.</returns>
        [ResponseType(typeof(AttributeFamilyResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAttributeFamily(int attributeFamilyId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetAttributeFamily(attributeFamilyId, RouteUri, RouteTemplate);
                response = !String.IsNullOrEmpty(data) ? CreateOKResponse<AttributeFamilyResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(),TraceLevel.Error);
                AttributeFamilyResponse data = new AttributeFamilyResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Get attributes by attribute group id.
        /// </summary>
        /// <param name="attributeGroupId">Attribute group ids.</param>
        /// <returns>Returns list of attributes.</returns>
        [ResponseType(typeof(AttributeGroupListResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage GetAttributesByGroupIds([FromBody] ParameterModel attributeGroupId)
        {
            HttpResponseMessage response;
            try
            {
                AttributeGroupMapperListModel data = _service.GetAttributesByGroupIds(attributeGroupId);
                response = CreateCreatedResponse(new AttributeGroupListResponse { AttributeGroupMappers = data });
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(data.AttributeGroupMappers)));
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(),TraceLevel.Error);
                AttributeGroupListResponse data = new AttributeGroupListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.NotFound };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Gets list of unassigned attribute groups.
        /// </summary>
        /// <returns>Returns list of unassigned attribute groups.</returns>
        [ResponseType(typeof(AttributeGroupListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetUnAssignedAttributeGroups()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetUnAssignedAttributeGroups(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<AttributeGroupListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(),TraceLevel.Error);
                AttributeGroupListResponse data = new AttributeGroupListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.NotFound };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        #region Family Locale

        /// <summary>
        /// Gets list of family locales.
        /// </summary>
        /// <returns>Returns family locales.</returns>
        [ResponseType(typeof(FamilyLocaleListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetFamilyLocale(int attributeFamilyId)
        {
            HttpResponseMessage response;
            try
            {
                //Get attribute types.
                string data = _cache.GetFamilyLocale(attributeFamilyId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<FamilyLocaleListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(),TraceLevel.Error);
                FamilyLocaleListResponse data = new FamilyLocaleListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Save locales.
        /// </summary>
        /// <param name="model">FamilyLocaleListModel model.</param>
        /// <returns>Returns saved locales.</returns>
        [ResponseType(typeof(FamilyLocaleListResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage SaveLocales([FromBody] FamilyLocaleListModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Create attribute group.
                FamilyLocaleListModel attributeFamilyLocales = _service.SaveLocales(model);
                response = !Equals(attributeFamilyLocales, null) ? CreateOKResponse(new FamilyLocaleListResponse { FamilyLocales = attributeFamilyLocales }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Warning);
                FamilyLocaleListResponse data = new FamilyLocaleListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode};
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(),TraceLevel.Error);
                FamilyLocaleListResponse data = new FamilyLocaleListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }
        #endregion

        #endregion
    }
}