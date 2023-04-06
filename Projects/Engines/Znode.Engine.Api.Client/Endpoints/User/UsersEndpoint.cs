namespace Znode.Engine.Api.Client.Endpoints
{
    //Configure the Endpoints used to create URLs for User related APIs
    public class UsersEndpoint : BaseEndpoint
    {
        // Get Account by Account Id.
        public static string Get(int accountId) => $"{ApiRoot}/users/{accountId}";

        // Login.
        public static string Login() => $"{ApiRoot}/users/login";

        // Change Password.
        public static string ChangePassword() => $"{ApiRoot}/users/changepassword";

        // Bulk Reset Password.
        public static string BulkResetPassword() => $"{ApiRoot}/users/bulkresetpassword";

        // Forgot Password.
        public static string ForgotPassword() => $"{ApiRoot}/users/forgotpassword";

        // Verify Reset Password Link Status
        public static string VerifyResetPasswordLinkStatus() => $"{ApiRoot}/users/verifyresetpasswordlinkstatus";

        // Get account details by user name.
        public static string GetByUsername() => $"{ApiRoot}/users/getbyusername";

        // Create user.
        public static string CreateUser() => $"{ApiRoot}/users/createusers";

        // Get user account data by account Id.
        public static string GetUserAccountData(int accountId, int portalId = 0) => $"{ApiRoot}/useraccounts/{accountId}/{portalId}";

        // Update user account.
        public static string UpdateUserAccountData(bool webStoreUser) => $"{ApiRoot}/useraccount/update/{webStoreUser}";

        // Update the username of registered user.
        public static string UpdateUsernameForRegisteredUser() => $"{ApiRoot}/username/updateusernameforregistereduser";
        
        //Get user account list on the basis of logged User Account Id.
        public static string GetUserAccountList(int loggedUserAccountId) => $"{ApiRoot}/useraccounts/list/{loggedUserAccountId}";

        //Delete user account.
        public static string Delete() => $"{ApiRoot}/useraccounts/delete";

        //Get the List of Sales Rep for Association
        public static string GetSalesRepListForAssociation() => $"{ApiRoot}/users/GetSalesRepListForAssociation";

        // Enable disable user account on basis of lockUser status.
        public static string EnableDisableAccount(bool lockUser) => $"{ApiRoot}/useraccounts/enabledisable/{lockUser}";

        // Create customer account.
        public static string CreateCustomerAccount() => $"{ApiRoot}/users/createcustomeraccount";

        // Update customer account.
        public static string UpdateCustomerAccount() => $"{ApiRoot}/customeraccount/update";

        //Get customer account list on the basis of logged User Account Id.
        public static string GetCustomerAccountList(int loggedUserAccountId) => $"{ApiRoot}/customeraccount/list/{loggedUserAccountId}";

        //Get customer account list on the basis of Portal Id.
        public static string GetUnAssociatedCustomerList(int portalId) => $"{ApiRoot}/unassociatedcustomeraccount/list/{portalId}";

        //Update user Account Mapping
        public static string UpdateUserAccountMapping() => $"{ApiRoot}/users/updateusersaccountmapping";

        //Get customer account list on the basis of logged User Account Id,current User Name and role Name.
        public static string GetCustomerAccountListForAdmin(int loggedUserAccountId, string currentUserName, string roleName) => $"{ApiRoot}/customeraccount/list/{loggedUserAccountId}/{currentUserName}/{roleName}";

        //Get customer account list on the basis of logged User Account Id,current User Name and role Name.
        public static string GetCustomerListForAdmin() => $"{ApiRoot}/customeraccount/getcustomerlistforadmin";

        //Get B2B customer account list.
        public static string GetAccountList() => $"{ApiRoot}/users/list";

        //Gets the assigned portals to user.
        public static string GetPortalIds(string aspNetUserId) => $"{ApiRoot}/users/getportalids/{aspNetUserId}";

        //Save portal ids againt the user.
        public static string SavePortalsIds() => $"{ApiRoot}/users/saveportalsids";

        //Sign up for news letter.
        public static string SignUpForNewsLetter() => $"{ApiRoot}/users/signupfornewsletter";

        // Check default admin password is reset.
        public static string IsDefaultAdminPasswordReset() => $"{ApiRoot}/users/isdefaultadminpasswordreset";

        // Convert shopper to admin.
        public static string ConvertShopperToAdmin() => $"{ApiRoot}/users/convertshoppertoadmin";

        // Update customer account.
        public static string UpdateCustomer() => $"{ApiRoot}/customeraccount/updatecustomer";


        #region Social Login
        // Social Login.
        public static string SocialLogin() => $"{ApiRoot}/users/sociallogin";
        #endregion

        #region Billing Account Number
        //Update Billing Account Number and Quote status
        public static string UpdateUserAndQuoteDetails() => $"{ApiRoot}/users/updateuserandquotedetails";
        #endregion

        #region Impersonation
        public static string ValidateCSRToken() => $"{ApiRoot}/users/validatecsrtoken";
        #endregion

        //Check and validate the UserName is an existing Shopper or not.
        public static string CheckIsUserNameAnExistingShopper() => $"{ApiRoot}/users/checkisusernameanexistingshopper";

        //Get the List of Sales Rep for Association
        public static string GetSalesRepListForAccount() => $"{ApiRoot}/users/GetSalesRepListForAccount";

        //Get customer account details on the basis of userId.
        public static string GetCustomerAccountDetails(int userId) => $"{ApiRoot}/users/getcustomeraccountdetails/{userId}";

        // Get user data by user Id.
        public static string GetUserDetailById(int userId, int portalId) => $"{ApiRoot}/user/getuserdetailbyid/{userId}/{portalId}";

        //Pay Invoice
        public static string PayInvoice() => $"{ApiRoot}/users/payinvoice";
    }
}
