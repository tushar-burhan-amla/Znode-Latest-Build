using System;
using System.Diagnostics;
using System.Linq;
using Znode.Multifront.PaymentApplication.Data;
using Znode.Multifront.PaymentApplication.Helpers;
using Znode.Multifront.PaymentApplication.Models;
using Znode.Multifront.PaymentApplication.Providers.AuthorizeNetAPI;
using Newtonsoft.Json;
using System.Net;
using System.Configuration;
using hostedIframe  = AuthorizeNet.Api.Contracts.V1;
using AuthorizeNet.Api.Controllers;
using AuthorizeNet.Api.Controllers.Bases;
using System.Web;
using System.Collections.Generic;

namespace Znode.Multifront.PaymentApplication.Providers
{
    /// <summary>
    /// This class will have all the methods of implementation of Authorize .Net payment provider 
    /// </summary>
   public class AuthorizeNetCustomerProvider : BaseProvider, IPaymentProviders
    {
        public const string AuthorizeNetResponseSuccess = "I00001";
        #region Public Methods
        /// <summary>
        /// Validate the credit card transaction number and returns status
        /// </summary>
        /// <param name="paymentModel"></param>
        /// <returns></returns>
        public GatewayResponseModel ValidateCreditcard(PaymentModel paymentModel)
        {
            GatewayResponseModel response = new GatewayResponseModel();
            try
            {
                //If GatewayPreAuthorize is false then call Create Capture Transaction to complete the payment process.
                if (!paymentModel.GatewayPreAuthorize || paymentModel.IsCapture)
                    return CreateCaptureTransaction(paymentModel);

                //To Create Customer Profile based on user input.
                if (string.IsNullOrEmpty(paymentModel.CustomerProfileId))
                {
                    response = CreateCustomer(paymentModel);

                }
                else
                {

                    response.IsSuccess = true;
                }
                response.TransactionId = paymentModel.TransactionId;
                response.IsGatewayPreAuthorize = paymentModel.GatewayPreAuthorize;
                
                return response;
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(paymentModel.PaymentApplicationSettingId, ex.Message);

                response.IsSuccess = false;
                response.GatewayResponseData = ex.Message;
            }
            return response;
        }

        /// <summary>
        /// get the customer profile
        /// </summary>
        /// <returns>The result of the operation</returns>
        private GatewayResponseModel GetCustomerProfileTransaction(PaymentModel paymentModel)
        {
            GatewayResponseModel gatewayResponse = new GatewayResponseModel();
            GetAuthNetRequest(paymentModel.GatewayLoginName, paymentModel.GatewayTransactionKey);

            var request = new hostedIframe.getTransactionDetailsRequest();
            request.transId = paymentModel.TransactionId;

            // instantiate the controller that will call the service
            var controller = new getTransactionDetailsController(request);
            controller.Execute();

            // get the response from the service (errors contained if any)
            var response = controller.GetApiResponse();
            
            if (!Equals(response, null))
            {
                gatewayResponse.IsSuccess = true;
                var paymentProfile = JsonConvert.SerializeObject(response.transaction.payment.Item);
                PaymentProfileModel paymentProfileModel = JsonConvert.DeserializeObject<PaymentProfileModel>(paymentProfile);
                paymentModel.CardNumber = paymentProfileModel.cardNumber.Substring(paymentProfileModel.cardNumber.Length - 4);
            }
            else
            {
                gatewayResponse.IsSuccess = false;
                gatewayResponse.ResponseText = "Unable to create customer profile.";
            }
            return gatewayResponse;
        }

        /// <summary>
        /// Create the customer profile
        /// </summary>
        /// <returns>The result of the operation</returns>
        private GatewayResponseModel CreateAuthCustomerProfile(PaymentModel paymentModel, GatewayResponseModel gatewayResponseModel)
        {
            GetAuthNetRequest(paymentModel.GatewayLoginName, paymentModel.GatewayTransactionKey);

            var customerProfile = new hostedIframe.customerProfileBaseType
            {
                merchantCustomerId = paymentModel.CustomerGUID,
                email = (string.IsNullOrEmpty(paymentModel.Email) ? paymentModel.BillingEmailId : paymentModel.Email)
            };

            var request = new hostedIframe.createCustomerProfileFromTransactionRequest
            {
                transId = paymentModel.TransactionId,
                customer = customerProfile
            };

            var controller = new createCustomerProfileFromTransactionController(request);
            controller.Execute();

            hostedIframe.createCustomerProfileResponse response = controller.GetApiResponse();

            // validate response
            if (response != null && response.messages.resultCode == hostedIframe.messageTypeEnum.Ok)
            {
                if (response != null && response.messages.message != null)
                {
                    gatewayResponseModel.IsSuccess = true;
                    gatewayResponseModel.CustomerProfileId = response.customerProfileId;
                    gatewayResponseModel.CustomerPaymentProfileId = response.customerPaymentProfileIdList.Length > 0 ? response.customerPaymentProfileIdList[0] : null;
                    gatewayResponseModel.CustomerShippingAddressId = response.customerShippingAddressIdList.Length > 0 ? response.customerShippingAddressIdList[0] : null;

                    paymentModel.CustomerProfileId = gatewayResponseModel.CustomerProfileId;
                    paymentModel.CustomerPaymentProfileId = gatewayResponseModel.CustomerPaymentProfileId;
                    paymentModel.CustomerShippingAddressId = gatewayResponseModel.CustomerShippingAddressId;
                }
            }
            else if (response != null)
            {
                Logging.LogMessage("Error Log: " + $"code: {response.messages.message[0].code}, msg: {response.messages.message[0].text}", Logging.Components.Payment.ToString(), TraceLevel.Error);
                gatewayResponseModel.IsSuccess = false;
            }
            
            return gatewayResponseModel;
        }

