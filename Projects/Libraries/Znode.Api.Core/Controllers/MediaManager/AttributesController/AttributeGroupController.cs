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
    public class AttributeGroupController : BaseController
    {
        #region Private Variables
        private readonly IAttributeGroupService _service;
        private readonly IAttributeGroupCache _cache;
        #endregion

        #region Default Constructor
        public AttributeGroupController(IAttributeGroupService service)
        {
            _service = service;
            _cache = new AttributeGroupCache(_service);
        }
        #endregion

        /// <summary>
        /// Gets list of attributeGroups.
        /// </summary>
        /// <returns> Returns list of attributeGroups</returns>
        [ResponseType(typeof(AttributeGroupListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage List()
        {
            HttpResponseMessage response;

            try
            {
                //Get list of menus.
                string data = _cache.GetAttributeGroups(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<AttributeGroupListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(),TraceLevel.Error);
                AttributeGroupListResponse data = new AttributeGroupListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }

            return response;
        }

        /// <summary>
        /// Method to create attributeGroups.
        /// </summary>
        /// <param name="model">AttributeGroup model.</param>
        /// <returns>Returns created attributeGroup.</returns>
        [ResponseType(typeof(AttributeGroupResponse))]
        [HttpPost]
        public virtual HttpResponseMessage Create([FromBody]AttributeGroupModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Create attributeGroup.
                var attributeGroup = _service.CreateAttributeGroup(model);
                if (!Equals(attributeGroup, null))
                {
                    response = CreateCreatedResponse(new AttributeGroupResponse { AttributeGroup = attributeGroup });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(attributeGroup.MediaAttributeGroupId)));
                }
                else
                {
                    response = CreateInternalServerErrorResponse();
                }
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Warning);
                AttributeGroupResponse data = new AttributeGroupResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(),TraceLevel.Error);
                AttributeGroupResponse data = new AttributeGroupResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }

            return response;
        }

        /// <summary>
        /// Gets a group.
        /// </summary>
        /// <param name="attributeGroupId">The ID of the group.</param>
        /// <returns>Returns the attribute group.</returns>
        [ResponseType(typeof(AttributeGroupResponse))]
        [HttpGet]
        public virtual HttpResponseMessage Get(int attributeGroupId)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetAttributeGroup(attributeGroupId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<AttributeGroupResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                AttributeGroupResponse data = new AttributeGroupResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                AttributeGroupResponse data = new AttributeGroupResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(),TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Method to update attribute group.
        /// </summary>
        /// <param name="model">AttributeGroupModel model.</param>
        /// <returns>Returns updated attribute group.</returns>
        [ResponseType(typeof(AttributeGroupResponse))]
        [HttpPut]
        public virtual HttpResponseMessage Update([FromBody] AttributeGroupModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Update attribute local.
                bool status = _service.UpdateAttributeGroup(model);
                response = status ? CreateCreatedResponse(new AttributeGroupResponse { AttributeGroup = model, ErrorCode = 0 }) : CreateInternalServerErrorResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(model.MediaAttributeGroupId)));
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Warning);
                AttributeGroupResponse data = new AttributeGroupResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(),TraceLevel.Error);
                AttributeGroupResponse data = new AttributeGroupResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }

            return response;
        }

        /// <summary>
        /// Method to delete attribute group.
        /// </summary>
        /// <param name="attributeGroupIds">AttributeGroupModel model.</param>
        /// <returns>Returns updated attribute group.</returns>
        [ResponseType(typeof(AttributeGroupResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage Delete(ParameterModel attributeGroupIds)
        {
            HttpResponseMessage response;
            try
            {
                //Delete attribute.
                bool deleted = _service.DeleteAttributeGroup(attributeGroupIds);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(),TraceLevel.Warning);
                AttributeGroupResponse data = new AttributeGroupResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(),TraceLevel.Error);
                AttributeGroupResponse data = new AttributeGroupResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Assign attributes to attribute group.
        /// </summary>
        /// <param name="model">Attribute group mapper models.</param>
        /// <returns>Http response containing boolean value whether attributes are associated or not.</returns>
        [ResponseType(typeof(AttributeGroupListResponse))]
        [HttpPost]
        public virtual HttpResponseMessage AssignAttributes([FromBody] AttributeGroupMapperListModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Assign Attribute to group.(Add entry in AttributeGroupMapper).
                AttributeGroupMapperListModel assignedAttributes = _service.AssignAttributes(model);
                response = assignedAttributes?.AttributeGroupMappers?.Count > 0 ? CreateOKResponse(new AttributeGroupListResponse { AttributeGroupMappers = assignedAttributes }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(),TraceLevel.Warning);
                AttributeGroupListResponse data = new AttributeGroupListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(),TraceLevel.Error);
                AttributeGroupListResponse data = new AttributeGroupListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Gets the list of assigned attributes.
        /// </summary>
        /// <returns>HTTP response containing list of assigned attributes.</returns>
        [ResponseType(typeof(AttributeGroupMapperListModel))]
        [HttpGet]
        public virtual HttpResponseMessage GetAssignedAttributes()
        {
            HttpResponseMessage response;

            try
            {
                //Get list of menus.
                string data = _cache.GetAssignedAttributes(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<AttributeGroupMapperListModel>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(),TraceLevel.Error);
                AttributeGroupMapperListResponse data = new AttributeGroupMapperListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }

            return response;
        }

        /// <summary>
        /// Updates an Attribute group mapper.
        /// </summary>
        /// <param name="model">Attribute group mapper to be updated.</param>
        /// <returns>Updated attribute group mapper model.</returns>
        [ResponseType(typeof(AttributeGroupMapperResponse))]
        [HttpPut]
        public virtual HttpResponseMessage UpdateAttributeGroupMapper([FromBody]AttributeGroupMapperModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Update attribute local.
                bool attributeGroupMapper = _service.UpdateAttributeGroupMapper(model);
                response = attributeGroupMapper ? CreateCreatedResponse(new AttributeGroupMapperResponse { AttributeGroupMapper = model, ErrorCode = 0 }) : CreateInternalServerErrorResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(model.MediaAttributeGroupMapperId)));
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Warning);
                AttributeGroupMapperResponse data = new AttributeGroupMapperResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(),TraceLevel.Error);
                AttributeGroupMapperResponse data = new AttributeGroupMapperResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }

            return response;

        }

        /// <summary>
        /// Deletes an attribute to attribute group association.
        /// </summary>
        /// <param name="attributeGroupMapperId">ID of the attribute group to be deleted.</param>
        /// <returns>Http response containing the status of delete operation.</returns>
        [HttpDelete]
        public virtual HttpResponseMessage DeleteAssociatedAttribute(int attributeGroupMapperId)
        {
            HttpResponseMessage response;
            try
            {
                //Delete attribute.
                var deleted = _service.DeleteAssociatedAttribute(attributeGroupMapperId);
                response = deleted ? CreateOKResponse() : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Warning);
                AttributeGroupMapperResponse data = new AttributeGroupMapperResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(),TraceLevel.Error);
                AttributeGroupMapperResponse data = new AttributeGroupMapperResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Gets list of attribute group locales.
        /// </summary>
        /// <param name="attributeGroupId">attribute group id.</param>
        /// <returns>Returns attribute group locales.</returns>
        [ResponseType(typeof(AttributeGroupListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAttributeGroupLocale(int attributeGroupId)
        {
            HttpResponseMessage response;
            try
            {
                //Get attribute types.
                string data = _cache.GetAttributeGroupLocales(attributeGroupId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<AttributeGroupListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(),TraceLevel.Error);
                AttributeGroupListResponse data = new AttributeGroupListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Gets the list of unassigned attributes.
        /// </summary>
        /// <returns>Returns the list of unassigned attributes.</returns>
        [ResponseType(typeof(AttributeListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetUnassignedAttributes(int attributeGroupId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.UnAssignedAttributes(attributeGroupId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<AttributeListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.MediaManager.ToString(), TraceLevel.Error);
                AttributeListResponse data = new AttributeListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);                
            }
            return response;
        }
    }
}