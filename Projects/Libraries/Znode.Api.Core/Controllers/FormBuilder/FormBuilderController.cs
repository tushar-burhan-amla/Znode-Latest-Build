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
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Api.Controllers
{
    public class FormBuilderController : BaseController
    {
        #region Private Variables
        private readonly IFormBuilderCache _cache;
        private readonly IFormBuilderService _service;
        #endregion

        #region Constructor
        public FormBuilderController(IFormBuilderService service)
        {
            _service = service;
            _cache = new FormBuilderCache(_service);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets list of Form Builder.
        /// </summary>
        /// <returns>Returns list of Form Builder.</returns>
        [HttpGet]
        [ResponseType(typeof(FormBuilderListResponse))]
        public virtual HttpResponseMessage List()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetFormBuilderList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<FormBuilderListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new FormBuilderListResponse { HasError = true, ErrorMessage = ex.Message });
            }

            return response;
        }

        /// <summary>
        /// Creates a new form.
        /// </summary>
        /// <param name="formBuilderModel">Model with form data.</param>
        /// <returns>Returns created form details.</returns>
        [ResponseType(typeof(FormBuilderResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage Create([FromBody] FormBuilderModel formBuilderModel)
        {
            HttpResponseMessage response;

            try
            {
                FormBuilderModel formModel = _service.CreateForm(formBuilderModel);
                if (HelperUtility.IsNotNull(formModel))
                {
                    response = CreateCreatedResponse(new FormBuilderResponse { FormBuilder = formModel });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(formModel.FormBuilderId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new FormBuilderResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new FormBuilderResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(),TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Get form details on the basis of form id.
        /// </summary>
        /// <param name="id">form id.</param>
        /// <returns>Return form details by id.</returns>
        [ResponseType(typeof(FormBuilderResponse))]
        [HttpGet]
        public virtual HttpResponseMessage Get(int id)
        {
            HttpResponseMessage response;
            try
            {
                //Get form details by id.
                string data = _cache.GetForm(id, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<FormBuilderResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new FormBuilderResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode});
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(),TraceLevel.Error);
                FormBuilderResponse data = new FormBuilderResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Delete FormBuilder.
        /// </summary>
        /// <param name="formBuilderId">FormBuilder Id.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage Delete(ParameterModel formBuilderId)
        {
            HttpResponseMessage response;

            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.DeleteFormBuilder(formBuilderId) });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new GiftCardResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex,string.Empty,TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new GiftCardResponse { HasError = true, ErrorMessage = ex.Message,ErrorCode= ErrorCodes.AssociationDeleteError });
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Check form code already exist or not.
        /// </summary>
        /// <param name="formCode">formCode</param>
        /// <returns>Returns true if form code already exist.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpGet]
        public virtual HttpResponseMessage IsFormCodeExist(string formCode)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.IsFormCodeExist(formCode) });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(),TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        ///  Get Form builder Attribute Group.
        /// </summary>
        /// <param name="formBuilderId">Int formbuilderId</param>
        /// <param name="localeId">Id localeId</param>
        /// <param name="mappingId">Int mappingId</param>
        /// <returns>Returns Form Builder Attribute Group Response.</returns>
        [ResponseType(typeof(FormBuilderAttributeGroupResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetFormAttributeGroup(int formBuilderId, int localeId, int mappingId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetFormAttributeGroup(formBuilderId, localeId, mappingId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<FormBuilderAttributeGroupResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(),TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new FormBuilderAttributeGroupResponse { HasError = true, ErrorMessage = ex.Message });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new FormBuilderAttributeGroupResponse { HasError = true, ErrorMessage = ex.Message });
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
        /// Get unassigned group list.
        /// </summary>
        /// <returns>Returns the list of unassigned groups.</returns>
        [ResponseType(typeof(AttributeEntityGroupListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetUnAssignedGroups()
        {
            HttpResponseMessage response;
            try
            {
                //Get unassigned group list.
                string data = _cache.GetUnAssignedGroups(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<AttributeEntityGroupListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex,string.Empty,TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new AttributeEntityGroupListResponse { HasError = true, ErrorMessage = ex.Message });
            }

            return response;
        }

        /// <summary>
        /// Associate groups to form.   
        /// </summary>
        /// <param name="globalAttributeGroupEntityModel">Model with groups data.</param>
        /// <returns>Returns true if associated successfully.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage AssociateGroups([FromBody]GlobalAttributeGroupEntityModel globalAttributeGroupEntityModel)
        {
            HttpResponseMessage response;
            try
            {
                bool status = _service.AssignGroupsToForm(globalAttributeGroupEntityModel);
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
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.NotFound });
            }
            return response;
        }

        /// <summary>
        /// Associate attributes to form.   
        /// </summary>
        /// <param name="globalAttributeGroupEntityModel">Model with attributes data.</param>
        /// <returns>Returns true if associated successfully.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage AssociateAttributes([FromBody]GlobalAttributeGroupEntityModel globalAttributeGroupEntityModel)
        {
            HttpResponseMessage response;
            try
            {
                bool status = _service.AssignAttributesToForm(globalAttributeGroupEntityModel);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = status });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message,ErrorCode= ErrorCodes.NotFound });
            }
            return response;
        }

        /// <summary>
        /// Update existing group.
        /// </summary>
        /// <param name="model">Model to update.</param>
        /// <returns>Returns updated model.</returns>
        [ResponseType(typeof(FormBuilderResponse))]
        [HttpPut]
        public virtual HttpResponseMessage Update([FromBody] FormBuilderModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Update formbuilder
                var attributeGroup = _service.Update(model);
                response = attributeGroup ? CreateCreatedResponse(new FormBuilderResponse { FormBuilder = model, ErrorCode = 0 }) : CreateInternalServerErrorResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(model.FormBuilderId)));
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new FormBuilderResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(),TraceLevel.Error);
                var data = new FormBuilderResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }


        /// <summary>
        /// Update Attribute DisplayOrder of form
        /// </summary>
        /// <param name="model">FormBuilderAttributeGroupDisplayOrderModel</param>
        /// <returns>Returns true if associated successfully.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPut]
        public virtual HttpResponseMessage UpdateAttributeDisplayOrder([FromBody]FormBuilderAttributeGroupDisplayOrderModel model)
        {
            HttpResponseMessage response;
            try
            {
                bool status = _service.UpdateAttributeDisplayOrder(model);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = status });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }


        /// <summary>
        /// Update group DisplayOrder of form
        /// </summary>
        /// <param name="model">FormBuilderAttributeGroupDisplayOrderModel</param>
        /// <returns>Returns true if associated successfully.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPut]
        public virtual HttpResponseMessage UpdateGroupDisplayOrder([FromBody]FormBuilderAttributeGroupDisplayOrderModel model)
        {
            HttpResponseMessage response;
            try
            {
                bool status = _service.UpdateGroupDisplayOrder(model);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = status });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Un associate group from form builder.
        /// </summary>
        /// <param name="formBuilderId">FormBuilder Id.</param>
        /// <param name="groupId">Group Id.</param>
        /// <returns>Returns true or false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage UnAssociateFormBuilderGroups(int formBuilderId, int groupId)
        {
            HttpResponseMessage response;
            try
            {
                bool status = _service.UnAssignFormBuilderGroup(formBuilderId, groupId);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = status });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="formBuilderId"></param>
        /// <param name="attributeId"></param>
        /// <returns></returns>
        public virtual HttpResponseMessage UnAssociateFormBuilderAttributes(int formBuilderId, int attributeId)
        {
            HttpResponseMessage response;
            try
            {
                bool status = _service.UnAssignFormBuilderAttribute(formBuilderId, attributeId);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = status });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        #region Form Template 

        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage CreateFormTemplate([FromBody]FormSubmitModel model)
        {
            HttpResponseMessage response;
            try
            {
                response = HelperUtility.IsNotNull(_service.CreateFormTemplate(model)) ? CreateCreatedResponse(new FormSubmitResponse { formSubmitModel = model, ErrorCode = 0 }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message,ErrorCode= ErrorCodes.NotFound });
            }
            return response;
        }

        /// <summary>
        /// Check unique form attribute code already exist or not.
        /// </summary>
        ///<param name="parameters">Parameter model with data.</param>
        /// <returns>Returns true if attribute code exist.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage FormAttributeValueUnique([FromBody] GlobalAttributeValueParameterModel parameters)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new StringResponse { Response = _service.FormAttributeValueUnique(parameters) });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }
        #endregion

        #endregion
    }
}
