using System.Collections.Generic;
using Znode.Engine.Api.Models;
using Znode.Engine.WebStore.Models;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.WebStore.Agents
{
    public interface ICheckoutAgent
    {
        /// <summary>
        /// Get the list of shipping options.
        /// </summary>
        /// <returns>list of shipping options.</returns>
        ShippingOptionListViewModel GetShippingOptions(string shippingTypeName = null, bool isQuote = false);

        /// <summary>
        /// Get the list of payment options.
        /// </summary>
        /// <returns>list of payment options.</returns>
        List<BaseDropDownOptions> PaymentOptions(bool isUsedForOfflinePayment = false, bool isQuotes = false);

        /// <summary>
        /// Set users's selected shipping billing addresses.
        /// </summary>
        /// <param name="addressId">Id of address.</param>
        /// <param name="userId">Id of user.</param>
        /// <param name="type">Address type (shipping/Billing).</param>
        void SetBillingShippingAddress(int addressId, int OtherAddressId, int userId, string type);


        /// <summary>
        /// Create guest user account.
        /// </summary>
        /// <param name="address">Address</param>
        /// <param name="emailAddress"> Email address</param>
        /// <returns></returns>
        UserViewModel CreateAnonymousUserAccount(AddressModel address, string emailAddress);


        /// <summary>
        /// Get addresses of guest users.
        /// </summary>
        /// <param name="cartModel">cartModel</param>
        /// <param name="submitOrderViewModel">submitOrderViewModel</param>
        /// <returns> List<AddressModel> </returns>
        List<AddressModel> GetAnonymousUserAddresses(ShoppingCartModel cartModel, SubmitOrderViewModel submitOrderViewModel);

        /// <summary>
        /// Submit an order.
        /// </summary>
        /// <param name="submitOrderViewModel"></param>
        /// <returns></returns>
        OrdersViewModel SubmitOrder(SubmitOrderViewModel submitOrderViewModel);

        /// <summary>
        /// This method for amazon pay
        /// </summary>
        /// <param name="submitOrderViewModel"></param>
        /// <returns></returns>
        bool IsAmazonPayEnable(SubmitOrderViewModel submitOrderViewModel);

        /// <summary>
        /// Get billing abd shipping address of user by address Id
        /// </summary>
        /// <param name="addressId">Address Id</param>
        /// <returns>AddressListViewModel</returns>
        AddressListViewModel GetBillingShippingAddress(string type = "", int addressId = 0, int otherAddressId = 0, int userId = 0, bool isCartAddress = false, bool IsFromEdit = false);

        /// <summary>
        /// Get billing abd shipping address of user by address Id
        /// </summary>
        /// <param name="addressId">Address Id</param>
        /// <param name="isCalculateCart">If set to true then only shopping cart calculate call will be made.</param>
        /// <returns>AddressListViewModel</returns>
        AddressListViewModel GetBillingShippingAddress(int userId, bool isCartAddress, string type = "", int addressId = 0, int otherAddressId = 0, bool IsFromEdit = false, bool isQuoteRequest = false, bool isCalculateCart = true);

        /// <summary>
        /// Set paypal token and map all data to "SubmitOrderViewModel"
        /// </summary>
        /// <param name="token">PayPalToken</param>
        /// <param name="shippingAddressId">ShippingAddressId</param>
        /// <param name="billingAddressId">BillingAddressId</param>
        /// <param name="shippingOptionId">ShippingOptionId</param>
        /// <param name="paymentSettingId">PaymentSettingId</param>
        /// <param name="additionalInstruction">AdditionalInstruction</param>
        /// <param name="paymentCode">string PaymentCode</param>
        /// <param name="inHandDate">string</param>
        /// <param name="jobName">string</param>
        /// <param name="shippingConstraintCode">string</param>
        /// <returns>SubmitOrderViewModel</returns>
        SubmitOrderViewModel SetPayPalToken(string token, int shippingAddressId, int billingAddressId, int shippingOptionId, int paymentSettingId, string additionalInstruction, string paymentCode, string orderNumber, string inHandDate = "", string jobName = "", string shippingConstraintCode = "");

        /// <summary>
        /// Get user details.
        /// </summary>
        /// <returns>CheckoutViewModel</returns>
        CheckoutViewModel GetUserDetails(int userId = 0);

        /// <summary>
        /// Get filter of shipping options.
        /// </summary>
        /// <param name="addressList">AddressListViewModel</param>
        /// <returns></returns>
        FilterCollection GetFilter(AddressListViewModel addressList);

        /// <summary>
        /// Get order view model.
        /// </summary>
        /// <param name="omsOrderId">omsOrderId</param>
        /// <returns></returns>
        OrdersViewModel GetOrderViewModel(int omsOrderId);

        /// <summary>
        /// Get Payment API Header
        /// </summary>
        /// <returns>API Header</returns>
        AjaxHeadersModel GetPaymentAPIHeader();

        /// <summary>
        /// Do PayPalExpress Payment Process
        /// </summary>
        /// <param name="model">Submit Payment Model</param>
        OrdersViewModel DoPayPalExpressPaymentProcess(Api.Models.SubmitPaymentModel model);

        /// <summary>
        /// Create single order line item.
        /// </summary>
        /// <param name="orderModel"></param>
        /// <param name="orderLineItemListModel"></param>
        void CreateSingleOrderLineItem(OrderModel orderModel, List<OrderLineItemModel> orderLineItemListModel);

        /// <summary>
        /// Update address details for search for location address.
        /// </summary>
        /// <param name="viewModel">Address view model with data to update.</param>
        /// <returns>Updated address view model.</returns>
        AddressViewModel UpdateSearchAddress(AddressViewModel viewModel);

        /// <summary>
        /// Get address details on the basis of address id.
        /// </summary>
        /// <param name="addressId">Address Id.</param>
        /// <param name="isCalculateCart">If set to true then only shopping cart calculate call will be made.</param>
        /// <returns>Returns address details</returns>
        AddressViewModel GetAddressById(int? addressId, string addressType = "", bool isCalculateCart = true);


        /// <summary>
        /// Set the recipient name for the respective address type in cart.
        /// </summary>
        /// <param name="firstName"></param>
        /// <param name="lastName"></param>
        /// <param name="addressType"></param>
        /// <returns></returns>
        AddressViewModel SetAddressRecipientNameInCart(string firstName, string lastName, string addressType = "");


        /// <summary>
        /// Get search locations on the basis of search term.
        /// </summary>
        /// <param name="searchTerm">Entered search term.</param>
        /// <param name="portalId">Portal Id.</param>
        /// <returns>Returns the list of matched search locations.</returns>
        List<AutoComplete> GetSearchLocation(string searchTerm, int portalId, string addressType);


        /// <summary>
        /// Get valid recommended addresses.
        /// </summary>
        /// <param name="model">Address view model with data.</param>
        /// <returns>Returns list of valid matched addresses.</returns>
        AddressListViewModel GetRecommendedAddressList(AddressViewModel model);



        /// <summary>
        /// Check whether alternate address exist, if exist append separator.
        /// </summary>
        /// <param name="model">Address model.</param>
        /// <returns>Return address.</returns>
        string CheckAndAppendAlternateAddress(AddressViewModel model);

        /// <summary>
        /// Get oms order id from cookie.
        /// </summary>
        /// <returns>OmsOrderId</returns>
        int GetOrderIdFromCookie();

        /// <summary>
        /// Generate the order number.
        /// </summary>
        /// <param name="portalId">portal Id</param>
        /// <returns>Order Number.</returns>
        string GenerateOrderNumber(int portalId);

        /// <summary>
        /// Get API Header
        /// </summary>
        /// <returns>API Header</returns>
        AjaxHeadersModel GetAppHeader();

        /// <summary>
        /// Set Billing and Shipping address
        /// </summary>
        /// <param name="type"></param>
        /// <param name="addressId"></param>
        /// <param name="addressListViewModel"></param>
        /// <returns>AddressViewModel</returns>
        AddressViewModel SetAddressByAddressType(string type, int addressId, AddressListViewModel addressListViewModel);

        #region AmazonPay
        /// <summary>
        /// Set AmazonPay token and map all data to "SubmitOrderViewModel"
        /// </summary>
        /// <param name="amazonOrderReferenceId">Amazon OrderReference Id</param>
        /// <param name="paymentType">Payment Type</param>
        /// <param name="shippingOptionId">Shipping Option Id</param>
        /// <param name="paymentSettingId">Payment Setting Id</param>
        /// <param name="paymentCode">paymentCode</param>
        /// <param name="orderNumber">order number to be passed</param>
        /// <param name="inHandDate">string</param>
        /// <param name="jobName">string</param>
        /// <param name="shippingConstraintCode">string</param>
        /// <returns>SubmitOrderViewModel</returns>
        SubmitOrderViewModel SetAmazonPayDetails(string amazonOrderReferenceId, string paymentType, int shippingOptionId, int paymentSettingId, string paymentCode, string additionalInstruction, string captureId, string orderNumber = "", string inHandDate = "", string jobName = "", string shippingConstraintCode = "");

        /// <summary>
        /// Get AmazonPay addres details from amazon.
        /// </summary>
        /// <param name="paymentSettingId">paymentSettingId</param>
        /// <param name="amazonOrderReferenceId">AmazonOrder Reference Id</param>
        /// <param name="total">Total</param>
        /// <returns>AddressViewModel</returns>
        AddressViewModel GetAmazonAddress(int paymentSettingId, string amazonOrderReferenceId, string total, string accessToken = null);

        /// <summary>
        /// Get AmazonPay details.
        /// </summary>
        /// <returns>PaymentSettingViewModel</returns>
        PaymentSettingViewModel AmazonPaymentSetting(bool isQuotes = false);

        /// <summary>
        /// Get amazon shipping option.
        /// </summary>
        /// <param name="amazonOrderReferenceId">AmazonOrderReferenceId</param>
        /// <param name="paymentSettingId">PaymentSettingId</param>
        /// <param name="total">Total</param>
        /// <param name="shippingTypeName">ShippingType Name</param>
        /// <returns>ShippingOptionListViewModel</returns>
        ShippingOptionListViewModel GetAmazonShippingOptions(string amazonOrderReferenceId, int paymentSettingId, string total, string shippingTypeName = null, string accessToken = null, string accountNumber = null, string shippingMethod = null);

        /// <summary>
        /// Get payment response.
        /// </summary>
        /// <param name="cartModel"></param>
        /// <param name="submitOrderViewModel"></param>
        /// <returns></returns>
        GatewayResponseModel GetPaymentResponse(ShoppingCartModel cartModel, SubmitOrderViewModel submitOrderViewModel);

        /// <summary>
        /// Ensure shopping cart calculation.
        /// </summary>
        /// <param name="cartModel"></param>
        /// <param name="submitOrderViewModel"></param>
        /// <returns></returns>
        ShoppingCartModel EnsureShoppingCartCalculations(ShoppingCartModel cartModel, SubmitOrderViewModel submitOrderViewModel = null);

        #endregion
    }
}
