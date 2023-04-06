using Braintree;
using System;
using System.Diagnostics;
using Znode.Multifront.PaymentApplication.Data;
using Znode.Multifront.PaymentApplication.Helpers;
using Znode.Multifront.PaymentApplication.Models;

namespace Znode.Multifront.PaymentApplication.Providers
{
    /// <summary>
    /// This is the class responsible for the payment related operations of braintree payment provider
    /// </summary>
    public class BraintreeProvider : BaseProvider, IPaymentProviders
    {
        #region Private Variable
        BraintreeGateway gateway = null;
        #endregion

        #region Public Methods

        /// <summary>
        /// This method is used to validate the credit cards.  Which will call the Capture call or create customer accordingly.
        /// </summary>
        /// <param name="paymentModel">Complete Payment Data from Multifront</param>
        /// <returns>Response from the payment gateway</returns>
        public GatewayResponseModel ValidateCreditcard(PaymentModel paymentModel)
        {
            gateway = MapBrainTreeCredentials(paymentModel);

            if (!string.IsNullOrEmpty(paymentModel.CustomerProfileId))
            {
                return (paymentModel.IsCapture)
                    ? CaptureTransaction(paymentModel) : AuthorizeTransaction(paymentModel);
            }
            return CreateCustomer(paymentModel);
        }

        /// <summary>
        /// This method will create the subscription in the payment gateway.
        /// </summary>
        /// <param name="paymentModel">Complete Payment Data from Multifront</param>
        /// <returns>Response from the payment gateway</returns>
        public GatewayResponseModel Subscription(PaymentModel paymentModel)
        {
            gateway = MapBrainTreeCredentials(paymentModel);
            GatewayResponseModel paymentGatewayResponse = new GatewayResponseModel();
            try
            {
                if (!string.IsNullOrEmpty(paymentModel.TransactionId))
                {
                    SubscriptionRequest subscriptionRequest = new SubscriptionRequest
                    {
                        Price = decimal.Parse(paymentModel.Subscription.InitialAmount.ToString()),
                        FirstBillingDate = DateTime.Now,
                        NeverExpires = false,
                        HasTrialPeriod = false,
                        NumberOfBillingCycles = paymentModel.Subscription.TotalCycles,
                        PlanId = paymentModel.Subscription.ProfileName,
                    };

                    Result<Subscription> subscriptionResult = gateway.Subscription.Create(subscriptionRequest);

                    paymentGatewayResponse.IsSuccess = subscriptionResult.IsSuccess();
                    paymentGatewayResponse.ResponseText = subscriptionResult.Message;
                }
                else
                {
                    paymentGatewayResponse.IsSuccess = false;
                    paymentGatewayResponse.ResponseText = "Error occurred while retrieving Transaction ID.";
                }
            }
            catch (Exception ex)
            {
                paymentGatewayResponse.IsSuccess = false;
                paymentGatewayResponse.ResponseText = ex.Message.ToString();
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(paymentModel.PaymentApplicationSettingId, ex.Message);
            }

            return paymentGatewayResponse;
        }

        /// <summary>
        /// This method will void the payment.
        /// </summary>
        /// <param name="paymentModel">Complete Payment Data from Multifront</param>
        /// <returns>Response from the payment gateway</returns>
        public GatewayResponseModel Void(PaymentModel paymentModel)
        {
            gateway = MapBrainTreeCredentials(paymentModel);
            GatewayResponseModel paymentGatewayResponse = new GatewayResponseModel();
            try
            {
                if (!string.IsNullOrEmpty(paymentModel.TransactionId))
                {
                    //Call gateway void method
                    Result<Transaction> transactionResult = gateway.Transaction.Void(paymentModel.TransactionId);
                    paymentGatewayResponse.IsSuccess = transactionResult.IsSuccess();
                    paymentGatewayResponse.ResponseText = transactionResult.Message;
                    if (paymentGatewayResponse.IsSuccess)
                        paymentGatewayResponse.PaymentStatus = ZnodePaymentStatus.VOIDED;
                }
                else
                {
                    paymentGatewayResponse.IsSuccess = false;
                    paymentGatewayResponse.ResponseText = "Error occurred while retrieving Transaction ID.";
                }
            }
            catch (Exception ex)
            {
                paymentGatewayResponse.IsSuccess = false;
                paymentGatewayResponse.ResponseText = ex.Message.ToString();
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(null, ex.Message);
            }
            return paymentGatewayResponse;
        }

