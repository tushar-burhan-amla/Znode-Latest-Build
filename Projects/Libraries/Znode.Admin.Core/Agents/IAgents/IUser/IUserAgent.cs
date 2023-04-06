using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Sorts;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Agents
{
    public interface IUserAgent
    {
        /// <summary>
        /// This method is used to login the user.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Return the login details.</returns>
        LoginViewModel Login(LoginViewModel model);

        /// <summary>
        /// This method is used to logout the user.
        /// </summary>
        void Logout();

        /// <summary>
        /// Function used to Change/Reset the user password.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Returns the Change/Reset password details.</returns>
        ChangePasswordViewModel ChangePassword(ChangePasswordViewModel model);

        /// <summary>
        /// Function used for forgot password.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Returns forgot password details.</returns>
        UserViewModel ForgotPassword(UserViewModel model);

        /// <summary>      
        /// This function will verify the Reset Password Link current status.
        /// </summary>
        /// <param name="model">ChangePasswordViewModel</param>
        /// <returns>Returns Status of Reset Password Link.</returns>
        ResetPasswordStatusTypes VerifyResetPasswordLinkStatus(ChangePasswordViewModel model);

        /// <summary>
        /// To Check whether the User details are present in Session.
        /// </summary>
        /// <returns>Returns true or false.</returns>
        bool CheckUserKey();

        /// <summary>
        /// Get available portal list.
        /// </summary>
        /// <returns>Returns list of available portals.</returns>
        List<SelectListItem> GetPortals();

        /// <summary>
        /// Get department list based on account id.
        /// </summary>
        /// <param name="accountId">accountId to get department list.</param>
        /// <returns>Returns list of department list.</returns>
        List<SelectListItem> GetAccountDepartments(int? accountId);

        /// <summary>
        /// Check whether user name exits or not.
        /// </summary>
        /// <param name="userName">userName to check.</param>
        /// <param name="portalId">portalId</param>
        /// <returns></returns>
        bool CheckUserNameExist(string userName, int portalId);

        /// <summary>
        /// Check whether user name is existing customer or not.
        /// </summary>
        /// <param name="username">userName to check.</param>
        /// <returns></returns>
        bool CheckIsUserNameAnExistingShopper(string username, out string errorMessage);

        #region Customers

        /// <summary>
        /// Create customer account.
        /// </summary>
        /// <param name="customerViewModel">Customer Account ViewModel.</param>
        /// <returns>Returns created customer account.</returns>
        CustomerViewModel CreateCustomerAccount(CustomerViewModel customerViewModel);

        /// <summary>
        /// Get customer account data by customer account id.
        /// </summary>
        /// <param name="userId">User id to get details of respective customer.</param>
        /// <returns>Returns Customer View Model.</returns>
        CustomerViewModel GetCustomerAccountById(int userId);

        /// <summary>
        /// Updates customer account.
        /// </summary>
        /// <param name="model">Customer View Model</param>
        /// <param name="errorMessage">error message</param>
        /// <returns>Returns true if account updated successfully, else return false.</returns>
        bool UpdateCustomerAccount(CustomerViewModel model, out string errorMessage);

        /// <summary>
        /// Delete customer account.
        /// </summary>
        /// <param name="UserIds">User Ids id of customer.</param>
        /// <param name="currentUserName">Logged in user name.</param>
        /// <param name="errorMessage">error message</param>
        /// <returns>Returns true if account deleted successfully, else return false.</returns>
        bool DeleteCustomerAccount(string UserIds, string currentUserName, out string errorMessage);

        /// <summary>
        /// Get customer account list.
        /// </summary>
        /// <param name="currentUserName">User name of logged in user.</param>
        /// <param name="filters">Filters for Account.</param>
        /// <param name="sorts">Sorts for Account.</param>
        /// <param name="page">Page.</param>
        /// <param name="portalId">Filter portalId.</param>
        /// <param name="portalName">Filter portalName</param>
        /// <returns>Returns AccountListModel.</returns>
        CustomerListViewModel GetCustomerAccountList(string currentUserName, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null, int portalId = 0, string portalName = null);

        /// <summary>
        ///  Get the Sales Rep Users List for Association
        /// </summary>
        /// <param name="filters">filter collection</param>
        /// <param name="sortCollection">sort collection</param>
        /// <param name="pageIndex">pageIndex</param>
        /// <param name="recordPerPage">recordPerPage</param>
        /// <returns></returns>
        UsersListViewModel GetSalesRepListForAssociation(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Enable Disable customer accounts.
        /// </summary>
        /// <param name="userId">User Ids whose account has to be enabled or disabled.</param>
        /// <param name="lockUser">To lock or unlock customer account.</param>
        /// <param name="errorMessage">error message</param>
        /// <returns>Returns true or false.</returns>
        bool EnableDisableUser(string userId, bool lockUser, out string errorMessage);

        /// <summary>
        /// Get available account list.
        /// </summary>
        /// <returns>Returns list of available account.</returns>
        List<SelectListItem> GetAccounts();

        /// <summary>
        /// Get b2b permission list.
        /// </summary>
        /// <returns>Returns list of b2b permission.</returns>
        string GetPermissionList(int? accountId, int accountPermissionId);

        /// <summary>
        /// Get approver list based on account id.
        /// </summary>
        /// <param name="accountId">accountId to get approver list.</param>
        /// <param name="userId">userId</param>
        /// <param name="currentUserName">currentUserName to get approver list.</param>
        /// <returns>Returns approver list.</returns>
        List<SelectListItem> GetApproverList(int accountId, int? userId, string currentUserName);

        /// <summary>
        /// Get the roles for account.
        /// </summary>
        /// <returns></returns>
        List<SelectListItem> GetAccountRoleList();

        /// <summary>
        /// Sets the customer view model.
        /// </summary>
        /// <param name="customerAccountDetails">Customer view model to set.</param>
        void SetCustomerViewModel(CustomerViewModel customerAccountDetails);

        /// <summary>
        /// Sets the customer OMS view model.
        /// </summary>
        /// <param name="customerAccountDetails">Customer view model to set.</param>
        void SetOMSCustomerViewModel(CustomerViewModel customerAccountDetails);

        /// <summary>
        /// Get states by country code.
        /// </summary>
        /// <param name="countryCode"></param>
        /// <returns></returns>
        List<SelectListItem> GetStates(string countryCode);

        SalesRepUsersListViewModel GetSalesRepListForAccount(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);

        #endregion

        #region Impersonation
        /// <summary>
        /// Get a WebStore URL with a "launch token" in the URL. When loaded, the URL starts a session for the customer
        /// without requiring login credentials. The URL is only valid for a short time period and can only be used once
        /// to help protect against unauthorized use.
        /// </summary>
        /// <param name="userId">ID of user.</param>
        /// <param name="portalId">ID of web store portal that the launch URL should launch into.</param>
        /// <returns>Returns created customer launch URL.</returns>
        string GetImpersonationUrl(int userId, int portalId);
        #endregion

        #region Users
        /// <summary>
        /// Reset password of the user account.
        /// </summary>
        /// <param name="userId">userId whose password has to be reset.</param>
        /// <param name="errorMessage">Error Message.</param>
        /// <returns>Returns true/false.</returns>
        bool BulkResetPassword(string userId, out string errorMessage);

        /// <summary>
        /// Reset password functionality for single account.
        /// </summary>
        /// <param name="userId">userId whose password has to be reset.</param>
        /// <param name="errorMessage">Error Message.</param>
        /// <returns>Returns UsersViewModel</returns>
        bool ResetPassword(int userId, out string errorMessage);

        /// <summary>
        /// Get user account list.
        /// </summary>
        /// <param name="currentUserName">User name of logged in user.</param>
        /// <param name="currentRoleName">Role name for store admin.(Admin)</param>
        /// <param name="filters">Filters for Account.</param>
        /// <param name="sortCollection">Sorts for Account.</param>
        /// <param name="pageIndex">Page Index.</param>
        /// <param name="recordPerPage">Page Size.</param>
        /// <returns>Returns UsersListViewModel.</returns>
        UsersListViewModel GetUserAccountList(string currentUserName, string currentRoleName, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Create User.
        /// </summary>
        /// <param name="usersViewModel">Users View Model</param>
        /// <returns>Returns UsersViewModel.</returns>
        UsersViewModel CreateUser(UsersViewModel usersViewModel);


        /// <summary>
        /// To convert the string of portalIds  into array of portalIds
        /// </summary>
        /// <param name="usersViewModel"></param>
        /// <returns>Returns UsersViewModel.</returns>
        UsersViewModel GetPortalIDsArray(UsersViewModel usersViewModel);

        /// <summary>
        /// Get user account data.
        /// </summary>
        /// <param name="userId">user Id to get user account data.</param>
        /// <returns>Returns UsersViewModel</returns>
        UsersViewModel GetUserAccountData(int userId);

        /// <summary>
        /// To get array of portalIds into string
        /// </summary>
        /// <param name="usersViewModel"></param>
        /// <returns>Returns UsersViewModel.</returns>
        UsersViewModel GetPortalIDsString(UsersViewModel usersViewModel);

        /// <summary>
        /// Update user account data.
        /// </summary>
        /// <param name="model">AccountModel.</param>
        /// <returns>Returns true/false.</returns>
        bool UpdateUserAccountData(UsersViewModel model);

        /// <summary>
        /// Delete user account.
        /// </summary>
        /// <param name="userId">userIds to be deleted.</param>
        /// <param name="currentUserName">Logged in user name.</param>
        /// <param name="errorMessage">Error Message.</param>
        /// <returns>Returns true if account deleted successfully, else false.</returns>
        bool DeleteUser(string userId, string currentUserName, out string errorMessage);

        /// <summary>
        /// Enable disable current user by account id.
        /// </summary>
        /// <param name="userId">userId to enable/disable account.</param>
        /// <param name="lockUser">To lock unlock user account.</param>
        /// <param name="currentUserName">Logged in UserName.</param>
        /// <param name="errorMessage">Error message to display.</param>
        /// <returns>Returns true/false.</returns>
        bool EnableDisableUser(string userId, bool lockUser, string currentUserName, out string errorMessage);

        /// <summary>
        /// Gets the assigned portals to user.
        /// </summary>
        /// <param name="aspNetUserId">User id.</param>
        /// <returns>Returns assigned portals to user.</returns>
        UserPortalViewModel GetPortalIds(string aspNetUserId, int userId);

        /// <summary>
        /// Show change password popup.
        /// </summary>
        /// <returns>true/false</returns>
        bool IsShowChangePasswordPopup();

        /// <summary>
        /// Save value in cookie for change password popup. 
        /// </summary>
        /// <returns>true/false</returns>
        bool SaveInCookie();

        /// <summary>
        /// Save portal ids againt the user.
        /// </summary>
        /// <param name="viewModel">Model to save portal ids againt the user.</param>
        /// <returns>Returns true if saved successfully.</returns>
        bool SavePortalsIds(UserPortalViewModel viewModel);

        /// <summary>
        /// Get user details from session.
        /// </summary>
        /// <returns>UserAddressDataViewModel</returns>
        UserAddressDataViewModel GetUserAccountViewModel();

        /// <summary>
        /// Get accounts by portal id
        /// </summary>
        /// <param name="portalId">Portal Id</param>
        /// <returns>SelectListItem</returns>
        List<SelectListItem> GetAccountsByPortal(int portalId);

        /// <summary>
        /// Set the Collapse Menu Status in Session
        /// </summary>
        /// <param name="status">status of Menu</param>
        void SetCollapseMenuStatus(bool status);

        /// <summary>
        /// Get account list.
        /// </summary>
        /// <param name="filters"></param>
        /// <param name="sortCollection"></param>
        /// <param name="pageIndex"></param>
        /// <param name="recordPerPage"></param>
        /// <returns></returns>
        AccountListViewModel GetAccountList(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null, string accountCode = null);

        /// <summary>
        /// Get store name based on portal id.
        /// </summary>
        /// <param name="portalId"></param>
        /// <returns></returns>
        string GetStoreName(int portalId);

        /// <summary>
        /// GetEntityAttributeDetails
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        GlobalAttributeEntityDetailsViewModel GetEntityAttributeDetails(int userId);

        /// <summary>
        /// Save Entity Attributes value
        /// </summary>
        /// <param name="model">BindDataModel</param>
        /// <param name="errorMessage">error message</param>
        /// <returns>returns Entity Attribute View Model</returns>
        EntityAttributeViewModel UpdateBillingNumber(BindDataModel model, out string errorMessage);

        /// <summary>
        /// Get User Cart details by user ID.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>Cart details as per user id.</returns>
        CreateOrderViewModel GetUserCartByUserId(int userId);

        /// <summary>
        /// Convert shopper ro admin.
        /// </summary>
        /// <param name="userId">userId whose password has to be reset.</param>
        /// <param name="errorMessage">Error Message.</param>
        /// <returns>Returns UsersViewModel</returns>
        UsersViewModel ConvertShopperToAdmin(UsersViewModel usersViewModel);
        #endregion

        /// <summary>
        /// Get customer account data by customer account id.
        /// </summary>
        /// <param name="userId">User id to get details of respective customer.</param>
        /// <returns>Returns Customer View Model.</returns>
        CustomerViewModel GetCustomerAccountDetails(int userId);

        /// <summary>
        /// To update the username of the registered user.
        /// </summary>
        /// <param name="userDetailsViewModel">UserDetailsViewModel</param>
        /// <returns>True if username updated successfully</returns>
        bool UpdateUsernameForRegisteredUser(UserDetailsViewModel userDetailsViewModel);
    }
}
