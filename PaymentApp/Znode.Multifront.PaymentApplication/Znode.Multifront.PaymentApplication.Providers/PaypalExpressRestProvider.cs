using PayPalCheckoutSdk.Orders;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using Znode.Multifront.PaymentApplication.Data;
using Znode.Multifront.PaymentApplication.Helpers;
using Znode.Multifront.PaymentApplication.Models;
using Znode.Multifront.PaymentFramework.Bussiness;

namespace Znode.Multifront.PaymentApplication.Providers
{
    public class PaypalExpressRestProvider : BaseService, IPaymentProviders
    {
        #region Private Variables
        private readonly IZnodePaymentRepository<ZNodePaymentSettingCredential> _paymentSettingCredentialRepository;
        private readonly IZnodePaymentRepository<ZNodePaymentSetting> _zNodePaymentSettingRepository;

        #endregion

        #region Constructor
        public PaypalExpressRestProvider()
        {
            _paymentSettingCredentialRepository = new ZnodePaymentRepository<ZNodePaymentSettingCredential>();
            _zNodePaymentSettingRepository = new ZnodePaymentRepository<ZNodePaymentSetting>();
        }
        #endregion

        #region Private Member Variables
        private readonly String ENDPOINT_CREATEORDER = "v2/checkout/orders";
        private readonly String ENDPOINT_AUTHORIZEORDER = "v2/checkout/orders/{id}/authorize";
        private readonly String ENDPOINT_CAPTURE = "v2/payments/authorizations/{authorization_id}/capture";
        private readonly String ENDPOINT_REFUND = "v2/payments/captures/{capture_id}/refund";
        private readonly String ENDPOINT_VOID = "v2/payments/authorizations/{authorization_id}/void";
        private readonly String ENDPOINT_TRANSACTIONDETAILS = "v2/payments/captures/{capture_id}";
        private readonly String ENDPOINT_REAUTHORIZE = "v2/payments/authorizations/{authorization_id}/reauthorize";
        #endregion

        #region Create Order

        /// <summary>
        /// Authorize and capture payment using Paypal Express
        /// </summary>
        /// <param name="paymentModel"></param>
        /// <returns></returns>
        public GatewayResponseModel ValidateCreditcard(PaymentModel paymentModel)
        {
            GatewayResponseModel response = new GatewayResponseModel();
            try
            {
                if (string.IsNullOrEmpty(paymentModel.TransactionId))
                {
                    return AuthorizeOrder(paymentModel);
                }
                else if (paymentModel.IsCapture)
                {
                    //If the 3-day honor period expires of authorization, call Reauthorize API to re-authorize the payment
                    if (GetDateDifference(paymentModel.TransactionDate) > 3)
                    {
                        response = ReAuthorize(paymentModel);
                        if (!response.IsSuccess)
                            return response;
                    }
                    return CaptureOrder(paymentModel);
                }
            }
            catch (Exception ex)
            {
                response.HasError = true;
                response.ErrorMessage = ex.Message;
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(paymentModel.PaymentApplicationSettingId, ex.Message);
            }
            return response;
        }

