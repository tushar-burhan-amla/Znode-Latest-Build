using CyberSource.Api;
using CyberSource.Clients;
using CyberSource.Model;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Net;
using Znode.Multifront.PaymentApplication.Data;
using Znode.Multifront.PaymentApplication.Helpers;
using Znode.Multifront.PaymentApplication.Models;
using CyberSource.Clients.SoapServiceReference;
using System.Web;

namespace Znode.Multifront.PaymentApplication.Providers
{
    /// <summary>
    /// This class will have all the methods of implementation of Cybersource payment provider 
    /// </summary>
    public class CyberSourceCustomerProviderProvider : BaseProvider, IPaymentProviders
    {
        public const string CyberSourceResponseSuccess = "100";

        /// <summary>
        /// Validate the credit card through Cybersource
        /// </summary>
        /// <param name="paymentModel"></param>
        /// <returns></returns>
        public GatewayResponseModel ValidateCreditcard(PaymentModel paymentModel)
        {
            GatewayResponseModel paymentGatewayResponse = new GatewayResponseModel();

            if (string.IsNullOrEmpty((paymentModel.CyberSourceToken)) && paymentModel.PaymentGUID != Guid.Empty)
            {
                PaymentMethodsService repository = new PaymentMethodsService();
                string cardNumber = paymentModel.CardNumber;
                string token = repository.GetSavedCardDetailsByPaymentGuid(paymentModel.PaymentGUID, out cardNumber);
                paymentModel.CustomerPaymentProfileId = DecryptPaymentToken(token);
                paymentModel.CardNumber = cardNumber;


            }


            paymentGatewayResponse = (paymentModel.IsCapture)
                    ? CaptureTransaction(paymentModel, paymentGatewayResponse) : AuthorizeTransaction(paymentModel, paymentGatewayResponse);
         
            return (!paymentModel.IsCapture && paymentGatewayResponse.IsSuccess) ? CreateCustomer(paymentModel, paymentGatewayResponse) : paymentGatewayResponse;
        }



        /// <summary>
        /// This method will call the refund payment of Cybersource payment provider
        /// </summary>
        /// <param name="paymentModel">payment model</param>
        /// <returns>returns the refund response</returns>
        public GatewayResponseModel Refund(PaymentModel paymentModel)
        {
            GatewayResponseModel paymentGatewayResponse = new GatewayResponseModel();
            try
            {
                //Create Request object and map parameters
                PtsV2PaymentsRefundPost201Response reply = RefundTransaction(paymentModel);
                //Map response
                paymentGatewayResponse.ResponseCode = reply.ProcessorInformation.ResponseCode;
                paymentGatewayResponse.GatewayResponseData = this.GetReasoncodeDescription(reply.ProcessorInformation.ResponseCode);

                //100 is success response code
                if (reply.ProcessorInformation.ResponseCode.Equals(CyberSourceResponseSuccess))
                {
                    paymentGatewayResponse.TransactionId = reply.Id;
                    paymentGatewayResponse.IsSuccess = true;
                    paymentGatewayResponse.PaymentStatus = ZnodePaymentStatus.REFUNDED;
                }
            }
            catch (Exception ex)
            {
                paymentGatewayResponse.GatewayResponseData = this.GetReasoncodeDescription(string.Empty);
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(null, ex.Message);
            }

            return paymentGatewayResponse;
        }

