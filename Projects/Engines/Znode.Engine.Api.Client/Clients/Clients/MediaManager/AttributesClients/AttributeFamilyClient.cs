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
    public class AttributeFamilyClient : BaseClient, IAttributeFamilyClient
    {
        #region Public Methods

        public virtual AttributeFamilyListModel GetAttributeFamilyList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = AttributeFamilyEndpoint.GetAttributeFamilyList();
            endpoint += BuildEndpointQueryString(null, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            AttributeFamilyListResponse response = GetResourceFromEndpoint<AttributeFamilyListResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };

            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            AttributeFamilyListModel list = new AttributeFamilyListModel { AttributeFamilies = response?.AttributeFamilies };
            list.MapPagingDataFromResponse(response);
            return list;
        }

        public virtual AttributeFamilyModel CreateAttributeFamily(AttributeFamilyModel model)
        {
            string endpoint = AttributeFamilyEndpoint.Create();
            ApiStatus status = new ApiStatus();

            AttributeFamilyResponse response = PostResourceToEndpoint<AttributeFamilyResponse>(endpoint, JsonConvert.SerializeObject(model), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);
            return response?.AttributeFamily;
        }

        public virtual AttributeGroupListModel GetAssignedAttributeGroups(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = AttributeFamilyEndpoint.GetAssignedAttributeGroups();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            AttributeGroupListResponse response = GetResourceFromEndpoint<AttributeGroupListResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };

            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            AttributeGroupListModel list = new AttributeGroupListModel { AttributeGroups = response?.AttributeGroups };
            list.MapPagingDataFromResponse(response);
            return list;
        }

        public virtual bool AssociateAttributeGroups(FamilyGroupAttributeListModel listModel)
        {
            string endpoint = AttributeFamilyEndpoint.AssignAttributeGroups();

            ApiStatus status = new ApiStatus();
            FamilyGroupAttributeResponse response = PostResourceToEndpoint<FamilyGroupAttributeResponse>(endpoint, JsonConvert.SerializeObject(listModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.NoContent);

            return true;
        }

        public virtual bool UnAssociateAttributeGroups(int attributeFamilyId, int attributeGroupId)
        {
            string endpoint = AttributeFamilyEndpoint.UnAssignAttributeGroups(attributeFamilyId, attributeGroupId);

            ApiStatus status = new ApiStatus();
            bool response = DeleteResourceFromEndpoint<FamilyGroupAttributeResponse>(endpoint, status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.NoContent);

            return true;
        }

        public virtual bool DeleteAttributeFamily(ParameterModel attributeFamilyId)
        {
            string endpoint = AttributeFamilyEndpoint.Delete();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(attributeFamilyId), status);

            //check the status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        public virtual AttributeFamilyModel GetAttributeFamily(int attributeFamilyId)
        {
            string endpoint = AttributeFamilyEndpoint.GetAttributeFamily(attributeFamilyId);

            ApiStatus status = new ApiStatus();
            AttributeFamilyResponse response = GetResourceFromEndpoint<AttributeFamilyResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.AttributeFamily;
        }

        public virtual AttributeGroupListModel GetUnAssignedAttributeGroups(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = AttributeFamilyEndpoint.GetUnAssignedAttributeGroups();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            AttributeGroupListResponse response = GetResourceFromEndpoint<AttributeGroupListResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };

            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            AttributeGroupListModel list = new AttributeGroupListModel { AttributeGroups = response?.AttributeGroups };
            list.MapPagingDataFromResponse(response);
            return list;
        }

        public virtual AttributeGroupMapperListModel GetAttributesByGroupIds(ParameterModel attributeGroupId)
        {
            //Get Endpoint.
            string endpoint = AttributeFamilyEndpoint.GetAttributesByGroupIds();

            //Get response.
            ApiStatus status = new ApiStatus();
            AttributeGroupListResponse response = PostResourceToEndpoint<AttributeGroupListResponse>(endpoint, JsonConvert.SerializeObject(attributeGroupId), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);
            AttributeGroupMapperListModel list = new AttributeGroupMapperListModel { AttributeGroupMappers = response?.AttributeGroupMappers?.AttributeGroupMappers };
            list.MapPagingDataFromResponse(response);

            return list;
        }
        #region Family Locale
        public virtual FamilyLocaleListModel GetFamilyLocale(int attributeFamilyId)
        {
            //Get Endpoint.
            string endpoint = AttributeFamilyEndpoint.GetFamilyLocale(attributeFamilyId);

            //Get response.
            ApiStatus status = new ApiStatus();
            FamilyLocaleListResponse response = GetResourceFromEndpoint<FamilyLocaleListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            FamilyLocaleListModel list = new FamilyLocaleListModel { FamilyLocales = response?.FamilyLocales?.FamilyLocales };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        public virtual FamilyLocaleListModel SaveLocales(FamilyLocaleListModel model)
        {
            //Get Endpoint.
            string endpoint = AttributeFamilyEndpoint.SaveLocales();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            FamilyLocaleListResponse response = PostResourceToEndpoint<FamilyLocaleListResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.FamilyLocales;
        }
        #endregion

        #endregion
    }
}
