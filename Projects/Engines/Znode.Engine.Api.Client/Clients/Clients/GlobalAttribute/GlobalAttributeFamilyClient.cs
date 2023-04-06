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
    public class GlobalAttributeFamilyClient : BaseClient , IGlobalAttributeFamilyClient
    {

        // Gets the List of Attribute Family
        public virtual GlobalAttributeFamilyListModel GetAttributeFamilyList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = GlobalAttributeFamilyEndpoint.List();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            GlobalAttributeFamilyListResponse response = GetResourceFromEndpoint<GlobalAttributeFamilyListResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };

            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            GlobalAttributeFamilyListModel list = new GlobalAttributeFamilyListModel { AttributeFamilyList = response?.AttributeFamily };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Create a new Attribute Family
        public virtual GlobalAttributeFamilyModel CreateAttributeFamily(GlobalAttributeFamilyCreateModel attributeFamilyModel)
        {
           
            //Get Endpoint.
            string endpoint = GlobalAttributeFamilyEndpoint.CreateAttributeFamily();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            GlobalAttributeFamilyResponse response = PostResourceToEndpoint<GlobalAttributeFamilyResponse>(endpoint, JsonConvert.SerializeObject(attributeFamilyModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.AttributeFamily;
        }


        //Update Global Attribute Family.
        public virtual GlobalAttributeFamilyModel UpdateAttributeFamily(GlobalAttributeFamilyUpdateModel attributeFamilyModel)
        {
            string endpoint = GlobalAttributeFamilyEndpoint.UpdateAttributeFamily();

            ApiStatus status = new ApiStatus();
            GlobalAttributeFamilyResponse response = PutResourceToEndpoint<GlobalAttributeFamilyResponse>(endpoint, JsonConvert.SerializeObject(attributeFamilyModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.AttributeFamily;
        }

        //Delete Global Attribute Family
        public virtual bool DeleteAttributeFamily(ParameterModel globalAttributeFamilyIds)
        {
            //Get Endpoint.
            string endpoint = GlobalAttributeFamilyEndpoint.DeleteAttributeFamily();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(globalAttributeFamilyIds), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        //Get the Attribute Family
        public virtual GlobalAttributeFamilyModel GetAttributeFamily(string familyCode)
        {
            //Get Endpoint.
            string endpoint = GlobalAttributeFamilyEndpoint.GetAttributeFamily(familyCode);

            ApiStatus status = new ApiStatus();
            GlobalAttributeFamilyResponse response = GetResourceFromEndpoint<GlobalAttributeFamilyResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.AttributeFamily;
        }

        //Returns the List of Groups that are associated to a Family
        public virtual GlobalAttributeGroupListModel GetAssignedAttributeGroups(string familyCode)
        {
            string endpoint = GlobalAttributeFamilyEndpoint.GetAssignedAttributeGroups(familyCode);
            

            ApiStatus status = new ApiStatus();
            GlobalAttributeGroupListResponse response = GetResourceFromEndpoint<GlobalAttributeGroupListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            GlobalAttributeGroupListModel list = new GlobalAttributeGroupListModel { AttributeGroupList = response?.AttributeGroups };

            return list ?? new GlobalAttributeGroupListModel();
        }

        //Returns the List of Unassigned groups of a Family
        public virtual GlobalAttributeGroupListModel GetUnassignedAttributeGroups(string familyCode)
        {
            string endpoint = GlobalAttributeFamilyEndpoint.GetUnassignedAttributeGroups(familyCode);

            ApiStatus status = new ApiStatus();
            GlobalAttributeGroupListResponse response = GetResourceFromEndpoint<GlobalAttributeGroupListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            GlobalAttributeGroupListModel list = new GlobalAttributeGroupListModel { AttributeGroupList = response?.AttributeGroups };

            return list ?? new GlobalAttributeGroupListModel();
        }

        //Assign Attribute Groups to a Family
        public virtual bool AssignAttributeGroups(string attributeGroupIds, string familyCode)
        {
            string endpoint = GlobalAttributeFamilyEndpoint.AssignAttributeGroups(attributeGroupIds, familyCode);
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(familyCode), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        //Unassign attribute Groups from a family
        public virtual bool UnassignAttributeGroups(string groupCode, string familyCode)
        {
            string endpoint = GlobalAttributeFamilyEndpoint.UnassignAttributeGroups(groupCode, familyCode);
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(groupCode), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        //Update Attribute Group Display Order
        public virtual bool UpdateAttributeGroupDisplayOrder(string groupCode, string familyCode, int displayOrder)
        {
            //Get Endpoint.
            string endpoint = GlobalAttributeFamilyEndpoint.UpdateAttributeGroupDisplayOrder(groupCode, familyCode, displayOrder);

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(groupCode), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        //Get the Attribute Family Locale
        public virtual GlobalAttributeFamilyLocaleListModel GetGlobalAttributeFamilyLocales(string familyCode)
        {
            string endpoint = GlobalAttributeFamilyEndpoint.GetGlobalAttributeFamilyLocales(familyCode);

            ApiStatus status = new ApiStatus();
            GlobalAttributeFamilyListResponse response = GetResourceFromEndpoint<GlobalAttributeFamilyListResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };

            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            GlobalAttributeFamilyLocaleListModel list = new GlobalAttributeFamilyLocaleListModel { AttributeFamilyLocales = response?.AttributeFamilyLocales?.AttributeFamilyLocales };
            list.MapPagingDataFromResponse(response);
            return list;
        }

        public virtual bool IsFamilyCodeExist(string familyCode)
        {
            //Get Endpoint.
            string endpoint = GlobalAttributeFamilyEndpoint.IsFamilyCodeExist(familyCode);

            //Get response.
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = GetResourceFromEndpoint<TrueFalseResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response.IsSuccess;
        }
    }



}