        /// <summary>
        /// This method will refund the payment amount to the relevant customer.
        /// </summary>
        /// <param name="paymentModel">Complete Payment Data from Multifront</param>
        /// <returns>Response from the payment gateway</returns>
        public GatewayResponseModel Refund(PaymentModel paymentModel)
        {
            gateway = MapBrainTreeCredentials(paymentModel);
            GatewayResponseModel paymentGatewayResponse = new GatewayResponseModel();
            try
            {
                if (!string.IsNullOrEmpty(paymentModel.TransactionId))
                {
                    //Refund Transaction
                    Result<Transaction> transactionResult = gateway.Transaction.Refund(paymentModel.TransactionId, decimal.Parse(paymentModel.Total));
                    paymentGatewayResponse.IsSuccess = transactionResult.IsSuccess();
                    paymentGatewayResponse.ResponseText = transactionResult.Message;
                    paymentGatewayResponse.TransactionId = transactionResult.Target.Id;
                    if (paymentGatewayResponse.IsSuccess)
                        paymentGatewayResponse.PaymentStatus = ZnodePaymentStatus.REFUNDED;
                }
                else
                    paymentGatewayResponse.ResponseText = "Error occurred while retrieving Transaction ID.";
            }
            catch (Exception ex)
            {
                paymentGatewayResponse.IsSuccess = false;
                paymentGatewayResponse.ResponseText = ex.Message.ToString();
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(null, ex.Message);
            }

            return paymentGatewayResponse;
        }

        public TransactionDetailsModel GetTransactionDetails(PaymentModel paymentModel)
        {
            if(Equals(paymentModel, null))
                return new TransactionDetailsModel();

            gateway = MapBrainTreeCredentials(paymentModel);
            TransactionDetailsModel transactionStatus = new TransactionDetailsModel();

            try
            {
                Transaction transaction = gateway.Transaction.Find(paymentModel.TransactionId);
                if (transaction?.Status != null)
                {
                    transactionStatus.TransactionStatus = transaction.Status.ToString();
                    transactionStatus.TransactionType = transaction.Type.ToString();
                    transactionStatus.TransactionId = paymentModel.TransactionId;
                    transactionStatus.IsSuccess = true;
                }
                else
                {
                    transactionStatus.IsSuccess = false;
                    transactionStatus.ResponseText = "Unable to get transaction details.";
                }
            }
            catch (Exception ex)
            {
                transactionStatus.IsSuccess = false;
                transactionStatus.ResponseText = ex.Message.ToString();
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(null, ex.Message);
            }

            return transactionStatus;
        }

        #endregion

        #region Privare Methods

