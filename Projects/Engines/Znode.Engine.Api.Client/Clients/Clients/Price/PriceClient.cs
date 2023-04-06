using Newtonsoft.Json;
using System.Collections.Generic;
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
    public class PriceClient : BaseClient, IPriceClient
    {
        //Create Price.
        public virtual PriceModel CreatePrice(PriceModel priceModel)
        {
            //Get Endpoint.
            string endpoint = PriceEndpoint.CreatePrice();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            PriceResponse response = PostResourceToEndpoint<PriceResponse>(endpoint, JsonConvert.SerializeObject(priceModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.Price;
        }

        //Get Price.
        public virtual PriceModel GetPrice(int priceListId)
        {
            string endpoint = PriceEndpoint.GetPrice(priceListId);

            ApiStatus status = new ApiStatus();
            PriceResponse response = GetResourceFromEndpoint<PriceResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.Price;
        }

        //Update Price.
        public virtual PriceModel UpdatePrice(PriceModel priceModel)
        {
            string endpoint = PriceEndpoint.UpdatePrice();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            PriceResponse response = PutResourceToEndpoint<PriceResponse>(endpoint, JsonConvert.SerializeObject(priceModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.Price;
        }

        // Gets the list of Price.
        public virtual PriceListModel GetPriceList(ExpandCollection expands, FilterCollection filters, SortCollection sorts) => GetPriceList(expands, filters, sorts, null, null);

        // Gets the list of Price.
        public virtual PriceListModel GetPriceList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = PriceEndpoint.GetPriceList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            PriceListResponse response = GetResourceFromEndpoint<PriceListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            PriceListModel list = new PriceListModel();
            if (HelperUtility.IsNotNull(response))
            {
                list.PriceList = response.PriceList;
                list.HasParentAccounts = response.HasParentAccounts;
                list.CustomerName = response.CustomerName;
            }
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Delete Price
        public virtual bool DeletePrice(ParameterModel priceListId)
        {
            string endpoint = PriceEndpoint.Delete();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(priceListId), status);

            //check the status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        // Copy a Price.
        public virtual bool CopyPrice(PriceModel priceModel)
        {
            string endpoint = PriceEndpoint.Copy();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(priceModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        #region SKU Price
        public virtual PriceSKUListModel GetSKUPriceList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = PriceEndpoint.SKUPriceList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            PriceSKUListResponse response = GetResourceFromEndpoint<PriceSKUListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            PriceSKUListModel priceSKUListModel = new PriceSKUListModel { PriceSKUList = response?.PriceSKUList };
            priceSKUListModel.MapPagingDataFromResponse(response);

            return priceSKUListModel;
        }

        public virtual PriceSKUModel AddSKUPrice(PriceSKUModel priceSKUModel)
        {
            string endpoint = PriceEndpoint.AddSKUPrice();

            ApiStatus status = new ApiStatus();
            PriceSKUResponse response = PostResourceToEndpoint<PriceSKUResponse>(endpoint, JsonConvert.SerializeObject(priceSKUModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);
            return response?.PriceSKU;
        }

        public virtual PriceSKUModel GetSKUPrice(int priceId)
        {
            string endpoint = PriceEndpoint.GetSKUPrice(priceId);

            ApiStatus status = new ApiStatus();
            PriceSKUResponse response = GetResourceFromEndpoint<PriceSKUResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response?.PriceSKU;
        }

        public virtual PriceSKUModel UpdateSKUPrice(PriceSKUModel priceSKUModel)
        {
            string endpoint = PriceEndpoint.UpdateSKUPrice();

            ApiStatus status = new ApiStatus();
            PriceSKUResponse response = PutResourceToEndpoint<PriceSKUResponse>(endpoint, JsonConvert.SerializeObject(priceSKUModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);
            return response?.PriceSKU;
        }

        public virtual bool DeleteSKUPrice(SKUPriceDeleteModel model)
        {
            string endpoint = PriceEndpoint.DeleteSKUPrice();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(model), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        public virtual UomListModel GetUomList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = PriceEndpoint.UomList();

            ApiStatus status = new ApiStatus();
            UomListResponse response = GetResourceFromEndpoint<UomListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            UomListModel uomListModel = new UomListModel { UomList = response?.UomList };
            return uomListModel;
        }

        //Get price by sku.
        public virtual PriceSKUModel GetPriceBySku(FilterCollection filters)
        {
            string endpoint = PriceEndpoint.GetPriceBySku();
            endpoint += BuildEndpointQueryString(null, filters, null, null, null);

            ApiStatus status = new ApiStatus();
            PriceSKUResponse response = GetResourceFromEndpoint<PriceSKUResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response?.PriceSKU;
        }

        //Get price details by sku.
        public virtual PriceSKUModel GetProductPricingDetailsBySku(ProductPriceListSKUDataModel productPriceListSKUDataModel)
        {
            string endpoint = PriceEndpoint.GetProductPricingDetailsBySku();

            ApiStatus status = new ApiStatus();
            PriceSKUResponse response = PostResourceToEndpoint<PriceSKUResponse>(endpoint, JsonConvert.SerializeObject(productPriceListSKUDataModel), status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response?.PriceSKU;
        }

        public virtual PriceSKUListModel GetPagedSkuPrice(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = PriceEndpoint.GetPagedSkuPrice();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            PriceSKUListResponse response = GetResourceFromEndpoint<PriceSKUListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            PriceSKUListModel priceSKUListModel = new PriceSKUListModel { PriceSKUList = response?.PriceSKUList };
            priceSKUListModel.MapPagingDataFromResponse(response);

            return priceSKUListModel;
        }
        #endregion

        #region Tier Price
        public virtual PriceTierListModel GetTierPriceList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = PriceEndpoint.TierPriceList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            TierPriceListResponse response = GetResourceFromEndpoint<TierPriceListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            PriceTierListModel tierPriceListModel = new PriceTierListModel { TierPriceList = response?.TierPriceList };
            tierPriceListModel.MapPagingDataFromResponse(response);

            return tierPriceListModel;
        }

        //Save tier price values.(add and update both)
        public virtual bool AddTierPrice(PriceTierListModel priceTierListModel)
        {
            string endpoint = PriceEndpoint.AddTierPrice();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(priceTierListModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response.IsSuccess;
        }

        public virtual PriceTierModel GetTierPrice(int priceTierId)
        {
            string endpoint = PriceEndpoint.GetTierPrice(priceTierId);

            ApiStatus status = new ApiStatus();
            TierPriceResponse response = GetResourceFromEndpoint<TierPriceResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response?.TierPrice;
        }

        public virtual PriceTierModel UpdateTierPrice(PriceTierModel priceTierModel)
        {
            string endpoint = PriceEndpoint.UpdateTierPrice();

            ApiStatus status = new ApiStatus();
            TierPriceResponse response = PutResourceToEndpoint<TierPriceResponse>(endpoint, JsonConvert.SerializeObject(priceTierModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);
            return response?.TierPrice;
        }

        public virtual bool DeleteTierPrice(int priceTierId)
        {
            string endpoint = PriceEndpoint.DeleteTierPrice(priceTierId);

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(priceTierId), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        #endregion

        #region Associate Store
        public virtual PricePortalListModel GetAssociatedStoreList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = PriceEndpoint.PricePortalList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            PricePortalListResponse response = GetResourceFromEndpoint<PricePortalListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            PricePortalListModel pricePortalListModel = new PricePortalListModel { PricePortalList = response?.PricePortalList };
            pricePortalListModel.MapPagingDataFromResponse(response);

            return pricePortalListModel;
        }

        public virtual PortalListModel GetUnAssociatedStoreList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = PriceEndpoint.GetUnAssociatedStoreList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            PortalListResponse response = GetResourceFromEndpoint<PortalListResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };

            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            PortalListModel list = new PortalListModel { PortalList = response?.PortalList };
            list.MapPagingDataFromResponse(response);
            return list;
        }

        public virtual bool AssociateStore(PricePortalListModel pricePortalModelListModel)
        {
            //creating endpoint here.
            string endpoint = PriceEndpoint.AssociateStore();
            ApiStatus status = new ApiStatus();

            //get response from API
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(pricePortalModelListModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response.IsSuccess;
        }

        public virtual bool RemoveAssociatedStores(RemoveAssociatedStoresModel model)
        {
            string endpoint = PriceEndpoint.RemoveAssociatedStores();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        //Get associated store precedence data.
        public virtual PricePortalModel GetAssociatedStorePrecedence(int priceListPortalId, ExpandCollection expands)
        {
            //Get Endpoint.
            string endpoint = PriceEndpoint.GetAssociatedStorePrecedence(priceListPortalId);
            endpoint += BuildEndpointQueryString(expands, null, null, null, null);

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            PricePortalResponse response = GetResourceFromEndpoint<PricePortalResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response?.PricePortal;
        }

        //Update associated store precedence data.
        public virtual PricePortalModel UpdateAssociatedStorePrecedence(PricePortalModel pricePortalModel)
        {
            //Get Endpoint.
            string endpoint = PriceEndpoint.UpdateAssociatedStorePrecedence();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            PricePortalResponse response = PutResourceToEndpoint<PricePortalResponse>(endpoint, JsonConvert.SerializeObject(pricePortalModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);
            return response?.PricePortal;
        }

        #endregion

        #region Associate Profile
        public virtual PriceProfileListModel GetAssociatedProfileList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = PriceEndpoint.PriceProfileList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            PriceProfileListResponse response = GetResourceFromEndpoint<PriceProfileListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            PriceProfileListModel priceProfileListModel = new PriceProfileListModel { PriceProfileList = response?.PriceProfileList };
            priceProfileListModel.MapPagingDataFromResponse(response);

            return priceProfileListModel;
        }

        public virtual ProfileListModel GetUnAssociatedProfileList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = PriceEndpoint.GetUnAssociatedProfileList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            ProfileListResponse response = GetResourceFromEndpoint<ProfileListResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };

            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            ProfileListModel list = new ProfileListModel { Profiles = response?.Profiles };
            list.MapPagingDataFromResponse(response);
            return list;
        }

        public virtual bool AssociateProfile(PriceProfileListModel priceProfileModelListModel)
        {
            string endpoint = PriceEndpoint.AssociateProfile();
            ApiStatus status = new ApiStatus();

            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(priceProfileModelListModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response.IsSuccess;
        }

        public virtual bool RemoveAssociatedProfiles(RemoveAssociatedProfilesModel model)
        {
            string endpoint = PriceEndpoint.RemoveAssociatedProfiles();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        //Get associated profiles precedence data.
        public virtual PriceProfileModel GetAssociatedProfilePrecedence(int priceListProfileId, ExpandCollection expands)
        {
            //Get Endpoint.
            string endpoint = PriceEndpoint.GetAssociatedProfilePrecedence(priceListProfileId);
            endpoint += BuildEndpointQueryString(expands, null, null, null, null);

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            PriceProfileResponse response = GetResourceFromEndpoint<PriceProfileResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response?.PriceProfile;
        }

        //Update associated profile precedence data.
        public virtual PriceProfileModel UpdateAssociatedProfilePrecedence(PriceProfileModel priceProfileModel)
        {
            //Get Endpoint.
            string endpoint = PriceEndpoint.UpdateAssociatedProfilePrecedence();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            PriceProfileResponse response = PutResourceToEndpoint<PriceProfileResponse>(endpoint, JsonConvert.SerializeObject(priceProfileModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);
            return response?.PriceProfile;
        }

        #endregion

        #region Associate Customer
        //Get Associated Customer List.
        public virtual PriceUserListModel GetAssociatedCustomerList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = PriceEndpoint.PriceCustomerList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            PriceUserListResponse response = GetResourceFromEndpoint<PriceUserListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            PriceUserListModel priceAccountListModel = new PriceUserListModel { PriceUserList = response?.PriceUserList };
            priceAccountListModel.MapPagingDataFromResponse(response);

            return priceAccountListModel;
        }

        //Get UnAssociated Customer List.
        public virtual PriceUserListModel GetUnAssociatedCustomerList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = PriceEndpoint.GetUnAssociatedCustomerList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            PriceUserListResponse response = GetResourceFromEndpoint<PriceUserListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            PriceUserListModel list = new PriceUserListModel { PriceUserList = response?.PriceUserList };
            list.MapPagingDataFromResponse(response);
            return list;
        }

        //Associate Customer.
        public virtual bool AssociateCustomer(PriceUserListModel priceAccountModelListModel)
        {
            string endpoint = PriceEndpoint.AssociateCustomer();
            ApiStatus status = new ApiStatus();

            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(priceAccountModelListModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response.IsSuccess;
        }

        //Delete Associated Customer.
        public virtual bool DeleteAssociatedCustomer(ParameterModel customerIds)
        {
            string endpoint = PriceEndpoint.DeleteAssociatedCustomer();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(customerIds), status);

            //check the status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        //Get associated customer precedence data.
        public virtual PriceUserModel GetAssociatedCustomerPrecedence(int priceListUserId, ExpandCollection expands)
        {
            //Get Endpoint.
            string endpoint = PriceEndpoint.GetAssociatedCustomerPrecedence(priceListUserId);
            endpoint += BuildEndpointQueryString(expands, null, null, null, null);

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            PriceUserResponse response = GetResourceFromEndpoint<PriceUserResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response?.PriceUser;
        }

        //Update associated store precedence data.
        public virtual PriceUserModel UpdateAssociatedCustomerPrecedence(PriceUserModel priceUserModel)
        {
            //Get Endpoint.
            string endpoint = PriceEndpoint.UpdateAssociatedCustomerPrecedence();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            PriceUserResponse response = PutResourceToEndpoint<PriceUserResponse>(endpoint, JsonConvert.SerializeObject(priceUserModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);
            return response?.PriceUser;
        }
        #endregion

        #region Associate Account

        //Get accounts associated to price list. 
        public virtual PriceAccountListModel GetAssociatedAccountList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = PriceEndpoint.PriceAccountList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            PriceAccountListResponse response = GetResourceFromEndpoint<PriceAccountListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            PriceAccountListModel priceAccountListModel = new PriceAccountListModel { PriceAccountList = response?.PriceAccountList };
            priceAccountListModel.MapPagingDataFromResponse(response);

            return priceAccountListModel;
        }

        //Get unassociated accounts to price list. 
        public virtual PriceAccountListModel GetUnAssociatedAccountList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = PriceEndpoint.GetUnAssociatedAccountList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            PriceAccountListResponse response = GetResourceFromEndpoint<PriceAccountListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            PriceAccountListModel list = new PriceAccountListModel { PriceAccountList = response?.PriceAccountList };
            list.MapPagingDataFromResponse(response);
            return list;
        }

        //Associate accounts to price list.
        public virtual bool AssociateAccount(PriceAccountListModel priceAccountModelListModel)
        {
            string endpoint = PriceEndpoint.AssociateAccount();
            ApiStatus status = new ApiStatus();

            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(priceAccountModelListModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response.IsSuccess;
        }

        //Remove associated accounts.
        public virtual bool RemoveAssociatedAccounts(RemoveAssociatedAccountsModel model)
        {
            string endpoint = PriceEndpoint.RemoveAssociatedAccounts();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        //Get associated accounts precedence data.
        public virtual PriceAccountModel GetAssociatedAccountPrecedence(int priceListUserId, ExpandCollection expands)
        {
            //Get Endpoint.
            string endpoint = PriceEndpoint.GetAssociatedAccountPrecedence(priceListUserId);
            endpoint += BuildEndpointQueryString(expands, null, null, null, null);

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            PriceAccountResponse response = GetResourceFromEndpoint<PriceAccountResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response?.PriceAccount;
        }

        //Update associated store precedence data.
        public virtual PriceAccountModel UpdateAssociatedAccountPrecedence(PriceAccountModel priceAccountModel)
        {
            //Get Endpoint.
            string endpoint = PriceEndpoint.UpdateAssociatedAccountPrecedence();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            PriceAccountResponse response = PutResourceToEndpoint<PriceAccountResponse>(endpoint, JsonConvert.SerializeObject(priceAccountModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);
            return response?.PriceAccount;
        }
        #endregion

        #region Price Management
        //Get unassociated price list.
        public virtual PriceListModel GetUnAssociatedPriceList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = PriceEndpoint.GetUnAssociatedPriceList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            PriceListResponse response = GetResourceFromEndpoint<PriceListResponse>(endpoint, status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };

            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            PriceListModel list = new PriceListModel { PriceList = response?.PriceList };
            list.MapPagingDataFromResponse(response);
            return list;
        }

        public virtual bool RemoveAssociatedPriceListToStore(ParameterModel model)
        {
            string endpoint = PriceEndpoint.RemoveAssociatedPriceListToStore();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        public virtual bool RemoveAssociatedPriceListToProfile(ParameterModel priceListProfileIds)
        {
            string endpoint = PriceEndpoint.RemoveAssociatedPriceListToProfile();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(priceListProfileIds), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        //Get associated price list precedence data for Store/Profile.
        public virtual PricePortalModel GetAssociatedPriceListPrecedence(PricePortalModel pricePortalModel)
        {
            string endpoint = PriceEndpoint.GetAssociatedPriceListPrecedence();

            ApiStatus status = new ApiStatus();
            PricePortalResponse response = PostResourceToEndpoint<PricePortalResponse>(endpoint, JsonConvert.SerializeObject(pricePortalModel), status);
            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);
            return response?.PricePortal;
        }

        //Update associated price list precedence data for Store/Profile.
        public virtual PricePortalModel UpdateAssociatedPriceListPrecedence(PricePortalModel pricePortalModel)
        {
            string endpoint = PriceEndpoint.UpdateAssociatedPriceListPrecedence();

            ApiStatus status = new ApiStatus();
            PricePortalResponse response = PutResourceToEndpoint<PricePortalResponse>(endpoint, JsonConvert.SerializeObject(pricePortalModel), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);
            return response?.PricePortal;
        }
        #endregion

        public virtual List<ExportPriceModel> GetExportPriceData(string priceListIds)
        {
            string endPoint = PriceEndpoint.GetExportPriceData(priceListIds);

            ApiStatus status = new ApiStatus();
            ExportPriceListResponse response = GetResourceFromEndpoint<ExportPriceListResponse>(endPoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            List<ExportPriceModel> list = response?.ExportPriceList;

            return list;
        }
    }
}