        public PtsV2PaymentsRefundPost201Response RefundTransaction(PaymentModel paymentModel)
        {
            var id = paymentModel.CustomerProfileId;

            // This is a ReferenceCode against order for Znode.
            string clientReferenceInformationCode = paymentModel.OrderNumber;
            Ptsv2paymentsClientReferenceInformation clientReferenceInformation = new Ptsv2paymentsClientReferenceInformation(
                Code: clientReferenceInformationCode
           );

            string orderInformationAmountDetailsTotalAmount = paymentModel.Total;
            string orderInformationAmountDetailsCurrency = paymentModel.GatewayCurrencyCode;
            Ptsv2paymentsidcapturesOrderInformationAmountDetails orderInformationAmountDetails = new Ptsv2paymentsidcapturesOrderInformationAmountDetails(
                TotalAmount: orderInformationAmountDetailsTotalAmount,
                Currency: orderInformationAmountDetailsCurrency
           );

            Ptsv2paymentsidrefundsOrderInformation orderInformation = new Ptsv2paymentsidrefundsOrderInformation(
                AmountDetails: orderInformationAmountDetails
           );

            var requestObj = new RefundPaymentRequest(
                ClientReferenceInformation: clientReferenceInformation,
                OrderInformation: orderInformation
           );

            try
            {
                CyberSource.Client.Configuration clientConfig = GetConfigurationDetails(paymentModel.PaymentCode);
                var apiInstance = new RefundApi(clientConfig);
                PtsV2PaymentsRefundPost201Response result = apiInstance.RefundPayment(requestObj, id);
                return result;
            }
            catch (Exception e)
            {
                Console.WriteLine("Exception on calling the API : " + e.Message);
                return null;
            }
        }

        public PtsV2PaymentsCapturesPost201Response Capture(PaymentModel paymentModel)
        {
            var id = paymentModel.CustomerProfileId;
            // This is a ReferenceCode against order for Znode.
            string clientReferenceInformationCode = "Capture_" + paymentModel.OrderNumber;
            Ptsv2paymentsClientReferenceInformation clientReferenceInformation = new Ptsv2paymentsClientReferenceInformation(
                Code: clientReferenceInformationCode
           );

            string orderInformationAmountDetailsTotalAmount = paymentModel.Total;
            string orderInformationAmountDetailsCurrency = paymentModel.GatewayCurrencyCode;
            Ptsv2paymentsidcapturesOrderInformationAmountDetails orderInformationAmountDetails = new Ptsv2paymentsidcapturesOrderInformationAmountDetails(
                TotalAmount: orderInformationAmountDetailsTotalAmount,
                Currency: orderInformationAmountDetailsCurrency
           );


            Ptsv2paymentsidcapturesOrderInformation orderInformation = new Ptsv2paymentsidcapturesOrderInformation(
                AmountDetails: orderInformationAmountDetails
           );

            var requestObj = new CapturePaymentRequest(
                ClientReferenceInformation: clientReferenceInformation,
                OrderInformation: orderInformation
           );

            try
            {
                CyberSource.Client.Configuration clientConfig = GetConfigurationDetails(paymentModel.PaymentCode);

                var apiInstance = new CaptureApi(clientConfig);
                PtsV2PaymentsCapturesPost201Response result = apiInstance.CapturePayment(requestObj, id);
                return result;
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(null, ex.Message);
                return null;
            }
        }

