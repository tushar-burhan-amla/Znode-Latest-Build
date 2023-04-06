using System;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Znode.Multifront.PaymentApplication.Data;
using Znode.Multifront.PaymentApplication.Helpers;
using Znode.Multifront.PaymentApplication.Models;
using Znode.Multifront.PaymentApplication.Models.Models;

namespace Znode.Multifront.PaymentApplication.Providers
{
    public class GatewayConnector : BaseConnector
    {
        #region Public Methods
        /// <summary>
        /// Execute and get Credit card processing and returns the GUID.
        /// </summary>
        /// <param name="paymentModel"></param>
        /// <returns></returns>
        public GatewayResponseModel GetResponse(PaymentModel paymentModel)
        {
            try
            {
                TransactionService repository = new TransactionService();

                if (!string.IsNullOrEmpty(paymentModel.TransactionId))
                {
                    ZnodeTransaction transactionDetails = repository.GetPayment(paymentModel.TransactionId);
                    if (!Equals(transactionDetails, null))
                    {
                        paymentModel.CustomerProfileId = transactionDetails.CustomerProfileId;
                        paymentModel.CustomerPaymentProfileId = transactionDetails.CustomerPaymentId;
                        paymentModel.TransactionId = transactionDetails.TransactionId;
                        paymentModel.GatewayCurrencyCode = transactionDetails.CurrencyCode;
                        paymentModel.CardDataToken = transactionDetails.Custom1;
                        paymentModel.OrderId = transactionDetails.Custom1;
                        paymentModel.GUID = Convert.ToString(transactionDetails.GUID);
                    }
                }

                paymentModel.Total = Math.Round(Convert.ToDecimal(paymentModel?.Total), 2, MidpointRounding.AwayFromZero).ToString();

                GatewayResponseModel response = ProcessCreditCard(paymentModel);

                if (Equals(response, null))
                    return new GatewayResponseModel { HasError = true, ErrorMessage = "Authorization failed" };

                if (response.IsSuccess)
                {
                    if (string.IsNullOrEmpty(paymentModel.TransactionId))
                    {
                        paymentModel.CustomerProfileId = response.CustomerProfileId;
                        paymentModel.CustomerPaymentProfileId = response.CustomerPaymentProfileId;
                    }

                    paymentModel.TransactionId = response.TransactionId;
                    paymentModel.ResponseText = response.GatewayResponseData;
                    paymentModel.ResponseCode = response.ResponseCode;
                    paymentModel.CardDataToken = response.Token;
                    paymentModel.PaymentStatusId = (int)response.PaymentStatus;
                    paymentModel.TransactionDate = DateTime.Now;
                    paymentModel.GUID = string.IsNullOrEmpty(paymentModel.GUID) ? repository.AddPayment(paymentModel) : repository.UpdatePayment(paymentModel);

                    if (paymentModel.Subscriptions.Any() && paymentModel.GatewayType != Convert.ToInt16(Enum.Parse(typeof(GatewayType), GatewayType.TWOCHECKOUT.ToString())).ToString())
                    {
                        GetSubscriptionResponse(paymentModel);
                    }
                    if (string.Equals(paymentModel.GatewayCode, PaymentConstant.CyberSource, StringComparison.OrdinalIgnoreCase))
                    {
                        return new GatewayResponseModel { Token = paymentModel.TransactionId, TransactionDate = paymentModel.TransactionDate, IsGatewayPreAuthorize = response.IsGatewayPreAuthorize, CustomerGUID = response.CustomerGUID,PaymentGUID = paymentModel.PaymentGUID, CardNumber = paymentModel.CardNumber };
                    }
                    if (string.Equals(paymentModel.GatewayCode, PaymentConstant.AuthorizeNet, StringComparison.OrdinalIgnoreCase))
                    {
                        return new GatewayResponseModel { Token = paymentModel.TransactionId, TransactionDate = paymentModel.TransactionDate, IsGatewayPreAuthorize = response.IsGatewayPreAuthorize, CardNumber = paymentModel.CardNumber };
                    }
                    return new GatewayResponseModel { Token = paymentModel.TransactionId, TransactionDate = paymentModel.TransactionDate, IsGatewayPreAuthorize = response.IsGatewayPreAuthorize };
                }
                else
                {
                    string message = string.Empty;
                    if (!Equals(response.GatewayResponseData, null))
                    {
                        message = response.GatewayResponseData.Replace("<br>", string.Empty);
                        Logging.LogMessage(message, Logging.Components.Payment.ToString(), TraceLevel.Error);
                    }
                    else if (!string.IsNullOrEmpty(response.ResponseCode) || !string.IsNullOrEmpty(response.ResponseText))
                    {
                        message = $"{response.ResponseCode} {response.ResponseText}";
                        Logging.LogMessage(message, Logging.Components.Payment.ToString(), TraceLevel.Error);
                    }
                    else
                    {
                        message = "Unable to contact payment provider.";
                        Logging.LogMessage(message, Logging.Components.Payment.ToString(), TraceLevel.Error);
                    }

                    LoggingService.LogActivity(paymentModel.PaymentApplicationSettingId, "Transaction failed", message);
                    if (!string.IsNullOrEmpty(response.CustomerProfileId) && !string.IsNullOrEmpty(response.CustomerPaymentProfileId))
                        DeleteSavedCCDetails(response.CustomerProfileId, response.CustomerPaymentProfileId);

                    return new GatewayResponseModel { HasError = true, ErrorMessage = message };
                }
            }
            catch (Exception ex)
            {
                LoggingService.LogActivity(paymentModel.PaymentApplicationSettingId, ex.Message);
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                return new GatewayResponseModel { HasError = true, ErrorMessage = ex.Message };
            }
        }

