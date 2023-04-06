using Newtonsoft.Json.Linq;
using System;
using System.Configuration;
using System.Diagnostics;
using Znode.Multifront.PaymentApplication.Data;
using Znode.Multifront.PaymentApplication.Helpers;
using Znode.Multifront.PaymentApplication.Models;

namespace Znode.Multifront.PaymentApplication.Providers
{
    public class CardConnectProvider : BaseProvider, IPaymentProviders
    {
        //Validate Card details
        public GatewayResponseModel ValidateCreditcard(PaymentModel paymentModel)
        {
            GatewayResponseModel response = new GatewayResponseModel();
            if ((!string.IsNullOrEmpty(paymentModel.CustomerProfileId) && !string.IsNullOrEmpty(paymentModel.CustomerPaymentProfileId)))
            {
                return (paymentModel.IsCapture) ? CaptureTransaction(paymentModel) : AuthorizePayment(paymentModel);
            }
            //To Create Customer Profile based on user input.
            response = CreateCustomer(paymentModel);
            return response;
        }

        //Authorize Transaction
        public GatewayResponseModel AuthorizePayment(PaymentModel paymentModel)
        {
            GatewayResponseModel gatewayResponse = new GatewayResponseModel();
            JObject request = new JObject();
            request.Add("merchid", paymentModel.GatewayTransactionKey);
            request.Add("profile", paymentModel.CustomerProfileId + "/" + paymentModel.CustomerPaymentProfileId);
            request.Add("amount", Math.Round(Convert.ToDecimal(paymentModel?.Total), 2).ToString("0.00"));
            request.Add("currency", paymentModel.GatewayCurrencyCode);
            request.Add("orderid", paymentModel.OrderId);
            request.Add("name", paymentModel.BillingFirstName);
            request.Add("ecomind", "E");
            request.Add("cof","C");
            request.Add("cofscheduled", "N");
            if (!string.IsNullOrEmpty(paymentModel.CardSecurityCode))
            {
                request.Add("cvv2", paymentModel.CardSecurityCode);
            }
            GetACHRequestModel(paymentModel, request);

            try
            {
                // Create the REST client
                CardConnectRestClient client = new CardConnectRestClient(GetAPIEndpoint(paymentModel), paymentModel.GatewayLoginName, paymentModel.GatewayLoginPassword);

                // Send an AuthTransaction request
                JObject response = client.AuthorizeTransaction(request);
                CardConnectResponseModel data = response.ToObject<CardConnectResponseModel>();

                if (string.Equals(data.Respstat, PaymentConstant.ApproveResponseCode, StringComparison.InvariantCultureIgnoreCase))
                {
                    gatewayResponse.IsGatewayPreAuthorize = paymentModel.GatewayPreAuthorize;
                    gatewayResponse.TransactionId = data.Retref;
                    gatewayResponse.IsSuccess = true;
                    gatewayResponse.CustomerProfileId = paymentModel.CustomerProfileId;
                    gatewayResponse.CustomerShippingAddressId = paymentModel.CustomerShippingAddressId;
                    gatewayResponse.CustomerPaymentProfileId = paymentModel.CustomerPaymentProfileId;
                    gatewayResponse.PaymentStatus = ZnodePaymentStatus.AUTHORIZED;
                }
                else
                {
                    gatewayResponse.IsSuccess = false;
                    gatewayResponse.GatewayResponseData = data.Resptext;
                    Logging.LogMessage("Transaction authorization failed.", Logging.Components.Payment.ToString(), TraceLevel.Info, data);
                }
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(paymentModel.PaymentApplicationSettingId, ex.Message);
            }
            return gatewayResponse;
        }

        //Capture Transaction
        public GatewayResponseModel CaptureTransaction(PaymentModel paymentModel)
        {
            Console.WriteLine("\nCapture Transaction Request");
            GatewayResponseModel gatewayResponse = new GatewayResponseModel();
            JObject request = new JObject();
            request.Add("merchid", paymentModel.GatewayTransactionKey);
            request.Add("retref", paymentModel.TransactionId);
            request.Add("cof","C");
            request.Add("cofscheduled", "N");

            try
            {
                // Create the CardConnect REST client
                CardConnectRestClient client = new CardConnectRestClient(GetAPIEndpoint(paymentModel), paymentModel.GatewayLoginName, paymentModel.GatewayLoginPassword);

                // Send a captureTransaction request
                JObject response = client.CaptureTransaction(request);
                CardConnectResponseModel data = response.ToObject<CardConnectResponseModel>();


                if (data.Setlstat.Contains("Capture"))
                {
                    gatewayResponse.IsGatewayPreAuthorize = paymentModel.GatewayPreAuthorize;
                    gatewayResponse.TransactionId = data.Retref;
                    gatewayResponse.IsSuccess = true;
                    gatewayResponse.PaymentStatus = ZnodePaymentStatus.CAPTURED;
                }
                else
                {
                    gatewayResponse.IsSuccess = false;
                    gatewayResponse.GatewayResponseData = data.Resptext;
                    Logging.LogMessage("Transaction Capture failed.", Logging.Components.Payment.ToString(), TraceLevel.Info, data);
                }
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(paymentModel.PaymentApplicationSettingId, ex.Message);
            }
            return gatewayResponse;
        }

