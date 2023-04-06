using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;

using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Maps;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;

using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Admin.Agents
{
    public class UserAgent : BaseAgent, IUserAgent
    {
        #region Private Variable
        private readonly IUserClient _userClient;
        private readonly IPortalClient _portalClient;
        private readonly IAccountClient _accountClient;
        private readonly IRoleClient _roleClient;
        private readonly IDomainClient _domainClient;
        private readonly IStateClient _stateClient;
        private readonly IGlobalAttributeEntityClient _globalAttributeEntityClient;
        private readonly IShoppingCartClient _shoppingCartClient;
        #endregion

        #region Public Constructor
        public UserAgent(IUserClient userClient, IPortalClient portalClient, IAccountClient accountClient, IRoleClient roleClient, IDomainClient domainClient, IStateClient stateClient, IGlobalAttributeEntityClient globalAttributeEntityClient, IShoppingCartClient shoppingCartClient)
        {
            _userClient = GetClient<IUserClient>(userClient);
            _portalClient = GetClient<IPortalClient>(portalClient);
            _accountClient = GetClient<IAccountClient>(accountClient);
            _roleClient = GetClient<IRoleClient>(roleClient);
            _domainClient = GetClient<IDomainClient>(domainClient);
            _stateClient = GetClient<IStateClient>(stateClient);
            _globalAttributeEntityClient = GetClient<IGlobalAttributeEntityClient>(globalAttributeEntityClient);
            _shoppingCartClient = GetClient<IShoppingCartClient>(shoppingCartClient);
        }
        #endregion

        #region Public Methods
        // This method is used to login the user.
        public virtual LoginViewModel Login(LoginViewModel model)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            try
            {
                UserModel accountModel = _userClient.Login(UserViewModelMap.ToLoginModel(model), null);
                //Reset password
                if (!string.IsNullOrEmpty(accountModel?.User?.PasswordToken))
                {
                    LoginViewModel loginViewModel = UserViewModelMap.ToLoginViewModel(accountModel);
                    loginViewModel.IsResetPassword = true;
                    loginViewModel.HasError = true;
                    loginViewModel.ErrorMessage = Admin_Resources.InvalidUserNamePassword;
                    return loginViewModel;
                }

                if (!accountModel.IsAdminUser)
                    return new LoginViewModel() { HasError = true, ErrorMessage = Admin_Resources.ErrorAccessDenied };

                SaveInSession(AdminConstants.UserAccountSessionKey, accountModel.ToViewModel<UserViewModel>());
                ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

                return UserViewModelMap.ToLoginViewModel(accountModel);
            }
            catch (ZnodeUnauthorizedException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case 1://Error Code For Reset Super Admin Details for the  first time login.
                        return ReturnErrorViewModel(model, ex, true);
                    case 2://Error Code to Reset the Password for the first time login.
                        return ReturnErrorViewModel(model, ex, false);
                    case ErrorCodes.AccountLocked:
                        return new LoginViewModel() { HasError = true, ErrorMessage = Admin_Resources.ErrorAccountLocked };
                    case ErrorCodes.TwoAttemptsToAccountLocked:
                        return new LoginViewModel() { HasError = true, ErrorMessage = Admin_Resources.ErrorTwoAttemptsRemain };
                    case ErrorCodes.OneAttemptToAccountLocked:
                        return new LoginViewModel() { HasError = true, ErrorMessage = Admin_Resources.ErrorOneAttemptRemain };
                    default:
                        return new LoginViewModel() { HasError = true, ErrorMessage = Admin_Resources.InvalidUserNamePassword };
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return new LoginViewModel() { HasError = true, ErrorMessage = Admin_Resources.InvalidUserNamePassword };
            }
        }

        // This method is used to logout the user.
        public virtual void Logout()
        {
            FormsAuthentication.SignOut();
            SessionHelper.Abandon();
        }

        // Function used to Change/Reset the user password.
        public virtual ChangePasswordViewModel ChangePassword(ChangePasswordViewModel model)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            try
            {
                _userClient.ChangePassword(UserViewModelMap.ToChangePasswordModel(model));
                ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

                return new ChangePasswordViewModel { SuccessMessage = Admin_Resources.SuccessPasswordChanged };
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                return new ChangePasswordViewModel { HasError = true, ErrorMessage = (string.IsNullOrEmpty(ex.ErrorMessage)) ? Admin_Resources.ErrorChangePassword : ex.ErrorMessage, };
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return (ChangePasswordViewModel)GetViewModelWithErrorMessage(model, Admin_Resources.Error);
            }
        }

        // Function used for forgot password.
        public virtual UserViewModel ForgotPassword(UserViewModel model)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            if (IsNotNull(model))
            {
                try
                {
                    if (IsNull(model.UserName))
                        model.UserName = model.EmailAddress;
                    UserModel userModel = _userClient.GetAccountByUser(model.UserName);
                    if (IsNotNull(userModel))
                    {
                        //Check whether the user is admin user or not.
                        if (!userModel.IsAdminUser)
                            return SetErrorProperties(model, Admin_Resources.ErrorAccessDenied);

                        //check if user model is null or not.
                        if (!model.HasError)
                        {
                            model.BaseUrl = GetDomainUrl();
                            model.EmailAddress = userModel.Email;
                            model = ResetPassword(model);
                                return model;
                        }
                    }
                    else
                        return SetErrorProperties(model, Admin_Resources.InvalidAccountInformation);
                }
                catch (ZnodeException ex)
                {
                    ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                    return SetErrorProperties(model, Admin_Resources.InvalidUsername);
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                    return SetErrorProperties(model, Admin_Resources.InvalidAccountInformation);
                }
            }
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            return model;
        }

        // This function will verify the Reset Password Link current status.
        public virtual ResetPasswordStatusTypes VerifyResetPasswordLinkStatus(ChangePasswordViewModel model)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            try
            {
                if (IsNotNull(model))
                {
                    int? resetPasswordStatus = _userClient.VerifyResetPasswordLinkStatus(UserViewModelMap.ToChangePasswordModel(model));
                    ZnodeLogging.LogMessage("Parameters", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { resetPasswordStatus = resetPasswordStatus });
                    switch (resetPasswordStatus)
                    {
                        case ErrorCodes.ResetPasswordContinue:
                            return ResetPasswordStatusTypes.Continue;
                        case ErrorCodes.ResetPasswordLinkExpired:
                            return ResetPasswordStatusTypes.LinkExpired;
                        case ErrorCodes.ResetPasswordNoRecord:
                            return ResetPasswordStatusTypes.NoRecord;
                        case ErrorCodes.ResetPasswordTokenMismatch:
                            return ResetPasswordStatusTypes.TokenMismatch;
                        default:
                            return ResetPasswordStatusTypes.NoRecord;
                    }
                }
                return ResetPasswordStatusTypes.NoRecord;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                return ResetPasswordStatusTypes.NoRecord;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return ResetPasswordStatusTypes.NoRecord;
            }
        }

        // To Check whether the User details are present in Session. Return true or false.
        public virtual bool CheckUserKey() => (IsNotNull(GetFromSession<UserModel>(AdminConstants.UserAccountSessionKey)));

        // Get portal name based on portal id.
        public virtual string GetStoreName(int portalId) => _portalClient.GetPortal(portalId, null)?.StoreName;

        // Get available portal list.
        public virtual List<SelectListItem> GetPortals()
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            List<SelectListItem> listItems = new List<SelectListItem>();

            //Get all available portal list.
            PortalListModel portalList = _portalClient.GetPortalList(null, null, null, null, null);

            if (portalList?.PortalList?.Count > 0)
            {
                listItems = (from item in portalList.PortalList
                             orderby item.StoreName ascending
                             select new SelectListItem
                             {
                                 Text = item.StoreName,
                                 Value = item.PortalId.ToString()
                             }).ToList();
            }
            ZnodeLogging.LogMessage("listItems list count:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, listItems?.Count());
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            return listItems;
        }

        public virtual bool IsShowChangePasswordPopup()
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            string value = "";
            bool status = false;
            if ((HttpContext.Current.Request.Browser.Cookies))
            {
                if (CookieHelper.IsCookieExists(AdminConstants.ChangePasswordPopup))
                {
                    value = HttpUtility.HtmlEncode(CookieHelper.GetCookieValue<string>(AdminConstants.ChangePasswordPopup));
                }
            }

            if (string.IsNullOrEmpty(value))
            {
                if (!_userClient.IsDefaultAdminPasswordReset())
                { status = true; }
            }
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return status ? true : false;
        }

        public virtual bool SaveInCookie()
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            if ((HttpContext.Current.Request.Browser.Cookies))
            {
                CookieHelper.SetCookie(AdminConstants.ChangePasswordPopup, "true", (Convert.ToDouble(ZnodeAdminSettings.CookieExpiresValue) * ZnodeConstant.MinutesInADay), true);
                return true;
            }
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            return false;
        }

        #endregion

        #region Customers
        // Create customer account.
        public virtual CustomerViewModel CreateCustomerAccount(CustomerViewModel customerViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            try
            {
                if (IsNotNull(customerViewModel))
                {
                    UserModel accountModel = _userClient.CreateCustomerAccount(CustomerViewModelMap.ToAccountFromCustomerViewModel(customerViewModel));
                    return IsNotNull(accountModel) ? accountModel.ToViewModel<CustomerViewModel>() : new CustomerViewModel();
                }
                ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

                return (CustomerViewModel)GetViewModelWithErrorMessage(new CustomerViewModel(), Admin_Resources.ErrorFailedToCreate);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        return (CustomerViewModel)GetViewModelWithErrorMessage(customerViewModel, Admin_Resources.ErrorUserNameAlreadyExists);
                    case ErrorCodes.ProfileNotPresent:
                        return (CustomerViewModel)GetViewModelWithErrorMessage(customerViewModel, Admin_Resources.ErrorProfileNotExists);
                    default:
                        return (CustomerViewModel)GetViewModelWithErrorMessage(customerViewModel, Admin_Resources.ErrorFailedToCreate);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return (CustomerViewModel)GetViewModelWithErrorMessage(customerViewModel, Admin_Resources.ErrorFailedToCreate);
            }
        }

        //Get customer account by customer account id.
        public virtual CustomerViewModel GetCustomerAccountById(int accountId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            //Sets the filter for UserId and IsAccountCustomer. 
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeUserEnum.UserId.ToString(), FilterOperators.Equals, accountId.ToString()));
            filters.Add(new FilterTuple(FilterKeys.CustomerEditMode, FilterOperators.Equals, accountId.ToString()));
            ZnodeLogging.LogMessage("Parameters:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { filters = filters });

            //Gets the list.
            UserListModel userList = _userClient.GetCustomerAccountList(HttpContext.Current.User.Identity.Name, filters, null, null, null);

            if (userList?.Users?.Count > 0)
                return userList.Users.First()?.ToViewModel<CustomerViewModel>();

            return new CustomerViewModel { HasError = true };
        }

        //Update customer account data.
        public virtual bool UpdateCustomerAccount(CustomerViewModel customerViewModel, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            errorMessage = Admin_Resources.UpdateErrorMessage;
            try
            {
                ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

                return _userClient.UpdateCustomerAccount(CustomerViewModelMap.ToAdminUpdateAccountModel(customerViewModel))?.UserId > 0;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        errorMessage = Admin_Resources.ErrorUserNameAlreadyExists;
                        break;
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
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                errorMessage = Admin_Resources.UpdateErrorMessage;
            }
            return false;

        }

        //AccountIds which are to be deleted.
        //Check Current Logged in user name - It cannot be deleted.
        public virtual bool DeleteCustomerAccount(string accountIds, string currentUserName, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            errorMessage = string.Empty;

            if (!string.IsNullOrEmpty(accountIds))
            {
                try
                {
                    //Get account id of current logged in user. 
                    int loggedUserAccountId = SessionProxyHelper.GetUserDetails().UserId;
                    ZnodeLogging.LogMessage("Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info,new { loggedUserAccountId = loggedUserAccountId });

                    string[] arrayIds = accountIds.Split(',');

                    //Check if array of Ids contain the Id of logged in user as it cannot be deleted.
                    if (arrayIds.Count(r => r == loggedUserAccountId.ToString()) > 0)
                    {
                        //If selected account Id  is the only logged in user Id, then return with an error message.
                        if (Equals(accountIds, loggedUserAccountId.ToString()))
                        {
                            errorMessage = Admin_Resources.FailedDeleteLoginUser;
                            return false;
                        }
                        else
                        {
                            //Remove logged in user Id from array of Ids and then delete those.
                            arrayIds = arrayIds.Where(val => val != loggedUserAccountId.ToString()).ToArray();

                            string accountIdsToDelete = string.Join(",", arrayIds);
                            ZnodeLogging.LogMessage("Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info, new { accountIdsToDelete = accountIdsToDelete });

                            if (_userClient.DeleteUser(new ParameterModel { Ids = accountIdsToDelete }))
                            {
                                errorMessage = Admin_Resources.SuccessDeleteMessageExceptLoginUser;
                                return true;
                            }
                            else
                            {
                                errorMessage = Admin_Resources.ErrorMessageHalfDelete;
                                return false;
                            }
                        }
                    }
                    else
                        return _userClient.DeleteUser(new ParameterModel { Ids = accountIds });
                }
                catch (ZnodeException ex)
                {
                    ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                    errorMessage = ex.ErrorCode == ErrorCodes.AssociationDeleteError ? Admin_Resources.ErrorDeleteAccount : Admin_Resources.ErrorMessageHalfDelete;
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                    errorMessage = Admin_Resources.ErrorMessageHalfDelete;
                }
            }
            return false;
        }

        //Get customer account list
        public virtual CustomerListViewModel GetCustomerAccountList(string currentUserName, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null, int portalId = 0, string portalName = null)
        {
            UserViewModel userViewModel = SessionProxyHelper.GetUserDetails();
            int loggedUserAccountId = userViewModel.UserId;
            string roleName = userViewModel?.RoleName;

            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            //Set the sort collection for user id desc.
            HelperMethods.SortUserIdDesc(ref sortCollection);

            //Get selected column data.
            string columnList = FilterHelpers.GetVisibleColumn();

            FilterCollection _filters = new FilterCollection();
            _filters.AddRange(filters);

            //Add portal id in filter collection.
            AddPortalIdInFilters(_filters, portalId);

            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { filters = _filters, sorts = sortCollection });

            AdminUserModel userAdminModel = AdminUserModelMapper(loggedUserAccountId, roleName, columnList, currentUserName);

            UserListModel userList = _userClient.GetCustomersForAdmin(userAdminModel, _filters, sortCollection, pageIndex, recordPerPage);
            CustomerListViewModel customerListViewModel = new CustomerListViewModel { List = userList?.Users?.ToViewModel<CustomerViewModel>().ToList() };

            BindStoreFilterValues(customerListViewModel, portalId, portalName);

            SetListPagingData(customerListViewModel, userList);
            customerListViewModel?.List?.ForEach(x => x.IsGuestUser = string.IsNullOrEmpty(x.AspNetUserId) ? true : false);
            //Set the Tool Menus for Customer Account List Grid View.

            if (!filters.Any(x => x.FilterName.Equals(FilterKeys.IsGuestUser, StringComparison.InvariantCultureIgnoreCase)))
                SetCustomerAccountToolMenuList(customerListViewModel);

            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            return customerListViewModel?.List?.Count > 0 ? customerListViewModel
                : new CustomerListViewModel() { PortalName = customerListViewModel.PortalName, PortalId = customerListViewModel.PortalId };
        }

        //Get the List of Sale Rep
        public virtual UsersListViewModel GetSalesRepListForAssociation(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null)
        {
            HelperMethods.SalesRepSort(ref sortCollection);

            UserListModel userListModel = _userClient.GetSalesRepListForAssociation(filters, sortCollection, pageIndex, recordPerPage);

            UsersListViewModel listViewModel = new UsersListViewModel { List = userListModel?.Users?.ToViewModel<UsersViewModel>().ToList() };

            SetListPagingData(listViewModel, userListModel);

            return listViewModel?.List?.Count > 0 ? listViewModel : new UsersListViewModel() { List = new List<UsersViewModel>() };
        }

        //Enable disable customer account.
        public virtual bool EnableDisableUser(string accountId, bool lockUser, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            errorMessage = string.Empty;
            if (!string.IsNullOrEmpty(accountId))
            {
                try
                {
                    return _userClient.EnableDisableAccount(new ParameterModel { Ids = accountId }, lockUser);
                }
                catch (ZnodeException ex)
                {
                    ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                    switch (ex.ErrorCode)
                    {
                        case ErrorCodes.LockOutEnabled:
                            errorMessage = Admin_Resources.LockOutDisabledErrorMessage;
                            break;
                        default:
                            errorMessage = Admin_Resources.ErrorLockUnlockRecords;
                            break;
                    }
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                    errorMessage = Admin_Resources.ErrorLockUnlockRecords;
                }
            }
            return false;
        }
        // Get available account list.
        public virtual List<SelectListItem> GetAccounts()
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            List<SelectListItem> listItems = new List<SelectListItem>();

            //Get all available portal list.
            AccountListModel accountList = _accountClient.GetAccountList(null, null, null, null, null);

            if (accountList?.Accounts?.Count > 0)
            {
                listItems = (from item in accountList.Accounts
                             select new SelectListItem
                             {
                                 Text = item.Name,
                                 Value = item.AccountId.ToString()
                             }).ToList();
            }
            ZnodeLogging.LogMessage("listItems list count:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, listItems?.Count());
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            return listItems;
        }

        // Get available account list.
        public virtual AccountListViewModel GetAccountList(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null, string accountCode = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            AccountListModel accountList = _accountClient.GetAccountList(null, filters, sortCollection, pageIndex, recordPerPage);
            accountList.Accounts = accountList?.Accounts?.Where(x => x.AccountCode != accountCode)?.ToList();
            if (accountList?.Accounts?.Count() > 0)
            {
                AccountListViewModel accountListViewModel = new AccountListViewModel() { AccountList = accountList?.Accounts?.ToViewModel<AccountViewModel>()?.ToList() };
                SetListPagingData(accountListViewModel, accountList);
                return accountListViewModel;
            }
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            return new AccountListViewModel();
        }

        // Get department list based on account id.
        public virtual List<SelectListItem> GetAccountDepartments(int? accountId)
          => new AccountAgent(GetClient<AccountClient>(), GetClient<AccessPermissionClient>(), GetClient<UserClient>(), GetClient<PriceClient>()).GetAccountDepartmentList(accountId);

        // Get b2b permission list.
        public virtual string GetPermissionList(int? accountId, int accountPermissionId)
            => new AccountAgent(GetClient<AccountClient>(), GetClient<AccessPermissionClient>(), GetClient<UserClient>(), GetClient<PriceClient>()).AccountPermissionList(accountId, accountPermissionId);

        // Get approver list based on account id.
        public virtual List<SelectListItem> GetApproverList(int accountId, int? userId, string currentUserName)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            List<SelectListItem> listItems = new List<SelectListItem>();

            //filters for approver list.
            FilterCollection filters = new FilterCollection();
            filters.Add(ZnodeAccountEnum.AccountId.ToString(), FilterOperators.Equals, accountId.ToString());
            filters.Add(FilterKeys.IsApprovalList, FilterOperators.NotEquals, ZnodeConstant.AccountUser);
            int loggedUserAccountId = SessionProxyHelper.GetUserDetails().UserId;
            ZnodeLogging.LogMessage("Parameters:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { loggedUserAccountId = loggedUserAccountId , filters = filters });

            //Get all available portal list.
            UserListModel approverList = _userClient.GetUserAccountList(loggedUserAccountId, filters, null, null, null);

            //Remove userId while updating the same user.
            UserModel itemToRemove = approverList?.Users?.FirstOrDefault(r => r.UserId == userId);
            if (IsNotNull(itemToRemove))
                approverList.Users.Remove(itemToRemove);

            if (approverList?.Users?.Count > 0)
            {
                listItems = (from item in approverList.Users
                             select new SelectListItem
                             {
                                 Text = item.FullName,
                                 Value = item.UserId.ToString(),
                             }).ToList();
            }
            ZnodeLogging.LogMessage("listItems list count:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, listItems?.Count());

            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            return listItems;
        }

        public virtual List<SelectListItem> GetAccountRoleList()
           => new AccountAgent(GetClient<AccountClient>(), GetClient<AccessPermissionClient>(), GetClient<UserClient>(), GetClient<PriceClient>()).GetAccountRoleList();

        // Sets the customer view model.
        public virtual void SetCustomerViewModel(CustomerViewModel customerAccountDetails)
        {
            ZnodeLogging.LogMessage("Agent method Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            if (IsNotNull(customerAccountDetails))
                SetCustomerData(customerAccountDetails);
        }

        // Sets the customer OMS view model.
        public virtual void SetOMSCustomerViewModel(CustomerViewModel customerAccountDetails)
        {
            ZnodeLogging.LogMessage("Agent method Execution started:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            if (IsNotNull(customerAccountDetails))
            {
                customerAccountDetails.Accounts = GetAccountsByPortal(Convert.ToInt32(customerAccountDetails.PortalId));
                SetCustomerData(customerAccountDetails);
            }
        }

        // Get accounts by portal Id.
        public virtual List<SelectListItem> GetAccountsByPortal(int portalId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            List<SelectListItem> listItems = new List<SelectListItem>();

            AccountListModel accountList = _accountClient.GetAccountList(null, null, null, null, null);

            if (accountList?.Accounts?.Count > 0)
            {
                List<AccountModel> accountListModel = null;
                //If the global level user creation is set to true then no need to filter the account by portal.
                if (DefaultSettingHelper.AllowGlobalLevelUserCreation)
                    accountListModel = accountList.Accounts.Select(s => new AccountModel { Name = s.Name, AccountId = s.AccountId }).ToList();
                else
                    accountListModel = accountList.Accounts.Where(w => w.PortalId == portalId).Select(s => new AccountModel { Name = s.Name, AccountId = s.AccountId }).ToList();

                if (accountListModel?.Count > 0)
                {
                    listItems = (from item in accountListModel
                                 select new SelectListItem
                                 {
                                     Text = item.Name,
                                     Value = item.AccountId.ToString()
                                 }).ToList();
                }
            }
            ZnodeLogging.LogMessage("listItems list count:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, listItems?.Count());
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            return listItems ?? new List<SelectListItem>();
        }

        // Get states by country code.
        public virtual List<SelectListItem> GetStates(string countryCode)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            if (!string.IsNullOrEmpty(countryCode))
            {
                FilterCollection _filter = new FilterCollection();
                _filter.Add(ZnodeStateEnum.CountryCode.ToString(), FilterOperators.Is, countryCode);
                _filter.Add(ZnodeStateEnum.IsActive.ToString(), FilterOperators.Equals, ZnodeConstant.TrueValue);

                SortCollection sort = new SortCollection();
                sort.Add(ZnodeStateEnum.StateName.ToString(), DynamicGridConstants.ASCKey);

                ZnodeLogging.LogMessage("Parameter:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { filter = _filter, sort = sort });

                return (from c in _stateClient.GetStateList(_filter, sort)?.States
                        select new SelectListItem
                        {
                            Text = c.StateName,
                            Value = c.StateCode
                        }).ToList();
            }
            return new List<SelectListItem>();
        }


        // Get User Cart details by user ID.
        public virtual CreateOrderViewModel GetUserCartByUserId(int userId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            UserModel userModel = _userClient.GetUserAccountData(userId);
            ShoppingCartModel shoppingCartModel = _shoppingCartClient.GetShoppingCart(new CartParameterModel
            {
                CookieMappingId = null,
                UserId = userId,
                PortalId = userModel.PortalId ?? 0,
                PublishedCatalogId = userModel.PublishCatalogId ?? 0,
                LocaleId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale),
            });

            SaveCartModelInSession(userId, shoppingCartModel);

            CreateOrderViewModel createOrderViewModel = new CreateOrderViewModel
            {
                UserId = userId,
                CartViewModel = shoppingCartModel.ToViewModel<CartViewModel>(),
                CustomerName = userModel.FullName,
                IsGuestUser = userModel.IsGuestUser
            };
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            return createOrderViewModel;
        }
        //Get the List of Sale Rep
        public virtual SalesRepUsersListViewModel GetSalesRepListForAccount(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null)
        {
            HelperMethods.SalesRepSort(ref sortCollection);

            SalesRepUserListModel userListModel = _userClient.GetSalesRepListForAccount(filters, sortCollection, pageIndex, recordPerPage);

            SalesRepUsersListViewModel listViewModel = new SalesRepUsersListViewModel { List = userListModel?.SalesRepUsers?.ToViewModel<SalesRepUsersViewModel>().ToList() };
            listViewModel.UserId = Convert.ToInt32(SessionProxyHelper.GetUserDetails()?.UserId);
            SetListPagingData(listViewModel, userListModel);

            return listViewModel?.List?.Count > 0 ? listViewModel : new SalesRepUsersListViewModel() { List = new List<SalesRepUsersViewModel>() };
        }
        #endregion

        #region Users
        //Get user account list  
        public virtual UsersListViewModel GetUserAccountList(string currentUserName, string currentRoleName, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            int loggedUserAccountId = SessionProxyHelper.GetUserDetails().UserId;
            filters.Add(AdminConstants.IsStoreAdmin.ToString(), FilterOperators.Equals, AdminConstants.True);
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { filters = filters, sorts = sortCollection });
            UserListModel userListModel = _userClient.GetUserAccountList(loggedUserAccountId, filters, sortCollection, pageIndex, recordPerPage);
            UsersListViewModel listViewModel = new UsersListViewModel { List = userListModel?.Users?.ToViewModel<UsersViewModel>().ToList() };

            SetListPagingData(listViewModel, userListModel);

            //Set the Tool Menus for User Account List Grid View.
            SetUserAccountToolMenuList(listViewModel);
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            return listViewModel?.List?.Count > 0 ? listViewModel : new UsersListViewModel() { List = new List<UsersViewModel>() };
        }

        //Reset Password in Bulk
        public virtual bool BulkResetPassword(string accountId, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            errorMessage = Admin_Resources.ErrorResetPassword;
            if (!string.IsNullOrEmpty(accountId))
            {
                try
                {
                    //TO DO:
                    //usersViewModel.BaseUrl = GetDomainUrl(areaName);
                    return _userClient.BulkResetPassword(new ParameterModel { Ids = accountId });
                }
                catch (ZnodeException exception)
                {
                    ZnodeLogging.LogMessage(exception, string.Empty, TraceLevel.Warning);
                    switch (exception.ErrorCode)
                    {
                        case ErrorCodes.ErrorResetPassword:
                            errorMessage = string.Format(Admin_Resources.ErrorResetPassword, exception.Message);
                            break;
                        case ErrorCodes.UserNameUnavailable:
                            errorMessage = exception.ErrorMessage;
                            break;
                        case ErrorCodes.AccountLocked:
                            errorMessage = Admin_Resources.ErrorResetAccountPassword;
                            break;
                        case ErrorCodes.EmailTemplateDoesNotExists:
                            errorMessage = Admin_Resources.ResetPasswordTemplateNotFound;
                            return false;
                        default:
                            break;
                    }
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                    return false;
                }
            }
            return false;
        }

        //Reset Password of Existing User User
        public virtual bool ResetPassword(int accountId, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            errorMessage = Admin_Resources.ErrorAccessDenied;
            UserModel accountModel = _userClient.GetUserAccountData(accountId);
            try
            {
                if (IsNotNull(accountModel))
                {
                    if (Equals(accountModel.User?.Email, accountModel.Email))
                    {
                        //Set the domain url.
                        bool isB2BCustomer = IsB2BCustomer(accountModel);
                        accountModel.BaseUrl = isB2BCustomer ? DefaultSettingHelper.AllowGlobalLevelUserCreation ? GetDomains(StoreAgent.CurrentStore?.PortalId) : GetDomains(accountModel.PortalId) : GetDomainUrl();
                        return IsNotNull(_userClient.ForgotPassword(accountModel));
                    }
                    else
                    {
                        errorMessage = Admin_Resources.ValidEmailAddress;
                        return false;
                    }
                }
                else
                {
                    errorMessage = Admin_Resources.ErrorAccessDenied;
                    return false;
                }
            }
            catch (ZnodeException exception)
            {
                ZnodeLogging.LogMessage(exception, string.Empty, TraceLevel.Warning);
                return GetErrorMessage(out errorMessage, exception);
            }
            catch (Exception exception)
            {
                ZnodeLogging.LogMessage(exception, string.Empty, TraceLevel.Error);
                errorMessage = Admin_Resources.ErrorAccessDenied;
                return false;
            }
        }

        //Create User
        public virtual UsersViewModel CreateUser(UsersViewModel usersViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            try
            {
                if (IsNotNull(usersViewModel))
                {
                    usersViewModel = GetPortalIDsArray(usersViewModel);
                    UserModel accountModel = _userClient.CreateUser(usersViewModel.ToModel<UserModel>());
                    if ((accountModel?.UserId > 0) && accountModel.IsEmailSentFailed)
                        return new UsersViewModel() { UserId = accountModel.UserId, IsEmailSentFailed = accountModel.IsEmailSentFailed };
                    return IsNotNull(accountModel) ? accountModel.ToViewModel<UsersViewModel>() : new UsersViewModel();
                }
                ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

                return (UsersViewModel)GetViewModelWithErrorMessage(usersViewModel, Admin_Resources.ErrorFailedToCreate);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.ProfileNotPresent:
                        return (UsersViewModel)GetViewModelWithErrorMessage(usersViewModel, Admin_Resources.ErrorProfileNotExists);
                    case ErrorCodes.EmailTemplateDoesNotExists:
                        return (UsersViewModel)GetViewModelWithErrorMessage(usersViewModel, Admin_Resources.EmailTemplateNotExists);
                    default:
                        return (UsersViewModel)GetViewModelWithErrorMessage(usersViewModel, Admin_Resources.ErrorUserNameAlreadyExists);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return (UsersViewModel)GetViewModelWithErrorMessage(usersViewModel, Admin_Resources.ErrorUserNameAlreadyExists);
            }
        }

        // To convert the string into array of portalIds        
        public virtual UsersViewModel GetPortalIDsArray(UsersViewModel usersViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            if (IsNotNull(usersViewModel.PortalIds) && usersViewModel.PortalIds.Length > 0)
            {
                usersViewModel.PortalIds = usersViewModel.PortalIds[0].Split(',');
            }
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            return usersViewModel;
        }

        // To convert the string into array of portalIds
        public virtual UsersViewModel GetPortalIDsString(UsersViewModel usersViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            if (IsNotNull(usersViewModel.PortalIds) && usersViewModel.PortalIds.Length > 0)
                usersViewModel.PortalIdString = String.Join(",", usersViewModel.PortalIds.Select(p => p.ToString()).ToArray());

            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return usersViewModel;
        }

        //Get user account data by account id.
        public virtual UsersViewModel GetUserAccountData(int accountId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            UsersViewModel model = _userClient.GetUserAccountData(accountId, null)?.ToViewModel<UsersViewModel>() ?? null;
            if (IsNotNull(model))
                return GetPortalIDsString(model);
            return model;
        }

        //Update Existing User 
        public virtual bool UpdateUserAccountData(UsersViewModel model)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            if (IsNotNull(model))
                return _userClient.UpdateUserAccountData(GetPortalIDsArray(model).ToModel<UserModel>(), false)?.UserId > 0;
            return false;
        }

        //Delete Existing User
        public virtual bool DeleteUser(string accountIds, string currentUserName, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            errorMessage = string.Empty;
            int loggedUserAccountId = SessionProxyHelper.GetUserDetails().UserId;
            ZnodeLogging.LogMessage("loggedUserAccountId:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info, new { loggedUserAccountId = loggedUserAccountId });
            if (!string.IsNullOrEmpty(accountIds))
            {
                try
                {
                    string[] IdArray = accountIds.Split(',');
                    if (IdArray.Count(r => r == loggedUserAccountId.ToString()) > 0)
                    {
                        if (Equals(accountIds, loggedUserAccountId.ToString()))
                        {
                            errorMessage = Admin_Resources.FailedDeleteLoginUser;
                            return false;
                        }
                        else
                        {
                            IdArray = IdArray.Where(val => val != loggedUserAccountId.ToString()).ToArray();
                            string accountIdsToDelete = string.Join(",", IdArray);
                            if (_userClient.DeleteUser(new ParameterModel { Ids = accountIdsToDelete }))
                            {
                                errorMessage = Admin_Resources.SuccessDeleteMessageExceptLoginUser;
                                return true;
                            }
                            else
                            {
                                errorMessage = Admin_Resources.ErrorMessageHalfDelete;
                                return false;
                            }
                        }
                    }
                    else
                        return _userClient.DeleteUser(new ParameterModel { Ids = accountIds });
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                    errorMessage = Admin_Resources.ErrorMessageHalfDelete;
                }
            }
            return false;
        }

        //Enable Or Disable Existing User
        public virtual bool EnableDisableUser(string accountId, bool lockUser, string currentUserName, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            errorMessage = string.Empty;

            //Logged in UserId- Remove it from string of account ids so that it does not get locked. 
            int loggedUserAccountId = SessionProxyHelper.GetUserDetails().UserId;
            ZnodeLogging.LogMessage("loggedUserAccountId:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info,new{ loggedUserAccountId= loggedUserAccountId });
            if (!string.IsNullOrEmpty(accountId))
            {
                try
                {
                    string[] IdArray = accountId.Split(',');
                    if (IdArray.Count(r => r == loggedUserAccountId.ToString()) > 0)
                    {
                        if (Equals(accountId, loggedUserAccountId.ToString()))
                        {
                            errorMessage = Admin_Resources.DisableErrorMsgLoggedUser;
                            return false;
                        }
                        else
                        {
                            IdArray = IdArray.Where(val => val != loggedUserAccountId.ToString()).ToArray();
                            string remainingAccountIds = string.Join(",", IdArray);
                            if (_userClient.EnableDisableAccount(new ParameterModel { Ids = remainingAccountIds }, lockUser))
                            {
                                errorMessage = lockUser ? Admin_Resources.SuccessMsgDisableExceptLoggedUser : Admin_Resources.SuccessMsgEnableExceptLoggedUser;
                                return true;
                            }
                            else
                            {
                                errorMessage = Admin_Resources.ErrorLockUnlockRecords;
                                return false;
                            }
                        }
                    }
                    else
                        return _userClient.EnableDisableAccount(new ParameterModel { Ids = accountId }, lockUser);
                }
                catch (ZnodeException ex)
                {
                    ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                    switch (ex.ErrorCode)
                    {
                        case ErrorCodes.LockOutEnabled:
                            errorMessage = Admin_Resources.LockOutDisabledErrorMessage;
                            break;
                        default:
                            errorMessage = Admin_Resources.ErrorLockUnlockRecords;
                            break;
                    }
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                    errorMessage = Admin_Resources.ErrorLockUnlockRecords;
                }
            }
            return false;
        }

        //Gets the assigned portals to user.
        public virtual UserPortalViewModel GetPortalIds(string aspNetUserId, int userId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            UserPortalViewModel userPortalViewModel = _userClient.GetPortalIds(aspNetUserId)?.ToViewModel<UserPortalViewModel>();
            if (IsNull(userPortalViewModel))
            {
                userPortalViewModel = new UserPortalViewModel();
                userPortalViewModel.AspNetUserId = aspNetUserId;
            }
            userPortalViewModel.Portals = GetPortals();
            userPortalViewModel.SelectedPortals = GetSelectedPortals(userPortalViewModel.Portals, userPortalViewModel.PortalIds);
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

            return userPortalViewModel;
        }

        //Save portal ids againt the user.
        public virtual bool SavePortalsIds(UserPortalViewModel viewModel)
        {
            if (IsNotNull(viewModel))
                return IsNotNull(_userClient.SavePortalsIds(viewModel.ToModel<UserPortalModel>()));
            return false;
        }

        /// <summary>
        /// Gets the user address view model from the session
        /// </summary>
        /// <returns>Returns the user view model with the orders, order history, wish list, profile for the user</returns>
        public virtual UserAddressDataViewModel GetUserAccountViewModel()
            => GetFromSession<UserAddressDataViewModel>(AdminConstants.OMSUserAccountSessionKey);

        /// <summary>
        /// Set the Collapse Menu Status in Session
        /// </summary>
        /// <param name="status">status of Menu</param>
        public virtual void SetCollapseMenuStatus(bool status)
        {
            SaveInSession<bool>(AdminConstants.CollapseMenuStatus, status);
        }

        //Check whether username already exists.
        public virtual bool CheckUserNameExist(string userName, int portalId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            userName = userName.Trim();
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(FilterKeys.Username, FilterOperators.Is, userName));
            if (!DefaultSettingHelper.AllowGlobalLevelUserCreation)
                filters.Add(new FilterTuple(FilterKeys.PortalId, FilterOperators.Equals, portalId.ToString()));

            ZnodeLogging.LogMessage("Parameters:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { filters = filters });

            UserListModel userList = _userClient.GetCustomerAccountList(HttpContext.Current.User.Identity.Name, filters, null, null, null);

            if (userList?.Users?.Count > 0)
                return userList.Users.FindIndex(x => string.Equals(x.UserName, userName, StringComparison.CurrentCultureIgnoreCase)) != -1;

            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return false;
        }

        //Check whether username already exists.
        public virtual bool CheckUserNameExist(string userName, int? portalId, int userId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            userName = userName.Trim();
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(FilterKeys.Username, FilterOperators.Is, userName));
            filters.Add(new FilterTuple(FilterKeys.UserId, FilterOperators.NotEquals, userId.ToString()));
            if (!DefaultSettingHelper.AllowGlobalLevelUserCreation)
                filters.Add(new FilterTuple(FilterKeys.PortalId, FilterOperators.Equals, portalId.ToString()));

            ZnodeLogging.LogMessage("Parameters:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { filters = filters });

            UserListModel userList = _userClient.GetCustomerAccountList(HttpContext.Current.User.Identity.Name, filters, null, null, null);

            if (userList?.Users?.Count > 0)
                return userList.Users.FindIndex(x => string.Equals(x.UserName, userName, StringComparison.CurrentCultureIgnoreCase)) != -1;

            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return false;
        }



        //Check whether username is existing shopper.
        public virtual bool CheckIsUserNameAnExistingShopper(string username, out string errorMessage)
        {
            errorMessage = string.Empty;
            bool status = false;
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                username = username.Trim();
                FilterCollection filters = new FilterCollection();
                filters.Add(new FilterTuple(FilterKeys.Username, FilterOperators.Is, username));

                ZnodeLogging.LogMessage("Parameters:", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, new { filters = filters });

                status = _userClient.CheckIsUserNameAnExistingShopper(username);

            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                errorMessage = ex.ErrorMessage;
                switch (ex.ErrorCode )
                {
                    case ErrorCodes.AlreadyExist :                       
                        return true;
                    case ErrorCodes.CustomerAccountError:                      
                        return true;
                    default:                     
                        return false;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return false;
            }
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return status;
        }

        //Convert shopper to admin User User
        public virtual UsersViewModel ConvertShopperToAdmin(UsersViewModel usersViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            try
            {
                if (IsNotNull(usersViewModel))
                {
                    usersViewModel = GetPortalIDsArray(usersViewModel);
                    UserModel accountModel = _userClient.ConvertShopperToAdmin(usersViewModel.ToModel<UserModel>());
                    return IsNotNull(accountModel) ? accountModel.ToViewModel<UsersViewModel>() : new UsersViewModel();
                }
                ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

                return (UsersViewModel)GetViewModelWithErrorMessage(usersViewModel, Admin_Resources.ErrorFailedToConvertShopperToAdmin);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return (UsersViewModel)GetViewModelWithErrorMessage(usersViewModel, Admin_Resources.ErrorFailedToConvertShopperToAdmin);
            }
        }

        #endregion

        #region Impersonation
        //Gets the webstore url on portal id.
        public string GetImpersonationUrl(int userId, int portalId)
        {
            var launchUrl = string.Empty;

            // Get webstore url for the perticular portal
            string webStoreUrl = GetDomains(portalId);
            if (string.IsNullOrEmpty(webStoreUrl))
            {
                return launchUrl;
            }
            // Get CSR userid
            int adminUserId = Convert.ToInt32(SessionProxyHelper.GetUserDetails()?.UserId);

            // Get shopper username from Id
            string userName = GetCustomerAccountById(userId)?.UserName;

            if (string.IsNullOrEmpty(userName) || adminUserId == 0)
            {
                return launchUrl;
            }
            // Generate encrypted token on the basis of CSRId, Webstore userId, webstore username
            var launchToken = GenerateToken(adminUserId, userId, userName);

            launchUrl = $"{webStoreUrl}/User/ImpersonationLogin?token={launchToken}";

            return launchUrl;
        }
        #endregion


        #region Update Billing Account Number
        /// <summary>
        /// Get Call of Update Billing Number
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public virtual GlobalAttributeEntityDetailsViewModel GetEntityAttributeDetails(int userId)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            GlobalAttributeEntityDetailsViewModel entityAttributeModel = new GlobalAttributeEntityDetailsViewModel();
            if (userId > 0)
            {
                entityAttributeModel = GlobalAttributeModelMap.ToGlobalAttributeEntityDetailViewModel(_globalAttributeEntityClient.GetEntityAttributeDetails(userId, "User"));
            }
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return entityAttributeModel ?? new GlobalAttributeEntityDetailsViewModel();
        }

        /// <summary>
        /// Save Call Update Billing Number
        /// </summary>
        /// <param name="model"></param>
        /// <param name="errorMessage"></param>
        /// <returns></returns>
        public virtual EntityAttributeViewModel UpdateBillingNumber(BindDataModel model, out string errorMessage)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            errorMessage = string.Empty;
            try
            {
                int entityValue = Convert.ToInt32(model.GetValue("EntityId"));
                string entityType = Convert.ToString(model.GetValue("EntityType"));
                ZnodeLogging.LogMessage("Parameter:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info, new { entityValue = entityValue, entityType = entityType });

                //Remove unwanted attributes present on form collection
                RemoveNonAttributeKeys(model);
                RemoveAttributeWithEmptyValue(model);
                RemoveAttrAndMceEditorKeyWord(model);
                EntityAttributeViewModel attributeViewModel = GetEntityAttributeViewModel(model);
                attributeViewModel.EntityValueId = entityValue;
                attributeViewModel.EntityType = entityType;
                EntityAttributeModel entityAttribute = _userClient.UpdateUserAndQuoteDetails(attributeViewModel.ToModel<EntityAttributeModel>());
                ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

                return entityAttribute.ToViewModel<EntityAttributeViewModel>() ?? new EntityAttributeViewModel();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                switch (ex.ErrorCode)
                {
                    case ErrorCodes.AlreadyExist:
                        errorMessage = Attributes_Resources.ErrorAttributeAlreadyExists;
                        return new EntityAttributeViewModel { HasError = true };

                    case ErrorCodes.SKUAlreadyExist:
                        errorMessage = PIM_Resources.ErrorSKUAlreadyExists;
                        return new EntityAttributeViewModel { HasError = true };

                    default:
                        errorMessage = Admin_Resources.ErrorFailedToCreate;
                        return new EntityAttributeViewModel { HasError = true };
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return new EntityAttributeViewModel { HasError = true, ErrorMessage = Admin_Resources.ErrorFailedToCreate };
            }
        }
        #endregion

        #region Private Methods
        private UserViewModel ResetPassword(UserViewModel model)
        {
            try
            {
                _userClient.ForgotPassword(UserViewModelMap.ToAccountModel(model));
                model.SuccessMessage = Admin_Resources.SuccessResetPassword;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Warning);
                SetErrorProperties(model, ex.ErrorMessage);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
            }
            return model;
        }

        //Method gets the Domain Base Url
        private string GetDomainUrl()
            => (!string.IsNullOrEmpty(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority)))
                ? HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) : string.Empty;

        //Update the Model with error properties.Return the updated model in AccountViewModelFormat
        private UserViewModel SetErrorProperties(UserViewModel model, string errorMessage)
        {
            model.HasError = true;
            model.ErrorMessage = errorMessage;
            return model;
        }

        // To Update the View Model, based on the error code conditions for the logged in user.
        //Return the mapped view model in LoginViewModel format.
        private LoginViewModel ReturnErrorViewModel(LoginViewModel model, ZnodeException ex, bool isResetAdmin)
        {
            UserModel accountModel = _userClient.GetAccountByUser(model.Username);
            SaveInSession(AdminConstants.UserAccountSessionKey, accountModel);
            SaveInSession(AdminConstants.ErrorCodeSessionKey, ex.ErrorCode);
            if (isResetAdmin)
                model.IsResetAdmin = true;
            else
                model.IsResetAdminPassword = true;

            model.HasError = true;
            model.ErrorMessage = ex.ErrorMessage;
            return model;
        }

        //Check whether the current login user has Customer user and B2B role or not.
        protected virtual bool IsB2BCustomer(UserModel accountModel)
        {
            //Get list of Roles.
            List<string> roleList = GetB2BRoles();
            ZnodeLogging.LogMessage("lstRole list count:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, roleList?.Count());
            if (IsNull(accountModel?.RoleName) || IsNull(roleList))
                return false;
            else
                return roleList.Contains(accountModel.RoleName);
        }

        //Gets the url on the basis of portal id.
        private string GetDomains(int? portalId)
        {
            FilterCollection filters = new FilterCollection()
            {
                new FilterTuple(ZnodeDomainEnum.PortalId.ToString(), FilterOperators.Equals, portalId.ToString()),
                new FilterTuple(ZnodeDomainEnum.ApplicationType.ToString(), FilterOperators.Is,ApplicationTypesEnum.WebStore.ToString()),
                new FilterTuple (ZnodeDomainEnum.IsActive .ToString(),FilterOperators.Equals ,AdminConstants.True),
                new FilterTuple (ZnodeDomainEnum.IsDefault .ToString(),FilterOperators.Equals ,AdminConstants.True)
            };

            SortCollection sorts = new SortCollection();
            sorts = HelperMethods.SortAsc(ZnodeDomainEnum.DomainId.ToString(), sorts);
            ZnodeLogging.LogMessage("Parameters:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { sorts = sorts, filters = filters });

            return _domainClient.GetDomains(filters, sorts, null, null)?.Domains?.Where(x => x.ApplicationType.Equals(ApplicationTypesEnum.WebStore.ToString(), StringComparison.CurrentCultureIgnoreCase)).ToViewModel<DomainViewModel>().ToList().Select(x => x.DomainName).FirstOrDefault();
        }

        //Get B2B Roles.
        private List<string> GetB2BRoles()
        {
            //Gets the filtered list of roles which are contains b2b roles and user role.
            List<RoleModel> rolesList = _roleClient.GetRoleList(null, null, null, null)?.Roles?
                     .Where(x => Equals(x.TypeOfRole?.ToLower(), ZnodeRoleEnum.B2B.ToString().ToLower()) || Equals(x.Name?.ToLower(), ZnodeRoleEnum.User.ToString().ToLower()) && x.IsActive == true).ToList();
            if (rolesList?.Count > 0)
                return rolesList.OrderBy(x => x.Name).Select(x => x.Name).ToList();
            return new List<string>();
        }

        //Get selected portals.
        private List<SelectListItem> GetSelectedPortals(List<SelectListItem> availablePortals, string[] selectedPortalIds)
        {
            List<SelectListItem> availableSelectedList = new List<SelectListItem>();
            if (selectedPortalIds?.Count() > 0 && availablePortals?.Count > 0)
            {
                foreach (string portalId in selectedPortalIds)
                {
                    SelectListItem portal = availablePortals.Find(x => x.Value == portalId);

                    //Check for null if the record is not present in available ports.
                    if (IsNotNull(portal))
                        availableSelectedList.Add(portal);
                }
            }
            ZnodeLogging.LogMessage("availableSelectedList list count:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, availableSelectedList?.Count());
            return availableSelectedList;
        }

        //Set the Tool Menus for User Account List Grid View.
        private void SetUserAccountToolMenuList(UsersListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = GetGridModel();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('AccountDeletePopup')", ControllerName = "User", ActionName = "DeleteUser" });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.Unlock, JSFunctionName = "EditableText.prototype.DialogDelete('accountEnable')", ControllerName = "User", ActionName = "EnableDisableAccount" });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.Lock, JSFunctionName = "EditableText.prototype.DialogDelete('accountdisable')", ControllerName = "User", ActionName = "EnableDisableAccount" });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ResetPasswordButtonText, JSFunctionName = "EditableText.prototype.DialogDelete('accountresetpassword')", ControllerName = "User", ActionName = "BulkResetPassword" });
            }
        }

        //Set the Tool Menus for Customer Account List Grid View.
        private void SetCustomerAccountToolMenuList(CustomerListViewModel model)
        {
            if (IsNotNull(model))
            {
                model.GridModel = GetGridModel();
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ButtonDelete, JSFunctionName = "EditableText.prototype.DialogDelete('MediaDeletePopup')", ControllerName = "User", ActionName = "CustomerDelete" });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.Unlock, JSFunctionName = "EditableText.prototype.DialogDelete('accountEnable')", ControllerName = "User", ActionName = "CustomerEnableDisableAccount" });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.Lock, JSFunctionName = "EditableText.prototype.DialogDelete('accountdisable')", ControllerName = "User", ActionName = "CustomerEnableDisableAccount" });
                model.GridModel.FilterColumn.ToolMenuList.Add(new ToolMenuModel { DisplayText = Admin_Resources.ResetPasswordButtonText, JSFunctionName = "EditableText.prototype.DialogDelete('accountresetpassword')", ControllerName = "User", ActionName = "BulkResetPassword" });
            }
        }

        //Get the error message on the basis of error codes.
        private static bool GetErrorMessage(out string errorMessage, ZnodeException exception)
        {
            switch (exception.ErrorCode)
            {
                case ErrorCodes.ErrorSendResetPasswordLink:
                    errorMessage = Admin_Resources.ErrorSendResetPasswordLink;
                    return false;

                case ErrorCodes.UserNameUnavailable:
                    errorMessage = exception.ErrorMessage;
                    return false;

                case ErrorCodes.AccountLocked:
                    errorMessage = Admin_Resources.ErrorResetAccountPassword;
                    return false;

                case ErrorCodes.EmailTemplateDoesNotExists:
                    errorMessage = Admin_Resources.ResetPasswordTemplateNotFound;
                    return false;

                default:
                    errorMessage = Admin_Resources.ErrorAccessDenied;
                    return false;
            }
        }

        private void SetCustomerData(CustomerViewModel customerAccountDetails)
        {
            customerAccountDetails.Roles = GetAccountRoleList();
            customerAccountDetails.Departments = GetAccountDepartments(customerAccountDetails.AccountId.GetValueOrDefault());
            customerAccountDetails.UserApprovalList = GetApproverList(customerAccountDetails.AccountId.GetValueOrDefault(), customerAccountDetails.UserId, HttpContext.Current.User.Identity.Name);
        }

        /// <summary>
        /// Bind Attribute Model
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        private EntityAttributeViewModel GetEntityAttributeViewModel(BindDataModel model)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            EntityAttributeViewModel entityAttributeModel = new EntityAttributeViewModel();

            model.ControlsData?.ToList().ForEach(item =>
            {
                List<object> itemList = new List<object>();
                itemList.AddRange(item.Key.Split('_'));
                if (itemList.Count() >= 5)
                {
                    entityAttributeModel.Attributes.Add(new EntityAttributeDetailsViewModel
                    {
                        AttributeCode = itemList[0].ToString(),
                        GlobalAttributeId = Convert.ToInt32(itemList[1]),
                        GlobalAttributeDefaultValueId = Convert.ToInt32(itemList[2]),
                        GlobalAttributeValueId = Convert.ToInt32(itemList[3]),
                        AttributeValue = item.Value.ToString().Trim(),
                        LocaleId = Convert.ToInt32(DefaultSettingHelper.DefaultLocale)
                    });
                }
            });
            ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            return entityAttributeModel;
        }

        //Add Portal id in filter collection
        private void AddPortalIdInFilters(FilterCollection filters, int portalId)
        {
            if (portalId > 0)
            {
                filters.RemoveAll(x => string.Equals(x.FilterName, FilterKeys.PortalId, StringComparison.InvariantCultureIgnoreCase));
                filters.RemoveAll(x => string.Equals(x.FilterName, AdminConstants.IsSalesRepList, StringComparison.InvariantCultureIgnoreCase));
                filters.Add(new FilterTuple(FilterKeys.PortalId.ToString(), FilterOperators.Equals, portalId.ToString()));
            }
        }

        //Map store filter values in view model
        private void BindStoreFilterValues(CustomerListViewModel customerListViewModel, int portalId, string portalName)
        {
            customerListViewModel.PortalName = string.IsNullOrEmpty(portalName) ? Admin_Resources.DefaultAllStores : portalName;
            customerListViewModel.PortalId = portalId;
        }

        //Generate encrypted token on the basis of CSRId, userid and username
        public string GenerateToken(int adminUserId, int userId, string userName)
        {
            string encryptedToken = ZnodeTokenHelper.GenerateCSRToken(adminUserId, userId, userName);
            // Browser's URL decoding converts "+" sign into space character.
            // This code will replace + sign with space.
            return encryptedToken.Replace("+", "%2B");
        }
        #endregion

        //Get customer account details by customer account id.
        public virtual CustomerViewModel GetCustomerAccountDetails(int userId)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                ZnodeLogging.LogMessage("Parameters:", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Verbose, new { AccountUserId = userId });

                //Gets the list.
                UserModel userModel = _userClient.GetCustomerAccountDetails(userId);

                ZnodeLogging.LogMessage("Agent method executed.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);

                if (IsNotNull(userModel))
                    return userModel?.ToViewModel<CustomerViewModel>();

                return new CustomerViewModel { HasError = true };
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                if (ex.ErrorCode == ErrorCodes.NotPermitted)
                {
                    throw new ZnodeException(ErrorCodes.NotPermitted, Admin_Resources.ErrorAccessMessage);
                }
                return new CustomerViewModel { HasError = true };
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                return new CustomerViewModel { HasError = true };
            }
        }

        // To update the username of the registered user
        public virtual bool UpdateUsernameForRegisteredUser(UserDetailsViewModel userDetailsViewModel)
        {
            try
            {
                ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Customers.ToString(), TraceLevel.Info);
                return _userClient.UpdateUsernameForRegisteredUser(userDetailsViewModel?.ToModel<UserDetailsModel>());
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);                
            }
            return false;
        }

        protected virtual AdminUserModel AdminUserModelMapper(int loggedUserAccountId, string roleName, string columnList, string currentUserName)
        {
            AdminUserModel userAdminModel = new AdminUserModel();
            userAdminModel.LoggedUserAccountId = loggedUserAccountId;
            userAdminModel.RoleName = roleName;
            userAdminModel.ColumnList = columnList;
            userAdminModel.UserName = currentUserName;

            return userAdminModel;
        }
    }
}