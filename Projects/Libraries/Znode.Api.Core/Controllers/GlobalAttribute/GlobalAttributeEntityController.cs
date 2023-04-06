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
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Api.Controllers
{
    public class GlobalAttributeEntityController : BaseController
    {
        #region Private readonly Variables
        private readonly IGlobalAttributeGroupEntityService _service;
        private readonly IGlobalAttributeEntityCache _cache;
        #endregion

        #region Public Constructor
        public GlobalAttributeEntityController(IGlobalAttributeGroupEntityService service)
        {
            _service = service;
            _cache = new GlobalAttributeEntityCache(_service);
        }
        #endregion

        /// <summary>
        /// Get all entity list.
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(GlobalEntityListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAllEntityList()
        {
            HttpResponseMessage response;
            try
            {
                //Get account list.
                string data = _cache.GetAllEntityList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<GlobalEntityListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex,string.Empty,TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new GlobalEntityListResponse { HasError = true, ErrorMessage = ex.Message });
            }

            return response;
        }

        /// <summary>
        /// Get assigned entity group list.
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(GlobalEntityListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAssignedEntityAttributeGroups()
        {
            HttpResponseMessage response;
            try
            {
                //Get account list.
                string data = _cache.GetAssignedEntityAttributeGroups(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<AttributeEntityGroupListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new AttributeEntityGroupListResponse { HasError = true, ErrorMessage = ex.Message });
            }

            return response;
        }

        /// <summary>
        /// Get unassigned entity group list.
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(GlobalEntityListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetUnAssignedEntityAttributeGroups()
        {
            HttpResponseMessage response;
            try
            {
                //Get account list.
                string data = _cache.GetUnAssignedEntityAttributeGroups(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<AttributeEntityGroupListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new AttributeEntityGroupListResponse { HasError = true, ErrorMessage = ex.Message });
            }

            return response;
        }

        /// <summary>
        /// Associate entity to groups.   
        /// </summary>
        /// <param name="globalAttributeGroupEntityModel"></param>
        /// <returns></returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage AssociateAttributeEntityToGroups([FromBody]GlobalAttributeGroupEntityModel globalAttributeGroupEntityModel)
        {
            HttpResponseMessage response;
            try
            {
                bool status = _service.AssignEntityGroups(globalAttributeGroupEntityModel);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = status });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message,ErrorCode= ErrorCodes.NotFound });
            }
            return response;
        }

        /// <summary>
        /// Update Attribute Group Display Order
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        [ResponseType(typeof(GlobalAttributeGroupResponse))]
        [HttpPut]
        public virtual HttpResponseMessage UpdateAttributeGroupDisplayOrder([FromBody] GlobalAttributeGroupModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Update global AttributeGroup.
                var attributeGroup = _service.UpdateAttributeGroupDisplayOrder(model);
                response = attributeGroup ? CreateCreatedResponse(new GlobalAttributeGroupResponse { AttributeGroup = model, ErrorCode = 0 }) : CreateInternalServerErrorResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(model.GlobalAttributeGroupId)));
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(),TraceLevel.Error);
                GlobalAttributeGroupResponse data = new GlobalAttributeGroupResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Un associate group from entity.
        /// </summary>
        /// <param name="entityId">Entity Id.</param>
        /// <param name="groupId">Group Id.</param>
        /// <returns>Returns true or false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage UnAssociateEntityGroups(int entityId, int groupId)
        {
            HttpResponseMessage response;
            try
            {
                bool status = _service.UnAssignEntityGroup(entityId, groupId);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = status });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Create/Update EntityAttribute.
        /// </summary>
        /// <param name="model"> entityattribute model</param>
        /// <returns>Return entityattributemodel .</returns>
        [ResponseType(typeof(EntityAttributeResponse))]
        [HttpPost]
        public virtual HttpResponseMessage SaveEntityAttributeDetails([FromBody] EntityAttributeModel model)
        {
            HttpResponseMessage response;

            try
            {
                var entityAttribute = _service.SaveEntityAttributeDetails(model);
                if (!Equals(entityAttribute, null))
                {
                    response = CreateCreatedResponse(new EntityAttributeResponse { EntityAttribute = entityAttribute });

                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(entityAttribute.EntityValueId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                EntityAttributeResponse data = new EntityAttributeResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(),TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new EntityAttributeResponse { HasError = true, ErrorMessage = ex.Message,ErrorCode= ErrorCodes.NotFound });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
            }

            return response;
        }


        /// <summary>
        /// Get Attribute Details for the Entity.
        /// </summary>
        /// <param name="entityId">Uses entity Id.</param>
        /// <param name="entityType">Uses entity Type.</param>
        /// <returns>Returns attributes Values for the Entity.</returns>
        [ResponseType(typeof(GlobalAttributeEntityResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetEntityAttributeDetails(int entityId, string entityType)
        {
            HttpResponseMessage response;
            try
            {
                //Get attribute types.
                string data = _cache.GetEntityAttributeDetails(entityId, entityType, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<GlobalAttributeEntityResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(),TraceLevel.Error);
                GlobalAttributeEntityResponse data = new GlobalAttributeEntityResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Get publish attributes.
        /// </summary>
        /// <param name="entityId">Uses entity Id.</param>
        /// <param name="entityType">Uses entity Type.</param>
        /// <returns>Returns publish global attributes.</returns>
        [ResponseType(typeof(GlobalAttributeEntityResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetGlobalEntityAttributes(int entityId, string entityType)
        {
            HttpResponseMessage response;
            try
            {
                //Get attributes data.
                string data = _cache.GetGlobalEntityAttributes(entityId, entityType, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<GlobalAttributeEntityResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(),TraceLevel.Error);
                GlobalAttributeEntityResponse data = new GlobalAttributeEntityResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// gets the global attributes based on the passed familyCode for setting the values for default container variant.
        /// </summary>
        /// <param name="familyCode">Uses family Code.</param>
        /// <param name="entityType">Uses entity Type.</param>
        /// <returns>Returns attributes Values for the Entity.</returns>
        [ResponseType(typeof(GlobalAttributeEntityResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetGlobalAttributesForDefaultVariantData(string familyCode, string entityType)
        {
            HttpResponseMessage response;
            try
            {
                //Get attribute types.
                string data = _cache.GetGlobalAttributesForDefaultVariantData(familyCode, entityType, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<GlobalAttributeEntityResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                GlobalAttributeEntityResponse data = new GlobalAttributeEntityResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Get Global Attribute details on the basis of variantId id and localeid
        /// </summary>
        /// <param name="variantId">variantId</param>
        /// <param name="localeId">localeId</param>
        /// <param name="entityType"> entity Type.</param>
        /// <returns>Returns attributes Values for the Entity.</returns>
        [ResponseType(typeof(GlobalAttributeEntityResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetGlobalAttributesForAssociatedVariant(int variantId, string entityType, int localeId = 0)
        {
            HttpResponseMessage response;
            try
            {
                //Get attribute types.
                string data = _cache.GetGlobalAttributesForAssociatedVariant(variantId, entityType, RouteUri, RouteTemplate, localeId);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<GlobalAttributeEntityResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                GlobalAttributeEntityResponse data = new GlobalAttributeEntityResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }
    }
}
