using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Helper;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Api.Controllers.OMS
{
    public class OrderController : BaseController
    {
        #region Private Variables
        private readonly IOrderCache _orderCache;
        private readonly IOrderService _orderService;
        #endregion

        #region Constructor
        public OrderController(IOrderService orderService)
        {
            _orderService = orderService;
            _orderCache = new OrderCache(_orderService);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Get the list of all Orders.
        /// </summary>
        /// <returns>Returns list of all orders.</returns>
        [ResponseType(typeof(OrderListResponse))]
        [HttpGet]
        public HttpResponseMessage List()
        {
            HttpResponseMessage response;
            try
            {
                string data = _orderCache.GetOrderList(RouteUri, RouteTemplate);
                response = string.IsNullOrEmpty(data) ? CreateNoContentResponse() : CreateOKResponse<OrderListResponse>(data);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new OrderListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new OrderListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get the list of all group orders.
        /// </summary>
        /// <returns>Returns list of all group orders.</returns>
        [ResponseType(typeof(OrderListResponse))]
        [HttpGet]
        public HttpResponseMessage GetGroupOrderList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _orderCache.GetGroupOrderList(RouteUri, RouteTemplate);
                response = string.IsNullOrEmpty(data) ? CreateNoContentResponse() : CreateOKResponse<OrderListResponse>(data);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new OrderListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new OrderListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }


        /// <summary>
        /// Create new order.
        /// </summary>
        /// <param name="shoppingCartModel">shopping cart model.</param>
        /// <returns>Creates new order.</returns>
        [ResponseType(typeof(OrderResponse))]
        [HttpPost, ValidateModel]
        public HttpResponseMessage Create([FromBody] ShoppingCartModel shoppingCartModel)
        {
            HttpResponseMessage response;
            try
            {
                OrderModel order = _orderService.CreateOrder(shoppingCartModel);

                response = IsNotNull(order) ? CreateCreatedResponse(new OrderResponse { Order = order }) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new OrderResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                string message = "";
                if (!string.IsNullOrEmpty(ex.InnerException?.InnerException?.Message))
                    message = string.Concat("Order detail is not valid. Message Detail:", ex.InnerException?.InnerException?.Message);
                else
                    message = ex.Message;

                response = CreateInternalServerErrorResponse(new OrderResponse { HasError = true, ErrorMessage = message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Create new order History.
        /// </summary>
        /// <param name="orderHistoryModel">Order History Model.</param>
        /// <returns>Creates new order History.</returns>
        [ResponseType(typeof(OrderHistoryResponse))]
        [HttpPost, ValidateModel]
        public HttpResponseMessage CreateOrderHistory([FromBody] OrderHistoryModel orderHistoryModel)
        {
            HttpResponseMessage response;
            try
            {
                OrderHistoryModel orderHistory = _orderService.CreateOrderHistory(orderHistoryModel);

                response = IsNotNull(orderHistory) ? CreateCreatedResponse(new OrderHistoryResponse { OrderHistory = orderHistory }) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new OrderHistoryResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new OrderHistoryResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Update existing order.
        /// </summary>
        /// <param name="model"></param>
        /// <returns>Updates existing order.</returns>
        [ResponseType(typeof(OrderResponse))]
        [HttpPut, ValidateModel]
        public HttpResponseMessage Update([FromBody] OrderModel model)
        {
            HttpResponseMessage response;

            try
            {
                OrderModel order = _orderService.UpdateOrder(model);
                response = IsNotNull(order) ? CreateOKResponse(new OrderResponse { Order = order }) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new OrderResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new OrderResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }

            return response;
        }

        [ResponseType(typeof(OrderPaymentResponse))]
        [HttpPost, ValidateModel]
        public HttpResponseMessage GetOrderDetailsForPayment([FromBody] OrderPaymentCreateModel model)
        {
            HttpResponseMessage response;

            try
            {
                OrderPaymentModel orderPaymentDetails = _orderService.GetOrderDetailsForPayment(model);
                response = IsNotNull(orderPaymentDetails) ? CreateOKResponse(new OrderPaymentResponse { OrderPaymentDetails = orderPaymentDetails }) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new OrderPaymentResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new OrderPaymentResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Update order status.
        /// </summary>
        /// <param name="model">OrderStateParameterModel</param>
        /// <returns>Returns true if updated successfully else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPut]
        public HttpResponseMessage UpdateOrderStatus([FromBody] OrderStateParameterModel model)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _orderService.UpdateOrderStatus(model) });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Update order Shipping Billing Address.
        /// </summary>
        /// <param name="model">AddressModel</param>
        /// <returns>Returns true if updated successfully else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPut]
        public HttpResponseMessage UpdateOrderAddress([FromBody] AddressModel model)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _orderService.UpdateOrderAddress(model) });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Update order status, external id and order notes by order number.
        /// </summary>
        /// <param name="model">Order details model containing order number, order status code and name, order notes and external id.</param>
        /// <returns>Returns true if order details are updated successfully else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPut, ValidateModel]
        public HttpResponseMessage UpdateOrderDetailsByOrderNumber([FromBody] OrderDetailsModel model)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _orderService.UpdateOrderDetailsByOrderNumber(model) });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Get order details by order id.
        /// </summary>
        /// <param name="orderId">order Id</param>
        /// <returns>Get order details.</returns>
        [ResponseType(typeof(OrderResponse))]
        public HttpResponseMessage Get(int orderId)
        {
            HttpResponseMessage response;

            try
            {
                string data = _orderCache.GetOrderById(orderId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<OrderResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                if (ex.ErrorCode == ErrorCodes.NotPermitted)
                {
                    response = CreateUnauthorizedResponse(new OrderResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                }
                else
                {
                    response = CreateInternalServerErrorResponse(new OrderResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                }
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new OrderResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>         
        /// Get order details by order number.         
        /// </summary>         
        /// <param name="orderNumber">order number</param>         
        /// <returns>Get order details.</returns>         
        [ResponseType(typeof(OrderResponse))]
        public HttpResponseMessage GetByOrderNumber(string orderNumber)
        {
            HttpResponseMessage response;
            try
            {
                string data = _orderCache.GetOrderByOrderNumber(orderNumber, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<OrderResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new OrderResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new OrderResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Resend order confirmation receipt via email.
        /// </summary>
        /// <returns>Email message.</returns>
        [ResponseType(typeof(StringResponse))]
        [HttpGet]
        public HttpResponseMessage ResendOrderConfirmationEmail(int orderId)
        {
            HttpResponseMessage response;

            try
            {
                string status = _orderCache.ResendOrderConfirmationEmail(orderId, RouteUri, RouteTemplate);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = Equals(status.ToLower(), "true")});
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Resend order confirmation mail for single cartitem.
        /// </summary>
        /// <param name="orderId">orderId</param>
        /// <param name="cartItemId">cartItemId</param>
        /// <returns>Returns true if successfull else returns false.</returns>
        [ResponseType(typeof(StringResponse))]
        [HttpGet]
        public HttpResponseMessage ResendOrderEmailForCartLineItem(int orderId, int cartItemId)
        {
            HttpResponseMessage response;
            try
            {
                string status = _orderCache.ResendOrderLineItemConfirmationEmail(orderId, cartItemId, RouteTemplate);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = Equals(status.ToLower(), "true")});
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Create new user.
        /// </summary>
        /// <param name="userAddressModel">user model.</param>
        /// <returns>Creates new user.</returns>
        [ResponseType(typeof(UserAddressResponse))]
        [HttpPost, ValidateModel]
        public HttpResponseMessage CreateNewCustomer([FromBody] UserAddressModel userAddressModel)
        {
            HttpResponseMessage response;
            try
            {
                userAddressModel = _orderService.CreateNewCustomer(userAddressModel);
                if (IsNotNull(userAddressModel))
                {
                    response = CreateCreatedResponse(new UserAddressResponse { userAddress = userAddressModel });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(userAddressModel.UserId)));
                }
                else
                    response = CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new UserAddressResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new UserAddressResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get order invoice details.
        /// </summary>
        /// <param name="filterIds">selected order ids</param>
        /// <returns>Gets order invoice details.</returns>
        [ResponseType(typeof(OrderListResponse))]
        [HttpPost]
        public HttpResponseMessage GetOrderDetailsForInvoice([FromBody] ParameterModel filterIds)
        {
            HttpResponseMessage response;
            try
            {
                string data = _orderCache.GetOrderDetailsForInvoice(filterIds, RouteUri, RouteTemplate);
                response = string.IsNullOrEmpty(data) ? CreateNoContentResponse() : CreateOKResponse<OrderListResponse>(data);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new OrderListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new OrderListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Update order payment status.
        /// </summary>
        /// <param name="orderId">orderId</param>
        /// <param name="paymentStatus">paymentStatus</param>
        /// <returns>Returns true if successfull else returns false</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpGet]
        public HttpResponseMessage UpdateOrderPaymentStatus(int orderId, string paymentStatus)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _orderService.UpdateOrderPaymentStatus(orderId, paymentStatus) });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Update order tracking Number.
        /// </summary>
        /// <param name="orderId">orderId</param>
        /// <param name="trackingNumber">trackingNumber</param>
        /// <returns>Returns true if successfull else returns false</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpGet, HttpPut]
        public HttpResponseMessage UpdateTrackingNumber(int orderId, string trackingNumber)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _orderService.UpdateTrackingNumber(orderId, trackingNumber) });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>        
        /// Update order tracking Number.        
        /// </summary>        
        /// <param name="orderNumber">orderNumber</param>        
        /// <param name="trackingNumber">trackingNumber</param>        
        /// <returns>Returns true if successfull else returns false</returns>        
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPut]
        public HttpResponseMessage UpdateTrackingByOrderNumber(string orderNumber, string trackingNumber)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _orderService.UpdateTrackingNumberByOrderNumber(orderNumber, trackingNumber) });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Add refund payment details.
        /// </summary>
        /// <param name="refundPaymentListModel">refundPaymentListModel</param>
        /// <returns>Returns true if successfull else returns false</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public HttpResponseMessage AddRefundPaymentDetails([FromBody] OrderItemsRefundModel refundPaymentListModel)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _orderService.AddRefundPaymentDetails(refundPaymentListModel) });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get orderline items with refund payment left. 
        /// </summary>
        /// <param name="orderDetailsId">orderDetailsId</param>
        /// <returns>Gets orderline items with refund payment left.</returns>
        [ResponseType(typeof(OrderResponse))]
        [HttpGet]
        public HttpResponseMessage GetOrderLineItemsWithRefund(int orderDetailsId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _orderCache.GetOrderLineItemsWithRefund(orderDetailsId, RouteUri, RouteTemplate);
                response = string.IsNullOrEmpty(data) ? CreateNoContentResponse() : CreateOKResponse<OrderResponse>(data);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new OrderResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get order details of a single product by order line item id.
        /// </summary>
        /// <param name="orderLineItemId">order line item id.</param>
        /// <returns> Get order details of a single product.</returns>
        [ResponseType(typeof(OrderResponse))]
        public HttpResponseMessage GetOrderByOrderLineItemId(int orderLineItemId)
        {
            HttpResponseMessage response;

            try
            {
                string data = _orderCache.GetOrderByOrderLineItemId(orderLineItemId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<OrderResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new OrderResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Add new order note.
        /// </summary>
        /// <param name="orderNotesModel">orderNotesModel contains notes details.</param>
        /// <returns>Returns true if successfull else returns false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public HttpResponseMessage AddOrderNote([FromBody] OrderNotesModel orderNotesModel)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _orderService.AddOrderNote(orderNotesModel) });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Get order note list.
        /// </summary>
        /// <param name="omsOrderId">order id.</param>
        /// <param name="omsQuoteId">oms Quote Id.</param>
        /// <returns>Gets order note list.</returns>
        [ResponseType(typeof(OrderListResponse))]
        [HttpGet]
        public HttpResponseMessage OrderNoteList(int omsOrderId, int omsQuoteId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _orderCache.GetOrderNoteList(omsOrderId, omsQuoteId, RouteUri, RouteTemplate);
                response = string.IsNullOrEmpty(data) ? CreateNoContentResponse() : CreateOKResponse<OrderListResponse>(data);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new OrderListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get Payment State list.
        /// </summary>
        /// <returns>Gets payment state list.</returns>
        [ResponseType(typeof(OrderPaymentStateResponse))]
        [HttpGet]
        public HttpResponseMessage GetPaymentStateList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _orderCache.GetOrderPaymentState(RouteUri, RouteTemplate);
                response = string.IsNullOrEmpty(data) ? CreateNoContentResponse() : CreateOKResponse<OrderPaymentStateResponse>(data);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new OrderPaymentStateResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Check inventory and min/max quantity.
        /// </summary>
        /// <param name="shoppingCartModel">shopping cart model.</param>
        /// <returns>Checks inventory and min/max quantity.</returns>
        [ResponseType(typeof(OrderResponse))]
        [HttpPost, ValidateModel]
        public HttpResponseMessage CheckInventoryAndMinMaxQuantity([FromBody] ShoppingCartModel shoppingCartModel)
        {
            HttpResponseMessage response;
            try
            {
                OrderModel order = _orderService.CheckInventoryAndMinMaxQuantity(shoppingCartModel);

                response = IsNotNull(order) ? CreateCreatedResponse(new OrderResponse { Order = order }) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new OrderResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new OrderResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get order state value.
        /// </summary>
        /// <param name="omsOrderStateId">omsOrderStateId</param>
        /// <returns>Gets order state value.</returns>
        [ResponseType(typeof(OrderStateResponses))]
        [HttpGet]
        public HttpResponseMessage GetOrderStateValueById(int omsOrderStateId)
        {
            HttpResponseMessage response;
            try
            {
                OrderStateModel model = _orderService.GetOrderStateValueById(omsOrderStateId);
                response = IsNotNull(model) ? CreateCreatedResponse(new OrderStateResponses { orderState = model }) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new OrderStateResponses { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Send returned order receipt via email.
        /// </summary>
        /// <returns>Returns true if successfull else returns false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpGet]
        public HttpResponseMessage SendReturnedOrderEmail(int orderId)
        {
            HttpResponseMessage response;
            try
            {
                string status = _orderCache.SendReturnedOrderEmail(orderId, RouteUri, RouteTemplate);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = Equals(status.ToLower(), "true")});
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;

        }

        [HttpPut]
        [ResponseType(typeof(OrderResponse))]
        public virtual HttpResponseMessage SendPOEmail([FromBody] SendInvoiceModel sendInvoiceModel)
        {
            HttpResponseMessage response;
            try
            {
                bool status = _orderService.SendPOEmail(sendInvoiceModel);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = status });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }

            return response;
        }

        /// <summary>
        /// Update order line item details.
        /// </summary>
        /// <param name="orderDetailsModel">Order Details Model.</param>
        /// <returns>HttpResponseMessage</returns>
        [ResponseType(typeof(OrderLineItemStatusResponse))]
        [HttpPost]
        public HttpResponseMessage UpdateOrderLineItems([FromBody]OrderLineItemDataListModel orderDetailsModel)
        {
            HttpResponseMessage response;
            try
            {
                OrderLineItemStatusListModel itemList = _orderService.UpdateOrderLineItems(orderDetailsModel);
                //Update order line item details.
                response = CreateOKResponse(new OrderLineItemStatusResponse { OrderLineItemStatusList = itemList });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new OrderLineItemStatusResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new OrderLineItemStatusResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Update Order Paypal Payment TransactionId
        /// </summary>
        /// <param name="orderId">orderId</param>
        /// <param name="transactionId">transactionId</param>
        /// <returns>Returns true if successfull else returns false</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpGet]
        public HttpResponseMessage UpdateOrderTransactionId(int orderId, string transactionId)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _orderService.UpdateOrderTransactionId(orderId, transactionId) });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new OrderResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Convert quote to the order.
        /// </summary>
        /// <param name="accountQuoteModel"></param>
        /// <returns>OrderResponse</returns>
        [ResponseType(typeof(OrderResponse))]
        [HttpPost, ValidateModel]
        public HttpResponseMessage ConvertToOrder([FromBody] AccountQuoteModel accountQuoteModel)
        {
            HttpResponseMessage response;
            try
            {
                OrderModel model = _orderService.ConvertToOrder(accountQuoteModel);
                response = IsNotNull(model) ? CreateCreatedResponse(new OrderResponse { Order = model }) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new OrderResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new OrderResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Reorder Complete Order
        /// </summary>
        /// <param name="orderId"></param>
        /// <param name="portalId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpGet]
        public HttpResponseMessage ReorderCompleteOrder(int orderId, int portalId, int userId = 0)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _orderService.ReorderCompleteOrder(orderId, portalId, userId) });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// reorder single line item.
        /// </summary>
        /// <param name="omsOrderLineItemsId"></param>
        /// <param name="portalId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpGet]
        public HttpResponseMessage ReorderSinglelineItemOrder(int omsOrderLineItemsId, int portalId, int userId = 0)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _orderService.ReorderCompleteOrder(0, portalId, userId, omsOrderLineItemsId) });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new OrderResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        ///  Get order Receipt details.
        /// </summary>
        /// <param name="orderId"> order Id to get Order Details</param>
        /// <returns> order details</returns>
        /// 
        [ResponseType(typeof(OrderResponse))]
        public HttpResponseMessage GetOrderReceiptByOrderId(int orderId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _orderCache.GetOrderReceiptByOrderId(orderId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<OrderResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new OrderResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new OrderResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Gets the address list for user Id.
        /// </summary>
        /// <returns>Returns Address list.</returns>
        [ResponseType(typeof(AddressListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAddressListWithShipment(int orderId, int userId)
        {
            HttpResponseMessage response;
            try
            {
                //Get account list.
                string data = _orderCache.GetAddressListWithShipment(orderId, userId, RouteUri, RouteTemplate);
                // TODO: Add info logs
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<AddressListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new AddressListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Customers.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Get the list of failed orders transaction.
        /// </summary>
        /// <returns>Returns list of all failed orders transaction.</returns>
        [ResponseType(typeof(FailedOrderTransactionListResponse))]
        [HttpGet]
        public HttpResponseMessage FailedOrderTransactionList()
        {
            HttpResponseMessage response;
            try
            {
                FailedOrderTransactionListResponse model = _orderCache.FailedOrderTransactionList(RouteUri, RouteTemplate);
                response = IsNotNull(model) ? CreateOKResponse(model) : CreateNoContentResponse();

            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new OrderListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }


        /// <summary>
        /// Save Order Payment Detail
        /// </summary>
        /// <param name="orderPaymentModel">orderPaymentModel</param>
        /// <returns></returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public HttpResponseMessage SaveOrderPaymentDetail([FromBody] OrderPaymentDataModel orderPaymentModel)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _orderService.SaveOrderPaymentDetail(orderPaymentModel) });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new OrderResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }

            return response;
        }


        #endregion
    }
}