        private GatewayResponseModel CreateCustomerProfileWithoutTransactionId(PaymentModel paymentModel, GatewayResponseModel gatewayResponseModel)
        {
            
                GetAuthNetRequest(paymentModel.GatewayLoginName, paymentModel.GatewayTransactionKey);

                var customerProfile = new hostedIframe.customerProfileBaseType
                {
                    merchantCustomerId = paymentModel.CustomerGUID,
                    email = (string.IsNullOrEmpty(paymentModel.Email) ? paymentModel.BillingEmailId : paymentModel.Email)
                };

                List<customerPaymentProfileType> paymentProfileList = new List<customerPaymentProfileType>();

                var customerProfile1 = new hostedIframe.customerProfileType();
            customerProfile1.merchantCustomerId = paymentModel.CustomerGUID;
            customerProfile1.email = (string.IsNullOrEmpty(paymentModel.Email) ? paymentModel.BillingEmailId : paymentModel.Email);

                var request = new hostedIframe.createCustomerProfileRequest { profile = customerProfile1};

                // instantiate the controller that will call the service
                var controller = new createCustomerProfileController(request);
                controller.Execute();

                // get the response from the service (errors contained if any)
                hostedIframe.createCustomerProfileResponse response = controller.GetApiResponse();

                // validate response 
                    if (response != null && response.messages.resultCode == hostedIframe.messageTypeEnum.Ok)
                    {
                        if (response.messages.message != null)
                        {
                            Console.WriteLine("Success!");
                            Console.WriteLine("Customer Profile ID: " + response.customerProfileId);

                            gatewayResponseModel.IsSuccess = true;
                            gatewayResponseModel.CustomerProfileId = response.customerProfileId;

                            paymentModel.CustomerProfileId = gatewayResponseModel.CustomerProfileId;
                        }
                    }
                    else
                    {
                        Console.WriteLine("Customer Profile Creation Failed.");
                        Console.WriteLine("Error Code: " + response.messages.message[0].code);
                        Console.WriteLine("Error message: " + response.messages.message[0].text);

                        Logging.LogMessage("Error Log: " + $"code: {response.messages.message[0].code}, msg: {response.messages.message[0].text}", Logging.Components.Payment.ToString(), TraceLevel.Error);
                        gatewayResponseModel.IsSuccess = false;
                        if (response?.messages?.message[0].code == "E00039")
                        {
                            string customerProfileId = response?.messages?.message[0].text;
                            customerProfileId = (customerProfileId?.Split(new string[] { "ID" }, StringSplitOptions.None)[1])?.Split(new string[] { "already" }, StringSplitOptions.None)[0].Trim();
                            paymentModel.CustomerProfileId = customerProfileId;
                            gatewayResponseModel.CustomerProfileId = customerProfileId;
                            gatewayResponseModel.IsSuccess = true;
                        }
                    }

                return gatewayResponseModel;
            
        }
        /// <summary>
        /// Create the customer profile
        /// </summary>
        /// <returns>The result of the operation</returns>
        private GatewayResponseModel CreateCustomerProfile(PaymentModel paymentModel)
        {
            GatewayResponseModel gatewayResponse = new GatewayResponseModel();
            createCustomerProfileRequest request = new createCustomerProfileRequest();
            XmlApiUtilities.PopulateMerchantAuthentication((ANetApiRequest)request, paymentModel.GatewayLoginName, paymentModel.GatewayTransactionKey);

            customerProfileType customer = new customerProfileType();
            customer.email = paymentModel.BillingEmailId;
            customer.description = $"New customer {DateTime.Now.Ticks.ToString()}";

            if (!string.IsNullOrEmpty(paymentModel.ShippingFirstName))
            {
                customer.shipToList = new customerAddressType[1];
                customer.shipToList[0] = new customerAddressType
                {
                    firstName = paymentModel.ShippingFirstName,
                    lastName = paymentModel.ShippingLastName,
                    zip = paymentModel.ShippingPostalCode,
                    country = paymentModel.ShippingCountryCode,
                    address = paymentModel.ShippingStreetAddress1,
                    city = paymentModel.ShippingCity,
                    state = paymentModel.ShippingStateCode,
                    company = paymentModel.CompanyName
                };
            }

            request.profile = customer;
            request.validationMode = paymentModel.GatewayTestMode ? validationModeEnum.testMode : validationModeEnum.liveMode;

            System.Xml.XmlDocument responseXml = null;
            bool bResult = XmlApiUtilities.PostRequest(request, out responseXml, paymentModel.GatewayTestMode);
            object response = null;
            createCustomerProfileResponse apiResponse = null;

            if (bResult)
                bResult = XmlApiUtilities.ProcessXmlResponse(responseXml, out response);
            if (!(response is createCustomerProfileResponse))
            {
                var errorResponse = (ANetApiResponse)response;
                gatewayResponse.IsSuccess = false;
                if (!Equals(errorResponse, null))
                {
                    gatewayResponse.GatewayResponseData = $"code: {errorResponse.messages.message[0].code}\n	  msg: {errorResponse.messages.message[0].text}";
                    gatewayResponse.ResponseText = errorResponse.messages.message[0].text;
                }
                return gatewayResponse;
            }

            if (bResult)
                apiResponse = (createCustomerProfileResponse)response;

            if (!Equals(apiResponse?.customerProfileId, null))
            {
                paymentModel.CustomerProfileId = apiResponse.customerProfileId;
                paymentModel.CustomerShippingAddressId = apiResponse.customerShippingAddressIdList.FirstOrDefault();
                gatewayResponse.CustomerShippingAddressId = apiResponse.customerShippingAddressIdList.FirstOrDefault();
                gatewayResponse.CustomerProfileId = apiResponse.customerProfileId;
                gatewayResponse.IsSuccess = true;
            }
            else
            {
                gatewayResponse.IsSuccess = false;
                gatewayResponse.ResponseText = "Unable to create customer profile.";
            }

            return gatewayResponse;
        }

