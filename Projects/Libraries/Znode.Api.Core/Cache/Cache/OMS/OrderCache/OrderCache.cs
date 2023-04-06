using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Api.Cache
{
    public class OrderCache : BaseCache, IOrderCache
    {
        #region Private Variables
        private readonly IOrderService _orderService;
        #endregion

        #region Constructor
        public OrderCache(IOrderService orderService)
        {
            _orderService = orderService;
        }
        #endregion

        #region Public Methods
        public virtual string GetOrderList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get data from service
                OrdersListModel orderList = _orderService.GetOrderList(Expands, Filters, Sorts, Page);
                if (orderList?.Orders?.Count > 0 || IsNotNull(orderList?.CustomerName))
                {
                    OrderListResponse response = new OrderListResponse { OrderList = orderList };

                    response.MapPagingDataFromModel(orderList);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get list of group orders.
        public virtual string GetGroupOrderList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get data from service
                OrdersListModel orderList = _orderService.GetGroupOrderList(Expands, Filters, Sorts, Page);
                if (orderList?.Orders?.Count > 0 || IsNotNull(orderList?.CustomerName))
                {
                    OrderListResponse response = new OrderListResponse { OrderList = orderList };

                    response.MapPagingDataFromModel(orderList);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetOrderById(int orderId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get data from service
                OrderModel order = _orderService.GetOrderById(orderId, Filters, Expands);
                if (IsNotNull(order))
                {
                    OrderResponse response = new OrderResponse { Order = order };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetOrderByOrderNumber(string orderNumber, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get data from service                 
                OrderModel order = _orderService.GetOrderByOrderNumber(orderNumber, Filters, Expands);
                if (IsNotNull(order))
                {
                    OrderResponse response = new OrderResponse { Order = order };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Resend order confirmation email.
        public virtual string ResendOrderConfirmationEmail(int orderId, string routeUri, string routeTemplate)
        {
            StringResponse response = new StringResponse { Response = _orderService.ResendOrderConfirmationEmail(orderId, Filters, Expands).ToString() };

            return response.Response;
        }

        //Resend order confirmation email.
        public virtual string ResendOrderLineItemConfirmationEmail(int orderId, int omsOrderLineItemId, string routeTemplate)
        {
            StringResponse response = new StringResponse { Response = _orderService.ResendOrderLineItemConfirmationEmail(orderId, omsOrderLineItemId.ToString(), Expands).ToString() };

            return response.Response;
        }

        //Get order invoice details.
        public virtual string GetOrderDetailsForInvoice(ParameterModel model, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get data from service
                OrdersListModel order = _orderService.GetOrderDetailsForInvoice(model, Expands, Filters);
                if (IsNotNull(order))
                {
                    OrderListResponse response = new OrderListResponse { OrderList = order };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get OrderLine Items With Refund payment left .
        public virtual string GetOrderLineItemsWithRefund(int orderDetailsId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get data from service
                OrderItemsRefundModel refundPaymentModel = _orderService.GetOrderLineItemsWithRefund(orderDetailsId);
                if (IsNotNull(refundPaymentModel?.RefundOrderLineitems?.Count > 0))
                {
                    OrderResponse response = new OrderResponse { RefundPayment = refundPaymentModel };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get Order by OrderLineItemId.
        public virtual string GetOrderByOrderLineItemId(int orderLineItemId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get data from service
                OrderModel order = _orderService.GetOrderByOrderLineItemId(orderLineItemId, Expands);
                if (IsNotNull(order))
                {
                    OrderResponse response = new OrderResponse { Order = order };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get Order Note List by order id.
        public virtual string GetOrderNoteList(int omsOrderId, int omsQuoteId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get data from service
                OrderNotesListModel orderNotesList = _orderService.GetOrderNoteList(omsOrderId, omsQuoteId);
                if (orderNotesList?.OrderNotes?.Count > 0)
                {
                    OrderListResponse response = new OrderListResponse { OrderNotesList = orderNotesList };

                    response.MapPagingDataFromModel(orderNotesList);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get Payment Status.

        public virtual string GetOrderPaymentState(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get data from service
                OrderPaymentStateResponse response = new OrderPaymentStateResponse() { PaymentStateList = _orderService.GetOrderPaymentState() };
                data = InsertIntoCache(routeUri, routeTemplate, response);
            }
            return data;
        }

        // Send returned order mail.
        public virtual string SendReturnedOrderEmail(int orderId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                StringResponse response = new StringResponse { Response = _orderService.SendReturnedOrderEmail(orderId, Expands, Filters).ToString() };

                return response.Response;
            }

            return data;
        }

        // Get order Receipt details.
        public virtual string GetOrderReceiptByOrderId(int orderId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get data from service
                OrderModel order = _orderService.GetOrderReceiptByOrderId(orderId);
                if (IsNotNull(order))
                {
                    OrderResponse response = new OrderResponse { Order = order };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public virtual string GetAddressListWithShipment(int orderId, int userId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Get data from service
                AddressListModel addressList = _orderService.GetAddressListWithShipment(orderId, userId);
                if (IsNotNull(addressList))
                {
                    AddressListResponse response = new AddressListResponse { AddressList = addressList.AddressList };

                    response.MapPagingDataFromModel(addressList);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        // Get realtime failed order transaction list So request directly passed to service.
        public virtual FailedOrderTransactionListResponse FailedOrderTransactionList(string routeUri, string routeTemplate)
        {
            //Get data from service
            FailedOrderTransactionListModel failedOrderTransactionListModel = _orderService.FailedOrderTransactionList(Expands, Filters, Sorts, Page);
            FailedOrderTransactionListResponse failedOrderTransactionListResponse = new FailedOrderTransactionListResponse {
                FailedOrderTransactionListModel = failedOrderTransactionListModel
            };
            failedOrderTransactionListResponse.MapPagingDataFromModel(failedOrderTransactionListModel);
            return failedOrderTransactionListResponse;
        }
        #endregion
    }
}