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
    public class PIMAttributeGroupController : BaseController
    {
        #region Private Variables
        private readonly IPIMAttributeGroupCache _cache;
        private readonly IPIMAttributeGroupService _service;
        #endregion

        #region Constructor
        public PIMAttributeGroupController(IPIMAttributeGroupService service)
        {
            _service = service;
            _cache = new PIMAttributeGroupCache(_service);
        }
        #endregion

        #region Controller Actions
        /// <summary>
        /// Gets the list of attribute group.
        /// </summary>
        /// <returns>Returns the list of attribute group.</returns>
        [ResponseType(typeof(PIMAttributeGroupListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage List()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetAttributeGroupList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PIMAttributeGroupListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                PIMAttributeGroupListResponse data = new PIMAttributeGroupListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Gets the attribute group.
        /// </summary>
        /// <param name="id">Attributes group id.</param>
        /// <returns>Return the attribute group.</returns>
        [ResponseType(typeof(PIMAttributeGroupResponse))]
        [HttpGet]
        public virtual HttpResponseMessage Get(int id)
        {
            HttpResponseMessage response;

            try
            {
                //Get PIM attribute group by ID.
                string data = _cache.GetAttributeGroup(id, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PIMAttributeGroupResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                PIMAttributeGroupResponse data = new PIMAttributeGroupResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Gets the list of assigned attributes.
        /// </summary>
        /// <returns>Returns the list of assigned attributes.</returns>
        [ResponseType(typeof(PIMAttributeGroupListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage AssignedAttributes()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.AssignedAttributes(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PIMAttributeGroupListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                var data = new PIMAttributeGroupListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Gets the list of unassigned attributes.
        /// </summary>
        /// <returns>Returns the list of unassigned attributes.</returns>
        [ResponseType(typeof(PIMAttributeListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage UnAssignedAttributes()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.UnAssignedAttributes(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PIMAttributeListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                var data = new PIMAttributeListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Create new group.
        /// </summary>
        /// <param name="model">model to create.</param>
        /// <returns>Returns created model.</returns>
        [ResponseType(typeof(PIMAttributeGroupResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage Create([FromBody] PIMAttributeGroupModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Create PIM attribute group.
                PIMAttributeGroupModel attributeGroup = _service.Create(model);
                if (!Equals(attributeGroup, null))
                {
                    response = CreateCreatedResponse(new PIMAttributeGroupResponse { AttributeGroup = attributeGroup });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(attributeGroup.PimAttributeGroupId)));
                }
                else
                {
                    response = CreateInternalServerErrorResponse();
                }
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                PIMAttributeGroupResponse data = new PIMAttributeGroupResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                PIMAttributeGroupResponse data = new PIMAttributeGroupResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Update existing group.
        /// </summary>
        /// <param name="model">model to update.</param>
        /// <returns>Returns updated model.</returns>
        [ResponseType(typeof(PIMAttributeGroupResponse))]
        [HttpPut]
        public virtual HttpResponseMessage Update([FromBody] PIMAttributeGroupModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Update PIM AttributeGroup.
                var attributeGroup = _service.UpdateAttributeGroup(model);
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

        /// <summary>
        /// Update Attribute Display Order
        /// </summary>
        /// <param name="model"> PIM Attribute Data Model</param>
        /// <returns>return response</returns>
        [ResponseType(typeof(PIMAttributeGroupResponse))]
        [HttpPut]
        public virtual HttpResponseMessage UpdateAttributeDisplayOrder([FromBody] PIMAttributeDataModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Update PIM Attribute.
                bool attribute = _service.UpdateAttributeDisplayOrder(model);
                response = attribute ? CreateCreatedResponse(new PIMAttributeDataResponse { PIMAttributeDataModel = model, ErrorCode = 0 }) : CreateInternalServerErrorResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(model.AttributeModel.PimAttributeId)));

            }
            catch (Exception ex)
            {
                PIMAttributeDataResponse data = new PIMAttributeDataResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Delete existing group.
        /// </summary>
        /// <param name="pimAttributeGroupids">Group ids to delete.</param>
        /// <returns>Return true if deleted successfully else false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage Delete(ParameterModel pimAttributeGroupids)
        {
            HttpResponseMessage response;
            try
            {
                //Delete attribute.
                bool deleted = _service.Delete(pimAttributeGroupids);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Gets list of attribute group locales.
        /// </summary>
        /// <param name="attributeGroupId">attribute group id.</param>
        /// <returns>Returns attribute group locales.</returns>
        [ResponseType(typeof(PIMAttributeGroupListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAttributeGroupLocale(int attributeGroupId)
        {
            HttpResponseMessage response;
            try
            {
                //Get attribute types.
                string data = _cache.GetAttributeGroupLocales(attributeGroupId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PIMAttributeGroupListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                PIMAttributeGroupListResponse data = new PIMAttributeGroupListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Associate attributes.
        /// </summary>
        /// <param name="model">PIM attribute group mapper list model.</param>
        /// <returns>Returns the inserted records.</returns>
        [ResponseType(typeof(PIMAttributeGroupListResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage AssociateAttributes([FromBody] PIMAttributeGroupMapperListModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Create PIM attribute group.
                PIMAttributeGroupMapperListModel attributeGroupLocales = _service.AssociateAttributes(model);
                response = !Equals(attributeGroupLocales, null) ? CreateOKResponse(new PIMAttributeGroupListResponse { AttributeGroupMappers = attributeGroupLocales }) : CreateInternalServerErrorResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                PIMAttributeGroupListResponse data = new PIMAttributeGroupListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Save attribute group locales.
        /// </summary>
        /// <param name="model">model to save.</param>
        /// <returns>Returns saved model.</returns>
        [ResponseType(typeof(PIMAttributeGroupListResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage SaveAttributeGroupLocales([FromBody] PIMAttributeGroupLocaleListModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Create PIM attribute group.
                PIMAttributeGroupLocaleListModel attributeGroupLocales = _service.SaveAttributeGroupLocales(model);
                response = !Equals(attributeGroupLocales, null) ? CreateOKResponse(new PIMAttributeGroupListResponse { AttributeGroupLocales = attributeGroupLocales }) : CreateInternalServerErrorResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                PIMAttributeGroupListResponse data = new PIMAttributeGroupListResponse { HasError = true, ErrorMessage = ex.Message };
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
        public virtual HttpResponseMessage RemoveAssociatedAttributes([FromBody] RemoveAssociatedAttributesModel model)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.RemoveAssociatedAttributes(model) });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Warning);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.PIM.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse();
            }
            return response;
        }
        #endregion
    }
}