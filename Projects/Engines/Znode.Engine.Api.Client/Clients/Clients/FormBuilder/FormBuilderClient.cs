using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public class FormBuilderClient : BaseClient, IFormBuilderClient
    {
        // Creates a form.
        public virtual FormBuilderModel CreateForm(FormBuilderModel formBuilderModel)
        {
            string endpoint = FormBuilderEndpoint.CreateForm();

            ApiStatus status = new ApiStatus();
            FormBuilderResponse response = PostResourceToEndpoint<FormBuilderResponse>(endpoint, JsonConvert.SerializeObject(formBuilderModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.FormBuilder;
        }

        // Get form details on the basis of form builder id.
        public virtual FormBuilderModel GetForm(int formBuilderId, ExpandCollection expands)
        {
            string endpoint = FormBuilderEndpoint.GetForm(formBuilderId);
            endpoint += BuildEndpointQueryString(expands);

            ApiStatus status = new ApiStatus();
            FormBuilderResponse response = GetResourceFromEndpoint<FormBuilderResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.FormBuilder;
        }

        //Delete form builder.
        public virtual bool DeleteFormBuilder(ParameterModel formBuilderId)
        {
            string endpoint = FormBuilderEndpoint.DeleteFormBuilder();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(formBuilderId), status);

            //check the status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        ///Gets the list of form builder
        public virtual FormBuilderListModel GetFormBuilderList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Create Endpoint to get the list of form builder.
            string endpoint = FormBuilderEndpoint.GetFormBuilderList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            FormBuilderListResponse response = GetResourceFromEndpoint<FormBuilderListResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            FormBuilderListModel list = new FormBuilderListModel { FormBuilderList = response?.FormBuilderList };
            list.MapPagingDataFromResponse(response);
            return list;
        }

        //Check form code already exist or not.
        public virtual bool IsFormCodeExist(string formCode)
        {
            //Get Endpoint.
            string endpoint = FormBuilderEndpoint.IsFormCodeExist(formCode);

            //Get response.
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = GetResourceFromEndpoint<TrueFalseResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response.IsSuccess;
        }

        //Get the list of unassigned attributes.
        public virtual GlobalAttributeListModel GetUnAssignedAttributes(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = FormBuilderEndpoint.GetUnAssignedAttributes();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            GlobalAttributeListResponse response = GetResourceFromEndpoint<GlobalAttributeListResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };

            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            GlobalAttributeListModel list = new GlobalAttributeListModel { Attributes = response?.Attributes };
            list.MapPagingDataFromResponse(response);
            return list;
        }

        //Get unassigned attribute groups.
        public virtual GlobalAttributeGroupListModel GetUnAssignedGroups(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = FormBuilderEndpoint.GetUnAssignedGroups();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            AttributeEntityGroupListResponse response = GetResourceFromEndpoint<AttributeEntityGroupListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            GlobalAttributeGroupListModel list = new GlobalAttributeGroupListModel { AttributeGroupList = response?.AttributeEntityGroupList };
            list.MapPagingDataFromResponse(response);
            return list ?? new GlobalAttributeGroupListModel();
        }

        //Get Attribute Details based on the Entity.
        public virtual FormBuilderAttributeGroupModel GetFormAttributeGroup(int formBuilderId, int localeId, int mappingId = 0)
        {
            string endpoint = FormBuilderEndpoint.GetFormAttributeGroup(formBuilderId, localeId, mappingId);

            ApiStatus status = new ApiStatus();
            FormBuilderAttributeGroupResponse response = GetResourceFromEndpoint<FormBuilderAttributeGroupResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.FormBuilderAttributeGroup;
        }

        //Associate groups.
        public virtual bool AssociateGroups(GlobalAttributeGroupEntityModel globalAttributeGroupEntityModel)
        {
            string endpoint = FormBuilderEndpoint.AssociateGroups();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(globalAttributeGroupEntityModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        //Associate attributes.
        public virtual bool AssociateAttributes(GlobalAttributeGroupEntityModel globalAttributeGroupEntityModel)
        {
            string endpoint = FormBuilderEndpoint.AssociateAttributes();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(globalAttributeGroupEntityModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        //Method to update fromBuilder.
        public virtual FormBuilderModel UpdateFormBuilder(FormBuilderModel formBuilderModel)
        {
            string endpoint = FormBuilderEndpoint.UpdateFormBuilder();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            FormBuilderResponse response = PutResourceToEndpoint<FormBuilderResponse>(endpoint, JsonConvert.SerializeObject(formBuilderModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.FormBuilder;
        }

        // method to update attribute display order
        public virtual bool UpdateAttributeDisplayOrder(FormBuilderAttributeGroupDisplayOrderModel model)
        {
            string endpoint = FormBuilderEndpoint.UpdateAttributeDisplayOrder();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PutResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        // method to update group display order
        public virtual bool UpdateGroupDisplayOrder(FormBuilderAttributeGroupDisplayOrderModel model)
        {
            string endpoint = FormBuilderEndpoint.UpdateGroupDisplayOrder();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PutResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        // method to unassociate group
        public virtual bool UnAssociateFormBuilderGroups(int formBuilderId, int groupId)
        {
            string endpoint = FormBuilderEndpoint.UnAssociateFormBuilderGroups(formBuilderId, groupId);
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(groupId), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        //method to unassociate attribute
        public virtual bool UnAssociateFormBuilderAttributes(int formBuilderId, int attributeId)
        {
            string endpoint = FormBuilderEndpoint.UnAssociateFormBuilderAttributes(formBuilderId, attributeId);
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(attributeId), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        //Method to Create form template.
        public virtual FormSubmitModel CreateFormTemplate(FormSubmitModel model)
        {
            string endpoint = FormBuilderEndpoint.CreateFormTemplate();

            ApiStatus status = new ApiStatus();
            FormSubmitResponse response = PostResourceToEndpoint<FormSubmitResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.formSubmitModel;
        }

        //Check is attribute value is unique or not.
        public virtual ParameterModel FormAttributeValueUnique(GlobalAttributeValueParameterModel attributeCodeValues)
        {
            //Get Endpoint.
            string endpoint = FormBuilderEndpoint.FormAttributeValueUnique();

            //Get response.
            ApiStatus status = new ApiStatus();
            StringResponse response = PostResourceToEndpoint<StringResponse>(endpoint, JsonConvert.SerializeObject(attributeCodeValues), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return new ParameterModel { Ids = response?.Response };
        }
    }
}
