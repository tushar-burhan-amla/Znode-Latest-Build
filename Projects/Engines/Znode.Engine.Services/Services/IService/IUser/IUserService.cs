using System.Collections.Generic;
using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IUserService
    {
        /// <summary>
        /// This method is used to logged in to the site
        /// </summary>
        /// <param name="portalId">integer Portal ID</param>
        /// <param name="model">AccountModel model</param>
        /// <param name="errorCode">Out param string error code</param>
        /// <param name="expand">Expand collection</param>
        /// <returns>Returns the data for the logged in user.</returns>
        UserModel Login(int portalId, UserModel model, out int? errorCode, NameValueCollection expand);
        /// <summary>
        /// This method is used to change/Reset the password.
        /// </summary>
        /// <param name="model">AccountModel model</param>
        /// <returns>Returns changed data in AccountModel format</returns>
        UserModel ChangePassword(int portalId, UserModel model);

        /// <summary>
        /// This method will send an email containing password reset link.
        /// </summary>
        /// <param name="accountIds">Parameter Model containing string of Account Ids.</param>
        /// <returns>Returns true/false.</returns>
        bool BulkResetPassword(ParameterModel accountIds);

        /// <summary>
        /// This method will send an email containing password reset link.
        /// </summary>
        /// <param name="portalId">integer Portal ID</param>
        /// <param name="model">AccountModel model</param>
        /// <returns>returns all the account model data</returns>
        UserModel ForgotPassword(int portalId, UserModel model,bool isUserCreateFromAdmin=false,bool isAdminUser=false);

        /// <summary>
        /// This function will verify the Reset Password Link current status.
        /// </summary>
        /// <param name="portalId">integer Portal ID</param>
        /// <param name="model">UserModel.</param>
        /// <returns>Returns Reset Password Link Status i.e. Error codes</returns>
        int? VerifyResetPasswordLinkStatus(int portalId, UserModel model);

        /// <summary>
        /// This method will enable/disable the admin account.
        /// </summary>
        /// <param name="userIds">Account Ids to enable/disable user account.</param> 
        /// <param name="lockUser">To lock or unlock account.</param>
        /// <returns>Returns true if user account deleted successfully, else return false.</returns>
        bool EnableDisableUser(ParameterModel userIds, bool lockUser);

        /// <summary>
        /// Enable Disable user account.
        /// </summary>
        /// <param name="accountDetails"></param>
        /// <param name="lockUser"></param>
        /// <param name="isAllowedToLock"></param>
        /// <returns></returns>
        bool EnableDisableUser(ZnodeUser accountDetails, bool lockUser, out bool isAllowedToLock);

        /// <summary>
        /// This method will Get the accountId details based on Username
        /// </summary>
        /// <param name="username">Username to get user account.</param>
        /// <param name="portalId">Portal Id.</param>
        /// <param name="isSocialLogin">Is social login user.</param>
        /// <returns>Returns the Account details in AccountModel format</returns>
        UserModel GetUserByUsername(string username, int portalId, bool isSocialLogin = false);

        /// <summary>
        /// Create Admin User account.
        /// </summary>
        /// <param name="model">User Model</param>
        /// <returns>Returns newly created account model.</returns>
        UserModel CreateAdminUser(UserModel model);

        /// <summary>
        /// Get user account details by account id.
        /// </summary>
        /// <param name="userId">User Id to get user account data.</param>
        /// <param name="expands">Expands collection.</param>
        /// <param name="portalId">portalId.</param>
        /// <returns>Returns AccountModel.</returns>
        UserModel GetUserById(int userId, NameValueCollection expands, int portalId = 0);

        /// <summary>
        /// Update user account details.
        /// </summary>
        /// <param name="accountModel">User model.</param>
        /// <param name="webstoreUser">Flag to identify a webstore user.</param>
        /// <returns>Bool value according the status of update operation.</returns>
        bool UpdateUserData(UserModel userModel, bool webStoreUser);

        /// <summary>
        /// Get user account list.
        /// </summary>
        /// <param name="loggedUserAccountId">loggedUserAccountId</param>
        /// <param name="expands">Expands for Account.</param>
        /// <param name="filters">Filters for Account.</param>
        /// <param name="sorts">Sorts for Account.</param>
        /// <param name="page">Page.</param>
        /// <param name="columnList">Comma separated selected column list</param>
        /// <returns>Returns AccountListModel.</returns>
        UserListModel GetUserList(int loggedUserAccountId, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page, string columnList = "");

        /// <summary>
        /// Convert filter into XML format
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        string GenerateFilterXML(FilterCollection filters);
        /// <summary>
        /// Get user account list.
        /// </summary>
        /// <param name="loggedUserAccountId">loggedUserAccountId</param>
        /// <param name="userName">userName</param>
        /// <param name="roleName">roleName</param>
        /// <param name="expands">Expands for Account.</param>
        /// <param name="filters">Filters for Account.</param>
        /// <param name="sorts">Sorts for Account.</param>
        /// <param name="page">Page.</param>
        /// <param name="columnList">Comma separated selected column list</param>
        /// <returns>Returns AccountListModel.</returns>
        UserListModel GetUserListForAdmin(int loggedUserAccountId, string userName, string roleName, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page, string columnList = "");

        /// <summary>
        /// Get List of Sales Rep for Association
        /// </summary>
        /// <param name="expands">Expands for Sales rep</param>
        /// <param name="filters">Filters Collection</param>
        /// <param name="sorts">Sort Collection</param>
        /// <param name="page">Page</param>
        /// <returns></returns>
        UserListModel GetSalesRepListForAssociation(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);
        /// <summary>
        /// Delete user account.
        /// </summary>
        /// <param name="accountId">Account Ids to be deleted.</param>
        /// <returns>Returns true if record deleted successfully, else return false.</returns>
        bool DeleteUser(ParameterModel accountIds);

        /// <summary>
        /// Create customer account.
        /// </summary>
        /// <param name="portalId"></param>
        /// <param name="model">User Model</param>
        /// <returns>Returns created customer account.</returns>
        UserModel CreateCustomer(int portalId, UserModel model);

        /// <summary>
        /// Update customer account.
        /// </summary>
        /// <param name="userModel">User Model.</param>
        /// <returns>Returns true if record updated successfully, else return false.</returns>
        bool UpdateCustomer(UserModel userModel);

        /// <summary>
        /// Gets the assigned portals to user.
        /// </summary>
        /// <param name="aspNetUserId">User id.</param>
        /// <returns>Returns assigned portals to user.</returns>
        UserPortalModel GetPortalIds(string aspNetUserId);

        /// <summary>
        /// Save portal ids againt the user.
        /// </summary>
        /// <param name="userPortalModel">Model to save portal ids againt the user.</param>
        /// <returns>Returns true if saved successfully.</returns>
        bool SavePortalsIds(UserPortalModel userPortalModel);

        /// <summary>
        /// Sign up for news letter.
        /// </summary>
        /// <param name="model">NewsLetterSignUpModel containing email address.</param>
        /// <returns>Returns true if email successfully subscribed for news letter else false.</returns>
        bool SignUpForNewsLetter(NewsLetterSignUpModel model);

        /// <summary>
        /// Check default admin password reset.
        /// </summary>        
        /// <returns>true/false</returns>
        bool IsDefaultAdminPasswordReset();

        #region Social Login
        /// <summary>
        /// Login to the 3rd party like facebook, google etc.
        /// </summary>
        /// <param name="model">SocialLoginModel</param>
        /// <returns>Returns SocialLoginModel.</returns>
        UserModel SocialLogin(SocialLoginModel model);

        /// <summary>
        /// Get the social login providers.
        /// </summary>
        /// <returns>Returns list of social login providers.</returns>
        List<SocialDomainModel> GetLoginProviders();
        #endregion

        /// <summary>
        /// Get user access permission list
        /// </summary>
        /// <param name="userId"></param>
        /// <returns>Returns user access permission code</returns>
        UserModel GetUserAccessPermission(int userId);

        /// <summary>
        /// Get customer profile
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="portalId">portalId</param>
        /// <returns>Returns customer profile by particular userID</returns>
        List<ProfileModel> GetCustomerProfile(int userId, int portalId);

        /// <summary>
        /// This method will convert shopper to admin.
        /// </summary>
        /// <param name="portalId">integer Portal ID</param>
        /// <param name="model">AccountModel model</param>
        /// <returns>returns all the account model data</returns>
        UserModel ConvertShopperToAdmin(UserModel model);

        /// <summary>
        /// Get UnAssociated Customer(s).
        /// </summary>
        /// <param name="portalId">portalId</param>
        /// <param name="expands">Expands for Account.</param>
        /// <param name="filters">Filters for Account.</param>
        /// <param name="sorts">Sorts for Account.</param>
        /// <param name="page">Page Size.</param>        
        /// <returns>Returns all user list which are not associate any account.</returns>
        UserListModel GetUnAssociatedCustomerList(int portalId, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Update User Account Mapping
        /// </summary>        
        /// <param name="userModel">all user list data along with account Id</param>
        /// <returns>Returns Update status</returns>
        bool UpdateUserAccountMapping(UserAccountModel userModel);

        /// <summary>
        /// Insert the access permission for user.
        /// </summary>
        /// <param name="model">model</param>
        /// <param name="filters">filters</param>
        /// <returns></returns>
        bool InsertAccessPermissionForUsers(UserAccountModel model, FilterCollection filters);

        /// <summary>
        /// Update customer account.
        /// </summary>
        /// <param name="userModel">User Model.</param>
        /// <returns>Returns true if record updated successfully, else return false.</returns>
        bool UpdateCustomer(UserModel userModel, bool webStoreUser);

        /// <summary>
        /// Check and validate the username is an existing Shopper or not.
        /// </summary>
        /// <param name="username">Username</param>
        /// <returns>Returns true /false.</returns>
        bool CheckIsUserNameAnExistingShopper(string username);

        /// <summary>
        /// Get List of Sales Rep for Association for given Portal Id.
        /// </summary>
        /// <param name="expands">Expands for Sales rep</param>
        /// <param name="filters">Filters Collection</param>
        /// <param name="sorts">Sort Collection</param>
        /// <param name="page">Page</param>
        /// <returns></returns>
        SalesRepUserListModel GetSalesRepListForAccount(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Remove all data from User Register Attempts.
        /// </summary>
        /// <returns> Boolean </returns>
        bool ClearAllUserRegisterAttempts();

        /// <summary>
        /// Get user account details
        /// </summary>
        /// <param name="userId">userId</param>
        /// <returns>Returns user account details.</returns>
        UserModel GetCustomerAccountdetails(int userId);

        /// <summary>
        /// Get user account details
        /// </summary>
        /// <param name="loggedUserAccountId">loggedUserAccountId</param>
        /// <param name="filters">filters</param>
        /// <returns>Returns user list</returns>
        List<UserModel> GetUserListForWebstore(int loggedUserAccountId, FilterCollection filters);

        /// <summary>
        /// Get Portal Details
        /// </summary>
        /// <param name="portalId"></param>
        /// <returns></returns>
        ZnodePortal GetPortalDetails(int portalId);

        /// <summary>
        /// Update user data in user table.
        /// </summary>
        /// <param name="userModel"></param>
        /// <param name="webStoreUser"></param>
        /// <returns></returns>
        bool UpdateUser(UserModel userModel, bool webStoreUser);

        /// <summary>
        /// Get User Detail By Id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="portalId"></param>
        /// <returns></returns>
        UserModel GetUserDetailById(int userId, int portalId);

        /// <summary>
        /// This method is used to pay invoice.
        /// </summary>
        /// <param name="model">PayInvoiceModel model</param>
        /// <returns>OrderModel</returns>
        OrderModel PayInvoice(PayInvoiceModel payInvoiceModel);
        /// <summary>
        /// This method is used to get domain detail of portal.
        /// </summary>
        /// <param name="model">PortalModel model</param>
        /// <returns>PortalModel</returns>
        PortalModel GetPortalDomain(int portalId);


        /// <summary>
        /// Update the username of registered user.
        /// </summary>
        /// <param name="userDetailsModel">UserDetailsModel</param>
        /// <returns>True if username updated successfully</returns>
        BooleanModel UpdateUsernameForRegisteredUser(UserDetailsModel userDetailsModel);

        /// <summary>
        /// Returns SMSOptIn flag by user id.
        /// </summary>
        /// <param name="userId">userId</param>
        /// <returns>isSMSOptIn</returns>
        bool IsSMSOptIn(int userId);
    }
}