        //Refund Transaction
        public GatewayResponseModel Refund(PaymentModel paymentModel)
        {
            GatewayResponseModel gatewayResponse = new GatewayResponseModel();
            JObject request = new JObject();
            request.Add("merchid", paymentModel.GatewayTransactionKey);
            request.Add("retref", paymentModel.TransactionId);
            request.Add("amount", Math.Round(Convert.ToDecimal(paymentModel?.Total), 2).ToString("0.00"));

            try
            {
                // Create the CardConnect REST client
                CardConnectRestClient client = new CardConnectRestClient(GetAPIEndpoint(paymentModel), paymentModel.GatewayLoginName, paymentModel.GatewayLoginPassword);

                // Send a refundTransaction request
                JObject response = client.RefundTransaction(request);
                CardConnectResponseModel data = response.ToObject<CardConnectResponseModel>();


                if (string.Equals(data.Respstat, PaymentConstant.DeclineResponseCode, StringComparison.InvariantCultureIgnoreCase))
                {
                    gatewayResponse.IsSuccess = false;
                    gatewayResponse.ResponseText = data.Resptext;
                    Logging.LogMessage("Transaction Refund failed.", Logging.Components.Payment.ToString(), TraceLevel.Info, data);
                }
                else
                {
                    gatewayResponse.TransactionId = data.Retref;
                    gatewayResponse.IsSuccess = true;
                    gatewayResponse.ResponseText = data.Respstat;
                    gatewayResponse.ResponseCode = data.Respcode;
                    gatewayResponse.PaymentStatus = ZnodePaymentStatus.REFUNDED;
                }
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(paymentModel.PaymentApplicationSettingId, ex.Message);
            }
            return gatewayResponse;
        }

        //Void Transaction
        public GatewayResponseModel Void(PaymentModel paymentModel)
        {
            GatewayResponseModel gatewayResponse = new GatewayResponseModel();
            JObject request = new JObject();
            request.Add("merchid", paymentModel.GatewayTransactionKey);
            request.Add("retref", paymentModel.TransactionId);

            try
            {
                // Create the CardConnect REST client
                CardConnectRestClient client = new CardConnectRestClient(GetAPIEndpoint(paymentModel), paymentModel.GatewayLoginName, paymentModel.GatewayLoginPassword);

                // Send a void Transaction request
                JObject response = client.VoidTransaction(request);
                CardConnectResponseModel data = response.ToObject<CardConnectResponseModel>();


                if (string.Equals(data.Respstat, PaymentConstant.DeclineResponseCode, StringComparison.InvariantCultureIgnoreCase))
                {
                    gatewayResponse.IsSuccess = false;
                    gatewayResponse.ResponseText = data.Resptext;
                    Logging.LogMessage("Transaction Void failed.", Logging.Components.Payment.ToString(), TraceLevel.Info, data);
                }
                else
                {
                    gatewayResponse.TransactionId = data.Retref;
                    gatewayResponse.IsSuccess = true;
                    gatewayResponse.PaymentStatus = ZnodePaymentStatus.VOIDED;
                }
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(paymentModel.PaymentApplicationSettingId, ex.Message);
            }
            return gatewayResponse;
        }

        public GatewayResponseModel Subscription(PaymentModel paymentModel)
        {
            return new GatewayResponseModel();
        }