        /// <summary>
        /// Create Order
        /// </summary>
        /// <param name="paymentModel"></param>
        /// <returns></returns>
        public GatewayResponseModel CreateOrder(PaymentModel paymentModel)
        {
            GatewayResponseModel gatewayResponse = new GatewayResponseModel();
            OrderRequest orderRequestModel = new OrderRequest();
            try
            {
                GatewayLoginUserNamePassword(paymentModel);
                PaypalExpressRestClient client = new PaypalExpressRestClient(GetAPIEndpoint(paymentModel), paymentModel.GatewayLoginName, paymentModel.GatewayLoginPassword, paymentModel.GatewayTestMode);
                orderRequestModel = BuildRequestBody(paymentModel);
                var createResponse = client.PostResourceFromEndpoint<Order, OrderRequest>(ENDPOINT_CREATEORDER, orderRequestModel);

                if (string.Equals(createResponse.Status, "CREATED", StringComparison.InvariantCultureIgnoreCase))
                {
                    paymentModel.PaymentToken = createResponse.Id;
                    paymentModel.TransactionId = createResponse.Id;
                    gatewayResponse.IsGatewayPreAuthorize = paymentModel.GatewayPreAuthorize;
                    gatewayResponse.IsSuccess = true;
                    gatewayResponse.ResponseText = createResponse.Links.Where(x => x.Rel == "approve").Select(y => y.Href).FirstOrDefault();
                    gatewayResponse.PaymentStatus = ZnodePaymentStatus.AUTHORIZED;
                    gatewayResponse.PaymentToken = createResponse.Id;
                    gatewayResponse.TransactionId = createResponse.Id;
                    ZnodeLogging.LogMessage($"Paypal Express checkout: Create Order successfully for order_Id: {paymentModel.OrderId}.", Logging.Components.Payment.ToString(), TraceLevel.Info);
                }
                else
                {
                    gatewayResponse.ResponseText = GetErrorMessage(createResponse);
                    gatewayResponse.HasError = true;
                    gatewayResponse.IsSuccess = false;
                    ZnodeLogging.LogMessage($"Paypal Express checkout: Create Order failed for order_Id: {paymentModel.OrderId}. Error: {gatewayResponse.ResponseText} ", Logging.Components.Payment.ToString(), TraceLevel.Info, createResponse);
                    ZnodeLogging.LogMessage($"Paypal Express checkout: create order failed for order_Id: {paymentModel.OrderId}. Error:  {gatewayResponse.ResponseText} ", Logging.Components.Payment.ToString(), TraceLevel.Error);
                }
            }
            catch (Exception ex)
            {
                gatewayResponse.HasError = true;
                gatewayResponse.ErrorMessage = "This payment option is currently unavailable. Please try again in a few minutes.";
                ZnodeLogging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(paymentModel.PaymentApplicationSettingId, ex.Message);
            }

            return gatewayResponse;
        }

        /// <summary>
        /// Authorize Order
        /// </summary>
        /// <param name="paymentModel"></param>
        /// <returns></returns>
        public GatewayResponseModel AuthorizeOrder(PaymentModel paymentModel)
        {
            AuthorizeRequest authorizeRequestModel = new AuthorizeRequest();
            GatewayResponseModel gatewayResponse = new GatewayResponseModel();

            GatewayLoginUserNamePassword(paymentModel);
            PaypalExpressRestClient client = new PaypalExpressRestClient(GetAPIEndpoint(paymentModel), paymentModel.GatewayLoginName, paymentModel.GatewayLoginPassword, paymentModel.GatewayTestMode);

            var requestUri = ENDPOINT_AUTHORIZEORDER.Replace("{id}", Uri.EscapeDataString(paymentModel.PaymentToken));
            var authorizeResponse = client.PostResourceFromEndpoint<Order, AuthorizeRequest>(requestUri, authorizeRequestModel);

            var authorizeId = GetCaptureAuthorizeId(authorizeResponse);
            if (authorizeResponse?.PurchaseUnits != null && string.Equals(authorizeResponse?.PurchaseUnits?[0].Payments?.Authorizations[0]?.Status, "CREATED", StringComparison.InvariantCultureIgnoreCase))
            {
                paymentModel.TransactionId = authorizeId;
                gatewayResponse.TransactionId = authorizeId;
                gatewayResponse.IsSuccess = true;
                gatewayResponse.PaymentStatus = ZnodePaymentStatus.AUTHORIZED;
                gatewayResponse.ResponseCode = "0";
                gatewayResponse.IsGatewayPreAuthorize = paymentModel.GatewayPreAuthorize;
                ZnodeLogging.LogMessage($"Paypal Express checkout: transaction authorization successfully for order_Id: {paymentModel.OrderId}.", Logging.Components.Payment.ToString(), TraceLevel.Info);
            }
            else
            {
                gatewayResponse.ResponseText = GetErrorMessage(authorizeResponse);
                gatewayResponse.HasError = true;
                gatewayResponse.IsSuccess = false;
                ZnodeLogging.LogMessage($"Paypal Express checkout: transaction authorization failed for order_Id: {paymentModel.OrderId}. Error: {gatewayResponse.ResponseText} ", Logging.Components.Payment.ToString(), TraceLevel.Info, authorizeResponse);
                ZnodeLogging.LogMessage($"Paypal Express checkout: transaction authorization failed for order_Id: {paymentModel.OrderId}. Error: {gatewayResponse.ResponseText} ", Logging.Components.Payment.ToString(), TraceLevel.Error);
            }

            return gatewayResponse;
        }

