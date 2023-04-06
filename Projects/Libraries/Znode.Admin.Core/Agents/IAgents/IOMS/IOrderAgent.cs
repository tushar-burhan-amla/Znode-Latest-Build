using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Agents
{
    public interface IOrderAgent
    {
        /// <summary>
        /// Get order details by order id.
        /// </summary>
        /// <param name="filters">Filter Collection</param>
        /// <param name="sortCollection">SortCollection</param>
        /// <param name="pageIndex">Current index of page.</param>
        /// <param name="recordPerPage">record per page.</param>
        /// <param name="userId">userId</param>
        /// <param name="accountId">accountId</param>
        /// <param name="portalId">portalId</param>
        /// <param name="portalName">portalName</param>
        /// <returns>OrdersListViewModel</returns>
        OrdersListViewModel GetOrderList(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null, int userId = 0, int accountId = 0, int portalId = 0, string portalName = null);

        /// <summary>
        /// Get failed order transaction list.
        /// </summary>
        /// <param name="filters">Filter Collection</param>
        /// <param name="sortCollection">SortCollection</param>
        /// <param name="pageIndex">Current index of page.</param>
        /// <param name="recordPerPage">record per page.</param>
        /// <returns>FailedOrderTransactionList View Model</returns>
        FailedOrderTransactionListViewModel FailedOrderTransactionList(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Get list of Portals, Catalogs and Accounts on the basis of which create new order.
        /// </summary>
        /// <param name="portalId">Portal id</param>
        /// <returns>CreateOrderViewModel.</returns>
        CreateOrderViewModel GetCreateOrderDetails(int portalId = 0);

        /// <summary>
        /// Get the list of customer on the basis of portalId and accountId.
        /// </summary>
        /// <param name="portalId">Id of Portal</param>
        /// <param name="filters">FilterCollection</param>
        /// <param name="sortCollection">Sort Collection</param>
        /// <param name="pageIndex">Current Page Index</param>
        /// <param name="recordPerPage">Total Record count</param>
        /// <returns>CustomerListViewModel</returns>
        CustomerListViewModel GetCustomerList(int portalId, int accountId, bool isAccountCustomer, FilterCollection filters, SortCollection sortCollection, int? pageIndex, int? recordPerPage);

        /// <summary>
        /// Get details of perticular user.
        /// </summary>
        /// <param name="portal">Id of portal.</param>
        /// <param name="userId">Id of user.</param>
        /// <returns>UserAddressDataViewModel</returns>
        UserAddressDataViewModel GetCustomerDetails(int portal, int userId);

        /// <summary>
        /// Create Update Customer Address.
        /// </summary>
        /// <param name="userAddressDataViewModel">UserAddressDataViewModel</param>
        /// <returns>UserAddressDataViewModel</returns>
        UserAddressDataViewModel CreateUpdateCustomerAddress(UserAddressDataViewModel userAddressDataViewModel);

        /// <summary>
        /// Get Country list to create new customer.
        /// </summary>
        /// <param name="portalId">Id of portal</param>
        /// <param name="userAddressDataViewModel">UserAddressDataViewModel</param>
        /// <returns>UserAddressDataViewModel</returns>
        UserAddressDataViewModel GetCountryList(int portalId, UserAddressDataViewModel userAddressDataViewModel = null);

        /// <summary>
        /// Get published product list.
        /// </summary>
        /// <param name="filters">FilterCollection</param>
        /// <param name="sorts">SortCollection</param>
        /// <param name="pageIndex">Current Page Index</param>
        /// <param name="pageSize">Total page size</param>
        /// <returns>PublishProductsListViewModel</returns>
        PublishProductsListViewModel GetPublishProducts(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get sorted customer list by name.
        /// </summary>
        /// <param name="portalId">current portal</param>
        /// <param name="customerName">searched customer name</param>
        /// <param name="isAccountCustomer">Is Account Customer</param>
        /// <returns></returns>
        List<CustomerViewModel> GetCustomerListByName(int portalId, string customerName, bool isAccountCustomer, int accountId);

        /// <summary>
        /// Sets the Selected Shipping Options and show the confirmation page details for the cart.
        /// </summary>
        /// <param name="cart">ShoppingCartModel</param>
        /// <param name="shippingAddress">AddressViewModel</param>
        /// <returns>Return the cart items, address as review order view model</returns>
        ReviewOrderViewModel GetCheckoutReview(ShoppingCartModel cart, AddressViewModel shippingAddress);

        /// <summary>
        /// Save user details in session.
        /// </summary>
        /// <param name="portalId">portal id</param>
        /// <param name="userId">user id</param>
        void SaveUserDetailsInSession(int portalId, int userId);

        /// <summary>
        /// Get calculated cart on shipping change.
        /// </summary>
        /// <param name="model">CreateOrderViewModel</param>
        /// <returns>ReviewOrderViewModel</returns>
        CartViewModel GetShippingChargesById(CreateOrderViewModel model);

        /// <summary>
        /// Get calculated cart on shipping change.
        /// </summary>
        /// <param name="UserId">UserId</param>
        /// <param name="ShippingId">ShippingId</param>
        /// <param name="omsOrderId">omsOrderId</param>
        /// <returns>CartViewModel</returns>
        CartViewModel GetShippingChargesForManage(int userId, int? shippingId, int? omsOrderId);

        /// <summary>
        /// Get user details by user id.
        /// </summary>
        /// <param name="userId">user Id</param>
        /// <returns>UserViewModel</returns>
        UserViewModel GetUserDetailsByUserId(int userId);

        /// <summary>
        /// Bind payment option list.
        /// </summary>
        /// <param name="userId">user id</param>
        ///  <param name="portalId">user id</param>
        /// <param name="paymentType">payment type name</param>
        /// <returns>List of payment option</returns>
        List<BaseDropDownOptions> BindPaymentList(int userId, int portalId = 0, string paymentType = "");

        /// <summary>
        /// Get User's full details like Shopping cart, shipping methods, payment option and review order.
        /// </summary>
        /// <param name="cartParameter">cartParameter model contains PortalId, UserId, CatalogId, CookieMappingId and LocaleId.</param>
        /// <returns>CreateOrderViewModel</returns>
        CreateOrderViewModel GetUserFullDetails(CartParameterModel cartParameter);

        /// <summary>
        /// Get Publish Product
        /// </summary>
        /// <param name="publishProductId">Publish Product Id</param>
        /// <param name="portalId">Portal Id</param>
        /// <returns>Publish Products View Model</returns>
        PublishProductsViewModel GetPublishProduct(int publishProductId, int localeId, int portalId, int userId, int catalogId, bool isQuote = false);

        /// <summary>
        /// Get Bundle Products
        /// </summary>
        /// <param name="publishProductId">publish Product Id</param>
        /// <returns></returns>
        List<BundleProductViewModel> GetBundleProduct(int publishProductId);

        /// <summary>
        /// Submit the order.
        /// </summary>
        /// <param name="createOrderViewModel">CreateOrderViewModel</param>
        /// <returns>CreateOrderViewModel</returns>
        CreateOrderViewModel SubmitOrder(CreateOrderViewModel createOrderViewModel);

        /// <summary>
        /// Set portalCatalog id and portal Id as filter.
        /// </summary>
        /// <param name="model">FilterCollectionData Model</param>
        /// <param name="portalCatalogId">portal Catalog Id</param>
        /// <param name="portalId">portal Id</param>
        /// <param name="portalId">user Id</param>
        void SetProductListFilter(FilterCollectionDataModel model, int portalCatalogId, int portalId, int userId);

        /// <summary>
        /// Remove and add filter for customer name for search.
        /// </summary>
        /// <param name="model">FilterCollectionDataModel</param>
        /// <param name="isAccountCustomer">Is Account Customer</param>
        void AddCustomerNameToFilterCollection(FilterCollectionDataModel model, bool isAccountCustomer);

        /// <summary>
        /// Get product price and inventory
        /// </summary>
        /// <param name="productSKU">product SKU</param>
        /// <param name="quantity">product quantity</param>
        /// <param name="addOnIds">addOn Ids</param>
        /// <param name="portalId">portal Id</param>
        /// <param name="parentProductId">parent product Id</param>
        /// <param name="omsOrderId">OMS Order Id</param>
        /// <param name="userId">userId</param>
        /// <returns>PublishProductsViewModel</returns>
        PublishProductsViewModel GetProductPriceAndInventory(string productSKU, string parentProductSKU, string quantity, string addOnIds, int portalId, int parentProductId, int omsOrderId = 0, int? userId = 0);

        /// <summary>
        /// Get Configurable product.
        /// </summary>
        /// <param name="parameters">model with parameter values.</param>
        /// <param name="codes">Attribute Codes</param>
        /// <param name="values">Attribute values</param>
        /// <returns>Return product view model.</returns>
        PublishProductsViewModel GetConfigurableProduct(ParameterProductModel parameters);

        /// <summary>
        /// Get ordered details of user for edit order.
        /// </summary>
        /// <param name="orderId">order id</param>
        /// <returns>CreateOrderViewModel</returns>
        CreateOrderViewModel EditOrder(int orderId, int userId = 0, int accountId = 0, string updatePageType = null);

        /// <summary>
        ///Get group product list.
        /// </summary>
        /// <param name="productId">product id.</param>
        /// <returns>list of associated products.</returns>
        List<GroupProductViewModel> GetGroupProductList(int productId, int localeId, int portalId, int userId, int? catalogId = 0);

        /// <summary>
        /// Get order review details.
        /// </summary>
        /// <param name="createOrderViewModel">CreateOrderViewModel</param>
        /// <returns>ReviewOrderViewModel</returns>
        ReviewOrderViewModel GetReviewOrder(CreateOrderViewModel createOrderViewModel);

        /// <summary>
        /// Update existing order.
        /// </summary>
        /// <param name="additionalNote">additionalNote</param>
        /// <returns>OrderViewModel</returns>
        OrderViewModel UpdateOrder(int orderId, string additionalNote);

        /// <summary>
        /// This method will use to call the payment and process the order
        /// </summary>
        /// <param name="submitPaymentViewModel">SubmitPaymentViewModel</param>
        /// <returns>Submit Order View Model</returns>
        SubmitOrderViewModel ProcessCreditCardPayment(SubmitPaymentViewModel submitPaymentViewModel, bool isPaypalExpressCheckout = false);

        /// <summary>
        /// To Process PayPal Express Checkout Payment
        /// </summary>
        /// <param name="paymentOptionId">int paymentOptionId</param>
        /// <param name="returnUrl">string returnUrl</param>
        /// <param name="cancelUrl">string cancelUrl</param>
        /// <returns>returns PayPal response</returns>
        List<string> ProcessPayPalCheckout(SubmitPaymentViewModel submitPaymentViewModel);

        /// <summary>
        /// Check Group Product Inventory
        /// </summary>
        /// <param name="productSKU">Product sku.</param>
        /// <param name="quantity">quantity selected</param>
        /// <param name="portalId">Portal Id.</param>
        /// <param name="localeId">Locale Id.</param>
        /// <returns>product view model</returns>
        GroupProductViewModel CheckGroupProductInventory(ProductParameterModel parameters, string productSKU, string quantity);

        /// <summary>
        /// Get order invoice details.
        /// </summary>
        /// <param name="orderIds">selected order ids</param>
        /// <returns>OrderInvoiceViewModel</returns>
        OrderInvoiceViewModel GetOrderInvoiceDetails(string orderIds);

        /// <summary>
        /// Capture payment
        /// </summary>
        /// <param name="OmsOrderId">OmsOrderId</param>
        /// <param name="paymentTransactionToken">paymentTransactionToken</param>
        /// <param name="isUpdateOrderHistory">true to update order history </param>
        /// <param name="errorMessage">out string error message.</param>
        /// <returns>Returns true if captured successfully else return false.</returns>
        bool CapturePayment(int OmsOrderId, string paymentTransactionToken, bool isUpdateOrderHistory, out string errorMessage);

        /// <summary>
        /// To capture payment.
        /// </summary>        
        /// <param name="paymentTransactionToken">paymentTransactionToken</param>        
        /// <returns>BooleanModel</returns>
        BooleanModel CapturePayment(string paymentTransactionToken);

        /// <summary>
        /// Update captured payment details in order.
        /// </summary>
        /// <param name="OmsOrderId">OmsOrderId</param>
        /// <param name="paymentTransactionToken">paymentTransactionToken</param>
        /// <param name="isUpdateOrderHistory">true to update order history</param>
        /// <param name="booleanModel">To provide payment capture status</param>
        /// <param name="errorMessage">out string error message.</param>
        /// <returns>Returns true if captured payment details updated in order successfully.</returns>
        bool UpdateCapturedPaymentDetails(int OmsOrderId, string paymentTransactionToken, bool isUpdateOrderHistory, BooleanModel booleanModel, out string errorMessage);

        /// <summary>
        /// Void Payment
        /// </summary>
        /// <param name="OmsOrderId">OmsOrderId</param>
        /// <param name="paymentTransactionToken">paymentTransactionToken</param>
        /// <param name="errorMessage">out string error message.</param>
        /// <returns>Returns true if voided successfully else return false.</returns>
        bool VoidPayment(int OmsOrderId, string paymentTransactionToken, out string errorMessage);

        /// <summary>
        /// Get User Account Address By AddressId.
        /// </summary>
        /// <param name="addressId">Address Id</param>
        /// <param name="isB2BCustomer">B2B customer flag</param>
        /// <param name="portalId">portal id</param>
        /// <returns>AddressViewModel</returns>
        AddressViewModel GetUserAccountAddressByAddressId(int addressId, bool isB2BCustomer, int portalId);

        /// <summary>
        /// Get User Account Address By AddressId.
        /// </summary>
        /// <param name="addressId">Address Id</param>
        /// <param name="isB2BCustomer">B2B customer flag</param>
        /// <param name="portalId">portal id</param>
        /// <param name="userId">user id</param>
        /// <returns>AddressViewModel</returns>
        AddressViewModel GetUserAccountAddressByAddressId(int addressId, bool isB2BCustomer, int portalId, int userId);

        /// <summary>
        /// Get User Address By addressId.
        /// </summary>
        /// <param name="addressId">address id.</param>
        /// <param name="userId">user id.</param>
        /// <param name="portalId">portal id</param>
        /// <returns>AddressViewModel</returns>
        AddressViewModel GetUserAddressById(int addressId, int userId, int portalId);

        /// <summary>
        /// Update Customer Address And Calculate.
        /// </summary>
        /// <param name="addressViewModel">AddressViewModel</param>
        /// <param name="IsManage">To know order call from manage or edit order</param>
        /// /// <param name="omsOrderId ">omsOrderId</param>
        /// <returns>CreateOrderViewModel</returns>
        CreateOrderViewModel UpdateCustomerAddressAndCalculate(AddressViewModel addressViewModel, bool isManage = false, int omsOrderId = 0);

        /// <summary>
        /// Adding additional notes into ordeModel
        /// </summary>
        /// <param name="model">OrderViewModel</param>
        bool ManageOrderNotes(OrderViewModel model);

        /// <summary>
        /// Submit quote for approval.
        /// </summary>
        /// <param name="createOrderViewModel">CreateOrderViewModel</param>
        /// <returns>Returns created order quote.</returns>
        CreateOrderViewModel SubmitQuote(CreateOrderViewModel createOrderViewModel);

        /// <summary>
        /// Manage order.
        /// </summary>
        /// <param name="orderId">order id</param>
        /// <returns>OrderViewModel</returns>
        OrderViewModel Manage(int orderId);

        /// <summary>
        /// Get order details.
        /// </summary>
        /// <param name="orderId">order id</param>
        /// <returns>OrderViewModel</returns>
        AddressViewModel GetorderdetailsById(int orderId, int shippingAddressId, int billingAddressId, string control, int portalId);

        /// <summary>
        /// Get Address Default Data.
        /// </summary>
        /// <param name="addressId">address Id</param>
        /// <param name="userId">user id</param>
        /// <param name="accountId">account id</param>
        /// <param name="portalId"> portal id</param>
        /// <returns>AddressViewModel</returns>
        AddressViewModel GetAddressDefaultData(int addressId, int userId, int accountId, int portalId);

        /// <summary>
        /// Get OrderLine Items With Refund payment left
        /// </summary>
        /// <param name="orderDetailsId">orderDetailsId</param>
        /// <returns>Refund Payment List View Model</returns>
        OrderItemsRefundViewModel GetOrderLineItemsWithRefund(int orderDetailsId);

        /// <summary>
        /// Add Refund Payment Details in admin and Refund the payment
        /// </summary>
        /// <param name="refundPaymentListViewModel">refundPaymentListViewModel</param>
        /// <param name="errorMessage">Error message is refund fails</param>
        /// <returns>Returns true if Refunded successfully else return false.</returns>
        bool AddRefundPaymentDetails(OrderItemsRefundViewModel refundPaymentListViewModel, out string errorMessage);

        /// <summary>
        /// Resend order confirmation email
        /// </summary>
        /// <param name="orderId">orderId</param>
        /// <returns>Email message</returns>
        string ResendOrderConfirmationEmail(int orderId);

        /// <summary>
        /// Resend order confirmation email for single cart item.
        /// </summary>
        /// <param name="orderId">orderId</param>
        /// <param name="omsOrderLineItemId">omsOrderLineItemId</param>
        /// <returns>Email message</returns>
        string ResendOrderConfirmationEmailForCart(int orderId, int omsOrderLineItemId);

        /// <summary>
        /// Get Order Status Details.
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        OrderStateParameterViewModel GetOrderStatusDetails(int orderId);

        /// <summary>
        /// Bind Order Status list.
        /// </summary>
        /// <returns>Order Status list</returns>
        List<SelectListItem> BindOrderStatus(FilterTuple filter = null);

        /// <summary>
        /// Update Order Status.
        /// </summary>
        /// <param name="model">OrderStateParameterViewModel</param>
        /// <returns>Returns true if updated successfully else return false.</returns>
        bool UpdateOrderStatus(OrderStateParameterViewModel model);

        /// <summary>
        /// Create quote for customer.
        /// </summary>
        /// <param name="userId">User id of the customer.</param>
        /// <param name="userId">Account id of the customer.</param>
        /// <returns>Returns CreateOrderViewModel.</returns>
        CreateOrderViewModel CreateQuoteForCustomer(int userId, int accountId);

        /// <summary>
        /// Add new order note.
        /// </summary>
        /// <param name="additionalNotes">additional Notes</param>
        /// <param name="omsOrderDetailsId">oms Order Details Id</param>
        /// <param name="omsOrderId">oms Order Id</param>
        /// <param name="omsQuoteId">oms Quote Id</param>
        /// <returns>List of OrderNotesViewModel </returns>
        List<OrderNotesViewModel> AddOrderNote(string additionalNotes, int? omsOrderDetailsId, int? omsQuoteId, int omsOrderId = 0);

        /// <summary>
        /// Get order data for receipt.
        /// </summary>
        /// <param name="omsOrderId">omsOrderId</param>
        /// <returns></returns>
        OrderViewModel GetDataForReceipt(int omsOrderId);

        /// <summary>
        /// Get PaymentGatway Name
        /// </summary>
        /// <param name="paymentSettingId">Payment Setting Id/param>
        /// <param name="userId">userId</param>
        /// <returns></returns>
        PaymentDetailsViewModel GetOrderPaymentDetails(int paymentSettingId, int? userId = 0);

        /// <summary>
        /// Get Attribute Validation By Codes
        /// </summary>
        /// <param name="attributecodes">Attribute Codes</param>
        /// <param name="productId">productId</param>
        /// <returns>PIMProductAttributeValuesViewModel list</returns>
        List<PIMProductAttributeValuesViewModel> GetAttributeValidationByCodes(Dictionary<string, string> personliseValues, int productId);

        /// <summary>
        /// Remove user details and cart from session.
        /// </summary>
        void RemoveUserDetailsFromSessions(int userId = 0);

        /// <summary>
        /// Update order Payment Status.
        /// </summary>
        /// <param name="OmsOrderId">Order id</param>
        /// <param name="paymentstatus">Payment status</param>
        /// <returns>Return OrderViewModel</returns>
        OrderViewModel UpdateOrderPaymentStatus(int OmsOrderId, string paymentstatus);

        /// <summary>
        /// Update order Payment Status.
        /// </summary>
        /// <param name="TrackingNumber">Tracking Number</param>
        /// <returns>Return OrderViewModel</returns>
        string UpdateTrackingNumber(int orderId, string trackingNumber);


        /// <summary>
        /// Update In hands date.
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="inHandDate"></param>
        /// <returns></returns>
        string UpdateInHandDate(int orderId, string inHandDate);

        /// <summary>
        /// Update Job Name
        /// </summary>
        /// <param name="orderId">int</param>
        /// <param name="jobName">string</param>
        /// <returns>string</returns>
        string UpdateJobName(int orderId, string jobName);

        /// <summary>
        /// Update Shipping Constraint Code
        /// </summary>
        /// <param name="orderId">int</param>
        /// <param name="shippingConstraintCode">string</param>
        /// <returns>string</returns>
        string UpdateShippingConstraintCode(int orderId, string shippingConstraintCode);

        /// <summary>
        /// Update order Shipping Account Number.
        /// </summary>
        /// <param name="ShippingAccountNumber">Shipping Account Number</param>
        /// <returns>Return OrderViewModel</returns>
        string UpdateShippingAccountNumber(int orderId, string shippingAccountNumber);

        /// <summary>
        /// Update order Shipping Method.
        /// </summary>
        /// <param name="ShippingMethod">ShippingMethod</param>
        /// <returns>Return OrderViewModel</returns>
        string UpdateShippingMethod(int orderId, string shippingMethod);

        /// <summary>
        /// Get Shipping options with Rates
        /// </summary>
        /// <param name="userId">userId</param>
        /// <param name="excludeCustomShippingFromCreateOrder">excludeCustomShippingFromCreateOrder</param>
        /// <returns>ShippingListViewModel</returns>
        ShippingListViewModel GetShippingListWithRates(int? userId = 0, bool excludeCustomShippingFromCreateOrder = false, bool isQuote = false);

        /// <summary>
        /// Get Shipping options with Rates for manage
        /// </summary>
        /// <returns>ShippingListViewModel</returns>
        ShippingListViewModel GetShippingListForManage(int orderId);

        /// <summary>
        /// Get re ordered details of user for edit order.
        /// </summary>
        /// <param name="orderId">order id</param>
        /// <returns>CreateOrderViewModel</returns>
        CreateOrderViewModel ReOrder(int orderId, int userId = 0, int accountId = 0, string updatePageType = null);

        /// <summary>
        /// Get the list of publish products containing sku.
        /// </summary>
        /// <param name="sku">SKU to filter products.</param>
        /// <returns>List<AutoComplete> for products.</returns>
        List<AutoComplete> GetProductListBySKU(string sku);

        /// <summary>
        /// Get order related Information from session.
        /// </summary>
        /// <returns>OrderInfoViewModel</returns>
        OrderInfoViewModel GetOrderInformation(int orderId);

        /// <summary>
        /// Get Customer Information from session.
        /// </summary>
        /// <returns></returns>
        CustomerInfoViewModel GetCustomerInformation(int orderId);

        /// <summary>
        /// Get orderline items from session.
        /// </summary>
        /// <returns></returns>
        CartViewModel GetOrderLineItems(int orderId);

        /// <summary>
        ///  Get order history.
        /// </summary>
        /// <param name="filters">Filter Collection</param>
        /// <param name="sortCollection">SortCollection</param>
        /// <param name="pageIndex">Current index of page.</param>
        /// <param name="recordPerPage">record per page.</param>
        /// <param name="userId">userId</param>
        /// <param name="accountId">accountId</param>
        /// <returns>OrderHistoryListViewModel</returns>
        OrderHistoryListViewModel GetOrderHistory(int orderId, FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Update CSR Discount, Tax cost, shipping cost
        /// </summary>
        /// <param name="amount">amount to update</param>
        /// <param name="pagetype">tax/shipping/CSRDiscount to update.</param>
        /// <returns>CartViewModel</returns>
        CartViewModel UpdateAmounts(int orderId, string amount, string pagetype);

        /// <summary>
        /// Update CSR Discount, Tax cost, shipping cost
        /// </summary>
        /// <param name="userId">userId</param>
        /// <param name="amount">amount to update</param>
        /// <param name="pagetype">tax/shipping/CSRDiscount to update.</param>
        /// <param name="isTaxExempt">isTaxExempt flag.</param>
        /// <param name="isQuote">isQuote flag.</param>
        /// <returns>CartViewModel</returns>
        CartViewModel UpdateCartTaxAmounts(int userId, string amount, string pagetype, bool isTaxExempt, bool isQuote = false);

        /// <summary>
        /// For binding order status list.
        /// </summary>
        /// <param name="omsOrderId">respective orderId</param>
        /// <param name="orderStatus">respective orderStatus</param>
        /// <param name="pageName">type of page</param>
        /// <returns></returns>
        OrderStatusList GetOrderStatus(int omsOrderId, string orderStatus, string pageName = null);

        /// <summary>
        /// For updating order session.
        /// </summary>
        /// <param name="OrderStatusList">depending on selection selectedItemId</param>
        /// <returns>OrderStatusList </returns>
        OrderStatusList UpdateOrderAndPaymentStatus(OrderStatusList orderStatus);

        /// <summary>
        /// For Maping and updating address
        /// </summary>
        /// <param name="createOrderViewModel">create Order View Model</param>
        /// <returns>Map CustomerInfoViewModel</returns>
        CustomerInfoViewModel MapAndUpdateCustomerAddress(CreateOrderViewModel createOrderViewModel);

        /// <summary>
        /// This method will use to call the payment and process the order
        /// </summary>
        /// <param name="submitPaymentViewModel">submitPaymentViewModel</param>
        /// <returns>Submit Order View Model</returns>
        SubmitOrderViewModel SubmitEditOrderpayment(SubmitPaymentViewModel submitPaymentViewModel);

        /// <summary>
        /// Updates the Quantity updated in the shopping cart page.
        /// </summary>
        /// <param name="orderDataModel">data to update.</param>
        /// <returns>Cart View Model.</returns>
        CartViewModel UpdateOrderLineItemDetails(ManageOrderDataModel orderDataModel);

        /// <summary>
        /// Get list of returned order line items.
        /// </summary>
        /// <param name="orderId">Id of Order,</param>
        /// <returns>ReturnOrderLineItemListViewModel</returns>
        ReturnOrderLineItemListViewModel GetReturnLineItems(int orderId);

        /// <summary>
        /// Get order state value by id
        /// </summary>
        /// <param name="omsOrderStateId">omsOrderStateId</param>
        /// <returns>isEdit</returns>
        bool GetOrderStateValueById(int omsOrderStateId);

        /// <summary>
        /// Send returned order mail.
        /// </summary>
        /// <param name="orderId">orderId</param>
        /// <returns>true/false</returns>
        bool SendReturnedOrderEmail(int orderId);

        /// <summary>
        /// Get the list of return line items.
        /// </summary>
        /// <param name="orderId">Id of Order.</param>
        /// <returns>ReturnOrderLineItemListViewModel</returns>
        ReturnOrderLineItemListViewModel GetReturnLineItemList(int orderId);

        /// <summary>
        /// Set TrackingUrl By omsOrderId.
        /// </summary>
        /// <param name="omsOrderId">omsOrderId</param>
        /// <returns></returns>
        void SetTrackingUrlByOrderId(int omsOrderId, List<CartItemViewModel> cartItemViewModel);

        /// <summary>
        /// Check the user is guest or not
        /// </summary>
        /// <param name="userId">UserId</param>
        /// <returns>Depending on userid returns true or false</returns>
        bool CheckForGuestUser(int userId);

        /// <summary>
        /// Bind shipping list.
        /// </summary>
        /// <param name="userId"> user Id</param>
        /// <param name="isQuote"></param>
        /// <returns></returns>
        ShippingListViewModel BindShippingList(int userId, bool isQuote = false);
                
        /// <summary>
        /// Bind shipping list.
        /// </summary>
        /// <returns></returns>
        ShippingListViewModel BindShippingList();

        /// <summary>
        /// UpdateReturnShippingHistory in session
        /// </summary>
        /// <param name="omsOrderLineItemsId"></param>
        /// <param name="omsOrderId"></param>
        /// <param name="isInsert"></param>
        bool UpdateReturnShippingHistory(int omsOrderLineItemsId, int omsOrderId, bool isInsert);

        /// <summary>
        /// Send mail for Purchase order
        /// </summary>
        /// <param name="model">SendInvoiceViewModel</param>
        /// <returns></returns>
        bool SendPOEmail(SendInvoiceViewModel invoiceModel);

        /// <summary>
        /// Add custom shipping amount.
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="estimateShippingCost"></param>
        /// <param name="userId">userId</param>
        /// <returns>true/false</returns>
        bool AddCustomShippingAmount(decimal? amount, decimal? estimateShippingCost, int? userId = 0, bool isQuote = false);

        /// <summary>
        /// Get Payment API Header
        /// </summary>
        /// <returns>API Header</returns>
        AjaxHeadersModel GetPaymentAPIHeader();

        /// <summary>
        /// Generate order number.
        /// </summary>
        /// <param name="portalId">Id of portal.</param>
        /// <returns>Order Number</returns>
        string GenerateOrderNumber(int portalId);

        /// <summary>
        //Generate Single Order Line Item 
        /// </summary>
        /// <param name="orderModel"></param>
        /// <param name="orderLineItemListModel"></param>
        void CreateSingleOrderLineItem(OrderViewModel orderModel, List<OrderLineItemViewModel> orderLineItemListModel);

        /// <summary>
        /// Get the list of customer on the basis of portalId,and search term(Phone Number or User Name).
        /// </summary>
        /// <param name="searchTerm">Value to searched</param>
        /// <param name="portalId">Id of Portal</param>
        /// <returns>List CustomerViewModel(Return list of complete customer details.)</returns>
        List<CustomerViewModel> GetCustomerList(string searchTerm, int portalId);

        /// <summary>
        /// Get details of payment option.
        /// </summary>
        /// <param name="portalCatalogId">portal Catalog Id</param>
        /// <param name="portalId">portal Id</param>
        /// <param name="portalId">user Id</param>
        /// <returns>CreateOrderViewModel</returns>
        PaymentSettingViewModel GetPaymentMethods(int portalId, int userId);

        /// <summary>
        /// Void or refund the payment to the user.
        /// </summary>
        /// <param name="orderModel">order model</param>
        /// <param name="OmsOrderId">order id.</param>
        /// <returns>Returns true if void  payment successfully else return false.</returns>
        bool VoidRefundPayment(OrderModel orderModel, bool isSaveOrder, out string errorMessage, int? OmsOrderId = null);

        /// <summary>
        /// Get estimated shipping rates.
        /// </summary>
        /// <param name="shoppingCart">Shopping cart details.</param>
        /// <param name="excludeCustomShippingFromCreateOrder"></param>
        /// <returns>flag to exclude custom shipping.</returns>
        ShippingListViewModel GetShippingListAndRates(ShoppingCartModel shoppingCart, bool excludeCustomShippingFromCreateOrder = false);

        /// <summary>
        /// Check inventory of the product.
        /// </summary>
        /// <param name="viewModel">Product details</param>
        /// <param name="quantity">Quantity of the product.</param>
        /// <param name="omsOrderId">Order id</param>
        /// <param name="userId">user id.</param>
        void CheckInventory(PublishProductsViewModel viewModel, decimal? quantity, int omsOrderId = 0, int? userId = 0);

        /// <summary>
        /// Check Add on inventory.
        /// </summary>
        /// <param name="model">Model with product details.</param>
        /// <param name="addOnIds">Add on ids</param>
        /// <param name="quantity">Quantity of the product.</param>
        void CheckAddOnInventory(PublishProductsViewModel model, string addOnIds, decimal quantity);

        /// <summary>
        /// get product filters
        /// </summary>
        /// <param name="portalId">portal id</param>
        /// <param name="localeId">locale id</param>
        /// <param name="catalogId">catalog id</param>
        /// <returns>Filter collection.</returns>
        FilterCollection GetProductFilters(int portalId, int localeId, int catalogId);

        /// <summary>
        /// To refund order line item transaction amount by transactionId
        /// </summary>
        /// <param name="orderDetailsId">orderDetailsId</param>
        /// <param name="transactionId">transactionId</param>
        /// <param name="transactionAmount">transactionAmount</param>
        /// <returns>Returns true if refund payment successfully else return false.</returns>
        bool RefundPaymentByAmount(int orderDetailsId, string transactionId, decimal transactionAmount, out string errorMessage,OrderModel orderModel);

        /// <summary>
        /// To refund order line item in case of Paypal express by giftCard.
        /// </summary>
        /// <param name="orderDetailsId">orderDetailsId</param>
        /// <param name="transactionId">transactionId</param>
        /// <param name="transactionAmount">transactionAmount</param>
        /// <returns>Returns true if refund payment successfully else return false.</returns>
        bool RefundPaymentByGiftCard(int orderDetailsId, string transactionId, decimal transactionAmount);

        /// <summary>
        /// Get calculated shopping cart model.
        /// </summary>
        /// <param name="shoppingCartModel">ShoppingCartModel</param>
        /// <param name="isEdit">isEdit</param>
        /// <returns>Return ShoppingCartModel</returns>
        ShoppingCartModel GetCalculatedShoppingCartForEditOrder(ShoppingCartModel shoppingCartModel, bool isEdit = true);

        /// <summary>
        /// Set created by and modified by user.
        /// </summary>
        /// <param name="userId">userId</param>
        /// <returns></returns>
        void SetCreatedByUser(int? userId);

        /// <summary>
        /// Set User Information
        /// </summary>
        /// <param name="model">ShoppingCartModel</param>
        /// <returns></returns>
        void SetUserInformation(ShoppingCartModel model);

        /// <summary>
        ///Set Users Payment Details
        /// </summary>
        /// <param name="paymentSettingId">paymentSettingId</param>
        /// <param name="model">ShoppingCartModel</param>
        /// <param name="isCOD">isCOD</param>
        /// <returns></returns>
        void SetUsersPaymentDetails(int paymentSettingId, ShoppingCartModel model, bool isCOD = true);

        /// <summary>
        /// Add Portal id in filter collection
        /// </summary>
        /// <param name="filters">FilterCollection</param>
        /// <param name="portalId">portalId</param>
        /// <returns></returns>
        void AddPortalIdInFilters(FilterCollection filters, int portalId);

        /// <summary>
        /// Remove key from dictionary.
        /// /// </summary>
        /// <param name="orderModel">OrderModel</param>
        /// <param name="key">key</param> 
        /// <param name="isFromLineItem">isFromLineItem</param>
        /// <returns></returns>
        void RemoveKeyFromDictionary(OrderModel orderModel, string key, bool isFromLineItem = false);

        /// <summary>
        /// Ge User Details
        /// </summary>
        /// <param name="cartParameter"></param>
        /// <returns></returns>
        QuoteCreateViewModel GetUserFullDetailsForQuote(CartParameterModel cartParameter);

        /// <summary>
        /// Get address list with shipment.
        /// </summary>
        /// <param name="addressId">addressId</param>
        /// <param name="userId">userId</param>
        /// <param name="portalId">portalId</param>
        /// <param name="orderId">orderId</param>
        /// <returns>AddressViewModel</returns>
        AddressViewModel GetAddressDefaultDataWithShipment(int addressId,int userId, int portalId, int orderId);

        /// <summary>
        /// Reorder complete order
        /// </summary>
        /// <param name="userId">userId</param>
        /// <param name="portalId">portalId</param>
        /// <param name="orderId">orderId</param>
        /// <returns>CreateOrderViewModel</returns>
        CreateOrderViewModel ReorderCompleteOrder(int userId, int portalId, int orderId);

        /// <summary>
        /// Get User Detail By userId
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="portalId"></param>
        /// <returns></returns>
        UserModel GetUserDetailById(int userId, int portalId);

        /// <summary>
        ///  Get payment history.
        /// </summary>
        /// <param name="orderId">orderId</param>
        /// <returns>PaymentHistoryListViewModel</returns>
        PaymentHistoryListViewModel GetPaymentHistory(int orderId);
    }
}