        /// <summary>
        /// This method will create the customer payment profile
        /// </summary>
        /// <param name="paymentModel">payment model</param>
        /// <returns></returns>
        private GatewayResponseModel CreateCustomerPaymentProfile(PaymentModel paymentModel)
        {
            GatewayResponseModel gatewayResponse = new GatewayResponseModel();
            customerPaymentProfileType newPaymentProfile = new customerPaymentProfileType();
            paymentType newPayment = new paymentType();

            creditCardType newCard = new creditCardType();
            newCard.cardNumber = paymentModel.CardNumber;
            newCard.expirationDate = paymentModel.CardExpiration;
            newCard.cardCode = paymentModel.CardSecurityCode;
            newPayment.Item = newCard;

            newPaymentProfile.payment = newPayment;
            //Save billing address for user.
            newPaymentProfile.billTo = new customerAddressType() { firstName = paymentModel.BillingFirstName, lastName = paymentModel.BillingLastName, address = $"{paymentModel.BillingStreetAddress1}{paymentModel.BillingStreetAddress2}", zip = paymentModel.BillingPostalCode, city = paymentModel.BillingCity, country = paymentModel.BillingCountryCode, phoneNumber = paymentModel.BillingPhoneNumber, state = paymentModel.BillingStateCode, company=paymentModel.CompanyName };
            createCustomerPaymentProfileRequest request = new createCustomerPaymentProfileRequest();
            XmlApiUtilities.PopulateMerchantAuthentication((ANetApiRequest)request, paymentModel.GatewayLoginName, paymentModel.GatewayTransactionKey);

            request.customerProfileId = paymentModel.CustomerProfileId;
            request.paymentProfile = newPaymentProfile;
            request.validationMode = paymentModel.GatewayTestMode ? validationModeEnum.testMode : validationModeEnum.liveMode;
            request.validationModeSpecified = true;

            System.Xml.XmlDocument responseXml = null;
            bool bResult = XmlApiUtilities.PostRequest(request, out responseXml, paymentModel.GatewayTestMode);
            object response = null;
            createCustomerPaymentProfileResponse apiResponse = null;

            if (bResult)
                bResult = XmlApiUtilities.ProcessXmlResponse(responseXml, out response);
            if (!(response is createCustomerPaymentProfileResponse))
            {
                var errorResponse = (ANetApiResponse)response;
                gatewayResponse.GatewayResponseData =
                    $"code: {errorResponse.messages.message[0].code}\n	  msg: {errorResponse.messages.message[0].text}";
                gatewayResponse.IsSuccess = false;
                gatewayResponse.ResponseText = errorResponse.messages.message[0].text;
                return gatewayResponse;
            }
            if (bResult)
                apiResponse = (createCustomerPaymentProfileResponse)response;
            
            if (apiResponse != null && apiResponse.messages.resultCode.Equals(messageTypeEnum.Ok))
            {
                gatewayResponse.IsSuccess = true;
                gatewayResponse.CustomerProfileId = paymentModel.CustomerProfileId;                
                gatewayResponse.CustomerShippingAddressId = paymentModel.CustomerShippingAddressId;
                gatewayResponse.CustomerPaymentProfileId = apiResponse.customerPaymentProfileId;
                gatewayResponse.ResponseText = apiResponse.messages.message.FirstOrDefault().text;
            }
            else
            {
                Logging.LogMessage("Error Log: " + $"code: {apiResponse?.messages.message[0].code}, msg: {apiResponse?.messages.message[0].text}", Logging.Components.Payment.ToString(), TraceLevel.Error);

                gatewayResponse.IsSuccess = false;
                gatewayResponse.GatewayResponseData = $"code: {apiResponse?.messages.message[0].code}, msg: {apiResponse?.messages.message[0].text}";
            }
            return gatewayResponse;
        }

        /// <summary>
        /// Create a transaction(Authorize or capture transaction)
        /// </summary>
        /// <param name="paymentModel">payment model</param>
        /// <returns>returns the create transaction response.</returns>
        private GatewayResponseModel CreateTransaction(PaymentModel paymentModel)
        {
            GatewayResponseModel gatewayResponse = new GatewayResponseModel();
            createCustomerProfileTransactionRequest request = new createCustomerProfileTransactionRequest();
            XmlApiUtilities.PopulateMerchantAuthentication((ANetApiRequest)request, paymentModel.GatewayLoginName, paymentModel.GatewayTransactionKey);

            profileTransactionType newTrans = new profileTransactionType();

            if (paymentModel.IsCapture)
            {
                profileTransPriorAuthCaptureType newItem = new profileTransPriorAuthCaptureType
                {
                    transId = paymentModel.TransactionId,
                    customerProfileId = paymentModel.CustomerProfileId,
                    customerPaymentProfileId = paymentModel.CustomerPaymentProfileId,
                    customerShippingAddressId = paymentModel.CustomerShippingAddressId,
                    amount = decimal.Parse(paymentModel.Total)
                };
                newTrans.Item = newItem;
            }
            else
            {
                profileTransAuthOnlyType newItem = new profileTransAuthOnlyType
                {
                    customerProfileId = paymentModel.CustomerProfileId,
                    customerPaymentProfileId = paymentModel.CustomerPaymentProfileId,
                    customerShippingAddressId = paymentModel.CustomerShippingAddressId,
                    amount = decimal.Parse(paymentModel.Total),
                    cardCode = paymentModel.CardSecurityCode,
                    order = new orderExType
                    {
                        purchaseOrderNumber = paymentModel.OrderId,
                        invoiceNumber = paymentModel.OrderId,
                    }
                };
                newTrans.Item = newItem;
            }

            request.transaction = newTrans;
          
            System.Xml.XmlDocument response_xml = null;
            bool bResult = XmlApiUtilities.PostRequest(request, out response_xml, paymentModel.GatewayTestMode);
            object response = null;
            createCustomerProfileTransactionResponse api_response = null;

            if (bResult) bResult = XmlApiUtilities.ProcessXmlResponse(response_xml, out response);
            if (!(response is createCustomerProfileTransactionResponse))
            {
                var errorResponse = (ANetApiResponse)response;
                if (!Equals(errorResponse, null))
                    gatewayResponse.GatewayResponseData = $"code: { errorResponse.messages.message[0].code}, msg: {errorResponse.messages.message[0].text}";
                gatewayResponse.IsSuccess = false;
                return gatewayResponse;
            }
            if (bResult) api_response = (createCustomerProfileTransactionResponse)response;
            if (!Equals(api_response, null) && api_response.messages.resultCode.Equals(messageTypeEnum.Ok))
            {
                gatewayResponse.GatewayResponseData = $"code: {api_response.messages.message[0].code}, msg: {api_response.messages.message[0].text}";

                var responseValues = api_response.directResponse.Split(',');
                if (responseValues.Count() > 6 && !responseValues[6].Equals("0"))
                {
                    gatewayResponse.IsGatewayPreAuthorize = paymentModel.GatewayPreAuthorize;
                    gatewayResponse.TransactionId = responseValues[6];
                    gatewayResponse.IsSuccess = true;
                    gatewayResponse.CustomerProfileId = paymentModel.CustomerProfileId;
                    gatewayResponse.CustomerShippingAddressId = paymentModel.CustomerShippingAddressId;
                    gatewayResponse.CustomerPaymentProfileId = paymentModel.CustomerPaymentProfileId;
                    gatewayResponse.PaymentStatus = paymentModel.IsCapture ? ZnodePaymentStatus.CAPTURED : ZnodePaymentStatus.AUTHORIZED;
                    gatewayResponse.TransactionDate = paymentModel.TransactionDate;
                }
                else
                {
                    gatewayResponse.IsSuccess = false;
                    gatewayResponse.GatewayResponseData = $"code: {api_response.messages.message[0].code}, msg: {api_response.messages.message[0].text}";
                }
            }
            else
            {
                gatewayResponse.IsSuccess = false;
                gatewayResponse.GatewayResponseData = $"code: {api_response.messages.message[0].code}, msg: {api_response.messages.message[0].text}";
            }

            return gatewayResponse;
        }

