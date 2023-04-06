using PayPal;
using PayPal.Api;
using System;
using System.Collections.Generic;
using Znode.Multifront.PaymentApplication.Models;

namespace Znode.Libraries.PaypalRest
{
    /// <summary>
    /// This class will deal with the Paypal Rest API.
    /// </summary>
    public class PaypalRestApi
    {
        /// <summary>
        /// Stores the Credit card details into Paypal Vault.
        /// </summary>
        /// <param name="paymentModel"></param>
        /// <returns></returns>
        public GatewayResponseModel StoreCreditCardInPaypal(PaymentModel paymentModel)
        {
            CreditCard creditCard = MapToCreditCard(paymentModel);
            GatewayResponseModel gatewayResponseModel = new GatewayResponseModel();
            try
            {
                //Getting the API Context to authenticate the call to Paypal Server
                APIContext apiContext = Configuration.GetAPIContext();
                // Storing the Credit Card Info in the PayPal Vault Server
                CreditCard createdCreditCard = creditCard.Create(apiContext);

                //Saving the User's Credit Card ID returned by the PayPal
                //You can use this ID for future payments via User's Credit Card
                if (createdCreditCard.state.Equals("ok"))
                {
                    paymentModel.CustomerProfileId = createdCreditCard.id;
                    return CreateTransaction(paymentModel);
                }
            }
            catch (PayPalException ex)
            {
                gatewayResponseModel.IsSuccess = false;
                if (ex.InnerException is ConnectionException)
                {
                    gatewayResponseModel.IsSuccess = false;
                    gatewayResponseModel.GatewayResponseData = ((ConnectionException)ex.InnerException).Response;
                }
                else
                {
                    gatewayResponseModel.GatewayResponseData = (ex.Message);
                }
            }
            catch (Exception ex)
            {
                gatewayResponseModel.IsSuccess = false;
                gatewayResponseModel.GatewayResponseData = ex.Message;
            }
            return gatewayResponseModel;
        }

        /// <summary>
        /// Create a transaction(Authorize or capture transaction)
        /// </summary>
        /// <param name="paymentModel"></param>
        /// <returns></returns>
        public GatewayResponseModel CreateTransaction(PaymentModel paymentModel)
        {
            APIContext apiContext = Configuration.GetAPIContext();
            GatewayResponseModel gatewayResponse = new GatewayResponseModel();
            if (paymentModel.IsCapture)
                return CapturePayment(paymentModel);

            else
            {
                //Check subtotal is greater than zero or not after deducting discounts.
                bool isSubTotal = false;
                // A resource representing a credit card that can be used to fund a payment.
                CreditCardToken creditCardToken = new CreditCardToken()
                {
                    credit_card_id = paymentModel.CustomerPaymentProfileId
                };

                if (string.IsNullOrEmpty(paymentModel.TaxCost))
                {
                    paymentModel.TaxCost = "0";
                }

                if (string.IsNullOrEmpty(paymentModel.ShippingCost))
                {
                    paymentModel.ShippingCost = "0";
                }

                //Match the total=subtotal+shippingcost+tax
                MapTransactionAmount(paymentModel, out isSubTotal);
                Amount amount = new Amount();
                if (isSubTotal)
                {
                    amount = new Amount()
                    {
                        currency = paymentModel.GatewayCurrencyCode,
                        total = String.Format("{0:0.00}", Convert.ToDecimal(paymentModel.Total)),
                        details = new Details()
                        {
                            shipping = String.Format("{0:0.00}", Convert.ToDecimal(paymentModel.ShippingCost)),
                            subtotal = String.Format("{0:0.00}", Convert.ToDecimal(paymentModel.SubTotal)),
                            tax = String.Format("{0:0.00}", Convert.ToDecimal(paymentModel.TaxCost))
                        }
                    };
                }
                else
                {
                    amount = new Amount()
                    {
                        currency = paymentModel.GatewayCurrencyCode,
                        total = String.Format("{0:0.00}", Convert.ToDecimal(paymentModel.Total)),
                    };
                }
                
                Transaction transaction = new Transaction()
                {
                    amount = amount,
                    description = "This is the payment transaction description.",
                };

                // A resource representing a Payer's funding instrument. For stored credit card payments, set the CreditCardToken field on this object.
                FundingInstrument fundInstrument = new FundingInstrument()
                {
                    credit_card_token = creditCardToken
                };

                // A Payment Resource; create one using the above types and intent as 'sale'
                Payment payment = new Payment()
                {
                    intent = "authorize",
                    payer = new Payer()
                    {
                        funding_instruments = new List<FundingInstrument>() { fundInstrument },
                        payment_method = "credit_card"
                    },
                    transactions = new List<Transaction>() { transaction }
                };

                // Create a payment using a valid APIContext
                var createdPayment = payment.Create(apiContext);
                if (createdPayment.state.Equals("approved"))
                {
                    gatewayResponse.IsSuccess = true;
                    if (createdPayment.transactions.Count > 0 && createdPayment.transactions[0].related_resources.Count > 0)
                    {
                        gatewayResponse.TransactionId = createdPayment.transactions[0].related_resources[0].authorization.id;
                    }
                    gatewayResponse.CustomerPaymentProfileId = createdPayment.id;
                    gatewayResponse.CustomerProfileId = paymentModel.CustomerProfileId;
                    gatewayResponse.IsGatewayPreAuthorize = paymentModel.GatewayPreAuthorize;
                    gatewayResponse.PaymentStatus = ZnodePaymentStatus.AUTHORIZED;
                }
                return gatewayResponse;
            }
        }