        public PtsV2PaymentsPost201Response Authorize(PaymentModel paymentModel)
        {
            // This is a ReferenceCode against order for Znode.
            string clientReferenceInformationCode = "Authorize_" + paymentModel.OrderNumber;
            Ptsv2paymentsClientReferenceInformation clientReferenceInformation = new Ptsv2paymentsClientReferenceInformation(
                Code: clientReferenceInformationCode
           );
            Ptsv2paymentsProcessingInformation processingInformation = null;


            var orderInformationAmountDetails = new Ptsv2paymentsOrderInformationAmountDetails
            {
                TotalAmount = paymentModel.Total,
                Currency = paymentModel.GatewayCurrencyCode
            };

            var orderInformationBillTo = new Ptsv2paymentsOrderInformationBillTo
            {
                Country = paymentModel.BillingCountryCode,
                FirstName = paymentModel.BillingFirstName,
                LastName = paymentModel.BillingLastName,
                Address1 = paymentModel.BillingStreetAddress1,
                PostalCode = paymentModel.BillingPostalCode,
                Locality = paymentModel.BillingCity,
                AdministrativeArea = paymentModel.BillingStateCode,
                Email = paymentModel.BillingEmailId
            };

            Ptsv2paymentsOrderInformationShipTo orderInformationShipTo = new Ptsv2paymentsOrderInformationShipTo(
                FirstName: paymentModel.ShippingFirstName,
                LastName: paymentModel.ShippingLastName,
                Address1: paymentModel.ShippingStreetAddress1,
                Locality: paymentModel.ShippingCity,
                AdministrativeArea: paymentModel.ShippingStateCode,
                PostalCode: paymentModel.ShippingPostalCode,
                Country: paymentModel.ShippingCountryCode
           );

            Ptsv2paymentsOrderInformation orderInformation = new Ptsv2paymentsOrderInformation(
                AmountDetails: orderInformationAmountDetails,
                BillTo: orderInformationBillTo,
                ShipTo: orderInformationShipTo
           );
            CreatePaymentRequest requestObj = null;
            if (!string.IsNullOrEmpty(paymentModel.CyberSourceToken))
            {
                List<string> processingInformationActionList = new List<string>();
                processingInformationActionList.Add("TOKEN_CREATE");

                List<string> processingInformationActionTokenTypes = new List<string>();
                processingInformationActionTokenTypes.Add("customer");
                processingInformationActionTokenTypes.Add("paymentInstrument");
                processingInformationActionTokenTypes.Add("shippingAddress");
                processingInformationActionTokenTypes.Add("card");

                bool processingInformationCapture = false;

                processingInformation = new Ptsv2paymentsProcessingInformation(
                    ActionList: processingInformationActionList,
                    ActionTokenTypes: processingInformationActionTokenTypes,
                    Capture: processingInformationCapture
               );

                string tokenInformationTransientTokenJwt = paymentModel.CyberSourceToken;
                Ptsv2paymentsTokenInformation tokenInformation = new Ptsv2paymentsTokenInformation(
                    TransientTokenJwt: tokenInformationTransientTokenJwt
               );
                requestObj = new CreatePaymentRequest(
                   ClientReferenceInformation: clientReferenceInformation,
                   ProcessingInformation: processingInformation,
                   OrderInformation: orderInformation,
                   TokenInformation: tokenInformation
              );
            }
            else
            {

                Ptsv2paymentsPaymentInformation tokenInformation1 = new Ptsv2paymentsPaymentInformation();
                tokenInformation1.PaymentInstrument = new Ptsv2paymentsPaymentInformationPaymentInstrument();
                tokenInformation1.PaymentInstrument.Id = paymentModel.CustomerPaymentProfileId;
                requestObj = new CreatePaymentRequest(
                   ClientReferenceInformation: clientReferenceInformation,
                   OrderInformation: orderInformation,
                   PaymentInformation: tokenInformation1
                   );
            }
            try
            {
                CyberSource.Client.Configuration clientConfig = GetConfigurationDetails(paymentModel.PaymentCode);
                PaymentsApi apiInstance = new PaymentsApi(clientConfig);
                PtsV2PaymentsPost201Response result = apiInstance.CreatePayment(requestObj);
                return result;
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(null, ex.Message);
                return null;
            }
        }

        private static Ptsv2paymentsOrderInformationAmountDetails OrderInformationCyberSource(PaymentModel paymentModel)
        {
            return new Ptsv2paymentsOrderInformationAmountDetails
            {
                TotalAmount = paymentModel.Total,
                Currency = paymentModel.GatewayCurrencyCode
            };
        }

        private CyberSource.Client.Configuration GetConfigurationDetails(string paymentCode)
        {
            Dictionary<string, string> configDictionary = new Configuration().GetConfiguration(paymentCode);
            CyberSource.Client.Configuration clientConfig = new CyberSource.Client.Configuration(merchConfigDictObj: configDictionary);
            return clientConfig;
        }

        /// <summary>
        /// This method will call the void payment of Cybersource payment provider
        /// </summary>
        /// <param name="paymentModel">payment model</param>
        /// <returns>returns the void response</returns>
        public GatewayResponseModel Void(PaymentModel paymentModel)
        {
            GatewayResponseModel paymentGatewayResponse = new GatewayResponseModel();

            try
            {
                //Create Request object and map parameters
                PtsV2PaymentsVoidsPost201Response reply = VoidTransaction(paymentModel);

                //Map response
                paymentGatewayResponse.Token = reply.ClientReferenceInformation.Code;
                paymentGatewayResponse.TransactionId = reply.Id;

                //100 is success response code
                if (string.Equals(reply.Status, "Voided", StringComparison.InvariantCultureIgnoreCase) || string.Equals(reply.Status, "Reversed", StringComparison.InvariantCultureIgnoreCase))
                {
                    paymentGatewayResponse.ResponseCode = CyberSourceResponseSuccess;
                    paymentGatewayResponse.GatewayResponseData = this.GetReasoncodeDescription(paymentGatewayResponse.ResponseCode);
                    paymentGatewayResponse.IsSuccess = true;
                }
            }
            catch (Exception ex)
            {
                paymentGatewayResponse.GatewayResponseData = this.GetReasoncodeDescription(string.Empty);
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(null, ex.Message);
            }

            return paymentGatewayResponse;
        }

