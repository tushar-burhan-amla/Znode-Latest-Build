using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Helper;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Api.Models.Responses.V2;
using Znode.Engine.Api.Models.V2;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using Znode.Libraries.Resources;

namespace Znode.Engine.Api.Controllers
{
    public class OrderV2Controller : BaseController
    {
        #region Private Variables
        private readonly IOrderCacheV2 _orderCache;
        private readonly IOrderServiceV2 _orderService;
        #endregion

        #region Constructor
        public OrderV2Controller(IOrderServiceV2 orderService)
        {
            _orderService = orderService;
            _orderCache = new OrderCacheV2(_orderService);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Get the list of all Orders.
        /// </summary>
        /// <returns>HttpResponseMessage</returns>
        [ResponseType(typeof(OrderListResponseV2))]
        [HttpGet]
        public HttpResponseMessage List()
        {
            HttpResponseMessage response;
            try
            {
                string data = _orderCache.GetOrderListV2(RouteUri, RouteTemplate);
                response = string.IsNullOrEmpty(data) ? CreateNoContentResponse() : CreateOKResponse<OrderListResponseV2>(data);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex.ToString());
                response = CreateInternalServerErrorResponse(new OrderListResponseV2 { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new OrderListResponseV2 { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex.ToString(), ZnodeLogging.Components.OMS.ToString());
            }
            return response;
        }

        /// <summary>
        /// Create new order.
        /// </summary>
        /// <param name="shoppingCartModel">shopping cart model.</param>
        [ResponseType(typeof(OrderResponseV2))]
        [ValidateModel]
        [HttpPost]
        public HttpResponseMessage CreateV2(CreateOrderModelV2 shoppingCartModel)
        {
            HttpResponseMessage response;
            try
            {
                OrderResponseModelV2 order = _orderService.CreateOrderV2(shoppingCartModel);

                response = IsNotNull(order) ? CreateCreatedResponse(new OrderResponseV2 { Order = order }) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex.ToString());
                response = CreateInternalServerErrorResponse(new OrderResponseV2 { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new OrderResponseV2 { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex.ToString() + ex.StackTrace, ZnodeLogging.Components.OMS.ToString());
            }
            return response;
        }

        /// <summary>
        /// Update the payment status of an order
        /// </summary>
        /// <param name="OmsOrderId">OmsOrderId</param>
        /// <param name="PaymentStatusId">PaymentStatusId</param>
        /// <returns>Returns true if success</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPut]
        public HttpResponseMessage UpdatePaymentStatusV2(int OmsOrderId, int PaymentStatusId)
        {
            HttpResponseMessage response;
            try
            {
                bool isRecordUpdated = _orderService.UpdatePaymentStatusV2(OmsOrderId, PaymentStatusId);
                response = CreateOKResponse(new TrueFalseResponse { booleanModel = new BooleanModel { IsSuccess = isRecordUpdated, ErrorMessage = isRecordUpdated ? string.Empty : Admin_Resources.ErrorSamePaymentStatusUpdate }, IsSuccess = isRecordUpdated });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.ToString(), ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new OrderResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Update the order/shipping status of an order
        /// </summary>
        /// <param name="model">Model to update</param>
        /// <returns></returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPut, ValidateModel]
        public HttpResponseMessage UpdateOrderShippingStatusV2([FromBody] OrderStateParameterModel model)
        {
            HttpResponseMessage response;
            try
            {
                bool isRecordUpdated = _orderService.UpdateOrderStatus(model);
                response = CreateOKResponse(new TrueFalseResponse { booleanModel = new BooleanModel { IsSuccess = isRecordUpdated, ErrorMessage = isRecordUpdated ? string.Empty : Admin_Resources.ErrorSamePaymentStatusUpdate }, IsSuccess = isRecordUpdated });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.OMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
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
        #endregion
    }
}