        /// <summary>
        ///  If the 3-day
        /// <param name="paymentModel"></param> honor period expires of authorization, call Reauthorize API to re-authorize the payment 
        /// </summary>
        /// <returns></returns>
        public GatewayResponseModel ReAuthorize(PaymentModel paymentModel)
        {
            GatewayResponseModel gatewayResponse = new GatewayResponseModel();

            GatewayLoginUserNamePassword(paymentModel);
            PaypalExpressRestClient client = new PaypalExpressRestClient(GetAPIEndpoint(paymentModel), paymentModel.GatewayLoginName, paymentModel.GatewayLoginPassword, paymentModel.GatewayTestMode);

            Money captureAmount = new Money()
            {
                CurrencyCode = paymentModel.GatewayCurrencyCode,
                Value = String.Format("{0:0.00}", Convert.ToDecimal(paymentModel.Total))
            };
            Capture captureRequest = new Capture
            {
                Amount = captureAmount,
                InvoiceId = paymentModel.OrderId
            };

            var requestUri = ENDPOINT_REAUTHORIZE.Replace("{authorization_id}", Uri.EscapeDataString(paymentModel.TransactionId));
            var authorizeResponse = client.PostResourceFromEndpoint<Order, Capture>(requestUri, captureRequest);

            if (string.Equals(authorizeResponse?.PurchaseUnits[0].Payments?.Authorizations[0]?.Status, "CREATED", StringComparison.InvariantCultureIgnoreCase))
            {
                gatewayResponse.IsSuccess = true;
                ZnodeLogging.LogMessage($"Paypal Express checkout: transaction reauthorize successfully for order_Id: {paymentModel.OrderId}.", Logging.Components.Payment.ToString(), TraceLevel.Info);
            }
            else
            {
                gatewayResponse.ResponseText = GetErrorMessage(authorizeResponse);
                gatewayResponse.HasError = true;
                gatewayResponse.IsSuccess = false;
                ZnodeLogging.LogMessage($"Paypal Express checkout: transaction reauthorize failed for order_Id: {paymentModel.OrderId}. Error: {gatewayResponse.ResponseText} ", Logging.Components.Payment.ToString(), TraceLevel.Error, authorizeResponse);
                ZnodeLogging.LogMessage($"Paypal Express checkout: transaction reauthorize failed for order_Id: {paymentModel.OrderId}. Error: {gatewayResponse.ResponseText} ", Logging.Components.Payment.ToString(), TraceLevel.Error);
            }

            return gatewayResponse;
        }

