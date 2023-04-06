using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Services.Maps;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Observer;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Services
{
    public class CustomerService : BaseService, ICustomerService
    {
        #region Private Variable
        private readonly IZnodeRepository<ZnodeUserProfile> _userProfile;
        private readonly IZnodeRepository<ZnodeProfile> _profile;
        private readonly IZnodeRepository<ZnodeReferralCommissionType> _referralCommissionType;
        private readonly IZnodeRepository<ZnodeUser> _user;
        private readonly IZnodeRepository<ZnodeUserAddress> _userAddress;
        private readonly IZnodeRepository<ZnodeAddress> _addressRepository;
        private readonly IZnodeRepository<ZnodePriceListUser> _priceListUserRepository;
        private readonly IZnodeRepository<ZnodeUserPortal> _userPortalRepository;
        private readonly IZnodeRepository<ZnodeOmsUsersReferralUrl> _usersReferralUrlRepository;
        private readonly IZnodeRepository<ZnodeDomain> _domainRepository;
        private readonly IZnodeRepository<View_CustomerReferralCommissionDetail> _viewCustomerReferralCommissionDetail;
        private readonly IZnodeRepository<ZnodePortalProfile> _portalProfile;
        #endregion

        #region Constructor
        public CustomerService()
        {
            _addressRepository = new ZnodeRepository<ZnodeAddress>();
            _userProfile = new ZnodeRepository<ZnodeUserProfile>();
            _referralCommissionType = new ZnodeRepository<ZnodeReferralCommissionType>();
            _user = new ZnodeRepository<ZnodeUser>();
            _profile = new ZnodeRepository<ZnodeProfile>();
            _userAddress = new ZnodeRepository<ZnodeUserAddress>();
            _priceListUserRepository = new ZnodeRepository<ZnodePriceListUser>();
            _userPortalRepository = new ZnodeRepository<ZnodeUserPortal>();
            _viewCustomerReferralCommissionDetail = new ZnodeRepository<View_CustomerReferralCommissionDetail>();
            _usersReferralUrlRepository = new ZnodeRepository<ZnodeOmsUsersReferralUrl>();
            _domainRepository = new ZnodeRepository<ZnodeDomain>();
            _portalProfile = new ZnodeRepository<ZnodePortalProfile>();
        }
        #endregion

        #region Public Methods
        #region Profile Association
        //Get associated profile list to customer.
        public virtual ProfileListModel GetAssociatedProfileList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to generate associatedProfiles list ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            int userId;
            Int32.TryParse(filters.FirstOrDefault(x => x.FilterName.Equals(ZnodeUserEnum.UserId.ToString(), StringComparison.InvariantCultureIgnoreCase))?.FilterValue, out userId);

            //Filters to get profile name list.
            FilterCollection newFilter = new FilterCollection();
            newFilter.AddRange(filters);

            filters.RemoveAll(x => string.Equals(x.FilterName, FilterKeys.ProfileName, StringComparison.CurrentCultureIgnoreCase) || string.Equals(x.FilterName, $"{ FilterKeys.ProfileName}{'|'}", StringComparison.CurrentCultureIgnoreCase));

            //Method to get associated profiles.
            List<ZnodeUserProfile> associatedProfiles = _userProfile.GetEntityList(pageListModel.EntityWhereClause.WhereClause, pageListModel.EntityWhereClause.FilterValues)?.ToList();
            ProfileListModel listModel = new ProfileListModel();
            if (associatedProfiles?.Count > 0)
            {
                newFilter.Add(new FilterTuple(ZnodeProfileEnum.ProfileId.ToString(), FilterOperators.In, string.Join(",", associatedProfiles.Select(x => x.ProfileId != null ? x.ProfileId : 0))));

                //Get profile list
                List<ZnodeProfile> list = _profile.GetPagedList(RemovefilterToGetProfileList(sorts, page, ref newFilter)?.EntityWhereClause.WhereClause, pageListModel.OrderBy, RemovefilterToGetProfileList(sorts, page, ref newFilter)?.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount)?.ToList();
                ZnodeLogging.LogMessage("List counts:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { associatedProfilesCount=associatedProfiles?.Count(),ZnodeProfileCount=list?.Count() });
                listModel = new ProfileListModel { Profiles = list.ToModel<ProfileModel>()?.ToList() };

                //set userId and IsDefault flag for each profile.
                foreach (var item in listModel.Profiles)
                {
                    item.UserId = userId;
                    item.IsDefault = (associatedProfiles?.Where(x => x.ProfileId == item.ProfileId)?.FirstOrDefault()?.IsDefault).GetValueOrDefault();
                }
            }

            //Get customer name and account id of user.
            GetCustomerNameAndAccountIdOfUser(userId, listModel);

            //Set for pagination
            listModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return listModel;
        }

        //Get list of unassociate profiles.
        public virtual ProfileListModel GetUnAssociatedProfileList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to generate profileIds list ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            //Filters to get profile name list.
            FilterCollection newFilter = new FilterCollection();
            newFilter.AddRange(filters);

            filters.RemoveAll(x => string.Equals(x.FilterName, FilterKeys.ProfileName, StringComparison.CurrentCultureIgnoreCase) || string.Equals(x.FilterName, $"{ FilterKeys.ProfileName}{'|'}", StringComparison.CurrentCultureIgnoreCase));

            //Get associated profile.
            List<int?> profileIds = _userProfile.GetEntityList(pageListModel.EntityWhereClause.WhereClause, pageListModel.EntityWhereClause.FilterValues).Select(x => x.ProfileId != null ? x.ProfileId : 0)?.ToList();
            ProfileListModel profileList;

            filters.Clear();
            if (profileIds?.Count() > 0)
                newFilter.Add(ZnodeProfileEnum.ProfileId.ToString(), FilterOperators.NotIn, string.Join(",", profileIds));
                newFilter.Add(ZnodeProfileEnum.ParentProfileId.ToString(), FilterOperators.Equals,"null");

            //Get unassociated profile list.
            List<ZnodeProfile> list = _profile.GetPagedList(RemovefilterToGetProfileList(sorts, page, ref newFilter)?.EntityWhereClause.WhereClause, pageListModel.OrderBy, RemovefilterToGetProfileList(sorts, page, ref newFilter)?.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount)?.ToList();
            ZnodeLogging.LogMessage("List counts:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { profileIdsCount= profileIds?.Count(), ZnodeProfileCount=list?.Count() });
            profileList = new ProfileListModel { Profiles = list.ToModel<ProfileModel>()?.ToList() };

            profileList.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return profileList;
        }

        //Unassociate associated profiles.
        public virtual bool UnAssociateProfiles(ParameterModel profileIds, int userId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            if (string.IsNullOrEmpty(profileIds.Ids))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorProfileIdNull);

            List<string> profileIdList = profileIds.Ids.Split(',').ToList();
            ZnodeLogging.LogMessage("profileIdList list count:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, profileIdList?.Count());
            //Get profile list of user if user Id > 0.
            if (userId > 0)
            {
                //Delete profiles from profileIds list except default profile.
                ZnodeLogging.LogMessage("Parameter for DeleteProfileAssociation", ZnodeLogging.Components.Customers.ToString(),TraceLevel.Verbose, new object[] { userId, profileIdList });

                DeleteProfileAssociation(userId, profileIdList);
            }

            //Generates filter clause for multiple profile ids.
            FilterCollection filter = new FilterCollection();
            filter.Add(new FilterTuple(ZnodeUserProfileEnum.ProfileId.ToString(), ProcedureFilterOperators.In, profileIds.Ids));
            filter.Add(new FilterTuple(ZnodeUserEnum.UserId.ToString(), ProcedureFilterOperators.Equals, userId.ToString()));

            //Returns true if deleted successfully else return false.
            bool IsDeleted = _userProfile.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection()).WhereClause);

            ZnodeLogging.LogMessage(IsDeleted ? string.Format(Admin_Resources.SuccessUnassociateProfile, profileIds.Ids, userId) : string.Format(Admin_Resources.ErrorDissociateProfile, profileIds.Ids, userId), ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return IsDeleted;
        }

        //Associate profiles to customer.
        public virtual bool AssociateProfiles(ParameterModelUserProfile model)
        {

            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter ParameterModelUserProfile having profileIds", ZnodeLogging.Components.Customers.ToString(),TraceLevel.Verbose, new object[] { model?.ProfileIds });
            if (model?.ProfileIds.Length < 0)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorModelCountLessThanOne);

            string[] profileIds = model?.ProfileIds.Split(',');

            List<ZnodeUserProfile> entriesToInsert = new List<ZnodeUserProfile>();

            if (IsNotNull(profileIds))
                foreach (string item in profileIds)
                    entriesToInsert.Add(new ZnodeUserProfile() { ProfileId = Convert.ToInt32(item), UserId = model.UserId });

            IEnumerable<ZnodeUserProfile> associateProfiles = _userProfile.Insert(entriesToInsert);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return associateProfiles?.Count() > 0;
        }

        //Set default profile for customer.
        public virtual bool SetDefaultProfile(ParameterModelUserProfile model)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter ParameterModelUserProfile having UserId and ProfileIds", ZnodeLogging.Components.Customers.ToString(),TraceLevel.Verbose, new object[] { model.UserId, model.ProfileIds });
            //Throw exception for not deleting default profile.
            if (IsNull(model) || (model.UserId < 1 || string.IsNullOrEmpty(model.ProfileIds)))
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.ErrorProfileIdLessThanOne);

            //Set IsDefault Profile flag false for all profiles of customer except new default profile.
            ZnodeUserProfile defaultUserProfile = _userProfile.Table.Where(x => x.UserId == model.UserId && x.IsDefault == true)?.FirstOrDefault();
            if (SetCustomerDefaultProfile(defaultUserProfile, false) || IsNull(defaultUserProfile))
            {
                int profileId = Convert.ToInt32(model.ProfileIds);

                //Set new default profile for customer.
                ZnodeUserProfile selectedUserProfile = _userProfile.Table.Where(x => x.UserId == model.UserId && x.ProfileId == profileId)?.FirstOrDefault();
                return SetCustomerDefaultProfile(selectedUserProfile, true);
            }
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return false;
        }

        //Get associated profile list to customer.
        public virtual ProfileListModel GetCustomerPortalProfilelist(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to generate ZnodeProfile list ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            int userId;
            Int32.TryParse(filters.FirstOrDefault(x => x.FilterName.Equals(ZnodeUserEnum.UserId.ToString(), StringComparison.InvariantCultureIgnoreCase))?.FilterValue, out userId);

            int portalId;
            Int32.TryParse(filters.FirstOrDefault(x => x.FilterName.Equals(ZnodePortalEnum.PortalId.ToString(), StringComparison.InvariantCultureIgnoreCase))?.FilterValue, out portalId);

            //Filters to get profile name list.
            FilterCollection newFilter = new FilterCollection();
            newFilter.AddRange(filters);

            filters.RemoveAll(x => string.Equals(x.FilterName, FilterKeys.PortalId, StringComparison.CurrentCultureIgnoreCase) || string.Equals(x.FilterName, $"{ FilterKeys.PortalId}{'|'}", StringComparison.CurrentCultureIgnoreCase));
            filters.RemoveAll(x => string.Equals(x.FilterName, FilterKeys.UserId, StringComparison.CurrentCultureIgnoreCase) || string.Equals(x.FilterName, $"{ FilterKeys.UserId}{'|'}", StringComparison.CurrentCultureIgnoreCase));

            //Method to get associated profiles.
            var associatedProfiles = (from userprofile in _userProfile.Table
                                      join portalProfile in _portalProfile.Table on userprofile.ProfileId equals portalProfile.ProfileId
                                      where userprofile.UserId == userId
                                      && portalProfile.PortalId == portalId
                                      select userprofile.ProfileId)?.ToList() ?? null;
            ZnodeLogging.LogMessage("associatedProfiles: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, associatedProfiles);
            ProfileListModel listModel = new ProfileListModel();
            if (associatedProfiles?.Count > 0)
            {
                newFilter.Add(new FilterTuple(ZnodeProfileEnum.ProfileId.ToString(), FilterOperators.In, string.Join(",", associatedProfiles)));

                //Get profile list
                List<ZnodeProfile> list = _profile.GetPagedList(RemovefilterToGetPortalProfileList(sorts, page, ref newFilter, null)?.EntityWhereClause.WhereClause, pageListModel.OrderBy, RemovefilterToGetPortalProfileList(sorts, page, ref newFilter, null)?.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount)?.ToList();
                ZnodeLogging.LogMessage("ZnodeProfile list count:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, list?.Count());
                listModel = new ProfileListModel { Profiles = list.ToModel<ProfileModel>()?.ToList() };
            }

            //Set for pagination
            listModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return listModel;
        }
        #endregion

        #region Customer Affiliate
        //Get list of referral commission type.
        public virtual ReferralCommissionTypeListModel GetReferralCommissionTypeList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to generate referralCommissionTypeList list ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            ReferralCommissionTypeListModel referralCommissionTypeList = new ReferralCommissionTypeListModel
            {
                ReferralCommissionTypes = _referralCommissionType.GetEntityList(pageListModel.EntityWhereClause.WhereClause, pageListModel.EntityWhereClause.FilterValues)?.AsEnumerable()
                .ToModel<ReferralCommissionTypeModel>()?.ToList()
            };
            referralCommissionTypeList.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return referralCommissionTypeList;
        }

        //Get customer affiliate data.
        public virtual ReferralCommissionModel GetCustomerAffiliate(int userId, NameValueCollection expands)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            if (userId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.ErrorUserIdLessThanOne);

            ZnodeUser user = _user.GetEntity(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(GetUserIdWhereClause(userId).ToFilterDataCollection()).WhereClause, GetExpands(expands));

            var userReferralUrl = _usersReferralUrlRepository.GetEntityList(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(GetUserIdPortalIdWhereClause(userId, PortalId).ToFilterDataCollection()).WhereClause);
            ZnodeLogging.LogMessage("userReferralUrl:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, userReferralUrl);
            if (IsNotNull(user))
                return GetReferralCommissionMode(user, userReferralUrl);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return null;
        }

        //Update customer affiliate data.
        public virtual bool UpdateCustomerAffiliate(ReferralCommissionModel referralCommissionModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            if (IsNull(referralCommissionModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorReferralCommissionModelNull);

            if (referralCommissionModel.UserId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.ErrorUserIdLessThanOne);

            ZnodeUser user = _user.Table.FirstOrDefault(x => x.UserId == referralCommissionModel.UserId);
            if (IsNotNull(user))
            {
                UpdateUserForAffiliate(referralCommissionModel, user);
                ZnodeLogging.LogMessage(string.Format(Admin_Resources.SuccessUpdateCustomerAffiliate, referralCommissionModel.UserId), ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
                return true;
            }
            ZnodeLogging.LogMessage(string.Format(Admin_Resources.ErrorUpdateCustomerAffiliate, referralCommissionModel.UserId), ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return false;
        }

        //Get list of referral commission.
        public virtual ReferralCommissionListModel GetReferralCommissionList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to generate referralCommissionList list ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            ReferralCommissionListModel referralCommissionList = new ReferralCommissionListModel
            {
                ReferralCommissions = _viewCustomerReferralCommissionDetail.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount)?.AsEnumerable()
                .ToModel<ReferralCommissionModel>()?.ToList()
            };
            referralCommissionList.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return referralCommissionList;
        }
        #endregion

        #region Address
        //Get customer address list.
        public virtual AddressListModel GetAddressList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            ZnodeLogging.LogMessage("pageListModel to generate  AddressListModel ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            AddressListModel customerAddresses = UserMap.ToAddressListModel(_userAddress.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, GetExpands(expands), pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount), pageListModel);
            
            //Set one time address availability flag
            customerAddresses?.AddressList?.ForEach(o => o.DontAddUpdateAddress = true);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
         
            return customerAddresses;
        }

        //Get customer address.
        public virtual AddressModel GetCustomerAddress(FilterCollection filters, NameValueCollection expands)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            //Check for filter present in filters.
            if (filters?.Count > 0)
            {
                EntityWhereClauseModel whereClause = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
                ZnodeLogging.LogMessage("whereClause generated", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, whereClause);
                return UserMap.ToModel(_userAddress.GetEntity(whereClause.WhereClause, GetExpands(expands), whereClause.FilterValues));
            }

            throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorFiltersEmpty);
        }

        //Create customer address.
        public virtual AddressModel CreateCustomerAddress(AddressModel addressModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter having AddressModel UserId and AddressId", ZnodeLogging.Components.Customers.ToString(),TraceLevel.Verbose, new object[] { addressModel.UserId, addressModel.AddressId });
            if (IsNull(addressModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelNotNull);

            //User id should be there for creating customer address.
            if (addressModel.UserId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorUserIdLessThanOne);

            //Sets the IsDefaultBilling and IsDefaultShipping flag to false in database for Customer.
            AddressHelper.SetAddressFlagsToFalse(addressModel);
            if (addressModel.AddressId < 1)
            {
                ZnodeAddress address = _addressRepository.Insert(addressModel.ToEntity<ZnodeAddress>());
                addressModel.AddressId = address.AddressId;
            }

            //Check for address created or not.
            if (addressModel?.AddressId > 0 && !addressModel.DontAddUpdateAddress)
            {
                //Insert into mapper table.
                _userAddress.Insert(new ZnodeUserAddress() { UserId = addressModel.UserId, AddressId = addressModel.AddressId });
            }
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return addressModel;
        }

        //Update customer address.
        public virtual bool UpdateCustomerAddress(AddressModel addressModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Parameter AddressModel having AddressId", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new object[] {addressModel.AddressId });
            if (Equals(addressModel, null))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelNotNull);

            if (addressModel.AddressId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.ErrorAddressIdLessThanOne);

            //Sets the IsDefaultBilling and IsDefaultShipping flag to false in database for Customer.
            AddressHelper.SetAddressFlagsToFalse(addressModel);

            bool status = false;

            ZnodeAddress addressEntity = _addressRepository.Table?.FirstOrDefault(x => x.AddressId == addressModel.AddressId);

            //if IsBilling and IsShipping is enabled on single record.
            if (addressEntity.IsShipping && addressEntity.IsBilling && addressModel.omsOrderId > 0)
            {
                addressModel.AddressId = 0;
                addressModel.DisplayName = string.Concat(addressModel.DisplayName, "_Copy");
                if (addressModel.FromBillingShipping == "billing")
                {
                    addressModel.IsShipping = false;
                    addressEntity.IsShipping = false;
                }
                else
                {
                    addressModel.IsBilling = false;
                    addressEntity.IsBilling = false;
                }
                //Update the record when isShippingBillingSame check box and set as default is checked.

                if (addressModel.AddressId < 1)
                {
                    ZnodeAddress address = _addressRepository.Insert(addressModel.ToEntity<ZnodeAddress>());
                    addressModel.AddressId = address.AddressId;
                }
                if (addressModel?.AddressId > 0)
                {
                    //Insert into mapper table.
                    ZnodeUserAddress address = _userAddress.Insert(new ZnodeUserAddress() { UserId = addressModel.UserId, AddressId = addressModel.AddressId });
                    status = _addressRepository.Update(addressEntity);
                }
            }
            else
            {
                status = _addressRepository.Update(addressModel.ToEntity<ZnodeAddress>());
            }
            ZnodeLogging.LogMessage(status ? Admin_Resources.SuccessUpdateAccountAddress : Admin_Resources.ErrorUpdateAccountAddress, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return status;
        }

        //Delete customer address.
        public virtual bool DeleteCustomerAddress(ParameterModel userAddressId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Paramters ParameterModel having Ids:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info, userAddressId?.Ids);
            if (string.IsNullOrEmpty(userAddressId?.Ids))
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ErrorIdsEmpty);

            NameValueCollection expands = new NameValueCollection();
            expands.Add(ZnodeUserAddressEnum.ZnodeAddress.ToString(), ZnodeUserAddressEnum.ZnodeAddress.ToString());

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeUserAddressEnum.UserAddressId.ToString(), FilterOperators.In, userAddressId?.Ids?.ToString()));
            string whereClause = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause;

            List<ZnodeUserAddress> list = _userAddress.GetEntityList(whereClause, GetExpands(expands))?.ToList();
            ZnodeLogging.LogMessage("ZnodeUserAddress list count:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, list?.Count());
            //Exclude the IsDefaultBilling and IsDefaultShipping true from list.
            List<ZnodeUserAddress> listToDelete = list?.FindAll(x => !x.ZnodeAddress.IsDefaultBilling && !x.ZnodeAddress.IsDefaultShipping);
            ZnodeLogging.LogMessage("ZnodeUserAddress list count:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, listToDelete?.Count());
            bool isDeleted = false;
            string idsToDelete = string.Join(",", listToDelete?.Select(x => x.UserAddressId));
            ZnodeLogging.LogMessage("idsToDelete:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, idsToDelete);
            if (!string.IsNullOrEmpty(idsToDelete))
            {
                filters = new FilterCollection();
                filters.Add(new FilterTuple(ZnodeUserAddressEnum.UserAddressId.ToString(), FilterOperators.In, idsToDelete));
                whereClause = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause;
                ZnodeLogging.LogMessage("whereClause: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, whereClause);
                isDeleted = _userAddress.Delete(whereClause);
                ZnodeLogging.LogMessage(isDeleted ? Admin_Resources.SuccessDeleteCustomerAddress : Admin_Resources.ErrorDeleteCustomerAddress, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            }
            //Some of the address is IsDefaultBilling or IsDefaultShipping then throw exception.
            if (list?.Count != listToDelete?.Count)
                throw new ZnodeException(ErrorCodes.NotPermitted, Admin_Resources.DeleteDefaultAddressError);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return isDeleted;
        }

        //Get list of search location that matches the entered search term.
        public AddressListModel GetSearchLocation(int portalId, string searchTerm)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Paramters portalId and searchTerm:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info, new object[] { portalId, searchTerm });
            if (portalId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne,Admin_Resources.PortalNotIdLessThanOne);

            if (string.IsNullOrEmpty(searchTerm) || (searchTerm.Length < 3))
                return new AddressListModel { AddressList = new List<AddressModel>() };

            IZnodeViewRepository<AddressModel> objStoredProc = new ZnodeViewRepository<AddressModel>();
            objStoredProc.SetParameter("@PortalId", portalId, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Keywords", searchTerm, ParameterDirection.Input, DbType.String);

            IList<AddressModel> list = objStoredProc.ExecuteStoredProcedureList("Znode_GetSearchLocation @PortalId,@Keywords");
            ZnodeLogging.LogMessage("AddressModel list count:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, list?.Count());
            AddressListModel listModel = new AddressListModel();
            listModel.AddressList = list?.ToList();
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return listModel;
        }

        //Update Search Address.
        public AddressModel UpdateSearchAddress(AddressModel addressModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Paramters AddressModel having AddressId:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info, addressModel.AddressId);
            if (HelperUtility.IsNull(addressModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelNotNull);

            if (addressModel.AddressId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.ErrorAddressIdLessThanOne);

            ZnodeAddress _entity = _addressRepository.GetById(addressModel.AddressId);
            if (HelperUtility.IsNotNull(_entity))
            {
                _entity.Address2 = addressModel.Address2;
                _entity.Address3 = addressModel.Address3;
                _entity.CityName = addressModel.CityName;
                _entity.PostalCode = addressModel.PostalCode;
                _entity.StateName = addressModel.StateName;
                _addressRepository.Update(_entity);
                addressModel = _entity.ToModel<AddressModel>();
                return addressModel;
            }
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return addressModel;
        }

        #endregion

        #region Associate Price
        //Associate price list to customer.
        public virtual bool AssociatePriceList(PriceUserModel priceUserModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Paramters AddressModel having UserId and PriceListIds:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info, new object[] { priceUserModel?.UserId, priceUserModel?.PriceListIds });
            if (priceUserModel?.UserId < 1 || string.IsNullOrEmpty(priceUserModel?.PriceListIds))
                throw new ZnodeException(ErrorCodes.IdLessThanOne,Admin_Resources.PriceListAndUserIdLessThanOne);

            return AssociatePriceListToCustomer(priceUserModel.PriceListIds.Split(','), priceUserModel.UserId);
        }

        //UnAssociate associated price list from customer.
        public virtual bool UnAssociatePriceList(PriceUserModel priceUserModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Paramters AddressModel having UserId and PriceListIds:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info, new object[] { priceUserModel?.UserId, priceUserModel?.PriceListIds });

            if (IsNull(priceUserModel) || string.IsNullOrEmpty(priceUserModel.PriceListIds) || priceUserModel.UserId < 0)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.ModelInvalid);

            bool isDeleted = UnAssociatePriceListFromCustomer(priceUserModel);

            ZnodeLogging.LogMessage(isDeleted ? Admin_Resources.SuccessUnassociatePriceListFromCustomer : Admin_Resources.ErrorDissociatePriceListFromCustomer, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return isDeleted;
        }

        //Get Associated Price List with Precedence data for Customer.
        public virtual PriceUserModel GetAssociatedPriceListPrecedence(PriceUserModel priceUserModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Paramters AddressModel having UserId and PriceListIds:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info, new object[] { priceUserModel?.UserId, priceUserModel?.PriceListIds });

            if (priceUserModel?.PriceListId < 1 || priceUserModel?.UserId < 1)
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.PriceListAndUserIdLessThanOne);

            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodePriceListPortalEnum.PriceListId.ToString(), ProcedureFilterOperators.Equals, priceUserModel?.PriceListId.ToString()));

            //Get price list precedence for customer.
            return GetPriceListPrecedenceForCustomer(priceUserModel, filters);
        }

        //Update the precedence value for associated price list for Customer.
        public virtual bool UpdateAssociatedPriceListPrecedence(PriceUserModel priceUserModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Paramters AddressModel having UserId and PriceListIds:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info, new object[] { priceUserModel?.UserId, priceUserModel?.PriceListIds });

            if (IsNull(priceUserModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelNotNull);

            //Update price list precedence for customer.
            bool status = _priceListUserRepository.Update(priceUserModel.ToEntity<ZnodePriceListUser>());

            ZnodeLogging.LogMessage(status ? Admin_Resources.SuccessUpdatePricePrecedence : Admin_Resources.ErrorUpdatePricePrecedence, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return status;
        }
        #endregion        
        #endregion

        #region Private Method
        //Associate PriceList to Customer.
        private bool AssociatePriceListToCustomer(string[] priceListIds, int userId)
        {
            ZnodeLogging.LogMessage("Input Paramters userId:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info, userId);
            List<ZnodePriceListUser> entitiesToInsert = new List<ZnodePriceListUser>();

            foreach (string item in priceListIds)
                entitiesToInsert.Add(new ZnodePriceListUser() { UserId = userId, PriceListId = Convert.ToInt32(item), Precedence = ZnodeConstant.DefaultPrecedence });
            ZnodeLogging.LogMessage("entitiesToInsert list count:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, entitiesToInsert?.Count());
            return _priceListUserRepository.Insert(entitiesToInsert)?.Count() > 0;
        }

        //UnAssociate PriceList from Customer.
        private bool UnAssociatePriceListFromCustomer(PriceUserModel priceUserModel)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodePriceListPortalEnum.PriceListId.ToString(), ProcedureFilterOperators.In, priceUserModel.PriceListIds));
            filters.Add(new FilterTuple(ZnodePriceListUserEnum.UserId.ToString(), ProcedureFilterOperators.Equals, priceUserModel.UserId.ToString()));

            //Delete mapping of customer against price.
            return _priceListUserRepository.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause);
        }

        //Get Associated PriceList Precedence for B2C Customer.
        private PriceUserModel GetPriceListPrecedenceForCustomer(PriceUserModel priceUserModel, FilterCollection filters)
        {
            ZnodeLogging.LogMessage("Input Paramters UserId:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info, priceUserModel.UserId);
            filters.Add(new FilterTuple(ZnodePriceListUserEnum.UserId.ToString(), ProcedureFilterOperators.Equals, priceUserModel.UserId.ToString()));
            return _priceListUserRepository.GetEntity(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause)?.ToModel<PriceUserModel>();
        }

        //Get referral commission model.
        private ReferralCommissionModel GetReferralCommissionMode(ZnodeUser user, IList<ZnodeOmsUsersReferralUrl> userReferralUrl)
        {
            ReferralCommissionModel referralCommissionModel = new ReferralCommissionModel();

            //Sets the properties of referral commission model.
            referralCommissionModel.UserId = user.UserId;
            referralCommissionModel.ReferralCommission = user.ReferralCommission;
            referralCommissionModel.ReferralCommissionTypeId = user.ReferralCommissionTypeId;
            referralCommissionModel.ReferralStatus = user.ReferralStatus;
            referralCommissionModel.CustomerName = $"{user.FirstName} {user.LastName}";
            referralCommissionModel.ReferralCommissionType = _referralCommissionType.Table.Where(x => x.ReferralCommissionTypeId == user.ReferralCommissionTypeId)?.Select(y => y.Name)?.FirstOrDefault();
            referralCommissionModel.CurrencyCode = GetPortalDefaultCurrencyCode(PortalId);
            referralCommissionModel.CultureCode = GetPortalDefaultCultureCode(PortalId);
            List<int> domainIds = userReferralUrl?.Select(x => x.DomainId).ToList();
            ZnodeLogging.LogMessage("domainIds list count:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, domainIds?.Count());
            if (domainIds?.Count > 0)
            {
                var domainName = _domainRepository.GetEntityList(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(GetDomainIdWhereClause(string.Join(",", domainIds.Select(n => n.ToString()).ToArray())).ToFilterDataCollection()).WhereClause)?.Select(x => x.DomainName);
                ZnodeLogging.LogMessage("domainName:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, domainName);
                referralCommissionModel.Url = string.Join(",", domainName);
            }

            //Sets the portal for customer.
            SetPortals(user, referralCommissionModel);

            //If Domains exists then get the domain ids.
            if (user.ZnodeOmsUsersReferralUrls?.Count > 0)
            {
                referralCommissionModel.DomainIds = user.ZnodeOmsUsersReferralUrls.Select(x => x.DomainId.ToString())?.ToArray();
                referralCommissionModel.PortalId = user.ZnodeOmsUsersReferralUrls.FirstOrDefault().PortalId;
                referralCommissionModel.CurrencyCode = GetPortalCurrencyCode(referralCommissionModel.PortalId); 
                referralCommissionModel.CultureCode = GetPortalDefaultCultureCode(referralCommissionModel.PortalId);
            }

            //Calculate total amount owed for the customer.
            CalculateOwedAmount(referralCommissionModel, user);
            return referralCommissionModel;
        }

        //Get portal currency code.
        private string GetPortalCurrencyCode(int portalId)
        {
            ZnodeLogging.LogMessage("portalId: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, portalId);
            IZnodeRepository<ZnodePortalUnit> _portalUnit = new ZnodeRepository<ZnodePortalUnit>();

            FilterCollection filter = new FilterCollection();
            filter.Add(new FilterTuple(ZnodePortalEnum.PortalId.ToString(), ProcedureFilterOperators.Equals, portalId.ToString()));

            NameValueCollection expand = new NameValueCollection();
            expand.Add(ZnodePortalUnitEnum.ZnodeCurrency.ToString(), ZnodePortalUnitEnum.ZnodeCurrency.ToString());

            return _portalUnit.GetEntity(DynamicClauseHelper.GenerateDynamicWhereClause(filter.ToFilterDataCollection()), GetExpands(expand))?.ZnodeCurrency?.CurrencyCode;
        }

        //Sets the portal for customer.
        private void SetPortals(ZnodeUser user, ReferralCommissionModel referralCommissionModel)
        {
            ZnodeLogging.LogMessage("Inserted ReferralCommissionModel having AccountId:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, user.AccountId);
            if (user.AccountId > 0)
            {
                IZnodeRepository<ZnodePortalAccount> _portalAccount = new ZnodeRepository<ZnodePortalAccount>();
                List<ZnodePortalAccount> userPortalList = _portalAccount.Table.Where(x => x.AccountId == user.AccountId).ToList();
                ZnodeLogging.LogMessage("userPortalList list count:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, userPortalList?.Count());
                //Set account portals for customer.
                SetAccountPortals(referralCommissionModel, userPortalList);
            }
            else
            {
                IZnodeRepository<ZnodeUserPortal> _userPortal = new ZnodeRepository<ZnodeUserPortal>();
                List<ZnodeUserPortal> userPortalList = _userPortal.Table.Where(x => x.UserId == user.UserId).ToList();
                //Set user portals for customer.
                ZnodeLogging.LogMessage("userPortalList list count:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, userPortalList?.Count());
                SetUserPortals(referralCommissionModel, userPortalList);
            }
        }

        //Set user portals for customer.
        private static void SetUserPortals(ReferralCommissionModel referralCommissionModel, List<ZnodeUserPortal> userPortalList)
        {
            if (userPortalList?.Count > 0)
            {
                IZnodeRepository<ZnodePortal> _portal = new ZnodeRepository<ZnodePortal>();
                //If customer having all portal access.
                if (IsNotNull(userPortalList.Find(x => x.PortalId == null)))
                    referralCommissionModel.Portals = _portal.GetEntityList(string.Empty)?.ToModel<PortalModel>().ToList();
                else
                {
                    FilterCollection portal = new FilterCollection() { new FilterTuple(ZnodePortalEnum.PortalId.ToString(), FilterOperators.In, string.Join(",", userPortalList.Select(x => x.PortalId))) };
                    referralCommissionModel.Portals = _portal.GetEntityList(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(portal.ToFilterDataCollection()).WhereClause)?.ToModel<PortalModel>().ToList();
                }
            }
        }

        //Set account portals for customer.
        private static void SetAccountPortals(ReferralCommissionModel referralCommissionModel, List<ZnodePortalAccount> userPortalList)
        {
            if (userPortalList?.Count > 0)
            {
                IZnodeRepository<ZnodePortal> _portal = new ZnodeRepository<ZnodePortal>();
                //If customer having all portal access.
                if (IsNotNull(userPortalList.Find(x => x.PortalId == null)))
                    referralCommissionModel.Portals = _portal.GetEntityList(string.Empty)?.ToModel<PortalModel>().ToList();
                else
                {
                    FilterCollection portal = new FilterCollection() { new FilterTuple(ZnodePortalEnum.PortalId.ToString(), FilterOperators.In, string.Join(",", userPortalList.Select(x => x.PortalId))) };
                    referralCommissionModel.Portals = _portal.GetEntityList(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(portal.ToFilterDataCollection()).WhereClause)?.ToModel<PortalModel>().ToList();
                }
            }
        }

        //Get Filter for user id.
        private static FilterCollection GetUserIdWhereClause(int userId)
           => new FilterCollection() { new FilterTuple(ZnodeUserEnum.UserId.ToString(), ProcedureFilterOperators.Equals, userId.ToString()) };

        //Get Filter for domain id.
        private static FilterCollection GetDomainIdWhereClause(string domainIds)
           => new FilterCollection() { new FilterTuple(ZnodeDomainEnum.DomainId.ToString(), ProcedureFilterOperators.In, domainIds.ToString()) };

        private static FilterCollection GetUserIdPortalIdWhereClause(int userId, int portalId)
        {
            ZnodeLogging.LogMessage("Input Paramters userId and portalId: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new object[] { userId, portalId });
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeUserEnum.UserId.ToString(), ProcedureFilterOperators.Equals, userId.ToString()));
            filters.Add(new FilterTuple(ZnodePortalEnum.PortalId.ToString(), ProcedureFilterOperators.Equals, portalId.ToString()));
            return filters;
        }

        //Update the user for affiliate data.
        private void UpdateUserForAffiliate(ReferralCommissionModel referralCommissionModel, ZnodeUser user)
        {
            //Set the user data.
            user.ReferralCommission = referralCommissionModel.ReferralCommission;
            user.ReferralCommissionTypeId = referralCommissionModel.ReferralCommissionTypeId;
            user.ReferralStatus = referralCommissionModel.ReferralStatus;
            //update in database.
            ZnodeLogging.LogMessage(_user.Update(user)
                ? Admin_Resources.SuccessUpdateUserForAffiliate : Admin_Resources.ErrorUpdateUserForAffiliate, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            //Update domain information in database.
            UpdateDomainInformation(referralCommissionModel, user);
        }

        //Update the domain information.
        private void UpdateDomainInformation(ReferralCommissionModel referralCommissionModel, ZnodeUser user)
        {
            IZnodeRepository<ZnodeOmsUsersReferralUrl> _omsUsersReferralUrl = new ZnodeRepository<ZnodeOmsUsersReferralUrl>();
            //Delete the existing mapping.
            ZnodeLogging.LogMessage(_omsUsersReferralUrl.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(GetUserIdWhereClause(referralCommissionModel.UserId).ToFilterDataCollection()).WhereClause)
                ? Admin_Resources.SuccessDeleteUsersReferralUrl : Admin_Resources.ErrorDeleteUsersReferralUrl, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);

            //Referral status is approved and the associated domain count is greater than zero.
            if (referralCommissionModel.ReferralStatus.Equals("A") && referralCommissionModel.DomainIds.Any())
            {
                List<ZnodeOmsUsersReferralUrl> urlReferralList = new List<ZnodeOmsUsersReferralUrl>();

                //Prepare the Url list to insert in database.
                foreach (string domain in referralCommissionModel.DomainIds)
                    urlReferralList.Add(new ZnodeOmsUsersReferralUrl { DomainId = Convert.ToInt32(domain), PortalId = referralCommissionModel.PortalId, UserId = referralCommissionModel.UserId });
                ZnodeLogging.LogMessage("urlReferralList list count:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, urlReferralList?.Count());
                //Insert the new Url mapping.
                _omsUsersReferralUrl.Insert(urlReferralList);

                EmailTrackingLinks(referralCommissionModel, user);
                var onTrackingLinksInit = new ZnodeEventNotifier<ReferralCommissionModel>(referralCommissionModel, EventConstant.OnTrackingLinks);
            }
        }

        //Get expands and add them to navigation properties
        private List<string> GetExpands(NameValueCollection expands)
        {
            List<string> navigationProperties = new List<string>();
            if (expands?.HasKeys() ?? false)
            {
                foreach (string key in expands.Keys)
                {
                    //Add expand keys
                    if (Equals(key.ToLower(), ZnodeUserAddressEnum.ZnodeAddress.ToString().ToLower())) SetExpands(ZnodeUserAddressEnum.ZnodeAddress.ToString(), navigationProperties);
                    if (key.Equals(ZnodeUserEnum.ZnodeOmsUsersReferralUrls.ToString(), StringComparison.InvariantCultureIgnoreCase)) SetExpands(ZnodeUserEnum.ZnodeOmsUsersReferralUrls.ToString(), navigationProperties);
                    if (Equals(key.ToLower(), ZnodeUserAddressEnum.ZnodeUser.ToString().ToLower())) SetExpands(ZnodeUserAddressEnum.ZnodeUser.ToString(), navigationProperties);
                    if (Equals(key.ToLower(), ZnodePortalUnitEnum.ZnodeCurrency.ToString().ToLower())) SetExpands(ZnodePortalUnitEnum.ZnodeCurrency.ToString(), navigationProperties);
                }
            }
            return navigationProperties;
        }

        //Method to remove filter to get profile list based on profilename.
        private PageListModel RemovefilterToGetProfileList(NameValueCollection sorts, NameValueCollection page, ref FilterCollection newFilter)
        {
            PageListModel pageListModel = new PageListModel(newFilter, sorts, page);
            newFilter.RemoveAll(x => x.FilterName == ZnodeUserProfileEnum.UserId.ToString().ToLower());
            return pageListModel;
        }

        //Method to remove filter to get profile list based on profilename.
        private PageListModel RemovefilterToGetPortalProfileList(NameValueCollection sorts, NameValueCollection page, ref FilterCollection newFilter, List<int> productIds)
        {
            PageListModel pageListModel = new PageListModel(newFilter, sorts, page);
            newFilter.RemoveAll(x => x.FilterName == ZnodeUserProfileEnum.UserId.ToString().ToLower());
            newFilter.RemoveAll(x => x.FilterName == ZnodePortalProfileEnum.PortalId.ToString().ToLower());
            return pageListModel;
        }

        private void EmailTrackingLinks(ReferralCommissionModel referralCommissionModel, ZnodeUser user)
        {
            var data = _userPortalRepository.Table.Where(x => x.UserId == user.UserId);
            // Gets the portal ids.
            int portalId = string.IsNullOrEmpty(data?.Select(x => x.PortalId.ToString())?.FirstOrDefault()) ?
                            PortalId :
                            Convert.ToInt32(data?.Select(x => x.PortalId.ToString())?.FirstOrDefault());
            ZnodeLogging.LogMessage("Parameter: ", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose,new { data=data, portalId= portalId });
            //Method to get Email Template.
            EmailTemplateMapperModel emailTemplateMapperModel = GetEmailTemplateByCode(ZnodeConstant.TrackingLinks, portalId);

            if (IsNotNull(emailTemplateMapperModel))
            {
                try
                {
                    //Email body of affiliate tracking.
                    ZnodeEmail.SendEmail(user.Email, ZnodeConfigManager.SiteConfig.AdminEmail, ZnodeEmail.GetBccEmail(emailTemplateMapperModel.IsEnableBcc, portalId, string.Empty), emailTemplateMapperModel.Subject, GetEmailBody(referralCommissionModel, user, emailTemplateMapperModel.Descriptions), true);
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
                }
            }
        }

        //Get the email body of affiliate tracking.
        private string GetEmailBody(ReferralCommissionModel referralCommissionModel, ZnodeUser user, string messageText)
        {
            //Get all the associated domain details.
            IZnodeRepository<ZnodeDomain> _omsDomain = new ZnodeRepository<ZnodeDomain>();
            FilterCollection domainFilter = new FilterCollection { new FilterTuple(ZnodeDomainEnum.DomainId.ToString(), FilterOperators.In, string.Join(",", referralCommissionModel.DomainIds)) };
            List<ZnodeDomain> domainList = _omsDomain.GetEntityList(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(domainFilter.ToFilterDataCollection()).WhereClause)?.ToList();
            ZnodeLogging.LogMessage("domainList list count:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, domainList?.Count());
            //Prepare the referral urls.
            string referralUrl = string.Empty;
            if(IsNotNull(domainList))
                foreach (ZnodeDomain domain in domainList)
                    referralUrl += $"{domain.DomainName}?affiliateId={user.UserId}<br>";

            //Replace the token with actual content to show.
            messageText = ReplaceTokenWithMessageText("#UserName#", user.FirstName, messageText);
            messageText = ReplaceTokenWithMessageText("#Links#", referralUrl, messageText);
            messageText = ReplaceTokenWithMessageText("#StoreLogo#", GetCustomPortalDetails(referralCommissionModel.PortalId)?.StoreLogo, messageText);

            return messageText;
        }

        //Calculate total amount owed by customer.
        private void CalculateOwedAmount(ReferralCommissionModel referralCommissionModel, ZnodeUser user)
        {
            IZnodeRepository<ZnodeGiftCard> _giftCardRepository = new ZnodeRepository<ZnodeGiftCard>();

            //Get list of order commission amount and calculate its total.
            referralCommissionModel.TotalCommission = (_viewCustomerReferralCommissionDetail.Table.Where(x => x.UserId == user.UserId)?.Select(x => x.OrderCommission)?.ToList()?.Sum()).GetValueOrDefault();

            //Get list of owed amount and calculate its total.
            referralCommissionModel.OwedAmount = _giftCardRepository.Table.Where(x => x.UserId == user.UserId && x.IsReferralCommission == true)?.Select(x => x.Amount)?.ToList()?.Sum();

            //Amount to be paid to customer.
            referralCommissionModel.OwedAmount = referralCommissionModel.TotalCommission - referralCommissionModel.OwedAmount.GetValueOrDefault();
            ZnodeLogging.LogMessage("TotalCommission,OwedAmount:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info, new object[] { referralCommissionModel.TotalCommission, referralCommissionModel.OwedAmount });
        }

        //Delete profiles from profileIds list except default profile.
        private void DeleteProfileAssociation(int userId, List<string> profileIdList)
        {
            ZnodeLogging.LogMessage("Input Paramters userId:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info, userId);
            List<ZnodeUserProfile> userProfiles = _userProfile.Table.Where(x => x.UserId == userId)?.ToList();
            ZnodeLogging.LogMessage("userProfiles list count:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, userProfiles?.Count());
            //Throw exception if only one default profile is associate to customer.
            if (userProfiles?.Count() == 1)
                throw new ZnodeException(ErrorCodes.RestrictSystemDefineDeletion, Admin_Resources.ErrorDeleteDefaultProfile);

            //Check if profileIds contains default profile.
            int defaultProfileId = (userProfiles.Where(x => x.UserId == userId && x.IsDefault == true)?.FirstOrDefault()?.ProfileId).GetValueOrDefault();
            ZnodeLogging.LogMessage("defaultProfileId:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info, defaultProfileId);
            if ((profileIdList?.Contains(defaultProfileId.ToString())).GetValueOrDefault())
            {
                //if profileIds contains default profile then remove it and delete association of all other profile for customer.
                profileIdList?.Remove(defaultProfileId.ToString());

                //Throw exception for not deleting default profile.
                if (profileIdList?.Count() == 0) 
                    throw new ZnodeException(ErrorCodes.AssociationDeleteError, Admin_Resources.ErrorDeleteDefaultProfile);

                //Generates filter clause for multiple profile ids.
                FilterCollection filters = new FilterCollection();
                filters.Add(new FilterTuple(ZnodeUserProfileEnum.ProfileId.ToString(), ProcedureFilterOperators.In, profileIdList?.Count() != 0 ? string.Join(",", profileIdList) : "0"));
                filters.Add(new FilterTuple(ZnodeUserEnum.UserId.ToString(), ProcedureFilterOperators.Equals, userId.ToString()));

                //Delete profiles present in the profileIdList according to filters
                _userProfile.Delete(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection()).WhereClause);                            
            }
        }

        //Set default profile for customer.
        private bool SetCustomerDefaultProfile(ZnodeUserProfile userProfile, bool defaultFlag)
        {
            if (IsNotNull(userProfile))
            {
                userProfile.IsDefault = defaultFlag;
                return _userProfile.Update(userProfile);
            }
            return false;
        }

        //Get customer name and account id of user.
        private void GetCustomerNameAndAccountIdOfUser(int userId, ProfileListModel listModel)
        {
            ZnodeLogging.LogMessage("Input Paramters userId:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info, userId);
            ZnodeUser user = _user.Table.Where(x => x.UserId == userId)?.FirstOrDefault();
            if(IsNotNull(user))
            {
                listModel.CustomerName = user.FirstName + " " + user.LastName;
                listModel.AccountId = user.AccountId.GetValueOrDefault();
            }
        }

        //Get default currency assigned to current portal.
        private string GetPortalDefaultCurrencyCode(int portalId)
        {
            ZnodeLogging.LogMessage("Input Paramters portalId:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info, portalId);
            IZnodeRepository<ZnodePortalUnit> _portalUnitRepository = new ZnodeRepository<ZnodePortalUnit>();
            FilterCollection filter = new FilterCollection();
            filter.Add(ZnodePortalEnum.PortalId.ToString(), FilterOperators.Equals, portalId.ToString());

            NameValueCollection expand = new NameValueCollection();
            expand.Add(ZnodePortalUnitEnum.ZnodeCurrency.ToString(), ZnodePortalUnitEnum.ZnodeCurrency.ToString());
            return _portalUnitRepository.GetEntity(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection()).WhereClause, GetExpands(expand))?.ZnodeCurrency?.CurrencyCode;
        }
       
        //Get default currency assigned to current portal.
        private string GetPortalDefaultCultureCode(int portalId)
        {
            ZnodeLogging.LogMessage("Input Paramters portalId:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info, portalId);
            IZnodeRepository<ZnodePortalUnit> _portalUnitRepository = new ZnodeRepository<ZnodePortalUnit>();
            FilterCollection filter = new FilterCollection();
            filter.Add(ZnodePortalEnum.PortalId.ToString(), FilterOperators.Equals, portalId.ToString());

            NameValueCollection expand = new NameValueCollection();
            expand.Add(ZnodePortalUnitEnum.ZnodeCurrency.ToString(), ZnodePortalUnitEnum.ZnodeCurrency.ToString());
            return _portalUnitRepository.GetEntity(DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filter.ToFilterDataCollection()).WhereClause, GetExpands(expand))?.ZnodeCulture?.CultureCode;
        }
        #endregion

    }
}