        /// <summary>
        /// This method will create the customer in the payment gateway.
        /// </summary>
        /// <param name="paymentModel">Complete Payment Data from Multifront</param>
        /// <returns>Response from the payment gateway</returns>
        private GatewayResponseModel CreateCustomer(PaymentModel paymentModel)
        {
            GatewayResponseModel response = new GatewayResponseModel();
            GatewayConnector gatewayConnector = new GatewayConnector();
            bool isSuccess = false;
            if (!paymentModel.IsAnonymousUser)
            {
                if (string.IsNullOrEmpty(paymentModel.CustomerGUID))
                {
                    // Create Customer and save Credit card info
                    if (paymentModel.IsSaveCreditCard)
                    {
                        response = CreatePaymentGatewayCustomer(paymentModel);
                        if (response.IsSuccess)
                            isSuccess = gatewayConnector.SaveCustomerDetails(paymentModel);
                        response.CustomerGUID = paymentModel.CustomerGUID;
                        response.PaymentToken = paymentModel.PaymentToken;
                        response.CustomerProfileId = paymentModel.CustomerProfileId;
                        response.CustomerPaymentProfileId = paymentModel.CustomerPaymentProfileId;
                        response.IsSuccess = isSuccess;
                        return response;
                    }
                    else
                        return CreatePaymentGatewayCustomer(paymentModel);
                }
                else
                {
                    //Save credit card info for existing customer
                    if (paymentModel.IsSaveCreditCard)
                    {
                        response = CreatePaymentGatewayVault(paymentModel);
                        if (response.IsSuccess)
                            isSuccess = gatewayConnector.SaveCustomerDetails(paymentModel);
                        response.CustomerGUID = paymentModel.CustomerGUID;
                        response.PaymentToken = paymentModel.PaymentToken;
                        response.CustomerProfileId = paymentModel.CustomerProfileId;
                        response.CustomerPaymentProfileId = paymentModel.CustomerPaymentProfileId;
                        response.IsSuccess = isSuccess;
                        return response;
                    }
                    else if (!string.IsNullOrEmpty(paymentModel.PaymentToken))
                    {
                        isSuccess = gatewayConnector.SaveCustomerDetails(paymentModel);
                        response.CustomerGUID = paymentModel.CustomerGUID;
                        response.PaymentToken = paymentModel.PaymentToken;
                        response.CustomerProfileId = paymentModel.CustomerProfileId;
                        response.CustomerPaymentProfileId = paymentModel.CustomerPaymentProfileId;
                        response.IsSuccess = isSuccess;
                        return response;
                    }
                    else if (!string.IsNullOrEmpty(paymentModel.PaymentMethodNonce))
                    {
                        isSuccess = gatewayConnector.SaveCustomerDetails(paymentModel,true);
                        PaymentMethodsService repository = new PaymentMethodsService();
                        ZnodePaymentMethod payment = repository.GetPaymentMethod(paymentModel.PaymentApplicationSettingId, paymentModel.CustomerGUID, paymentModel.CardNumber, Convert.ToInt32(paymentModel.CardExpirationMonth), Convert.ToInt32(paymentModel.CardExpirationYear));
                        response.CustomerGUID = paymentModel.CustomerGUID;
                        response.PaymentToken = payment.Token;
                        response.CustomerProfileId = payment.CustomerProfileId;
                        response.CustomerPaymentProfileId = payment.Token;
                        response.IsSuccess = isSuccess;
                        return response;
                    }
                    else
                        return CreatePaymentGatewayCustomer(paymentModel);   //As it is which is the normal one without vault.
                }
            }
            else
                return CreatePaymentGatewayCustomer(paymentModel);   //As it is which is the normal one without vault.
        }

        /// <summary>
        /// This method will Authorize transaction.
        /// </summary>
        /// <param name="paymentModel">Complete Payment Data from Multifront</param>
        /// <returns>Response from the payment gateway</returns>
        private GatewayResponseModel AuthorizeTransaction(PaymentModel paymentModel)
        {
            try
            {
                TransactionRequest transRequest = new TransactionRequest
                {
                    Amount = decimal.Parse(paymentModel.Total),
                    CustomerId = paymentModel.CustomerProfileId,
                    PaymentMethodToken = paymentModel.CustomerPaymentProfileId,
                    PaymentMethodNonce = paymentModel.PaymentMethodNonce, //This field is specific for braintree PCI compliance.

                    //SubmitForSettlement= true will proceed with capture payment, SubmitForSettlement= false will Authorize payment 
                    Options = new TransactionOptionsRequest { SubmitForSettlement = false },
                    OrderId = paymentModel.OrderId //Order number pass to transaction details of braintree sandbox
                };
                Result<Transaction> transactionResult = gateway.Transaction.Sale(transRequest);
                return MapGatewayResponseModel(transactionResult, paymentModel, false);
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(null, ex.Message);
                return new GatewayResponseModel
                {
                    GatewayResponseData = ex.Message.ToString(),
                    IsSuccess = false
                };
            }
        }

        /// <summary>
        /// Capture the Authorized Transaction
        /// </summary>
        /// <param name="paymentModel">paymentModel</param>
        /// <returns>GatewayResponseModel</returns>
        private GatewayResponseModel CaptureTransaction(PaymentModel paymentModel)
        {
            try
            {
                //Capture Transaction
                Result<Transaction> transactionResult = gateway.Transaction.SubmitForSettlement(paymentModel.TransactionId);
                return MapGatewayResponseModel(transactionResult, paymentModel, true);
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(null, ex.Message);
                return new GatewayResponseModel
                {
                    GatewayResponseData = ex.Message.ToString(),
                    IsSuccess = false
                };

            }
        }