        /// <summary>
        /// Capture Order based on authorization Id
        /// </summary>
        /// <param name="paymentModel"></param>
        /// <returns></returns>
        public GatewayResponseModel CaptureOrder(PaymentModel paymentModel)
        {
            Order captureResponseModel = new Order();
            GatewayResponseModel gatewayResponse = new GatewayResponseModel();
            GatewayLoginUserNamePassword(paymentModel);
            PaypalExpressRestClient client = new PaypalExpressRestClient(GetAPIEndpoint(paymentModel), paymentModel.GatewayLoginName, paymentModel.GatewayLoginPassword, paymentModel.GatewayTestMode);

            Money captureAmount = new Money()
            {
                CurrencyCode = paymentModel.GatewayCurrencyCode,
                Value = String.Format("{0:0.00}", Convert.ToDecimal(paymentModel.Total))
            };
            Capture captureRequest = new Capture
            {
                Amount = captureAmount,
                FinalCapture = true,
                InvoiceId = paymentModel.OrderId
            };

            var requestUri = ENDPOINT_CAPTURE.Replace("{authorization_id}", Uri.EscapeDataString(paymentModel.TransactionId));
            captureResponseModel = client.PostResourceFromEndpoint<Order, Capture>(requestUri, captureRequest);

            if (string.Equals(captureResponseModel.Status, "COMPLETED", StringComparison.InvariantCultureIgnoreCase))
            {
                gatewayResponse.TransactionId = captureResponseModel.Id;
                gatewayResponse.IsSuccess = true;
                gatewayResponse.PaymentStatus = ZnodePaymentStatus.CAPTURED;
                gatewayResponse.ResponseCode = "0";
                gatewayResponse.IsGatewayPreAuthorize = paymentModel.GatewayPreAuthorize;
                ZnodeLogging.LogMessage($"Paypal Express checkout: transaction capture successfully for order_Id: {paymentModel.OrderId}.", Logging.Components.Payment.ToString(), TraceLevel.Info);
            }
            else
            {
                gatewayResponse.ResponseText = GetErrorMessage(captureResponseModel);
                gatewayResponse.HasError = true;
                gatewayResponse.IsSuccess = false;
                ZnodeLogging.LogMessage($"Paypal Express checkout: transaction capture failed for order_Id: {paymentModel.OrderId}. Error: {gatewayResponse.ResponseText} ", Logging.Components.Payment.ToString(), TraceLevel.Info, captureResponseModel);
                ZnodeLogging.LogMessage($"Paypal Express checkout: transaction capture failed for order_Id: {paymentModel.OrderId}. Error: {gatewayResponse.ResponseText} ", Logging.Components.Payment.ToString(), TraceLevel.Error);
            }

            return gatewayResponse;
        }

        #endregion

        /// <summary>
        /// This method is used to void the payments done earlier
        /// </summary>
        /// <param name="paymentModel">payment model</param>
        /// <returns>returns the response of the void method from paypal</returns>
        public GatewayResponseModel Void(PaymentModel paymentModel)
        {
            GatewayResponseModel gatewayResponse = new GatewayResponseModel();
            GatewayLoginUserNamePassword(paymentModel);
            PaypalExpressRestClient client = new PaypalExpressRestClient(GetAPIEndpoint(paymentModel), paymentModel.GatewayLoginName, paymentModel.GatewayLoginPassword, paymentModel.GatewayTestMode);

            var requestUri = ENDPOINT_VOID.Replace("{authorization_id}", Uri.EscapeDataString(paymentModel.TransactionId));
            Order voidResponse = client.PostResourceFromEndpoint<Order>(requestUri);

            if (!Equals(voidResponse.Status, null) && voidResponse.Status.Equals("voided", StringComparison.InvariantCultureIgnoreCase))
            {
                gatewayResponse.TransactionId = voidResponse.Id;
                gatewayResponse.IsSuccess = true;
                gatewayResponse.PaymentStatus = ZnodePaymentStatus.VOIDED;
                gatewayResponse.ResponseCode = "0";
                ZnodeLogging.LogMessage($"Paypal Express checkout: transaction void successfully for order_Id: {paymentModel.OrderId}.", Logging.Components.Payment.ToString(), TraceLevel.Info);
            }
            else
            {
                gatewayResponse.ResponseText = GetErrorMessage(voidResponse);
                gatewayResponse.HasError = true;
                gatewayResponse.IsSuccess = false;
                ZnodeLogging.LogMessage($"Paypal Express checkout: transaction void failed for order_Id: {paymentModel.OrderId}. Error: {gatewayResponse.ResponseText} ", Logging.Components.Payment.ToString(), TraceLevel.Info, voidResponse);
                ZnodeLogging.LogMessage($"Paypal Express checkout: transaction void failed for order_Id: {paymentModel.OrderId}. Error: {gatewayResponse.ResponseText} ", Logging.Components.Payment.ToString(), TraceLevel.Error);
            }

            return gatewayResponse;
        }

