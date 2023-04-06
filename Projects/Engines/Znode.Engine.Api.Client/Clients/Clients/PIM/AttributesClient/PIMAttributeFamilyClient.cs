using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Client.Expands;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;

namespace Znode.Engine.Api.Client
{
    public class PIMAttributeFamilyClient : BaseClient, IPIMAttributeFamilyClient
    {
        //Get PIM attribute family list.
        public virtual PIMAttributeFamilyListModel GetAttributeFamilyList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = PIMAttributeFamilyEndpoint.GetAttributeFamilyList();
            endpoint += BuildEndpointQueryString(null, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            PIMAttributeFamilyListResponse response = GetResourceFromEndpoint<PIMAttributeFamilyListResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };

            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            PIMAttributeFamilyListModel list = new PIMAttributeFamilyListModel { PIMAttributeFamilies = response?.PIMAttributeFamilies };
            list.MapPagingDataFromResponse(response);
            return list;
        }

        //Create PIM attribute family.
        public virtual PIMAttributeFamilyModel CreateAttributeFamily(PIMAttributeFamilyModel model)
        {
            string endpoint = PIMAttributeFamilyEndpoint.Create();
            ApiStatus status = new ApiStatus();

            PIMAttributeFamilyResponse response = PostResourceToEndpoint<PIMAttributeFamilyResponse>(endpoint, JsonConvert.SerializeObject(model), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);
            return response?.PIMAttributeFamily;
        }