        /// <summary>
        /// Capture the Payment by using the Credit card Id stored in the Paypal Vault.
        /// </summary>
        /// <param name="paymentModel"></param>
        /// <returns></returns>
        public GatewayResponseModel CapturePayment(PaymentModel paymentModel)
        {
            APIContext apiContext = Configuration.GetAPIContext();
            GatewayResponseModel gatewayResponse = new GatewayResponseModel();

            try
            {
                Capture capture = new Capture();
                Amount captureAmount = new Amount();
                captureAmount.currency = paymentModel.GatewayCurrencyCode;
                captureAmount.total = String.Format("{0:0.00}", Convert.ToDecimal(paymentModel.Total));
                capture.amount = captureAmount;
                capture.is_final_capture = true;

                var responseCapture = Authorization.Capture(apiContext, paymentModel.TransactionId, capture);

                if (responseCapture.state.Equals("completed"))
                {
                    gatewayResponse.IsSuccess = true;
                    gatewayResponse.TransactionId = responseCapture.id;
                    gatewayResponse.PaymentStatus = ZnodePaymentStatus.CAPTURED;
                }
            }
            catch (PayPalException ex)
            {
                gatewayResponse.GatewayResponseData = ex.InnerException is ConnectionException ? ((ConnectionException)ex.InnerException).Response : (ex.Message);
            }
            catch (Exception ex)
            {
                gatewayResponse.IsSuccess = false;
                gatewayResponse.GatewayResponseData = ex.Message;
            }
            return gatewayResponse;
        }

        /// <summary>
        /// This method is used for the refund the payment.
        /// </summary>
        /// <param name="paymentModel">payment model</param>
        /// <returns>returns the response of the refund process</returns>
        public GatewayResponseModel Refund(PaymentModel paymentModel)
        {
            APIContext apiContext = Configuration.GetAPIContext();
            var capture = Capture.Get(apiContext, paymentModel.CaptureTransactionId);
            Refund refund = new Refund();

            Amount refundAmount = new Amount();
            refundAmount.currency = paymentModel.GatewayCurrencyCode;
            refundAmount.total = paymentModel.Total;

            refund.amount = refundAmount;
            Refund responseRefund = capture.Refund(apiContext, refund);

            if (responseRefund.state.Equals("completed"))
            {
                GatewayResponseModel response = new GatewayResponseModel()
                {
                    GatewayResponseData = responseRefund.state,
                    TransactionId = responseRefund.id,
                    IsSuccess = true,
                    PaymentStatus = ZnodePaymentStatus.REFUNDED
                };
                return response;
            }
            return new GatewayResponseModel { IsSuccess = false, GatewayResponseData = "Unable to process request" };
        }