        /// <summary>
        /// Create a Customer using card holder data.
        /// </summary>
        /// <param name="paymentModel"></param>
        /// <returns></returns>
        private GatewayResponseModel GetCustomer(PaymentModel paymentModel)
        {
            GatewayResponseModel gatewayResponseModel = new GatewayResponseModel();
            try
            {
                CustomerRequest customerRequest = new CustomerRequest
                {
                    FirstName = paymentModel.BillingFirstName,
                    LastName = paymentModel.BillingLastName,
                    Company = string.Empty,
                    Email = paymentModel.BillingEmailId,
                    Fax = paymentModel.BillingPhoneNumber,
                    Phone = paymentModel.BillingPhoneNumber,
                    Website = string.Empty,
                };

                Result<Customer> resultCustomer = gateway.Customer.Create(customerRequest);
                resultCustomer.IsSuccess();
                string customerId = resultCustomer.Target.Id;
                gatewayResponseModel.IsSuccess = !string.IsNullOrEmpty(customerId);
                gatewayResponseModel.CustomerProfileId = customerId;
            }
            catch (Exception ex)
            {
                gatewayResponseModel.IsSuccess = false;
                gatewayResponseModel.GatewayResponseData = ex.Message.ToString();
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(null, ex.Message);
            }

            return gatewayResponseModel;
        }

        /// <summary>
        /// Create the Customer Payment
        /// </summary>
        /// <param name="paymentModel"></param>
        /// <returns>returns customer payment response</returns>
        private GatewayResponseModel CreateCustomerPayment(PaymentModel paymentModel)
        {
            GatewayResponseModel response = new GatewayResponseModel();

                var request = new PaymentMethodRequest
            {
                CustomerId = paymentModel.CustomerProfileId,
                PaymentMethodNonce = paymentModel.PaymentMethodNonce
            };

            Result<PaymentMethod> ccResult = gateway.PaymentMethod.Create(request);
            response.IsSuccess = ccResult.IsSuccess();
            if (response.IsSuccess)
            {
                response.CustomerProfileId = paymentModel.CustomerProfileId;
                response.CustomerPaymentProfileId = ccResult.Target.Token;
                paymentModel.CreditCardImageUrl = ccResult.Target.ImageUrl;
            }
            else
            {
                response.CustomerProfileId = paymentModel.CustomerProfileId;
                response.ResponseText = ccResult.Message;
            }
            return response;
        }

        /// <summary>
        /// To Customer new customer account and ADD vault
        /// </summary>
        /// <param name="paymentModel"></param>
        /// <returns>returns gateway response</returns>
        private GatewayResponseModel CreatePaymentGatewayCustomer(PaymentModel paymentModel)
        {
            GatewayResponseModel response = GetCustomer(paymentModel);
            if (!string.IsNullOrEmpty(response.CustomerProfileId))
            {
                paymentModel.CustomerProfileId = response.CustomerProfileId;
                response = CreateCustomerPayment(paymentModel);
                paymentModel.CustomerPaymentProfileId = response.CustomerPaymentProfileId;

                if (response.IsSuccess && paymentModel.IsAnonymousUser)
                    response.IsSuccess = new GatewayConnector().SavePaymentDetails(paymentModel);
            }
            return Equals(response, null) ? new GatewayResponseModel() : response;
        }

        /// <summary>
        /// To vault using existing customer id
        /// </summary>
        /// <param name="paymentModel"></param>
        /// <returns>returns gateway response</returns>
        private GatewayResponseModel CreatePaymentGatewayVault(PaymentModel paymentModel)
        {
            GatewayResponseModel gatewayResponseModel = new GatewayResponseModel();

            ZnodePaymentMethod payment = new PaymentMethodsService().GetPaymentMethod(paymentModel.PaymentApplicationSettingId, paymentModel.CustomerGUID,true);
            if (!Equals(payment, null) && !string.IsNullOrEmpty(payment.CustomerProfileId))
                paymentModel.CustomerProfileId = payment.CustomerProfileId;
            else
            {
                gatewayResponseModel = GetCustomer(paymentModel);
                paymentModel.CustomerProfileId = gatewayResponseModel.CustomerProfileId;
            }
            if (!string.IsNullOrEmpty(paymentModel.CustomerProfileId))
            {
                gatewayResponseModel = CreateCustomerPayment(paymentModel);
                paymentModel.CustomerPaymentProfileId = gatewayResponseModel.CustomerPaymentProfileId;
            }
            return Equals(gatewayResponseModel, null) ? new GatewayResponseModel() : gatewayResponseModel;
        }

