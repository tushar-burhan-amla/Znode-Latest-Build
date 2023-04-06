using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Admin.Agents
{
    public class CustomerAgent : BaseAgent, ICustomerAgent
    {
        #region Private Variables
        private readonly ICustomerClient _customerClient;
        private readonly IDomainClient _domainClient;
        private readonly IProfileClient _profileClient;
        private readonly IAccountClient _accountClient;
        private readonly IPriceClient _priceClient;
        private readonly IGiftCardClient _giftCardClient;
        private readonly IOrderClient _orderClient;
        #endregion

        #region Constructor
        public CustomerAgent(ICustomerClient customerClient, IProfileClient profileClient, IAccountClient accountClient, IDomainClient domainClient, IPriceClient priceClient, IGiftCardClient giftCardClient, IOrderClient orderClient)
        {
            _customerClient = GetClient<ICustomerClient>(customerClient);
            _profileClient = GetClient<IProfileClient>(profileClient);
            _accountClient = GetClient<IAccountClient>(accountClient);
            _domainClient = GetClient<IDomainClient>(domainClient);
            _priceClient = GetClient<IPriceClient>(priceClient);
            _giftCardClient = GetClient<IGiftCardClient>(giftCardClient);
            _orderClient = GetClient<IOrderClient>(orderClient);
        }
        #endregion

        #region public virtual Methods
        #region Profile Association
        // Get associated profile list.
        public virtual ProfileListViewModel GetAssociatedProfileList(FilterCollectionDataModel model, int userId)
        {
            ZnodeLogging.LogMessage("Agent Method Agent Method Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            //Set Filters for userId.
            HelperMethods.SetUserIdFilters(model.Filters, userId);

            //Get the sort collection for profilename asc.
            model.SortCollection = HelperMethods.SortAsc(ZnodeProfileEnum.ProfileName.ToString(), model.SortCollection);

            //Get list of associated profile.
            ProfileListModel list = _customerClient.GetAssociatedProfilelist(null, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            ProfileListViewModel listViewModel = new ProfileListViewModel { List = list?.Profiles?.ToViewModel<ProfileViewModel>()?.ToList(), CustomerName = list?.CustomerName, AccountId = list.AccountId };
            listViewModel.UserId = userId;
            SetListPagingData(listViewModel, list);

            //Set tool options for this grid.
            SetAssociatedProfileListToolMenu(listViewModel);
            ZnodeLogging.LogMessage("Agent Method Executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
           
            // checking whether guest user or not.
            IUserClient _userClient = new UserClient();
            bool? isGuestUser = _userClient.GetUserAccountData(userId)?.IsGuestUser;
            GridModel gridModel;

            if (isGuestUser == true)
            {
                gridModel = FilterHelpers.GetDynamicGridModel(model, listViewModel.List, GridListType.ZnodeGuestUserProfile.ToString(), string.Empty, null, true, true, listViewModel?.GridModel?.FilterColumn?.ToolMenuList);                
            }
            else
            {
                gridModel = FilterHelpers.GetDynamicGridModel(model, listViewModel.List, GridListType.ZnodeUserProfile.ToString(), string.Empty, null, true, true, listViewModel?.GridModel?.FilterColumn?.ToolMenuList);
            }
            gridModel.TotalRecordCount = listViewModel.TotalResults;
            listViewModel.GridModel = gridModel;
            return list?.Profiles?.Count > 0 ? listViewModel : new ProfileListViewModel() { List = new List<ProfileViewModel>(), UserId = userId, CustomerName = list?.CustomerName,GridModel=gridModel};
        }

        //Get list of unassociate profiles.
        public virtual ProfileListViewModel GetUnAssociatedProfileList(FilterCollectionDataModel model, int userId)
        {
            ZnodeLogging.LogMessage("Agent Method Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            //Set Filters for userId.
            HelperMethods.SetUserIdFilters(model.Filters, userId);

            //Get list of unassociated categories.
            ProfileListModel list = _customerClient.GetUnAssociatedProfileList(null, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);

            ProfileListViewModel listViewModel = new ProfileListViewModel { List = list?.Profiles?.ToViewModel<ProfileViewModel>()?.ToList() };
            listViewModel.UserId = userId;

            SetListPagingData(listViewModel, list);
            ZnodeLogging.LogMessage("Agent Method Executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return list?.Profiles?.Count > 0 ? listViewModel : new ProfileListViewModel() { List = new List<ProfileViewModel>(), UserId = userId };
        }

        //Remove associated profiles.
        public virtual bool UnAssociateProfiles(string profileIds, int userId, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent Method Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            errorMessage = Admin_Resources.UnassignError;
            try
            {
                return _customerClient.UnAssociateProfiles(new ParameterModel { Ids = profileIds }, userId);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.InvalidData:
                        errorMessage = Admin_Resources.TextInvalidData;
                        break;
                    case ErrorCodes.AssociationDeleteError:
                        errorMessage = Admin_Resources.ErrorDefaultProfileDelete;
                        break;
                    case ErrorCodes.RestrictSystemDefineDeletion:
                        errorMessage = Admin_Resources.ErrorDefaultProfileDelete;
                        break;
                    default:
                        errorMessage = Admin_Resources.UnassignError;
                        break;
                }
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                errorMessage = Admin_Resources.UnassignError;
                return false;
            }

        }

        //Associate profiles to customer.
        public virtual bool AssociateProfiles(string profileIds, int userId)
        {
            ZnodeLogging.LogMessage("Agent Method Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            try
            {
                return _customerClient.AssociateProfiles(new ParameterModelUserProfile { ProfileIds = profileIds, UserId = userId });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return false;
            }
        }

        //
        public virtual bool SetDefaultProfile(int userId, string profileId, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent Method Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            errorMessage = Admin_Resources.UpdateErrorMessage;
            try
            {
                if (userId > 0 && !string.IsNullOrEmpty(profileId))
                    return _customerClient.SetDefaultProfile(new ParameterModelUserProfile { UserId = userId, ProfileIds = profileId });

                return false;

            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.IdLessThanOne:
                        errorMessage = Admin_Resources.InvalidAccountInformation;
                        break;
                    case ErrorCodes.NotDeleteActiveRecord:
                        errorMessage = Admin_Resources.ErrorDefaultProfile;
                        break;
                    default:
                        errorMessage = Admin_Resources.UpdateErrorMessage;
                        break;
                }
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                errorMessage = Admin_Resources.UpdateErrorMessage;
                return false;
            }
        }
        #endregion

        #region Customer Note
        //Add customer note.
        public virtual NoteViewModel CreateCustomerNote(NoteViewModel noteViewModel)
        {
            ZnodeLogging.LogMessage("Agent Method Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            try
            {
                if (IsNotNull(noteViewModel))
                    return _accountClient.CreateAccountNote(noteViewModel.ToModel<NoteModel>())?.ToViewModel<NoteViewModel>();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        return (NoteViewModel)GetViewModelWithErrorMessage(noteViewModel, Admin_Resources.ErrorNoteAlreadyExists);
                    default:
                        return (NoteViewModel)GetViewModelWithErrorMessage(noteViewModel, Admin_Resources.ErrorFailedToCreate);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return (NoteViewModel)GetViewModelWithErrorMessage(noteViewModel, Admin_Resources.ErrorFailedToCreate);
            }
            return new NoteViewModel { HasError = true, ErrorMessage = Admin_Resources.ErrorFailedToCreate };
        }

        //Get customer note list from database.
        public virtual NoteListViewModel GetCustomerNotes(int userId, FilterCollection filters, SortCollection sorts, int? pageIndex, int? recordPerPage)
        {
            ZnodeLogging.LogMessage("Agent Method Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            //Get the sort collection for note id desc.
            sorts = HelperMethods.SortDesc(ZnodeNoteEnum.NoteId.ToString(), sorts);

            //Set filters for user id.
            HelperMethods.SetUserIdFilters(filters, userId);

            ZnodeLogging.LogMessage("Input parameters filters and sorts:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sorts });
            NoteListModel noteListModel = _accountClient.GetAccountNotes(null, filters, sorts, pageIndex, recordPerPage);
            NoteListViewModel noteListViewModel = new NoteListViewModel { Notes = noteListModel?.Notes?.ToViewModel<NoteViewModel>().ToList(), CustomerName = noteListModel?.CustomerName };
            SetListPagingData(noteListViewModel, noteListModel);

            //Set the Tool Menus for Customer Notes List on Grid View.
            SetCustomerNotesToolMenus(noteListViewModel);
            ZnodeLogging.LogMessage("Agent Method Executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return noteListViewModel?.Notes?.Count > 0 ? noteListViewModel : new NoteListViewModel() { Notes = new List<NoteViewModel>(), CustomerName = noteListModel?.CustomerName };
        }

        //Get customer note by noteId.
        public virtual NoteViewModel GetCustomerNote(int noteId)
        {
            ZnodeLogging.LogMessage("Agent Method Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            NoteModel noteModel = _accountClient.GetAccountNote(noteId);
            ZnodeLogging.LogMessage("Agent Method Executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return IsNotNull(noteModel) ? noteModel.ToViewModel<NoteViewModel>() : new NoteViewModel();
        }

        //Update customer note.
        public virtual NoteViewModel UpdateCustomerNote(NoteViewModel noteViewModel)
        {
            ZnodeLogging.LogMessage("Agent Method Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            try
            {
                return _accountClient.UpdateAccountNote(noteViewModel?.ToModel<NoteModel>())?.ToViewModel<NoteViewModel>();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                return (NoteViewModel)GetViewModelWithErrorMessage(noteViewModel, Admin_Resources.UpdateErrorMessage);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return (NoteViewModel)GetViewModelWithErrorMessage(noteViewModel, Admin_Resources.UpdateErrorMessage);
            }
        }

        //Delete Customer note on the basis of noteIds.
        public virtual bool DeleteCustomerNote(string noteIds)
        {
            ZnodeLogging.LogMessage("Agent Method Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            try
            {
                return _accountClient.DeleteAccountNote(new ParameterModel { Ids = noteIds });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return false;
            }
        }

        //Get Customer associated Order List.
        public virtual OrdersListViewModel GetCustomerOrderList(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null, int userId = 0)
        {
            ZnodeLogging.LogMessage("Agent Method Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            //If userId is greater than zero set filter for user id.
            if (userId > 0)
                HelperMethods.SetUserIdFilters(filters, userId);
            ZnodeLogging.LogMessage("Input parameters filters and sorts:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sortCollection });

            OrdersListModel orderList = _orderClient.GetOrderList(null, filters, sortCollection, pageIndex, recordPerPage);
            OrdersListViewModel ordersListViewModel = new OrdersListViewModel { List = orderList?.Orders?.ToViewModel<OrderViewModel>()?.ToList() };

            if (ordersListViewModel?.List?.Count > 0)
            {
                SetListPagingData(ordersListViewModel, orderList);

                foreach (OrderViewModel item in ordersListViewModel.List)
                {
                    item.UpdatePageType = AdminConstants.CustomerOrderHistory;
                    item.UserId = userId;
                    SetOrderListData(item);
                }
            }
            ZnodeLogging.LogMessage("Agent Method Executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            //If Order List View Model count is greater then BindDataToViewModel binds required data to order list view model.
            return ordersListViewModel?.List?.Count > 0 ? BindDataToViewModel(orderList, ordersListViewModel, userId) : new OrdersListViewModel() { UserId = userId, CustomerName = orderList.CustomerName };
        }

        #endregion

        #region Address
        public virtual AddressListViewModel GetAddressList(int userId, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent Method Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            IUserClient _userClient = new UserClient();
            UserModel userModel = _userClient.GetUserAccountData(userId);
            if (IsNotNull(userModel))
            {
                SetFiltersForAddress(filters, userModel);

                //Get the sort collection for address id desc.
                sortCollection = HelperMethods.SortDesc(ZnodeAddressEnum.AddressId.ToString(), sortCollection);

                //expand for address.
                ExpandCollection expands = new ExpandCollection();
                expands.Add(ZnodeUserAddressEnum.ZnodeAddress.ToString());
                ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { expands = expands, filters = filters, sorts = sortCollection });

                AddressListModel addressList = userModel.AccountId > 0
                    ? _accountClient.GetAddressList(expands, filters, sortCollection, pageIndex, recordPerPage)
                    : _customerClient.GetAddressList(expands, filters, sortCollection, pageIndex, recordPerPage);

                AddressListViewModel addressListViewModel = new AddressListViewModel { AddressList = addressList?.AddressList?.ToViewModel<AddressViewModel>().ToList() };                
                SetListPagingData(addressListViewModel, addressList);
                addressListViewModel.AccountId = Convert.ToInt32(userModel.AccountId);
                addressListViewModel.AccountName = userModel.Accountname;
                addressListViewModel.CustomerName = $"{userModel.FirstName} {userModel.LastName}";
                if(userModel.IsGuestUser && IsNotNull(addressListViewModel.AddressList))
                     addressListViewModel.AddressList.First().IsGuest = userModel.IsGuestUser;

                //Check if role name is administrator, set IsRoleAdministrator flag to true.
                addressListViewModel.IsRoleAdministrator = string.Equals(userModel.RoleName, ZnodeRoleEnum.Administrator.ToString(), StringComparison.CurrentCultureIgnoreCase);

                //Set the Tool Menus for Customer Address List Grid View.
                SetCustomerAddressToolMenus(addressListViewModel);
                ZnodeLogging.LogMessage("Agent Method Executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
                return addressListViewModel?.AddressList?.Count > 0 ? addressListViewModel
                    : new AddressListViewModel { AddressList = new List<AddressViewModel>(), AccountId = Convert.ToInt32(userModel.AccountId), AccountName = userModel.Accountname, CustomerName = $"{userModel.FirstName} {userModel.LastName}" };
            }
            return new AddressListViewModel { AddressList = new List<AddressViewModel>() };
        }

        //Get the address list.
        public virtual AddressListViewModel GetAddressList(FilterCollectionDataModel model, int userId)
        {
            ZnodeLogging.LogMessage("Agent Method Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);

            AddressListViewModel list = GetAddressList(userId, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            list.UserId = userId;

            // checking whether guest user or not.
            bool? checkIsGuestUser = list.AddressList.FirstOrDefault(x => x.UserId == userId)?.IsGuest;

            //Assign default view filter and sorting if exists for the first request.
            FilterHelpers.GetDefaultView((checkIsGuestUser != null && checkIsGuestUser == true) ? GridListType.ZnodeGuestUserAddress.ToString() : GridListType.AccountUserAddress.ToString(), model);
            ZnodeLogging.LogMessage("Input parameters having:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { filters = model.Filters, sorts = model.SortCollection });

           if (list.AccountId < 1)
            {
                if ((checkIsGuestUser != null && checkIsGuestUser == true) ? true : false)
                {
                    list.GridModel = FilterHelpers.GetDynamicGridModel(model, list.AddressList, GridListType.ZnodeGuestUserAddress.ToString(), string.Empty, null, true, true, list?.GridModel?.FilterColumn?.ToolMenuList);
                }
                else
                {
                    list.GridModel = FilterHelpers.GetDynamicGridModel(model, list.AddressList, GridListType.ZnodeUserAddress.ToString(), string.Empty, null, true, true, list?.GridModel?.FilterColumn?.ToolMenuList);
                }
            }
            else
            {
                if (list.IsRoleAdministrator)
                {
                    //Get the grid model.
                    list.GridModel = FilterHelpers.GetDynamicGridModel(model, list.AddressList, GridListType.ZnodeAccountUsersAddress.ToString(), string.Empty, null, true, true, list?.GridModel?.FilterColumn?.ToolMenuList);
                    list.AddressList.ForEach(x => x.UserId = userId);
                }
                else
                    list.GridModel = FilterHelpers.GetDynamicGridModel(model, list.AddressList, GridListType.AccountUserAddress.ToString(), string.Empty, null, true, true, list?.GridModel?.FilterColumn?.ToolMenuList);
            }

            //Set the total record count
            list.GridModel.TotalRecordCount = list.TotalResults;
            ZnodeLogging.LogMessage("Agent Method Executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return list;
        }

        //Create Customer Address.
        public virtual AddressViewModel CreateCustomerAddress(AddressViewModel addressViewModel)
        {
            ZnodeLogging.LogMessage("Agent Method Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            try
            {
                //If accountId is greater, create address for account else for customer.
                if (addressViewModel.AccountId > 0)
                    return _accountClient.CreateAccountAddress(addressViewModel?.ToModel<AddressModel>())?.ToViewModel<AddressViewModel>();
                else
                {
                    //Set default Billing and Shipping address true, if it is customer's first address.
                    SetDefaultBillingShipping(addressViewModel);
                    return _customerClient.CreateCustomerAddress(addressViewModel?.ToModel<AddressModel>())?.ToViewModel<AddressViewModel>();
                }
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.InvalidData:
                        return (AddressViewModel)GetViewModelWithErrorMessage(addressViewModel, Admin_Resources.ErrorDefaultBillingAndShippingNotSelected);
                    default:
                        return (AddressViewModel)GetViewModelWithErrorMessage(addressViewModel, Admin_Resources.ErrorFailedToCreate);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return (AddressViewModel)GetViewModelWithErrorMessage(addressViewModel, Admin_Resources.ErrorFailedToCreate);
            }
        }

        public virtual AddressViewModel GetCustomerAddress(int userAddressId, int accountAddressId, int userId)
        {
            ZnodeLogging.LogMessage("Agent Method Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            //expand for address.
            ExpandCollection expands = new ExpandCollection();
            expands.Add(ZnodeUserAddressEnum.ZnodeAddress.ToString());

            ZnodeLogging.LogMessage("Input parameters expands:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { expands = expands });
            FilterCollection filters = new FilterCollection();
            AddressModel addressModel = new AddressModel();

            //If accountAddressId is greater, get address for account else for customer.
            if (accountAddressId > 0)
            {
                filters.Add(ZnodeAccountAddressEnum.AccountAddressId.ToString(), FilterOperators.Equals, accountAddressId.ToString());
                addressModel = _accountClient.GetAccountAddress(expands, filters);
                addressModel.UserId = userId;
            }
            else
            {
                expands.Add(ZnodeUserAddressEnum.ZnodeUser.ToString());
                filters.Add(ZnodeUserAddressEnum.UserAddressId.ToString(), FilterOperators.Equals, userAddressId.ToString());
                addressModel = _customerClient.GetCustomerAddress(expands, filters);
            }
            ZnodeLogging.LogMessage("Agent Method Executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return IsNotNull(addressModel) ? addressModel.ToViewModel<AddressViewModel>() : new AddressViewModel() { UserId = userId };
        }

        public virtual AddressViewModel UpdateCustomerAddress(AddressViewModel addressViewModel)
        {
            ZnodeLogging.LogMessage("Agent Method Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            try
            {
                //If accountAddressId is greater, update address for account else for customer.
                if (addressViewModel.AccountAddressId > 0)
                    return _accountClient.UpdateAccountAddress(addressViewModel?.ToModel<AddressModel>())?.ToViewModel<AddressViewModel>();
                else
                    return _customerClient.UpdateCustomerAddress(addressViewModel?.ToModel<AddressModel>())?.ToViewModel<AddressViewModel>();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return (AddressViewModel)GetViewModelWithErrorMessage(addressViewModel, Admin_Resources.UpdateErrorMessage);
            }
        }

        public virtual bool DeleteCustomerAddress(string userAddressId, string accountAddressId, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent Method Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            errorMessage = Admin_Resources.ErrorFailedToDelete;
            try
            {
                //If accountAddressId is greater, delete account address else customer address.
                if (!string.IsNullOrEmpty(accountAddressId))
                    return _accountClient.DeleteAccountAddress(new ParameterModel { Ids = accountAddressId });
                else
                    return _customerClient.DeleteCustomerAddress(new ParameterModel { Ids = userAddressId });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                errorMessage = Admin_Resources.ErrorDeleteDefaultAddress;
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return false;
            }
        }
        #endregion

        #region Affiliate
        //Get the customer affiliate data.
        public virtual CustomerAffiliateViewModel GetCustomerAffiliateData(int userId)
        {
            ZnodeLogging.LogMessage("Agent Method Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            if (userId > 0)
            {
                ReferralCommissionModel model = _customerClient.GetCustomerAffiliate(userId, new ExpandCollection() { ZnodeUserEnum.ZnodeOmsUsersReferralUrls.ToString() });

                //If model is not null.
                if (IsNotNull(model))
                {
                    CustomerAffiliateViewModel viewModel = model.ToViewModel<CustomerAffiliateViewModel>();

                    //Binds the portals.
                    viewModel.AvailablePortals = model.Portals?.Count > 0
                        ? model.Portals.Select(x => new SelectListItem { Text = x.StoreName, Value = x.PortalId.ToString() }).ToList()
                        : new List<SelectListItem>();

                    viewModel.ApprovalStatusList = HelperMethods.GetApprovalStatusDictionary();

                    //Get the referral commission types list.
                    viewModel.ReferralCommissionTypes = GetReferralCommissionTypes();
                    return viewModel;
                }
            }
            return new CustomerAffiliateViewModel() { HasError = true };
        }

        //Gets the webstore url on portal id.
        public virtual List<SelectListItem> GetDomains(int portalId, int userId)
        {
            ZnodeLogging.LogMessage("Agent Method Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            FilterCollection filters = new FilterCollection()
            { new FilterTuple(ZnodeDomainEnum.PortalId.ToString(), FilterOperators.Equals, portalId.ToString()),
              new FilterTuple(ZnodeDomainEnum.ApplicationType.ToString(), FilterOperators.Like,ApplicationTypesEnum.WebStore.ToString() )
            };

            ZnodeLogging.LogMessage("Filters:", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Verbose, new { filters = filters });
            DomainListModel domainList = _domainClient.GetDomains(filters, null, null, null);

            if (domainList?.Domains?.Count > 0)
                return domainList.Domains.Select(x => new SelectListItem { Text = $"{x.DomainName}?affiliateId={userId}", Value = x.DomainId.ToString() }).ToList();
            ZnodeLogging.LogMessage("Agent Method Executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return new List<SelectListItem>();
        }

        //Create Tab Structure.
        public virtual TabViewListModel CreateTabStructure(int userId)
        {
            ZnodeLogging.LogMessage("Agent Method Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            TabViewListModel TabStructModel = new TabViewListModel();
            TabStructModel.MaintainAllTabData = false;
            string controllerName = Admin_Resources.CustomerHeader;
            ZnodeLogging.LogMessage("controllerName:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { controllerName = controllerName });

            TabStructModel.Tabs.Add(new TabViewModel() { Id = 1, Name = Admin_Resources.LabelReferralCommissionList, IsVisible = true, Url = $"/{controllerName}/GetReferralCommissionList?userId={userId}", IsSelected = true });

            TabStructModel.Tabs.Add(new TabViewModel() { Id = 2, Name = Admin_Resources.LabelAccountPaymentList, IsVisible = true, Url = $"/{controllerName}/GetPaymentList?userId={userId}" });
            ZnodeLogging.LogMessage("Agent Method Executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return TabStructModel;
        }

        //Save the affiliate data.
        public virtual CustomerAffiliateViewModel SaveAffiliateData(CustomerAffiliateViewModel viewModel)
        {
            ZnodeLogging.LogMessage("Agent Method Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            if (HelperUtility.IsNotNull(viewModel))
            {
                ReferralCommissionModel updatedModel = _customerClient.UpdateCustomerAffiliate(viewModel.ToModel<ReferralCommissionModel>());
                if (HelperUtility.IsNotNull(updatedModel))
                    return viewModel;
            }
            ZnodeLogging.LogMessage("Agent Method Executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return new CustomerAffiliateViewModel { HasError = true };
        }

        // Get referral commission list.
        public virtual ReferralCommissionListViewModel GetReferralCommissionList(FilterCollectionDataModel model, int userId)
        {
            ZnodeLogging.LogMessage("Agent Method Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            //Set Filters for userId.
            HelperMethods.SetUserIdFilters(model.Filters, userId);
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { filters = model.Filters, sorts = model.SortCollection });

            // Get referral commission list.
            ReferralCommissionListModel list = _customerClient.GetReferralCommissionlist(null, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            if (list?.ReferralCommissions?.Count > 0)
            {
                ReferralCommissionListViewModel listViewModel = new ReferralCommissionListViewModel { List = list?.ReferralCommissions?.ToViewModel<ReferralCommissionViewModel>()?.ToList() };
                listViewModel.UserId = userId;

                SetListPagingData(listViewModel, list);
                return listViewModel;
            }
            ZnodeLogging.LogMessage("Agent Method Executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return new ReferralCommissionListViewModel() { List = new List<ReferralCommissionViewModel>(), UserId = userId };
        }

        // Get referral commission payment list.
        public virtual GiftCardListViewModel GetPaymentList(FilterCollectionDataModel model, int userId)
        {
            ZnodeLogging.LogMessage("Agent Method Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            //Set Filters for userId.
            HelperMethods.SetUserIdFilters(model.Filters, userId);
            SetIsReferralCommissionFilters(model.Filters);

            SortCollection sortlist = new SortCollection();
            sortlist.Add(SortKeys.ModifiedDate, DynamicGridConstants.DESCKey);

            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { expands = model.Expands, filters = model.Filters, sorts = model.SortCollection });
            GiftCardListModel giftCardList = _giftCardClient.GetGiftCardList(model.Expands, model.Filters, model.SortCollection, model.Page, model.RecordPerPage);
            GiftCardListViewModel listViewModel = new GiftCardListViewModel { GiftCardList = giftCardList?.GiftCardList?.ToViewModel<GiftCardViewModel>()?.ToList(), ReferralCommissionCount = giftCardList.ReferralCommissionCount };
            listViewModel.UserId = userId;
            SetListPagingData(listViewModel, giftCardList);
            ZnodeLogging.LogMessage("Agent Method Executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return giftCardList?.GiftCardList?.Count > 0 ? listViewModel : new GiftCardListViewModel() { GiftCardList = new List<GiftCardViewModel>(), UserId = userId, ReferralCommissionCount = listViewModel.ReferralCommissionCount };
        }

        //Get the data required to create referral commission payment.
        public virtual GiftCardViewModel GetDataToCreatePayment(int userId)
        {
            ZnodeLogging.LogMessage("Agent Method Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            GiftCardViewModel viewModel = new GiftCardViewModel();
            if (userId > 0)
            {
                ReferralCommissionModel referralCommissionModel = _customerClient.GetCustomerAffiliate(userId, new ExpandCollection() { ZnodeUserEnum.ZnodeOmsUsersReferralUrls.ToString() });
                viewModel.OwedAmount = referralCommissionModel.OwedAmount.GetValueOrDefault();
                viewModel.CurrencyCode = DefaultSettingHelper.DefaultCurrency;
                viewModel.CultureCode = DefaultSettingHelper.DefaultCulture;
                viewModel.UserId = userId;
                viewModel.PortalId = referralCommissionModel.PortalId;
                viewModel.StartDate = DateTime.Now;

                if (referralCommissionModel.OwedAmount.GetValueOrDefault() <= 0)
                    return (GiftCardViewModel)GetViewModelWithErrorMessage(viewModel, Admin_Resources.PaymentPaidErrorMessage);

                viewModel.CardNumber = _giftCardClient.GetRandomGiftCardNumber();
            }
            ZnodeLogging.LogMessage("Agent Method Executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return viewModel;
        }
        #endregion

        #region Associate Price
        //Get Price list for Customer.
        public virtual PriceListViewModel GetPriceList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize, int userId)
        {
            ZnodeLogging.LogMessage("Agent Method Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            //Set filter for customer id and set account mode filter.
            SetFiltersForId(filters, userId, ZnodeUserEnum.UserId.ToString());
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { expands = expands, filters = filters, sorts = sorts });

            PriceListModel priceList = _priceClient.GetPriceList(expands, filters, sorts, pageIndex, pageSize);
            PriceListViewModel listViewModel = new PriceListViewModel { PriceList = priceList?.PriceList?.ToViewModel<PriceViewModel>().ToList() };

            SetListPagingData(listViewModel, priceList);

            SetPriceListToolMenuForCustomer(listViewModel);
            if (IsNotNull(listViewModel))
            {
                listViewModel.PriceList?.ForEach(x => x.UserId = userId);
                listViewModel.UserId = userId;
                listViewModel.CustomerName = priceList.CustomerName;
            }
            ZnodeLogging.LogMessage("Agent Method Executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return priceList?.PriceList?.Count > 0 ? listViewModel : new PriceListViewModel() { PriceList = new List<PriceViewModel>(), UserId = userId, CustomerName = priceList?.CustomerName };
        }

        //Get UnAssociated Price List to Customer.
        public virtual PriceListViewModel GetUnAssociatedPriceList(int userId, ExpandCollection expands = null, FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent Method Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            //Set filter for user id and set user mode filter.
            SetFiltersForId(filters, userId, ZnodeUserEnum.UserId.ToString());
            try
            {
                PriceListModel priceList = _priceClient.GetUnAssociatedPriceList(expands, filters, sorts, pageIndex, recordPerPage);
                PriceListViewModel priceListViewModel = new PriceListViewModel { PriceList = priceList?.PriceList?.ToViewModel<PriceViewModel>().ToList() };
                SetListPagingData(priceListViewModel, priceList);
                ZnodeLogging.LogMessage("Agent Method Executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
                return priceList?.PriceList?.Count > 0 ? priceListViewModel : new PriceListViewModel() { PriceList = new List<PriceViewModel>() };
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return (PriceListViewModel)GetViewModelWithErrorMessage(new PriceListViewModel() { PriceList = new List<PriceViewModel>() }, string.Empty);
            }
        }

        //Associate Price List to Customer.
        public virtual bool AssociatePriceList(int userId, string priceListIds)
        {
            ZnodeLogging.LogMessage("Agent Method Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            try
            {
                return _customerClient.AssociatePriceList(new PriceUserModel { UserId = userId, PriceListIds = priceListIds });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return false;
            }
        }

        //Remove associated price list from Customer.
        public virtual bool UnAssociatePriceList(string priceListIds, int userId, out string message)
        {
            ZnodeLogging.LogMessage("Agent Method Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            message = string.Empty;
            try
            {
                return _customerClient.UnAssociatePriceList(new PriceUserModel() { PriceListIds = priceListIds, UserId = userId });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AssociationDeleteError:
                        message = Admin_Resources.CustomerPriceListDeleteError;
                        return false;
                    default:
                        message = Admin_Resources.ErrorFailedToDelete;
                        return false;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                message = Admin_Resources.ErrorFailedToDelete;
                return false;
            }
        }

        //Get associated price list precedence value for Account.
        public virtual PriceUserViewModel GetAssociatedPriceListPrecedence(int priceListId, int userId, string listName)
        {
            ZnodeLogging.LogMessage("Agent Method Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            PriceUserModel priceUserModel = _customerClient.GetAssociatedPriceListPrecedence(new PriceUserModel { PriceListId = priceListId, UserId = userId });
            if (IsNotNull(priceUserModel))
            {
                priceUserModel.Name = listName;
                return priceUserModel.ToViewModel<PriceUserViewModel>();
            }
            ZnodeLogging.LogMessage("Agent Method Executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return new PriceUserViewModel() { Name = listName, PriceListId = priceListId, UserId = userId, HasError = true };
        }

        //Update associated price list precedence value for Customer.
        public virtual bool UpdateAssociatedPriceListPrecedence(PriceUserViewModel priceUserViewModel)
        {
            ZnodeLogging.LogMessage("Agent Method Execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            try
            {
                priceUserViewModel = _customerClient.UpdateAssociatedPriceListPrecedence(priceUserViewModel?.ToModel<PriceUserModel>())?.ToViewModel<PriceUserViewModel>();

                return (IsNotNull(priceUserViewModel) && !priceUserViewModel.HasError) ? true : false;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                priceUserViewModel.ErrorMessage = Admin_Resources.UpdateErrorMessage;
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                priceUserViewModel.ErrorMessage = Admin_Resources.UpdateErrorMessage;
                return false;
            }
        }
        #endregion 
        #endregion

        #region Private Method
        //Set tool option menus for associated profile list grid.
        private void SetAssociatedProfileListToolMenu(ProfileListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.IsDefault, JSFunctionName = "Customer.prototype.SetdefaultProfile()", ControllerName = "Customer", ActionName = "SetDefaultProfile" });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('AssociatedProfilePopup')", ControllerName = "Customer", ActionName = "UnAssociateProfiles" });
            }
        }

        //Get the referral commission types list.
        private List<SelectListItem> GetReferralCommissionTypes()
        {
            ReferralCommissionTypeListModel referralCommissionTypes = _customerClient.GetReferralCommissionTypeList(null, null, null, null, null);

            return referralCommissionTypes?.ReferralCommissionTypes?.Count > 0 ?
                referralCommissionTypes.ReferralCommissionTypes.Select(x => new SelectListItem { Text = x.Name, Value = x.ReferralCommissionTypeId.ToString() }).ToList()
                : new List<SelectListItem>();
        }

        //Set filters for ids.
        private void SetFiltersForId(FilterCollection filters, int id, string filterKey)
        {
            ZnodeLogging.LogMessage("Input parameters filters:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { filters = filters });
            if (IsNotNull(filters))
            {
                //If user id is already present in filters, remove it.
                filters.RemoveAll(x => x.Item1 == ZnodeUserEnum.UserId.ToString());

                //Add new id into filters.
                filters.Add(new FilterTuple(filterKey, FilterOperators.Equals, id.ToString()));


                //If Mode is already present in filters, remove it.
                filters.RemoveAll(x => x.Item1 == FilterKeys.Mode);

                //Add New Mode into filters.
                filters.Add(new FilterTuple(FilterKeys.Mode, FilterOperators.Equals, ZnodeConstant.User));
            }
        }

        //Set the Tool Menus for Customer Notes List Grid View.
        private void SetCustomerNotesToolMenus(NoteListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('NoteDeletePopup')", ControllerName = "Customer", ActionName = "DeleteCustomerNote" });
            }
        }

        //Set the Tool Menus for Customer Address List Grid View.
        private void SetCustomerAddressToolMenus(AddressListViewModel model)
        {
            if (IsNotNull(model) && (model.AccountId < 1 || model.IsRoleAdministrator) && IsNotNull(model.AddressList) && IsNotNull(model.AddressList.First()) && !model.AddressList.First().IsGuest)
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('AddressDeletePopup')", ControllerName = "Customer", ActionName = "DeleteAddress" });
            }
        }

        //Set the Tool Menus for Price List Grid View.
        private void SetPriceListToolMenuForCustomer(PriceListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('PriceListDeletePopup')", ControllerName = "Customer", ActionName = "UnAssociatePriceList" });
            }
        }

        //Set the filters for getting the address.
        private void SetFiltersForAddress(FilterCollection filters, UserModel userModel)
        {
            ZnodeLogging.LogMessage("Input parameters filters:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { filters = filters });
            if (userModel.AccountId > 0)
            {
                //If Id Already present in filters Remove It
                filters.RemoveAll(x => x.Item1 == ZnodeNoteEnum.UserId.ToString());
                //Set filters for account id.
                HelperMethods.SetAccountIdFilters(filters, Convert.ToInt32(userModel.AccountId));
            }
            else
            {
                //If AccountId Already present in filters Remove It
                filters.RemoveAll(x => x.Item1.ToLower() == ZnodeAccountPermissionEnum.AccountId.ToString().ToLower());
                //Set filters for user id.
                HelperMethods.SetUserIdFilters(filters, userModel.UserId);
            }
        }

        //Set default Billing and Shipping address true, if it is customer's first address.
        private void SetDefaultBillingShipping(AddressViewModel addressViewModel)
        {
            if (Equals(addressViewModel?.ListCount, 0))
            {
                addressViewModel.IsDefaultBilling = true;
                addressViewModel.IsDefaultShipping = true;
            }
        }

        //Set tool option menus for referral commission list grid.
        private void SetReferralCommissionListToolMenu(ReferralCommissionListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = new GridModel();
                model.GridModel.FilterColumn = new FilterColumnListModel();
                model.GridModel.FilterColumn.ToolMenuList = new List<ToolMenuModel>();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('AssociatedProfilePopup')", ControllerName = "Customer", ActionName = "GetReferralCommissionList" });
            }
        }

        //Set IsReferralCommission flag true to get referral commission payment list. 
        private void SetIsReferralCommissionFilters(FilterCollection filters)
        {
            if (IsNotNull(filters))
            {

                ZnodeLogging.LogMessage("Input parameters filters:", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Verbose, new { filters = filters });
                //If flag Already present in filters Remove It
                filters.RemoveAll(x => string.Equals(x.FilterName, ZnodeGiftCardEnum.IsReferralCommission.ToString(), StringComparison.CurrentCultureIgnoreCase));

                //Add IsReferralCommission true filter.
                filters.Add(ZnodeGiftCardEnum.IsReferralCommission.ToString(), FilterOperators.Equals, FilterKeys.ActiveTrue);
            }
        }

        //Bind required data to order list view model.
        private OrdersListViewModel BindDataToViewModel(OrdersListModel ordersListModel, OrdersListViewModel ordersListViewModel, int userId)
        {
            //Get Order State List.
            ordersListViewModel.CustomerName = ordersListModel.CustomerName;
            ordersListViewModel.UserId = userId;
            return ordersListViewModel;
        }

        //Set order list data to OrderViewModel.
        private static void SetOrderListData(OrderViewModel order)
        {
            if (IsNotNull(order))
                order.OrderTotalWithCurrency = HelperMethods.FormatPriceWithCurrency(order.Total, order.CultureCode);
        }
        #endregion

        #region Impersonation
        // Get list of available portals for the perticular shopper.
        public virtual ImpersonationViewModel GetImpersonationByUserId(int userId)
        {

            CustomerAffiliateViewModel customerAffiliateViewModel = GetCustomerAffiliateData(userId);
            if (HelperUtility.IsNull(customerAffiliateViewModel))
            {
                return null;
            }
            ImpersonationViewModel impersonationViewModel = new ImpersonationViewModel();
            impersonationViewModel.UserId = userId;
            impersonationViewModel.AvailablePortals = customerAffiliateViewModel.AvailablePortals;
            impersonationViewModel.CustomerName = customerAffiliateViewModel.CustomerName;
            if (impersonationViewModel?.AvailablePortals?.Count > 1)
            {
                impersonationViewModel?.AvailablePortals.Insert(0, new SelectListItem { Text = Admin_Resources.LabelSelectStore, Value = "0" });
            }
            else if (impersonationViewModel.AvailablePortals?.Count == 1)
            {
                impersonationViewModel.PortalId = Convert.ToInt32(impersonationViewModel?.AvailablePortals.FirstOrDefault().Value);
            }
            return impersonationViewModel;
        }
        #endregion
    }
}