using SecureNetRestApiSDK.Api.Controllers;
using SecureNetRestApiSDK.Api.Models;
using SecureNetRestApiSDK.Api.Requests;
using SecureNetRestApiSDK.Api.Responses;
using SNET.Core;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using Znode.Multifront.PaymentApplication.Data;
using Znode.Multifront.PaymentApplication.Helpers;
using Znode.Multifront.PaymentApplication.Models;

namespace Znode.Multifront.PaymentApplication.Providers
{
    public class SecureNetCustomerProvider : BaseProvider, IPaymentProviders
    {
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
                if (!string.IsNullOrEmpty(paymentModel.CustomerProfileId))
                    return (paymentModel.IsCapture)
                        ? CaptureTransaction(paymentModel) : AuthorizeTransaction(paymentModel);

                //To Create Customer Profile based on user input.
                response = CreateCustomer(paymentModel);
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
        /// Refund payment
        /// </summary>
        /// <param name="paymentModel">PaymentModel paymentModel</param>
        /// <returns>returns Payment Gateway Response</returns>
        public GatewayResponseModel Refund(PaymentModel paymentModel)
        {
            GatewayResponseModel gatewayResponse = new GatewayResponseModel();
            RefundRequest request = new RefundRequest
            {
                TransactionId = Convert.ToInt32(paymentModel.TransactionId),
                Amount = decimal.Parse(paymentModel.Total),
                DeveloperApplication = new DeveloperApplication
                {
                    DeveloperId = Convert.ToInt32(ConfigurationManager.AppSettings["SecureNetDeveloperId"]),
                    Version = Convert.ToString(ConfigurationManager.AppSettings["SecureNetVersion"])
                }
            };

            APIContext apiContext = SetAPIDetails(paymentModel);
            PaymentsController controller = new PaymentsController();
            RefundResponse response = controller.ProcessRequest<RefundResponse>(apiContext, request);

            if (response.Success)
            {
                gatewayResponse.TransactionId = Convert.ToString(response.Transaction.TransactionId);
                gatewayResponse.PaymentStatus = ZnodePaymentStatus.REFUNDED;
            }

            gatewayResponse.GatewayResponseData = $"code: {response.ResponseCode}, msg: {response.Message}";
            gatewayResponse.IsSuccess = response.Success;

            return gatewayResponse;
        }

        /// <summary>
        /// Void payment
        /// </summary>
        /// <param name="paymentModel">PaymentModel paymentModel</param>
        /// <returns>returns payment gatway response</returns>
        public GatewayResponseModel Void(PaymentModel paymentModel)
        {
            GatewayResponseModel gatewayResponse = new GatewayResponseModel();
            VoidRequest request = new VoidRequest
            {
                TransactionId = Convert.ToInt32(paymentModel.TransactionId),
                DeveloperApplication = new DeveloperApplication
                {
                    DeveloperId = Convert.ToInt32(ConfigurationManager.AppSettings["SecureNetDeveloperId"]),
                    Version = Convert.ToString(ConfigurationManager.AppSettings["SecureNetVersion"])
                }
            };

            APIContext apiContext = SetAPIDetails(paymentModel);
            PaymentsController controller = new PaymentsController();

            VoidResponse response = controller.ProcessRequest<VoidResponse>(apiContext, request);

            if (response.Success)
            {
                gatewayResponse.GatewayResponseData = $"code: {response.ResponseCode}, msg: {response.Message}";
                gatewayResponse.TransactionId = Convert.ToString(response.Transaction.TransactionId);
            }
            gatewayResponse.IsSuccess = response.Success;
            gatewayResponse.PaymentStatus = ZnodePaymentStatus.VOIDED;
            return gatewayResponse;
        }

        /// <summary>
        /// This method will create the subscription in the payment gateway.
        /// </summary>
        /// <param name="paymentModel">Complete Payment Data from Multifront</param>
        /// <returns>Response from the payment gateway</returns>
        public GatewayResponseModel Subscription(PaymentModel paymentModel)
        {
            GatewayResponseModel gatewayResponse = new GatewayResponseModel();

            //create Request object
            AddRecurringPaymentPlanRequest request = new AddRecurringPaymentPlanRequest
            {
                CustomerId = paymentModel.CustomerProfileId,
                Plan = MapRecurringPaymentPlan(paymentModel),
                DeveloperApplication = new DeveloperApplication
                {
                    DeveloperId = Convert.ToInt32(ConfigurationManager.AppSettings["SecureNetDeveloperId"]),
                    Version = Convert.ToString(ConfigurationManager.AppSettings["SecureNetVersion"])
                }
            };

            //To set Payment API details.
            APIContext apiContext = SetAPIDetails(paymentModel);
            PaymentsController controller = new PaymentsController();

            AddRecurringPaymentPlanResponse response = controller.ProcessRequest<AddRecurringPaymentPlanResponse>(apiContext, request);

            if (response.Success)
            {
                gatewayResponse.GatewayResponseData = $"code: {response.ResponseCode}, msg: {response.Message}";
                gatewayResponse.TransactionId = Convert.ToString(response.PlanId);
            }
            gatewayResponse.IsSuccess = response.Success;
            return gatewayResponse;
        }

