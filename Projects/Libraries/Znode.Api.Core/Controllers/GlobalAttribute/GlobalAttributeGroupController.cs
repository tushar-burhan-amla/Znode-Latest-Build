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
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Api.Controllers
{
    public class GlobalAttributeGroupController : BaseController
    {
        #region Private Variables
        private readonly IGlobalAttributeGroupCache _cache;
        private readonly IGlobalAttributeGroupService _service;
        #endregion

        #region Constructor
        public GlobalAttributeGroupController(IGlobalAttributeGroupService service)
        {
            _service = service;
            _cache = new GlobalAttributeGroupCache(_service);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets the list of attribute group.
        /// </summary>
        /// <returns>Returns the list of attribute group.</returns>
        [ResponseType(typeof(GlobalAttributeGroupListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage List()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetAttributeGroupList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<GlobalAttributeGroupListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(),TraceLevel.Error);
                GlobalAttributeGroupListResponse data = new GlobalAttributeGroupListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Create new group.
        /// </summary>
        /// <param name="model">model to create.</param>
        /// <returns>Returns created model.</returns>
        [ResponseType(typeof(GlobalAttributeGroupResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage Create([FromBody] GlobalAttributeGroupModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Create global attribute group.
                GlobalAttributeGroupModel attributeGroup = _service.Create(model);
                if (!Equals(attributeGroup, null))
                {
                    response = CreateCreatedResponse(new GlobalAttributeGroupResponse { AttributeGroup = attributeGroup });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(attributeGroup.GlobalAttributeGroupId)));
                }
                else
                {
                    response = CreateInternalServerErrorResponse();
                }
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(),TraceLevel.Warning);
                GlobalAttributeGroupResponse data = new GlobalAttributeGroupResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
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
        /// Get the attribute group.
        /// </summary>
        /// <param name="id">Attributes group id.</param>
        /// <returns>Return the attribute group.</returns>
        [ResponseType(typeof(GlobalAttributeGroupResponse))]
        [HttpGet]
        public virtual HttpResponseMessage Get(int id)
        {
            HttpResponseMessage response;

            try
            {
                //Get global attribute group by ID.
                string data = _cache.GetAttributeGroup(id, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<GlobalAttributeGroupResponse>(data) : CreateNoContentResponse();
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
        /// Gets list of attribute group locales.
        /// </summary>
        /// <param name="attributeGroupId">attribute group id.</param>
        /// <returns>Returns attribute group locales.</returns>
        [ResponseType(typeof(GlobalAttributeGroupListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAttributeGroupLocale(int attributeGroupId)
        {
            HttpResponseMessage response;
            try
            {
                //Get attribute types.
                string data = _cache.GetAttributeGroupLocales(attributeGroupId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<GlobalAttributeGroupListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(),TraceLevel.Error);
                GlobalAttributeGroupListResponse data = new GlobalAttributeGroupListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Update existing group.
        /// </summary>
        /// <param name="model">Model to update.</param>
        /// <returns>Returns updated model.</returns>
        [ResponseType(typeof(GlobalAttributeGroupResponse))]
        [HttpPut]
        public virtual HttpResponseMessage Update([FromBody] GlobalAttributeGroupModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Update global AttributeGroup.
                var attributeGroup = _service.Update(model);
                response = attributeGroup ? CreateCreatedResponse(new GlobalAttributeGroupResponse { AttributeGroup = model, ErrorCode = 0 }) : CreateInternalServerErrorResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(model.GlobalAttributeGroupId)));
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
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
        /// Delete Global Attribute Group.
        /// </summary>
        /// <param name="globalAttributeGroupids">Group ids to delete.</param>
        /// <returns></returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage Delete(ParameterModel globalAttributeGroupids)
        {
            HttpResponseMessage response;
            try
            {
                //Delete attribute group
                bool deleted = _service.Delete(globalAttributeGroupids);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(),TraceLevel.Warning);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(),TraceLevel.Error);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Get the list of assigned attributes.
        /// </summary>
        /// <returns>Returns the list of assigned attributes.</returns>
        [ResponseType(typeof(GlobalAttributeGroupListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage AssignedAttributes()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.AssignedAttributes(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<GlobalAttributeGroupListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(),TraceLevel.Error);
                var data = new GlobalAttributeGroupListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Get the list of unassigned attributes.
        /// </summary>
        /// <returns>Returns the list of unassigned attributes.</returns>
        [ResponseType(typeof(GlobalAttributeListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage UnAssignedAttributes()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.UnAssignedAttributes(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<GlobalAttributeListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(),TraceLevel.Error);
                var data = new GlobalAttributeListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Associate attributes.
        /// </summary>
        /// <param name="model">Global attribute group mapper list model.</param>
        /// <returns>Returns the inserted records.</returns>
        [ResponseType(typeof(GlobalAttributeGroupListResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage AssociateAttributes([FromBody] GlobalAttributeGroupMapperListModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Create global attribute group.
                GlobalAttributeGroupMapperListModel attributeGroupLocales = _service.AssociateAttributes(model);
                response = IsNotNull(attributeGroupLocales) ? CreateOKResponse(new GlobalAttributeGroupListResponse { AttributeGroupMappers = attributeGroupLocales }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(),TraceLevel.Error);
                GlobalAttributeGroupListResponse data = new GlobalAttributeGroupListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Remove associated attributes.
        /// </summary>
        /// <param name="model">model contains data to remove.</param>
        /// <returns></returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage RemoveAssociatedAttributes([FromBody] RemoveGroupAttributesModel model)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.RemoveAssociatedAttributes(model) });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(),TraceLevel.Warning);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(),TraceLevel.Error);
                response = CreateInternalServerErrorResponse();
            }
            return response;
        }

        /// <summary>
        /// Update Attribute Display Order
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Return response</returns>
        [ResponseType(typeof(GlobalAttributeGroupResponse))]
        [HttpPut]
        public virtual HttpResponseMessage UpdateAttributeDisplayOrder([FromBody] GlobalAttributeDataModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Update global Attribute.
                bool attribute = _service.UpdateAttributeDisplayOrder(model);
                response = attribute ? CreateCreatedResponse(new GlobalAttributeDataResponse { GlobalAttributeDataModel = model, ErrorCode = 0 }) : CreateInternalServerErrorResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(model.AttributeModel.GlobalAttributeId)));

            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                GlobalAttributeDataResponse data = new GlobalAttributeDataResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(),TraceLevel.Error);
            }
            return response;
        }
        #endregion
    }
}
