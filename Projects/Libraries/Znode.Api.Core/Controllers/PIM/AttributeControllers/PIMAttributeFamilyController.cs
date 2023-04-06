using System;
using System.Net.Http;
using System.Web.Http;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Helper;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using System.Web.Http.Description;
using Znode.Libraries.Framework.Business;
using System.Diagnostics;

namespace Znode.Engine.Api.Controllers
{
    public class PIMAttributeFamilyController : BaseController
    {
        #region Private Variables
        private readonly IPIMAttributeFamilyCache _cache;
        private readonly IPIMAttributeFamilyService _service;
        #endregion

        #region Constructor
        public PIMAttributeFamilyController(IPIMAttributeFamilyService service)
        {
            _service = service;
            _cache = new PIMAttributeFamilyCache(_service);
        }
        #endregion

        #region Public Action Methods

        /// <summary>
        /// Get list of Attribute Family.
        /// </summary>
        /// <returns>Returns list of Attribute Family.</returns>
        [ResponseType(typeof(PIMAttributeFamilyListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage List()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetAttributeFamilyList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PIMAttributeFamilyListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                PIMAttributeFamilyListResponse data = new PIMAttributeFamilyListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Get attribute family by attribute family id.
        /// </summary>
        /// <param name="attributeFamilyId">Attribute Family Id.</param>
        /// <returns>Returns an attribute family.</returns>
        [ResponseType(typeof(PIMAttributeFamilyResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAttributeFamily(int attributeFamilyId)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetAttributeFamily(attributeFamilyId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PIMAttributeFamilyResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                PIMAttributeFamilyResponse data = new PIMAttributeFamilyResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Get assigned attribute groups.
        /// </summary>
        /// <returns>Returns list of assigned attribute groups.</returns>
        [ResponseType(typeof(PIMAttributeGroupListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAssignedAttributeGroups()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetAssignedAttributeGroups(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PIMAttributeGroupListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                PIMAttributeGroupListResponse data = new PIMAttributeGroupListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.NotFound };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Create Attribute Family.
        /// </summary>
        /// <param name="model">PIMAttributeFamilyModel model.</param>
        /// <returns>Returns created model.</returns>
        [ResponseType(typeof(PIMAttributeFamilyResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage Create([FromBody] PIMAttributeFamilyModel model)
        {
            HttpResponseMessage response;

            try
            {
                PIMAttributeFamilyModel attributeFamily = _service.Create(model);
                if (!Equals(attributeFamily, null))
                {
                    response = CreateCreatedResponse(new PIMAttributeFamilyResponse { PIMAttributeFamily = attributeFamily });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(attributeFamily.PimAttributeFamilyId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                PIMAttributeFamilyResponse data = new PIMAttributeFamilyResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.NotFound };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Delete attribute family.
        /// </summary>
        /// <param name="attributeFamilyId">Attribute Family Id.</param>
        /// <returns>Return true if deleted successfully else false.</returns>
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
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.AssociationDeleteError };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Associate attribute group to family.
        /// </summary>
        /// <param name="listModel">PIMFamilyGroupAttributeListModel model.</param>
        /// <returns>Returns associated attribute groups.</returns>
        [ResponseType(typeof(PIMFamilyGroupAttributeResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage AssociateAttributeGroup([FromBody] PIMFamilyGroupAttributeListModel listModel)
        {
            HttpResponseMessage response;
            try
            {
                response = _service.AssignAttributeGroups(listModel) ? CreateNoContentResponse() : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                PIMFamilyGroupAttributeResponse data = new PIMFamilyGroupAttributeResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                PIMFamilyGroupAttributeResponse data = new PIMFamilyGroupAttributeResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// UnAssociate Attribute Group from family.
        /// </summary>
        /// <param name="attributeFamilyId">Attribute Family Id.</param>
        /// <param name="attributeGroupId">Attribute group Id.</param>
        /// <param name="isCategory">Category status</param>
        /// <returns>Returns true or false.</returns>
        [ResponseType(typeof(PIMFamilyGroupAttributeResponse))]
        [HttpDelete]
        public virtual HttpResponseMessage UnAssociateAttributeGroup(int attributeFamilyId, int attributeGroupId, bool isCategory)
        {
            HttpResponseMessage response;
            try
            {
                response = _service.UnAssignAttributeGroups(attributeFamilyId, attributeGroupId, isCategory) ? CreateNoContentResponse() : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                PIMFamilyGroupAttributeResponse data = new PIMFamilyGroupAttributeResponse { HasError = true, ErrorMessage = ex.ErrorMessage, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                PIMFamilyGroupAttributeResponse data = new PIMFamilyGroupAttributeResponse { HasError = true, ErrorMessage = ex.Message  };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Gets list of unassigned attribute groups.
        /// </summary>
        /// <returns>Response with list of unassigned attribute groups </returns>
        [ResponseType(typeof(PIMAttributeGroupListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetUnAssignedAttributeGroups()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetUnAssignedAttributeGroups(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PIMAttributeGroupListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                PIMAttributeGroupListResponse data = new PIMAttributeGroupListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.NotFound };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Gets list of family locales.
        /// </summary>
        /// <param name="attributeFamilyId">Attribute Family Id. </param>
        /// <returns>Returns family locales.</returns>
        [ResponseType(typeof(PIMFamilyLocaleListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetFamilyLocale(int attributeFamilyId)
        {
            HttpResponseMessage response;
            try
            {
                //Get attribute types.
                string data = _cache.GetFamilyLocale(attributeFamilyId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PIMFamilyLocaleListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                PIMFamilyLocaleListResponse data = new PIMFamilyLocaleListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Save locales.
        /// </summary>
        /// <param name="model">PIMFamilyLocaleListModel model.</param>
        /// <returns>Returns saved locales.</returns>
        [ResponseType(typeof(PIMFamilyLocaleListResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage SaveLocales([FromBody] PIMFamilyLocaleListModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Create PIM attribute group.
                PIMFamilyLocaleListModel attributeFamilyLocales = _service.SaveLocales(model);
                response = !Equals(attributeFamilyLocales, null) ? CreateOKResponse(new PIMFamilyLocaleListResponse { FamilyLocales = attributeFamilyLocales }) : CreateInternalServerErrorResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                PIMFamilyLocaleListResponse data = new PIMFamilyLocaleListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Get attributes by attribute group id.
        /// </summary>
        /// <param name="attributeGroupId">Attribute group ids.</param>
        /// <returns>Returns list of attributes.</returns>
        [ResponseType(typeof(PIMAttributeGroupListResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage GetAttributesByGroupIds([FromBody] ParameterModel attributeGroupId)
        {
            HttpResponseMessage response;
            try
            {
                PIMAttributeGroupMapperListModel data = _service.GetAttributesByGroupIds(attributeGroupId);
                response = CreateCreatedResponse(new PIMAttributeGroupListResponse { AttributeGroupMappers = data });
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(data.AttributeGroupMappers)));
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                PIMAttributeGroupListResponse data = new PIMAttributeGroupListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.NotFound };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Update Attribute Group Display Order
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [ResponseType(typeof(PIMAttributeGroupResponse))]
        [HttpPut]
        public virtual HttpResponseMessage UpdateAttributeGroupDisplayOrder([FromBody] PIMAttributeGroupModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Update PIM AttributeGroup.
                var attributeGroup = _service.UpdateAttributeGroupDisplayOrder(model);
                response = attributeGroup ? CreateCreatedResponse(new PIMAttributeGroupResponse { AttributeGroup = model, ErrorCode = 0 }) : CreateInternalServerErrorResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(model.PimAttributeGroupId)));
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                PIMAttributeGroupResponse data = new PIMAttributeGroupResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        #region Attributes
        /// <summary>
        /// Get assigned attribute groups.
        /// </summary>
        /// <returns>Returns list of assigned attributes.</returns>
        [ResponseType(typeof(PIMAttributeListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAssignedAttributes()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetAssignedAttributes(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PIMAttributeListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                PIMAttributeListResponse data = new PIMAttributeListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.NotFound };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Get list of unassigned attributes.
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(PIMAttributeListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetUnAssignedAttributes()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetUnAssignedAttributes(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PIMAttributeListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                PIMAttributeListResponse data = new PIMAttributeListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.NotFound };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Associate attributes to group.
        /// </summary>
        /// <param name="model">Attribute data model.</param>
        /// <returns>Returns associated attribute groups.</returns>
        [ResponseType(typeof(PIMFamilyGroupAttributeResponse))]
        [HttpPost]
        public virtual HttpResponseMessage AssignAttributes([FromBody] AttributeDataModel model)
        {
            HttpResponseMessage response;
            try
            {
                response = _service.AssignAttributes(model) ? CreateNoContentResponse() : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                PIMFamilyGroupAttributeResponse data = new PIMFamilyGroupAttributeResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                PIMFamilyGroupAttributeResponse data = new PIMFamilyGroupAttributeResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Unassociate attribute from group.
        /// </summary>
        /// <param name="model">Uses model with data.</param>
        /// <returns>Returns true or false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage UnAssignAttributes([FromBody] AttributeDataModel model)
        {
            HttpResponseMessage response;
            try
            {
                bool unassigned = _service.UnAssignAttributes(model);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = unassigned });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                PIMFamilyGroupAttributeResponse data = new PIMFamilyGroupAttributeResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                PIMFamilyGroupAttributeResponse data = new PIMFamilyGroupAttributeResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;     
        } 
        #endregion
        #endregion
    }
}