        public PtsV2PaymentsVoidsPost201Response VoidTransaction(PaymentModel paymentModel)
        {
            var id = paymentModel.CustomerProfileId;

            string clientReferenceInformationCode = "Void_" + paymentModel.OrderNumber;
            Ptsv2paymentsidreversalsClientReferenceInformation clientReferenceInformation = new Ptsv2paymentsidreversalsClientReferenceInformation(
                Code: clientReferenceInformationCode
           );

            var requestObj = new VoidPaymentRequest(
                ClientReferenceInformation: clientReferenceInformation
           );

            try
            {
                CyberSource.Client.Configuration clientConfig = GetConfigurationDetails(paymentModel.PaymentCode);
                var apiInstance = new VoidApi(clientConfig);
                PtsV2PaymentsVoidsPost201Response result = apiInstance.VoidPayment(requestObj, id);
                return result;
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(null, ex.Message);
                return null;
            }
        }

        /// <summary>
        /// This method will call the Subscription method of Cybersource payment provider
        /// </summary>
        /// <param name="paymentModel">payment model</param>
        /// <returns>returns the void response</returns>
        public GatewayResponseModel Subscription(PaymentModel paymentModel)
        {
            GatewayResponseModel paymentGatewayResponse = new GatewayResponseModel();
            try
            {
                //Create Recurring Subscription object and map parameters
                RecurringSubscriptionInfo recurringSubscriptionInfo = new RecurringSubscriptionInfo();
                recurringSubscriptionInfo.amount = paymentModel.Subscription.InitialAmount.ToString();
                recurringSubscriptionInfo.numberOfPayments = paymentModel.Subscription.TotalCycles.ToString();
                recurringSubscriptionInfo.numberOfPaymentsToAdd = paymentModel.Subscription.Frequency.ToString();
                recurringSubscriptionInfo.subscriptionID = paymentModel.CustomerProfileId;
                recurringSubscriptionInfo.startDate = DateTime.Now.ToString("yyyyMMdd");
                recurringSubscriptionInfo.frequency = "on-demand";

                switch (paymentModel.Subscription.Period)
                {
                    case "WEEK":
                        recurringSubscriptionInfo.frequency = "weekly";
                        recurringSubscriptionInfo.endDate = DateTime.Now.AddDays((Convert.ToInt32(paymentModel.Subscription.Frequency) * 7 * paymentModel.Subscription.TotalCycles)).ToString("yyyyMMdd");
                        break;
                    case "MONTH":
                        recurringSubscriptionInfo.frequency = "monthly";
                        recurringSubscriptionInfo.endDate = DateTime.Now.AddMonths(Convert.ToInt32(paymentModel.Subscription.Frequency) * paymentModel.Subscription.TotalCycles).ToString("yyyyMMdd");
                        break;
                    case "YEAR":
                        recurringSubscriptionInfo.frequency = "annually";
                        recurringSubscriptionInfo.endDate = DateTime.Now.AddYears(Convert.ToInt32(paymentModel.Subscription.Frequency) * paymentModel.Subscription.TotalCycles).ToString("yyyyMMdd");
                        break;
                }

                PurchaseTotals purchaseTotals = new PurchaseTotals();
                purchaseTotals.currency = string.IsNullOrEmpty(paymentModel.GatewayCurrencyCode) ? Convert.ToString(ConfigurationManager.AppSettings["CurrencyCode"]) : paymentModel.GatewayCurrencyCode;
                purchaseTotals.grandTotalAmount = paymentModel.Subscription.InitialAmount.ToString();

                // Create request object and map parameters
                RequestMessage request = new RequestMessage();
                request.card = MapToCardObject(paymentModel);
                request.purchaseTotals = purchaseTotals;
                request.recurringSubscriptionInfo = recurringSubscriptionInfo;
                request.merchantReferenceCode = Guid.NewGuid().ToString();

                //Submit RecurringSubscription request
                ReplyMessage reply = SoapClient.RunTransaction(request);

                paymentGatewayResponse.ResponseCode = reply.reasonCode;
                paymentGatewayResponse.GatewayResponseData = this.GetReasoncodeDescription(reply.reasonCode);

                //100 is success response code
                if (reply.reasonCode.Equals(CyberSourceResponseSuccess))
                {
                    paymentGatewayResponse.IsSuccess = true;
                    paymentGatewayResponse.TransactionId = reply.requestID;
                    paymentGatewayResponse.CardAuthCode = reply.requestToken;
                }
            }
            catch (Exception ex)
            {
                paymentGatewayResponse.GatewayResponseData = this.GetReasoncodeDescription(string.Empty);
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(null, ex.Message);
            }

            return paymentGatewayResponse;
        }