        /// <summary>
        /// This method used to refund the payment that has been returned or cancel order.
        /// </summary>
        /// <param name="paymentModel">Payment model</param>
        /// <returns>Transaction details after refund</returns>
        public GatewayResponseModel Refund(PaymentModel paymentModel)
        {
            GatewayResponseModel gatewayResponse = new GatewayResponseModel();
            Order response = new Order();
            try
            {
                GatewayLoginUserNamePassword(paymentModel);
                //Need to be fetched from the database
                PaypalExpressRestClient client = new PaypalExpressRestClient(GetAPIEndpoint(paymentModel), paymentModel.GatewayLoginName, paymentModel.GatewayLoginPassword, paymentModel.GatewayTestMode);
                var requestUri = ENDPOINT_REFUND.Replace("{capture_id}", Uri.EscapeDataString(paymentModel.CaptureTransactionId));

                Refund refundRequest = new Refund
                {
                    InvoiceId = paymentModel.OrderId,
                    Amount = new Money
                    {
                        Value = Convert.ToString(Math.Round(decimal.Parse(paymentModel.Total), 2)),
                        CurrencyCode = paymentModel.GatewayCurrencyCode,
                    }
                };

                response = client.PostResourceFromEndpoint<Order, Refund>(requestUri, refundRequest);

                if (!Equals(response.Status, null) && response.Status.Equals("completed", StringComparison.InvariantCultureIgnoreCase))
                {
                    gatewayResponse.TransactionId = response.Id;
                    gatewayResponse.IsSuccess = true;
                    gatewayResponse.PaymentStatus = ZnodePaymentStatus.REFUNDED;
                    gatewayResponse.ResponseCode = "0";
                    ZnodeLogging.LogMessage($"Paypal Express checkout: transaction refund successfully for order_Id: {paymentModel.OrderId}.", Logging.Components.Payment.ToString(), TraceLevel.Info);
                }
                else
                {
                    gatewayResponse.ResponseText = GetErrorMessage(response);
                    gatewayResponse.HasError = true;
                    gatewayResponse.IsSuccess = false;
                    ZnodeLogging.LogMessage($"Paypal Express checkout: transaction refund failed for order_Id: {paymentModel.OrderId}. Error: {gatewayResponse.ResponseText} ", Logging.Components.Payment.ToString(), TraceLevel.Info, response);
                    ZnodeLogging.LogMessage($"Paypal Express checkout: transaction refund failed for order_Id: {paymentModel.OrderId}. Error: {gatewayResponse.ResponseText} ", Logging.Components.Payment.ToString(), TraceLevel.Error);
                }

                return gatewayResponse;
            }
            catch (Exception ex)
            {
                gatewayResponse.HasError = true;
                ZnodeLogging.LogMessage($"Paypal Express checkout: transaction refund failed for order_Id: {paymentModel.OrderId}. Error: {gatewayResponse.ResponseText} ", Logging.Components.Payment.ToString(), TraceLevel.Info, ex.Message);
                ZnodeLogging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
            }
            return new GatewayResponseModel { IsSuccess = false, GatewayResponseData = "Unable to process request" };
        }