        public hostedIframe.ANetApiResponse VoidTransaction(PaymentModel paymentModel)
        {
            GetAuthNetRequest(paymentModel.GatewayLoginName, paymentModel.GatewayTransactionKey);

            var transactionRequest = new hostedIframe.transactionRequestType
            {
                transactionType = transactionTypeEnum.voidTransaction.ToString(),    // refund type
                refTransId      = paymentModel.TransactionId
            };

            hostedIframe.createTransactionRequest request = new hostedIframe.createTransactionRequest { transactionRequest = transactionRequest };

            // instantiate the controller that will call the service
            var controller = new createTransactionController(request);
            controller.Execute();

            // get the response from the service (errors contained if any)
            var response = controller.GetApiResponse();

            return response;
        }

        /// <summary>
        /// This method will call the capture payment of Authorize .Net payment provider
        /// </summary>
        /// <param name="paymentModel">payment model</param>
        /// <returns>returns the capture response</returns>

        public hostedIframe.ANetApiResponse CaptureTransaction(PaymentModel paymentModel)
        {
            GetAuthNetRequest(paymentModel.GatewayLoginName, paymentModel.GatewayTransactionKey);

            var transactionRequest = new hostedIframe.transactionRequestType
            {
                transactionType = transactionTypeEnum.priorAuthCaptureTransaction.ToString(),    // capture prior only
                amount = Convert.ToDecimal(paymentModel.Total),
                refTransId = paymentModel.TransactionId
            };
            hostedIframe.createTransactionRequest request = new hostedIframe.createTransactionRequest { transactionRequest = transactionRequest };

            // instantiate the controller that will call the service
            var controller = new createTransactionController(request);
            controller.Execute();

            // get the response from the service (errors contained if any)
            var response = controller.GetApiResponse();

            return response;
        }

        public AuthorizeNet.Api.Contracts.V1.createTransactionResponse RefundTransaction(PaymentModel paymentModel)
        {
            object billingAddress = null;
            object shippingAddress = null;
            object orderInformationBillTo = null;
            
            GetAuthNetRequest(paymentModel.GatewayLoginName, paymentModel.GatewayTransactionKey);

            var creditCard = new hostedIframe.creditCardType
            {
                cardNumber = paymentModel.CardNumber,
                expirationDate = "XXXX"
            };

            //standard api call to retrieve response
            var paymentType = new hostedIframe.paymentType { Item = creditCard };

            var orderType = new hostedIframe.orderType
            {
                invoiceNumber = paymentModel.OrderId
            };

            if (paymentModel.GatewayCode == PaymentConstant.AuthorizeNet)
            {
                billingAddress = new hostedIframe.customerAddressType
                {
                    firstName = paymentModel.BillingFirstName,
                    lastName = paymentModel.BillingLastName,
                    state = paymentModel.BillingStateCode,
                    country = paymentModel.BillingCountryCode,
                    company = paymentModel.BillingCompanyName,
                    address = paymentModel.BillingStreetAddress1,
                    city = paymentModel.BillingCity,
                    zip = paymentModel.BillingPostalCode,
                    phoneNumber = paymentModel.BillingPhoneNumber,
                    
                };

                shippingAddress = new hostedIframe.customerAddressType
                {
                    firstName = paymentModel.ShippingFirstName,
                    lastName = paymentModel.ShippingLastName,
                    state = paymentModel.ShippingStateCode,
                    country = paymentModel.ShippingCountryCode,
                    company = paymentModel.ShippingCompanyName,
                    address = paymentModel.ShippingStreetAddress1,
                    city = paymentModel.ShippingCity,
                    zip = paymentModel.ShippingPostalCode,
                    phoneNumber = paymentModel.ShippingPhoneNumber,
                    //faxNumber = paymentModel.ShippingFaxNumber,
                };
                orderInformationBillTo = new hostedIframe.customerDataType
                {
                    email = paymentModel.BillingEmailId,
                };
            }

            var transactionRequest = new hostedIframe.transactionRequestType
            {
                transactionType = transactionTypeEnum.refundTransaction.ToString(),    // refund type
                payment = paymentType,
                amount = Convert.ToDecimal(paymentModel.Total),
                refTransId = paymentModel.TransactionId,
                order = orderType,
                billTo = (hostedIframe.customerAddressType)billingAddress,
                shipTo = (hostedIframe.customerAddressType)shippingAddress,
                customer = (hostedIframe.customerDataType)orderInformationBillTo,
            };

            hostedIframe.createTransactionRequest request = new hostedIframe.createTransactionRequest { transactionRequest = transactionRequest ,refId=paymentModel.OrderId};

            // instantiate the controller that will call the service
            var controller = new createTransactionController(request);
            controller.Execute();

            // get the response from the service (errors contained if any)
            AuthorizeNet.Api.Contracts.V1.createTransactionResponse response = controller.GetApiResponse();

            return response;
        }
        /// <summary>
        /// This method will call the refund payment of Authorize .Net payment provider
        /// </summary>
        /// <param name="paymentModel">payment model</param>
        /// <returns>returns the refund response</returns>
        public GatewayResponseModel Refund(PaymentModel paymentModel)
        {
            //get last 4 digit card number
            GetCustomerProfileTransaction(paymentModel);

            AuthorizeNet.Api.Contracts.V1.createTransactionResponse response = RefundTransaction(paymentModel);
            GatewayResponseModel gatewayResponse = new GatewayResponseModel();
            var responseCode = response.messages.message.FirstOrDefault(x => x.code == AuthorizeNetResponseSuccess); 
            
            if (!Equals(responseCode, null))
            {
                gatewayResponse.TransactionId = response.transactionResponse.transId;
                gatewayResponse.IsSuccess = true;
                gatewayResponse.PaymentStatus = ZnodePaymentStatus.REFUNDED;
            }
           
            return gatewayResponse;
        }