        //Get Transaction status
        public TransactionDetailsModel GetTransactionDetails(PaymentModel paymentModel)
        {
            TransactionDetailsModel gatewayResponse = new TransactionDetailsModel();

            if (!string.IsNullOrEmpty(paymentModel.TransactionId))
            {
                // Create the CardConnect REST client
                CardConnectRestClient client = new CardConnectRestClient(GetAPIEndpoint(paymentModel), paymentModel.GatewayLoginName, paymentModel.GatewayLoginPassword);

                // Send a inquire Transaction request
                JObject response = client.InquireTransaction(paymentModel.GatewayTransactionKey, paymentModel.TransactionId);
                CardConnectResponseModel data = response.ToObject<CardConnectResponseModel>();

                if (string.Equals(data.Resptext, "Txn not found", StringComparison.InvariantCultureIgnoreCase) || string.Equals(data.Respstat, PaymentConstant.DeclineResponseCode, StringComparison.InvariantCultureIgnoreCase))
                {
                    gatewayResponse.IsSuccess = false;
                    gatewayResponse.ResponseText = "Unable to get transaction details.";
                }
                else
                {
                    gatewayResponse.TransactionStatus = string.Equals(data.Setlstat, "Accepted", StringComparison.InvariantCultureIgnoreCase) ? "SettledSuccessfully" : data.Setlstat;
                    gatewayResponse.TransactionId = data.Retref;
                    gatewayResponse.IsRefundable = string.Equals(data.Refundable, "Y", StringComparison.InvariantCultureIgnoreCase);
                    gatewayResponse.IsVoidable = string.Equals(data.Voidable, "Y", StringComparison.InvariantCultureIgnoreCase);
                    gatewayResponse.ResponseCode = data.Respcode;
                    gatewayResponse.IsSuccess = true;
                }
            }
            return gatewayResponse;
        }