        //Get assign attribute group list.
        public virtual PIMAttributeGroupListModel GetAssignedAttributeGroups(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = PIMAttributeFamilyEndpoint.GetAssignedAttributeGroups();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            PIMAttributeGroupListResponse response = GetResourceFromEndpoint<PIMAttributeGroupListResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };

            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            PIMAttributeGroupListModel list = new PIMAttributeGroupListModel { AttributeGroupList = response?.AttributeGroups };
            list.MapPagingDataFromResponse(response);
            return list;
        }

        //Get unassign attribute group list.
        public virtual PIMAttributeGroupListModel GetUnAssignedAttributeGroups(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = PIMAttributeFamilyEndpoint.GetUnAssignedAttributeGroups();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            PIMAttributeGroupListResponse response = GetResourceFromEndpoint<PIMAttributeGroupListResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };

            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            PIMAttributeGroupListModel list = new PIMAttributeGroupListModel { AttributeGroupList = response?.AttributeGroups };
            list.MapPagingDataFromResponse(response);
            return list;
        }

        //Associate attribute group to family.
        public virtual bool AssociateAttributeGroups(PIMFamilyGroupAttributeListModel listModel)
        {
            string endpoint = PIMAttributeFamilyEndpoint.AssociateAttributeGroups();

            ApiStatus status = new ApiStatus();

            PIMFamilyGroupAttributeResponse response = PostResourceToEndpoint<PIMFamilyGroupAttributeResponse>(endpoint, JsonConvert.SerializeObject(listModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.NoContent);

            return true;
        }

        //Unassociate attribute group from family.
        public virtual bool UnAssociateAttributeGroups(int attributeFamilyId, int attributeGroupId, bool isCategory)
        {
            string endpoint = PIMAttributeFamilyEndpoint.UnAssociateAttributeGroups(attributeFamilyId, attributeGroupId, isCategory);

            ApiStatus status = new ApiStatus();

            bool response = DeleteResourceFromEndpoint<PIMFamilyGroupAttributeResponse>(endpoint, status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.NoContent);

            return true;
        }

        //Delete PIM attribute family.
        public virtual bool DeleteAttributeFamily(ParameterModel attributeFamilyId)
        {
            string endpoint = PIMAttributeFamilyEndpoint.Delete();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(attributeFamilyId), status);

            //check the status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        //Get PIM attribute family by Id.
        public virtual PIMAttributeFamilyModel GetAttributeFamily(int attributeFamilyId)
        {
            string endpoint = PIMAttributeFamilyEndpoint.GetAttributeFamily(attributeFamilyId);

            ApiStatus status = new ApiStatus();
            PIMAttributeFamilyResponse response = GetResourceFromEndpoint<PIMAttributeFamilyResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.PIMAttributeFamily;
        }

        //Get family locales by family Id.
        public virtual PIMFamilyLocaleListModel GetFamilyLocale(int attributeFamilyId)
        {
            //Get Endpoint.
            string endpoint = PIMAttributeFamilyEndpoint.GetFamilyLocale(attributeFamilyId);

            //Get response.
            ApiStatus status = new ApiStatus();
            PIMFamilyLocaleListResponse response = GetResourceFromEndpoint<PIMFamilyLocaleListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            PIMFamilyLocaleListModel list = new PIMFamilyLocaleListModel { FamilyLocales = response?.FamilyLocales?.FamilyLocales };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Save PIM attribute family locale.
        public virtual PIMFamilyLocaleListModel SaveLocales(PIMFamilyLocaleListModel model)
        {
            //Get Endpoint.
            string endpoint = PIMAttributeFamilyEndpoint.SaveLocales();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            PIMFamilyLocaleListResponse response = PostResourceToEndpoint<PIMFamilyLocaleListResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.FamilyLocales;
        }

        //Get PIM attributes by group Id.
        public virtual PIMAttributeGroupMapperListModel GetAttributesByGroupIds(ParameterModel attributeGroupId)
        {
            //Get Endpoint.
            string endpoint = PIMAttributeFamilyEndpoint.GetAttributesByGroupIds();

            //Get response.
            ApiStatus status = new ApiStatus();
            PIMAttributeGroupListResponse response = PostResourceToEndpoint<PIMAttributeGroupListResponse>(endpoint, JsonConvert.SerializeObject(attributeGroupId), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);
            PIMAttributeGroupMapperListModel list = new PIMAttributeGroupMapperListModel { AttributeGroupMappers = response?.AttributeGroupMappers?.AttributeGroupMappers };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Update Attribute Group Display Order
        public virtual PIMAttributeGroupModel UpdateAttributeGroupDisplayOrder(PIMAttributeGroupModel attributeGroupModel)
        {
            //Get Endpoint.
            string endpoint = PIMAttributeFamilyEndpoint.UpdateAttributeGroupDisplayOrder();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            PIMAttributeGroupResponse response = PutResourceToEndpoint<PIMAttributeGroupResponse>(endpoint, JsonConvert.SerializeObject(attributeGroupModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.AttributeGroup;
        }

        #region Attributes
        //Get assigned attributes list.
        public virtual PIMAttributeListModel GetAssignedAttributes(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = PIMAttributeFamilyEndpoint.GetAssignedAttributes();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            PIMAttributeListResponse response = GetResourceFromEndpoint<PIMAttributeListResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };

            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            PIMAttributeListModel list = new PIMAttributeListModel { Attributes = response?.Attributes };
            list.MapPagingDataFromResponse(response);
            return list;
        }

        //Get unassign attribute list.
        public virtual PIMAttributeListModel GetUnAssignedAttributes(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = PIMAttributeFamilyEndpoint.GetUnAssignedAttributes();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            PIMAttributeListResponse response = GetResourceFromEndpoint<PIMAttributeListResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };

            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            PIMAttributeListModel list = new PIMAttributeListModel { Attributes = response?.Attributes };
            list.MapPagingDataFromResponse(response);
            return list;
        }

        //Associate attribute to group.
        public virtual bool AssignAttributes(AttributeDataModel model)
        {
            string endpoint = PIMAttributeFamilyEndpoint.AssignAttributes();

            ApiStatus status = new ApiStatus();

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.NoContent);

            return true;
        }

        //Unassociate attribute from group.
        public virtual bool UnAssignAttributes(AttributeDataModel model)
        {
            string endpoint = PIMAttributeFamilyEndpoint.UnAssignAttributes();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(model),status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        } 
        #endregion
    }
}
