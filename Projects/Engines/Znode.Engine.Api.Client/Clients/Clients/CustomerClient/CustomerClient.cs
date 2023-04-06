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
    public class CustomerClient : BaseClient, ICustomerClient
    {
        #region Profile Association.
        // Get unassociated profile list.
        public virtual ProfileListModel GetUnAssociatedProfileList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = CustomerEndpoint.GetUnAssociatedProfileList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            ProfileListResponse response = GetResourceFromEndpoint<ProfileListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            ProfileListModel list = new ProfileListModel { Profiles = response?.Profiles };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Get associated profile list based on customer.
        public virtual ProfileListModel GetAssociatedProfilelist(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = CustomerEndpoint.GetAssociatedProfilelist();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            ProfileListResponse response = GetResourceFromEndpoint<ProfileListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            ProfileListModel list = new ProfileListModel();
            if (HelperUtility.IsNotNull(response))
            {
                list.Profiles = response.Profiles;
                list.CustomerName = response.CustomerName;
                list.AccountId = response.AccountId;
            }
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //UnAssociate associated profiles by profileId.
        public virtual bool UnAssociateProfiles(ParameterModel profileIds, int userId)
        {
            string endpoint = CustomerEndpoint.UnAssociateProfiles(userId);
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(profileIds), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response?.IsSuccess ?? false;
        }

        //Associate unassociated profiles by profile id.
        public virtual bool AssociateProfiles(ParameterModelUserProfile model)
        {
            string endpoint = CustomerEndpoint.AssociateProfiles();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response?.IsSuccess ?? false;
        }

        //Set default profile for user.
        public virtual bool SetDefaultProfile(ParameterModelUserProfile model)
        {
            string endpoint = CustomerEndpoint.SetDefaultProfile();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response?.IsSuccess ?? false;
        }

        //Get customer profile list based on portal.
        public virtual ProfileListModel GetCustomerPortalProfilelist(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = CustomerEndpoint.GetCustomerPortalProfilelist();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            ProfileListResponse response = GetResourceFromEndpoint<ProfileListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            ProfileListModel list = new ProfileListModel();
            if (HelperUtility.IsNotNull(response))
            {
                list.Profiles = response.Profiles;
                list.CustomerName = response.CustomerName;
                list.AccountId = response.AccountId;
            }
            list.MapPagingDataFromResponse(response);

            return list;
        }

        #endregion

        #region Affiliate
        // Get list of referral commission type.
        public virtual ReferralCommissionTypeListModel GetReferralCommissionTypeList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = CustomerEndpoint.GetReferralCommissionTypeList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            ReferralCommissionListResponse response = GetResourceFromEndpoint<ReferralCommissionListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            ReferralCommissionTypeListModel list = new ReferralCommissionTypeListModel { ReferralCommissionTypes = response?.ReferralCommissionTypes };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        // Get customer affiliate data.
        public virtual ReferralCommissionModel GetCustomerAffiliate(int userId, ExpandCollection expands)
        {
            string endpoint = CustomerEndpoint.GetCustomerAffiliate(userId);
            endpoint += BuildEndpointQueryString(expands, null, null, null, null);

            ApiStatus status = new ApiStatus();
            ReferralCommissionResponse response = GetResourceFromEndpoint<ReferralCommissionResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.ReferralCommission;
        }

        //Update customer affiliate.
        public virtual ReferralCommissionModel UpdateCustomerAffiliate(ReferralCommissionModel model)
        {
            string endpoint = CustomerEndpoint.UpdateCustomerAffiliate();

            ApiStatus status = new ApiStatus();
            ReferralCommissionResponse response = PutResourceToEndpoint<ReferralCommissionResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.ReferralCommission;
        }

        //Get Referral Commission list based on customer.
        public virtual ReferralCommissionListModel GetReferralCommissionlist(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = CustomerEndpoint.GetReferralCommissionlist();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            ReferralCommissionListResponse response = GetResourceFromEndpoint<ReferralCommissionListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            ReferralCommissionListModel list = new ReferralCommissionListModel { ReferralCommissions = response?.ReferralCommissions };
            list.MapPagingDataFromResponse(response);

            return list;
        }
        #endregion

        #region Address
        //Get the address list.
        public virtual AddressListModel GetAddressList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get endpoint having api url.
            string endpoint = CustomerEndpoint.GetAddressList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            AddressListResponse response = GetResourceFromEndpoint<AddressListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };

            //Check status of response.
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            AddressListModel list = new AddressListModel { AddressList = response?.AddressList };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Get customer address. 
        public virtual AddressModel GetCustomerAddress(ExpandCollection expands, FilterCollection filters)
        {
            //Get endpoint having api url.
            string endpoint = CustomerEndpoint.GetCustomerAddress();
            endpoint += BuildEndpointQueryString(expands, filters, null, null, null);

            ApiStatus status = new ApiStatus();
            AddressResponse response = GetResourceFromEndpoint<AddressResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };

            //Check status of response.
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.Address;
        }

        //Create customer address.
        public virtual AddressModel CreateCustomerAddress(AddressModel addressModel)
        {
            //Get endpoint having api url.
            string endpoint = CustomerEndpoint.CreateCustomerAddress();

            ApiStatus status = new ApiStatus();
            AddressResponse response = PostResourceToEndpoint<AddressResponse>(endpoint, JsonConvert.SerializeObject(addressModel), status);

            //Check status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.Address;
        }

        //Update customer address.
        public virtual AddressModel UpdateCustomerAddress(AddressModel addressModel)
        {
            //Get endpoint having api url.
            string endpoint = CustomerEndpoint.UpdateCustomerAddress();

            ApiStatus status = new ApiStatus();
            AddressResponse response = PutResourceToEndpoint<AddressResponse>(endpoint, JsonConvert.SerializeObject(addressModel), status);

            //Check status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.Address;
        }

        //Delete customer address.
        public virtual bool DeleteCustomerAddress(ParameterModel userAddressId)
        {
            //Get endpoint having api url.
            string endpoint = CustomerEndpoint.DeleteCustomerAddress();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(userAddressId), status);

            //Check status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        //Get search locations.
        public AddressListModel GetSearchLocation(int portalId, string searchTerm)
        {
            string endpoint = CustomerEndpoint.GetSearchLocation(portalId, searchTerm);

            ApiStatus status = new ApiStatus();
            AddressListResponse response = GetResourceFromEndpoint<AddressListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };

            //Check status of response.
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            AddressListModel list = new AddressListModel { AddressList = response?.AddressList };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Update search address.
        public AddressModel UpdateSearchAddress(AddressModel addressModel)
        {
            string endpoint = CustomerEndpoint.UpdateSearchAddress();

            ApiStatus status = new ApiStatus();
            WebStoreAccountResponse response = PutResourceToEndpoint<WebStoreAccountResponse>(endpoint, JsonConvert.SerializeObject(addressModel), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.AccountAddress;
        }

        #endregion

        #region Associate Price
        //Associate Price to Customer.
        public virtual bool AssociatePriceList(PriceUserModel priceUserModel)
        {
            string endpoint = CustomerEndpoint.AssociatePriceList();
            ApiStatus status = new ApiStatus();

            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(priceUserModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        //UnAssociate associated price list from Customer.
        public virtual bool UnAssociatePriceList(PriceUserModel priceUserModel)
        {
            string endpoint = CustomerEndpoint.UnAssociatePriceList();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(priceUserModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        //Get associated price list precedence value for Customer.
        public virtual PriceUserModel GetAssociatedPriceListPrecedence(PriceUserModel priceUserModel)
        {
            string endpoint = CustomerEndpoint.GetAssociatedPriceListPrecedence();

            ApiStatus status = new ApiStatus();
            PriceUserResponse response = PostResourceToEndpoint<PriceUserResponse>(endpoint, JsonConvert.SerializeObject(priceUserModel), status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response?.PriceUser;
        }

        //Update associated price list precedence value Customer.
        public virtual PriceUserModel UpdateAssociatedPriceListPrecedence(PriceUserModel priceUserModel)
        {
            string endpoint = CustomerEndpoint.UpdateAssociatedPriceListPrecedence();

            ApiStatus status = new ApiStatus();
            PriceUserResponse response = PutResourceToEndpoint<PriceUserResponse>(endpoint, JsonConvert.SerializeObject(priceUserModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);
            return response?.PriceUser;
        }
        #endregion
    }
}
