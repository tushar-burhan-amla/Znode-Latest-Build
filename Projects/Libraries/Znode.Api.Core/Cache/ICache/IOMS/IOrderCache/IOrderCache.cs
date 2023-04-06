using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;

namespace Znode.Engine.Api.Cache
{
    public interface IOrderCache
    {
        /// <summary>
        /// Get the list of all orders.
        /// </summary>
        /// <param name="routeUri">URI to route.</param>
        /// <param name="routeTemplate">Template of route.</param>
        /// <returns>list of order in string format by serializing it.</returns>
        string GetOrderList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get the list of all group orders.
        /// </summary>
        /// <param name="routeUri">URI to route.</param>
        /// <param name="routeTemplate">Template of route.</param>
        /// <returns>list of order in string format by serializing it.</returns>
        string GetGroupOrderList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get order details by order id.
        /// </summary>
        /// <param name="orderId">OrderId</param>
        /// <param name="routeUri">URI to route.</param>
        /// <param name="routeTemplate">Template of route.</param>
        /// <returns>order details</returns>
        string GetOrderById(int orderId, string routeUri, string routeTemplate);

        /// <summary>         
        /// Get order details by order id.         
        /// </summary>         
        /// <param name="orderNumber">orderNumber</param>         
        /// <param name="routeUri">URI to route.</param>         
        /// <param name="routeTemplate">Template of route.</param>         
        /// <returns>order details</returns>         
        string GetOrderByOrderNumber(string orderNumber, string routeUri, string routeTemplate);

        /// <summary>
        /// Get order invoice details.
        /// </summary>
        /// <param name="model">ParameterModel</param>
        /// <param name="routeUri">URI to route.</param>
        /// <param name="routeTemplate">Template of route.</param>
        /// <returns>list of order invoice in string format by serializing it.</returns>
        string GetOrderDetailsForInvoice(ParameterModel model, string routeUri, string routeTemplate);

        /// <summary>
        /// Get OrderLine Items With Refund payment left 
        /// </summary>
        /// <param name="orderDetailsId">orderDetailsId</param>
        /// <param name="routeUri">URI to route.</param>
        /// <param name="routeTemplate">Template of route.</param>
        /// <returns>list of Order Line Items with refund payment left</returns>
        string GetOrderLineItemsWithRefund(int orderDetailsId, string routeUri, string routeTemplate);

        /// <summary>
        /// Resend order confirmation email
        /// </summary>
        /// <param name="orderId">OrderId</param>
        /// <param name="routeUri">URI to route.</param>
        /// <param name="routeTemplate">Template of route.</param>
        /// <returns>Email message</returns>
        string ResendOrderConfirmationEmail(int orderId, string routeUri, string routeTemplate);

        /// <summary>
        /// Resend email for line item.
        /// </summary>
        /// <param name="orderId">orderId</param>
        /// <param name="omsOrderLineItemId">omsOrderLineItemId</param>
        /// <param name="routeTemplate">routeTemplate</param>
        /// <returns></returns>
        string ResendOrderLineItemConfirmationEmail(int orderId, int omsOrderLineItemId, string routeTemplate);

        /// <summary>
        /// Get order by order line item id for reordering a single product.
        /// </summary>
        /// <param name="orderLineItemId">Order line item id for an order.</param>
        ///  <param name="routeUri">URI to route.</param>
        ///  <param name="routeTemplate">Template of route.</param>
        /// <returns>Returns order data.</returns>
        string GetOrderByOrderLineItemId(int orderLineItemId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get Order Note List.
        /// </summary>
        /// <param name="omsOrderId">Order Id</param>
        /// <param name="omsQuoteId">omsQuoteId.</param>
        ///  <param name="routeUri">URI to route.</param>
        ///  <param name="routeTemplate">Template of route.</param>
        /// <returns>Returns order note data.</returns>
        string GetOrderNoteList(int omsOrderId, int omsQuoteId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get order payment state
        /// </summary>
        /// <param name="routeUri">URI to route.</param>
        /// <param name="routeTemplate">routeTemplate to route.</param>
        /// <returns> Return payment data list.</returns>
        string GetOrderPaymentState(string routeUri, string routeTemplate);

        /// <summary>
        /// Send returned order email
        /// </summary>
        /// <param name="orderId">OrderId</param>
        /// <param name="routeUri">URI to route.</param>
        /// <param name="routeTemplate">Template of route.</param>
        /// <returns>true/false</returns>
        string SendReturnedOrderEmail(int orderId, string routeUri, string routeTemplate);

        /// <summary>
        ///  Get order Receipt details.
        /// </summary>
        /// <param name="orderId">OrderId to get order details</param>
        /// <param name="routeUri">URI to route.</param>
        /// <param name="routeTemplate">Template of route.</param>
        /// <returns>order details</returns>
        string GetOrderReceiptByOrderId(int orderId, string routeUri, string routeTemplate);


        /// <summary>
        /// Get Address List With Shipment
        /// </summary>
        /// <param name="orderId">OrderId to get shipment details</param>
        /// <param name="userId">UserId to get address details</param>
        /// <param name="routeUri">URI to route.</param>
        /// <param name="routeTemplate">Template of route.</param>
        /// <returns>string</returns>
        string GetAddressListWithShipment(int orderId, int userId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get the list of failed orders transaction.
        /// </summary>
        /// <param name="routeUri">URI to route.</param>
        /// <param name="routeTemplate">Template of route.</param>
        /// <returns>list of failed orders transaction.</returns>
        FailedOrderTransactionListResponse FailedOrderTransactionList(string routeUri, string routeTemplate);

    }
}