        /// <summary>
        /// To create customer profile using Payment API
        /// </summary>
        /// <param name="paymentModel"></param>
        /// <returns></returns>
        public GatewayResponseModel GetCustomerResponse(PaymentModel paymentModel)
        {
            try
            {
                return ProcessCreditCard(paymentModel);
            }
            catch (Exception ex)
            {
                LoggingService.LogActivity(paymentModel.PaymentApplicationSettingId, ex.Message);
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);

                GatewayResponseModel errorModel = new GatewayResponseModel();
                errorModel.IsSuccess = false;
                errorModel.ResponseText = ex.Message;

                return errorModel;
            }
        }

        /// <summary>
        /// To create gateway token
        /// </summary>
        /// <param name="paymentModel"></param>
        /// <returns></returns>
        public PaymentGatewayTokenModel GetGatewayToken(PaymentGatewayTokenModel gatewayTokenModel)
        {
            try
            {
                return ProcessTokenGenerator(gatewayTokenModel);
            }
            catch (Exception ex)
            {
                LoggingService.LogActivity(gatewayTokenModel.PaymentSettingId, ex.Message);
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);

                PaymentGatewayTokenModel errorModel = new PaymentGatewayTokenModel();
                //errorModel.IsSuccess = false;
                //errorModel.ResponseText = ex.Message;
                return errorModel;
            }
        }

        /// <summary>
        /// Execute and Get Capture status from gateway provider
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public BooleanModel GetCaptureResponse(string token)
        {
            TransactionService repository = new TransactionService();
            PaymentModel paymentModel = new PaymentModel { TransactionId = token, IsCapture = true };
            var domainName = HttpContext.Current.Request.Headers["Znode-DomainName"];
            var vendorLoginUrl = Convert.ToString(ConfigurationManager.AppSettings["AdminWebsiteUrl"]);
            vendorLoginUrl = vendorLoginUrl?.Replace("https://", "");

            if (!string.IsNullOrEmpty(token))
            {
                ZnodeTransaction transactionDetails = repository.GetPayment(token);
                if (!Equals(transactionDetails, null))
                {
                    paymentModel.CustomerProfileId = transactionDetails.CustomerProfileId;
                    paymentModel.CustomerPaymentProfileId = transactionDetails.CustomerPaymentId;
                    paymentModel.PaymentApplicationSettingId = transactionDetails.PaymentSettingId.Value;
                    paymentModel.TransactionId = transactionDetails.TransactionId;
                    paymentModel.Total = (Math.Round(Convert.ToDecimal(transactionDetails.Amount), 2, MidpointRounding.AwayFromZero)).ToString();
                    paymentModel.GatewayCurrencyCode = transactionDetails.CurrencyCode;
                    paymentModel.CardDataToken = transactionDetails.Custom1;
                    paymentModel.OrderId = transactionDetails.Custom1;
                    paymentModel.CaptureTransactionId = transactionDetails.CaptureTransactionId;
                    paymentModel.GUID = Convert.ToString(transactionDetails.GUID);
                    paymentModel.TransactionDate = transactionDetails?.ModifiedDate;
                    if (string.Equals(Convert.ToString(domainName), Convert.ToString(vendorLoginUrl), StringComparison.OrdinalIgnoreCase))                        
                    {
                        paymentModel.IsOrderFromAdmin = true;
                    }
                    PaymentSettingsService paymentSettingsService = new PaymentSettingsService();
                    paymentModel.PaymentCode = paymentSettingsService.GetPaymentSetting(transactionDetails.PaymentSettingId.GetValueOrDefault()).PaymentCode;
                    GatewayResponseModel response = ProcessCreditCard(paymentModel);

                    if (response.IsSuccess)
                    {
                        paymentModel.CaptureTransactionId = response.TransactionId;
                        paymentModel.PaymentStatusId = (int)response.PaymentStatus;
                        paymentModel.ResponseText = response.GatewayResponseData;
                        paymentModel.ResponseCode = response.ResponseCode;
                        paymentModel.CardDataToken = response.Token;
                        repository.UpdatePayment(paymentModel);
                        LoggingService.LogActivity(paymentModel.PaymentApplicationSettingId, "Transaction Captured");
                        return new BooleanModel { IsSuccess = true };
                    }
                    else
                    {
                        string message = response.GatewayResponseData?.Replace("<br>", "");
                        LoggingService.LogActivity(paymentModel.PaymentApplicationSettingId, "Transaction Capture failed", message);
                        Logging.LogMessage(message, Logging.Components.Payment.ToString(), TraceLevel.Error);
                        return new BooleanModel { HasError = true, ErrorMessage = message };
                    }
                }
            }
            return new BooleanModel { HasError = true, ErrorMessage = "Invalid Payment Transaction Token" };
        }

        /// <summary>
        /// Get Refund/Void status from gateway provider
        /// </summary>
        /// <param name="token"></param>
        /// <param name="amount"></param>
        /// <param name="isVoid"></param>
        /// <returns></returns>
        public BooleanModel GetRefundVoidResponse(string token, decimal amount, bool isCompleteOrderRefund, bool isVoid,AddressModel BillingAddress, AddressModel ShippingAddress)
        {
            TransactionService repository = new TransactionService();

            if (!string.IsNullOrEmpty(token))
            {
                ZnodeTransaction transactionDetails = repository.GetPayment(token);
                if (!Equals(transactionDetails, null))
                {
                    PaymentModel paymentModel = new PaymentModel();
                    paymentModel.CustomerProfileId = transactionDetails.CustomerProfileId;
                    paymentModel.CustomerPaymentProfileId = transactionDetails.CustomerPaymentId;
                    paymentModel.PaymentApplicationSettingId = transactionDetails.PaymentSettingId.Value;
                    paymentModel.TransactionId = transactionDetails.TransactionId;
                    paymentModel.GatewayCurrencyCode = transactionDetails.CurrencyCode;
                    paymentModel.CardDataToken = transactionDetails.Custom1;
                    paymentModel.OrderId = transactionDetails.Custom1;
                    paymentModel.GUID = Convert.ToString(transactionDetails.GUID);
                    paymentModel.CaptureTransactionId = transactionDetails.CaptureTransactionId;

                    if (ShippingAddress != null)
                    {
                        paymentModel.ShippingCity = ShippingAddress.CityName;
                        paymentModel.ShippingFirstName = ShippingAddress.FirstName;
                        paymentModel.ShippingLastName = ShippingAddress.LastName;
                        paymentModel.ShippingCountryCode = ShippingAddress.CountryName;
                        paymentModel.ShippingPostalCode = ShippingAddress.PostalCode;
                        paymentModel.ShippingPhoneNumber = ShippingAddress.PhoneNumber;
                        paymentModel.ShippingCompanyName = ShippingAddress.CompanyName;
                        paymentModel.ShippingStreetAddress1 = ShippingAddress.Address1;
                        paymentModel.GatewayCode = ShippingAddress.GatewayCode;
                        paymentModel.ShippingStateCode = ShippingAddress.StateName;
                        paymentModel.ShippingStreetAddress1 = ShippingAddress.Address1;

                    }
                    if (BillingAddress != null)
                    {
                        paymentModel.BillingCompanyName = BillingAddress.CompanyName;
                        paymentModel.BillingPhoneNumber = BillingAddress.PhoneNumber;
                        paymentModel.BillingEmailId = BillingAddress.EmailAddress;
                    }
                    
                    if (!isVoid)
                    {                        
                        paymentModel.Total = isCompleteOrderRefund ? Convert.ToString(transactionDetails.Amount - (transactionDetails.RefundAmount ?? 0m)) : amount.ToString();
                        //check if the refund amount should not be greater than order total and previous refunds.
                        if (CheckRefundAmountWithOrderTotal(amount, transactionDetails))
                        {
                            string message = "Refund amount exceed the order total";
                            Logging.LogMessage(message, Logging.Components.Payment.ToString(), TraceLevel.Info);
                            return new BooleanModel { HasError = true, ErrorMessage = message };
                        }
                    }
                    GetPaypalExpressCode(paymentModel);

                    GatewayResponseModel response = this.ProcessRefundVoid(paymentModel, isVoid);

                    if (response.IsSuccess)
                    {
                        paymentModel.RefundTransactionId = response.TransactionId;

                        paymentModel.ResponseText = response.GatewayResponseData;
                        paymentModel.ResponseCode = response.ResponseCode;
                        paymentModel.PaymentStatusId = (int)response.PaymentStatus;
                        if (!isVoid)
                            paymentModel.RefundAmount = amount;
                        repository.UpdatePayment(paymentModel);
                        LoggingService.LogActivity(paymentModel.PaymentApplicationSettingId, isVoid ? "Void transaction" : "Refund transaction");
                        return new BooleanModel { IsSuccess = true };
                    }
                    else
                    {
                        string message = response.GatewayResponseData?.Replace("<br>", "");
                        LoggingService.LogActivity(paymentModel.PaymentApplicationSettingId, isVoid ? "Transaction Void failed" : "Transaction Refund failed", message);
                        Logging.LogMessage(message, Logging.Components.Payment.ToString(), TraceLevel.Error);
                        return new BooleanModel { HasError = true, ErrorMessage = message };
                    }
                }
            }
            return new BooleanModel { HasError = true, ErrorMessage = "Invalid Payment Transaction Token" };
        }

        public void GetPaypalExpressCode(PaymentModel paymentModel)
        {
            PaymentSettingsService paymentSettingsService = new PaymentSettingsService();
            string paymentCode = paymentSettingsService.GetPaymentCodeAndTypes(paymentModel.PaymentApplicationSettingId);
            paymentModel.PaymentCode = string.IsNullOrEmpty(paymentCode) ? paymentModel.PaymentCode : paymentCode;

        }

        /// <summary>
        /// Get the subscription response(Subscription Created/Failed)
        /// </summary>
        /// <param name="paymentModel">Model of payment</param>
        /// <returns>Response in the form of string</returns>
        public string GetSubscriptionResponse(PaymentModel paymentModel)
        {
            TransactionService repository = new TransactionService();
            foreach (SubscriptionModel subscriptionModel in paymentModel.Subscriptions)
            {
                paymentModel.Subscription = subscriptionModel;
                GatewayResponseModel response = this.ProcessSubscription(paymentModel);
                if (response.IsSuccess)
                {
                    paymentModel.TransactionId = response.TransactionId;

                    repository.UpdatePayment(paymentModel);
                    LoggingService.LogActivity(paymentModel.PaymentApplicationSettingId, "Subscription Created");
                    // return "Success";
                }
                else
                {
                    string message = response.GatewayResponseData.Replace("<br>", "");
                    LoggingService.LogActivity(paymentModel.PaymentApplicationSettingId, "Subscription failed", message);
                    Logging.LogMessage(message, Logging.Components.Payment.ToString(), TraceLevel.Error);
                    // return string.Format("Message={0}", message);
                }

            }
            return "";
        }

        /// <summary>
        /// Execute and Get Capture status from gateway provider
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public GatewayResponseModel GetPaypalResponse(PaymentModel paymentModel)
        {
            GatewayResponseModel gatewayResponse = new GatewayResponseModel();
            paymentModel = GetPaymentSettingsModel(paymentModel);
            TransactionService repository = new TransactionService();
            Log4NetHelper.ReplaceLog4NetDLL(Convert.ToInt16(Enum.Parse(typeof(GatewayType), GatewayType.PAYPALEXPRESS.ToString())).ToString());
            var paypal = new Znode.Libraries.Paypal.PaypalGateway(paymentModel);

            paypal.PaymentActionTypeCode = "Sale";

            Znode.Libraries.Paypal.PaypalResponse response = paypal.DoPaypalExpressCheckout(paymentModel);

            if (!Equals(response.ResponseCode, "0"))//  return response.ResponseText;
            {
                gatewayResponse.IsSuccess = false;
                gatewayResponse.ResponseText = response.ResponseText;
                gatewayResponse.ResponseCode = response.ResponseCode;
                gatewayResponse.PaymentToken = response.PayalToken;
            }
            else//  return response.HostUrl;// Redirect to paypal server
            {
                gatewayResponse.IsSuccess = true;
                gatewayResponse.ResponseText = response.HostUrl;
                gatewayResponse.ResponseCode = response.ResponseCode;
                gatewayResponse.PaymentToken = response.PayalToken;
            }
            return gatewayResponse;
        }

        public GatewayResponseModel GetFinalizedPaypalResponse(PaymentModel paymentModel)
        {
            GatewayResponseModel gatewayResponse = new GatewayResponseModel();
            paymentModel = GetPaymentSettingsModel(paymentModel);
            TransactionService repository = new TransactionService();
            Log4NetHelper.ReplaceLog4NetDLL(Convert.ToInt16(Enum.Parse(typeof(GatewayType), GatewayType.PAYPALEXPRESS.ToString())).ToString());
            var paypal = new Znode.Libraries.Paypal.PaypalGateway(paymentModel);

            paypal.PaymentActionTypeCode = "Sale";
            LoggingService.LogActivity(paymentModel?.PaymentApplicationSettingId, $"{paymentModel?.PaymentApplicationSettingId} GetFinalizedPaypalResponse called"); Znode.Libraries.Paypal.PaypalResponse response = new Znode.Libraries.Paypal.PaypalResponse();

            response = paypal.GetExpressCheckoutDetails(paymentModel?.PaymentToken.ToString());
            gatewayResponse = paypal.DoExpressCheckoutPayment(paymentModel?.PaymentToken.ToString(), response.PayerID);
            if ((gatewayResponse?.IsSuccess).GetValueOrDefault())
            {
                paymentModel.TransactionId = gatewayResponse.TransactionId;
                paymentModel.TransactionDate = DateTime.Now;
                repository.AddPayment(paymentModel);
            }
            LoggingService.LogActivity(paymentModel?.PaymentApplicationSettingId, $"{paymentModel?.PaymentApplicationSettingId} after GetFinalizedPaypalResponse called");
            return gatewayResponse;
        }

        /// <summary>
        /// Execute and PaypalExpress Create order from gateway provider
        /// </summary>
        /// <param name="PaymentModel"></param>
        /// <returns></returns>
        public GatewayResponseModel GetPaypalExpressResponse(PaymentModel paymentModel)
        {
            GatewayResponseModel gatewayResponse = new GatewayResponseModel();
            PaypalExpressRestProvider paypalExpressRestProvider = new PaypalExpressRestProvider();
            paymentModel = GetPaymentSettingsModel(paymentModel);
            gatewayResponse = paypalExpressRestProvider.CreateOrder(paymentModel);

            return gatewayResponse;
        }

        /// <summary>
        /// Execute and PaypalExpress Capture and Authorize from gateway provider
        /// </summary>
        /// <param name="PaymentModel"></param>
        /// <returns></returns>
        public GatewayResponseModel GetFinalizedPaypalOrderResponse(PaymentModel paymentModel)
        {
            GatewayResponseModel gatewayResponse = new GatewayResponseModel();
            paymentModel = GetPaymentSettingsModel(paymentModel);
            TransactionService repository = new TransactionService();
            gatewayResponse = ProcessCreditCard(paymentModel);
            if ((gatewayResponse?.IsSuccess).GetValueOrDefault())
            {
                paymentModel.TransactionId = gatewayResponse.TransactionId;
                paymentModel.TransactionDate = DateTime.Now;
                repository.AddPayment(paymentModel);
            }
            LoggingService.LogActivity(paymentModel?.PaymentApplicationSettingId, $"{paymentModel?.PaymentApplicationSettingId} after GetFinalizedOrderResponse called");
            return gatewayResponse;
        }


        /// <summary>
        /// To save customer details in payment database
        /// </summary>
        /// <param name="paymentModel">Model of payment</param>
        /// <returns> returns true/false</returns>
        public bool SaveCustomerDetails(PaymentModel paymentModel, bool skipAddpayment = false)
        {
            bool isSuccess = true;
            try
            {
                //Save cc details in vault.
                if (string.IsNullOrEmpty(paymentModel.CustomerGUID) && string.IsNullOrEmpty(paymentModel.PaymentToken))
                {
                    if (AddCustomer(paymentModel) && AddAddress(paymentModel) && AddPayment(paymentModel))
                    {
                        paymentModel.CustomerProfileId = paymentModel.CustomerProfileId;
                        paymentModel.CustomerPaymentProfileId = paymentModel.CustomerPaymentProfileId; 
                    }
                    else
                        paymentModel.ResponseText = "Unable to create customer please try again.";
                }
                else if (!string.IsNullOrEmpty(paymentModel.CustomerGUID) && string.IsNullOrEmpty(paymentModel.PaymentToken) && !skipAddpayment)
                {
                    //Save data in ZnodePaymentAddress and ZnodePaymentMethod
                    if (AddAddress(paymentModel) && AddPayment(paymentModel))
                        paymentModel.CustomerPaymentProfileId = paymentModel.CustomerPaymentProfileId;
                    paymentModel.PaymentToken = paymentModel.PaymentToken;

                }
                else if (!string.IsNullOrEmpty(paymentModel.CustomerGUID) && !string.IsNullOrEmpty(paymentModel.PaymentToken))
                {
                    //Get CustomerProfileId & token from ZnodePaymentMethods table having GUID = CustomerGUID and paymentsettingId = PaymentSettingId
                    PaymentMethodsService repository = new PaymentMethodsService();
                    ZnodePaymentMethod payment = repository.GetPaymentMethod(paymentModel.PaymentApplicationSettingId, paymentModel.CustomerGUID, paymentModel.PaymentToken);
                    if (!Equals(payment, null))
                    {
                        paymentModel.CustomerProfileId = payment.CustomerProfileId;
                        paymentModel.CustomerPaymentProfileId = payment.Token;
                    }
                }
                else if (!string.IsNullOrEmpty(paymentModel.CustomerGUID) && !string.IsNullOrEmpty(paymentModel.PaymentMethodNonce))
                {
                    //Get CustomerProfileId & token from ZnodePaymentMethods table having GUID = CustomerGUID and paymentsettingId = PaymentSettingId
                    PaymentMethodsService repository = new PaymentMethodsService();
                    ZnodePaymentMethod payment = repository.GetPaymentMethod(paymentModel.PaymentApplicationSettingId, paymentModel.CustomerGUID, paymentModel.CardNumber, Convert.ToInt32(paymentModel.CardExpirationMonth), Convert.ToInt32(paymentModel.CardExpirationYear));
                    if (!Equals(payment, null))
                    {
                        paymentModel.CustomerProfileId = payment.CustomerProfileId;
                        paymentModel.CustomerPaymentProfileId = payment.Token;
                    }
                }
            }
            catch (Exception ex)
            {
                LoggingService.LogActivity(paymentModel.PaymentApplicationSettingId, ex.Message);
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                isSuccess = false;
            }
            return isSuccess;
        }

        /// <summary>
        /// To save payment details in payment database for anonymousUser
        /// </summary>
        /// <param name="paymentModel">Model of payment</param>
        /// <returns>Boolean Value true/false</returns>
        public bool SavePaymentDetails(PaymentModel paymentModel)
        {
            bool isSuccess = true;
            try
            {
                if (!string.IsNullOrEmpty(paymentModel.CustomerProfileId) && !string.IsNullOrEmpty(paymentModel.CustomerPaymentProfileId))
                {
                    if (AddPayment(paymentModel))
                    {
                        paymentModel.CustomerProfileId = paymentModel.CustomerProfileId;
                        paymentModel.CustomerPaymentProfileId = paymentModel.CustomerPaymentProfileId;
                    }
                    else
                        paymentModel.ResponseText = "Unable to create customer please try again.";
                }
            }
            catch (Exception ex)
            {
                LoggingService.LogActivity(paymentModel.PaymentApplicationSettingId, ex.Message);
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                isSuccess = false;
            }
            return isSuccess;
        }

        public TransactionDetailsModel GetTransactionDetails(string transactionId)
        {
            TransactionService repository = new TransactionService();
            PaymentModel paymentModel = new PaymentModel();
            if (!string.IsNullOrEmpty(transactionId))
            {
                var transactionDetails = repository.GetPayment(transactionId);
                if (!Equals(transactionDetails, null))
                {
                    //PaymentModel paymentModel = new PaymentModel();
                    paymentModel.CustomerProfileId = transactionDetails.CustomerProfileId;
                    paymentModel.CustomerPaymentProfileId = transactionDetails.CustomerPaymentId;
                    paymentModel.PaymentApplicationSettingId = transactionDetails.PaymentSettingId.Value;
                    paymentModel.TransactionId = transactionDetails.TransactionId;
                    paymentModel.GatewayCurrencyCode = transactionDetails.CurrencyCode;
                    paymentModel.CardDataToken = transactionDetails.Custom1;
                    paymentModel.OrderId = transactionDetails.Custom1;
                    paymentModel.GUID = Convert.ToString(transactionDetails.GUID);
                    paymentModel.CaptureTransactionId = transactionDetails.CaptureTransactionId;
                    GetPaypalExpressCode(paymentModel);
                }
            }
            IPaymentProviders _provider = GetPaymentProviderObject(paymentModel);
            if (!Equals(_provider, null))
                return _provider.GetTransactionDetails(paymentModel);

            return null;
        }

        //Return Refund Transaction id.
        public string GetRefundTransactionId(string transactionId)
        {
            TransactionService repository = new TransactionService();
            ZnodeTransaction transactionDetails = repository.GetPayment(transactionId);
            
            return transactionDetails?.RefundTransactionId;
        }
        #endregion

        #region Private Methods
        //Get Payment Settings details from DB and assigned to Payment Model.
        private PaymentModel GetPaymentSettingsModel(PaymentModel paymentModel)
        {
            PaymentSettingsService paymentrepository = new PaymentSettingsService();
            if (!Equals(paymentModel, null))
            {
                PaymentSettingsModel paymentSetting = !string.IsNullOrEmpty(paymentModel.PaymentCode) ? paymentrepository.GetPaymentSettingWithCredentials(paymentModel.PaymentCode) : paymentrepository.GetPaymentSettingWithCredentials(paymentModel.PaymentApplicationSettingId);
                if (!Equals(paymentSetting, null))
                {
                    paymentModel.GatewayTestMode = paymentSetting.TestMode;
                    paymentModel.GatewayPreAuthorize = paymentSetting.PreAuthorize;
                    paymentModel.GatewayLoginName = paymentSetting.GatewayUsername;
                    paymentModel.GatewayLoginPassword = paymentSetting.GatewayPassword;
                    paymentModel.GatewayTransactionKey = paymentSetting.TransactionKey;
                    paymentModel.GatewayPreAuthorize = paymentSetting.PreAuthorize;
                    paymentModel.GatewayType = Equals(paymentSetting.PaymentGatewayId, null) ? string.Empty : paymentSetting.PaymentGatewayId.Value.ToString();
                    paymentModel.Vendor = paymentSetting.Vendor;
                    paymentModel.Partner = paymentSetting.Partner;
                    paymentModel.PaymentApplicationSettingId = paymentSetting.PaymentSettingId;
                    PaymentSettingsService paymentSettingsService = new PaymentSettingsService();
                    paymentModel.PaymentCode = paymentSettingsService.GetPaymentSetting(paymentSetting.PaymentSettingId).PaymentCode;
                    paymentModel.GatewayCode = paymentSetting.GatewayCode;
                    if (paymentSetting.PaymentTypeId == 5)
                        paymentModel.RefundTransactionId = paymentrepository.GetAmazonUpdateTransactionId(paymentModel.TransactionId);
                }
            }
            else
            {
                paymentModel = new PaymentModel();
            }
            return paymentModel;
        }

        //Process Credit card based on Gateway type.
        private GatewayResponseModel ProcessCreditCard(PaymentModel paymentModel)
        {
            IPaymentProviders _provider = GetPaymentProviderObject(paymentModel);
            if (!Equals(_provider, null))
                return _provider.ValidateCreditcard(paymentModel);
            return null;
        }
        public PaymentGatewayTokenModel ProcessTokenGenerator(PaymentGatewayTokenModel gatewayTokenModel)
        {

            IPaymentProviders _provider = GetPaymentProvider(gatewayTokenModel.PaymentGatewayId,gatewayTokenModel.PaymentSettingId, gatewayTokenModel.GatewayCode);
            if (!Equals(_provider, null))
                return _provider.TokenGenerator(gatewayTokenModel);
            return null;
        }

        //Process Refund/Void status based on Gateway type.
        private GatewayResponseModel ProcessRefundVoid(PaymentModel paymentModel, bool isVoid = false)
        {
            IPaymentProviders _provider = GetPaymentProviderObject(paymentModel);
            if (!Equals(_provider, null))
                return isVoid ? _provider.Void(paymentModel) : _provider.Refund(paymentModel);

            return null;
        }

        //Process the subscription based on Gateway.
        private GatewayResponseModel ProcessSubscription(PaymentModel paymentModel)
        {
            IPaymentProviders _provider = GetPaymentProviderObject(paymentModel);
            if (!Equals(_provider, null))
                return _provider.Subscription(paymentModel);

            return null;
        }

        private IPaymentProviders GetPaymentProviderObject(PaymentModel paymentModel)
        {
            paymentModel = GetPaymentSettingsModel(paymentModel);
            return GetPaymentProvider(paymentModel.GatewayType, Convert.ToInt32(paymentModel.PaymentApplicationSettingId));
        }

        private IPaymentProviders GetPaymentProvider(string gatewayId, int paymentApplicationSettingId ,string gatewayCode= null)
        {
            if (!string.IsNullOrEmpty(gatewayId))
            {
                IPaymentProviders _provider = GetProvider(GetGatewayClassName(Convert.ToInt32(gatewayId)));
                Log4NetHelper.ReplaceLog4NetDLL(gatewayId);
                if (!Equals(_provider, null))
                    return _provider;
            }
            else if (!string.IsNullOrEmpty(gatewayCode))
            { 
                    IPaymentProviders _provider = GetProvider(GetGatewayClassNameByGatewayCode(gatewayCode));
                    if (!Equals(_provider, null))
                        return _provider;
            }
            else
            {
                IPaymentProviders _provider = GetProvider(GetPaymentTypeClassName(paymentApplicationSettingId));
                if (!Equals(_provider, null))
                    return _provider;
            }
            return null;
        }


        //Add new customer to PaymentCustomers table and return customer guid back in model.
        private bool AddCustomer(PaymentModel model)
        {
            PaymentCustomersService repository = new PaymentCustomersService();
            SetCardHolderName(model);
            string customerId = repository.AddPaymentCustomers(model);
            if (!string.IsNullOrEmpty(customerId))
            {
                model.CustomerGUID = customerId;
                return true;
            }
            return false;
        }

        //Add new customer to PaymentAddress table and return addressId guid back in model.
        private bool AddAddress(PaymentModel model)
        {
            PaymentAddressService repository = new PaymentAddressService();
            SetCardHolderName(model);
            string addressId = repository.AddPaymentAddress(model);
            if (!string.IsNullOrEmpty(addressId))
            {
                model.AddressId = addressId;
                return true;
            }
            return false;
        }

        //Add new payment  to PaymentMethod table and return PaymentGUID guid back in model.

        private bool AddPayment(PaymentModel model)
        {
            model.CreditCardLastFourDigit = model.CardNumber.Substring(model.CardNumber.Length - 4, 4);
            PaymentMethodsService repository = new PaymentMethodsService();
            string paymentToken = repository.AddPaymentMethods(model);
            if (!string.IsNullOrEmpty(paymentToken))
            {
                model.PaymentToken = paymentToken;
                return true;
            }
            return false;
        }

        //Set Card Holder First Name & Last Name in payment model from cardholder name field.
        private void SetCardHolderName(PaymentModel model)
        {
            if (!string.IsNullOrEmpty(model.CardHolderName))
            {
                string[] cardHolderName = model.CardHolderName.Split(' ');
                model.CardHolderFirstName = cardHolderName[0].Trim();
                if (cardHolderName.Length > 1)
                    model.CardHolderLastName = cardHolderName[1].Trim();
            }
        }

        //Delete PaymentMethod and PaymentAddress if transaction fails and data is saved in PaymentMethod and PaymentAddress table.
        private void DeleteSavedCCDetails(string customerProfileId, string CustomerPaymentProfileId)
        {
            if (!string.IsNullOrEmpty(customerProfileId) && !string.IsNullOrEmpty(CustomerPaymentProfileId))
            {
                TransactionService transactionRepository = new TransactionService();
                PaymentMethodsService paymentMethodsRepository = new PaymentMethodsService();
                if (!transactionRepository.IsTransactionPresent(customerProfileId, CustomerPaymentProfileId))
                    paymentMethodsRepository.DeletePaymentMethods(customerProfileId, CustomerPaymentProfileId);

                Logging.LogMessage("Saved card details deleted.", Logging.Components.Payment.ToString(), TraceLevel.Info);
            }
        }


        //Check the Refund amount should be smaller than Order total plus previous refund amount
        private bool CheckRefundAmountWithOrderTotal(decimal amount, ZnodeTransaction transactionDetails)
        {
            bool isGreater = false;
            decimal dbRefundAmount = transactionDetails.RefundAmount.HasValue ? transactionDetails.RefundAmount.Value : 0.0M;

            if ((dbRefundAmount + amount) > transactionDetails.Amount.Value)
                isGreater = true;

            return isGreater;
        }

        #endregion

        #region Amazon Pay

        /// <summary>
        /// Execute and Get Capture status from gateway provider
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        public GatewayResponseModel GetAmazonPayAddress(PaymentModel paymentModel)
        {
            GatewayResponseModel gatewayResponse = new GatewayResponseModel { PaymentModel = new PaymentModel() };
            paymentModel = GetPaymentSettingsModel(paymentModel);
            IPaymentProviders _provider = new AmazonPayProvider();
            GatewayResponseModel response = _provider.Subscription(paymentModel);

            if (response.HasError)
            {
                gatewayResponse.IsSuccess = false;
                gatewayResponse.ResponseText = response.ResponseText;
                gatewayResponse.ResponseCode = response.ResponseCode;
                gatewayResponse.PaymentToken = response.PaymentToken;
            }
            else
            {
                gatewayResponse.IsSuccess = true;
                gatewayResponse.ResponseText = response.HostUrl;
                gatewayResponse.ResponseCode = response.ResponseCode;
            }
            gatewayResponse.PaymentModel = response.PaymentModel;
            return gatewayResponse;
        }
        #endregion
    }
}
