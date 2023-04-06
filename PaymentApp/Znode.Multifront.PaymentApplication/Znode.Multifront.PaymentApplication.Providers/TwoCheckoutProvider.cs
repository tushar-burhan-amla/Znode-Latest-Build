using System;
using System.Configuration;
using TwoCheckout;
using Znode.Multifront.PaymentApplication.Models;

namespace Znode.Multifront.PaymentApplication.Providers
{
    public class TwoCheckoutProvider : BaseProvider, IPaymentProviders
    {
        /// <summary>
        /// Validate Credit card
        /// </summary>
        /// <param name="paymentModel"></param>
        /// <returns></returns>
        public GatewayResponseModel ValidateCreditcard(PaymentModel paymentModel)
        {
            bool tcoSandboxMode = true;
            if (!Equals(ConfigurationManager.AppSettings["TwoCheckOutPaymentSandboxMode"], null))
                tcoSandboxMode = bool.Parse(ConfigurationManager.AppSettings["TwoCheckOutPaymentSandboxMode"].ToString());

            TwoCheckoutConfig.SandboxUrl = ConfigurationManager.AppSettings["TwoCOCheckoutPath"];
            TwoCheckoutConfig.SellerID = paymentModel.Vendor;
            TwoCheckoutConfig.PrivateKey = paymentModel.GatewayTransactionKey;
            TwoCheckoutConfig.Sandbox = tcoSandboxMode;

            if (!string.IsNullOrEmpty(paymentModel.TransactionId))
                return Capture(paymentModel);

            try
            {
                AuthBillingAddress Billing = new AuthBillingAddress();
                Billing.addrLine1 = paymentModel.BillingStreetAddress1;
                Billing.city = paymentModel.BillingCity;
                Billing.zipCode = paymentModel.BillingPostalCode;
                Billing.state = paymentModel.BillingStateCode;
                Billing.country = paymentModel.BillingCountryCode;
                Billing.name = paymentModel.BillingFirstName;
                Billing.email = paymentModel.BillingEmailId;
                Billing.phoneNumber = paymentModel.BillingPhoneNumber;

                ChargeAuthorizeServiceOptions Customer = new ChargeAuthorizeServiceOptions();
                Customer.total = decimal.Parse(paymentModel.Total);
                Customer.currency = string.IsNullOrEmpty(paymentModel.GatewayCurrencyCode) ? Convert.ToString(ConfigurationManager.AppSettings["CurrencyCode"]) : paymentModel.GatewayCurrencyCode;
                Customer.merchantOrderId = paymentModel.OrderId;
                Customer.billingAddr = Billing;
                Customer.token = paymentModel.TwoCOToken;
                if (paymentModel.Subscriptions.Count > 0)
                {
                    Customer.lineItems = new System.Collections.Generic.List<AuthLineitem>();
                    foreach (SubscriptionModel subscription in paymentModel.Subscriptions)
                    {
                        AuthLineitem lineitem = new AuthLineitem();

                        lineitem.recurrence = subscription.Frequency + " " + subscription.Period.ToUpper();
                        lineitem.duration = subscription.TotalCycles.ToString() + " " + subscription.Period.ToUpper(); // "Forever";                        
                        lineitem.name = subscription.ProfileName;
                        lineitem.price = subscription.Amount;

                        Customer.lineItems.Add(lineitem);
                    }

                }
                ChargeService Charge = new ChargeService();

                Authorization result = Charge.Authorize(Customer);
                if (result.responseCode.Equals("APPROVED"))
                    return new GatewayResponseModel { IsSuccess = true, TransactionId = result.transactionId.ToString(), Token = result.orderNumber.ToString(), PaymentStatus = ZnodePaymentStatus.CAPTURED };
                else
                    return new GatewayResponseModel { IsSuccess = false, GatewayResponseData = result.responseMsg };
            }
            catch (TwoCheckoutException e)
            {
                return new GatewayResponseModel { IsSuccess = false, GatewayResponseData = e.message };
            }
        }

        public TransactionDetailsModel GetTransactionDetails(PaymentModel paymentModel)
        {
            return new TransactionDetailsModel();
        }