        public TransactionDetailsModel GetTransactionDetails(PaymentModel paymentModel)
        {
            return new TransactionDetailsModel();
        }

        /// <summary>
        /// Create a transaction based on Customer Payment profile Id
        /// </summary>
        private GatewayResponseModel AuthorizeTransaction(PaymentModel paymentModel)
        {
            var gatewayResponse = new GatewayResponseModel();
            try
            {
                return AuthorizePayment(paymentModel);
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(paymentModel.PaymentApplicationSettingId, ex.Message);

                return new GatewayResponseModel
                {
                    GatewayResponseData = ex.Message.ToString(),
                    IsSuccess = false
                };
            }
        }

        /// <summary>
        /// Capture payment by transaction Id
        /// </summary>
        /// <param name="paymentModel"></param>
        /// <returns></returns>
        private GatewayResponseModel CaptureTransaction(PaymentModel paymentModel)
        {
            GatewayResponseModel gatewayResponse = new GatewayResponseModel();
            try
            {
                PriorAuthCaptureRequest request = new PriorAuthCaptureRequest
                {
                    Amount = Convert.ToDecimal(paymentModel.Total),
                    TransactionId = Convert.ToInt32(paymentModel.TransactionId),
                    DeveloperApplication = new DeveloperApplication
                    {
                        DeveloperId = Convert.ToInt32(ConfigurationManager.AppSettings["SecureNetDeveloperId"]),
                        Version = Convert.ToString(ConfigurationManager.AppSettings["SecureNetVersion"])
                    }
                };

                APIContext apiContext = SetAPIDetails(paymentModel);
                PaymentsController controller = new PaymentsController();
                PriorAuthCaptureResponse response = controller.ProcessRequest<PriorAuthCaptureResponse>(apiContext, request);

                return (response.Success)
                    ? new GatewayResponseModel { IsSuccess = true, TransactionId = Convert.ToString(response.Transaction.TransactionId), PaymentStatus = ZnodePaymentStatus.CAPTURED }
                    : new GatewayResponseModel { IsSuccess = false, GatewayResponseData = response.Message };
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(paymentModel.PaymentApplicationSettingId, ex.Message);

                gatewayResponse.GatewayResponseData = ex.Message.ToString();
                gatewayResponse.IsSuccess = false;
            }
            return gatewayResponse;
        }

        /// <summary>
        /// Create the Customer
        /// </summary>
        /// <param name="paymentModel">PaymentModel paymentModel</param>
        /// <returns>returns customer response</returns>
        private GatewayResponseModel GetCustomer(PaymentModel paymentModel)
        {
            GatewayResponseModel gatewayResponse = new GatewayResponseModel();
            CreateCustomerRequest request = new CreateCustomerRequest
            {
                FirstName = paymentModel.BillingFirstName,
                LastName = paymentModel.BillingLastName,
                PhoneNumber = paymentModel.BillingPhoneNumber,
                EmailAddress = paymentModel.BillingEmailId,
                SendEmailReceipts = true,
                Notes = string.Empty,
                Address = new Address
                {
                    Line1 = paymentModel.BillingStreetAddress1,
                    City = paymentModel.BillingCity,
                    State = paymentModel.BillingStateCode,
                    Zip = paymentModel.BillingPostalCode,
                },
                DeveloperApplication = new DeveloperApplication
                {
                    DeveloperId = Convert.ToInt32(ConfigurationManager.AppSettings["SecureNetDeveloperId"]),
                    Version = Convert.ToString(ConfigurationManager.AppSettings["SecureNetVersion"])
                }
            };

            APIContext apiContext = SetAPIDetails(paymentModel);
            CustomersController controller = new CustomersController();
            CreateCustomerResponse response = controller.ProcessRequest<CreateCustomerResponse>(apiContext, request);

            gatewayResponse.IsSuccess = response.Success;
            gatewayResponse.CustomerProfileId = response.CustomerId;
            gatewayResponse.ResponseText = response.Message;
            return gatewayResponse;
        }

