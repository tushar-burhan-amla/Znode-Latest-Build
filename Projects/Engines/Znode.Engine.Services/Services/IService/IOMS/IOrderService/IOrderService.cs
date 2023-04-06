using System.Collections.Generic;
using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.ShoppingCart;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IOrderService
    {
        /// <summary>
        /// Get the list of all orders.
        /// </summary>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="page">paging parameters.</param>
        /// <returns>OrdersList Model.</returns>
        OrdersListModel GetOrderList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get the list of failed order transaction list.
        /// </summary>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="page">paging parameters.</param>
        /// <returns>FailedOrderTransactionList Model.</returns>
        FailedOrderTransactionListModel FailedOrderTransactionList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get the list of all group orders.
        /// </summary>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="page">paging parameters.</param>
        /// <returns>OrdersList Model.</returns>
        OrdersListModel GetGroupOrderList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);


        /// <summary>
        /// Create new order.
        /// </summary>
        /// <param name="model">ShoppingCartModel</param>
        /// <returns>OrderModel</returns>
        OrderModel CreateOrder(ShoppingCartModel model);

        /// <summary>
        /// update existing order.
        /// </summary>
        /// <param name="model">OrderModel</param>
        /// <returns>OrderModel</returns>
        OrderModel UpdateOrder(OrderModel model);

        /// <summary>
        /// To get order details for payment.
        /// </summary>
        /// <param name="orderPaymentCreateModel">OrderPaymentCreateModel</param>
        /// <returns>OrderPaymentModel</returns>
        OrderPaymentModel GetOrderDetailsForPayment(OrderPaymentCreateModel orderPaymentCreateModel);

        /// <summary>
        /// get order details by order id.
        /// </summary>
        /// <param name="orderId">orderId</param>
        /// <param name="expands">Expand Collection.</param>
        /// <returns>OrderModel</returns>
        OrderModel GetOrderById(int orderId, FilterCollection filters, NameValueCollection expands);

        /// <summary>       
        /// get order details by order number.      
        /// </summary>       
        /// <param name="orderNumber">orderNumber</param>       
        /// <param name="expands">Expand Collection.</param>       
        /// <returns>OrderModel</returns>       
        OrderModel GetOrderByOrderNumber(string orderNumber, FilterCollection filters, NameValueCollection expands);

        /// <summary>
        /// Create new Customer.
        /// </summary>
        /// <param name="userAddressModel">UserAddressModel</param>
        /// <returns>UserAddressModel</returns>
        UserAddressModel CreateNewCustomer(UserAddressModel userAddressModel);

        /// <summary>
        /// Get order invoice details.
        /// </summary>
        /// <param name="orderIds">selected order ids</param>
        /// <param name="expands">NameValueCollection</param>
        /// <returns>Return order list.</returns>
        OrdersListModel GetOrderDetailsForInvoice(ParameterModel orderIds, NameValueCollection expands, FilterCollection filters = null);

        /// <summary>
        /// Update Order Payment Status
        /// </summary>
        /// <param name="orderId">orderId</param>
        /// <param name="paymentStatus">paymentStatus</param>
        /// <returns>Returns true if updated successfully else return false.</returns>
        bool UpdateOrderPaymentStatus(int orderId, string paymentStatus, int? paymentStateId = null, int createdBy = 0, int modifiedBy = 0);

        /// <summary>
        /// Add Refund Payment details
        /// </summary>
        /// <param name="refundPaymentListModel">refundPaymentListModel</param>
        /// <returns>Return True if added successfully else false</returns>
        bool AddRefundPaymentDetails(OrderItemsRefundModel refundPaymentListModel);

        /// <summary>
        /// Get OrderLine Items With Refund payment left 
        /// </summary>
        /// <param name="orderDetailsId">orderDetailsId</param>
        /// <returns>RefundPaymentListModel</returns>
        OrderItemsRefundModel GetOrderLineItemsWithRefund(int orderDetailsId);

        /// <summary>
        /// Resend order confirmation email
        /// </summary>
        /// <param name="orderId">orderId</param>
        /// <param name="filters">FilterCollection</param>
        /// <param name="expands">NameValueCollection</param>
        /// <returns>Return true/false</returns>
        bool ResendOrderConfirmationEmail(int orderId, FilterCollection filters, NameValueCollection expands);

        /// <summary>
        /// For sending mail for cart line items.
        /// </summary>
        /// <param name="orderId">orderId</param>
        /// <param name="omsOrderLineId">omsOrderLineId</param>
        /// <param name="expands">expands</param>
        /// <returns>Return True or false</returns>
        bool ResendOrderLineItemConfirmationEmail(int orderId, string omsOrderLineId, NameValueCollection expands, bool isEnableBcc = false);

        /// <summary>
        /// Update order status.
        /// </summary>
        /// <param name="model">OrderStateParameterModel</param>
        /// <returns>Returns true if updated successfully else return false.</returns>
        bool UpdateOrderStatus(OrderStateParameterModel model);

        /// <summary>
        /// Update order Shipping Billing Address.
        /// </summary>
        /// <param name="model">OrderStateParameterModel</param>
        /// <returns>Returns true if updated successfully else return false.</returns>
        bool UpdateOrderAddress(AddressModel model);

        /// <summary>
        /// Update TrackingNumber.
        /// </summary>
        /// <param name="orderId">orderId</param>
        /// /// <param name="trackingNumber">trackingNumber</param>
        /// <returns>Returns true if updated successfully else return false.</returns>
        bool UpdateTrackingNumber(int orderId, string trackingNumber);

        /// <summary>
        /// Update TrackingNumber by order number.
        /// </summary>
        /// <param name="orderNumber">orderNumber</param>
        /// /// <param name="trackingNumber">trackingNumber</param>
        /// <returns>Returns true if updated successfully else return false.</returns>
        bool UpdateTrackingNumberByOrderNumber(string orderNumber, string trackingNumber);

        /// <summary>
        /// Get order by order line item id for reordering a single product.
        /// </summary>
        /// <param name="orderLineItemId">Order line item id for an order.</param>
        /// <param name="expands">Expand collection for orders.</param>
        /// <returns>Returns OrderModel.</returns>
        OrderModel GetOrderByOrderLineItemId(int orderLineItemId, NameValueCollection expands);

        /// <summary>
        /// Add new order note.
        /// </summary>
        /// <param name="orderNotesModel">orderNotesModel contains notes details.</param>
        /// <returns>Returns true if inserted successfully else return false.</returns>
        bool AddOrderNote(OrderNotesModel orderNotesModel);

        /// <summary>
        /// Get Order Note List.
        /// </summary>
        /// <param name="omsOrderId">order Id</param>
        /// <param name="omsQuoteId">omsQuoteId</param>
        /// <returns>OrderNotesListModel</returns>
        OrderNotesListModel GetOrderNoteList(int omsOrderId, int omsQuoteId);

        /// <summary>
        /// Check inventory and min/max quantity.
        /// </summary>
        /// <param name="shoppingCartModel"></param>
        /// <returns>OrderModel</returns>
        OrderModel CheckInventoryAndMinMaxQuantity(ShoppingCartModel shoppingCartModel);

        /// <summary>
        /// Get order Status.
        /// </summary>
        /// <returns></returns>
        List<OrderPaymentStateModel> GetOrderPaymentState();

        /// <summary>
        /// Save order History
        /// </summary>
        /// <param name="orderHistoryModel">orderHistoryModel</param>
        /// <returns>OrderHistoryModel</returns>
        OrderHistoryModel CreateOrderHistory(OrderHistoryModel orderHistoryModel);

        /// <summary>
        /// Get order state value by omsOrderStateId
        /// </summary>
        /// <param name="omsOrderStateId">omsOrderStateId</param>
        /// <returns>OrderStateModel</returns>
        OrderStateModel GetOrderStateValueById(int omsOrderStateId);

        /// <summary>
        /// Send returned order mail.
        /// </summary>
        /// <param name="orderId">orderId</param>
        /// <param name="expands">NameValueCollection</param>
        /// <param name="filters">FilterCollection</param>
        /// <returns>true/false</returns>
        bool SendReturnedOrderEmail(int orderId, NameValueCollection expands, FilterCollection filters);

        /// <summary>
        /// To generate unique order number on basis of current date.
        /// </summary>
        /// <param name="submitOrderModel">submitOrderModel</param>
        /// <param name="portalId">portalId</param>
        /// <returns>unique order number</returns>
        string GenerateOrderNumber(SubmitOrderModel submitOrderModel, ParameterModel portalId = null);

        /// <summary>
        /// Get Order List by SP.
        /// </summary>
        /// <param name="pageListModel"></param>
        /// <param name="userId"></param>
        /// <param name="fromAdmin"></param>
        /// <returns></returns>
        IList<OrderModel> GetOrderList(PageListModel pageListModel, int userId, int fromAdmin);

        /// <summary>
        /// Send Email for purchase order.
        /// </summary>
        /// <param name="sendInvoiceModel">SendInvoiceModel to Send Email .</param>
        /// <returns>true/false</returns>
        bool SendPOEmail(SendInvoiceModel sendInvoiceModel);

        /// <summary>
        /// Update order line item details.
        /// </summary>
        /// <param name="orderDetailListModel">Order line item list model.</param>
        /// <returns>Returns updated status model.</returns>
        OrderLineItemStatusListModel UpdateOrderLineItems(OrderLineItemDataListModel orderDetailListModel);

        /// <summary>
        /// Update order status, external id and order notes by order number.
        /// </summary>
        /// <param name="model">Order Details model containing order number, order state, order notes and external id.</param>
        /// <returns>Returns true if order details are successfully updated by order number.</returns>
        bool UpdateOrderDetailsByOrderNumber(OrderDetailsModel model);

        /// <summary>
        ///Update Order Paypal Payment TransactionId
        /// </summary>
        /// <param name="orderId">orderId</param>
        /// <param name="transactionId">transactionId</param>
        /// <returns>Returns true if updated successfully else return false.</returns>
        bool UpdateOrderTransactionId(int orderId, string transactionId, int createdBy = 0, int modifiedBy = 0);

        /// <summary>
        /// Convert the quote to order.
        /// </summary>
        /// <param name="accountQuoteModel">Account Quote Model.</param>
        /// <returns>Returns created order.</returns>
        OrderModel ConvertToOrder(AccountQuoteModel accountQuoteModel);

        /// <summary>
        /// Check quantity with in-stock inventory
        /// </summary>
        /// <param name="checkout">checkout model</param>
        /// <param name="inventoryList">Inventory list</param>
        /// <returns></returns>
        bool CheckQuantityWithInventory(IZnodeCheckout checkout, List<InventorySKUModel> inventoryList);
        /// <summary>
        /// Reorder Complete Order as well as single line item.
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="portalId"></param>
        /// <param name="userId"></param>
        /// <param name="omsOrderLineItemsId"></param>
        /// <returns></returns>
        bool ReorderCompleteOrder(int orderId, int portalId, int userId = 0, int omsOrderLineItemsId = 0);

        /// <summary>
        ///  Get order Receipt details.
        /// </summary>
        /// <param name="orderId">Order Id to get order Detail</param>
        /// <returns> order Receipt details</returns>
        OrderModel GetOrderReceiptByOrderId(int orderId);

        /// <summary>
        /// Save Order to database
        /// </summary>
        /// <param name="model"></param>
        /// <param name="updateordermodel">ShoppingCartModel</param>
        /// <param name="isTaxCostUpdated">Set to true if tax amount of order is changed else set to false</param>
        /// <returns>OrderModel</returns>
        OrderModel SaveOrder(ShoppingCartModel model, SubmitOrderModel updateOrderModel = null, bool isTaxCostUpdated = true);

        /// <summary>
        /// Get Shopping Cart Details
        /// </summary>
        /// <param name="quoteDetails"></param>
        /// <param name="accountQuoteModel"></param>
        /// <param name="cartParameterModel"></param>
        /// <returns>Shopping Cart Model</returns>
        ShoppingCartModel GetShoppingCartDetails(ZnodeOmsQuote quoteDetails, AccountQuoteModel accountQuoteModel, CartParameterModel cartParameterModel);

        /// <summary>
        ///  Set Order Discount.
        /// </summary>
        /// <param name="OrderModel">model</param>
        /// <returns> </returns>
        void SetOrderDiscount(OrderModel model);

        /// <summary>
        /// Get Address List With Shipment
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="userId"></param>
        /// <returns>AddressListModel</returns>
        AddressListModel GetAddressListWithShipment(int orderId, int userId);

        /// <summary>
        /// Get Order Details based on the Expand parameter
        /// </summary>
        /// <param name="expands"></param>
        /// <param name="pageListModel"></param>
        /// <param name="userId"></param>
        /// <param name="fromAdmin"></param>
        /// <param name="storedProcedureName"></param>
        /// <returns></returns>
        IList<OrderModel> GetOrderListWithExpands(NameValueCollection expands, PageListModel pageListModel, int userId, int fromAdmin, string storedProcedureName);


        /// <summary>
        /// Get the ip address when user is not placing the order through impersonation.
        /// </summary>
        /// <returns></returns>
        string GetIpAddress();

        /// <summary>
        /// Get the OrderLineItemsIds comma seperated.
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        string GetOrderLineItemsIds(OrderModel model);

        /// <summary>
        /// This method use to add the price of addon to main product.
        /// </summary>
        /// <param name="orderModel"></param>
        /// <param name="addonOrderLineItems"></param>
        void AddonPrice(OrderModel orderModel, List<OrderLineItemModel> addonOrderLineItems);

        /// <summary>
        /// Update Order Tracking Number.
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        bool UpdateBillingAddress(int orderId, AddressModel model);

        /// <summary>
        /// Get Html Resend Receipt For Email.
        /// </summary>
        /// <param name="orderModel"></param>
        /// <param name="isFromReturnedReceipt"></param>
        /// <param name="isEnableBcc"></param>
        /// <returns></returns>
        string GetHtmlResendReceiptForEmail(OrderModel orderModel, bool isFromReturnedReceipt, out bool isEnableBcc);

        /// <summary>
        /// Send Order status email.
        /// </summary>
        /// <param name="orderModel"></param>
        void SendOrderStatusEmail(OrderModel orderModel);

        /// <summary>
        /// Get order by order id for returned order.
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="expands"></param>
        /// <param name="filters"></param>
        /// <returns></returns>
        OrderModel GetOrderByIdForReturn(int orderId, NameValueCollection expands, FilterCollection filters);


        /// <summary>
        /// Calculate tax cost for partially return items.
        /// </summary>
        /// <param name="portalId"></param>
        /// <param name="model"></param>
        /// <param name="returnItems"></param>
        void CalculateReturnItemTax(int portalId, OrderModel model, List<ReturnOrderLineItemModel> returnItems);

        /// <summary>
        /// Get AddOns return line item for calculate tax cost.
        /// </summary>
        /// <param name="orderLineItemList"></param>
        /// <param name="returnItem"></param>
        /// <returns></returns>
        List<ReturnOrderLineItemModel> GetAddOnReturnItem(List<OrderLineItemModel> orderLineItemList, ReturnOrderLineItemModel returnItem);

        /// <summary>
        /// Map Add ons return line items.
        /// </summary>
        /// <param name="orderLineItemModel"></param>
        /// <param name="returnItem"></param>
        /// <returns></returns>
        ReturnOrderLineItemModel GetAddOnReturnLineItem(OrderLineItemModel orderLineItemModel, ReturnOrderLineItemModel returnItem);

        /// <summary>
        /// Check if all the order line items need to return and if yes then update the status of all return items.
        /// </summary>
        /// <param name="model">OrderModel</param>
        /// <param name="isReturnItemsUpdateRequired">If set to false then return items should not get updated in database.</param>
        /// <returns></returns>
        bool IsReturnAllItems(OrderModel model, bool isReturnItemsUpdateRequired = true);

        /// <summary>
        /// Expands necessary to get OrderDetails.
        /// </summary>
        /// <returns></returns>
        NameValueCollection GetOrderExpands();

        /// <summary>
        /// Get portal pixel tracking details.
        /// </summary>
        /// <param name="orderModel"></param>
        void GetPortalPixelTracking(OrderModel orderModel);

        /// <summary>
        /// Get order details
        /// </summary>
        /// <param name="order"></param>
        /// <param name="orderDetail"></param>
        /// <param name="isFromOrderReceipt"></param>
        /// <param name="isOrderHistory"></param>
        /// <param name="isFromReorder"></param>
        /// <param name="expands"></param>
        /// <param name="isFromReturnLineItem"></param>
        /// <returns></returns>
        OrderModel GetOrderDetails(ZnodeOmsOrder order, ZnodeOmsOrderDetail orderDetail, bool isFromOrderReceipt, bool isOrderHistory, bool isFromReorder, NameValueCollection expands = null, bool isFromReturnLineItem = false);

        /// <summary>
        /// Get Download product key of product.
        /// </summary>
        /// <param name="sku"></param>
        /// <param name="quantity"></param>
        /// <param name="omsOrderLineItemsId"></param>
        /// <returns></returns>
        string GetProductKey(string sku, decimal quantity, int omsOrderLineItemsId);

        /// <summary>
        /// Get Downloadable product key of product
        /// </summary>
        /// <param name="sku"></param>
        /// <param name="omsOrderLineItemsId"></param>
        /// <param name="downloadableProductkeys"></param>
        /// <returns></returns>
        string GetProductKey(string sku, int omsOrderLineItemsId, List<string> downloadableProductkeys);

        /// <summary>
        /// to get shipping address
        /// </summary>
        /// <param name="orderBilling"></param>
        /// <returns></returns>
        string GetOrderBillingAddress(OrderModel orderBilling);

        /// <summary>
        /// To set shopping cart data to checkout object.
        /// </summary>
        /// <param name="checkout"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        IZnodeCheckout SetShoppingCartDataToCheckout(IZnodeCheckout checkout, ShoppingCartModel model);

        /// <summary>
        /// To revert product inventory for updating existion order .
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="userId"></param>
        /// <param name="omsOrderLineitemIds"></param>
        /// <param name="isRevertAll"></param>
        /// <returns></returns>
        bool RevertOrderInventory(int orderId, int? userId, string omsOrderLineitemIds = "", int isRevertAll = 0);

        /// <summary>
        /// Save Order Payment Detail
        /// </summary>
        /// <param name="orderPaymentModel">orderPaymentModel</param>
        /// <returns></returns>
        bool SaveOrderPaymentDetail(OrderPaymentDataModel orderPaymentModel);

        /// <summary>
        /// Get Payment History
        /// </summary>
        /// <param name="orderModel">orderModel</param>
        /// <returns></return
        void GetPaymentHistory(OrderModel orderModel);
    }
}