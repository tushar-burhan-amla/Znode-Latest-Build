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
    public class GlobalAttributeGroupClient : BaseClient, IGlobalAttributeGroupClient
    {
        //Get paged global Attribute Group List.
        public virtual GlobalAttributeGroupListModel GetAttributeGroupList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = GlobalAttributeGroupEndpoint.GetAttributeGroups();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            GlobalAttributeGroupListResponse response = GetResourceFromEndpoint<GlobalAttributeGroupListResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };

            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            GlobalAttributeGroupListModel list = new GlobalAttributeGroupListModel { AttributeGroupList = response?.AttributeGroups };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Create global attribute group
        public virtual GlobalAttributeGroupModel CreateAttributeGroupModel(GlobalAttributeGroupModel attributeGroupModel)
        {
            //Get Endpoint.
            string endpoint = GlobalAttributeGroupEndpoint.CreateAttributeGroup();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            GlobalAttributeGroupResponse response = PostResourceToEndpoint<GlobalAttributeGroupResponse>(endpoint, JsonConvert.SerializeObject(attributeGroupModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.AttributeGroup;
        }

        //Get attribute group by id.
        public virtual GlobalAttributeGroupModel GetAttributeGroup(int id, ExpandCollection expands)
        {
            //Get Endpoint.
            string endpoint = GlobalAttributeGroupEndpoint.GetAttributeGroup(id);
            endpoint += BuildEndpointQueryString(expands);

            ApiStatus status = new ApiStatus();
            GlobalAttributeGroupResponse response = GetResourceFromEndpoint<GlobalAttributeGroupResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.AttributeGroup;
        }

        //Updates Global Attribute Group.
        public virtual GlobalAttributeGroupModel UpdateAttributeGroupModel(GlobalAttributeGroupModel attributeGroupModel)
        {
            string endpoint = GlobalAttributeGroupEndpoint.UpdateAttributeGroup();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            GlobalAttributeGroupResponse response = PutResourceToEndpoint<GlobalAttributeGroupResponse>(endpoint, JsonConvert.SerializeObject(attributeGroupModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.AttributeGroup;
        }

        //Get global attribute group locale.
        public virtual GlobalAttributeGroupLocaleListModel GetGlobalAttributeGroupLocales(int globalAttributeGroupLocaleId)
        {
            string endpoint = GlobalAttributeGroupEndpoint.GetGlobalAttributeGroupLocales(globalAttributeGroupLocaleId);

            ApiStatus status = new ApiStatus();
            GlobalAttributeGroupListResponse response = GetResourceFromEndpoint<GlobalAttributeGroupListResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };

            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            GlobalAttributeGroupLocaleListModel list = new GlobalAttributeGroupLocaleListModel { AttributeGroupLocales = response?.AttributeGroupLocales?.AttributeGroupLocales };
            list.MapPagingDataFromResponse(response);
            return list;
        }

        //Delete Global Attribute Group
        public virtual bool DeleteAttributeGroup(ParameterModel globalAttributeGroupIds)
        {
            //Get Endpoint.
            string endpoint = GlobalAttributeGroupEndpoint.DeleteAttributeGroup();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(globalAttributeGroupIds), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        //Get the list of associated attributes.
        public virtual GlobalAttributeGroupMapperListModel GetAssignedAttributes(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = GlobalAttributeGroupEndpoint.GetAssignedAttributes();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            GlobalAttributeGroupListResponse response = GetResourceFromEndpoint<GlobalAttributeGroupListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            GlobalAttributeGroupMapperListModel list = new GlobalAttributeGroupMapperListModel { AttributeGroupMappers = response?.AttributeGroupMappers.AttributeGroupMappers };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Get the list of unassociated attributes.
        public virtual GlobalAttributeListModel GetUnAssignedAttributes(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = GlobalAttributeGroupEndpoint.GetUnAssignedAttributes();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            GlobalAttributeListResponse response = GetResourceFromEndpoint<GlobalAttributeListResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };

            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            GlobalAttributeListModel list = new GlobalAttributeListModel { Attributes = response?.Attributes };
            list.MapPagingDataFromResponse(response);
            return list;
        }

        //Associate global attributes with group.
        public virtual GlobalAttributeGroupMapperListModel AssociateAttributes(GlobalAttributeGroupMapperListModel model)
        {
            //Get Endpoint.
            string endpoint = GlobalAttributeGroupEndpoint.AssociateAttributes();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            GlobalAttributeGroupListResponse response = PostResourceToEndpoint<GlobalAttributeGroupListResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.AttributeGroupMappers;
        }

        //Remove associated attribute.
        public virtual bool RemoveAssociatedAttributes(RemoveGroupAttributesModel model)
        {
            //Get Endpoint.
            string endpoint = GlobalAttributeGroupEndpoint.RemoveAssociatedAttributes();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        //Update Attribute Display Order
        public virtual GlobalAttributeDataModel UpdateAttributeDisplayOrder(GlobalAttributeDataModel globalAttributeDataModel)
        {
            string endpoint = GlobalAttributeGroupEndpoint.UpdateAttributeDisplayOrder();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            GlobalAttributeDataResponse response = PutResourceToEndpoint<GlobalAttributeDataResponse>(endpoint, JsonConvert.SerializeObject(globalAttributeDataModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.GlobalAttributeDataModel;
        }
    }
}