        /// <summary>
        /// This method will call the void payment of Authorize .Net payment provider
        /// </summary>
        /// <param name="paymentModel">payment model</param>
        /// <returns>returns the void response</returns>
        public GatewayResponseModel Void(PaymentModel paymentModel)
        {
            hostedIframe.ANetApiResponse response =  VoidTransaction(paymentModel);
            GatewayResponseModel gatewayResponse = new GatewayResponseModel();
            var responseCode = response.messages.message.FirstOrDefault(x => x.code == AuthorizeNetResponseSuccess);

            if (Equals(responseCode, null))
            {
                hostedIframe.ANetApiResponse errorResponse = (hostedIframe.ANetApiResponse)response;
                if (!Equals(errorResponse, null))
                    gatewayResponse.GatewayResponseData = $"code: {errorResponse.messages.message[0].code}, msg: {errorResponse.messages.message[0].text}";
                gatewayResponse.IsSuccess = false;
                return gatewayResponse;
            }
            if (!Equals(responseCode, null))
            {
                    gatewayResponse.TransactionId = paymentModel.TransactionId;
                    gatewayResponse.IsSuccess = true;
                    gatewayResponse.PaymentStatus = ZnodePaymentStatus.VOIDED;
            }
            return gatewayResponse;
        }

        /// <summary>
        /// This method will call the subscription
        /// </summary>
        /// <param name="paymentModel">PaymentModel model</param>
        /// <returns>Response from the payment provider</returns>
        public GatewayResponseModel Subscription(PaymentModel paymentModel)
        {
            GatewayResponseModel gatewayResponse = new GatewayResponseModel();
            ARBCreateSubscriptionRequest request = new ARBCreateSubscriptionRequest();

            XmlApiUtilities.PopulateMerchantAuthentication((ANetApiRequest)request, paymentModel.GatewayLoginName, paymentModel.GatewayTransactionKey);

            ARBSubscriptionType sub = new ARBSubscriptionType();
            sub.name = paymentModel.Subscription.ProfileName;

            sub.payment = new paymentType();
            sub.payment.Item = new creditCardType { cardNumber = paymentModel.CardNumber, expirationDate = $"{paymentModel.CardExpirationYear}-{paymentModel.CardExpirationMonth}" };

            // subscription payment schedule
            sub.paymentSchedule = new paymentScheduleType();
            sub.paymentSchedule.startDate = DateTime.Today;
            sub.paymentSchedule.startDateSpecified = true;

            // Disable trial
            sub.paymentSchedule.trialOccurrences = 0;
            sub.paymentSchedule.trialOccurrencesSpecified = true;
            sub.trialAmount = 0.00M;
            sub.trialAmountSpecified = true;

            sub.billTo = new nameAndAddressType { firstName = paymentModel.BillingFirstName, lastName = paymentModel.BillingLastName };

            if (paymentModel.Subscription.TotalCycles.Equals(0))
                sub.paymentSchedule.totalOccurrences = 9999;
            else
                sub.paymentSchedule.totalOccurrences = short.Parse(paymentModel.Subscription.TotalCycles.ToString());

            sub.paymentSchedule.totalOccurrencesSpecified = true;

            sub.amount = paymentModel.Subscription.Amount;
            sub.amountSpecified = true;

            int frequencyLength = 0;
            sub.paymentSchedule.interval = new paymentScheduleTypeInterval();
            sub.paymentSchedule.interval.unit = this.BillingPeriod(paymentModel.Subscription.Period, paymentModel.Subscription.Frequency, out frequencyLength);
            sub.paymentSchedule.interval.length = (short)frequencyLength;

            sub.order = new orderType();
            sub.order.invoiceNumber = paymentModel.Subscription.InvoiceNo;

            request.subscription = sub;
            request.refId = paymentModel.TransactionId;

            System.Xml.XmlDocument responseXml = null;
            bool bResult = XmlApiUtilities.PostRequest(request, out responseXml, paymentModel.GatewayTestMode);
            object response = null;
            ARBCreateSubscriptionResponse api_response = null;

            if (bResult)
                bResult = XmlApiUtilities.ProcessXmlResponse(responseXml, out response);

            if (bResult)
                api_response = (ARBCreateSubscriptionResponse)response;
            if (!Equals(api_response, null) && api_response.messages.resultCode.Equals(messageTypeEnum.Ok))
            {
                gatewayResponse.GatewayResponseData = $"code: {api_response.messages.message[0].code}, msg: {api_response.messages.message[0].text}";
                if (!string.IsNullOrEmpty(api_response.subscriptionId))
                {
                    gatewayResponse.TransactionId = api_response.subscriptionId;
                    gatewayResponse.IsSuccess = true;
                }
            }
            else
            {
                gatewayResponse.GatewayResponseData = $"code: {api_response.messages.message[0].code}, msg: {api_response.messages.message[0].text}";
                gatewayResponse.IsSuccess = false;
            }
            return gatewayResponse;

        }
        #endregion

        #region Private Methods
        /// <summary>
        /// Get the customer profile
        /// </summary>
        /// <param name="profile_id">The ID of the customer that we are getting the profile for</param>
        /// <returns>The profile that was returned</returns>
        private customerProfileMaskedType GetCustomerProfile(PaymentModel paymentModel)
        {
            customerProfileMaskedType out_profile = null;

            getCustomerProfileRequest request = new getCustomerProfileRequest();
            XmlApiUtilities.PopulateMerchantAuthentication((ANetApiRequest)request, paymentModel.GatewayLoginName, paymentModel.GatewayTransactionKey);

            request.customerProfileId = paymentModel.CustomerProfileId;

            System.Xml.XmlDocument response_xml = null;
            bool bResult = XmlApiUtilities.PostRequest(request, out response_xml, paymentModel.GatewayTestMode);
            object response = null;
            getCustomerProfileResponse api_response = null;

            if (bResult)
                bResult = XmlApiUtilities.ProcessXmlResponse(response_xml, out response);
            if (!(response is getCustomerProfileResponse))
            {
                ANetApiResponse ErrorResponse = (ANetApiResponse)response;
                return out_profile;
            }
            if (bResult)
                api_response = (getCustomerProfileResponse)response;
            if (!Equals(api_response, null))
                out_profile = api_response.profile;

            return out_profile;
        }