        /// <summary>
        /// This method is used for the void the payment.
        /// </summary>
        /// <param name="paymentModel"></param>
        /// <returns>returns the response of the void payment process</returns>
        public GatewayResponseModel VoidPayment(PaymentModel paymentModel)
        {
            APIContext apiContext = Configuration.GetAPIContext();
            var responseAuthorization = Authorization.Void(apiContext, paymentModel.TransactionId);

            if (responseAuthorization.state.Equals("voided"))
            {
                GatewayResponseModel response = new GatewayResponseModel()
                {
                    TransactionId = responseAuthorization.id,
                    IsSuccess = true,
                    PaymentStatus = ZnodePaymentStatus.VOIDED
                };
                return response;
            }
            return new GatewayResponseModel { IsSuccess = false, GatewayResponseData = "Unable to process request" };
        }

        /// <summary>
        /// This method will create the currency object
        /// </summary>
        /// <param name="value">value to be converted</param>
        /// <returns>returns the currency object</returns>
        private static Currency GetCurrency(string value) => new Currency() { value = value, currency = "USD" };

        /// <summary>
        /// This method will be used for the making the payment.
        /// </summary>
        /// <param name="paymentModel">payment model</param>
        /// <returns>returns the response of the card payment model</returns>
        public GatewayResponseModel CardPayment(PaymentModel paymentModel)
        {
            GatewayResponseModel gatewayResponseModel = new GatewayResponseModel();
            // ### Api Context
            // Pass in a `APIContext` object to authenticate 
            // the call and to send a unique request id 
            APIContext apiContext = Configuration.GetAPIContext();

            CreditCard creditCard = MapToCreditCard(paymentModel);
            Amount amount = new Amount();
            amount.total = paymentModel.Total;
            amount.currency = paymentModel.GatewayCurrencyCode;
            amount.details = new Details { subtotal = paymentModel.SubTotal, tax = paymentModel.TaxCost, shipping = paymentModel.ShippingCost };

            Transaction transaction = new Transaction();
            transaction.amount = amount;
            transaction.description = "Card payment description.";

            List<Transaction> transactions = new List<Transaction>();
            transactions.Add(transaction);

            FundingInstrument fundingInstrument = new FundingInstrument();
            fundingInstrument.credit_card = creditCard;

            List<FundingInstrument> fundingInstruments = new List<FundingInstrument>();
            fundingInstruments.Add(fundingInstrument);

            Payer payer = new Payer();
            payer.funding_instruments = fundingInstruments;
            payer.payment_method = "credit_card";

            Payment payment = new Payment();
            payment.intent = "sale";
            payment.payer = payer;
            payment.transactions = transactions;

            try
            {
                // Create a payment using a valid APIContext
                Payment createdPayment = payment.Create(apiContext);
                if (createdPayment.state.Equals("ok") || createdPayment.state.Equals("completed") || createdPayment.state.Equals("approved"))
                {
                    gatewayResponseModel.IsSuccess = true;
                    gatewayResponseModel.CardAuthCode = createdPayment.id;
                    if (createdPayment.transactions.Count > 0 && createdPayment.transactions[0].related_resources.Count > 0)
                    {
                        gatewayResponseModel.TransactionId = createdPayment.transactions[0].related_resources[0].sale.id;
                    }
                }
            }
            catch (Exception ex)
            {
                gatewayResponseModel.IsSuccess = false;
                gatewayResponseModel.GatewayResponseData = ex.Message;
            }
            return gatewayResponseModel;
        }