        /// <summary>
        /// Get the transaction details based on capture id. 
        /// </summary>
        /// <param name="paymentModel">Payment model</param>
        /// <returns>Transaction details for the payment.</returns>
        public TransactionDetailsModel GetTransactionDetails(PaymentModel paymentModel)
        {
            TransactionDetailsModel gatewayResponse = new TransactionDetailsModel();
            Order response = new Order();
            try
            {
                GatewayLoginUserNamePassword(paymentModel);
                //Need to be fetched from the database
                PaypalExpressRestClient client = new PaypalExpressRestClient(GetAPIEndpoint(paymentModel), paymentModel.GatewayLoginName, paymentModel.GatewayLoginPassword, paymentModel.GatewayTestMode);
                var requestUri = ENDPOINT_TRANSACTIONDETAILS.Replace("{capture_id}", Uri.EscapeDataString(paymentModel.CaptureTransactionId));

                response = client.GetResourceFromEndpoint<Order>(requestUri);
                
                if (!Equals(response.Status, null) && (response.Status.Equals("completed", StringComparison.InvariantCultureIgnoreCase) || (response.Status.Equals("PARTIALLY_REFUNDED", StringComparison.InvariantCultureIgnoreCase))))
                {
                    gatewayResponse.TransactionStatus = string.Equals(response.Status, "Completed", StringComparison.InvariantCultureIgnoreCase) ? "SettledSuccessfully" : response.Status;
                    gatewayResponse.TransactionId = response.Id;
                    gatewayResponse.ResponseCode = "0";
                    gatewayResponse.IsSuccess = true;
                    ZnodeLogging.LogMessage($"Paypal Express checkout: get transaction detail successfully for order_Id: {paymentModel.OrderId}.", Logging.Components.Payment.ToString(), TraceLevel.Info);
                }
                else
                {
                    gatewayResponse.ResponseText = GetErrorMessage(response);
                    gatewayResponse.HasError = true;
                    gatewayResponse.ErrorMessage = "Unable to get transaction details.";
                    ZnodeLogging.LogMessage($"Paypal Express checkout: get transaction detail failed for order_Id: {paymentModel.OrderId}. Error: {gatewayResponse.ResponseText} ", Logging.Components.Payment.ToString(), TraceLevel.Info,response);
                    ZnodeLogging.LogMessage($"Paypal Express checkout: get transaction detail failed for order_Id: {paymentModel.OrderId}. Error: {gatewayResponse.ResponseText} ", Logging.Components.Payment.ToString(), TraceLevel.Error);
                }
                return gatewayResponse;
            }
            catch (Exception ex)
            {
                gatewayResponse.IsSuccess = false;
                gatewayResponse.HasError = true;
                gatewayResponse.ErrorMessage = ex.Message;
                ZnodeLogging.LogMessage($"Paypal Express checkout: get transaction detail failed for order_Id: {paymentModel.OrderId}. Error: {gatewayResponse.ResponseText} ", Logging.Components.Payment.ToString(), TraceLevel.Info, ex.Message);
                ZnodeLogging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
            }
            return gatewayResponse;
        }

        #region Private Methods
        private string GetErrorMessage(Order response)
        {
            string message = string.Empty;
            if (response?.Links?.Count == 1)
            {
                string[] errorMessage = response?.Links?.Where(x => x.Rel == "information_link").Select(y => y.Href).FirstOrDefault().ToString().Split('-');
                message = errorMessage?[1];
            }
            return message;
        }
        /// <summary>
        /// Get Date difference 
        /// </summary>
        /// <param name="TransactionDate"></param>
        /// <returns>Int value Number of Days</returns>
        private int GetDateDifference(DateTime? transactionDate)
        {
            return (DateTime.Now.Date - (transactionDate ?? DateTime.Now.Date)).Days;
        }

        /// <summary>
        /// Get Capture/Authorize Id
        /// </summary>
        /// <param name="paymentModel"></param>
        /// <returns></returns>

        private string GetCaptureAuthorizeId(Order orderResponse)
        {
            var id = string.Empty;
            if (orderResponse != null)
            {
                if (orderResponse.PurchaseUnits?.Count > 0)
                {
                    if (string.Equals(orderResponse.CheckoutPaymentIntent, "CAPTURE", StringComparison.InvariantCultureIgnoreCase))
                    {
                        id = orderResponse.PurchaseUnits?.Select(p => p.Payments)?.Select(x => x.Captures).FirstOrDefault().Select(x => x.Id)?.FirstOrDefault().ToString();
                    }
                    else if (string.Equals(orderResponse.CheckoutPaymentIntent, "AUTHORIZE", StringComparison.InvariantCultureIgnoreCase))
                    {
                        id = orderResponse.PurchaseUnits?.Select(p => p.Payments)?.Select(x => x.Authorizations).FirstOrDefault().Select(x => x.Id)?.FirstOrDefault().ToString();
                    }
                }
            }
            return id;
        }

