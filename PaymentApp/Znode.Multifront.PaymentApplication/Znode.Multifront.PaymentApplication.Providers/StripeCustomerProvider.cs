using Stripe;
using System;
using System.Configuration;
using System.Diagnostics;
using Znode.Multifront.PaymentApplication.Data;
using Znode.Multifront.PaymentApplication.Helpers;
using Znode.Multifront.PaymentApplication.Models;

namespace Znode.Multifront.PaymentApplication.Providers
{
    public class StripeCustomerProvider : BaseProvider, IPaymentProviders
    {
        /// <summary>
        /// Validate the credit card through Stripe
        /// </summary>
        /// <param name="paymentModel"></param>
        /// <returns></returns>
        public GatewayResponseModel ValidateCreditcard(PaymentModel paymentModel)
        {
            //Create stripe service object
            StripeChargeService chargeService = new StripeChargeService(paymentModel.GatewayLoginName);
            GatewayResponseModel gatewayResponse = new GatewayResponseModel();

            //Stripe response object
            StripeCharge currentcharge;

            //Capture transaction if transaction id present
            if (!string.IsNullOrEmpty(paymentModel.TransactionId))
            {
                currentcharge = chargeService.Capture(paymentModel.TransactionId);

                return (currentcharge.Captured.HasValue && currentcharge.Captured.Value)
                    ? new GatewayResponseModel { IsSuccess = true, TransactionId = currentcharge.Id, PaymentStatus = ZnodePaymentStatus.CAPTURED }
                    : new GatewayResponseModel { IsSuccess = false, GatewayResponseData = currentcharge.FailureMessage };
            }

            // Create Customer if customer Profile Id not present
            if (string.IsNullOrEmpty(paymentModel.CustomerProfileId))
                return CreateCustomer(paymentModel);

            // set StripeChargeCreateOptions object for Authorize payment
            StripeChargeCreateOptions stripeCharge = new StripeChargeCreateOptions
            {
                //Stripe Accept Amount in Sub-divisions Multiply by 100 convert amount into subdivisions
                //Note will not work for RO,LD,JD,ID,Rls,RMB currency symbols
                Amount = Convert.ToInt32(Convert.ToDecimal(paymentModel.Total) * 100),
                Currency = string.IsNullOrEmpty(paymentModel.GatewayCurrencyCode) ? Convert.ToString(ConfigurationManager.AppSettings["CurrencyCode"]) : paymentModel.GatewayCurrencyCode,
                CustomerId = paymentModel.CustomerProfileId,
                SourceTokenOrExistingSourceId = paymentModel.CustomerPaymentProfileId,
                Capture = false
            };

            //Authorize payment
            currentcharge = chargeService.Create(stripeCharge);

            if (!string.IsNullOrEmpty(currentcharge.Id))
            {
                gatewayResponse.IsSuccess = true;
                gatewayResponse.CustomerProfileId = paymentModel.CustomerProfileId;
                gatewayResponse.TransactionId = currentcharge.Id;
                gatewayResponse.CustomerPaymentProfileId = paymentModel.CustomerPaymentProfileId;
                gatewayResponse.PaymentStatus = ZnodePaymentStatus.AUTHORIZED;
                gatewayResponse.IsGatewayPreAuthorize = paymentModel.GatewayPreAuthorize;
                return gatewayResponse;
            }
            return new GatewayResponseModel { IsSuccess = false, GatewayResponseData = currentcharge.FailureMessage };
        }

        /// <summary>
        /// Refund transaction using the existing transaction key with the specified amount.
        /// </summary>
        /// <param name="paymentModel"></param>
        /// <returns></returns>
        public GatewayResponseModel Refund(PaymentModel paymentModel)
        {
            StripeRefundService stripeRefundService = new StripeRefundService(paymentModel.GatewayLoginName);

            //Stripe Accept Amount in Sub-divisions Multiply by 100 convert amount into subdivisions
            //Note will not work for RO,LD,JD,ID,Rls,RMB currency symbols
            //Call Refund service
            StripeRefund response = stripeRefundService.Create(paymentModel.TransactionId, new StripeRefundCreateOptions { Amount = Convert.ToInt32(Convert.ToDecimal(paymentModel.Total) * 100) });

            return (string.Equals(response.Status, "succeeded", StringComparison.OrdinalIgnoreCase))
                ? new GatewayResponseModel { IsSuccess = true, TransactionId = response.Id, PaymentStatus = ZnodePaymentStatus.REFUNDED }
                : new GatewayResponseModel { IsSuccess = false, GatewayResponseData = response.Status };
        }

