using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public class GlobalAttributeEntityClient : BaseClient, IGlobalAttributeEntityClient
    {
        public virtual GlobalEntityListModel GetGlobalEntity()
        {
            string endpoint = GlobalAttributeEndpoint.GetGlobalEntity();
            endpoint += BuildEndpointQueryString(null, null, null, null, null);

            ApiStatus status = new ApiStatus();
            GlobalEntityListResponse response = GetResourceFromEndpoint<GlobalEntityListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            GlobalEntityListModel list = new GlobalEntityListModel { GlobalEntityList = response?.GlobalEntityList };

            return list;
        }

        public virtual GlobalAttributeGroupListModel GetAssignedEntityAttributeGroups(ExpandCollection expands, FilterCollection filters, SortCollection sortCollection, int? pageIndex, int? pageSize)
        {
            string endpoint = GlobalAttributeEndpoint.GetAssignedEntityAttributeGroups();
            endpoint += BuildEndpointQueryString(expands, filters, null, null, null);

            ApiStatus status = new ApiStatus();
            AttributeEntityGroupListResponse response = GetResourceFromEndpoint<AttributeEntityGroupListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            GlobalAttributeGroupListModel list = new GlobalAttributeGroupListModel { AttributeGroupList = response?.AttributeEntityGroupList };

            return list ?? new GlobalAttributeGroupListModel();
        }


        public virtual GlobalAttributeGroupListModel GetUnAssignedEntityAttributeGroups(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = GlobalAttributeEndpoint.GetUnAssignedEntityAttributeGroups();
            endpoint += BuildEndpointQueryString(null, filters, null, null, null);

            ApiStatus status = new ApiStatus();
            AttributeEntityGroupListResponse response = GetResourceFromEndpoint<AttributeEntityGroupListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            GlobalAttributeGroupListModel list = new GlobalAttributeGroupListModel { AttributeGroupList = response?.AttributeEntityGroupList };

            return list ?? new GlobalAttributeGroupListModel();
        }

        public virtual bool AssociateAttributeEntityToGroups(GlobalAttributeGroupEntityModel globalAttributeGroupEntityModel)
        {
            string endpoint = GlobalAttributeEndpoint.AssociateAttributeEntityToGroups();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(globalAttributeGroupEntityModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        //Un associate group from entity.
        public virtual bool UnAssociateEntityGroups(int entityId, int groupId)
        {
            string endpoint = GlobalAttributeEndpoint.UnAssociateEntityGroups(entityId, groupId);
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(groupId), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        //Update Attribute Group Display Order
        public virtual GlobalAttributeGroupModel UpdateAttributeGroupDisplayOrder(GlobalAttributeGroupModel attributeGroupModel)
        {
            //Get Endpoint.
            string endpoint = GlobalAttributeEndpoint.UpdateAttributeGroupDisplayOrder();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            GlobalAttributeGroupResponse response = PutResourceToEndpoint<GlobalAttributeGroupResponse>(endpoint, JsonConvert.SerializeObject(attributeGroupModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.AttributeGroup;
        }

        //Get Attribute Details based on the Entity.
        public virtual GlobalAttributeEntityDetailsModel GetEntityAttributeDetails(int entityId, string entityType)
        {
            string endpoint = GlobalAttributeEndpoint.GetEntityAttributeDetails(entityId, entityType);

            ApiStatus status = new ApiStatus();
            GlobalAttributeEntityResponse response = GetResourceFromEndpoint<GlobalAttributeEntityResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.EntityDetails;
        }

        public virtual EntityAttributeModel SaveEntityAttributeDetails(EntityAttributeModel model)
        {
            string endpoint = GlobalAttributeEndpoint.SaveEntityAttributeDetails();

            ApiStatus status = new ApiStatus();
            EntityAttributeResponse response = PostResourceToEndpoint<EntityAttributeResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.EntityAttribute;
        }

        //Get publish global attributes.
        public virtual GlobalSelectedAttributeEntityDetailsModel GetGlobalEntityAttributes(int entityId, string entityType, FilterCollection filters)
        {
            //Get Endpoint.
            string endpoint = GlobalAttributeEndpoint.GetGlobalEntityAttributes(entityId, entityType);
            endpoint += BuildEndpointQueryString(null, filters, null, null, null);

            ApiStatus status = new ApiStatus();
            GlobalSelectedAttributeEntityResponse response = GetResourceFromEndpoint<GlobalSelectedAttributeEntityResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.EntityDetails;
        }

        //gets the global attributes based on the passed familyCode for setting the values for default container variant. 
        public virtual GlobalAttributeEntityDetailsModel GetGlobalAttributesForDefaultVariantData(string familyCode, string entityType)
        {
            string endpoint = GlobalAttributeEndpoint.GetGlobalAttributesForDefaultVariantData(familyCode, entityType);

            ApiStatus status = new ApiStatus();
            GlobalAttributeEntityResponse response = GetResourceFromEndpoint<GlobalAttributeEntityResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.EntityDetails;
        }

        // Get Global Attribute details on the basis of variant id and localeid
        public virtual GlobalAttributeEntityDetailsModel GetGlobalAttributesForAssociatedVariant(int variantId, string entityType, int localeId = 0)
        {
            string endpoint = GlobalAttributeEndpoint.GetGlobalAttributesForAssociatedVariant(variantId, entityType, localeId);

            ApiStatus status = new ApiStatus();
            GlobalAttributeEntityResponse response = GetResourceFromEndpoint<GlobalAttributeEntityResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.EntityDetails;
        }
    }
}
