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
    public class AttributeGroupClient : BaseClient, IAttributeGroupClient
    {
        #region Public Methods

        //Get AttributeGroup list.
        public virtual AttributeGroupListModel GetAttributeGroupList(ExpandCollection expands, FilterCollection filters, SortCollection sorts) => GetAttributeGroupList(expands, filters, sorts, null, null);

        public virtual AttributeGroupListModel GetAttributeGroupList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = AttributeGroupEndpoint.GetAttributeGroupList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            AttributeGroupListResponse response = GetResourceFromEndpoint<AttributeGroupListResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };

            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            AttributeGroupListModel list = new AttributeGroupListModel { AttributeGroups = response?.AttributeGroups };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Create AttributeGroup
        public virtual AttributeGroupModel CreateAttributeGroup(AttributeGroupModel model)
        {
            //Get Endpoint.
            string endpoint = AttributeGroupEndpoint.Create();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            AttributeGroupResponse response = PostResourceToEndpoint<AttributeGroupResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.AttributeGroup;
        }

        //Get AttributeGroup
        public virtual AttributeGroupModel GetAttributeGroup(int attributeGroupId)
        {
            string endpoint = AttributeGroupEndpoint.Get(attributeGroupId);

            ApiStatus status = new ApiStatus();
            AttributeGroupResponse response = GetResourceFromEndpoint<AttributeGroupResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.AttributeGroup;
        }

        //Update attribute group locale
        public virtual AttributeGroupModel UpdateAttributeGroup(AttributeGroupModel model)
        {
            //Get Endpoint
            string endpoint = AttributeGroupEndpoint.Update();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            AttributeGroupResponse response = PutResourceToEndpoint<AttributeGroupResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.AttributeGroup;
        }

        //Delete attribute group
        public virtual bool DeleteAttributeGroup(ParameterModel attributeGroupIds)
        {
            //Get Endpoint.
            string endpoint = AttributeGroupEndpoint.Delete();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(attributeGroupIds), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        //Assign attributes to a group.
        public virtual bool AssignAttributes(AttributeGroupMapperListModel attributeGroupMapperList)
        {
            //Get Endpoint.
            string endpoint = AttributeGroupEndpoint.AssignAttributes();

            ApiStatus status = new ApiStatus();
            AttributeGroupListResponse response = PostResourceToEndpoint<AttributeGroupListResponse>(endpoint, JsonConvert.SerializeObject(attributeGroupMapperList), status);

            //check status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.AttributeGroupMappers?.AttributeGroupMappers?.Count > 0;
        }

        //Gets list of associated attributes to an attribute group.
        public virtual AttributeGroupMapperListModel GetAssociatedAttributes(ExpandCollection expands, FilterCollection filters, SortCollection sorts) => GetAssociatedAttributes(expands, filters, sorts, null, null);

        //Gets paged list of associated attributes to an attribute group.
        public virtual AttributeGroupMapperListModel GetAssociatedAttributes(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = AttributeGroupEndpoint.GetAssociatedAttributes();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            AttributeGroupMapperListResponse response = GetResourceFromEndpoint<AttributeGroupMapperListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            AttributeGroupMapperListModel list = new AttributeGroupMapperListModel { AttributeGroupMappers = response?.AttributeGroupMappers };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Deletes an associated attribute to an attribute group.
        public virtual bool DeleteAssociatedAttribute(int attributeGroupMapperId)
        {
            //Get Endpoint.
            string endpoint = AttributeGroupEndpoint.DeleteAssociatedAttribute(attributeGroupMapperId);

            ApiStatus status = new ApiStatus();
            bool deleted = DeleteResourceFromEndpoint<AttributeGroupMapperResponse>(endpoint, status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return deleted;
        }

        //Update Attribute group mapper.
        public virtual AttributeGroupMapperModel UpdateAttributeGroupMapper(AttributeGroupMapperModel model)
        {
            //Get Endpoint
            string endpoint = AttributeGroupEndpoint.UpdateAttributeGroupMapper();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            AttributeGroupMapperResponse response = PutResourceToEndpoint<AttributeGroupMapperResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.AttributeGroupMapper;
        }

        //Gets list of attribute group locales.
        public virtual AttributeGroupLocaleListModel GetAttributeGroupLocales(int attributeGroupLocaleId)
        {
            string endpoint = AttributeGroupEndpoint.GetAttributeGroupLocales(attributeGroupLocaleId);

            ApiStatus status = new ApiStatus();
            AttributeGroupListResponse response = GetResourceFromEndpoint<AttributeGroupListResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };

            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            AttributeGroupLocaleListModel list = new AttributeGroupLocaleListModel { AttributeGroupLocales = response?.AttributeGroupLocales?.AttributeGroupLocales };
            list.MapPagingDataFromResponse(response);
            return list;
        }

        //Gets paged list of unassociated attributes to an attribute group.
        public virtual AttributesListDataModel GetUnAssignedAttributes(int attributegroupId, ExpandCollection expands, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = AttributeGroupEndpoint.GetUnAssignedAttributes(attributegroupId);
            endpoint += BuildEndpointQueryString(expands, null, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            AttributeListResponse response = GetResourceFromEndpoint<AttributeListResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };

            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            AttributesListDataModel list = new AttributesListDataModel { Attributes = response?.Attributes };
            list.MapPagingDataFromResponse(response);
            return list;
        }

        #endregion
    }
}