        /// <summary>
        /// This method will create the billing plan
        /// </summary>
        /// <param name="subscriptionModel"></param>
        /// <returns>returns the billing plan response</returns>
        private Plan CreateBillingPlan(SubscriptionModel subscriptionModel)
        {
            return new Plan
            {
                name = subscriptionModel.ProfileName,
                description = subscriptionModel.InvoiceNo,
                type = "fixed",
                merchant_preferences = new MerchantPreferences()
                {
                    setup_fee = GetCurrency(subscriptionModel.Amount.ToString()),
                    auto_bill_amount = "YES",
                    initial_fail_amount_action = "CONTINUE",
                    max_fail_attempts = "0",
                    return_url = "http://localhost.test.com/",
                    cancel_url = "http://localhost.test.com/?cancel",
                },
                payment_definitions = new List<PaymentDefinition>
                {                  
                    // Define the standard payment plan. It will represent a monthly
                    // plan for $19.99 USD that charges once month for 11 months.
                    new PaymentDefinition
                    {
                        name = "Standard Plan",
                        type = "REGULAR",
                        frequency = subscriptionModel.Period,
                        frequency_interval = subscriptionModel.Frequency,
                        amount = GetCurrency(subscriptionModel.Amount.ToString()),
                        cycles = subscriptionModel.TotalCycles.ToString(),
                    }
                }
            };
        }

        /// <summary>
        /// This method will create the billing agreement.
        /// </summary>
        /// <param name="paymentModel">payment model</param>
        /// <returns>returns the billing agreement response</returns>
        private GatewayResponseModel CreateBillingAgreement(PaymentModel paymentModel)
        {
            var plan = CreateBillingPlan(paymentModel.Subscription);
            string guid = Convert.ToString((new Random()).Next(100000));

            APIContext apiContext = Configuration.GetAPIContext();

            var createdPlan = plan.Create(apiContext);

            // Activate the plan
            PatchRequest patchRequest = new PatchRequest()
            {
                new Patch()
                {
                    op = "replace",
                    path = "/",
                    value = new Plan() { state = "ACTIVE" }
                }
            };

            createdPlan.Update(apiContext, patchRequest);

            // With the plan created and activated, we can now create the billing agreement.
            Payer payer = new Payer
            {
                payment_method = "credit_card",
                funding_instruments = new List<FundingInstrument>
                {
                    new FundingInstrument
                    {
                        credit_card=MapToCreditCard(paymentModel)
                    }
                }
            };

            Agreement agreement = new Agreement()
            {
                name = paymentModel.Subscription.ProfileName,
                description = paymentModel.Subscription.InvoiceNo,
                payer = payer,
                plan = new Plan() { id = createdPlan.id },
                start_date = DateTime.UtcNow.AddDays(1).ToString("yyyy-MM-ddTHH:mm:ssZ")
            };

            // Create the billing agreement.
            Agreement createdAgreement = agreement.Create(apiContext);
            return new GatewayResponseModel { IsSuccess = true, TransactionId = createdAgreement.id };
        }

        /// <summary>
        /// This method will create the Subscription
        /// </summary>
        /// <param name="paymentModel">payment model</param>
        /// <returns>returns the create subscription response</returns>
        public GatewayResponseModel CreateSubscription(PaymentModel paymentModel) => CreateBillingAgreement(paymentModel);

        /// <summary>
        /// This method will create customer using credit card detail
        /// </summary>
        /// <param name="paymentModel">PaymentModel paymentModel</param>
        /// <returns>returns paypal response</returns>
        public GatewayResponseModel CreateCustomerInPaypal(PaymentModel paymentModel)
        {
            //Creating the CreditCard Object and assigning values
            CreditCard creditCard = MapToCreditCard(paymentModel, true);
            GatewayResponseModel gatewayResponseModel = new GatewayResponseModel();
            try
            {
                //Getting the API Context to authenticate the call to Paypal Server
                APIContext apiContext = Configuration.GetAPIContext();
                // Storing the Credit Card Info in the PayPal Vault Server
                CreditCard createdCreditCard = creditCard.Create(apiContext);

                //Saving the User's Credit Card ID returned by the PayPal
                //You can use this ID for future payments via User's Credit Card
                if (createdCreditCard.state.Equals("ok"))
                {
                    paymentModel.CustomerProfileId = creditCard.external_customer_id;
                    paymentModel.CustomerPaymentProfileId = createdCreditCard.id;
                    gatewayResponseModel.CustomerPaymentProfileId = createdCreditCard.id;
                    gatewayResponseModel.CustomerProfileId = creditCard.external_customer_id;
                    gatewayResponseModel.IsSuccess = true;
                    return gatewayResponseModel;
                }
                else
                {
                    gatewayResponseModel.IsSuccess = false;
                }
            }
            catch (PayPalException ex)
            {
                gatewayResponseModel.IsSuccess = false;
                if (ex.InnerException is ConnectionException)
                {
                    gatewayResponseModel.IsSuccess = false;
                    gatewayResponseModel.GatewayResponseData = ((ConnectionException)ex.InnerException).Response;
                }
                else
                {
                    gatewayResponseModel.GatewayResponseData = (ex.Message);
                }
            }
            catch (Exception ex)
            {
                gatewayResponseModel.IsSuccess = false;
                gatewayResponseModel.GatewayResponseData = ex.Message;
            }
            return gatewayResponseModel;
        }