        /// <summary>
        /// This method will create customer profile 
        /// </summary>
        /// <param name="paymentModel">PaymentModel paymentModel</param>
        /// <returns>returns customer details response</returns>
        private GatewayResponseModel CreateCustomer(PaymentModel paymentModel)
        {
            GatewayResponseModel response = new GatewayResponseModel();
            GatewayConnector gatewayConnector = new GatewayConnector();
            bool isSuccess = false;
            //Customer Logged IN
            if (!paymentModel.IsAnonymousUser && paymentModel.IsSaveCreditCard)
            {
                CreateAuthCustomerProfile(paymentModel, response);
                if (response.IsSuccess)
                    isSuccess = gatewayConnector.SaveCustomerDetails(paymentModel);
                response.CustomerGUID = paymentModel.CustomerGUID;
                response.PaymentToken = paymentModel.PaymentToken;
                paymentModel.CustomerProfileId = response.CustomerProfileId;
                paymentModel.CustomerPaymentProfileId = response.CustomerPaymentProfileId;
                response.IsSuccess = isSuccess;
                return response;
            }
            else
            {
                CreateCustomerProfileWithoutTransactionId(paymentModel, response);//As it is which is the normal one without vault.
                if (response.IsSuccess)
                    isSuccess = gatewayConnector.SaveCustomerDetails(paymentModel);
                response.CustomerGUID = paymentModel.CustomerGUID;
                response.PaymentToken = paymentModel.PaymentToken;
                paymentModel.CustomerProfileId = response.CustomerProfileId;
                paymentModel.CustomerPaymentProfileId = response.CustomerPaymentProfileId;
                response.IsSuccess = isSuccess;
                return response;
            }
        }

        /// <summary>
        /// This method will return shipping addressId by customer profileid
        /// </summary>
        /// <param name="paymentModel">payment Model</param>
        /// <returns>shipping Address Id</returns>
        private string GetCustomerShippingAddressId(PaymentModel paymentModel)
        {
            createCustomerShippingAddressRequest request = GetShippingRequest(paymentModel);
            System.Xml.XmlDocument responseXml = null;
            bool bResult = XmlApiUtilities.PostRequest(request, out responseXml, paymentModel.GatewayTestMode);
            object response = null;
            createCustomerShippingAddressResponse apiResponse = new createCustomerShippingAddressResponse();

            if (bResult && XmlApiUtilities.ProcessXmlResponse(responseXml, out response))
                apiResponse = (createCustomerShippingAddressResponse)response;

            return apiResponse.customerAddressId;
        }

        /// <summary>
        /// Get ShippingAddress API request Param
        /// </summary>
        /// <param name="paymentModel">payment Model</param>
        /// <returns>request param</returns>
        private createCustomerShippingAddressRequest GetShippingRequest(PaymentModel paymentModel)
        {
            createCustomerShippingAddressRequest request = new createCustomerShippingAddressRequest();
            XmlApiUtilities.PopulateMerchantAuthentication((ANetApiRequest)request, paymentModel.GatewayLoginName, paymentModel.GatewayTransactionKey);

            customerAddressType customer = new customerAddressType()
            {
                firstName = paymentModel.ShippingFirstName,
                lastName = paymentModel.ShippingLastName,
                zip = paymentModel.ShippingPostalCode,
                country = paymentModel.ShippingCountryCode,
                address = paymentModel.ShippingStreetAddress1,
                city = paymentModel.ShippingCity,
                state = paymentModel.ShippingStateCode
            };
            request.address = customer;
            request.customerProfileId = paymentModel.CustomerProfileId;
            return request;
        }

        /// <summary>
        /// To Customer new customer account and ADD vault
        /// </summary>
        /// <param name="paymentModel"></param>
        /// <returns>returns gateway response</returns>
        private GatewayResponseModel CreatePaymentGatewayCustomer(PaymentModel paymentModel)
        {
            GatewayResponseModel response = CreateCustomerProfile(paymentModel);
            if (!string.IsNullOrEmpty(response.CustomerProfileId))
            {
                paymentModel.CustomerProfileId = response.CustomerProfileId;
                paymentModel.CustomerShippingAddressId = response.CustomerShippingAddressId;
                if (response.IsSuccess && paymentModel.IsAnonymousUser)
                {
                    GatewayConnector gatewayConnector = new GatewayConnector();
                    response.IsSuccess = gatewayConnector.SavePaymentDetails(paymentModel);
                }
            }
            return response;
        }

        /// <summary>
        /// To vault using existing customer id
        /// </summary>
        /// <param name="paymentModel"></param>
        /// <returns>returns gateway response</returns>
        private GatewayResponseModel CreatePaymentGatewayVault(PaymentModel paymentModel)
        {
            GatewayResponseModel response = new GatewayResponseModel();
            //get customer profile id from db using guid if not exit the create user
            PaymentMethodsService repository = new PaymentMethodsService();
            ZnodePaymentMethod payment = repository.GetPaymentMethod(paymentModel.PaymentApplicationSettingId, paymentModel.CustomerGUID);
            if (!Equals(payment, null) && !string.IsNullOrEmpty(payment.CustomerProfileId))
                paymentModel.CustomerProfileId = payment.CustomerProfileId;
            else
            {
                response = CreateCustomerProfile(paymentModel);
                paymentModel.CustomerProfileId = response.CustomerProfileId;
            }
            if (!string.IsNullOrEmpty(paymentModel.CustomerProfileId))
            {
                response = CreateCustomerPaymentProfile(paymentModel);
                paymentModel.CustomerPaymentProfileId = response.CustomerPaymentProfileId;
                response.IsSuccess = !Equals(response.CustomerPaymentProfileId, null);
            }
            return Equals(response, null) ? new GatewayResponseModel() : response;
        }

        /// <summary>
        /// To capture using existing customer id
        /// </summary>
        /// <param name="paymentModel"></param>
        /// <returns>returns gateway response</returns>
        private GatewayResponseModel CreateCaptureTransaction(PaymentModel paymentModel)
        {
            hostedIframe.ANetApiResponse response = CaptureTransaction(paymentModel);
            GatewayResponseModel gatewayResponse = new GatewayResponseModel();
            var responseCode = response.messages.message.FirstOrDefault(x => x.code == AuthorizeNetResponseSuccess);
            if (Equals(responseCode, null))
            {
                hostedIframe.ANetApiResponse errorResponse = (hostedIframe.ANetApiResponse)response;
                if (!Equals(errorResponse, null))
                    gatewayResponse.GatewayResponseData = $"code: {errorResponse.messages.message[0].code}, msg: {errorResponse.messages.message[0].text}";
                gatewayResponse.IsSuccess = false;
                return gatewayResponse;
            }
            if (!Equals(responseCode, null))
            {
                gatewayResponse.TransactionId = paymentModel.TransactionId;
                gatewayResponse.IsSuccess = true;
                gatewayResponse.PaymentStatus = ZnodePaymentStatus.CAPTURED;
            }
            return gatewayResponse;
        }
        #endregion