        public TransactionDetailsModel GetTransactionDetails(PaymentModel paymentModel)
        {
            return new TransactionDetailsModel();
        }

        #region private methods
        /// <summary>
        /// Create payment transaction based on created customer token.
        /// </summary>
        /// <param name="paymentModel"></param>
        /// <returns></returns>
        private GatewayResponseModel AuthorizeTransaction(PaymentModel paymentModel, GatewayResponseModel paymentGatewayResponse)
        {

            try
            {
                PtsV2PaymentsPost201Response reply = Authorize(paymentModel);
                //Map response
                //TO DO: Need to confirm customer profile id is not binded in saved card scenario.
                //   if (!string.IsNullOrEmpty(paymentModel.CyberSourceToken))
                //    {
                paymentGatewayResponse.ResponseCode = reply.ProcessorInformation.ResponseCode;
                paymentGatewayResponse.GatewayResponseData = this.GetReasoncodeDescription(reply.ProcessorInformation.ResponseCode);
                paymentGatewayResponse.CustomerProfileId = reply.Id;
                paymentModel.CustomerProfileId = paymentGatewayResponse.CustomerProfileId;
                //paymentGatewayResponse.CustomerPaymentProfileId = reply.TokenInformation.PaymentInstrument.Id;
                paymentGatewayResponse.Token = paymentModel.CyberSourceToken;
                //    }

                if (!string.IsNullOrEmpty(paymentModel.CyberSourceToken))
                {
                    paymentModel.InstrumentIdentifierId = reply.TokenInformation.InstrumentIdentifier.Id.ToString();
                    paymentModel.CustomerId = reply.TokenInformation.Customer.Id.ToString();
                    paymentGatewayResponse.CustomerPaymentProfileId = reply.TokenInformation.PaymentInstrument.Id.ToString();
                    paymentModel.CustomerPaymentProfileId = paymentGatewayResponse.CustomerPaymentProfileId;

                    paymentModel.CardNumber = reply.TokenInformation.InstrumentIdentifier.Id.Substring(reply.TokenInformation.InstrumentIdentifier.Id.Length - 4, 4); ;
                }
                paymentGatewayResponse.Token = paymentModel.CustomerProfileId;
                //TssV2TransactionsGet200Response response = GetTransactionDetail(paymentModel);
                //paymentModel.CardExpirationMonth = response.PaymentInformation.Card.ExpirationMonth;
                //paymentModel.CardExpirationYear = response.PaymentInformation.Card.ExpirationYear;



                //100 is success response code
                if (reply.ProcessorInformation.ResponseCode.Equals(CyberSourceResponseSuccess))
                {
                    paymentGatewayResponse.IsSuccess = true;
                    paymentGatewayResponse.TransactionId = reply.ReconciliationId;
                    paymentGatewayResponse.IsGatewayPreAuthorize = paymentModel.GatewayPreAuthorize;
                    paymentGatewayResponse.PaymentStatus = ZnodePaymentStatus.AUTHORIZED;
                    paymentGatewayResponse.PaymentGUID = paymentModel.PaymentGUID;
                }
            }
            catch (Exception ex)
            {
                paymentGatewayResponse.GatewayResponseData = this.GetReasoncodeDescription(string.Empty);
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(null, ex.Message);
            }
            return paymentGatewayResponse;
        }