        //Map GatewayResponseModel
        private GatewayResponseModel MapGatewayResponseModel(Result<Transaction> transactionResult, PaymentModel paymentModel, bool isCapture)
        {
            GatewayResponseModel gatewayResponseModel = new GatewayResponseModel();
            gatewayResponseModel.IsSuccess = transactionResult.IsSuccess();

            if (gatewayResponseModel.IsSuccess)
            {
                gatewayResponseModel.TransactionId = transactionResult.Target.Id;
                gatewayResponseModel.PaymentStatus = isCapture ? ZnodePaymentStatus.CAPTURED : ZnodePaymentStatus.AUTHORIZED;
                gatewayResponseModel.CustomerProfileId = paymentModel.CustomerProfileId;
                gatewayResponseModel.CustomerPaymentProfileId = paymentModel.CustomerPaymentProfileId;
                gatewayResponseModel.ResponseText = transactionResult.Message;
                gatewayResponseModel.IsGatewayPreAuthorize = paymentModel.GatewayPreAuthorize;
            }
            else
            {
                gatewayResponseModel.ResponseText = transactionResult.Transaction.ProcessorResponseText;
                gatewayResponseModel.ResponseCode = transactionResult.Transaction.ProcessorResponseCode;
            }
            return gatewayResponseModel;
        }

        //Map Braintree Credentials
        private BraintreeGateway MapBrainTreeCredentials(PaymentModel paymentModel)
        => new BraintreeGateway
        {
            Environment = paymentModel.GatewayTestMode ? Braintree.Environment.SANDBOX : Braintree.Environment.PRODUCTION,
            MerchantId = paymentModel.GatewayLoginName,
            PublicKey = paymentModel.GatewayLoginPassword,
            PrivateKey = paymentModel.GatewayTransactionKey
        };

        public PaymentGatewayTokenModel TokenGenerator(PaymentGatewayTokenModel paymentGatewayTokenModel)
        {
            if (!string.IsNullOrEmpty(paymentGatewayTokenModel.CustomerGUID))
            {
                PaymentSettingsModel paymentSettingsModel = new PaymentSettingsService().GetPaymentSettingsByPaymentCode(paymentGatewayTokenModel.PaymentCode);
                ZnodePaymentMethod payment = new PaymentMethodsService().GetPaymentMethodForACH(paymentSettingsModel.PaymentSettingId, paymentGatewayTokenModel.CustomerGUID);
                if (payment != null)
                {
                    paymentGatewayTokenModel.PaymentGatewayToken = MapBrainTreeCredentials(new PaymentModel { GatewayTestMode = paymentGatewayTokenModel.GatewayTestMode, GatewayLoginName = paymentGatewayTokenModel.GatewayLoginName, GatewayLoginPassword = paymentGatewayTokenModel.GatewayPassword, GatewayTransactionKey = paymentGatewayTokenModel.GatewayTransactionKey }).ClientToken.generate(new ClientTokenRequest
                    {
                        CustomerId = payment.CustomerProfileId
                    });
                    paymentGatewayTokenModel.CustomerProfileId = payment.CustomerProfileId;

                }
                else
                {
                    paymentGatewayTokenModel.PaymentGatewayToken = MapBrainTreeCredentials(new PaymentModel { GatewayTestMode = paymentGatewayTokenModel.GatewayTestMode, GatewayLoginName = paymentGatewayTokenModel.GatewayLoginName, GatewayLoginPassword = paymentGatewayTokenModel.GatewayPassword, GatewayTransactionKey = paymentGatewayTokenModel.GatewayTransactionKey }).ClientToken.generate();
                }
            }
            else
            {
                paymentGatewayTokenModel.PaymentGatewayToken = MapBrainTreeCredentials(new PaymentModel { GatewayTestMode = paymentGatewayTokenModel.GatewayTestMode, GatewayLoginName = paymentGatewayTokenModel.GatewayLoginName, GatewayLoginPassword = paymentGatewayTokenModel.GatewayPassword, GatewayTransactionKey = paymentGatewayTokenModel.GatewayTransactionKey }).ClientToken.generate();
            }
            return paymentGatewayTokenModel;
        }
        #endregion
    }
}
