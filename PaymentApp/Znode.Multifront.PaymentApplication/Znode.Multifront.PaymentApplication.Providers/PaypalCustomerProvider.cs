using System;
using System.Configuration;
using System.Diagnostics;
using Znode.Libraries.PaypalRest;
using Znode.Multifront.PaymentApplication.Data;
using Znode.Multifront.PaymentApplication.Helpers;
using Znode.Multifront.PaymentApplication.Models;

namespace Znode.Multifront.PaymentApplication.Providers
{
    public class PaypalCustomerProvider : BaseProvider, IPaymentProviders
    {
        /// <summary>
        /// Validate the credit card based on the input
        /// </summary>
        /// <param name="paymentModel"></param>
        /// <returns></returns>
        public GatewayResponseModel ValidateCreditcard(PaymentModel paymentModel)
        {
            PaypalRestApi paypal = new PaypalRestApi();

            // If CustomerProfileId is present then it will process the capture payment
            if (!string.IsNullOrEmpty(paymentModel.CustomerProfileId))
            {
                paymentModel.GatewayCurrencyCode = Convert.ToString(ConfigurationManager.AppSettings["CurrencyCode"]);
                return paypal.CreateTransaction(paymentModel);
            }
            else
                return CreateCustomer(paymentModel);
        }

        /// <summary>
        /// Refund Payment
        /// </summary>
        /// <param name="paymentModel"></param>
        /// <returns></returns>
        public GatewayResponseModel Refund(PaymentModel paymentModel)
            => new PaypalRestApi().Refund(paymentModel);

        /// <summary>
        /// Void Transaction
        /// </summary>
        /// <param name="paymentModel"></param>
        /// <returns></returns>
        public GatewayResponseModel Void(PaymentModel paymentModel)
             => new PaypalRestApi().VoidPayment(paymentModel);

        /// <summary>
        /// Create Subscription
        /// </summary>
        /// <param name="paymentModel"></param>
        /// <returns></returns>
        public GatewayResponseModel Subscription(PaymentModel paymentModel)
        {
            try
            {
                PaypalRestApi paypal = new PaypalRestApi();
                return paypal.CreateSubscription(paymentModel);
            }
            catch (Exception ex)
            {
                LoggingService.LogActivity(paymentModel.PaymentApplicationSettingId, ex.Message, paymentModel.TransactionId);
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                return new GatewayResponseModel { IsSuccess = false, GatewayResponseData = "An error occurred" };
            }
        }

        public TransactionDetailsModel GetTransactionDetails(PaymentModel paymentModel)
        {
            return new TransactionDetailsModel();
        }

        /// <summary>
        /// This method will create customer profile 
        /// </summary>
        /// <param name="paymentModel">PaymentModel paymentModel</param>
        /// <returns>returns customer details response</returns>
        private GatewayResponseModel CreateCustomer(PaymentModel paymentModel)
        {
            PaypalRestApi paypal = new PaypalRestApi();
            GatewayResponseModel response = new GatewayResponseModel();
            GatewayConnector gatewayConnector = new GatewayConnector();
            bool isSuccess = false;

            if (Equals(paymentModel.IsAnonymousUser, false))
            {
                if (string.IsNullOrEmpty(paymentModel.CustomerGUID))
                {
                    if (Equals(paymentModel.IsSaveCreditCard, true))
                    {
                        response = paypal.CreateCustomerInPaypal(paymentModel);
                        if (response.IsSuccess)
                            isSuccess = gatewayConnector.SaveCustomerDetails(paymentModel);
                        response.CustomerGUID = paymentModel.CustomerGUID;
                        response.PaymentToken = paymentModel.CustomerPaymentProfileId;
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
                    if (Equals(paymentModel.IsSaveCreditCard, true))
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
                        response.PaymentToken = paymentModel.CustomerPaymentProfileId;
                        response.CustomerProfileId = paymentModel.CustomerProfileId;
                        response.CustomerPaymentProfileId = paymentModel.CustomerPaymentProfileId;
                        response.IsSuccess = isSuccess;
                        return response;
                    }
                    else
                        return CreatePaymentGatewayCustomer(paymentModel);
                }
            }
            else
                return CreatePaymentGatewayCustomer(paymentModel);
        }

        /// <summary>
        /// To Customer new customer account and ADD vault
        /// </summary>
        /// <param name="paymentModel"></param>
        /// <returns>returns gateway response</returns>
        private GatewayResponseModel CreatePaymentGatewayCustomer(PaymentModel paymentModel)
        {
            PaypalRestApi paypal = new PaypalRestApi();
            GatewayResponseModel response = paypal.CreateCustomerInPaypal(paymentModel);
            if (response.IsSuccess && paymentModel.IsAnonymousUser)
            {
                GatewayConnector gatewayConnector = new GatewayConnector();
                response.IsSuccess = gatewayConnector.SavePaymentDetails(paymentModel);
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
            GatewayResponseModel response = new GatewayResponseModel();
            PaymentMethodsService repository = new PaymentMethodsService();
            PaypalRestApi paypal = new PaypalRestApi();
            ZnodePaymentMethod payment = repository.GetPaymentMethod(paymentModel.PaymentApplicationSettingId, paymentModel.CustomerGUID);

            if (!Equals(payment, null) && !string.IsNullOrEmpty(payment.CustomerProfileId))
            {
                paymentModel.CustomerProfileId = payment.CustomerProfileId;
                response = paypal.CreateCustomerInPaypal(paymentModel);
            }
            else
                response = paypal.CreateCustomerInPaypal(paymentModel);
            return Equals(response, null) ? new GatewayResponseModel() : response;
        }

        public PaymentGatewayTokenModel TokenGenerator(PaymentGatewayTokenModel paymentModel)
        {
            throw new NotImplementedException();
        }
    }
}