        /// <summary>
        /// This method will create customer profile 
        /// </summary>
        /// <param name="paymentModel">PaymentModel paymentModel</param>
        /// <returns>returns customer details response</returns>
        private GatewayResponseModel CreateCustomer(PaymentModel paymentModel, GatewayResponseModel response)
        {
            GatewayConnector gatewayConnector = new GatewayConnector();

            bool isSuccess = false;

            if (!paymentModel.IsAnonymousUser)
            {
                if (string.IsNullOrEmpty(paymentModel.CustomerGUID))
                {
                    if (paymentModel.IsSaveCreditCard)
                    {
                        response = CreateCustomerPayment(paymentModel, response);
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
                        return CreateCustomerPayment(paymentModel, response);
                }
                else
                {
                    if (paymentModel.IsSaveCreditCard)
                    {

                        response = CreatePaymentGatewayVault(paymentModel, response);
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
                        return CreateCustomerPayment(paymentModel, response);
                }
            }
            else
                return CreateCustomerPayment(paymentModel, response);
        }


        public static TmsV2CustomersResponse CreateCustomerCybersource(PaymentModel paymentModel)
        {
            string buyerInformationMerchantCustomerID = "Your customer identifier";
            string buyerInformationEmail = paymentModel.BillingEmailId;
            Tmsv2customersBuyerInformation buyerInformation = new Tmsv2customersBuyerInformation(
                MerchantCustomerID: buyerInformationMerchantCustomerID,
                Email: buyerInformationEmail
           );

            string clientReferenceInformationCode = "test_customer";
            Tmsv2customersClientReferenceInformation clientReferenceInformation = new Tmsv2customersClientReferenceInformation(
                Code: clientReferenceInformationCode
           );


            List<Tmsv2customersMerchantDefinedInformation> merchantDefinedInformation = new List<Tmsv2customersMerchantDefinedInformation>();
            string merchantDefinedInformationName1 = "data1";
            string merchantDefinedInformationValue1 = "Your customer data";
            merchantDefinedInformation.Add(new Tmsv2customersMerchantDefinedInformation(
                Name: merchantDefinedInformationName1,
                Value: merchantDefinedInformationValue1
           ));

            var requestObj = new PostCustomerRequest(
                BuyerInformation: buyerInformation,
                ClientReferenceInformation: clientReferenceInformation,
                MerchantDefinedInformation: merchantDefinedInformation
           );

            try
            {
                var configDictionary = new Configuration().GetConfiguration(paymentModel.PaymentCode);
                var clientConfig = new CyberSource.Client.Configuration(merchConfigDictObj: configDictionary);

                var apiInstance = new CustomerApi(clientConfig);
                TmsV2CustomersResponse result = apiInstance.PostCustomer(requestObj);
                return result;
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(null, ex.Message);
                return null;
            }
        }
        /// <summary>
        /// To vault using existing customer id
        /// </summary>
        /// <param name="paymentModel"></param>
        /// <returns>returns gateway response</returns>
        private GatewayResponseModel CreatePaymentGatewayVault(PaymentModel paymentModel, GatewayResponseModel paymentGatewayResponse)
        {
            GatewayResponseModel response = new GatewayResponseModel();
            PaymentMethodsService repository = new PaymentMethodsService();
            ZnodePaymentMethod payment = repository.GetPaymentMethod(paymentModel.PaymentApplicationSettingId, paymentModel.CustomerGUID);
            if (!Equals(payment, null) && !string.IsNullOrEmpty(payment.CustomerProfileId))
            {
                paymentModel.CustomerProfileId = payment.CustomerProfileId;
                response = CreateCustomerPayment(paymentModel, response);
            }
            else
                response = CreateCustomerPayment(paymentModel, response);

            return Equals(response, null) ? new GatewayResponseModel() : paymentGatewayResponse;
        }

        /// <summary>
        /// Create the Customer Payment
        /// </summary>
        /// <param name="paymentModel">PaymentModel paymentModel</param>
        /// <returns>returns customer payment response</returns>
        private GatewayResponseModel CreateCustomerPayment(PaymentModel paymentModel, GatewayResponseModel paymentGatewayResponse)
        {
            try
            {
                TmsV2CustomersResponse reply = CreateCustomerCybersource(paymentModel);
                
            }
            catch (Exception ex)
            {
                paymentGatewayResponse.GatewayResponseData = this.GetReasoncodeDescription(string.Empty);
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(null, ex.Message);
            }

            return paymentGatewayResponse;
        }

        /// <summary>
        /// This method will call the capture method of Cybersource payment provider
        /// </summary>
        /// <param name="paymentModel"></param>
        /// <returns></returns>
        private GatewayResponseModel CaptureTransaction(PaymentModel paymentModel, GatewayResponseModel paymentGatewayResponse)
        {
            try
            {
                PtsV2PaymentsCapturesPost201Response reply = Capture(paymentModel);
                //Map response
                paymentGatewayResponse.ResponseCode = CyberSourceResponseSuccess;
                paymentGatewayResponse.GatewayResponseData = this.GetReasoncodeDescription(paymentGatewayResponse.ResponseCode);
                paymentGatewayResponse.TransactionId = reply.ReconciliationId;
                paymentGatewayResponse.CardAuthCode = paymentModel.CyberSourceToken;

                //100 is success response code
                if (paymentGatewayResponse.ResponseCode.Equals(CyberSourceResponseSuccess))
                {
                    paymentGatewayResponse.IsSuccess = true;
                    paymentGatewayResponse.PaymentStatus = ZnodePaymentStatus.CAPTURED;
                }
            }
            catch (Exception ex)
            {
                paymentGatewayResponse.GatewayResponseData = this.GetReasoncodeDescription(string.Empty);
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(null, ex.Message);
            }
            return paymentGatewayResponse;
        }

        /// <summary>
        /// Get the reason code description
        /// </summary>
        /// <param name="reasonCode">Reason code</param>
        /// <returns>Returns the reason description</returns>
        private string GetReasoncodeDescription(string reasonCode)
        {
            switch (reasonCode)
            {
                case "100":
                    return "Successful transaction.";
                case "203":
                    return "General decline of the card.";
                case "204":
                    return "Insufficient funds in the account.";
                case "208":
                    return "Inactive card or card not authorized for card-not-present transactions.";
                case "210":
                    return "The card has reached the credit limit.";
                case "211":
                    return "Invalid card verification number.";
                case "232":
                    return "The card type is not accepted by the payment processor.";
                case "234":
                    return "There is a problem with your CyberSource merchant configuration.";
                case "235":
                    return "The requested amount exceeds the originally authorized amount. Occurs, for example, if you try to capture an amount larger than the original authorization amount.";
                case "237":
                    return "The authorization has already been reversed.";
                case "238":
                    return "The authorization has already been captured.";
                case "239":
                    return "The requested transaction amount must match the previous transaction amount.";
                case "240":
                    return "The card type sent is invalid or does not correlate with the credit card number.";
                case "241":
                    return "The request ID is invalid.";
                case "243":
                    return "The transaction has already been settled or reversed.";
                case "247":
                    return "You requested a credit for a capture that was previously voided.";
                default:
                    break;
            }

            return "Unable to process, please contact customer support.";
        }

        /// <summary>
        /// This method will return the Credit Card Type number
        /// </summary>
        /// <param name="cardType"></param>
        /// <returns>returns the Credit Card Type number</returns>
        private string GetCardType(string cardType)
        {
            string ccType = "000";
            switch (cardType.ToLower())
            {
                case "amex":
                case "american express":
                    ccType = "003";//"Amex";
                    break;
                case "visa":
                    ccType = "001";//"Visa";
                    break;
                case "mastercard":
                case "master card":
                    ccType = "002";//"MasterCard";
                    break;
                case "discover":
                    ccType = "004";//"Discover";
                    break;
            }
            return ccType;
        }

        //Map payment model to BillTo object
        private CyberSource.Clients.SoapServiceReference.BillTo MapToBillToObject(PaymentModel paymentModel)
           => new CyberSource.Clients.SoapServiceReference.BillTo
           {
               firstName = paymentModel.BillingFirstName,
               lastName = paymentModel.BillingLastName,
               email = paymentModel.BillingEmailId,
               street1 = paymentModel.BillingStreetAddress1,
               street2 = paymentModel.BillingStreetAddress2,
               city = paymentModel.BillingCity,
               state = paymentModel.BillingStateCode,
               postalCode = paymentModel.BillingPostalCode,
               country = paymentModel.BillingCountryCode
           };

        //Map payment model to Card object
        private Card MapToCardObject(PaymentModel paymentModel)
            => new Card
            {
                accountNumber = paymentModel.CardNumber,
                expirationMonth = paymentModel.CardExpirationMonth,
                expirationYear = paymentModel.CardExpirationYear,
                cvNumber = paymentModel.CardSecurityCode,
                cardType = GetCardType(paymentModel.CardType)
            };

        public PaymentGatewayTokenModel TokenGenerator(PaymentGatewayTokenModel paymentGatewayTokenModel)
        {
            PaymentGatewayTokenModel gatewaytoken = new PaymentGatewayTokenModel();
            /**
             * Generating Capture Context Request Payload
             * Defining Encryption Type = RsaOaep
             * Defining TargetOrigin = https://localhost:44315
             * 
             */
            var domainName = HttpContext.Current.Request.Headers["Znode-DomainName"];

            var requestObj = new GeneratePublicKeyRequest("RsaOaep256", "https://" + domainName);

            try
            {
                Dictionary<string, string> configDictionary = new Configuration().GetConfiguration(paymentGatewayTokenModel.PaymentCode);
                var clientConfig = new CyberSource.Client.Configuration(merchConfigDictObj: configDictionary);
                var apiInstance = new KeyGenerationApi(clientConfig);

                /**
                 * Initiating public Key request 
                 * query paramiter set to format=JWT for Flex 11
                 */
                var result = apiInstance.GeneratePublicKey("JWT", requestObj);
                Console.WriteLine(result);
                var key = result.KeyId;
                gatewaytoken.PaymentGatewayToken = key;
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(null, ex.Message);
            }
            return gatewaytoken;
            //   return Request.CreateResponse(HttpStatusCode.OK, new StringResponse { Response = key });
        }
        #endregion
    }
    public class Configuration
    {
        // initialize dictionary object
        private readonly Dictionary<string, string> _configurationDictionary = new Dictionary<string, string>();

        public Dictionary<string, string> GetConfiguration(string paymentCode)
        {
            PaymentSettingCredentialsService repository = new PaymentSettingCredentialsService();
            PaymentSettingCredentialsModel znodePaymentSetting = repository.GetPaymentSettingCredentials(paymentCode);

            _configurationDictionary.Add("authenticationType", "HTTP_SIGNATURE");
            _configurationDictionary.Add("merchantID", znodePaymentSetting.GatewayUsername);
            _configurationDictionary.Add("merchantsecretKey", znodePaymentSetting.TransactionKey);
            _configurationDictionary.Add("merchantKeyId", znodePaymentSetting.GatewayPassword);
                       
            _configurationDictionary.Add("runEnvironment", "apitest.cybersource.com");
            _configurationDictionary.Add("keyAlias", znodePaymentSetting.GatewayUsername);
            _configurationDictionary.Add("keyPass", znodePaymentSetting.GatewayUsername);
            _configurationDictionary.Add("timeout", "300000");

            // Configs related to meta key
            _configurationDictionary.Add("portfolioID", string.Empty);
            _configurationDictionary.Add("useMetaKey", "false");

            // Configs related to OAuth
            _configurationDictionary.Add("enableClientCert", "false");
            _configurationDictionary.Add("clientCertDirectory", "Resource");
            _configurationDictionary.Add("clientCertFile", "");
            _configurationDictionary.Add("clientCertPassword", "");
            _configurationDictionary.Add("clientId", "");
            _configurationDictionary.Add("clientSecret", "");

            return _configurationDictionary;
        }
    }
}