        #region Protected Methods
        /// <summary>
        /// Calculate the billing period
        /// </summary>
        /// <param name="Unit">Unit for the billing</param>
        /// <param name="Frequency">Frequency for the billing</param>
        /// <param name="NewFrequency">New Frequency for the billing</param>
        /// <returns>Returns the ARB Subscription Unit</returns>
        protected ARBSubscriptionUnitEnum BillingPeriod(string Unit, string Frequency, out int NewFrequency)
        {
            int freqValue = int.Parse(Frequency);
            NewFrequency = freqValue;

            if (Unit.Equals("DAY", StringComparison.OrdinalIgnoreCase))
                return ARBSubscriptionUnitEnum.days;
            else if (Unit.Equals("WEEK", StringComparison.OrdinalIgnoreCase))
            {
                NewFrequency = freqValue * 7; // Change it to days.
                return ARBSubscriptionUnitEnum.days;
            }
            else if (Unit.Equals("MONTH", StringComparison.OrdinalIgnoreCase))
                return ARBSubscriptionUnitEnum.months;
            else if (Unit.Equals("YEAR", StringComparison.OrdinalIgnoreCase))
            {
                NewFrequency = freqValue * 12; // Change this value into months
                return ARBSubscriptionUnitEnum.months;
            }
            return ARBSubscriptionUnitEnum.days;
        }
        #endregion

        public TransactionDetailsModel GetTransactionDetails(PaymentModel paymentModel)
        {
            string transStatus = string.Empty;
            TransactionDetailsModel gatewayResponse = new TransactionDetailsModel();
            getTransactionDetailsRequest request = new getTransactionDetailsRequest();
            XmlApiUtilities.PopulateMerchantAuthentication((ANetApiRequest)request, paymentModel.GatewayLoginName, paymentModel.GatewayTransactionKey);

            request.transId = paymentModel.TransactionId;

            System.Xml.XmlDocument response_xml = null;
            bool bResult = XmlApiUtilities.PostRequest(request, out response_xml, paymentModel.GatewayTestMode);
            object response = null;
            getTransactionDetailsResponse api_response = null;

            if (bResult)
                bResult = XmlApiUtilities.ProcessXmlResponse(response_xml, out response);
            if (!(response is getTransactionDetailsResponse))
            {
                var errorResponse = (ANetApiResponse)response;
                gatewayResponse.IsSuccess = false;
                if (!Equals(errorResponse, null))
                {
                    gatewayResponse.ResponseText = errorResponse.messages.message[0].text;
                }
                return gatewayResponse;
            }
            if (bResult)
                api_response = (getTransactionDetailsResponse)response;

            if (!Equals(api_response?.transaction?.transactionStatus, null))
            {
                gatewayResponse.TransactionStatus = api_response.transaction.transactionStatus;
                gatewayResponse.TransactionType = api_response.transaction.transactionType;
                gatewayResponse.TransactionId = api_response.transaction.transId;
                gatewayResponse.ResponseCode = api_response.transaction.responseReasonDescription;
                gatewayResponse.IsSuccess = true;
                gatewayResponse.GatewayCode = paymentModel.GatewayCode;
            }
            else
            {
                gatewayResponse.IsSuccess = false;
                gatewayResponse.ResponseText = "Unable to get transaction details.";
            }

            return gatewayResponse;
        }

