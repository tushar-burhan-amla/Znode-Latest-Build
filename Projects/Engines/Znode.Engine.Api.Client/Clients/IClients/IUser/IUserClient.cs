using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IUserClient : IBaseClient
    {
        /// <summary>
        /// This method is used to logged in to the site
        /// </summary>
        /// <param name="model">AccountModel model</param>
        /// <returns>Returns the data for the logged in</returns>
        UserModel Login(UserModel model, ExpandCollection expands);

        /// <summary>
        /// This method is used to change/Reset the password
        /// </summary>
        /// <param name="model">AccountModel model</param>
        /// <returns>Returns changed data in AccountModel format</returns>
        UserModel ChangePassword(UserModel model);

        /// <summary>
        /// Function used for bulk reset password.
        /// </summary>
        /// <param name="model">Parameter model containing string of User Ids.</param>
        /// <returns>Returns true/false</returns>
        bool BulkResetPassword(ParameterModel userId);

        /// <summary>
        /// Function used for forgot password.
        /// </summary>
        /// <param name="model">AccountModel model</param>
        /// <returns>Returns forgot password details.</returns>
        UserModel ForgotPassword(UserModel model);

        /// <summary>
        /// This function will check the Reset Password Link current status.
        /// </summary>
        /// <param name="model">UserModel.</param>
        /// <returns>Returns Status of Reset Password Link.</returns>
        int? VerifyResetPasswordLinkStatus(UserModel model);

        /// <summary>
        /// This method will get the User details by user name
        /// </summary>
        /// <param name="username">User Name</param>
        /// <returns>Returns the account details in AccountModel format</returns>
        UserModel GetAccountByUser(string username);

        /// <summary>
        /// Create user account.
        /// </summary>
        /// <param name="userModel">User Model</param>
        /// <returns>Returns newly created account model.</returns>
        UserModel CreateUser(UserModel userModel);

        /// <summary>
        /// Get User account details by account id.
        /// </summary>
        /// <param name="UserId">User Id to get user account data.</param>
        /// <param name="portalId">portalId.</param>
        /// <returns>Returns AccountModel.</returns>
        UserModel GetUserAccountData(int userId, int portalId = 0);

        /// <summary>
        /// Get User account details by account id.
        /// </summary>
        /// <param name="UserId">User Id to get user account data.</param>
        /// <param name="expands">Expands for Account.</param>
        /// <param name="portalId">portalId</param>
        /// <returns>Returns AccountModel.</returns>
        UserModel GetUserAccountData(int userId, ExpandCollection expands, int portalId = 0);

        /// <summary>
        /// Get List of Sales Rep for Association
        /// </summary>
        /// <param name="filters">filter collection</param>
        /// <param name="sorts">sort collection</param>
        /// <param name="pageIndex">pageIndex</param>
        /// <param name="pageSize">pageSize</param>
        /// <returns></returns>
        UserListModel GetSalesRepListForAssociation(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Update user account details.
        /// </summary>
        /// <param name="model">User Model.</param>
        /// <param name="webstoreUser">Flag to identify a webstore user.</param>
        /// <returns>Returns updated account model.</returns>
        UserModel UpdateUserAccountData(UserModel model, bool webstoreUser);

        /// <summary>
        /// Get user account list.
        /// </summary>
        /// <param name="loggedUserAccountId">loggedUserAccountId</param>
        /// <param name="filters">Filters for account.</param>
        /// <param name="sorts">Sorts for Account.</param>
        /// <param name="pageIndex">Page Index.</param>
        /// <param name="pageSize">Size of page.</param>
        /// <returns>Returns AccountListModel.</returns>
        UserListModel GetUserAccountList(int loggedUserAccountId, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Delete user account.
        /// </summary>
        /// <param name="UserId">User Id in Parameter Model.</param>
        /// <returns>Returns true if account deleted successfully, else false.</returns>
        bool DeleteUser(ParameterModel userId);

        /// <summary>
        /// Enable disable account of user on the basis of account id.
        /// </summary>
        /// <param name="UserId">User Ids to enable disable user account.</param>
        /// <param name="lockUser">To lock or unlock account.</param>
        /// <returns>Returns true/false</returns>
        bool EnableDisableAccount(ParameterModel userId, bool lockUser);

        /// <summary>
        /// Create Customer Account.
        /// </summary>
        /// <param name="accountModel">User Model.</param>
        /// <returns>Returns created customer account.</returns>
        UserModel CreateCustomerAccount(UserModel accountModel);

        /// <summary>
        /// Update customer account.
        /// </summary>
        /// <param name="model">User Model.</param>
        /// <returns>Returns updated customer account model.</returns>
        UserModel UpdateCustomerAccount(UserModel model);

        /// <summary>
        /// Get customer account list.
        /// </summary>
        /// <param name="currentUserName">Logged in user name.</param>
        /// <param name="filters">Filters for account.</param>
        /// <param name="sorts">Sorts for Account.</param>
        /// <param name="pageIndex">Page Index.</param>
        /// <param name="pageSize">Size of page.</param>
        /// <param name="columnList">Comma separated selected column list</param>
        /// <returns>Returns AccountListModel.</returns>
        UserListModel GetCustomerAccountList(string currentUserName, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize, string columnList = "");


        /// <summary>
        /// Get UnAssociated Customer(s).
        /// </summary>
        /// <param name="portalId">Portal Id.</param>
        /// <param name="filters">Filters for account.</param>
        /// <param name="sorts">Sorts for Account.</param>
        /// <param name="pageIndex">Page Index.</param>
        /// <param name="pageSize">Size of page.</param>        
        /// <returns>Returns User List Model.</returns>
        UserListModel GetUnAssociatedCustomerList(int portalId, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);


        /// <summary>
        /// Update User Account Mapping
        /// </summary>
        /// <param name="userModel">User Account model</param>
        /// <returns>Returns true if updated successfully.</returns>
        bool UpdateUserAccountMapping(UserAccountModel userModel);


        /// <summary>
        /// Get customer account list.
        /// </summary>
        /// <param name="currentUserName">Logged in user name.</param>
        /// <param name="loggedUserAccountId">Logged in user id.</param>
        /// <param name="filters">Filters for account.</param>
        /// <param name="sorts">Sorts for Account.</param>
        /// <param name="pageIndex">Page Index.</param>
        /// <param name="pageSize">Size of page.</param>
        /// <param name="columnList">Comma separated selected column list</param>
        /// <returns>Returns AccountListModel.</returns>
        UserListModel GetCustomerListForAdmin(string currentUserName, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize, string columnList = "", int loggedUserAccountId = 0, string roleName = null);
        
        /// <summary>
        /// Get customer account list.
        /// </summary>
        /// <param name="userAdminModel">userAdminModel.</param>
        /// <param name="filters">Filters for account.</param>
        /// <param name="sorts">Sorts for Account.</param>
        /// <param name="pageIndex">Page Index.</param>
        /// <param name="pageSize">Size of page.</param>
        /// <returns>Returns customer list model.</returns>
        UserListModel GetCustomersForAdmin(AdminUserModel userAdminModel, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get B2B customer account list.
        /// </summary>
        /// <param name="expands"></param>
        /// <param name="filters"></param>
        /// <param name="sorts"></param>
        /// <param name="pageIndex"></param>
        /// <param name="pageSize"></param>
        /// <returns></returns>
        UserListModel GetAccountList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Gets the assigned portals to user.
        /// </summary>
        /// <param name="aspNetUserId">User id.</param>
        /// <returns>Returns assigned portals to user.</returns>
        UserPortalModel GetPortalIds(string aspNetUserId);

        /// <summary>
        /// Save portal ids againt the user.
        /// </summary>
        /// <param name="model">Model to save portal ids againt the user.</param>
        /// <returns>Returns the user portal model.</returns>
        UserPortalModel SavePortalsIds(UserPortalModel model);

        /// <summary>
        /// Sign up for news letter.
        /// </summary>
        /// <param name="model">NewsLetterSignUpModel containing email address.</param>
        /// <returns>Returns true if email successfully subscribed for news letter else false.</returns>
        bool SignUpForNewsLetter(NewsLetterSignUpModel model);

        /// <summary>
        /// check default admin password is reset.
        /// </summary>
        /// <returns>true/false</returns>
        bool IsDefaultAdminPasswordReset();

        /// <summary>
        /// Convert shopper to admin User User.
        /// </summary>
        /// <param name="model">AccountModel model</param>
        /// <returns>Returns user details.</returns>
        UserModel ConvertShopperToAdmin(UserModel model);

        /// <summary>
        /// Update customer account.
        /// </summary>
        /// <param name="model">User Model.</param>
        /// <returns>Returns updated customer account model.</returns>
        UserModel UpdateCustomer(UserModel model);


        #region Social Login
        /// <summary>
        /// Login to the 3rd party like facebook, google etc.
        /// </summary>
        /// <param name="model">SocialLoginModel</param>
        /// <returns>Returns UserModel after login.</returns>
        UserModel SocialLogin(SocialLoginModel model);

        /// <summary>
        /// Update Billing Account Number global attribute and Quote status
        /// </summary>
        /// <param name="entityAttributeModel"></param>
        /// <returns></returns>
        EntityAttributeModel UpdateUserAndQuoteDetails(EntityAttributeModel entityAttributeModel);

        /// <summary>
        /// Validate CSR Token 
        /// </summary>
        /// <param name="token"></param>
        /// <returns name="ImpersonationModel"></returns>
        ImpersonationAPIModel ValidateCSRToken(string token);

        /// <summary>
        /// Check and validate the username is an existing Shopper or not.
        /// </summary>
        /// <param name="username"> Username </param>
        /// <returns>true/false</returns>
        bool CheckIsUserNameAnExistingShopper(string username);

        /// <summary>
        /// Get List of Sales Rep for Association
        /// </summary>
        /// <param name="filters">filter collection</param>
        /// <param name="sorts">sort collection</param>
        /// <param name="pageIndex">pageIndex</param>
        /// <param name="pageSize">pageSize</param>
        /// <returns></returns>
        SalesRepUserListModel GetSalesRepListForAccount(FilterCollection filters, SortCollection sortCollection, int? pageIndex, int? recordPerPage);
        #endregion

        /// <summary>
        /// Get customer account details
        /// </summary>
        /// <param name="userId">Account userId</param>
        /// <returns>Returns UserModel</returns>
        UserModel GetCustomerAccountDetails(int userId);

        /// <summary>
        /// Get User Detail By Id
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="portalId"></param>
        /// <returns></returns>
        UserModel GetUserDetailById(int userId, int portalId);

        /// <summary>
        /// Get user order list of account.
        /// </summary>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="pageIndex">Current index of page.</param>
        /// <param name="pageSize">record per page.</param>
        /// <param name="accountId">Account Id</param>
        /// <returns>Returns user order list of account.</returns>
        OrdersListModel GetAccountUserOrderList(int accountId, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// This method is used to pay invoice
        /// </summary>
        /// <param name="model">PayInvoiceModel model</param>
        /// <returns>Returns the data for the payed invoice</returns>
        OrderModel PayInvoice(PayInvoiceModel payInvoiceModel);

        /// <summary>
        /// To update the username of registered user.
        /// </summary>
        /// <param name="userDetailsModel">UserDetailsModel</param>
        /// <returns>True if username updated successfully</returns>
        bool UpdateUsernameForRegisteredUser(UserDetailsModel userDetailsModel);
    }
}