        /// <summary>
        /// Void is not implemented
        /// </summary>
        /// <param name="paymentModel"></param>
        /// <returns></returns>
        public GatewayResponseModel Void(PaymentModel paymentModel)
        {
            //=> new GatewayResponseModel { IsSuccess = false, GatewayResponseData = "" };
            StripeChargeService chargeService = new StripeChargeService(paymentModel.GatewayLoginName);
            StripeCharge StripeCharge = chargeService.Get(paymentModel.TransactionId);

            //Check Transaction captured or not
            if (!Convert.ToBoolean(StripeCharge.Captured))
            {
                StripeRefundService stripeRefundService = new StripeRefundService(paymentModel.GatewayLoginName);

                //Void(Release) not captured transaction
                StripeRefund response = stripeRefundService.Create(paymentModel.TransactionId);

                return (string.Equals(response.Status, "succeeded", StringComparison.OrdinalIgnoreCase))
                    ? new GatewayResponseModel { IsSuccess = true, TransactionId = response.Id, PaymentStatus = ZnodePaymentStatus.REFUNDED }
                    : new GatewayResponseModel { IsSuccess = false, GatewayResponseData = response.Status };
            }
            return new GatewayResponseModel { IsSuccess = false, GatewayResponseData = "Unable to void Captured transaction" }; ;
        }
        /// <summary>
        /// Create a Subscription plan
        /// </summary>
        /// <param name="paymentModel"></param>
        /// <returns></returns>
        public GatewayResponseModel Subscription(PaymentModel paymentModel)
        {
            try
            {
                string key = paymentModel.GatewayLoginName;
                StripePlanCreateOptions myPlan = new StripePlanCreateOptions();

                //Stripe Accept Amount in Sub-divisions Multiply by 100 convert amount into subdivisions
                //Note will not work for RO,LD,JD,ID,Rls,RMB currency symbols
                myPlan.Amount = Convert.ToInt32(Convert.ToDecimal(paymentModel.Subscription.Amount) * 100);
                myPlan.Currency = string.IsNullOrEmpty(paymentModel.GatewayCurrencyCode) ? Convert.ToString(ConfigurationManager.AppSettings["CurrencyCode"]) : paymentModel.GatewayCurrencyCode;
                myPlan.Interval = paymentModel.Subscription.Period.ToLower();
                myPlan.Name = paymentModel.Subscription.ProfileName;
                myPlan.IntervalCount = paymentModel.Subscription.TotalCycles;    // amount of time that will lapse before the customer is billed

                myPlan.Id = $"my-plan- { Guid.NewGuid()}";
                StripePlanService planService = new StripePlanService(key);
                StripePlan planresponse = planService.Create(myPlan);
                if (!Equals(planresponse, null))
                {
                    StripeSubscriptionService subscriptionService = new StripeSubscriptionService(key);
                    StripeSubscription stripeSubscription = subscriptionService.Create(paymentModel.CustomerProfileId, planresponse.Id);

                    if (!Equals(stripeSubscription, null))
                        return new GatewayResponseModel { IsSuccess = true, TransactionId = stripeSubscription.Id };
                }
            }
            catch (Exception ex)
            {
                LoggingService.LogActivity(paymentModel.PaymentApplicationSettingId, ex.Message);
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
            }
            return new GatewayResponseModel { IsSuccess = false, GatewayResponseData = "Unable to process your request" };
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
            GatewayResponseModel response = new GatewayResponseModel();
            GatewayConnector gatewayConnector = new GatewayConnector();
            bool isSuccess = false;
            if (Equals(paymentModel.IsAnonymousUser, false))
            {
                if (string.IsNullOrEmpty(paymentModel.CustomerGUID))
                {
                    if (Equals(paymentModel.IsSaveCreditCard, true))
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
            }
            return Equals(response, null) ? new GatewayResponseModel() : response;
        }
        /// <summary>
        /// Create a Customer using card holder data.
        /// </summary>
        /// <param name="paymentModel"></param>
        /// <returns></returns>
        private GatewayResponseModel GetCustomer(PaymentModel paymentModel)
        {
            GatewayResponseModel response = new GatewayResponseModel();
            StripeCustomerCreateOptions customer = new StripeCustomerCreateOptions
            {
                Email = paymentModel.BillingEmailId,
                Description = paymentModel.BillingEmailId
            };
            StripeCustomerService customerservice = new StripeCustomerService(paymentModel.GatewayLoginName);
            StripeCustomer customerResponse = customerservice.Create(customer);
            string customerId = customerResponse.Id;
            response.IsSuccess = string.IsNullOrEmpty(customerId) ? false : true;
            response.CustomerProfileId = customerId;
            return response;
        }

        /// <summary>
        /// Create the Customer Payment
        /// </summary>
        /// <param name="paymentModel"></param>
        /// <returns>returns customer payment response</returns>
        private GatewayResponseModel CreateCustomerPayment(PaymentModel paymentModel)
        {
            GatewayResponseModel response = new GatewayResponseModel();
            StripeCardCreateOptions createCard = new StripeCardCreateOptions
            {
                SourceCard = new SourceCard
                {
                    Number = paymentModel.CardNumber,
                    ExpirationMonth = paymentModel.CardExpirationMonth,
                    ExpirationYear = paymentModel.CardExpirationYear,
                    Name = paymentModel.CardHolderName,
                    AddressCity = paymentModel.BillingCity,
                    AddressCountry = paymentModel.BillingCountryCode,
                    AddressLine1 = paymentModel.BillingStreetAddress1
                }
            };
            StripeCardService cardService = new StripeCardService(paymentModel.GatewayLoginName);
            StripeCard cardresponse = cardService.Create(paymentModel.CustomerProfileId, createCard);
            string cardId = cardresponse.Id;
            response.IsSuccess = string.IsNullOrEmpty(cardId) ? false : true;
            response.CustomerProfileId = paymentModel.CustomerProfileId;
            response.CustomerPaymentProfileId = cardId;
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

        public PaymentGatewayTokenModel TokenGenerator(PaymentGatewayTokenModel paymentModel)
        {
            throw new NotImplementedException();
        }
    }
}