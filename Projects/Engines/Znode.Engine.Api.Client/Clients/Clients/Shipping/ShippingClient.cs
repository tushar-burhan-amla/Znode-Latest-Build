using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Api.Client.Expands;

namespace Znode.Engine.Api.Client
{
    public class ShippingClient : BaseClient, IShippingClient
    {
        #region Shipping
        //Get shipping by shipping Id
        public virtual ShippingModel GetShipping(int shippingId)
        {
            //Get Endpoint.
            string endpoint = ShippingEndpoint.GetShippingById(shippingId);

            //Get response.
            ApiStatus status = new ApiStatus();
            ShippingResponse response = GetResourceFromEndpoint<ShippingResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.Shipping;
        }

        public virtual ShippingListModel GetShippingList(ExpandCollection expands, FilterCollection filters, SortCollection sorts)
            => GetShippingList(expands, filters, sorts, null, null);

        //Get shipping list 
        public virtual ShippingListModel GetShippingList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = ShippingEndpoint.GetShippingList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response.
            ApiStatus status = new ApiStatus();
            ShippingListResponse response = GetResourceFromEndpoint<ShippingListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            //Shipping list.
            ShippingListModel shippingList = new ShippingListModel { ShippingList = response?.ShippingList };
            shippingList.MapPagingDataFromResponse(response);

            return shippingList;
        }

        //Create shipping 
        public virtual ShippingModel CreateShipping(ShippingModel model)
        {
            //Get Endpoint
            string endpoint = ShippingEndpoint.Create();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            ShippingResponse response = PostResourceToEndpoint<ShippingResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.Shipping;
        }

        //Update shipping 
        public virtual ShippingModel UpdateShipping(ShippingModel model)
        {
            //Get Endpoint
            string endpoint = ShippingEndpoint.Update();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            ShippingResponse response = PostResourceToEndpoint<ShippingResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            //Check and set status
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.Shipping;
        }

        //Delete shipping by shipping id
        public virtual bool DeleteShipping(ParameterModel shippingId)
        {
            //Get Endpoint.
            string endpoint = ShippingEndpoint.Delete();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(shippingId), status);

            //check the status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }
        #endregion

        #region Shipping SKU

        public virtual ShippingSKUListModel GetShippingSKUList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = ShippingEndpoint.ShippingSKUList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            ShippingSKUListResponse response = GetResourceFromEndpoint<ShippingSKUListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            ShippingSKUListModel shippingSKUListModel = new ShippingSKUListModel { ShippingSKUList = response?.ShippingSKUList };
            shippingSKUListModel.MapPagingDataFromResponse(response);

