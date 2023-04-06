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
    public class PIMAttributeGroupClient : BaseClient, IPIMAttributeGroupClient
    {
        //Get AttributeGroup list.
        public virtual PIMAttributeGroupListModel GetAttributeGroupList(ExpandCollection expands, FilterCollection filters, SortCollection sorts) => GetAttributeGroupList(expands, filters, sorts, null, null);

        //Gets paged PIM Attribute Group List.
        public virtual PIMAttributeGroupListModel GetAttributeGroupList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = PIMAttributeGroupEndpoint.GetAttributeGroups();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            PIMAttributeGroupListResponse response = GetResourceFromEndpoint<PIMAttributeGroupListResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };

            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            PIMAttributeGroupListModel list = new PIMAttributeGroupListModel { AttributeGroupList = response?.AttributeGroups };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Gets Attribute Group by ID.
        public virtual PIMAttributeGroupModel GetAttributeGroup(int id, ExpandCollection expands)
        {
            //Get Endpoint.
            string endpoint = PIMAttributeGroupEndpoint.GetAttributeGroup(id);
            endpoint += BuildEndpointQueryString(expands);

            ApiStatus status = new ApiStatus();
            PIMAttributeGroupResponse response = GetResourceFromEndpoint<PIMAttributeGroupResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.AttributeGroup;
        }

        //Create Attribute Group
        public virtual PIMAttributeGroupModel CreateAttributeGroupModel(PIMAttributeGroupModel attributeGroupModel)
        {
            //Get Endpoint.
            string endpoint = PIMAttributeGroupEndpoint.CreateAttributeGroup();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            PIMAttributeGroupResponse response = PostResourceToEndpoint<PIMAttributeGroupResponse>(endpoint, JsonConvert.SerializeObject(attributeGroupModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.AttributeGroup;
        }

        //Update PIM Attribute Group
        public virtual PIMAttributeGroupModel UpdateAttributeGroupModel(PIMAttributeGroupModel attributeGroupModel)
        {
            string endpoint = PIMAttributeGroupEndpoint.UpdateAttributeGroup();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            PIMAttributeGroupResponse response = PutResourceToEndpoint<PIMAttributeGroupResponse>(endpoint, JsonConvert.SerializeObject(attributeGroupModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.AttributeGroup;
        }

        //Update Attribute Display Order
        public virtual PIMAttributeDataModel UpdateAttributeDisplayOrder(PIMAttributeDataModel pimAttributeDataModel)
        {
            string endpoint = PIMAttributeGroupEndpoint.UpdateAttributeDisplayOrder();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            PIMAttributeDataResponse response = PutResourceToEndpoint<PIMAttributeDataResponse>(endpoint, JsonConvert.SerializeObject(pimAttributeDataModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.PIMAttributeDataModel;
        }

        //Delete PIMAttributeGroup
        public virtual bool DeleteAttributeGroupModel(ParameterModel pimAttributeGroupIds)
        {
            //Get Endpoint.
            string endpoint = PIMAttributeGroupEndpoint.DeleteAttributeGroup();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(pimAttributeGroupIds), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        //Gets paged list of associated attributes to an attribute group.
        public virtual PIMAttributeGroupMapperListModel GetAssignedAttributes(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = PIMAttributeGroupEndpoint.GetAssignedAttributes();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            PIMAttributeGroupListResponse response = GetResourceFromEndpoint<PIMAttributeGroupListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            PIMAttributeGroupMapperListModel list = new PIMAttributeGroupMapperListModel { AttributeGroupMappers = response?.AttributeGroupMappers.AttributeGroupMappers };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Gets paged list of unassociated attributes to an attribute group.
        public virtual PIMAttributeListModel GetUnAssignedAttributes(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = PIMAttributeGroupEndpoint.GetUnAssignedAttributes();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            PIMAttributeListResponse response = GetResourceFromEndpoint<PIMAttributeListResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };

            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            PIMAttributeListModel list = new PIMAttributeListModel { Attributes = response?.Attributes };
            list.MapPagingDataFromResponse(response);
            return list;
        }

        //Gets paged list of unassociated attributes to an attribute group.
        public virtual PIMAttributeGroupLocaleListModel GetPIMAttributeGroupLocales(int pimAttributeGroupLocale)
        {
            string endpoint = PIMAttributeGroupEndpoint.GetPIMAttributeGroupLocales(pimAttributeGroupLocale);

            ApiStatus status = new ApiStatus();
            PIMAttributeGroupListResponse response = GetResourceFromEndpoint<PIMAttributeGroupListResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };

            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            PIMAttributeGroupLocaleListModel list = new PIMAttributeGroupLocaleListModel { AttributeGroupLocales = response?.AttributeGroupLocales?.AttributeGroupLocales };
            list.MapPagingDataFromResponse(response);
            return list;
        }

        //Save PIM attribute group locales.
        public virtual PIMAttributeGroupLocaleListModel SaveAttributeGroupLocales(PIMAttributeGroupLocaleListModel model)
        {
            //Get Endpoint.
            string endpoint = PIMAttributeGroupEndpoint.SaveAttributeGroupLocales();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            PIMAttributeGroupListResponse response = PostResourceToEndpoint<PIMAttributeGroupListResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.AttributeGroupLocales;
        }

        //Associate PIM attributes with group.
        public virtual PIMAttributeGroupMapperListModel AssociateAttributes(PIMAttributeGroupMapperListModel model)
        {
            //Get Endpoint.
            string endpoint = PIMAttributeGroupEndpoint.AssociateAttributes();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            PIMAttributeGroupListResponse response = PostResourceToEndpoint<PIMAttributeGroupListResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.AttributeGroupMappers;
        }

        //Remove associated attribute.
        public virtual bool RemoveAssociatedAttributes(RemoveAssociatedAttributesModel model)
        {
            //Get Endpoint.
            string endpoint = PIMAttributeGroupEndpoint.RemoveAssociatedAttributes();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }
    }
}
