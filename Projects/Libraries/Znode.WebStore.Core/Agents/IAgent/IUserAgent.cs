using Microsoft.AspNet.Identity.Owin;
using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.WebStore.Models;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.WebStore.Agents
{
    public interface IUserAgent
    {

        /// <summary>
        /// Login Method to Authenticate the User Credentials.
        /// </summary>
        /// <param name="model">LoginViewModel</param>
        /// <returns>Return the User Details in LoginViewModel Model</returns>
        LoginViewModel Login(LoginViewModel model);

        /// <summary>
        /// Login to create Authenticated session for the user specified by the launch token.
        /// </summary>
        /// <param name="launchToken">Customer launch token.</param>
        /// <returns>Return the User Details in LoginViewModel Model</returns>
        LoginViewModel ImpersonationLogin(string token);

        /// <summary>
        /// Log out the current logged in user.
        /// </summary>
        void Logout();

        /// <summary>
        /// Function used to Change/Reset the user password.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Returns the Change/Reset password details.</returns>
        ChangePasswordViewModel ChangePassword(ChangePasswordViewModel model);

        /// <summary>
        ///Save referral user id in session. 
        /// </summary>
        /// <param name="affiliateId">referral user id.</param>
        void SetAffiliateId(string affiliateId);

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
        /// Create the Customer.
        /// </summary>
        /// <param name="model">RegisterViewModel</param>
        /// <returns>Return the Customer Details in RegisterViewModel.</returns>
        RegisterViewModel SignUp(RegisterViewModel model);

        /// <summary>
        ///  Get available profile list for user.
        /// </summary>
        /// <param name="profiles"></param>
        /// <returns></returns>
        List<SelectListItem> GetProfiles(List<ProfileModel> profiles);

        /// <summary>
        /// Sign Up the User for NewsLetters.
        /// </summary>
        /// <param name="model">NewsLetterSignUpViewModel</param>
        /// <param name="message">Message</param>
        /// <returns>returns true/false</returns>
        bool SignUpForNewsLetter(NewsLetterSignUpViewModel model, out string message);

        /// <summary>
        /// Get Address information on the basis of Address Id.
        /// </summary>
        /// <param name="addressId">addressId</param>        
        /// <returns>model</returns>
        AddressViewModel GetAddress(int? addressId);

        /// <summary>
        /// Create address.
        /// </summary>
        /// <param name="addressViewModel">Address ViewModel.</param>
        /// <returns>Returns created model.</returns>
        AddressViewModel CreateUpdateAddress(AddressViewModel addressViewModel, string addressType = null, bool isShippingBillingDifferent = false, bool isCreateAccountForGuestUser = false);

        /// <summary>
        /// Get address for User.
        /// </summary>
        /// <param name="isAddressBook">IsAddressBook flag.</param>
        /// <returns>AddressListViewModel</returns>
        AddressListViewModel GetAddressList(bool isAddressBook = false);

        /// <summary>
        /// Get address for User.
        /// </summary>
        /// <param name="isAddressBook">IsAddressBook flag.</param>
        /// <returns>AddressListViewModel</returns>
        AddressListViewModel GetAddressList(int userId, bool isAddressBook = false);

        /// <summary>
        /// Updated the address based on the address id and the setting the Default billing / shipping 
        /// </summary>           
        /// <returns>model</returns>
        AddressViewModel UpdateAddress(int addressid, bool isDefaultBillingAddress);

        /// <summary>
        /// Delete Address on the basis of Address Id.
        /// </summary>
        /// <param name="addressId">addressId</param> 
        /// <param name="addressViewModel">addressViewModel</param>
        /// <returns>Returns status</returns>
        bool DeleteAddress(int? addressId, AddressViewModel addressViewModel, out string message);

        /// <summary>
        /// Create user product add to wishlist.
        /// </summary>
        /// <param name="productSKU">product sku</param>
        /// <returns>true or false</returns>
        bool CreateWishList(string productSKU, string AddOnProductSKUs);

        /// <summary>
        /// Get wishlisted products.
        /// </summary>
        /// <returns>Return WishListListViewModel.</returns>
        WishListListViewModel GetWishLists();

        /// <summary>
        /// gets the anonymous user address
        /// </summary>
        /// <returns>returns address</returns>
        AddressListViewModel GetAnonymousUserAddress();

        /// <summary>
        /// Delete wishlist against a user.
        /// </summary>
        /// <param name="wishListId">WishList Id.</param>
        /// <returns>Returns true if wishlist deleted successfully else return false.</returns>
        bool DeleteWishList(int wishListId);

        /// <summary>
        /// Get current user details.
        /// </summary>
        /// <returns>UserViewModel</returns>
        UserViewModel GetUserViewModelFromSession();

        /// <summary>
        ///Get Current login user product review list.
        /// </summary>
        /// <returns>List of product reviews.</returns>
        List<ProductReviewViewModel> GetProductReviewList();

        List<SelectListItem> GetCountries(bool flag = true);

        /// <summary>
        /// Update user profile data.
        /// </summary>
        /// <param name="model">UserViewModel.</param>
        /// <param name="webStoreUser">Flag to identify a webstore user.</param>
        /// <returns>Returns updated user profile.</returns>
        UserViewModel UpdateProfile(UserViewModel model, bool webStoreUser);

        /// <summary>
        /// Get order history list against a user.
        /// </summary>
        /// <param name="filters">Filters.</param>
        /// <param name="sortCollection">Sort.</param>
        /// <param name="pageIndex">Page index.</param>
        /// <param name="recordPerPage">Record per page.</param>
        /// <returns>Returns list of orders.</returns>
        OrdersListViewModel GetOrderList(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Get user dashboard data to display.
        /// </summary>
        /// <returns>Returns UserViewModel containing user dashboard data.</returns>
        UserViewModel GetDashboard();

        /// <summary>
        /// Get reorder items by order id.
        /// </summary>
        /// <param name="orderId">Order Id.</param>
        /// <returns>Returns list of cart item model.</returns>
        List<CartItemViewModel> GetReorderItems(int orderId);

        /// <summary>
        /// Get order details to generate order receipt.
        /// </summary>
        /// <param name="orderId">Order id.</param>
        /// <param name="portaId"></param>
        /// <returns>Returns order details in OrdersViewModel.</returns>
        OrdersViewModel GetOrderDetails(int orderId, int portaId = 0);

        /// <summary>
        /// Get Order details on the basis of User's Order Number, Firstname and Lastname.
        /// </summary>
        /// <param name="orderNumber">orderNumber of order placed.</param>
        /// <param name="firstName">firstName of user.</param>
        /// <param name="lastName">lastName of user.</param>
        /// <param name="emailAddress">emailAddress of user.</param>
        /// <returns>Returns order details in OrdersViewModel.</returns>
        OrdersViewModel GetOrderDetails(string orderNumber, string firstName, string lastName, string emailAddress);

        /// <summary>
        /// Create single order line item.
        /// </summary>
        /// <param name="orderModel"></param>
        /// <param name="orderLineItemListModel"></param>
        void CreateSingleOrderLineItem(OrderModel orderModel, List<OrderLineItemModel> orderLineItemListModel);

        /// <summary>
        /// Get order data to reorder single product.
        /// </summary>
        /// <param name="orderLineItemId">Order line item id.</param>
        /// <returns>Returns CartItemViewModel.</returns>
        CartItemViewModel GetOrderByOrderLineItemId(int orderLineItemId);

        /// <summary>
        /// Get Account Information.
        /// </summary>
        /// <returns>AccountViewModel</returns>
        AccountViewModel GetAccountInformation();

        /// <summary>
        /// Update customer address.
        /// </summary>
        /// <param name="addressViewModel">addressViewModel</param>
        /// <returns></returns>
        AddressViewModel UpdateAccountInformation(AddressViewModel addressViewModel);

        #region Quote History
        /// <summary>
        /// Get quote list.
        /// </summary>
        /// <param name="filters">Filters.</param>
        /// <param name="sortCollection">Sort.</param>
        /// <param name="pageIndex">Page index.</param>
        /// <param name="recordPerPage">Record per page.</param>
        /// <param name="isPendingPayment">Is pending payment flag.</param>
        /// <returns>Returns list of quote.</returns>
        AccountQuoteListViewModel GetAccountQuoteList(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null, bool isPendingPayment = false);

        /// <summary>
        /// Update multiple quote status.
        /// </summary>
        /// <param name="quoteId">Quote ids to be updated.</param>
        /// <param name="status">Quote status.</param>
        /// <returns></returns>
        bool UpdateQuoteStatus(string quoteId, int status);

        /// <summary>
        /// Get Quote View by omsQuoteId.
        /// </summary>
        /// <param name="omsQuoteId">omsQuoteId</param>
        /// <param name="IsQuoteLineItemUpdated">IsQuoteLineItemUpdated if true update quote total.</param>
        /// <returns></returns>
        AccountQuoteViewModel GetQuoteView(int omsQuoteId, bool IsQuoteLineItemUpdated = false);

        /// <summary>
        /// Update the Account Quote Details.
        /// </summary>
        /// <param name="accountQuoteViewModel">accountQuoteViewModel</param>
        /// <returns></returns>
        bool UpdateQuoteStatus(AccountQuoteViewModel accountQuoteViewModel);

        /// <summary>
        /// Update Quote Line Item Quantity.
        /// </summary>
        /// <param name="cartItemViewModel">cartItemViewModel</param>
        /// <returns>If updated returns true.</returns>
        bool UpdateQuoteLineItemQuantity(CartItemViewModel cartItemViewModel);

        /// <summary>
        /// Delete Quote Line Item.
        /// </summary>
        /// <param name="omsQuoteLineItemId">omsQuoteLineItemId</param>
        /// <param name="omsQuoteId">omsQuoteId to delete record.</param>
        /// <returns>true if deleted else false.</returns>
        bool DeleteQuoteLineItem(int omsQuoteLineItemId, int omsQuoteId = 0);

        /// <summary>
        /// Deletes all items from shopping cart and add quote line items to shopping cart.
        /// </summary>
        /// <param name="accountQuoteViewModel"></param>
        /// <returns></returns>
        bool AddQuoteToCart(AccountQuoteViewModel accountQuoteViewModel);

        /// <summary>
        /// Create/Update Quotes.
        /// </summary>
        /// <param name="model">SubmitQuoteViewModel.</param>
        /// <param name="message">Message.</param>
        /// <returns>Returns create quote.</returns>
        bool CreateQuote(SubmitQuoteViewModel submitQuoteViewModel, out string message);
        #endregion

        /// <summary>
        /// Generate order invoice.
        /// </summary>
        /// <param name="orderIds">Selected order ids.</param>
        /// <returns>OrdersListViewModel.</returns>
        OrdersListViewModel GetOrderInvoiceDetails(string orderIds);

        /// <summary>
        /// Get billing address detail.
        /// </summary>
        /// <param name="billingAddressId"> Selected Billing address Id</param>
        /// <param name="shippingAddressId"> Selected Billing address Id</param>
        /// <returns>AddressListViewModel</returns>
        AddressListViewModel GetBillingAddressDetail(int billingAddressId, int shippingAddressId);

        /// <summary>
        /// Login to the 3rd party like facebook, google etc.
        /// </summary>
        /// <param name="loginInfo">External login info to login.</param>
        /// <param name="isPersistent">Is persistent.</param>
        /// <returns>Returns LoginViewModel.</returns>
        LoginViewModel SocialLogin(ExternalLoginInfo loginInfo, bool isPersistent, string username = null);

        /// <summary>
        /// Logout from social login.
        /// </summary>
        /// <param name="loginInfo">Login information</param>
        /// <returns>Return url to redirect.</returns>
        string Logout(ExternalLoginInfo loginInfo);

        /// <summary>
        /// Remove guest user details from session.
        /// </summary>
        void RemoveGuestUserSession();

        /// <summary>
        /// Get recommended address list.
        /// </summary>
        /// <param name="addressViewModel"></param>
        /// <returns>AddressListViewModel</returns>
        AddressListViewModel GetRecommendedAddress(AddressViewModel addressViewModel);

        /// <summary>
        /// Get states by country code.
        /// </summary>
        /// <param name="countryCode"></param>
        /// <returns></returns>
        List<SelectListItem> GetStates(string countryCode);

        /// <summary>
        /// Get shipping billing addresses.
        /// </summary>
        /// <returns>AddressListViewModel</returns>
        AddressListViewModel GetshippingBillingAddress();

        /// <summary>
        /// Get the login provider details from database.
        /// </summary>
        /// <returns>SocialModel</returns>
        SocialModel GetLoginProviders();

        /// <summary>
        /// Get Authorization Header
        /// </summary>
        /// <param name="domainName"></param>
        /// <param name="domainKey"></param>
        /// <param name="endpoint"></param>
        /// <returns></returns>
        string GetAuthorizationHeader(string domainName, string domainKey, string endpoint = "");

        /// <summary>
        /// Remove pending orders count and pending payments count from session.
        /// </summary>
        /// <param name="userId">UserId of the logged in user</param>
        void ClearUserDashboardPendingOrderDetailsCountFromSession(int userId);

        #region Template      
        /// <summary>
        /// Is template name exists.
        /// </summary>
        /// <param name="templateName">Template name to check whether it exists.</param>
        /// <param name="omsTemplateId">OmsTemplateId</param>
        /// <returns>Returns true if exists else false.</returns>
        bool IsTemplateNameExist(string templateName, int omsTemplateId = 0);
        #endregion


        /// <summary>
        /// Get customer account list.
        /// </summary>
        /// <param name="currentUserName">User name of logged in user.</param>
        /// <param name="filters">Filters for Account.</param>
        /// <param name="sorts">Sorts for Account.</param>
        /// <param name="page">Page.</param>
        /// <returns>Returns AccountListModel.</returns>
        CustomerListViewModel GetCustomerAccountList(string currentUserName, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Get the details of accounts customer.
        /// </summary>
        /// <param name="userId">User id.</param>
        /// <returns>Returns details of customer.</returns>
        CustomerAccountViewModel GetAccountsCustomer(int userId);

        /// <summary>
        /// Update the accounts customer.
        /// </summary>
        /// <param name="model">Model to update in database.</param>
        /// <param name="errorMessage">Error message</param>
        /// <returns>Returns true if updated successfully.</returns>
        bool UpdateCustomerAccount(CustomerAccountViewModel model, out string errorMessage);

        /// <summary>
        /// Set the customer account view model.
        /// </summary>
        /// <param name="model">Model to set the values.</param>
        /// <param name="accountId">Account Id to get the list.</param>
        void SetCustomerAccountViewModel(CustomerAccountViewModel model, int accountId);

        /// <summary>
        /// Get List of Active Countries.
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        List<SelectListItem> GetAccountDepartmentList(int accountId);

        /// <summary>
        /// Gets the list of b2b roles.
        /// </summary>
        /// <param name="customerAccountViewModel"></param>
        /// <returns></returns>
        List<SelectListItem> GetAccountRoleList(CustomerAccountViewModel customerAccountViewModel);

        /// <summary>
        /// Delete customer account.
        /// </summary>
        /// <param name="UserIds">User Ids id of customer.</param>
        /// <param name="currentUserName">Logged in user name.</param>
        /// <param name="errorMessage">error message</param>
        /// <returns>Returns true if account deleted successfully, else return false.</returns>
        bool DeleteCustomer(string UserIds, string currentUserName, out string errorMessage);

        /// <summary>
        /// Enable Disable customer accounts.
        /// </summary>
        /// <param name="userId">User Ids whose account has to be enabled or disabled.</param>
        /// <param name="lockUser">To lock or unlock customer account.</param>
        /// <param name="errorMessage">error message</param>
        /// <returns>Returns true or false.</returns>
        bool EnableDisableUser(string userId, bool lockUser, out string errorMessage);

        /// <summary>
        /// Get b2b permission list.
        /// </summary>
        /// <returns>Returns list of b2b permission.</returns>
        string GetPermissionList(int accountId, int accountPermissionId);

        /// <summary>
        /// Get the html for drop down of account permission list.
        /// </summary>
        /// <param name="accountId"></param>
        /// <param name="accountPermissionId"></param>
        /// <returns></returns>
        string AccountPermissionList(int accountId, int? accountPermissionId);

        /// <summary>
        /// Get the grid model for binding the tools.
        /// </summary>
        /// <returns></returns>
        GridModel GetGridModel();

        /// <summary>
        /// Get account department list.
        /// </summary>
        /// <param name="filters"></param>
        /// <param name="sorts"></param>
        /// <param name="pageIndex"></param>
        /// <param name="recordPerPage"></param>
        /// <returns></returns>
        AccountDepartmentListViewModel GetAccountDepartments(FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Get the Account Details based of the Account Id.
        /// </summary>
        /// <param name="accountId"></param>
        /// <returns></returns>
        AccountDataViewModel GetAccountById(int accountId = 0);

        /// <summary>
        /// Set filters for account id.
        /// </summary>
        /// <param name="filters"></param>
        /// <param name="accountId"></param>
        void SetFiltersForAccountId(FilterCollection filters, int accountId);

        /// <summary>
        /// Get department list based on account id.
        /// </summary>
        /// <param name="accountId">accountId to get department list.</param>
        /// <returns>Returns list of department list.</returns>
        List<SelectListItem> GetAccountDepartments(int accountId);

        /// <summary>
        /// Get approver list based on account id.
        /// </summary>
        /// <param name="accountId">accountId to get approver list.</param>
        /// <param name="userId">userId</param>
        /// <param name="currentUserName">currentUserName to get approver list.</param>
        /// <returns>Returns approver list.</returns>
        List<SelectListItem> GetApproverList(int accountId, int? userId, string currentUserName);

        /// <summary>
        /// Reset password functionality for single account.
        /// </summary>
        /// <param name="userId">userId whose password has to be reset.</param>
        /// <param name="errorMessage">Error Message.</param>
        /// <returns>Returns UsersViewModel</returns>
        bool ResetPassword(int userId, out string errorMessage);

        /// <summary>
        /// Reset password of the user account.
        /// </summary>
        /// <param name="userId">userId whose password has to be reset.</param>
        /// <param name="errorMessage">Error Message.</param>
        /// <returns>Returns true/false.</returns>
        bool BulkResetPassword(string userId, out string errorMessage);

        /// <summary>
        /// Get address details.
        /// </summary>
        /// <param name="addressViewModel">Address ViewModel.</param>
        /// <returns>Returns model with data.</returns>
        AddressViewModel GetAddressDetail(AddressViewModel addressViewModel);

        /// <summary>
        /// Get the list of user approvers.
        /// </summary>
        /// <param name="omsQuoteId">Oms quote Id.</param>
        /// <param name="showAllApprovers">Flag to decide whether to show all approvers or filtered approvers on the basis of their status and budget amount.</param>
        /// <returns>Returns list of user approvers.</returns>
        UserApproverListViewModel GetUserApproverList(int omsQuoteId, bool showAllApprovers);

        /// <summary>
        /// Convert quote to order.
        /// </summary>
        /// <param name="accountQuoteViewModel">AccountQuoteViewModel model.</param>
        /// <returns>Returns given quote to order.</returns>
        OrdersViewModel ConvertToOrder(AccountQuoteViewModel accountQuoteViewModel);

        /// <summary>
        /// Validate User Budget.
        /// </summary>
        /// <param name="message">Message.</param>
        /// <returns>Returns Budget is Valid.</returns>
        bool ValidateUserBudget(out string message);

        /// <summary>
        /// Get User account details by account id.
        /// </summary>
        /// <param name="UserId">User Id to get user account data.</param>
        /// <param name="portalId">portalId.</param>
        /// <returns>Returns AccountModel.</returns>
        UserViewModel GetUserAccountData(int UserId, int portalId = 0);

        /// <summary>
        /// to set login user profile Id
        /// </summary>
        /// <param name="userModel"></param>
        void SetLoginUserProfile(UserModel userModel);

        /// <summary>
        ///  Create cart from existing single line items
        /// </summary>
        /// <param name="orderId">Always remain 0.</param>
        /// <param name="OmsOrderLineItemsId">item orderline item id.</param>
        void ReordersingleLineOrderItem(int omsOrderLineItemsId);

        /// <summary>
        /// To update the user profile Id in the user session
        /// </summary>
        /// <param name="profileId">int profileId</param>
        /// <returns>returns true if success else false.</returns>
        bool ChangeUserProfile(int profileId);

        /// <summary>
        /// Gets the address from addressId and type specified.
        /// </summary>
        /// <param name="addressId">Address id of the address to get.</param>
        /// <param name="type">Type of address to get. Shipping or Billing</param>
        /// <returns>returns AddressViewModel</returns>
        AddressViewModel GetAddressByAddressType(int? addressId, string type);

        /// <summary>
        /// Get State list by Country Code
        /// </summary>
        /// <param name="countryCode"></param>
        /// <returns></returns>
        List<SelectListItem> GetStateListByCountryCode(string countryCode);

        /// <summary>
        ///  After login it returns string.
        /// </summary>
        /// <param name="loginViewModel">loginViewModel</param>
        /// <returns>Returns string</returns>
        string GetReturnUrlAfterLogin(LoginViewModel loginViewModel);

        /// <summary>
        /// Create cart from existing order
        /// </summary>
        /// <param name="orderId">Order Id.</param>
        /// <returns></returns>
        void ReorderCompleteOrder(int orderId);

        /// <summary>
        /// Get user dashboard data to display.
        /// </summary>
        /// <returns>Returns UserViewModel containing user dashboard data.</returns>
        UserViewModel GetAccountMenus();

        /// <summary>
        /// Set expands for receipt.
        /// </summary>
        ExpandCollection SetExpandsForReceipt();

        /// <summary>
        /// Get order details by id.
        /// </summary>
        /// <param name="orderId">order id </param>
        /// <param name="filters">filters</param>
        /// <returns>Order model.</returns>
        OrderModel GetOrderBasedOnPortalId(int orderId, FilterCollection filters);

        /// <summary>
        /// Returns vouchers list.
        /// </summary>
        /// <param name="filters">filters</param>
        /// <param name="sortCollection">sortCollection</param>
        /// <param name="pageIndex">pageIndex</param>
        /// <param name="recordPerPage">recordPerPage</param>
        /// <returns> returns VoucherListView Model containing voucher list</returns>
        VoucherListViewModel GetVouchers(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        ///  Returns Voucher history list.
        /// </summary>
        /// <param name="voucherId"> Voucher Id</param>
        /// <param name="filters">filters Collection</param>
        /// <param name="sortCollection">sort Collection</param>
        /// <param name="pageIndex">Page Index</param>
        /// <param name="recordPerPage">recordPerPage</param>
        /// <returns> returns VoucherHistoryListView Model containing voucher history list</returns>
        VoucherHistoryListViewModel GetVoucherHistoryList(int voucherId, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Get order details by order id.
        /// </summary>
        /// <param name="filters">Filter Collection</param>
        /// <param name="sortCollection">SortCollection</param>
        /// <param name="pageIndex">Current index of page.</param>
        /// <param name="recordPerPage">record per page.</param>
        /// <param name="accountId">Account Id</param>
        /// <returns>OrdersListViewModel</returns>
        OrdersListViewModel GetAccountUserOrderList(int accountId, int userId, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);


        /// <summary>
        /// This method is used to paying invoice.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>OrdersViewModel.</returns>
        OrdersViewModel PayInvoice(PayInvoiceViewModel payInvoiceModel);
    }
}