        /// <summary>
        /// Create the Customer Payment
        /// </summary>
        /// <param name="paymentModel"></param>
        /// <returns>returns customer payment response</returns>
        private GatewayResponseModel CreateCustomerPayment(PaymentModel paymentModel)
        {
            GatewayResponseModel gatewayResponse = new GatewayResponseModel();
            AddPaymentMethodRequest request = new AddPaymentMethodRequest
            {
                CustomerId = paymentModel.CustomerProfileId,
                Card = new Card
                {
                    Number = paymentModel.CardNumber,
                    ExpirationDate = string.Concat(paymentModel.CardExpirationMonth, "/", paymentModel.CardExpirationYear),
                    Address = new Address
                    {
                        Line1 = paymentModel.BillingStreetAddress1,
                        City = paymentModel.BillingCity,
                        State = paymentModel.BillingStateCode,
                        Zip = paymentModel.BillingPostalCode
                    },
                    FirstName = paymentModel.BillingFirstName,
                    LastName = paymentModel.BillingLastName
                },
                Phone = paymentModel.BillingPhoneNumber,
                Notes = string.Empty,
                AccountDuplicateCheckIndicator = 0,
                Primary = true,
                DeveloperApplication = new DeveloperApplication
                {
                    DeveloperId = Convert.ToInt32(ConfigurationManager.AppSettings["SecureNetDeveloperId"]),
                    Version = Convert.ToString(ConfigurationManager.AppSettings["SecureNetVersion"])
                }
            };

            APIContext apiContext = SetAPIDetails(paymentModel);
            CustomersController controller = new CustomersController();
            AddPaymentMethodResponse response = controller.ProcessRequest<AddPaymentMethodResponse>(apiContext, request);

            gatewayResponse.IsSuccess = response.Success;
            gatewayResponse.CustomerProfileId = paymentModel.CustomerProfileId;
            gatewayResponse.CustomerPaymentProfileId = response.VaultPaymentMethod.PaymentId;
            gatewayResponse.ResponseText = response.Message;
            return gatewayResponse;
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
            if (Equals(paymentModel.IsAnonymousUser, false))
            {
                //First Time user CustomerGUID is null 
                if (string.IsNullOrEmpty(paymentModel.CustomerGUID))
                {
                    //Saved card for future use is True
                    if (Equals(paymentModel.IsSaveCreditCard, true))
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
                        return CreatePaymentGatewayCustomer(paymentModel);//As it is - regular one. no need to save in database    
                }
                else //existing user CustomerGUID is present 
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
                        response.PaymentToken = paymentModel.PaymentToken;
                        response.CustomerProfileId = paymentModel.CustomerProfileId;
                        response.CustomerPaymentProfileId = paymentModel.CustomerPaymentProfileId;
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
        /// To set Payment API details
        /// </summary>
        /// <param name="model">PaymentModel model</param>
        /// <returns>returns APIContext</returns>
        private APIContext SetAPIDetails(PaymentModel model)
        {
            APIContext apiContext = new APIContext();
            apiContext.Config = new Dictionary<string, string>();
            apiContext.Config.Add("secureNetId", model.GatewayLoginName);
            apiContext.Config.Add("secureKey", model.GatewayTransactionKey);
            apiContext.Config.Add("mode", model.GatewayTestMode ? Convert.ToString(ConfigurationManager.AppSettings["SecureNetTestMode"]) : Convert.ToString(ConfigurationManager.AppSettings["SecureNetLiveMode"]));
            apiContext.Config.Add("endpoint", model.GatewayTestMode ? Convert.ToString(ConfigurationManager.AppSettings["SecureNetSandboxEndpoint"]) : Convert.ToString(ConfigurationManager.AppSettings["SecureNetLiveEndpoint"]));
            apiContext.Config.Add("timeout", Convert.ToString(ConfigurationManager.AppSettings["SecureNetTimeout"]));
            apiContext.Config.Add("connectionTimeout", Convert.ToString(ConfigurationManager.AppSettings["SecureNetTimeout"]));
            apiContext.Config.Add("developerId", Convert.ToString(ConfigurationManager.AppSettings["SecureNetDeveloperId"]));
            apiContext.Config.Add("versionId", Convert.ToString(ConfigurationManager.AppSettings["SecureNetVersion"]));
            return apiContext;
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
                {
                    GatewayConnector gatewayConnector = new GatewayConnector();
                    response.IsSuccess = gatewayConnector.SavePaymentDetails(paymentModel);
                }
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
            //get customer profile id from db using guid if not exit the create user
            PaymentMethodsService repository = new PaymentMethodsService();
            ZnodePaymentMethod payment = repository.GetPaymentMethod(paymentModel.PaymentApplicationSettingId, paymentModel.CustomerGUID);
            if (!Equals(payment, null) && !string.IsNullOrEmpty(payment.CustomerProfileId))
                paymentModel.CustomerProfileId = payment.CustomerProfileId;
            else
            {
                response = GetCustomer(paymentModel);
                paymentModel.CustomerProfileId = response.CustomerProfileId;
            }
            if (!string.IsNullOrEmpty(paymentModel.CustomerProfileId))
            {
                response = CreateCustomerPayment(paymentModel);
                paymentModel.CustomerPaymentProfileId = response.CustomerPaymentProfileId;
            }
            return Equals(response, null) ? new GatewayResponseModel() : response;
        }

        //Authorize the Payment
        private GatewayResponseModel AuthorizePayment(PaymentModel paymentModel)
        {
            AuthorizeRequest authorizeRequest = new AuthorizeRequest
            {
                Amount = Convert.ToDecimal(paymentModel.Total),
                PaymentVaultToken = new PaymentVaultToken
                {
                    CustomerId = paymentModel.CustomerProfileId,
                    PaymentMethodId = paymentModel.CustomerPaymentProfileId,
                    PaymentType = Convert.ToString(ConfigurationManager.AppSettings["SecureNetPaymentType"])
                },
                DeveloperApplication = new DeveloperApplication
                {
                    DeveloperId = Convert.ToInt32(ConfigurationManager.AppSettings["SecureNetDeveloperId"]),
                    Version = Convert.ToString(ConfigurationManager.AppSettings["SecureNetVersion"])
                },
                ExtendedInformation = new ExtendedInformation
                {
                    TypeOfGoods = Convert.ToString(ConfigurationManager.AppSettings["SecureNetTypeOfGoods"])
                }
            };

            APIContext apiContext = SetAPIDetails(paymentModel);
            PaymentsController controller = new PaymentsController();

            AuthorizeResponse response = controller.ProcessRequest<AuthorizeResponse>(apiContext, authorizeRequest);
            GatewayResponseModel gatewayResponse = new GatewayResponseModel();
            if (response.Success)
            {
                gatewayResponse.TransactionId = Convert.ToString(response.Transaction.TransactionId);
                gatewayResponse.IsSuccess = true;
                gatewayResponse.GatewayResponseData = $"code: {response.ResponseCode}, msg: { response.Message}";
                gatewayResponse.CustomerProfileId = paymentModel.CustomerProfileId;
                gatewayResponse.CustomerPaymentProfileId = paymentModel.CustomerPaymentProfileId;
                gatewayResponse.PaymentStatus = ZnodePaymentStatus.AUTHORIZED;
                gatewayResponse.IsGatewayPreAuthorize = paymentModel.GatewayPreAuthorize;
            }
            else
            {
                gatewayResponse.IsSuccess = false;
                gatewayResponse.GatewayResponseData = $"code: {response.ResponseCode}, msg: {response.Message}";
            }
            return gatewayResponse;
        }

        //Map RecurringPaymentPlan object
        private RecurringPaymentPlan MapRecurringPaymentPlan(PaymentModel paymentModel)
        {
            RecurringPaymentPlan recurringPaymentPlan = new RecurringPaymentPlan();
            recurringPaymentPlan.Amount = Convert.ToDecimal(paymentModel.Subscription.Amount);
            recurringPaymentPlan.Active = true;
            recurringPaymentPlan.PrimaryPaymentMethodId = paymentModel.CustomerPaymentProfileId;
            recurringPaymentPlan.Frequency = 1;

            //Start date should be always greater than todays date.
            recurringPaymentPlan.StartDate = DateTime.Now.AddDays(1);
            switch (paymentModel.Subscription.Period)
            {
                case "WEEK":
                    recurringPaymentPlan.CycleType = "WEEKLY";
                    recurringPaymentPlan.DayOfTheWeek = 1;
                    recurringPaymentPlan.EndDate = DateTime.Now.AddDays((Convert.ToInt32(paymentModel.Subscription.Frequency) * 7 * paymentModel.Subscription.TotalCycles));
                    break;
                case "MONTH":
                    recurringPaymentPlan.CycleType = "MONTHLY";
                    recurringPaymentPlan.DayOfTheMonth = 1;
                    recurringPaymentPlan.EndDate = DateTime.Now.AddMonths(Convert.ToInt32(paymentModel.Subscription.Frequency) * paymentModel.Subscription.TotalCycles);
                    break;
                case "YEAR":
                    recurringPaymentPlan.CycleType = "ANNUALLY";
                    recurringPaymentPlan.Month = DateTime.Now.Month;
                    recurringPaymentPlan.EndDate = DateTime.Now.AddYears(Convert.ToInt32(paymentModel.Subscription.Frequency) * paymentModel.Subscription.TotalCycles);
                    break;
            }

            return recurringPaymentPlan;
        }

        public PaymentGatewayTokenModel TokenGenerator(PaymentGatewayTokenModel paymentModel)
        {
            throw new NotImplementedException();
        }
    }
}