        //Map PaymentModel to CreditCard
        private CreditCard MapToCreditCard(PaymentModel paymentModel, bool mapExternal_customer_id = false)
        {
            //Creating the CreditCard Object and assigning values
            CreditCard creditCard = new CreditCard();
            creditCard.expire_month = int.Parse(paymentModel.CardExpirationMonth);
            creditCard.expire_year = int.Parse(paymentModel.CardExpirationYear);
            creditCard.number = paymentModel.CardNumber;
            creditCard.type = paymentModel.CardType.ToLower();
            creditCard.cvv2 = paymentModel.CardSecurityCode;
            creditCard.external_customer_id = mapExternal_customer_id ?
                (String.IsNullOrEmpty(paymentModel.CustomerProfileId) ?
                Convert.ToString((new Random()).Next(100000000, 999999999)) :
                paymentModel.CustomerProfileId) :
                string.Empty;
            creditCard.first_name = paymentModel.BillingFirstName;
            creditCard.last_name = paymentModel.BillingLastName;
            creditCard.billing_address = new Address
            {
                city = paymentModel.BillingCity,
                country_code = paymentModel.BillingCountryCode,
                line1 = paymentModel.BillingStreetAddress1,
                line2 = paymentModel.BillingStreetAddress2,
                phone = paymentModel.BillingPhoneNumber,
                postal_code = paymentModel.BillingPostalCode,
                state = paymentModel.BillingStateCode
            };
            return creditCard;
        }

        //Match the total=subtotal+shippingcost+tax
        protected virtual void MapTransactionAmount(PaymentModel paymentModel, out bool isSubTotal)
        {
            decimal subTotal = Convert.ToDecimal(paymentModel.SubTotal);
            decimal discount = Convert.ToDecimal(paymentModel.Discount);
            decimal giftCardAmount = Convert.ToDecimal(paymentModel.GiftCardAmount);
            decimal csrDiscountAmount = Convert.ToDecimal(paymentModel.CSRDiscountAmount);
            decimal shippingCost = Convert.ToDecimal(paymentModel.ShippingCost);
            decimal shippingHandlingCharges = Convert.ToDecimal(paymentModel.ShippingHandlingCharges);
            decimal shippingDiscount = Convert.ToDecimal(paymentModel.ShippingDiscount);
            decimal calculatedSubtotal = subTotal - discount - giftCardAmount - csrDiscountAmount;
            decimal calculatedShippingCost = shippingCost + shippingHandlingCharges - shippingDiscount;

            if (subTotal > discount + giftCardAmount + csrDiscountAmount)
            {
                paymentModel.SubTotal = calculatedSubtotal == subTotal ? paymentModel.SubTotal : String.Format("{0:0.00}", subTotal - discount - giftCardAmount - csrDiscountAmount);
                paymentModel.ShippingCost = calculatedShippingCost == shippingCost ? paymentModel.ShippingCost : String.Format("{0:0.00}", shippingCost + shippingHandlingCharges - shippingDiscount);
                isSubTotal = true;
            }
            else
            {
                isSubTotal = false;
            }
        }
    }
}