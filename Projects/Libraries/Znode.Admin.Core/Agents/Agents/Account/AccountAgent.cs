using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Maps;
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
    public class AccountAgent : BaseAgent, IAccountAgent
    {
        #region Private Variables
        private readonly IAccountClient _accountClient;
        private readonly IRoleAgent _roleAgent;
        private readonly IAccessPermissionClient _accessPermissionClient;
        private readonly IUserClient _userClient;
        private readonly IPriceClient _priceClient;
        #endregion

        #region Constructor
        public AccountAgent(IAccountClient accountClient, IAccessPermissionClient accessPermissionClient, IUserClient userClient, IPriceClient priceClient)
        {
            _accountClient = GetClient<IAccountClient>(accountClient);
            _roleAgent = new RoleAgent(GetClient<RoleClient>());
            _accessPermissionClient = GetClient<IAccessPermissionClient>(accessPermissionClient);
            _userClient = GetClient<IUserClient>(userClient);
            _priceClient = GetClient<IPriceClient>(priceClient);
        }
        #endregion

        #region Public Methods
        //Get user account list  
        public virtual AccountListViewModel GetAccountList(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            if (IsNull(filters))
                filters = new FilterCollection();

            //Get the sort collection for account id desc.
            sortCollection = HelperMethods.SortDesc(ZnodeAccountEnum.AccountId.ToString(), sortCollection);
            ZnodeLogging.LogMessage("Input parameters filters,sortCollection:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { filters= filters, sortCollection = sortCollection });
            AccountListModel accountList = _accountClient.GetAccountList(null, filters, sortCollection, pageIndex, recordPerPage);
            AccountListViewModel companyViewModel = new AccountListViewModel { AccountList = accountList?.Accounts?.ToViewModel<AccountViewModel>().ToList() };
            SetListPagingData(companyViewModel, accountList);

            //Set tool menu for account grid list view.
            SetAccountToolMenus(companyViewModel);
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);

            return companyViewModel?.AccountList?.Count > 0 ? companyViewModel
                : new AccountListViewModel { AccountList = new List<AccountViewModel>() };
        }

        //Get the Sub Account List based on the Parent Account Id.
        public virtual AccountListViewModel GetSubAccountList(int parentAccountId, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            if (IsNull(filters))
                filters = new FilterCollection();

            ZnodeLogging.LogMessage("Input parameters filters,sortCollection:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { filters = filters, sortCollection = sortCollection });

            //Sets the parent account id filter.
            if (parentAccountId > 0)
                SetParentAccountIdFilter(filters, parentAccountId);

            AccountListModel accountList = _accountClient.GetAccountList(null, filters, sortCollection, pageIndex, recordPerPage);
            AccountListViewModel companyViewModel = new AccountListViewModel { AccountList = accountList?.Accounts?.ToViewModel<AccountViewModel>().ToList() };
            SetListPagingData(companyViewModel, accountList);

            //Get Parent Account Details, to bind the details.
            AccountDataViewModel accountDetails = GetAccountById(parentAccountId);
            companyViewModel.HasParentAccounts = accountDetails.HasParentAccounts;
            companyViewModel.AccountName = accountDetails?.CompanyAccount?.Name;

            //Set the Tool Menus for Sub Account List on Grid View.
            SetSubAccountToolMenus(companyViewModel);
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);

            return companyViewModel?.AccountList?.Count > 0 ? companyViewModel
                : new AccountListViewModel { AccountList = new List<AccountViewModel>(), AccountName = accountDetails?.CompanyAccount?.Name, HasParentAccounts = accountDetails.HasParentAccounts };
        }

        //Create the New Account.
        public virtual AccountDataViewModel Create(AccountDataViewModel model)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            try
            {
                AccountDataModel accountDetail = model.ToModel<AccountDataModel>();
                if (IsNotNull(accountDetail?.CompanyAccount) && !String.IsNullOrEmpty(DefaultSettingHelper.DefaultLocale))
                    accountDetail.CompanyAccount.LocaleId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale);
                AccountDataModel accountModel = _accountClient.Create(accountDetail);
                if (accountModel?.CompanyAccount?.AccountId > 0)
                    model.CompanyAccount.AccountId = accountModel.CompanyAccount.AccountId;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        return (AccountDataViewModel)GetViewModelWithErrorMessage(model, Admin_Resources.ErrorAccountNameAlreadyExists);
                    default:
                        return (AccountDataViewModel)GetViewModelWithErrorMessage(model, Admin_Resources.ErrorFailedToCreate);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
                return (AccountDataViewModel)GetViewModelWithErrorMessage(model, Admin_Resources.ErrorFailedToCreate);
            }
            return model;
        }

        //Get the Account Details based of the Account Id.
        public virtual AccountDataViewModel GetAccountById(int accountId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            AccountDataViewModel model = new AccountDataViewModel();
            if (accountId > 0)
            {
                AccountModel accountModel = _accountClient.GetAccount(accountId);
                if (IsNotNull(accountModel))
                {
                    model.CompanyAccount = accountModel.ToViewModel<AccountViewModel>();
                    model.HasParentAccounts = model.CompanyAccount?.ParentAccountId <= 0;
                    model.AccountId = accountId;
                    model.CompanyAccount.IsDefault = IsNull(model.CompanyAccount.PublishCatalogId) ? true : false;
                }
            }
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);

            return model;
        }

        //Get all the Parent Account List.
        public virtual List<SelectListItem> GetParentAccountList(int portalId = 0)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            List<SelectListItem> parentAccountsList = new List<SelectListItem>();

            AccountListViewModel accounts = portalId > 0 ? GetAccountList(new FilterCollection() { new FilterTuple(ZnodePortalEnum.PortalId.ToString(), FilterOperators.Equals, portalId.ToString()) })
                                                         : GetAccountList();

            if (accounts?.AccountList?.Count > 0)
            {
                parentAccountsList = (from item in accounts.AccountList
                                      where item.ParentAccountId.Equals(0)
                                      orderby item.Name ascending
                                      select new SelectListItem
                                      {
                                          Text = IsNotNull(item.AccountCode) ? item.Name + " | " + item.AccountCode : item.Name,
                                          Value = item.AccountId.ToString()
                                      }).ToList();
            }
            ZnodeLogging.LogMessage("parentAccountsList count:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { parentAccountsList?.Count });
            parentAccountsList.Insert(0, new SelectListItem() { Text = Admin_Resources.SelectParentAccount, Value = "0" });
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);

            return parentAccountsList;
        }

        //Update the Account.
        public virtual AccountDataViewModel UpdateAccount(AccountDataViewModel model)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            try
            {
                model.AccountId = model.CompanyAccount.ParentAccountId;
                AccountDataModel accountModel = _accountClient.UpdateAccount(model.ToModel<AccountDataModel>());
                return IsNotNull(accountModel) ? accountModel.ToViewModel<AccountDataViewModel>() : (AccountDataViewModel)GetViewModelWithErrorMessage(new AccountDataViewModel(), Admin_Resources.UpdateErrorMessage);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        return (AccountDataViewModel)GetViewModelWithErrorMessage(model, Admin_Resources.ErrorAccountNameAlreadyExists);
                    default:
                        return (AccountDataViewModel)GetViewModelWithErrorMessage(model, Admin_Resources.UpdateErrorMessage);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
                return (AccountDataViewModel)GetViewModelWithErrorMessage(model, Admin_Resources.UpdateErrorMessage);
            }
        }

        //Delete the Account.
        public virtual bool DeleteAccount(string accountIds, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            message = Admin_Resources.ErrorFailedToDelete;
            try
            {
                bool isDeleted = _accountClient.DeleteAccount(new ParameterModel { Ids = accountIds });
                if (!isDeleted)
                    message = Admin_Resources.ErrorDeleteAccount;
                return isDeleted;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
                message = Admin_Resources.ErrorFailedToDelete;
                return false;
            }
        }

        //Set the account data model.
        public virtual void SetAccountDataModel(int parentAccountId, AccountDataViewModel model)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            if (IsNotNull(model))
            {
                model.CompanyAccount.ParentAccountName = model.CompanyAccount.Name;
                model.CompanyAccount.Name = string.Empty;
                model.CompanyAccount.AccountCode = string.Empty;
                model.CompanyAccount.ParentAccountId = parentAccountId;
                model.CompanyAccount.ExternalId = null;
                model.CompanyAccount.IsDefault = true;
                model.CompanyAccount.PublishCatalogId = null;
                model.CompanyAccount.CatalogName = "";

                //Bind Countries List.
                model.CompanyAccount.Address = new AddressViewModel() { Countries = HelperMethods.GetCountries(), IsDefaultBilling = true, IsDefaultShipping = true };
            }
            model = new AccountDataViewModel();
        }

        //Check whether username already exists.
        public virtual bool CheckAccountNameExist(string accountName, int accountId, int portalId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            if (IsNotNull(accountName))
            {
                accountName = accountName.Trim();
                FilterCollection filters = new FilterCollection();
                filters.Add(new FilterTuple(FilterKeys.Name, FilterOperators.Is, accountName));
                if (!DefaultSettingHelper.AllowGlobalLevelUserCreation)
                    filters.Add(new FilterTuple(FilterKeys.PortalId, FilterOperators.Equals, portalId.ToString()));
                ZnodeLogging.LogMessage("filters:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose,new { filters= filters });
                AccountListModel accountList = _accountClient.GetAccountList(null, filters, null, null, null);

                if (accountList?.Accounts?.Count > 0)
                {
                    if (accountId > 0)
                        //Set the status in edit mode.
                        accountList.Accounts.RemoveAll(x => x.AccountId == accountId);

                    return accountList.Accounts.FindIndex(x => string.Equals(x.Name, accountName, StringComparison.CurrentCultureIgnoreCase)) != -1;
                }
            }
            return false;
        }

        #region Account Note
        //Create account note.
        public virtual NoteViewModel CreateAccountNote(NoteViewModel noteViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            try
            {
                if (IsNotNull(noteViewModel))
                {
                    noteViewModel.UserId = Convert.ToInt32(SessionProxyHelper.GetUserDetails()?.UserId);
                    return _accountClient.CreateAccountNote(noteViewModel.ToModel<NoteModel>())?.ToViewModel<NoteViewModel>();
                }
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Warning);
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
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
                return (NoteViewModel)GetViewModelWithErrorMessage(noteViewModel, Admin_Resources.ErrorFailedToCreate);
            }
            return new NoteViewModel { HasError = true, ErrorMessage = Admin_Resources.ErrorFailedToCreate };
        }

        //Get account note list.
        public virtual NoteListViewModel GetAccountNotes(FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            //Get the sort collection for note id desc.
            sorts = HelperMethods.SortDesc(ZnodeNoteEnum.NoteId.ToString(), sorts);
            ZnodeLogging.LogMessage("Input parameters filters,sortCollection:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sorts });

            NoteListModel noteListModel = _accountClient.GetAccountNotes(null, filters, sorts, pageIndex, recordPerPage);
            NoteListViewModel noteListViewModel = new NoteListViewModel { Notes = noteListModel?.Notes?.ToViewModel<NoteViewModel>().ToList() };
            SetListPagingData(noteListViewModel, noteListModel);

            //Set the Tool Menus for Account Notes List on Grid View.
            SetAccountNotesToolMenus(noteListViewModel);
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);

            return noteListViewModel?.Notes?.Count > 0 ? noteListViewModel
                : new NoteListViewModel() { Notes = new List<NoteViewModel>() };
        }

        //Get account note by noteId.
        public virtual NoteViewModel GetAccountNote(int noteId)
            => _accountClient.GetAccountNote(noteId)?.ToViewModel<NoteViewModel>() ?? new NoteViewModel();

        //Update account note.
        public virtual NoteViewModel UpdateAccountNote(NoteViewModel noteViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            try
            {
                return _accountClient.UpdateAccountNote(noteViewModel?.ToModel<NoteModel>())?.ToViewModel<NoteViewModel>();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Warning);
                return (NoteViewModel)GetViewModelWithErrorMessage(noteViewModel, Admin_Resources.UpdateErrorMessage);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
                return (NoteViewModel)GetViewModelWithErrorMessage(noteViewModel, Admin_Resources.UpdateErrorMessage);
            }
        }

        //Delete account note.
        public virtual bool DeleteAccountNote(string noteId, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            message = Admin_Resources.ErrorFailedToDelete;
            try
            {
                return _accountClient.DeleteAccountNote(new ParameterModel { Ids = noteId });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
                message = Admin_Resources.ErrorFailedToDelete;
            }
            return false;
        }
        #endregion

        #region Account Department
        //Create account department.
        public virtual AccountDepartmentViewModel CreateAccountDepartment(AccountDepartmentViewModel departmentViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            try
            {
                return _accountClient.CreateAccountDepartment(departmentViewModel?.ToModel<AccountDepartmentModel>())?.ToViewModel<AccountDepartmentViewModel>();

            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        return (AccountDepartmentViewModel)GetViewModelWithErrorMessage(departmentViewModel, Admin_Resources.ErrorDepartmentAlreadyExists);
                    default:
                        return (AccountDepartmentViewModel)GetViewModelWithErrorMessage(departmentViewModel, Admin_Resources.ErrorFailedToCreate);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
                return (AccountDepartmentViewModel)GetViewModelWithErrorMessage(departmentViewModel, Admin_Resources.ErrorFailedToCreate);
            }
        }

        //Get account department list.
        public virtual AccountDepartmentListViewModel GetAccountDepartments(FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            int accountId;
            //Get the sort collection for department id desc.
            sorts = HelperMethods.SortDesc(ZnodeDepartmentEnum.DepartmentId.ToString(), sorts);
            ZnodeLogging.LogMessage("Input parameters filters,sortCollection:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sorts });

            AccountDepartmentListModel departmentListModel = _accountClient.GetAccountDepartments(null, filters, sorts, pageIndex, recordPerPage);
            AccountDepartmentListViewModel departmentListViewModel = new AccountDepartmentListViewModel { Departments = departmentListModel?.Departments?.ToViewModel<AccountDepartmentViewModel>().ToList() };
            SetListPagingData(departmentListViewModel, departmentListModel);

            //Get the AccountId value from the filter collection.
            int.TryParse(filters?.Find(x => x.FilterName == FilterKeys.AccountId.ToString()).FilterValue, out accountId);

            //Get Parent Account Details, to bind the details.
            AccountDataViewModel accountDetails = GetAccountById(accountId);
            departmentListViewModel.HasParentAccounts = accountDetails.HasParentAccounts;
            departmentListViewModel.AccountName = accountDetails?.CompanyAccount?.Name;
            departmentListViewModel.AccountId = accountDetails.AccountId;

            //Set the Tool Menus for Account Department List on Grid View.
            SetAccountDepartmentToolMenus(departmentListViewModel);
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return departmentListViewModel?.Departments?.Count > 0 ? departmentListViewModel
                : new AccountDepartmentListViewModel() { Departments = new List<AccountDepartmentViewModel>(), HasParentAccounts = accountDetails.HasParentAccounts, AccountName = accountDetails?.CompanyAccount?.Name, AccountId = accountDetails.AccountId };
        }

        //Get account department by department id.
        public virtual AccountDepartmentViewModel GetAccountDepartment(int departmentId, int accountId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            AccountDepartmentModel departmentModel = _accountClient.GetAccountDepartment(departmentId);
            if (IsNotNull(departmentModel))
            {
                AccountDepartmentViewModel viewModel = departmentModel.ToViewModel<AccountDepartmentViewModel>();
                viewModel.AccountIdToRedirect = accountId;
                return viewModel;
            }
            return new AccountDepartmentViewModel();
        }

        //Update account department.
        public virtual bool UpdateAccountDepartment(AccountDepartmentViewModel departmentViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            try
            {
                AccountDepartmentModel accountDepartmentModel = _accountClient.UpdateAccountDepartment(departmentViewModel?.ToModel<AccountDepartmentModel>());
                departmentViewModel.HasError = accountDepartmentModel?.AccountId < 0;
                ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
                return (IsNotNull(departmentViewModel) && !departmentViewModel.HasError);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        departmentViewModel.ErrorMessage = Admin_Resources.ErrorDepartmentAlreadyExists;
                        break;
                    default:
                        departmentViewModel.ErrorMessage = Admin_Resources.UpdateErrorMessage;
                        break;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
                departmentViewModel.ErrorMessage = Admin_Resources.UpdateErrorMessage;
            }
            return false;
        }

        //Delete account department.
        public virtual bool DeleteAccountDepartment(string departmentIds, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            message = Admin_Resources.ErrorFailedToDelete;
            try
            {
                return _accountClient.DeleteAccountDepartment(new ParameterModel { Ids = departmentIds });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Warning);
                if (Equals(ex.ErrorCode, ErrorCodes.AssociationDeleteError))
                    message = Admin_Resources.DepartmentAssociationError;
                else
                    message = Admin_Resources.ErrorFailedToDelete;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
                message = Admin_Resources.ErrorFailedToDelete;
            }
            return false;
        }

        //Set filters for account id.
        public virtual void SetFiltersForAccountId(FilterCollection filters, int? accountId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            if (!Equals(filters, null))
            {
                //Checking For AccountId already Exists in Filters Or Not 
                if (filters.Exists(x => x.Item1 == FilterKeys.AccountId))
                {
                    //If AccountId Already present in filters Remove It
                    filters.RemoveAll(x => x.Item1 == FilterKeys.AccountId);

                    //Add New AccountId Into filters
                    filters.Add(new FilterTuple(FilterKeys.AccountId, FilterOperators.Equals, accountId.ToString()));
                }
                else
                    filters.Add(new FilterTuple(FilterKeys.AccountId, FilterOperators.Equals, accountId.ToString()));
            }
            ZnodeLogging.LogMessage("Input parameters filters:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { filters = filters});

        }

        //Set filters for IsAssociated.
        public virtual void SetFiltersForIsAssociated(FilterCollection filters, string isAssociated)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            if (IsNotNull(filters))
            {
                //Checking For IsAssociated already Exists in Filters Or Not 
                if (filters.Exists(x => x.Item1 == FilterKeys.IsAssociated))
                {
                    //If IsAssociated Already present in filters Remove It
                    filters.RemoveAll(x => x.Item1 == FilterKeys.IsAssociated);

                    //Add New AccountId Into filters
                    filters.Add(new FilterTuple(FilterKeys.IsAssociated, FilterOperators.Equals, isAssociated));
                }
                else
                    filters.Add(new FilterTuple(FilterKeys.IsAssociated, FilterOperators.Equals, isAssociated));
            }
            ZnodeLogging.LogMessage("Input parameters filters:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { filters = filters });

        }
        #endregion

        #region Customer
        //Get customer account list
        public virtual CustomerListViewModel GetCustomerAccountList(string currentUserName, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            //Set the sort collection for user id desc.

            //Get selected column data.
            string columnList = FilterHelpers.GetVisibleColumn();

            HelperMethods.SortUserIdDesc(ref sortCollection);
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { filters = filters, sortCollection= sortCollection });

            UserListModel userList = _userClient.GetCustomerAccountList(currentUserName, filters, sortCollection, pageIndex, recordPerPage, columnList);

            CustomerListViewModel customerListViewModel = new CustomerListViewModel { List = userList?.Users?.ToViewModel<CustomerViewModel>().ToList() };
            SetListPagingData(customerListViewModel, userList);
            int accountId;
            //Get the AccountId value from the filter collection.
            int.TryParse(filters?.Find(x => x.FilterName == FilterKeys.AccountId.ToString()).FilterValue, out accountId);

            //Get Parent Account Details, to bind the details.
            AccountDataViewModel accountDetails = GetAccountById(accountId);
            customerListViewModel.HasParentAccounts = accountDetails.HasParentAccounts;
            customerListViewModel.AccountName = accountDetails?.CompanyAccount?.Name;
            customerListViewModel.AccountId = accountDetails.AccountId;
            customerListViewModel.PortalId = accountDetails.CompanyAccount?.PortalId ?? customerListViewModel.PortalId;
            //Set the Tool Menus for Customer Account List Grid View.
            SetCustomerAccountListToolMenus(customerListViewModel);
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return customerListViewModel?.List?.Count > 0 ? customerListViewModel
                : new CustomerListViewModel() { HasParentAccounts = accountDetails.HasParentAccounts, AccountName = accountDetails?.CompanyAccount?.Name, AccountId = accountDetails.AccountId, PortalId = accountDetails.CompanyAccount?.PortalId ?? 0 };
        }

        //Set customer account view model.
        public virtual void SetCustomerAccountViewModel(CustomerAccountViewModel customerAccountViewModel, int accountId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            //if null then create new instance of the same
            if (IsNull(customerAccountViewModel))
                customerAccountViewModel = new CustomerAccountViewModel();
            //Sets the properties of customerAccountViewModel
            customerAccountViewModel.AccountDepartmentList = GetAccountDepartmentList(accountId);
            customerAccountViewModel.Roles = GetAccountRoleList();
            customerAccountViewModel.UserApprovalList = new UserAgent(GetClient<UserClient>(), GetClient<PortalClient>(), GetClient<AccountClient>(), GetClient<RoleClient>(), GetClient<DomainClient>(), GetClient<StateClient>(), GetClient<GlobalAttributeEntityClient>(), GetClient<ShoppingCartClient>()).GetApproverList(accountId, customerAccountViewModel.UserId, HttpContext.Current.User.Identity.Name);
            customerAccountViewModel.AccountId = accountId;
            AccountDataViewModel accountDataViewModel = GetAccountById(accountId);
            customerAccountViewModel.AccountName = accountDataViewModel?.CompanyAccount?.Name;
            customerAccountViewModel.PortalId = accountDataViewModel?.CompanyAccount?.PortalId;
        }

        //Get account customer by user id.
        public virtual CustomerAccountViewModel GetAccountsCustomer(int userId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            //Sets the filter for UserId and IsAccountCustomer. 
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeUserEnum.UserId.ToString(), FilterOperators.Equals, userId.ToString()));
            HelperMethods.SetIsAccountCustomerFilter(filters, 1);
            ZnodeLogging.LogMessage("Input parameters filters:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { filters = filters });
            //Gets the list.
            UserListModel userList = _userClient.GetCustomerAccountList(HttpContext.Current.User.Identity.Name, filters, null, null, null);
            if (userList?.Users?.Count > 0)
                return userList.Users.First()?.ToViewModel<CustomerAccountViewModel>();

            return new CustomerAccountViewModel { HasError = true };
        }

        //Create customer account.
        public virtual CustomerAccountViewModel CreateCustomerAccount(CustomerAccountViewModel customerViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            try
            {
                UserModel accountModel = _userClient.CreateCustomerAccount(CustomerViewModelMap.ToUserModel(customerViewModel));
                ZnodeLogging.LogMessage("Agent method execution Done.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);

                return IsNotNull(accountModel) ? accountModel.ToViewModel<CustomerAccountViewModel>() : new CustomerAccountViewModel();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        return (CustomerAccountViewModel)GetViewModelWithErrorMessage(customerViewModel, Admin_Resources.LoginNameAlreadyExistErrorMessage);
                    case ErrorCodes.ProfileNotPresent:
                        return (CustomerAccountViewModel)GetViewModelWithErrorMessage(customerViewModel, Admin_Resources.ErrorProfileNotExists);
                    default:
                        return (CustomerAccountViewModel)GetViewModelWithErrorMessage(customerViewModel, Admin_Resources.ErrorFailedToCreate);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
                return (CustomerAccountViewModel)GetViewModelWithErrorMessage(customerViewModel, Admin_Resources.ErrorFailedToCreate);
            }
        }

        //Update customer account data.
        public virtual bool UpdateCustomerAccount(CustomerAccountViewModel model, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            errorMessage = Admin_Resources.UpdateErrorMessage;
            try
            {
                return _userClient.UpdateCustomerAccount(CustomerViewModelMap.ToUserModel(model))?.UserId > 0;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.ProfileNotPresent:
                        errorMessage = Admin_Resources.ErrorProfileNotExistsWhileUpdate;
                        break;
                    default:
                        errorMessage = Admin_Resources.UpdateErrorMessage;
                        break;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
                errorMessage = Admin_Resources.UpdateErrorMessage;
            }
            return false;

        }

        //Get unassociated Customer(s).
        public virtual CustomerListViewModel GetUnAssociatedCustomerList(int portalId, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            UserListModel userList = _userClient.GetUnAssociatedCustomerList(portalId, filters, sortCollection, pageIndex, recordPerPage);
            CustomerListViewModel customerListViewModel = new CustomerListViewModel { List = userList.Users?.ToViewModel<CustomerViewModel>().ToList() };
            SetListPagingData(customerListViewModel, userList);
            return customerListViewModel;
        }

        //Update user account mapping
        public virtual bool UpdateUserAccountMapping(UserAccountViewModel userModel, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            errorMessage = Admin_Resources.UpdateErrorMessage;
            try
            {                
                return _userClient.UpdateUserAccountMapping(userModel.ToModel<UserAccountModel>());
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.InvalidData:
                        errorMessage = ex.ErrorMessage;
                        return false;
                    default:
                        errorMessage = Admin_Resources.UpdateErrorMessage;
                        return false;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                errorMessage = Admin_Resources.UpdateErrorMessage;
            }
            return false;
        }
        #endregion

        #region Address
        //Get address list.
        public virtual AddressListViewModel GetAddressList(int accountId, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            //Get the sort collection for address id desc.
            sortCollection = HelperMethods.SortDesc(ZnodeAddressEnum.AddressId.ToString(), sortCollection);

            //expand for address.
            ExpandCollection expands = new ExpandCollection();
            expands.Add(ZnodeAccountAddressEnum.ZnodeAddress.ToString());
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { filters = filters, sortCollection = sortCollection , expands = expands });

            AddressListModel addressList = _accountClient.GetAddressList(expands, filters, sortCollection, pageIndex, recordPerPage);
            AddressListViewModel addressListViewModel = new AddressListViewModel { AddressList = addressList?.AddressList?.ToViewModel<AddressViewModel>().ToList() };
            SetListPagingData(addressListViewModel, addressList);
            SetAddressToolMenus(addressListViewModel);

            //Get Parent Account Details, to bind the details.
            AccountDataViewModel accountDetails = GetAccountById(accountId);
            addressListViewModel.HasParentAccounts = accountDetails.HasParentAccounts;
            addressListViewModel.AccountName = accountDetails?.CompanyAccount?.Name;
            addressListViewModel.AccountId = accountDetails.AccountId;
            //
            if (addressListViewModel?.AddressList?.Count > 0)
                addressListViewModel.AddressList.Select(c => { c.PortalId = accountDetails.CompanyAccount.PortalId.GetValueOrDefault(); return c; }).ToList();
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return addressListViewModel?.AddressList?.Count > 0 ? addressListViewModel
                : new AddressListViewModel { AddressList = new List<AddressViewModel>(), HasParentAccounts = accountDetails.HasParentAccounts, AccountName = accountDetails?.CompanyAccount?.Name, AccountId = accountDetails.AccountId };
        }

        private void SetAddressToolMenus(AddressListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = GetGridModel();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('DeletePopup')", ControllerName = "Account", ActionName = "DeleteAddress" });
            }
        }

        //Create account address.
        public virtual AddressViewModel CreateAccountAddress(AddressViewModel addressViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            try
            {
                return _accountClient.CreateAccountAddress(addressViewModel?.ToModel<AddressModel>())?.ToViewModel<AddressViewModel>();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
                return (AddressViewModel)GetViewModelWithErrorMessage(addressViewModel, Admin_Resources.ErrorFailedToCreate);
            }
        }

        //Get account address on the basis of account address id.
        public virtual AddressViewModel GetAccountAddress(int accountAddressId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeAccountAddressEnum.AccountAddressId.ToString(), FilterOperators.Equals, accountAddressId.ToString()));

            //expand for address.
            ExpandCollection expands = new ExpandCollection();
            expands.Add(ZnodeAccountAddressEnum.ZnodeAddress.ToString());
            ZnodeLogging.LogMessage("Parameters:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { expands = expands, filters = filters });
            AddressModel addressModel = _accountClient.GetAccountAddress(expands, filters);
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return HelperUtility.IsNotNull(addressModel) ? addressModel.ToViewModel<AddressViewModel>() : new AddressViewModel();
        }

        //Update account address.
        public virtual AddressViewModel UpdateAccountAddress(AddressViewModel addressViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            try
            {
                return _accountClient.UpdateAccountAddress(addressViewModel?.ToModel<AddressModel>())?.ToViewModel<AddressViewModel>();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
                return (AddressViewModel)GetViewModelWithErrorMessage(addressViewModel, Admin_Resources.UpdateErrorMessage);
            }
        }

        //Delete account address.
        public virtual bool DeleteAccountAddress(string accountAddressId, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            message = Admin_Resources.ErrorDeleteAddress;
            try
            {
                return _accountClient.DeleteAccountAddress(new ParameterModel { Ids = accountAddressId });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
                message = Admin_Resources.ErrorDeleteAddress;
                return false;
            }
        }
        #endregion

        #region Associate Price
        //Get Price list.
        public virtual PriceListViewModel GetPriceList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize, int accountId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            //Set filter for account id and set account mode filter.
            SetFiltersForId(filters, accountId, FilterKeys.AccountId);
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sorts, expands = expands });

            PriceListModel priceList = _priceClient.GetPriceList(expands, filters, sorts, pageIndex, pageSize);
            PriceListViewModel listViewModel = new PriceListViewModel { PriceList = priceList?.PriceList?.ToViewModel<PriceViewModel>().ToList() };

            SetListPagingData(listViewModel, priceList);

            SetPriceListToolMenuForAccount(listViewModel);
            if (listViewModel?.PriceList?.Count > 0)
            {
                listViewModel?.PriceList?.ForEach(x => x.AccountId = accountId);
                listViewModel.HasParentAccounts = listViewModel.PriceList.Select(x => x.IsParentAccount).FirstOrDefault();
            }

            //Get Parent Account Details, to bind the details.
            AccountDataViewModel accountDetails = GetAccountById(accountId);
            listViewModel.AccountName = accountDetails?.CompanyAccount?.Name;
            listViewModel.AccountId = accountDetails.AccountId;
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return priceList?.PriceList?.Count > 0 ? listViewModel : new PriceListViewModel() { PriceList = new List<PriceViewModel>(), AccountId = accountId, AccountName = accountDetails?.CompanyAccount?.Name, HasParentAccounts = priceList.HasParentAccounts };
        }

        //Get UnAssociated Price List to Customer.
        public virtual PriceListViewModel GetUnAssociatedPriceList(int accountId, ExpandCollection expands = null, FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            //Set filter for account id and set account mode filter.
            SetFiltersForId(filters, accountId, FilterKeys.AccountId);
            try
            {
                ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sorts, expands = expands });

                PriceListModel priceList = _priceClient.GetUnAssociatedPriceList(expands, filters, sorts, pageIndex, recordPerPage);
                PriceListViewModel priceListViewModel = new PriceListViewModel { PriceList = priceList?.PriceList?.ToViewModel<PriceViewModel>().ToList() };
                SetListPagingData(priceListViewModel, priceList);
                return priceList?.PriceList?.Count > 0 ? priceListViewModel : new PriceListViewModel() { PriceList = new List<PriceViewModel>() };
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
                return (PriceListViewModel)GetViewModelWithErrorMessage(new PriceListViewModel() { PriceList = new List<PriceViewModel>() }, string.Empty);
            }
        }

        //Associate pricelist to account.
        public virtual bool AssociatePriceList(int accountId, string priceListIds)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            try
            {
                return _accountClient.AssociatePriceList(new PriceAccountModel { AccountId = accountId, PriceListIds = priceListIds });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
                return false;
            }
        }

        //Remove associated price list from Account.
        public virtual bool UnAssociatedPriceList(string priceListIds, int accountId)
         => _accountClient.UnAssociatedPriceList(new PriceAccountModel() { PriceListIds = priceListIds, AccountId = accountId });

        //Get associated price list precedence value for Account.
        public virtual PriceAccountViewModel GetAssociatedPriceListPrecedence(int priceListId, int accountId, string listName)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            PriceAccountModel priceAccountModel = _accountClient.GetAssociatedPriceListPrecedence(new PriceAccountModel { PriceListId = priceListId, AccountId = accountId });
            if (IsNotNull(priceAccountModel))
            {
                priceAccountModel.Name = listName;
                return priceAccountModel.ToViewModel<PriceAccountViewModel>();
            }
            return new PriceAccountViewModel() { Name = listName };
        }

        //Update associated price list precedence value for Account.
        public virtual bool UpdateAssociatedPriceListPrecedence(PriceAccountViewModel priceAccountViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            try
            {
                priceAccountViewModel = _accountClient.UpdateAssociatedPriceListPrecedence(priceAccountViewModel?.ToModel<PriceAccountModel>())?.ToViewModel<PriceAccountViewModel>();
                return (IsNotNull(priceAccountViewModel) && !priceAccountViewModel.HasError) ? true : false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
                priceAccountViewModel.ErrorMessage = Admin_Resources.UpdateErrorMessage;
                return false;
            }
        }
        #endregion

        #region Account Order
        //Get user order list of account.
        public virtual OrdersListViewModel GetAccountUserOrderList(int accountId, int userId, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            string updatePageType = userId > 0 ? AdminConstants.OrderHistory : AdminConstants.AccountOrderHistory;
            ZnodeLogging.LogMessage("updatePageType:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info,new { updatePageType = updatePageType });
            if (filters.Exists(x => x.Item1 == ZnodeUserEnum.UserId.ToString()))
                //If UserId Already present in filters Remove It.
                filters.RemoveAll(x => x.Item1 == ZnodeUserEnum.UserId.ToString());

            if (userId > 0)
                SetUserIdFilter(filters, userId);

            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { filters = filters, sortCollection = sortCollection});

            OrdersListModel orderList = _accountClient.GetAccountUserOrderList(accountId, null, filters, sortCollection, pageIndex, recordPerPage);
            if (orderList.Orders?.Count() > 0)
            {
                foreach (var item in orderList.Orders)
                {
                    item.UpdatePageType = updatePageType;
                    item.AccountId = accountId;
                    item.UserId = userId > 0 ? userId : item.UserId;
                    item.OrderTotalWithCurrency = HelperMethods.FormatPriceWithCurrency(item.OrderAmount, item.CultureCode);
                }
                return BindDataToOrderListViewModel(orderList, accountId, userId);
            }

            return new OrdersListViewModel() { AccountName = GetAccountById(accountId)?.CompanyAccount?.Name, HasParentAccounts = (orderList?.HasParentAccounts).GetValueOrDefault() };
        }

        #endregion

        #region Account Profile
        //Get profiles for account.
        public virtual ProfileListViewModel GetAssociatedProfile(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize, int accountId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            //Add new accountId Into filters.
            SetFiltersForAccountId(filters, accountId);
            SetFiltersForIsAssociated(filters, FilterKeys.ActiveTrue);
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sorts, expands = expands });

            ProfileListModel profileList = _accountClient.GetAssociatedUnAssociatedProfile(expands, filters, sorts, pageIndex, pageSize);
            ProfileListViewModel listViewModel = new ProfileListViewModel { List = profileList?.Profiles?.ToViewModel<ProfileViewModel>().ToList(), HasParentAccounts = profileList.HasParentAccounts };

            SetListPagingData(listViewModel, profileList);

            SetProfileToolMenuForAccount(listViewModel);

            //Bind account details by account id.
            AccountDataViewModel accountDetails = GetAccountDetails(accountId, listViewModel);
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);

            return profileList?.Profiles?.Count > 0 ? listViewModel : new ProfileListViewModel() { List = new List<ProfileViewModel>(), AccountId = accountId, AccountName = accountDetails?.CompanyAccount?.Name, HasParentAccounts = profileList.HasParentAccounts };
        }

        //Get UnAssociated profiles for account.
        public virtual ProfileListViewModel GetUnAssociatedProfile(int accountId, ExpandCollection expands = null, FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            //Add New accountId Into filters
            SetFiltersForAccountId(filters, accountId);
            SetFiltersForIsAssociated(filters, FilterKeys.ActiveFalse);
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sorts, expands = expands });

            ProfileListModel profileList = _accountClient.GetAssociatedUnAssociatedProfile(expands, filters, sorts, pageIndex, recordPerPage);
			           
            ProfileListViewModel profileListViewModel = new ProfileListViewModel { List = profileList?.Profiles?.ToViewModel<ProfileViewModel>().ToList() };
            SetListPagingData(profileListViewModel, profileList);
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);

            return profileList?.Profiles?.Count > 0 ? profileListViewModel : new ProfileListViewModel() { List = new List<ProfileViewModel>() };
        }

        //Associate profiles to account.
        public virtual bool AssociateProfile(int accountId, string profileIds)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            try
            {
                return _accountClient.AssociateProfile(new ProfileModel { AccountId = accountId, ProfileIds = profileIds });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
                return false;
            }
        }

        //Remove associated profiles from Account.
        public virtual bool UnAssociateProfile(string accountProfileIds, int accountId, out string message)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            message = string.Empty;
            try
            {
                return _accountClient.UnAssociateProfile(new AccountProfileModel() { AccountProfileIds = accountProfileIds, AccountId = accountId });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.DefaultDataDeletionError:
                        message = ex.ErrorMessage;
                        return false;
                    default:
                        message = Admin_Resources.UnassignError;
                        return false;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
                message = Admin_Resources.UnassignError;
                return false;
            }
        }

        //Set default profile for account.
        public virtual bool SetDefaultProfile(int accountId, int accountProfileId, int profileId, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            errorMessage = Admin_Resources.UpdateErrorMessage;
            try
            {
                if (accountId > 0 && accountProfileId > 0 && profileId > 0)
                    return _accountClient.AssociateProfile(new ProfileModel { AccountId = accountId, ProfileId = profileId, AccountProfileId = accountProfileId, IsDefault = true });

                return false;

            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Warning);
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
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
                return false;
            }
        }
        #endregion

        #region Approval Routing
        //Get approver level list by user id.
        public UserApproverListViewModel GetApproverLevelList(int userId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeOmsTemplateEnum.UserId.ToString(), FilterOperators.Equals, userId.ToString()));
            filters.Add(new FilterTuple(ZnodeConstant.ShowAllApprovers.ToString().ToLower(), FilterOperators.Equals, true.ToString()));
            filters.Add(new FilterTuple(ZnodeConstant.User.ToString(), FilterOperators.Equals, true.ToString()));
            ZnodeLogging.LogMessage("filters:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { filters = filters });
            UserApproverListModel listModel = _accountClient.GetApproverLevelList(null, filters, null, null, null);
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);

            return new UserApproverListViewModel { UserApprover = listModel?.UserApprovers?.ToViewModel<UserApproverViewModel>()?.ToList(), AccountPermissionAccessId = listModel.AccountPermissionAccessId, AccountUserPermissionId = listModel.AccountUserPermissionId, AccountId = listModel.AccountId, PortalId = listModel.PortalId };
        }

        //Create approver level.
        public virtual bool CreateApproverLevel(UserApproverViewModel model, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            errorMessage = string.Empty;
            try
            {
                return _accountClient.CreateApproverLevel(model.ToModel<UserApproverModel>());
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);

                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        errorMessage = Admin_Resources.AlreadyAssignedApprover;
                        break;
                    default:
                        errorMessage = model.UserApproverId.Equals(0) ? Admin_Resources.ErrorFailedToCreate : Admin_Resources.UpdateErrorMessage;
                        break;
                }

                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
                return false;
            }
        }

        //Save permission setting.
        public PermissionCodeViewModel SavePermissionSetting(PermissionCodeViewModel model)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            try
            {
                PermissionCodeModel userApproveModel = _accountClient.SavePermissionSetting(model.ToModel<PermissionCodeModel>());
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);

                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AtLeastSelectOne:
                        return (PermissionCodeViewModel)GetViewModelWithErrorMessage(model, Admin_Resources.AssociateAtLeastOneApprover);
                    default:
                        return (PermissionCodeViewModel)GetViewModelWithErrorMessage(model, Admin_Resources.ErrorFailedToUpdate);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
            }
            return model;
        }

        //Get level list.
        public List<SelectListItem> GetLevelsList(ApproverLevelListModel approverLevelListModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            if (approverLevelListModel?.ApproverLevelList?.Count > 0)
                return approverLevelListModel.ApproverLevelList.Select(item => new SelectListItem() { Text = item?.LevelName, Value = item?.ApproverLevelId.ToString() }).ToList();

            return new List<SelectListItem>();
        }

        //Get approver order list.
        public List<SelectListItem> GetApproverOrder(ApproverLevelListModel approverLevelListModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            List<SelectListItem> orders = new List<SelectListItem>();
            var approverOrder = new SelectListItem()
            {
                Value = Convert.ToString(0),
                Text = AdminConstants.Select
            };
            orders.Insert(0, approverOrder);
            if (approverLevelListModel?.ApproverLevelList?.Count > 0)
                orders.AddRange(Enumerable.Range(1, Convert.ToInt32(approverLevelListModel?.ApproverLevelList?.Count)).Select(x => new SelectListItem { Text = Convert.ToString(x), Value = Convert.ToString(x) }));

            ZnodeLogging.LogMessage("orders list count:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, orders?.Count());
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);

            return orders;
        }

        //Delete approver level by id.
        public bool DeleteApproverLevelById(string approverLevelId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            try
            {
                return _accountClient.DeleteApproverLevel(new ParameterModel { Ids = approverLevelId });
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

        //Get level list 
        public virtual UserApproverViewModel GetLevelList()
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            ApproverLevelListModel approverLevelListModel = _accountClient.GetLevelsList(null, null, null, null);
            UserApproverViewModel model = new UserApproverViewModel();
            model.IsAddMode = true;
            model.Levels = GetLevelsList(approverLevelListModel);
            model.ApproverOrders = GetApproverOrder(approverLevelListModel);
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);

            return model;
        }

        //Get approver users by username
        public virtual List<UserApproverViewModel> GetApproverUsersByName(string searchTerm, int? portalId, int? accountId, int? userId, string approvalUserIds = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            int isAccountCustomer = 1;
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(AspNetUserClaimEnum.UserId.ToString(), FilterOperators.NotIn, userId.ToString()));
            filters.Add(new FilterTuple(AspNetUserEnum.UserName.ToString(), FilterOperators.StartsWith, searchTerm));
            filters.Add(new FilterTuple(ZnodePortalAccountEnum.PortalId.ToString(), FilterOperators.Equals, portalId.ToString()));
            ZnodeLogging.LogMessage("parameters:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { filters = filters });
            if (!accountId.Equals(0))
            {
                filters.Add(new FilterTuple(ZnodePortalAccountEnum.AccountId.ToString(), FilterOperators.Equals, accountId.ToString()));
                filters.Add(new FilterTuple(View_CustomerUserDetailEnum.IsAccountCustomer.ToString(), FilterOperators.Equals, isAccountCustomer.ToString()));
            }

            string currentUserName = HttpContext.Current.User.Identity.Name;

            int loggedUserAccountId = SessionProxyHelper.GetUserDetails().UserId;

            UserListModel userList = _userClient.GetUserAccountList(loggedUserAccountId, filters, null, null, null);

            if (!string.IsNullOrEmpty(approvalUserIds))
            {
                int[] approvalUsersIdArray = Array.ConvertAll(approvalUserIds.Split(','), int.Parse);
                for (int index = 0; index < approvalUsersIdArray.Count(); index++)
                {
                    int? approvalUserId = approvalUsersIdArray[index];
                    userList?.Users?.Remove(userList.Users.Where(x => x.UserId == approvalUserId).FirstOrDefault());
                }
            }
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return userList?.Users?.Count > 0 ? userList.Users.ToViewModel<UserApproverViewModel>().ToList() : new List<UserApproverViewModel>();
        }
        #endregion


        //Get parent account List
        public virtual ParentAccountListViewModel GetParentAccountList(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            ParentAccountListModel parentAccountList = _accountClient.GetParentAccountList(null, filters, sortCollection, pageIndex, recordPerPage);

            if (parentAccountList?.ParentAccount?.Count() > 0)
            {
                ParentAccountListViewModel parentAccountListViewModel = new ParentAccountListViewModel() { ParentAccountList = parentAccountList?.ParentAccount?.ToViewModel<ParentAccountViewModel>()?.ToList() };
                SetListPagingData(parentAccountListViewModel, parentAccountList);
                return parentAccountListViewModel;
            }

            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            return new ParentAccountListViewModel();
        }

        //To set the portal id in filter
        public virtual void SetPortalIdFilter(FilterCollection filters, int portalId)
        {
            if (portalId == 0)
            {
                portalId = SessionHelper.GetDataFromSession<int>("portalId");
            }

            if (!Equals(filters, null) && portalId > 0)
            {
                //Check for portalId filter already exists in filters list or not 
                if (filters.Exists(x => x.Item1.ToLower() == ZnodePortalEnum.PortalId.ToString().ToLower()))
                {
                    //Remove portalId filter if already present in filters list
                    filters.RemoveAll(x => x.Item1.ToLower() == ZnodePortalEnum.PortalId.ToString().ToLower());

                    SessionHelper.SaveDataInSession<int>("portalId", portalId);

                    //Add new portalId into filters
                    filters.Add(new FilterTuple(ZnodePortalEnum.PortalId.ToString().ToLower(), FilterOperators.Equals, portalId.ToString()));

                }
                else
                {
                    SessionHelper.SaveDataInSession<int>("portalId", portalId);
                    filters.Add(new FilterTuple(ZnodePortalEnum.PortalId.ToString().ToLower(), FilterOperators.Equals, portalId.ToString()));
                }
            }
        }

        #endregion

        #region Private Methods
        //Get List of Active Countries.
        public virtual List<SelectListItem> GetAccountDepartmentList(int? accountId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            List<SelectListItem> departmentSelectList = new List<SelectListItem>();
            FilterCollection filter = new FilterCollection();
            SetFiltersForAccountId(filter, accountId);
            AccountDepartmentListViewModel departments = GetAccountDepartments(filter, null, null, null);

            if (departments?.Departments?.Count > 0)
            {
                foreach (AccountDepartmentViewModel department in departments.Departments)
                    departmentSelectList.Add(new SelectListItem() { Text = department.DepartmentName, Value = department.DepartmentId.ToString() });
            }
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return departmentSelectList;
        }

        //Gets the list of b2b roles.
        public virtual List<SelectListItem> GetAccountRoleList()
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            SortCollection sorts = new SortCollection();
            sorts.Add(AspNetRoleEnum.Name.ToString(), DynamicGridConstants.ASCKey);
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { sorts = sorts });
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return _roleAgent.RoleList(new FilterCollection() { new FilterTuple(AspNetRoleEnum.TypeOfRole.ToString(), FilterOperators.Is, AdminConstants.B2B) }, sorts);
        }

        //Get account details by account id.
        private AccountDataViewModel GetAccountDetails(int accountId, ProfileListViewModel listViewModel)
        {
            //Get Parent Account Details, to bind the details.
            AccountDataViewModel accountDetails = GetAccountById(accountId);
            listViewModel.AccountName = accountDetails?.CompanyAccount?.Name;
            listViewModel.AccountId = accountDetails.AccountId;
            return accountDetails;
        }

        //Get the html for drop down of account permission list.
        public virtual string AccountPermissionList(int? accountId, int? accountPermissionId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeAccountEnum.AccountId.ToString(), FilterOperators.Equals, accountId.ToString()));
            ZnodeLogging.LogMessage("parameters:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { filters = filters });
            AccessPermissionListModel accountPermissions = _accessPermissionClient.AccountPermissionList(null, filters, null, null, null);
            string permissionsOption = "<select name='AccountPermissionAccessId' id='ddlPermission'>";
            //Binds the option in select list.
            if (accountPermissions?.AccountPermissions?.Count > 0)
                foreach (AccessPermissionModel access in accountPermissions.AccountPermissions)
                {
                    string selected = access.AccountPermissionAccessId == accountPermissionId ? " selected='selected'" : " ";
                    permissionsOption += "<option data-permissioncode=" + access.PermissionCode + " value=" + access.AccountPermissionAccessId + selected + ">" + access.PermissionsName + "</option>";
                }
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
            return permissionsOption += "</select>";
        }

        //Set the Tool Menus for Account List Grid View.
        private void SetAccountToolMenus(AccountListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = GetGridModel();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('DeletePopup')", ControllerName = "Account", ActionName = "Delete" });
            }
        }

        //Set the Tool Menus for Sub Account List Grid View.
        private void SetAccountNotesToolMenus(NoteListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = GetGridModel();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('NoteDeletePopup')", ControllerName = "Account", ActionName = "DeleteAccountNote" });
            }
        }

        //Set the Tool Menus for Account Notes List Grid View.
        private void SetSubAccountToolMenus(AccountListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = GetGridModel();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('DeletePopup')", ControllerName = "Account", ActionName = "Delete" });
            }
        }

        //Set the Tool Menus for Account Department List Grid View.
        private void SetAccountDepartmentToolMenus(AccountDepartmentListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = GetGridModel();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('DepartmentDeletePopup')", ControllerName = "Account", ActionName = "DeleteAccountDepartment" });
            }
        }

        //Set the Tool Menus for Customer Account List Grid View.
        private void SetCustomerAccountListToolMenus(CustomerListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = GetGridModel();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.Unlock, JSFunctionName = "EditableText.prototype.DialogDelete('accountEnable')", ControllerName = "Account", ActionName = "CustomerEnableDisableAccount" });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.Lock, JSFunctionName = "EditableText.prototype.DialogDelete('accountdisable')", ControllerName = "Account", ActionName = "CustomerEnableDisableAccount" });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ResetPasswordButtonText, JSFunctionName = "EditableText.prototype.DialogDelete('accountresetpassword')", ControllerName = "Account", ActionName = "BulkResetPassword" });
            }
        }

        private void SetFiltersForId(FilterCollection filters, int id, string filterKey)
        {
            if (IsNotNull(filters))
            {
                //If account id is already present in filters, remove it.
                filters.RemoveAll(x => x.Item1 == FilterKeys.AccountId);

                //Add new id into filters.
                filters.Add(new FilterTuple(filterKey, FilterOperators.Equals, id.ToString()));

                //If Mode is already present in filters, remove it.
                filters.RemoveAll(x => x.Item1 == FilterKeys.Mode);

                //Add New Mode into filters.
                filters.Add(new FilterTuple(FilterKeys.Mode, FilterOperators.Equals, ZnodeConstant.Account));
                ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { filters = filters });
            }
        }

        //Set the Tool Menus for Price List Grid View.
        private void SetPriceListToolMenuForAccount(PriceListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = GetGridModel();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('PriceListDeletePopup')", ControllerName = "Account", ActionName = "UnAssociatePriceList" });
            }
        }

        //Set the Tool Menus for profile Grid View.
        private void SetProfileToolMenuForAccount(ProfileListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = GetGridModel();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.IsDefault, JSFunctionName = "Account.prototype.SetDefaultProfile()", ControllerName = "Account", ActionName = "SetDefaultProfile" });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('DeleteAssociatedProfiles')", ControllerName = "Account", ActionName = "UnAssociateProfileToAccount" });
            }
        }

        //Sets the parent account id filter.
        private void SetParentAccountIdFilter(FilterCollection filters, int parentAccountId)
        {
            if (IsNotNull(filters))
            {
                //Checking For ParentAccountId already Exists in Filters Or Not 
                if (filters.Exists(x => x.Item1.Equals(ZnodeAccountEnum.ParentAccountId.ToString(), StringComparison.InvariantCultureIgnoreCase)))
                {
                    //If ParentAccountId Already present in filters Remove It
                    filters.RemoveAll(x => x.Item1.Equals(ZnodeAccountEnum.ParentAccountId.ToString(), StringComparison.InvariantCultureIgnoreCase));

                    //Add New ParentAccountId Into filters
                    filters.Add(new FilterTuple(ZnodeAccountEnum.ParentAccountId.ToString(), FilterOperators.Equals, parentAccountId.ToString()));
                }
                else
                    filters.Add(new FilterTuple(ZnodeAccountEnum.ParentAccountId.ToString(), FilterOperators.Equals, parentAccountId.ToString()));

                ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { filters = filters });
            }
        }

        //Set filter for User Id.
        private static void SetUserIdFilter(FilterCollection filters, int userId)
        {
            //Checking For UserId already Exists in Filters Or Not 
            if (filters.Exists(x => x.Item1 == ZnodeUserEnum.UserId.ToString()))
                //If UserId Already present in filters Remove It.
                filters.RemoveAll(x => x.Item1 == ZnodeUserEnum.UserId.ToString());

            //Add New UserId Into filters
            filters.Add(new FilterTuple(ZnodeUserEnum.UserId.ToString(), FilterOperators.Equals, userId.ToString()));
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Verbose, new { filters = filters });
            return;
        }

        //Bind required data to OrderListViewModel.
        private OrdersListViewModel BindDataToOrderListViewModel(OrdersListModel orderList, int accountId, int userId)
        {
            OrdersListViewModel ordersListViewModel = new OrdersListViewModel() { List = orderList.Orders?.ToViewModel<OrderViewModel>()?.ToList() };

            //Get Parent Account Details, to bind the details.
            ordersListViewModel.AccountName = GetAccountById(accountId)?.CompanyAccount?.Name;
            ordersListViewModel.AccountId = accountId;
            ordersListViewModel.UserId = userId;
            ordersListViewModel.HasParentAccounts = orderList.HasParentAccounts;
            SetListPagingData(ordersListViewModel, orderList);
            return ordersListViewModel;
        }      
        #endregion
    }
}