        //Create Customer
        public GatewayResponseModel CreateCustomer(PaymentModel paymentModel)
        {
            GatewayResponseModel response = new GatewayResponseModel();
            GatewayConnector gatewayConnector = new GatewayConnector();

            bool isSuccess = false;
            //Customer Logged IN
            if (!paymentModel.IsAnonymousUser)
            {
                //First Time user CustomerGUID is null 
                if (string.IsNullOrEmpty(paymentModel.CustomerGUID))
                {
                    //Saved card for future use is True
                    if (paymentModel.IsSaveCreditCard)
                    {
                        //Create Customer and add it to vault.
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
                    else //Saved card for future use is false
                        return CreatePaymentGatewayCustomer(paymentModel);
                }
                else //existing user CustomerGUID is present 
                {
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
                    else
                        return CreatePaymentGatewayCustomer(paymentModel);
                }
            }
            else
                return CreatePaymentGatewayCustomer(paymentModel);//As it is which is the normal one without vault.
        }

        //Create Customer Payment Profile
        private GatewayResponseModel CreatePaymentGatewayCustomer(PaymentModel paymentModel)
        {
            GatewayResponseModel response = CreateCustomerProfile(paymentModel);
            if (!string.IsNullOrEmpty(response.CustomerProfileId))
            {
                paymentModel.CustomerProfileId = response.CustomerProfileId;
                paymentModel.CustomerShippingAddressId = response.CustomerShippingAddressId;
                paymentModel.CustomerPaymentProfileId = response.CustomerPaymentProfileId;
                if (response.IsSuccess && paymentModel.IsAnonymousUser)
                {
                    GatewayConnector gatewayConnector = new GatewayConnector();
                    response.IsSuccess = gatewayConnector.SavePaymentDetails(paymentModel);
                }
            }
            return Equals(response, null) ? new GatewayResponseModel() : response;
        }

        private GatewayResponseModel CreatePaymentGatewayVault(PaymentModel paymentModel)
        {
            GatewayResponseModel response = new GatewayResponseModel();
            //get customer profile id from db using guid if not exit the create user
            PaymentMethodsService repository = new PaymentMethodsService();
            ZnodePaymentMethod payment = null;
            if (paymentModel.IsACHPayment)
                payment = repository.GetPaymentMethodForACH(paymentModel.PaymentApplicationSettingId, paymentModel.CustomerGUID);
            else
             payment = repository.GetPaymentMethod(paymentModel.PaymentApplicationSettingId, paymentModel.CustomerGUID);

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

        // Create Profile Request
        private GatewayResponseModel CreateCustomerPaymentProfile(PaymentModel paymentModel)
        {
            GatewayResponseModel gatewayResponse = new GatewayResponseModel();
            JObject request = new JObject();
            request.Add("merchid", paymentModel.GatewayTransactionKey);
            request.Add("profile", paymentModel.CustomerProfileId);
            request.Add("account", paymentModel.CardDataToken);
            request.Add("expiry", paymentModel.CardExpirationYear + paymentModel.CardExpirationMonth);
            request.Add("name", paymentModel.CardHolderName);
            request.Add("address", paymentModel.BillingStreetAddress1);
            request.Add("city", paymentModel.BillingCity);
            request.Add("region", paymentModel.BillingStateCode);
            request.Add("country", paymentModel.BillingCountryCode);
            request.Add("postal", paymentModel.BillingPostalCode);
            request.Add("email", paymentModel.BillingEmailId);
            request.Add("phone", paymentModel.BillingPhoneNumber);

            try
            {
                // Create the CardConnect REST client
                CardConnectRestClient client = new CardConnectRestClient(GetAPIEndpoint(paymentModel), paymentModel.GatewayLoginName, paymentModel.GatewayLoginPassword);

                // Create profile using Profile Service
                JObject response = client.ProfileCreate(request);
                CardConnectResponseModel data = response.ToObject<CardConnectResponseModel>();

                // Handle response
                if (!Equals(data?.Profileid, null))
                {
                    paymentModel.CustomerProfileId = data.Profileid;
                    gatewayResponse.CustomerProfileId = data.Profileid;
                    gatewayResponse.CustomerPaymentProfileId = data.Acctid;
                    gatewayResponse.IsSuccess = true;
                }
                else
                {
                    gatewayResponse.IsSuccess = false;
                    gatewayResponse.ResponseText = "Unable to create customer profile.";
                }

                if (gatewayResponse.IsSuccess && paymentModel.IsAnonymousUser)
                {
                    GatewayConnector gatewayConnector = new GatewayConnector();
                    gatewayResponse.IsSuccess = gatewayConnector.SavePaymentDetails(paymentModel);
                }
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(paymentModel.PaymentApplicationSettingId, ex.Message);
            }
            return gatewayResponse;
        }

        // Create Profile Request
        private GatewayResponseModel CreateCustomerProfile(PaymentModel paymentModel)
        {
            GatewayResponseModel gatewayResponse = new GatewayResponseModel();
            JObject request = new JObject();
            request.Add("merchid", paymentModel.GatewayTransactionKey);
            request.Add("account", paymentModel.CardDataToken);
            request.Add("expiry", paymentModel.CardExpirationYear + paymentModel.CardExpirationMonth);
            request.Add("name", paymentModel.CardHolderName);
            request.Add("address", paymentModel.BillingStreetAddress1);
            request.Add("city", paymentModel.BillingCity);
            request.Add("region", paymentModel.BillingStateCode);
            request.Add("country", paymentModel.BillingCountryCode);
            request.Add("postal", paymentModel.BillingPostalCode);
            request.Add("email", paymentModel.BillingEmailId);
            request.Add("phone", paymentModel.BillingPhoneNumber);
            request.Add("ecomind", "E");
            request.Add("amount", Math.Round(Convert.ToDecimal(paymentModel?.Total), 2).ToString("0.00"));
            request.Add("cof", "C");
            request.Add("cofscheduled", "N");

            try
            {
                // Create the CardConnect REST client
                CardConnectRestClient client = new CardConnectRestClient(GetAPIEndpoint(paymentModel), paymentModel.GatewayLoginName, paymentModel.GatewayLoginPassword);

                // Create profile using Profile Service
                JObject response = client.ProfileCreate(request);
                CardConnectResponseModel data = response.ToObject<CardConnectResponseModel>();

                // Handle response
                if (Equals(data?.Respstat, PaymentConstant.ApproveResponseCode))
                {
                    paymentModel.CustomerProfileId = data.Profileid;
                    gatewayResponse.CustomerProfileId = data.Profileid;
                    gatewayResponse.CustomerPaymentProfileId = data.Acctid;
                    gatewayResponse.IsSuccess = true;

                }
                else
                {
                    gatewayResponse.IsSuccess = false;
                    gatewayResponse.ResponseText = "Unable to create customer profile.";
                }

                if (gatewayResponse.IsSuccess && paymentModel.IsAnonymousUser)
                {
                    GatewayConnector gatewayConnector = new GatewayConnector();
                    gatewayResponse.IsSuccess = gatewayConnector.SavePaymentDetails(paymentModel);
                }
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(paymentModel.PaymentApplicationSettingId, ex.Message);
            }
            return gatewayResponse;
        }

        //Get API URL
        private string GetAPIEndpoint(PaymentModel model)
        {
            return model.GatewayTestMode ? Convert.ToString(ConfigurationManager.AppSettings["CardConnectTestURL"]) : Convert.ToString(ConfigurationManager.AppSettings["CardConnectLiveURL"]);
        }

        //Get additional ach request model
        protected virtual void GetACHRequestModel(PaymentModel paymentModel, JObject request)
        {
            if (paymentModel.IsACHPayment)
            {
                request.Add("accttype", "ECHK");
            }
        }

        public PaymentGatewayTokenModel TokenGenerator(PaymentGatewayTokenModel paymentModel)
        {
            //Not Implemented
            throw new NotImplementedException();
        }
    }
}


