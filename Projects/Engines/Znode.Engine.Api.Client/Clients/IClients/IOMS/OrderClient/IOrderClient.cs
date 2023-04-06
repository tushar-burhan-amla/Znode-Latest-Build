using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IOrderClient : IBaseClient
    {
        /// <summary>
        /// Get the list of all orders.
        /// </summary>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="pageIndex">Current index of page.</param>
        /// <param name="pageSize">record per page.</param>
        /// <returns>OrdersListModel.</returns>
        OrdersListModel GetOrderList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get the list of all the failed orders transaction.
        /// </summary>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="pageIndex">Current index of page.</param>
        /// <param name="pageSize">record per page.</param>
        /// <returns>FailedOrderTransactionListModel.</returns>
        FailedOrderTransactionListModel FailedOrderTransactionList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get the list of all group orders.
        /// </summary>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="pageIndex">Current index of page.</param>
        /// <param name="pageSize">record per page.</param>
        /// <returns>OrdersListModel.</returns>
        OrdersListModel GetGroupOrderList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);


        /// <summary>
        /// Create new order.
        /// </summary>
        /// <param name="model">shopping cart model</param>
        /// <returns>OrderModel</returns>
        OrderModel CreateOrder(ShoppingCartModel model);

        /// <summary>
        /// Update existing order.
        /// </summary>
        /// <param name="model">OrderModel</param>
        /// <returns>OrderModel</returns>
        OrderModel UpdateOrder(OrderModel model);

        /// <summary>
        /// To get the order details for payment.
        /// </summary>
        /// <param name="model">OrderPaymentCreateModel</param>
        /// <returns>OrderPaymentModel</returns>
        OrderPaymentModel GetOrderDetailsForPayment(OrderPaymentCreateModel model);

        /// <summary>
        /// Get Product details.
        /// </summary>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="pageIndex">Current index of page.</param>
        /// <param name="pageSize">record per page.</param>
        /// <returns>ProductDetailsList Model</returns>
        ProductDetailsListModel GetProducts(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Create new Customer.
        /// </summary>
        /// <param name="userAddressModel">UserAddressModel</param>
        /// <returns>UserAddressModel</returns>
        UserAddressModel CreateNewCustomer(UserAddressModel userAddressModel);

        /// <summary>
        /// get order details by order id.
        /// </summary>
        /// <param name="orderId">order id</param>
        /// <param name="expands">Expand Collection</param>
        /// <returns>OrderModel</returns>
        OrderModel GetOrderById(int orderId, ExpandCollection expands, FilterCollection filters = null);

        /// <summary>
        /// Get order Receipt details.
        /// </summary>
        /// <param name="orderId">Order Id for order Detail</param>
        /// <returns>order details</returns>
        OrderModel GetOrderReceiptByOrderId(int orderId);

        /// <summary>
        /// Get order invoice details.
        /// </summary>
        /// <param name="filterIds">selected order ids</param>
        /// <param name="expands">ExpandCollection</param>
        /// <returns>Order list.</returns>
        OrdersListModel GetOrderDetailsForInvoice(ParameterModel filterIds, ExpandCollection expands, FilterCollection filters = null);

        /// <summary>
        /// Update Order Payment Status
        /// </summary>
        /// <param name="orderId">orderId</param>
        /// <param name="paymentStatus">paymentStatus</param>
        /// <returns>Returns true if updated successfully else return false.</returns>
        bool UpdateOrderPaymentStatus(int orderId, string paymentStatus);

        /// <summary>
        /// Update Tracking Number
        /// </summary>
        /// <param name="orderId">orderId</param>
        /// <param name="trackingNumber">trackingNumber</param>
        /// <returns>Returns true if updated successfully else return false.</returns>
        bool UpdateTrackingNumber(int orderId, string trackingNumber);

        /// <summary>
        /// Get OrderLine Items With Refund payment left 
        /// </summary>
        /// <param name="orderDetailsId">orderDetailsId</param>
        /// <returns>Refund Payment List Model</returns>
        OrderItemsRefundModel GetOrderLineItemsWithRefund(int orderDetailsId);

        /// <summary>
        /// Add Refund payment details
        /// </summary>
        /// <param name="refundPaymentListModel">refundPaymentListModel</param>
        /// <returns>Returns true if Added successfully else return false.</returns>
        bool AddRefundPaymentDetails(OrderItemsRefundModel refundPaymentListModel);

        /// <summary>
        /// Resend order confirmation email.
        /// </summary>
        /// <param name="orderId">orderId</param>
        /// <param name="filters">FilterCollection</param>
        /// <param name="expands">ExpandCollection</param>
        /// <returns>email message</returns>
        bool ResendOrderConfirmationEmail(int orderId, FilterCollection filters, ExpandCollection expands);

        /// <summary>
        /// Resend order confirmation email for single cart line item.
        /// </summary>
        /// <param name="orderId">orderId</param>
        /// <param name="omsOrderLineItemId">omsOrderLineItemId</param>
        /// <param name="expands">ExpandCollection</param>
        /// <returns></returns>
        bool ResendOrderEmailForCartLineItem(int orderId, int omsOrderLineItemId, ExpandCollection expands);

        /// <summary>
        /// Update order status.
        /// </summary>
        /// <param name="model">OrderStateParameterModel</param>
        /// <returns>true/false</returns>
        bool UpdateOrderStatus(OrderStateParameterModel model);

        /// <summary>
        /// Update order Shipping Billing Address.
        /// </summary>
        /// <param name="model">OrderStateParameterModel</param>
        /// <returns>true/false</returns>
        bool UpdateOrderAddress(AddressModel model);


        /// <summary>
        /// Get order by order line item id for reordering a single product.
        /// </summary>
        /// <param name="orderLineItemId">Order line item id for an order.</param>
        /// <param name="expands">Expand collection for orders.</param>
        /// <returns>Returns OrderModel.</returns>
        OrderModel GetOrderByOrderLineItemId(int orderLineItemId, ExpandCollection expands);

        /// <summary>
        /// Add Order Note.
        /// </summary>
        /// <param name="additionalNotes">additional Notes</param>
        /// <param name="omsOrderDetailsId">omsOrder Details Id</param>
        /// <returns></returns>
        bool AddOrderNote(OrderNotesModel orderNotesModel);

        /// <summary>
        /// Get Order Notes List.
        /// </summary>
        /// <param name="omsOrderId">oms Order Id</param>
        /// <param name="omsQuoteId">oms Quote Id</param>
        /// <returns>OrderNotesListModel</returns>
        OrderNotesListModel GetOrderNotesList(int omsOrderId, int omsQuoteId);

        /// <summary>
        /// Check inventory and min/max quantity.
        /// </summary>
        /// <param name="shoppingCartModel"></param>
        /// <returns>OrderModel</returns>
        OrderModel CheckInventoryAndMinMaxQuantity(ShoppingCartModel shoppingCartModel);

        /// <summary>
        /// Get Payment State List.
        /// </summary>
        /// <returns>Payment State list.</returns>
        PaymentStateListModel GetPaymentStateList();

        /// <summary>
        /// Create Order History
        /// </summary>
        /// <param name="orderHistoryModel">orderHistoryModel</param>
        /// <returns>OrderHistoryModel</returns>
        OrderHistoryModel CreateOrderHistory(OrderHistoryModel orderHistoryModel);

        /// <summary>
        /// Get Order state value.
        /// </summary>
        /// <param name="omsOrderStateId">omsOrderStateId</param>
        /// <returns>OrderStateModel</returns>
        OrderStateModel GetOrderStateValueById(int omsOrderStateId);

        /// <summary>
        /// send returned order mail
        /// </summary>
        /// <param name="orderId">orderId</param>
        /// <param name="expands">ExpandCollection</param>
        /// <param name="filters">FilterCollection</param>
        /// <returns>true/false</returns>
        bool SendReturnedOrderEmail(int orderId, ExpandCollection expands, FilterCollection filters = null);

        /// <summary>
        /// Send Email for purchase order.
        /// </summary>
        /// <param name="SendInvoiceModel">SendInvoiceModel to Send Email.</param>
        /// <returns>true/false</returns>
        bool SendPOEmail(SendInvoiceModel model);

        /// <summary>
        /// Update order status, external id and order notes by order number.
        /// </summary>
        /// <param name="model">Order Details model containing order number, order status code and name, order notes and external id.</param>
        /// <returns>Returns true if order details are successfully updated by order number.</returns>
        bool UpdateOrderDetailsByOrderNumber(OrderDetailsModel model);


        /// <summary>
        /// Update Transaction Id
        /// </summary>
        /// <param name="orderId">orderId</param>
        /// <param name="transactionId">TransactionId</param>
        /// <returns>Returns true if updated successfully else return false.</returns>
        bool UpdateOrderTransactionId(int orderId, string transactionId);

        /// <summary>
        /// Reorder single line item
        /// </summary>
        /// <param name="OmsOrderLineItemsId"></param>
        /// <param name="portalId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        bool ReorderSinglelineItemOrder(int OmsOrderLineItemsId, int portalId, int userId = 0);

        
        /// <summary>
        /// Reorder Complete Order
        /// </summary>
        /// <param name="orderId"></param>
        /// <returns></returns>
        bool ReorderCompleteOrder(int orderId, int portalId, int userId = 0);

        /// <summary>
        /// Get Address List With Shipment
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="userId"></param>
        /// <returns>AddressListModel</returns>
        AddressListModel GetAddressListWithShipment(int orderId, int userId);

    }
}
