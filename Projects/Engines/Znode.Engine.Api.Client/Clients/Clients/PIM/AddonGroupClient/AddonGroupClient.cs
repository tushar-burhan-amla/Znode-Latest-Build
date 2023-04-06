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
    public class AddonGroupClient : BaseClient, IAddonGroupClient
    {
        #region Add-on Group

        //Get addonGroup using filter collection.
        public virtual AddonGroupModel GetAddonGroup(FilterCollection filters, ExpandCollection expands)
        {
            string endpoint = AddonGroupEndPoint.GetAddonGroup();
            endpoint += BuildEndpointQueryString(expands, filters, null, null, null);

            ApiStatus status = new ApiStatus();
            AddonGroupResponse response = GetResourceFromEndpoint<AddonGroupResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.AddonGroup;
        }

        //Create addonGroup.
        public virtual AddonGroupModel CreateAddonGroup(AddonGroupModel addonGroupModel)
        {
            string endpoint = AddonGroupEndPoint.CreateAddonGroup();

            ApiStatus status = new ApiStatus();
            AddonGroupResponse response = PostResourceToEndpoint<AddonGroupResponse>(endpoint, JsonConvert.SerializeObject(addonGroupModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.AddonGroup;
        }

        //Update addonGroup.
        public virtual AddonGroupModel UpdateAddonGroup(AddonGroupModel addonGroupModel)
        {
            string endpoint = AddonGroupEndPoint.UpdateAddonGroup();

            ApiStatus status = new ApiStatus();
            AddonGroupResponse response = PutResourceToEndpoint<AddonGroupResponse>(endpoint, JsonConvert.SerializeObject(addonGroupModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response?.AddonGroup;
        }

        //Get addonGroup list using filter collection.
        public virtual AddonGroupListModel GetAddonGroupList(ExpandCollection expands, FilterCollection filters, SortCollection sorts)
            => GetAddonGroupList(expands, filters, sorts, null, null);

        //Get addonGroup list using filter collection.
        public virtual AddonGroupListModel GetAddonGroupList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = AddonGroupEndPoint.GetAddonGroupList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            AddonGroupListResponse response = GetResourceFromEndpoint<AddonGroupListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            AddonGroupListModel list = new AddonGroupListModel { AddonGroups = response?.AddonGroups };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Delete addonGroup by addonGroupIds.
        public virtual bool DeleteAddonGroup(ParameterModel addonGroupIds)
        {
            string endpoint = AddonGroupEndPoint.DeleteAddonGroup();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(addonGroupIds), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        #endregion Add-on Group

        #region Add-on Group Product association

        //Create addonGroup Product association.
        public virtual bool AssociateAddonGroupProduct(AddonGroupProductListModel addonGroupProducts)
        {
            string endpoint = AddonGroupEndPoint.AssociateAddonGroupProduct();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(addonGroupProducts), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        //Get associated add-on group product association.
        public virtual ProductDetailsListModel GetAssociatedAddonGroupProductAssociation(int addonGroupId, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = AddonGroupEndPoint.GetAssociatedAddonGroupProduct(addonGroupId);
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            ProductDetailsListResponse response = GetResourceFromEndpoint<ProductDetailsListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            ProductDetailsListModel list = new ProductDetailsListModel
            {
                ProductDetailList = response?.ProductDetailList,
                Locale = response?.Locale,
                AttributeColumnName = response?.AttributeColumnName,
                XmlDataList = response?.XmlDataList
            };

            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Get unassociated add-on group product association.
        public virtual ProductDetailsListModel GetUnassociatedAddonGroupProductAssociation(int addonGroupId, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = AddonGroupEndPoint.GetUnassociatedAddonGroupProduct(addonGroupId);
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            ProductDetailsListResponse response = GetResourceFromEndpoint<ProductDetailsListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            ProductDetailsListModel list = new ProductDetailsListModel
            {
                ProductDetailList = response?.ProductDetailList,
                Locale = response?.Locale,
                AttributeColumnName = response?.AttributeColumnName,
                XmlDataList = response?.XmlDataList
            };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Delete addonGroup product association by addonGroupProductIds.
        public virtual bool DeleteAddonGroupProductAssociation(ParameterModel addonGroupProductIds)
        {
            string endpoint = AddonGroupEndPoint.DeleteAddonGroupProductAssociation();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(addonGroupProductIds), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        #endregion Add-on Group Product association
    }
}