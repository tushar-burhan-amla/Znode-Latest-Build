using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Services.Maps;
using Znode.Libraries.Observer;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Services
{
    public partial class AccountService
    {
        #region Public Methods

        //Gets store locator list. 
        public virtual AddressListModel GetUserAddressList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel generated to get list of user address: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            AddressListModel list = UserAddressMap.ToListModel(_userAddressRepository.GetEntityList(pageListModel.EntityWhereClause.WhereClause, GetExpands(expands), pageListModel.EntityWhereClause.FilterValues).ToList());
            ZnodeLogging.LogMessage("CustomerName and User address list count: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, new { CustomerName = list?.CustomerName, UserAddressListCount = list?.AddressList?.Count });
            
            if (HelperUtility.IsNotNull(list?.AddressList))
            {
                if (list.AddressList.Any(x => x.UserId != Convert.ToInt32(filters?.FirstOrDefault(o => o.FilterName.ToLower() == ZnodeUserEnum.UserId.ToString().ToLower())?.FilterValue)))
                    throw new ZnodeException(ErrorCodes.InvalidData, Api_Resources.HttpCode_401_AccessDeniedMsg);
            }

            //Set one time address availability flag
            list?.AddressList?.ForEach(o => o.DontAddUpdateAddress = false);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            return list;
        }

        //Create Account Address.
        public virtual AddressModel CreateWebStoreAccountAddress(AddressModel addressModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);

            if (HelperUtility.IsNull(addressModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelNotNull);


            //Set Country and state name.
            SetCountryAndStateName(addressModel);
            
            //If Account Id is greater then 0 i.e,B2B then Set the IsDefaultBilling and IsDefaultShipping flag to false in database for Account. 
            if (addressModel.AccountId > 0)
            {
                IAccountService accountService = GetService<IAccountService>();
                accountService.CreateAccountAddress(addressModel);
                ZnodeLogging.LogMessage("AddressId, AccountId, UserId and AccountAddressId addressModel to be returned: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, new { AddressId = addressModel?.AddressId, AccountId = addressModel?.AccountId, UserId = addressModel?.UserId, AccountAddressId = addressModel?.AccountAddressId });
                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
                return addressModel;
            }
            else
            {

                ICustomerService customerService = GetService<ICustomerService>();
                customerService.CreateCustomerAddress(addressModel);
                ZnodeLogging.LogMessage("AddressId, AccountId, UserId and AccountAddressId addressModel to be returned: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, new { AddressId = addressModel?.AddressId, AccountId = addressModel?.AccountId, UserId = addressModel?.UserId, AccountAddressId = addressModel?.AccountAddressId });
                ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
                return addressModel;
            }
        }

        //Get Address information by Address Id.
        public virtual AddressModel GetAddress(int addressId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameter addressId: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, addressId);

            if (addressId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.ErrorAddressIdLessThanOne);

            //Get address by address id.
            AddressModel addressModel = _addressRepository.GetById(addressId)?.ToModel<AddressModel>();

            //Check if address is account's address in ZnodeAccountAddress mapper. 
            ZnodeAccountAddress accountAddress = _accountAddressRepository.Table.Where(x => x.AddressId == addressId)?.ToList()?.FirstOrDefault();
            if(HelperUtility.IsNotNull(addressModel))
            {
                if (accountAddress?.AccountAddressId > 0)
                {
                    addressModel.AccountId = accountAddress.AccountId;
                    addressModel.AccountAddressId = accountAddress.AccountAddressId;
                    ZnodeLogging.LogMessage("AccountId and AccountAddressId values in addressModel: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, new object[]{ addressModel?.AccountId, addressModel?.AccountAddressId });
                }
                else
                {
                    addressModel.UserId = (_userAddressRepository.Table.Where(x => x.AddressId == addressId)?.ToList()?.FirstOrDefault()?.UserId).GetValueOrDefault();
                    ZnodeLogging.LogMessage("UserId value in addressModel: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, addressModel?.UserId);
                }
            }
            return HelperUtility.IsNotNull(addressModel) ? addressModel : new AddressModel();
        }

        //Update Account Address.
        public virtual bool UpdateWebStoreAccountAddress(AddressModel addressModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);

            if (HelperUtility.IsNull(addressModel))
                throw new ZnodeException(ErrorCodes.NullModel,Admin_Resources.ModelNotNull);

            if (addressModel.AddressId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.ErrorAddressIdLessThanOne);

            if (!CheckValidAddress(addressModel))
                throw new ZnodeException(ErrorCodes.InvalidData, WebStore_Resources.HttpCode_401_AccessDeniedMsg);

            ZnodeLogging.LogMessage("AddressId, AccountId, UserId and AccountAddressId input addressModel: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, new { AddressId = addressModel?.AddressId, AccountId = addressModel?.AccountId, UserId = addressModel?.UserId, AccountAddressId = addressModel?.AccountAddressId });
            // set Modified By from UserId.
            // We have to save modified by for log activity in Impersonation log.
            if(addressModel?.ModifiedBy <= 0 && addressModel?.UserId > 0)
            {
                addressModel.ModifiedBy = addressModel.UserId;
            }
            //If Account Id is greater then 0 i.e,B2B then Sets the IsDefaultBilling and IsDefaultShipping flag to false in database for Account. 
            if (addressModel.AccountId > 0)
            {
                IAccountService accountService = ZnodeDependencyResolver.GetService<IAccountService>();
                return accountService.UpdateAccountAddress(addressModel);
            }
            else
            {
                //Get entity from address repository where AddressId equals with addressModel.AddressId
                ZnodeAddress addressEntity = _addressRepository.Table?.FirstOrDefault(x => x.AddressId == addressModel.AddressId);

                if (HelperUtility.IsNull(addressEntity))
                    throw new ZnodeException(ErrorCodes.NotFound, Admin_Resources.AddressEntityNull);

                //if IsDefaultBilling and IsDefaultShipping is enabled on single record.
                if (addressEntity.IsDefaultBilling && addressEntity.IsDefaultShipping)
                {
                    //Update the record when isShippingBillingSame check box and set as default is checked.
                    if (!addressModel.IsShippingBillingDifferent && addressModel.IsDefaultShipping && addressModel.IsDefaultBilling)
                    {
                        return UpdateAddress(addressModel.ToEntity<ZnodeAddress>(), addressModel);
                    }
                    else
                    {
                        SetBillingShippingFlag(addressModel);
                        CreateWebStoreAccountAddress(addressModel);

                        //return true on success insert
                        return addressModel.AddressId > 0 ;
                    }
                }

                //Update record when IsDefaultBilling and IsDefaultShipping is disabled on single record.
                return UpdateAddress(addressModel.ToEntity<ZnodeAddress>(), addressModel);
                
            }

        }

        //Delete address on the basis of address Id and userId.
        public virtual bool DeleteAddress(int? addressId, int? userId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input parameters addressId and userId to delete address: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, new object[] { addressId, userId });

            if (addressId < 0 && userId < 0)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.IdCanNotBeLessThanOne);

            //Set expand for ZnodeAddress.
            NameValueCollection expands = SetExpands();

            FilterCollection filters = new FilterCollection();
            //Set filters for addressId and userId.
            string whereClause = SetFilters(addressId, userId, filters);
            ZnodeLogging.LogMessage("whereClause to get ZnodeUserAddress: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, whereClause);

            ZnodeUserAddress _znodeUserAddress = _userAddressRepository.GetEntity(whereClause, GetExpands(expands));
            ZnodeLogging.LogMessage("AddressId, UserId and UserAddressId of _znodeUserAddress: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, new { AddressId = _znodeUserAddress?.AddressId, UserId = _znodeUserAddress?.UserId, UserAddressId = _znodeUserAddress?.UserAddressId });
            //Delete UserAddress.
            return DeleteUserAddress(_znodeUserAddress, filters);
        }
        #endregion

        #region Private Methods 
        //Set filters for addressId and userId.
        private string SetFilters(int? addressId, int? userId, FilterCollection filters)
        {
            filters.Add(new FilterTuple(ZnodeUserAddressEnum.AddressId.ToString(), FilterOperators.Equals, addressId.ToString()));
            filters.Add(new FilterTuple(ZnodeUserAddressEnum.UserId.ToString(), FilterOperators.Equals, userId.ToString()));
            return (DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause);
        }

        //Set expand for ZnodeAddress.
        private NameValueCollection SetExpands()
        {
            NameValueCollection expands = new NameValueCollection();
            expands.Add(ZnodeUserAddressEnum.ZnodeAddress.ToString(), ZnodeUserAddressEnum.ZnodeAddress.ToString());
            return expands;
        }

        //Return boolean result for deleted UserAddress.
        private bool DeleteUserAddress(ZnodeUserAddress _znodeUserAddress, FilterCollection filters)
        {
            bool isDeleted = false;
            if (HelperUtility.IsNotNull(_znodeUserAddress))
            {
                if (!_znodeUserAddress.ZnodeAddress.IsDefaultBilling && !_znodeUserAddress.ZnodeAddress.IsDefaultShipping)
                {
                    filters.Add(new FilterTuple(ZnodeUserAddressEnum.UserAddressId.ToString(), FilterOperators.In, Convert.ToString(_znodeUserAddress.UserAddressId)));
                    string whereClause = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause;
                    ZnodeLogging.LogMessage("whereClause to delete user address: ", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Verbose, whereClause);
                    isDeleted = _userAddressRepository.Delete(whereClause);
                    ZnodeLogging.LogMessage(isDeleted ? string.Format(Admin_Resources.SuccessAddressDelete, _znodeUserAddress.UserAddressId) : string.Format(Admin_Resources.ErrorAddressDelete, _znodeUserAddress.UserAddressId), ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
                }
                //Some of the address is IsDefaultBilling or IsDefaultShipping then throw exception.
                else
                    throw new ZnodeException(ErrorCodes.NotPermitted, Admin_Resources.ErrorDeleteDefault);
            }
            return isDeleted;
        }

        //Set country and state name.
        private void SetCountryAndStateName(AddressModel model)
        {
            IStateService state = GetService<IStateService>();
            FilterCollection filter = null;

            if (HelperUtility.IsNotNull(model.StateCode))
            {
                filter = new FilterCollection();
                filter.Add(new FilterTuple(ZnodeStateEnum.StateCode.ToString(), FilterOperators.Is, model.StateCode.ToString()));
                model.StateName = !string.IsNullOrEmpty(model.StateName) ? model.StateName : state.GetStateList(filter, null, null)?.States?.FirstOrDefault()?.StateName;
            }
        }

        //Flags IsDefaultBilling and isDefaultShipping are modified depending on conditions
        private void SetBillingShippingFlag(AddressModel addressModel)
        {
            if (!addressModel.IsAddressBook)
            {
                //Update IsDefaultBilling so that old billing address remains Default billing address when set as default is not checked for Shipping Section.
                if (addressModel.IsDefaultBilling && addressModel.IsShipping)
                     addressModel.IsDefaultBilling = false;

            //Update IsDefaultShipping so that old billing address remains Default shipping address when when set as default is not checked for Billing Section.
            else if (addressModel.IsDefaultShipping && addressModel.IsBilling)
                     addressModel.IsDefaultShipping = false;
            
            //Update IsDefaultShipping and IsDefaultBilling so that new row inserted is neither default shipping nor default billing when no checkbox is checked or only isShippingBillingSame is checked.
            else
            {
                addressModel.IsDefaultBilling = false;
                addressModel.IsDefaultShipping = false;
            }
            }
            //AddressId is set to zero to insert in database
            addressModel.AddressId = 0;
        }

        private bool UpdateAddress(ZnodeAddress _entity, AddressModel addressModel)
        {
            //Sets the IsDefaultBilling and IsDefaultShipping flag to false in database for Customer.
            AddressHelper.SetAddressFlagsToFalse(addressModel);
            bool addressUpdated = _addressRepository.Update(_entity);
            if (addressUpdated)
            {
                if (addressModel.AccountId > 0)
                {
                    _entity.ZnodeAccountAddresses = new List<ZnodeAccountAddress>();
                    _entity.ZnodeAccountAddresses.Add(new ZnodeAccountAddress { AccountId = addressModel.AccountId });
                }
                else
                {
                    _entity.ZnodeUserAddresses = new List<ZnodeUserAddress>();
                    _entity.ZnodeUserAddresses.Add(new ZnodeUserAddress { UserId = addressModel.UserId });
                }
                new ERPInitializer<ZnodeAddress>(_entity, "BillToAddressUpdate");
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Info);
            return addressUpdated;
        }

        //Check Valid Address ID for Current Login User
        private bool CheckValidAddress(AddressModel addressModel)
        {
           var AddressId= (from addressid in _addressRepository.Table
                             where addressid.AddressId == addressModel.AddressId
                             && addressid.CreatedBy == (from createdby in _addressRepository.Table
                                                       where createdby.AddressId == addressModel.AddressId
                                                       select createdby.CreatedBy).FirstOrDefault()
                             select addressid.AddressId).FirstOrDefault(); 

            if(AddressId == addressModel.AddressId)
            {
                return true;
            }
            return false;
        }
        #endregion
    }
}