        /// <summary>
        /// Prepared create Order Request Body
        /// </summary>
        /// <param name="paymentModel"></param>
        /// <returns></returns>
        private OrderRequest BuildRequestBody(PaymentModel paymentModel)
        {
            List<PurchaseUnitRequest> purchaseUnits = new List<PurchaseUnitRequest>();
            ApplicationContext applicationContext = new ApplicationContext
            {
                ReturnUrl = paymentModel.ReturnUrl,
                CancelUrl = paymentModel.CancelUrl,
                BrandName = paymentModel.PortalName,
            };
            OrderRequest orderRequest = new OrderRequest
            {
                CheckoutPaymentIntent = "AUTHORIZE",
                ApplicationContext = applicationContext,
            };

            //PurchaseUnits
            PurchaseUnitRequest purchaseUnit = new PurchaseUnitRequest
            {
                InvoiceId = paymentModel.OrderId,
                ReferenceId = Guid.NewGuid().ToString(),
                CustomId = paymentModel.CustomerId,
            };

            AmountWithBreakdown amountWithBreakdown = new AmountWithBreakdown();
            amountWithBreakdown.CurrencyCode = paymentModel.GatewayCurrencyCode;
            amountWithBreakdown.Value = String.Format("{0:0.00}", Convert.ToDecimal(paymentModel.Total));

            purchaseUnit.AmountWithBreakdown = amountWithBreakdown;
            purchaseUnits.Add(purchaseUnit);

            //Shipping Detail
            purchaseUnit.ShippingDetail = new ShippingDetail
            {
                Name = new Name
                {
                    FullName = paymentModel.ShippingFirstName + paymentModel.ShippingLastName
                },
                AddressPortable = new AddressPortable
                {
                    AddressLine1 = paymentModel.ShippingStreetAddress1,
                    AddressLine2 = paymentModel.ShippingStreetAddress2,
                    AdminArea2 = paymentModel.ShippingCity,
                    AdminArea1 = paymentModel.ShippingStateCode,
                    PostalCode = paymentModel.ShippingPostalCode,
                    CountryCode = paymentModel.ShippingCountryCode?.ToUpper()
                }
            };

            orderRequest.PurchaseUnits = purchaseUnits;

            return orderRequest;
        }

        /// <summary>
        /// Get API EndPoint
        /// </summary>
        /// <param name="paymentModel"></param>
        /// <returns></returns>
        private string GetAPIEndpoint(PaymentModel model)
        {
            return model.GatewayTestMode ? Convert.ToString(ConfigurationManager.AppSettings["PaypalSandboxURL"]) : Convert.ToString(ConfigurationManager.AppSettings["PaypalLiveURL"]);
        }

        /// <summary>
        /// Get Gateway Login User Name Password from database.
        /// </summary>
        /// <param name="paymentModel">Payment model</param>
        private PaymentModel GatewayLoginUserNamePassword(PaymentModel paymentModel)
        {
            var gatewayLoginUserNamePassword = (from zNodePaymentSetting in _zNodePaymentSettingRepository.Table
                                                join zNodePaymentSettingCredential in _paymentSettingCredentialRepository.Table on zNodePaymentSetting.PaymentSettingId equals zNodePaymentSettingCredential.PaymentSettingId
                                                where zNodePaymentSetting.PaymentSettingId == paymentModel.PaymentApplicationSettingId && zNodePaymentSettingCredential.TestMode == paymentModel.GatewayTestMode
                                                select new { GatewayLoginPassword = zNodePaymentSettingCredential.GatewayPassword, GatewayLoginUserName = zNodePaymentSettingCredential.GatewayUsername })?.FirstOrDefault();

            paymentModel.GatewayLoginName = Decrypt(gatewayLoginUserNamePassword.GatewayLoginUserName);
            paymentModel.GatewayLoginPassword = Decrypt(gatewayLoginUserNamePassword.GatewayLoginPassword);
            return paymentModel;

        }

        #endregion

        public PaymentGatewayTokenModel TokenGenerator(PaymentGatewayTokenModel paymentGatewayTokenModel)
        {
            throw new NotImplementedException();
        }

        public GatewayResponseModel Subscription(PaymentModel paymentModel)
        {
            throw new NotImplementedException();
        }




    }
}