            return shippingSKUListModel;
        }


        public virtual ShippingSKUModel AddShippingSKU(ShippingSKUModel shippingSKUModel)
        {
            string endpoint = ShippingEndpoint.AddShippingSKU();

            ApiStatus status = new ApiStatus();
            ShippingSKUResponse response = PostResourceToEndpoint<ShippingSKUResponse>(endpoint, JsonConvert.SerializeObject(shippingSKUModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);
            return response?.ShippingSKU;
        }


        public virtual bool DeleteShippingSKU(string shippingSKUId)
        {
            string endpoint = ShippingEndpoint.DeleteShippingSKU();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(new ParameterModel() { Ids = shippingSKUId }), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        #endregion

        #region Shipping Service Code

        //Get shipping service code by Id.
        public virtual ShippingServiceCodeModel GetShippingServiceCode(int shippingServiceCodeId)
        {
            //Get Endpoint.
            string endpoint = ShippingEndpoint.GetShippingServiceCodeById(shippingServiceCodeId);

            //Get response.
            ApiStatus status = new ApiStatus();
            ShippingServiceCodeResponse response = GetResourceFromEndpoint<ShippingServiceCodeResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.ShippingServiceCode;
        }

        //Get list of shipping service code.
        public virtual ShippingServiceCodeListModel GetShippingServiceCodeList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = ShippingEndpoint.GetShippingServiceCodeList();
            endpoint += BuildEndpointQueryString(null, filters, sorts, pageIndex, pageSize);

            //Get response.
            ApiStatus status = new ApiStatus();
            ShippingServiceCodeListResponse response = GetResourceFromEndpoint<ShippingServiceCodeListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            //Shipping list.
            ShippingServiceCodeListModel shippingServiceCodeList = new ShippingServiceCodeListModel { ShippingServiceCodes = response?.ShippingServiceCodes };
            shippingServiceCodeList.MapPagingDataFromResponse(response);

            return shippingServiceCodeList;
        }

        #endregion

        #region Shipping Rule

        public virtual ShippingRuleListModel GetShippingRuleList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = ShippingEndpoint.ShippingRuleList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            ShippingRuleListResponse response = GetResourceFromEndpoint<ShippingRuleListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            ShippingRuleListModel shippingRuleListModel = new ShippingRuleListModel { ShippingRuleList = response?.ShippingRuleList };
            shippingRuleListModel.MapPagingDataFromResponse(response);

            return shippingRuleListModel;
        }

        public virtual ShippingRuleModel GetShippingRule(int shippingRuleId)
        {
            string endpoint = ShippingEndpoint.GetShippingRule(shippingRuleId);

            ApiStatus status = new ApiStatus();
            ShippingRuleResponse response = GetResourceFromEndpoint<ShippingRuleResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response?.ShippingRule;
        }

        public virtual ShippingRuleModel AddShippingRule(ShippingRuleModel shippingRuleModel)
        {
            string endpoint = ShippingEndpoint.AddShippingRule();

            ApiStatus status = new ApiStatus();
            ShippingRuleResponse response = PostResourceToEndpoint<ShippingRuleResponse>(endpoint, JsonConvert.SerializeObject(shippingRuleModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);
            return response?.ShippingRule;
        }

        public virtual ShippingRuleModel UpdateShippingRule(ShippingRuleModel shippingRuleModel)
        {
            string endpoint = ShippingEndpoint.UpdateShippingRule();

            ApiStatus status = new ApiStatus();
            ShippingRuleResponse response = PutResourceToEndpoint<ShippingRuleResponse>(endpoint, JsonConvert.SerializeObject(shippingRuleModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);
            return response?.ShippingRule;
        }

        public virtual bool DeleteShippingRule(string shippingRuleId)
        {
            string endpoint = ShippingEndpoint.DeleteShippingRule();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(new ParameterModel() { Ids = shippingRuleId }), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        #endregion

        #region Shipping Rule Type

        //Get list of shipping rule types.
        public virtual ShippingRuleTypeListModel GetShippingRuleTypeList(FilterCollection filters, SortCollection sorts)
        {
            //Get Endpoint.
            string endpoint = ShippingEndpoint.GetShippingRuleTypeList();
            endpoint += BuildEndpointQueryString(null, filters, sorts, 0, 0);

            //Get response.
            ApiStatus status = new ApiStatus();
            ShippingRuleTypeListResponse response = GetResourceFromEndpoint<ShippingRuleTypeListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            //Shipping rule type list.
            ShippingRuleTypeListModel shippingRuleTypeList = new ShippingRuleTypeListModel { ShippingRuleTypeList = response?.ShippingRuleTypeList };
            shippingRuleTypeList.MapPagingDataFromResponse(response);

            return shippingRuleTypeList;
        }

        #endregion

        #region Portal/Profile Shipping
        //Get associated shipping list for profile/portal.
        public virtual ShippingListModel GetAssociatedShippingList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = ShippingEndpoint.GetAssociatedShippingList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();

            //Get response from API
            ShippingListResponse response = GetResourceFromEndpoint<ShippingListResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            ShippingListModel shippingListModel = new ShippingListModel();
            if (HelperUtility.IsNotNull(response))
                shippingListModel.ShippingList = response.ShippingList; shippingListModel.ProfileName = response.ProfileName; shippingListModel.PortalName = response.PortalName;

            shippingListModel.MapPagingDataFromResponse(response);

            return shippingListModel;
        }

        //Get list of unassociated shipping for profile/portal.
        public virtual ShippingListModel GetUnAssociatedShippingList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = ShippingEndpoint.GetUnAssociatedShippingList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();

            //Get response from API
            ShippingListResponse response = GetResourceFromEndpoint<ShippingListResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            ShippingListModel shippingListModel = new ShippingListModel { ShippingList = response?.ShippingList };
            shippingListModel.MapPagingDataFromResponse(response);
            return shippingListModel;
        }

        //Associate shipping to profile/portal.
        public virtual bool AssociateShipping(PortalProfileShippingModel portalProfileShippingModel)
        {
            //Creating endpoint here.
            string endpoint = ShippingEndpoint.AssociateShipping();
            ApiStatus status = new ApiStatus();

            //Get response from API
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(portalProfileShippingModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response.IsSuccess;
        }

        //Remove associated shipping from profile/portal.
        public virtual bool UnAssociateAssociatedShipping(PortalProfileShippingModel portalProfileShippingModel)
        {
            string endpoint = ShippingEndpoint.UnAssociateAssociatedShipping();

            ApiStatus status = new ApiStatus();

            //Get response from API
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(portalProfileShippingModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }
        #endregion

        //Check shipping address is valid or not.
        public virtual BooleanModel IsShippingAddressValid(AddressModel model)
        {
            //Get endpoint.
            string endpoint = ShippingEndpoint.IsAddressValid();

            //Get response.
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return new BooleanModel { IsSuccess = response.IsSuccess, ErrorMessage = response.ErrorMessage };
        }

        // Get recommended address.
        public virtual AddressListModel RecommendedAddress(AddressModel model)
        {
            //Get Endpoint.
            string endpoint = ShippingEndpoint.RecommendedAddress();

            //Get response.
            ApiStatus status = new ApiStatus();
            AddressListResponse response = PostResourceToEndpoint<AddressListResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            return new AddressListModel { AddressList = response.AddressList };
        }

        //Update Shipping of portal.
        public virtual bool UpdateShippingToPortal(PortalProfileShippingModel portalProfileShippingModel)
        {
            //Creating endpoint here.
            string endpoint = ShippingEndpoint.UpdateShippingToPortal();
            ApiStatus status = new ApiStatus();

            //Get response from API
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(portalProfileShippingModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response.IsSuccess;
        }

        //Update profile shipping.
        public virtual bool UpdateProfileShipping(PortalProfileShippingModel model)
        {
            string endpoint = ShippingEndpoint.UpdateProfileShipping();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PutResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(model), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response?.IsSuccess ?? false;
        }
    }
}
