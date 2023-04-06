using System;
using System.Collections.Generic;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Services.Maps;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Services
{
    public static class AddressHelper
    {
        #region "Private variables"

        private static readonly IZnodeRepository<ZnodeAddress> _addressRepository = new ZnodeRepository<ZnodeAddress>();
        private static readonly IZnodeRepository<ZnodeUserAddress> _userAddressRepository = new ZnodeRepository<ZnodeUserAddress>();
        private static readonly IZnodeRepository<ZnodeUser> _userRepository = new ZnodeRepository<ZnodeUser>();

        #endregion

        #region "Public Methods"

        //Sets the IsDefaultBilling and IsDefaultShipping flag to false in database for Account.
        public static void UpdateAddressFlagsToFalse(AddressModel addressModel, FilterCollection filters)
        {
            List<ZnodeAddress> listToUpdate = null;

            //If the models IsDefaultBilling and IsDefaultShipping flag is true.
            if (addressModel.IsDefaultBilling && addressModel.IsDefaultShipping)
            {
                //Get Address list to update.
                listToUpdate = GetAddressListToUpdate(filters);

                //Sort the list on basis of IsDefaultBilling and IsDefaultShipping flag is true. 
                listToUpdate = listToUpdate.FindAll(x => x.IsDefaultBilling || x.IsDefaultShipping);

                //Set the IsDefaultBilling and IsDefaultShipping flag to false. 
                if (!IsGuestUser(addressModel.UserId))
                {
                    listToUpdate?.ForEach(x => { x.IsDefaultBilling = false; x.IsDefaultShipping = false; });
                }
            }
            //If the models IsDefaultBilling flag is true.
            else if (addressModel.IsDefaultBilling)
            {
                filters.Add(new FilterTuple(ZnodeAddressEnum.IsDefaultBilling.ToString(), ProcedureFilterOperators.Equals, ZnodeConstant.TrueValue));
                //Get Address list to update.
                listToUpdate = GetAddressListToUpdate(filters);

                //Set the IsDefaultBilling flag to false.
                if (!IsGuestUser(addressModel.UserId))
                {
                    listToUpdate.ForEach(x => { x.IsDefaultBilling = false; });
                }
            }
            //If the models IsDefaultShipping flag is true.
            else if (addressModel.IsDefaultShipping)
            {
                filters.Add(new FilterTuple(ZnodeAddressEnum.IsDefaultShipping.ToString(), ProcedureFilterOperators.Equals, ZnodeConstant.TrueValue));

                //Get Address list to update.
                listToUpdate = GetAddressListToUpdate(filters);

                //Set the IsDefaultShipping flag to false.
                if (!IsGuestUser(addressModel.UserId))
                {
                    listToUpdate.ForEach(x => { x.IsDefaultShipping = false; });
                }
            }
            UpdateAddressList(listToUpdate);
        }

        //Sets the IsDefaultBilling and IsDefaultShipping flag to false in database for Customer.
        public static void SetAddressFlagsToFalse(AddressModel addressModel)
        {
            //Add filter for user id.
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeUserAddressEnum.UserId.ToString(), ProcedureFilterOperators.Equals, addressModel.UserId.ToString()));

            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());

            //Get customer address list on the basis of user id passed in filter.
            IList<ZnodeUserAddress> assignedAddress = _userAddressRepository.GetEntityList(whereClauseModel.WhereClause);
            string commaSeperatedAddressIds = string.Join(",", assignedAddress?.Select(x => x.AddressId));
            filters = new FilterCollection();
            if (assignedAddress.Count < 1)
            {
                commaSeperatedAddressIds = string.Join(",", addressModel.AddressId);
            }
            if (!string.IsNullOrEmpty(commaSeperatedAddressIds))
            {
                filters.Add(new FilterTuple(ZnodeAccountAddressEnum.AddressId.ToString(), ProcedureFilterOperators.In, commaSeperatedAddressIds));

                //Update IsDefaultBilling and IsDefaultShipping flag to false in database for Customer.
                UpdateAddressFlagsToFalse(addressModel, filters);
            }
            else
            {
                if (!addressModel.IsShippingBillingDifferent && (!addressModel.IsDefaultBilling || !addressModel.IsDefaultShipping) && !addressModel.IsGuest)
                    throw new ZnodeException(ErrorCodes.InvalidData, "Default Billing and Shipping Address must be selected for New Customer.");
                ZnodeAddress address = _addressRepository.Insert(addressModel.ToEntity<ZnodeAddress>());
                if(HelperUtility.IsNotNull(address))
                {
                    ZnodeLogging.LogMessage(address?.AddressId > 0 ? "Address created successfully." : "Address not created.");
                    addressModel.AddressId = address.AddressId;
                }
            }
        }

        //Update address list of customer.
        private static void UpdateAddressList(List<ZnodeAddress> listToUpdate)
        {
            if (listToUpdate?.Count > 0)
                listToUpdate.ForEach(x => _addressRepository.Update(x));
        }

        //Get Address list to update.
        private static List<ZnodeAddress> GetAddressListToUpdate(FilterCollection filters)
        {
            EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());

            //Gets the entity list.
            return _addressRepository.GetEntityList(whereClauseModel.WhereClause, whereClauseModel.FilterValues)?.ToList();
        }

        // Get state code by stateName
        public static string GetStateCode(string stateName, string countryCode)
        {
            stateName = !string.IsNullOrEmpty(stateName) ? stateName.Trim() : null;
            if ((stateName?.Length > 3))
            {
                IZnodeRepository<ZnodeState> _stateRepository = new ZnodeRepository<ZnodeState>();
                //to state code by stateName
                string stateCode = Convert.ToString((from state in _stateRepository.Table
                                                     where state.StateName == stateName
                                                     && state.CountryCode == countryCode
                                                     select state.StateCode).FirstOrDefault());
                return stateCode ?? string.Empty;
            }
            return stateName ?? string.Empty;
        }

        //check if it is guest user or not
        public static bool IsGuestUser(int userId)
        {
            ZnodeUser user = _userRepository.Table.FirstOrDefault(x => x.UserId == userId && x.AspNetUserId == null);
            if (HelperUtility.IsNotNull(user))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #endregion
    }
}
