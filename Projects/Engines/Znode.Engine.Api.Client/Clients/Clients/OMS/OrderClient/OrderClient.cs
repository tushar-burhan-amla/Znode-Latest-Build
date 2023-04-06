using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Api.Client
{
    public class OrderClient : BaseClient, IOrderClient
    {
        // Gets the list of orders.
        public virtual OrdersListModel GetOrderList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = OrderEndpoint.List();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ZnodeLogging.LogMessage($"OrderClient.GetOrderList : {endpoint}", "OrderCreate", System.Diagnostics.TraceLevel.Info);

            //Get response
            ApiStatus status = new ApiStatus();
            OrderListResponse response = GetResourceFromEndpoint<OrderListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            OrdersListModel list = new OrdersListModel { Orders = response?.OrderList?.Orders, CustomerName = response?.OrderList?.CustomerName };
            list.MapPagingDataFromResponse(response);

            return list;

        }

        // Gets the list of orders.
        public virtual OrdersListModel GetGroupOrderList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = OrderEndpoint.GetGroupOrderList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ZnodeLogging.LogMessage($"OrderClient.GetGroupOrderList : {endpoint}", "OrderCreate", System.Diagnostics.TraceLevel.Info);

            //Get response
            ApiStatus status = new ApiStatus();
            OrderListResponse response = GetResourceFromEndpoint<OrderListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            OrdersListModel list = new OrdersListModel { Orders = response?.OrderList?.Orders, CustomerName = response?.OrderList?.CustomerName };
            list.MapPagingDataFromResponse(response);

            return list;

        }

        // Create new order.
        public virtual OrderModel CreateOrder(ShoppingCartModel model)
        {
            //Get Endpoint.
            string endpoint = OrderEndpoint.Create();

            //Get response
            ApiStatus status = new ApiStatus();
            OrderResponse response = PostResourceToEndpoint<OrderResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.Created };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.Order;
        }

        // Update existing order.
        public virtual OrderModel UpdateOrder(OrderModel model)
        {
            //Get Endpoint.
            string endpoint = OrderEndpoint.Update();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            OrderResponse response = PutResourceToEndpoint<OrderResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.Order;
        }

        // Update existing order.
        public virtual OrderPaymentModel GetOrderDetailsForPayment(OrderPaymentCreateModel model)
        {
            //Get Endpoint.
            string endpoint = OrderEndpoint.GetOrderDetailsForPayment();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            OrderPaymentResponse response = PostResourceToEndpoint<OrderPaymentResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.OrderPaymentDetails;
        }

        //Get order details by order id.
        public virtual OrderModel GetOrder(int orderId, ExpandCollection expands)
        {
            //Get Endpoint.
            string endpoint = OrderEndpoint.Get(orderId);
            endpoint += BuildEndpointQueryString(expands);

            //Get response
            ApiStatus status = new ApiStatus();
            OrderResponse response = GetResourceFromEndpoint<OrderResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.Order;
        }

        //Get Products Details.
        public virtual ProductDetailsListModel GetProducts(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = SEOSettingEndpoints.GetPublishedProducts();
            endpoint += BuildEndpointQueryString(null, filters, sorts, pageIndex, pageSize);

            //Get response
            ApiStatus status = new ApiStatus();
            ProductDetailsListResponse response = GetResourceFromEndpoint<ProductDetailsListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            ProductDetailsListModel list = new ProductDetailsListModel { ProductDetailList = response?.ProductDetailList };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        // Create new Customer.
        public virtual UserAddressModel CreateNewCustomer(UserAddressModel userAddressModel)
        {
            //Get Endpoint.
            string endpoint = OrderEndpoint.CreateNewCustomer();

            //Get response
            ApiStatus status = new ApiStatus();
            UserAddressResponse response = PostResourceToEndpoint<UserAddressResponse>(endpoint, JsonConvert.SerializeObject(userAddressModel), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.Created };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.userAddress;
        }

        // Get order details by order id.
        public virtual OrderModel GetOrderById(int orderId, ExpandCollection expands, FilterCollection filters = null)
        {
            string endpoint = OrderEndpoint.Get(orderId);
            endpoint += BuildEndpointQueryString(expands, filters, null, null, null);

            ApiStatus status = new ApiStatus();
            OrderResponse response = GetResourceFromEndpoint<OrderResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.Order;
        }

        //Get order Receipt details.
        public virtual OrderModel GetOrderReceiptByOrderId(int orderId)
        {
            string endpoint = OrderEndpoint.GetOrderReceiptByOrderId(orderId);

            ApiStatus status = new ApiStatus();
            OrderResponse response = GetResourceFromEndpoint<OrderResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.Order;
        }

        //Get order invoice details.
        public virtual OrdersListModel GetOrderDetailsForInvoice(ParameterModel filterIds, ExpandCollection expands, FilterCollection filters = null)
        {
            //Get Endpoint.
            string endpoint = OrderEndpoint.GetOrderDetailsForInvoice();
            endpoint += BuildEndpointQueryString(expands, filters, null, null, null);

            //Get response
            ApiStatus status = new ApiStatus();
            OrderListResponse response = PostResourceToEndpoint<OrderListResponse>(endpoint, JsonConvert.SerializeObject(filterIds), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            OrdersListModel list = new OrdersListModel { Orders = (Equals(response?.OrderList, null)) ? null : response?.OrderList?.Orders };
            return list;
        }

        //Update Order Payment Status
        public virtual bool UpdateOrderPaymentStatus(int orderId, string paymentStatus)
        {
            string endpoint = OrderEndpoint.UpdateOrderPaymentStatus(orderId, paymentStatus);
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = GetResourceFromEndpoint<TrueFalseResponse>(endpoint, status);

            //check the status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        //Update tracking Number
        public virtual bool UpdateTrackingNumber(int orderId, string trackingNumber)
        {
            string endpoint = OrderEndpoint.UpdateTrackingNumber(orderId, trackingNumber);
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = GetResourceFromEndpoint<TrueFalseResponse>(endpoint, status);

            //check the status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }
        //Get OrderLine Items With Refund payment left
        public virtual OrderItemsRefundModel GetOrderLineItemsWithRefund(int orderDetailsId)
        {
            string endpoint = OrderEndpoint.GetOrderLineItemsWithRefund(orderDetailsId);
            ApiStatus status = new ApiStatus();
            OrderResponse response = GetResourceFromEndpoint<OrderResponse>(endpoint, status);

            //check the status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.RefundPayment;
        }

        // Create new Customer.
        public virtual bool AddRefundPaymentDetails(OrderItemsRefundModel refundPaymentListModel)
        {
            //Get Endpoint.
            string endpoint = OrderEndpoint.AddRefundPaymentDetails();

            //Get response
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(refundPaymentListModel), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response.IsSuccess;
        }

        //Get order details by order id.
        public virtual bool ResendOrderConfirmationEmail(int orderId, FilterCollection filters, ExpandCollection expands)
        {
            string endpoint = OrderEndpoint.ResendOrderConfirmationEmail(orderId);
            endpoint += BuildEndpointQueryString(expands, filters, null, null, null);
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = GetResourceFromEndpoint<TrueFalseResponse>(endpoint, status);

            //check the status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        //Get order details by order id.
        public virtual bool ResendOrderEmailForCartLineItem(int orderId, int omsOrderLineItemId, ExpandCollection expands)
        {
            string endpoint = OrderEndpoint.ResendOrderEmailForCartLineItem(orderId, omsOrderLineItemId);
            endpoint += BuildEndpointQueryString(expands, null, null, null, null);
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = GetResourceFromEndpoint<TrueFalseResponse>(endpoint, status);

            //check the status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        //Update order status.
        public virtual bool UpdateOrderStatus(OrderStateParameterModel model)
        {
            //Get Endpoint.
            string endpoint = OrderEndpoint.UpdateOrderStatus();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PutResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(model), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        //Update order Shipping Billing Address.
        public virtual bool UpdateOrderAddress(AddressModel model)
        {
            //Get Endpoint.
            string endpoint = OrderEndpoint.UpdateOrderAddress();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PutResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(model), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        //Update order status, external Id and Order Notes by Order number.
        public virtual bool UpdateOrderDetailsByOrderNumber(OrderDetailsModel model)
        {
            //Get Endpoint.
            string endpoint = OrderEndpoint.UpdateOrderDetailsByOrderNumber();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PutResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(model), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        //Get order details by order id.
        public virtual OrderModel GetOrderByOrderLineItemId(int orderLineItemId, ExpandCollection expands)
        {
            //Get Endpoint.
            string endpoint = OrderEndpoint.GetOrderByOrderLineItemId(orderLineItemId);
            endpoint += BuildEndpointQueryString(expands);

            //Get response
            ApiStatus status = new ApiStatus();
            OrderResponse response = GetResourceFromEndpoint<OrderResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.Order;
        }

        //Add new order Note.
        public virtual bool AddOrderNote(OrderNotesModel orderNotesModel)
        {
            string endpoint = OrderEndpoint.AddOrderNote();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(orderNotesModel), status);

            //check the status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        //Get Order Notes List.
        public virtual OrderNotesListModel GetOrderNotesList(int omsOrderId, int omsQuoteId)
        {
            //Get Endpoint.
            string endpoint = OrderEndpoint.GetOrderNotesList(omsOrderId, omsQuoteId);

            //Get response
            ApiStatus status = new ApiStatus();
            OrderListResponse response = GetResourceFromEndpoint<OrderListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            OrderNotesListModel list = new OrderNotesListModel { OrderNotes = response?.OrderNotesList?.OrderNotes };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        // Check inventory and min/max quantity.
        public virtual OrderModel CheckInventoryAndMinMaxQuantity(ShoppingCartModel shoppingCartModel)
        {
            //Get Endpoint.
            string endpoint = OrderEndpoint.CheckInventoryAndMinMaxQuantity();

            //Get response
            ApiStatus status = new ApiStatus();
            OrderResponse response = PostResourceToEndpoint<OrderResponse>(endpoint, JsonConvert.SerializeObject(shoppingCartModel), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.Created };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.Order;
        }

        public virtual PaymentStateListModel GetPaymentStateList()
        {
            string endpoint = OrderEndpoint.GetPaymentStateList();

            //Get response
            ApiStatus status = new ApiStatus();
            OrderPaymentStateResponse response = GetResourceFromEndpoint<OrderPaymentStateResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return new PaymentStateListModel { PaymentStateList = response?.PaymentStateList };
        }

        // Create new order History.
        public virtual OrderHistoryModel CreateOrderHistory(OrderHistoryModel orderHistoryModel)
        {
            //Get Endpoint.
            string endpoint = OrderEndpoint.createOrderHistory();

            //Get response
            ApiStatus status = new ApiStatus();
            OrderHistoryResponse response = PostResourceToEndpoint<OrderHistoryResponse>(endpoint, JsonConvert.SerializeObject(orderHistoryModel), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.Created };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.OrderHistory;
        }

        //Get order state by OmsOrderStateId.
        public virtual OrderStateModel GetOrderStateValueById(int omsOrderStateId)
        {
            //Get Endpoint.
            string endpoint = OrderEndpoint.GetOrderStateValueById(omsOrderStateId);

            //Get response
            ApiStatus status = new ApiStatus();
            OrderStateResponses response = GetResourceFromEndpoint<OrderStateResponses>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.Created };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.orderState;
        }

        // Send returned order mail.
        public virtual bool SendReturnedOrderEmail(int orderId, ExpandCollection expands, FilterCollection filters)
        {
            string endpoint = OrderEndpoint.SendReturnedOrderEmail(orderId);
            endpoint += BuildEndpointQueryString(expands, filters, null, null, null);

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = GetResourceFromEndpoint<TrueFalseResponse>(endpoint, status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        public virtual bool SendPOEmail(SendInvoiceModel model)
        {
            string endpoint = OrderEndpoint.SendPOEmail();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PutResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(model), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);
            return response.HasError;
        }

        //Update order Transaction Id
        public virtual bool UpdateOrderTransactionId(int orderId, string transactionId)
        {
            string endpoint = OrderEndpoint.UpdateOrderTransactionId(orderId, transactionId);
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = GetResourceFromEndpoint<TrueFalseResponse>(endpoint, status);

            //check the status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        public virtual bool ReorderSinglelineItemOrder(int OmsOrderLineItemsId, int portalId, int userId = 0)
        {
            string endpoint = OrderEndpoint.ReorderSinglelineItemOrder(OmsOrderLineItemsId, portalId, userId);
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = GetResourceFromEndpoint<TrueFalseResponse>(endpoint, status);

            //check the status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        

        //Reorder Complete Order
        public virtual bool ReorderCompleteOrder(int orderId, int portalId, int userId = 0)
        {
            string endpoint = OrderEndpoint.ReorderCompleteOrder(orderId, portalId, userId);
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = GetResourceFromEndpoint<TrueFalseResponse>(endpoint, status);

            //check the status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        //Get the address list.
        public virtual AddressListModel GetAddressListWithShipment(int orderId, int userId)
        {
            //Get endpoint having api url.
            string endpoint = OrderEndpoint.GetAddressListWithShipment(orderId, userId);

            ApiStatus status = new ApiStatus();
            AddressListResponse response = GetResourceFromEndpoint<AddressListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };

            //Check status of response.
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            AddressListModel list = new AddressListModel { AddressList = response?.AddressList };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        // Gets the list of failed orders transactions.
        public virtual FailedOrderTransactionListModel FailedOrderTransactionList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            //Get Endpoint.
            string endpoint = OrderEndpoint.FailedOrderTransactionList();
            endpoint += BuildEndpointQueryString(null, filters, sorts, pageIndex, pageSize);
            
            //Get response
            ApiStatus status = new ApiStatus();
            FailedOrderTransactionListResponse response = GetResourceFromEndpoint<FailedOrderTransactionListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            FailedOrderTransactionListModel list = new FailedOrderTransactionListModel { FailedOrderTransactionModel = response.FailedOrderTransactionListModel.FailedOrderTransactionModel };
            list.MapPagingDataFromResponse(response);

            return list;

        }

        // Save Order Payment Detail
        public virtual bool SaveOrderPaymentDetail(OrderPaymentModel model)
        {
            string endpoint = OrderEndpoint.SaveOrderPaymentDetail();
            ApiStatus status = new ApiStatus();

            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.Created };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response.IsSuccess;
        }
    }
}