        /// <summary>
        /// This method is used to capture the TwoCheckOut Payment
        /// </summary>
        /// <param name="paymentModel">payment model</param>
        /// <returns>response of the capture method</returns>
        private GatewayResponseModel Capture(PaymentModel paymentModel)
        {
            TwoCheckoutConfig.ApiUsername = paymentModel.GatewayLoginName;
            TwoCheckoutConfig.ApiPassword = paymentModel.GatewayLoginPassword;
            TwoCheckoutConfig.SandboxUrl = ConfigurationManager.AppSettings["TwoCOCheckoutPath"];
            TwoCheckoutConfig.SellerID = paymentModel.Vendor;
            TwoCheckoutConfig.PrivateKey = paymentModel.GatewayTransactionKey;
            TwoCheckoutConfig.Sandbox = true;

            SaleService ServiceObject = new SaleService();
            SaleShipServiceOptions ArgsObject = new SaleShipServiceOptions();
            ArgsObject.sale_id = long.Parse(paymentModel.OrderId);
            ArgsObject.tracking_number = "test";
            TwoCheckoutResponse result = ServiceObject.Ship(ArgsObject);

            if (result.response_code.Equals("APPROVED"))
                return new GatewayResponseModel { IsSuccess = true, TransactionId = paymentModel.TransactionId, PaymentStatus = ZnodePaymentStatus.CAPTURED };
            return new GatewayResponseModel { IsSuccess = false, GatewayResponseData = "Unable to Capture" };
        }

        /// <summary>
        /// This method is used to refund the TwoCheckOut Payment
        /// </summary>
        /// <param name="paymentModel">payment model</param>
        /// <returns>response of the refund method</returns>
        public GatewayResponseModel Refund(PaymentModel paymentModel)
        {
            TwoCheckoutConfig.SandboxUrl = System.Configuration.ConfigurationManager.AppSettings["TwoCOCheckoutPath"];
            TwoCheckoutConfig.SellerID = paymentModel.Vendor;
            TwoCheckoutConfig.PrivateKey = paymentModel.GatewayTransactionKey;

            TwoCheckoutConfig.ApiUsername = paymentModel.GatewayLoginName;
            TwoCheckoutConfig.ApiPassword = paymentModel.GatewayLoginPassword;
            TwoCheckoutConfig.Sandbox = true;
            try
            {
                SaleService ServiceObject = new SaleService();
                SaleRefundServiceOptions ArgsObject = new SaleRefundServiceOptions();
                ArgsObject.invoice_id = long.Parse(paymentModel.TransactionId);
                ArgsObject.sale_id = long.Parse(paymentModel.OrderId);
                ArgsObject.amount = decimal.Parse(paymentModel.Total);
                ArgsObject.currency = string.IsNullOrEmpty(paymentModel.GatewayCurrencyCode) ? Convert.ToString(ConfigurationManager.AppSettings["CurrencyCode"])?.ToLower() : paymentModel.GatewayCurrencyCode.ToLower();
                ArgsObject.comment = "Refund";
                ArgsObject.category = 5;

                TwoCheckoutResponse result = ServiceObject.Refund(ArgsObject);
                if (result.response_code.Equals("OK"))
                    return new GatewayResponseModel { IsSuccess = true, TransactionId = paymentModel.TransactionId, PaymentStatus = ZnodePaymentStatus.REFUNDED };
                return new GatewayResponseModel { IsSuccess = false, GatewayResponseData = "Error" };
            }
            catch (TwoCheckoutException e)
            {
                return new GatewayResponseModel { IsSuccess = false, GatewayResponseData = e.Message };
            }
        }

        /// <summary>
        /// This method is used to void the TwoCheckOut Payment.
        /// </summary>
        /// <param name="paymentModel">payment model</param>
        /// <returns>response of the void method</returns>
        public GatewayResponseModel Void(PaymentModel paymentModel)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// This method is used for the Subscription payment in the TwoCheckout.
        /// </summary>
        /// <param name="paymentModel">payment model</param>
        /// <returns>response of the void method</returns>
        public GatewayResponseModel Subscription(PaymentModel paymentModel)
        {
            throw new NotImplementedException();
        }

        public PaymentGatewayTokenModel TokenGenerator(PaymentGatewayTokenModel paymentModel)
        {
            throw new NotImplementedException();
        }
    }
}