        /// <summary>
        /// This method Generates the token for iframe for AuthorizeNet
        /// </summary>
        /// <param name="paymentGatewayTokenModel">PaymentGatewayTokenModel</param>
        /// <returns>Token for the iframe rendering</returns>
        public PaymentGatewayTokenModel TokenGenerator(PaymentGatewayTokenModel paymentGatewayTokenModel)
        {
            object billingAddress=null;
            object shippingAddress = null;
            object orderInformationBillTo = null;
            GetAuthNetRequest(paymentGatewayTokenModel.GatewayLoginName, paymentGatewayTokenModel.GatewayTransactionKey);
            paymentGatewayTokenModel.PaymentGatewayTokenUrl = Convert.ToString(ConfigurationManager.AppSettings["AuthorizeNetIFrameUrl"]); //ZnodeApiSettings.AuthorizeNetIFrameUrl;

            hostedIframe.settingType[] settings = new hostedIframe.settingType[10];
            settings[0] = new hostedIframe.settingType();
            settings[0].settingName = settingNameEnum.hostedPaymentBillingAddressOptions.ToString();
            settings[0].settingValue = "{\"show\": false , \"required\": false }";
            settings[1] = new hostedIframe.settingType();
            settings[1].settingName = settingNameEnum.hostedPaymentButtonOptions.ToString();
            settings[1].settingValue = "{\"text\": \"PAY AND SUBMIT\"}";
            settings[2] = new hostedIframe.settingType();
            settings[2].settingName = settingNameEnum.hostedPaymentCustomerOptions.ToString();
            settings[2].settingValue = "{\"showEmail\": false ,\"requiredEmail\": false ,\"addPaymentProfile\": true}";
            settings[3] = new hostedIframe.settingType();
            settings[3].settingName = settingNameEnum.hostedPaymentOrderOptions.ToString();
            settings[3].settingValue = "{\"show\": true }";
            settings[4] = new hostedIframe.settingType();
            settings[4].settingName = settingNameEnum.hostedPaymentPaymentOptions.ToString();
            settings[4].settingValue = "{\"cardCodeRequired\": true, \"showCreditCard\": true, \"showBankAccount\": false }";
            settings[5] = new hostedIframe.settingType();
            settings[5].settingName = settingNameEnum.hostedPaymentReturnOptions.ToString();
            settings[5].settingValue = "{\"showReceipt\": false ,\"url\":\"" + paymentGatewayTokenModel.IFrameUrl + "\",\"urlText\":\"Continue\",\"cancelUrlText\":\"CANCEL\"}";
            settings[6] = new hostedIframe.settingType();
            settings[6].settingName = settingNameEnum.hostedPaymentSecurityOptions.ToString();
            settings[6].settingValue = "{\"captcha\": false }";
            settings[7] = new hostedIframe.settingType();
            settings[7].settingName = settingNameEnum.hostedPaymentShippingAddressOptions.ToString();
            settings[7].settingValue = "{\"show\": false ,\"required\": false }";
            settings[8] = new hostedIframe.settingType();
            settings[8].settingName = settingNameEnum.hostedPaymentStyleOptions.ToString();
            settings[8].settingValue = paymentGatewayTokenModel.IsAdminRequestUrl?"{\"bgColor\": \"#5db043\"}": "{\"bgColor\": \"#ff6f00\"}";

            settings[9] = new hostedIframe.settingType();
            settings[9].settingName = settingNameEnum.hostedPaymentIFrameCommunicatorUrl.ToString();
            settings[9].settingValue = "{\"url\": \"" + paymentGatewayTokenModel.IFrameUrl + "\"}";
            
            var orderType = new hostedIframe.orderType
            {
                invoiceNumber = paymentGatewayTokenModel.OrderNumber
            };
            if (paymentGatewayTokenModel.GatewayCode == PaymentConstant.AuthorizeNet)
            {
                billingAddress = new hostedIframe.customerAddressType
                {
                    firstName = paymentGatewayTokenModel.BillingAddress.FirstName,
                    lastName = paymentGatewayTokenModel.BillingAddress.LastName,
                    state = paymentGatewayTokenModel.BillingAddress.StateName,
                    country = paymentGatewayTokenModel.BillingAddress.CountryName,
                    company = paymentGatewayTokenModel.BillingAddress.CompanyName,
                    address = paymentGatewayTokenModel.BillingAddress.Address1,
                    city = paymentGatewayTokenModel.BillingAddress.CityName,
                    zip = paymentGatewayTokenModel.BillingAddress.PostalCode,
                    phoneNumber = paymentGatewayTokenModel.BillingAddress.PhoneNumber,
                    faxNumber = paymentGatewayTokenModel.BillingAddress.FaxNumber,
                };

                shippingAddress = new hostedIframe.customerAddressType
                {
                    firstName = paymentGatewayTokenModel.ShippingAddress.FirstName,
                    lastName = paymentGatewayTokenModel.ShippingAddress.LastName,
                    state = paymentGatewayTokenModel.ShippingAddress.StateName,
                    country = paymentGatewayTokenModel.ShippingAddress.CountryName,
                    company = paymentGatewayTokenModel.ShippingAddress.CompanyName,
                    address = paymentGatewayTokenModel.ShippingAddress.Address1,
                    city = paymentGatewayTokenModel.ShippingAddress.CityName,
                    zip = paymentGatewayTokenModel.ShippingAddress.PostalCode,
                    phoneNumber = paymentGatewayTokenModel.ShippingAddress.PhoneNumber,
                    faxNumber = paymentGatewayTokenModel.ShippingAddress.FaxNumber,
                };
                 orderInformationBillTo = new hostedIframe.customerDataType
                {
                    email = paymentGatewayTokenModel.BillingAddress.EmailAddress,
                };
            }
            var transactionRequest = new hostedIframe.transactionRequestType
                {
                    transactionType = transactionTypeEnum.authOnlyTransaction.ToString(),    // charge the card
                    amount = paymentGatewayTokenModel.Total,
                    order = orderType,
                    billTo = (hostedIframe.customerAddressType)billingAddress,
                    shipTo = (hostedIframe.customerAddressType)shippingAddress,
                    customer = (hostedIframe.customerDataType)orderInformationBillTo,
                };
           

            if(paymentGatewayTokenModel.UserId > 0)
            {
                transactionRequest.profile = new hostedIframe.customerProfilePaymentType
                {
                    customerProfileId = GetCustomerProfileId(paymentGatewayTokenModel.UserId)
                };
                paymentGatewayTokenModel.CustomerProfileId = transactionRequest.profile.customerProfileId;
            }

            var request = new hostedIframe.getHostedPaymentPageRequest();
            request.transactionRequest = transactionRequest;
            request.hostedPaymentSettings = settings;

           
            ServicePointManager.SecurityProtocol = SecurityProtocolType.Ssl3 | SecurityProtocolType.Tls12 | SecurityProtocolType.Tls11 | SecurityProtocolType.Tls;
            // instantiate the contoller that will call the service
            var controller = new getHostedPaymentPageController(request);
            controller.Execute();
            // get the response from the service (errors contained if any)
            var response = controller.GetApiResponse();
            ////validate
            if (response != null && response.messages.resultCode == hostedIframe.messageTypeEnum.Ok)
            {
                paymentGatewayTokenModel.PaymentGatewayToken = response.token;
            }

            return paymentGatewayTokenModel;
        }

        //This method fetches the customerprofile id from znodepaymentmethod table based on user id.
        private string GetCustomerProfileId(int userId)
        {
            string customerProfileId = string.Empty;
            if (userId > 0)
            {
                IZnodePaymentRepository<ZnodePaymentMethod> _paymentRepository = new ZnodePaymentRepository<ZnodePaymentMethod>();
                customerProfileId = _paymentRepository.Table?.FirstOrDefault(payment => payment.UserId == userId && !string.IsNullOrEmpty(payment.CustomerProfileId))?.CustomerProfileId;
            }
            return customerProfileId;
        }

        //This method Gets the run environment and merchant authentication headers.
        private void GetAuthNetRequest(string gatewayLoginName, string gatewayTransactionKey)
        {
            ApiOperationBase<hostedIframe.ANetApiRequest, hostedIframe.ANetApiResponse>.RunEnvironment = Convert.ToBoolean(ConfigurationManager.AppSettings["IsSandboxAuthorizeNetAccount"]) ? AuthorizeNet.Environment.SANDBOX : AuthorizeNet.Environment.PRODUCTION;

            // define the merchant information (authentication / transaction id)
            ApiOperationBase<hostedIframe.ANetApiRequest, hostedIframe.ANetApiResponse>.MerchantAuthentication = new hostedIframe.merchantAuthenticationType()
            {
                name = gatewayLoginName,
                ItemElementName = hostedIframe.ItemChoiceType.transactionKey,
                Item = gatewayTransactionKey
            };
        }
    }
}
