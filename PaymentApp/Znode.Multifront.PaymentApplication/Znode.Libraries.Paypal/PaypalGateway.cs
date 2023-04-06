using com.paypal.sdk.profiles;
using com.paypal.sdk.services;
using com.paypal.soap.api;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Linq;
using Znode.Multifront.PaymentApplication.Helpers;
using Znode.Multifront.PaymentApplication.Models;

namespace Znode.Libraries.Paypal
{
    /// <summary>
    /// PayPal Gateway Implementation 
    /// </summary>
    public class PaypalGateway
    {
        #region Private Member Variables
        /// <summary>
        /// Read only Instance object for Call Services 
        /// </summary>
        private readonly CallerServices caller;

        private string _ECRedirectUrl = string.Empty;
        private string _ECCancelUrl = string.Empty;
        private string _paymentActionTypeCode = "Sale";

        private PaymentModel paymentModel = new PaymentModel();
        #endregion

        #region Public properties

        /// <summary>
        /// Gets or sets the payment action type code which Specifies whether the transaction is a Sale, an Authorization, or an Order
        /// Use Sale (authorize and capture) or Authorize (authorize only) mode when processing payments.
        /// </summary>
        public string PaymentActionTypeCode
        {
            get { return this._paymentActionTypeCode; }
            set { this._paymentActionTypeCode = value; }
        }

        #endregion

        #region Protected properties

        /// <summary>
        /// Gets or set the paypal server url to post request
        /// </summary>
        private string PaypalServerUrl
        {
            get
            {
                return paymentModel.GatewayTestMode ? Convert.ToString(ConfigurationManager.AppSettings["PaypalSandboxURL"]) // Test environment URL
                    : Convert.ToString(ConfigurationManager.AppSettings["PaypalLiveURL"]); // Production Site URL
            }
        }

        #endregion

        #region Public Methods

        /// <summary>
        /// Initializes a new instance of the PaypalGateway class.
        /// </summary>
        /// <param name="PaymentSetting">Payment Setting</param>
        /// <param name="ECRedirectUrl">Redirect URL</param>
        /// <param name="ECReturnUrl">Return URL</param>
        /// <param name="addressCartItems"></param>
        public PaypalGateway(PaymentModel paymentModel)
        {
            this.paymentModel = paymentModel;
            this.caller = new CallerServices();

            string gatewayLoginID = paymentModel.GatewayLoginName;
            string gatewayPassword = paymentModel.GatewayLoginPassword;
            string transactionKey = paymentModel.GatewayTransactionKey;
            bool IsTestMode = paymentModel.GatewayTestMode;
            this.caller.APIProfile = CreateAPIProfile(gatewayLoginID, gatewayPassword, transactionKey, IsTestMode);
            this._ECCancelUrl = paymentModel.CancelUrl;
            this._ECRedirectUrl = paymentModel.ReturnUrl;
        }

        /// <summary>
        /// Method initiate an Express Checkout transaction
        /// </summary>
        /// <returns>Returns GatewayResponse object</returns>
        public PaypalResponse DoPaypalExpressCheckout(PaymentModel paymentModel)
        {
            PaypalResponse paypalResponse = new PaypalResponse();
            try
            {
                // Create the request object
                SetExpressCheckoutRequestType expCheckoutRequest = new SetExpressCheckoutRequestType();

                // Create the request details object
                expCheckoutRequest.SetExpressCheckoutRequestDetails = new SetExpressCheckoutRequestDetailsType();
                expCheckoutRequest.SetExpressCheckoutRequestDetails.PaymentAction = (PaymentActionCodeType)Enum.Parse(typeof(PaymentActionCodeType), this._paymentActionTypeCode);
                expCheckoutRequest.SetExpressCheckoutRequestDetails.PaymentActionSpecified = true;
                if (string.IsNullOrEmpty(paymentModel.TaxCost))
                    paymentModel.TaxCost = "0.0";
                if (string.IsNullOrEmpty(paymentModel.ShippingCost))
                    paymentModel.ShippingCost = "0.0";
                if (string.IsNullOrEmpty(paymentModel.Discount))
                    paymentModel.Discount = "0.0";
                if (string.IsNullOrEmpty(paymentModel.ShippingHandlingCharges))
                    paymentModel.ShippingHandlingCharges = "0.0";
                if (string.IsNullOrEmpty(paymentModel.ShippingDiscount))
                    paymentModel.ShippingDiscount = "0.0";
                // Set Number of Recurring Payments Profile to be created
                int count = paymentModel.Subscriptions.Count();

                BillingAgreementDetailsType[] BADetailsArray = new BillingAgreementDetailsType[count];
                int index = 0;
                foreach (var Item in paymentModel.Subscriptions)
                {
                    BillingAgreementDetailsType BADetailsType = new BillingAgreementDetailsType();
                    BADetailsType.BillingType = BillingCodeType.RecurringPayments;
                    BADetailsType.BillingAgreementDescription = Item.ProfileName;
                    BADetailsArray.SetValue(BADetailsType, index++);
                }

                if (!Equals(paymentModel, null) && string.IsNullOrEmpty(paymentModel.GatewayCurrencyCode))
                    paymentModel.GatewayCurrencyCode = Convert.ToString(ConfigurationManager.AppSettings["CurrencyCode"]);

                List<PaymentDetailsItemType> paypalItems = GetCartItemDetails(paymentModel);

                Decimal orderTotal = Decimal.Parse(paymentModel.TaxCost) + Decimal.Parse(paymentModel.SubTotal) + Decimal.Parse(paymentModel.ShippingCost) + Decimal.Parse(paymentModel.ShippingHandlingCharges) - Decimal.Parse(paymentModel.ShippingDiscount);
                if (!string.IsNullOrEmpty(paymentModel.Discount) && Convert.ToDecimal(paymentModel.Discount) > 0)
                    orderTotal = orderTotal - Decimal.Parse(paymentModel.Discount);
                if (!string.IsNullOrEmpty(paymentModel.GiftCardAmount) && Convert.ToDecimal(paymentModel.GiftCardAmount) > 0)
                    orderTotal = orderTotal - Decimal.Parse(paymentModel.GiftCardAmount);

                PaymentDetailsType paymentDetails = new PaymentDetailsType();
                paymentDetails.ItemTotal = this.GetSubTotal(paypalItems.ToArray());
                paymentDetails.ShippingTotal = this.GetAmount(Decimal.Parse(paymentModel.ShippingCost));
                paymentDetails.TaxTotal = this.GetAmount(Decimal.Parse(paymentModel.TaxCost));
                paymentDetails.ShippingDiscount = this.GetAmount(Decimal.Parse(paymentModel.ShippingDiscount));
                paymentDetails.HandlingTotal = this.GetAmount(Decimal.Parse(paymentModel.ShippingHandlingCharges));
                paymentDetails.OrderTotal = this.GetAmount(orderTotal);
                paymentDetails.PaymentDetailsItem = paypalItems.ToArray();
                paymentDetails.ButtonSource = ConfigurationManager.AppSettings["PaypalBNCode"];

                paymentDetails.InvoiceID = paymentModel.OrderId;
                expCheckoutRequest.SetExpressCheckoutRequestDetails.PaymentDetails = new PaymentDetailsType[] { paymentDetails };

                SetBillingShippingDetails(expCheckoutRequest, paymentModel);

                expCheckoutRequest.SetExpressCheckoutRequestDetails.BillingAgreementDetails = BADetailsArray;
                expCheckoutRequest.SetExpressCheckoutRequestDetails.CancelURL = this._ECCancelUrl;
                expCheckoutRequest.SetExpressCheckoutRequestDetails.ReturnURL = this._ECRedirectUrl;

                SetExpressCheckoutResponseType ECResponsetype = (SetExpressCheckoutResponseType)this.caller.Call("SetExpressCheckout", expCheckoutRequest);

                if (ECResponsetype.Ack.Equals(AckCodeType.Success) || ECResponsetype.Ack.Equals(AckCodeType.SuccessWithWarning))
                {
                    paypalResponse.PayalToken = ECResponsetype.Token;
                    string hostURL = paypalResponse.HostUrl;
                    paypalResponse.HostUrl = String.Format(hostURL, this.PaypalServerUrl, ECResponsetype.Token);
                    paypalResponse.ResponseCode = "0";
                    Logging.LogMessage($"Paypal Express checkout SetExpressCheckout called successfully. Paypal Response HostUrl is { paypalResponse.HostUrl} ", Logging.Components.Payment.ToString(), TraceLevel.Info);
                }
                else
                {
                    paypalResponse.ResponseCode = ECResponsetype.Errors[0].ErrorCode;
                    paypalResponse.ResponseText = (decimal.Parse(paymentDetails.ItemTotal.Value).Equals(0) && paypalResponse.ResponseCode.Equals("10413")) ?
                        "Sum of all product(s) amount should be greater than discount amount." :
                        paypalResponse.ResponseCode.Equals("10736") ? "The shipping address and PayPal address does not match." : ECResponsetype.Errors[0].LongMessage;
                    Logging.LogMessage($"Paypal Express checkout SetExpressCheckout failed to call. Response Text: {paypalResponse.ResponseText}  Response Code: {paypalResponse.ResponseCode}", Logging.Components.Payment.ToString(), TraceLevel.Error);
                }
            }
            catch (Exception ex)
            {
                paypalResponse.ResponseText = ex.Message;
                paypalResponse.ResponseCode = "-1";
            }

            return paypalResponse;
        }

        /// <summary>
        /// Get information about an Express Checkout transaction
        /// </summary>
        /// <param name="token">Token of the checkout</param>
        /// <returns>Paypal Response</returns>
        public PaypalResponse GetExpressCheckoutDetails(string token)
        {
            // Create the request object
            GetExpressCheckoutDetailsRequestType expCheckoutRequest = new GetExpressCheckoutDetailsRequestType();
            PaypalResponse paypalResponse = new PaypalResponse();

            expCheckoutRequest.Token = token;

            GetExpressCheckoutDetailsResponseType ECDetailsResponse = (GetExpressCheckoutDetailsResponseType)this.caller.Call("GetExpressCheckoutDetails", expCheckoutRequest);

            if (ECDetailsResponse.Ack.Equals(AckCodeType.SuccessWithWarning) || ECDetailsResponse.Ack.Equals(AckCodeType.Success))
            {
                paypalResponse.PayerID = ECDetailsResponse.GetExpressCheckoutDetailsResponseDetails.PayerInfo.PayerID;
                paypalResponse.ResponseCode = "0";
                Logging.LogMessage($"Paypal Express checkout GetExpressCheckoutDetails called successfully. Payer Id=  {paypalResponse.PayerID} ", Logging.Components.Payment.ToString(), TraceLevel.Info);
            }
            else
            {
                paypalResponse.ResponseText = ECDetailsResponse.Errors[0].LongMessage;
                paypalResponse.ResponseCode = ECDetailsResponse.Errors[0].ErrorCode;
                Logging.LogMessage($"Paypal Express checkout GetExpressCheckoutDetails failed to call. Response Text: { paypalResponse.ResponseText}  Response Code: {paypalResponse.ResponseCode} ", Logging.Components.Payment.ToString(), TraceLevel.Error);
            }

            return paypalResponse;
        }

        /// <summary>
        /// This method is used for the Refund the payment.
        /// </summary>
        /// <param name="paymentModel">Payment model</param>
        /// <returns>Response of the refund functionality</returns>
        public GatewayResponseModel Refund(PaymentModel paymentModel)
        {
            // Create the request object
            RefundTransactionRequestType refundRequest = new RefundTransactionRequestType();
            GatewayResponseModel paypalResponse = new GatewayResponseModel();

            refundRequest.Amount = GetAmount(decimal.Parse(paymentModel.Total));
            refundRequest.TransactionID = paymentModel.TransactionId;

            var requestResponse = (RefundTransactionResponseType)this.caller.Call("RefundTransaction", refundRequest);

            if (requestResponse.Ack.Equals(AckCodeType.SuccessWithWarning) || requestResponse.Ack.Equals(AckCodeType.Success))
            {
                paypalResponse.TransactionId = requestResponse.RefundTransactionID;
                paypalResponse.IsSuccess = true;
                paypalResponse.ResponseCode = "0";
            }
            else
            {
                paypalResponse.IsSuccess = false;
                paypalResponse.GatewayResponseData = requestResponse.Errors[0].LongMessage;
                paypalResponse.ResponseCode = requestResponse.Errors[0].ErrorCode;
            }

            return paypalResponse;
        }

        /// <summary>
        /// This method is used to void the payments done earlier
        /// </summary>
        /// <param name="paymentModel">payment model</param>
        /// <returns>returns the response of the void method from paypal</returns>
        public GatewayResponseModel Void(PaymentModel paymentModel)
        {
            // Create the request object
            DoVoidRequestType voidRequest = new DoVoidRequestType();
            GatewayResponseModel paypalResponse = new GatewayResponseModel();

            voidRequest.AuthorizationID = paymentModel.TransactionId;
            var requestResponse = (DoVoidResponseType)this.caller.Call("DoVoid", voidRequest);
            if (requestResponse.Ack.Equals(AckCodeType.SuccessWithWarning) || requestResponse.Ack.Equals(AckCodeType.Success))
            {
                paypalResponse.TransactionId = requestResponse.AuthorizationID;

                paypalResponse.IsSuccess = true;
                paypalResponse.ResponseCode = "0";
            }
            else
            {
                paypalResponse.IsSuccess = false;
                paypalResponse.GatewayResponseData = requestResponse.Errors[0].LongMessage;
                paypalResponse.ResponseCode = requestResponse.Errors[0].ErrorCode;
            }

            return paypalResponse;
        }

        /// <summary>
        /// Complete an Express Checkout transaction. Returns ZNodePaymentResponse object
        /// </summary>
        /// <param name="token">token of the express checkout</param>
        /// <param name="payerID">ID of the payer</param>
        /// <returns>returns the payment response</returns>
        public GatewayResponseModel DoExpressCheckoutPayment(string token, string payerID)
        {
            // Create the request object
            DoExpressCheckoutPaymentRequestType expCheckoutRequest = new DoExpressCheckoutPaymentRequestType();
            PaypalResponse response = new PaypalResponse();
            GatewayResponseModel payment_response = new GatewayResponseModel();

            CurrencyCodeType currencyCodeTypeId = (CurrencyCodeType)Enum.Parse(typeof(CurrencyCodeType), this.paymentModel.GatewayCurrencyCode);

            // Create the request details object
            expCheckoutRequest.DoExpressCheckoutPaymentRequestDetails = new DoExpressCheckoutPaymentRequestDetailsType();
            expCheckoutRequest.DoExpressCheckoutPaymentRequestDetails.Token = token;
            expCheckoutRequest.DoExpressCheckoutPaymentRequestDetails.PayerID = payerID;
            expCheckoutRequest.DoExpressCheckoutPaymentRequestDetails.PaymentAction = (PaymentActionCodeType)Enum.Parse(typeof(PaymentActionCodeType), this._paymentActionTypeCode);


            PaymentDetailsType paymentDetails = new PaymentDetailsType();
            paymentDetails.ItemTotal = this.GetAmount(Decimal.Parse(paymentModel.SubTotal));
            paymentDetails.ShippingTotal = this.GetAmount(Decimal.Parse(paymentModel.ShippingCost));
            paymentDetails.TaxTotal = this.GetAmount(Decimal.Parse(paymentModel.TaxCost));
            paymentDetails.HandlingTotal = this.GetAmount(decimal.Parse(paymentModel.ShippingHandlingCharges));
            paymentDetails.OrderTotal = this.GetAmount(decimal.Parse(paymentModel.Total));
            paymentDetails.InvoiceID = paymentModel.OrderId;
            paymentDetails.ButtonSource = ConfigurationManager.AppSettings["PaypalBNCode"];                 
            //We are assigning the discounted price and gift card discount and coupon code discount into shipping discount
            //For that we used ShippingDiscount property.
            paymentDetails.ShippingDiscount = GetGiftDiscountAmount(paymentModel.Discount, paymentModel.GiftCardAmount, paymentModel.ShippingDiscount);

            expCheckoutRequest.DoExpressCheckoutPaymentRequestDetails.PaymentDetails = new PaymentDetailsType[] { paymentDetails };

            // Create the response object
            DoExpressCheckoutPaymentResponseType ECPaymentResponse = (DoExpressCheckoutPaymentResponseType)this.caller.Call("DoExpressCheckoutPayment", expCheckoutRequest);

            if (ECPaymentResponse.Ack.Equals(AckCodeType.Success) || ECPaymentResponse.Ack.Equals(AckCodeType.SuccessWithWarning))
            {
                Logging.LogMessage($"Paypal Express checkout DoExpressCheckoutPayment called successfully. Your Transaction Id is Started { payment_response.TransactionId} ", Logging.Components.Payment.ToString(), TraceLevel.Info);
                GetGatewayResponseModel(payment_response, ECPaymentResponse, response, token, payerID, ZnodePaymentStatus.CAPTURED);
                Logging.LogMessage($"Paypal Express checkout DoExpressCheckoutPayment called successfully. Your Transaction Id is Completed { payment_response.TransactionId} ", Logging.Components.Payment.ToString(), TraceLevel.Info);
            }

            else if (ECPaymentResponse.Ack.Equals(AckCodeType.Failure) || ECPaymentResponse.Ack.Equals(AckCodeType.FailureWithWarning))
            {
                payment_response.IsSuccess = false;
                var error = ECPaymentResponse.Errors.FirstOrDefault();
                if (!Equals(error, null))
                    payment_response.ResponseText = error.LongMessage;
                payment_response.PaymentStatus = ZnodePaymentStatus.DECLINED;
                payment_response.HasError = true;
                Logging.LogMessage($"Paypal Express checkout DoExpressCheckoutPayment failed to call.  Response text: {payment_response.ResponseText}  Payment Status:  { payment_response.PaymentStatus }", Logging.Components.Payment.ToString(), TraceLevel.Error);
            }
            else if (ECPaymentResponse.Ack.Equals(AckCodeType.Warning) || ECPaymentResponse.Ack.Equals(AckCodeType.PartialSuccess) || ECPaymentResponse.Ack.Equals(AckCodeType.CustomCode))
            {
                Logging.LogMessage($"Paypal Express checkout DoExpressCheckoutPayment called with Warning or PartialSuccess or CustomCode Started. Your Transaction Id is { payment_response?.TransactionId} ", Logging.Components.Payment.ToString(), TraceLevel.Info);
                GetGatewayResponseModel(payment_response, ECPaymentResponse, response, token, payerID, ZnodePaymentStatus.PENDINGFORREVIEW);
                Logging.LogMessage($"Paypal Express checkout DoExpressCheckoutPayment called with Warning or PartialSuccess or CustomCode Completed. Your Transaction Id is { payment_response?.TransactionId} ", Logging.Components.Payment.ToString(), TraceLevel.Info);
            }
            else
            {
                payment_response.IsSuccess = true;
                payment_response.PaymentStatus = ZnodePaymentStatus.PENDING;
            }

            return payment_response;
        }

        /// <summary>
        /// Method creates Recurring Payments Profile
        /// </summary>
        /// <param name="token">token of the payment</param>
        /// <param name="payerID">ID of the payer</param>
        /// <param name="recurringBillingInfo">Recurring bill information</param>
        /// <param name="ProfileDesc">profile description</param>
        /// <returns>Returns ZNodeSubscriptionResponse object</returns>
        public GatewayResponseModel CreateRecurringPaymentsProfile(string token, string payerID, PaypalRecurringBillingInfo recurringBillingInfo, string ProfileDesc)
        {
            // string token,string payerID, DateTime date, string amount, int BF, BillingPeriodType BP, CurrencyCodeType currencyCodeType,PaymentSetting PaymentSetting, string ECRedirectUrl, string ECReturnUrl)
            PaypalResponse response = new PaypalResponse();
            GatewayResponseModel subscription_response = new GatewayResponseModel();

            DateTime currentdate = System.DateTime.Now.Date;

            // create the request object
            CreateRecurringPaymentsProfileRequestType recPaymentRequest = new CreateRecurringPaymentsProfileRequestType();
            recPaymentRequest.Version = "51.0";

            // Add request-specific fields to the request object.
            recPaymentRequest.CreateRecurringPaymentsProfileRequestDetails = new CreateRecurringPaymentsProfileRequestDetailsType();
            recPaymentRequest.CreateRecurringPaymentsProfileRequestDetails.Token = token;
            recPaymentRequest.CreateRecurringPaymentsProfileRequestDetails.RecurringPaymentsProfileDetails = new RecurringPaymentsProfileDetailsType();
            recPaymentRequest.CreateRecurringPaymentsProfileRequestDetails.RecurringPaymentsProfileDetails.BillingStartDate = currentdate;
            recPaymentRequest.CreateRecurringPaymentsProfileRequestDetails.ScheduleDetails = new ScheduleDetailsType();
            recPaymentRequest.CreateRecurringPaymentsProfileRequestDetails.ScheduleDetails.PaymentPeriod = new BillingPeriodDetailsType();
            recPaymentRequest.CreateRecurringPaymentsProfileRequestDetails.ScheduleDetails.PaymentPeriod.Amount = new BasicAmountType();
            recPaymentRequest.CreateRecurringPaymentsProfileRequestDetails.ScheduleDetails.PaymentPeriod.Amount.Value = recurringBillingInfo.Amount.ToString("N2");
            recPaymentRequest.CreateRecurringPaymentsProfileRequestDetails.ScheduleDetails.PaymentPeriod.Amount.currencyID = (CurrencyCodeType)Enum.Parse(typeof(CurrencyCodeType), this.paymentModel.GatewayCurrencyCode);
            recPaymentRequest.CreateRecurringPaymentsProfileRequestDetails.ScheduleDetails.PaymentPeriod.BillingFrequency = int.Parse(recurringBillingInfo.Frequency.ToString());
            recPaymentRequest.CreateRecurringPaymentsProfileRequestDetails.ScheduleDetails.PaymentPeriod.BillingPeriod = (BillingPeriodType)Enum.Parse(typeof(BillingPeriodType), response.GetPaypalBillingPeriod(recurringBillingInfo.Period));
            recPaymentRequest.CreateRecurringPaymentsProfileRequestDetails.ScheduleDetails.PaymentPeriod.TotalBillingCycles = recurringBillingInfo.TotalCycles;
            recPaymentRequest.CreateRecurringPaymentsProfileRequestDetails.ScheduleDetails.Description = ProfileDesc;

            // Execute the API operation and obtain the response.
            CreateRecurringPaymentsProfileResponseType refundTransResponse = new CreateRecurringPaymentsProfileResponseType();
            refundTransResponse = (CreateRecurringPaymentsProfileResponseType)this.caller.Call("CreateRecurringPaymentsProfile", recPaymentRequest);
            if (refundTransResponse.Ack.Equals(AckCodeType.SuccessWithWarning) || refundTransResponse.Ack.Equals(AckCodeType.Success))
            {
                subscription_response.ResponseText = refundTransResponse.Ack.ToString();
                subscription_response.ReferenceNumber = refundTransResponse.CreateRecurringPaymentsProfileResponseDetails.ProfileID.ToString();
                subscription_response.IsSuccess = true;
            }
            else
            {
                subscription_response.ResponseText = refundTransResponse.Errors[0].LongMessage;
                subscription_response.ResponseCode = refundTransResponse.Errors[0].ErrorCode;
            }

            return subscription_response;
        }

        #endregion

        #region private methods

        /// <summary>
        /// Get the BasicAmountType for the amount value.
        /// </summary>
        /// <param name="amount">Amount value to format.</param>
        /// <returns></returns>
        private BasicAmountType GetAmount(decimal amount)
        {
            BasicAmountType moneyAmount = new BasicAmountType();
            moneyAmount.currencyID = (CurrencyCodeType)Enum.Parse(typeof(CurrencyCodeType), paymentModel.GatewayCurrencyCode);
            moneyAmount.Value = Math.Round(amount, 2).ToString();
            return moneyAmount;
        }


       
        /// <summary>
        /// Add gift amount and discount amount
        /// </summary>
        /// <param name="discount"></param>
        /// <param name="giftCardAmount"></param>
        /// <returns></returns>
        private BasicAmountType GetGiftDiscountAmount(string discount, string giftCardAmount,string shippingDiscount)
        {
            decimal totalGiftAmt=0;
            BasicAmountType moneyAmount = new BasicAmountType();
            moneyAmount.currencyID = (CurrencyCodeType)Enum.Parse(typeof(CurrencyCodeType), paymentModel.GatewayCurrencyCode);
            if (!string.IsNullOrEmpty(discount))
                //We are assigning the discounted price into shipping discount
                totalGiftAmt = decimal.Parse(discount);

            if (!string.IsNullOrEmpty(giftCardAmount))
                //We are assigning the discounted price into shipping discount
                totalGiftAmt = totalGiftAmt+ decimal.Parse(giftCardAmount);

            if (!string.IsNullOrEmpty(shippingDiscount))
                totalGiftAmt = totalGiftAmt + decimal.Parse(shippingDiscount);

            moneyAmount.Value = Math.Round(totalGiftAmt, 2).ToString();
            return moneyAmount;
        }

        /// <summary>
        /// Get the item sub-total value.
        /// </summary>
        /// <param name="paymentDetailsItemArray">PaymentDetailsItemType array object.</param>
        /// <returns>Returns the formatted BasicAmountType object.</returns>
        private BasicAmountType GetSubTotal(PaymentDetailsItemType[] paymentDetailsItemArray)
        {
            decimal itemSubtotal = 0;
            foreach (PaymentDetailsItemType lineItem in paymentDetailsItemArray)
                itemSubtotal += decimal.Parse(lineItem.Amount.Value) * int.Parse(lineItem.Quantity);

            return this.GetAmount(itemSubtotal);
        }

        /// <summary>
        /// Initiate Merchant details for an API
        /// </summary>
        /// <param name="apiUsername">API User Name</param>
        /// <param name="apiPassword">API Password</param>
        /// <param name="signature">API Signature</param>
        /// <param name="IsTestMode">Test mode or not</param>
        /// <returns>Returns the Profile</returns>
        private static IAPIProfile CreateAPIProfile(string apiUsername, string apiPassword, string signature, bool IsTestMode)
        {
            IAPIProfile profile = ProfileFactory.createSignatureAPIProfile();
            profile.APIUsername = apiUsername;
            profile.APIPassword = apiPassword;
            profile.APISignature = signature;
            profile.Subject = string.Empty;

            if (!IsTestMode)
                profile.Environment = "live";// Test mode is not enabled,then set Profile Environment as "live" - Production Server

            return profile;
        }

        /// <summary>
        /// To set product details for showing on paypal gateway
        /// </summary>
        /// <param name="paymentModel">PaymentModel paymentModel</param>
        /// <returns>return update List<PaymentDetailsItemType></returns>
        private List<PaymentDetailsItemType> GetCartItemDetails(PaymentModel paymentModel)
        {
            PaymentDetailsItemType item = null;
            List<PaymentDetailsItemType> paypalItems = new List<PaymentDetailsItemType>();
            if (!Equals(paymentModel.CartItems, null) && !string.IsNullOrEmpty(paymentModel.CartItems[0].ProductName))
            {
                foreach (var product in paymentModel.CartItems)
                {
                    item = new PaymentDetailsItemType();
                    item.Name = product.ProductName;
                    item.Description = product.ProductDescription;
                    item.Number = product.ProductNumber;
                    item.Quantity = GetQuantity(Convert.ToString(product.Quantity));
                    item.Amount = this.GetAmount(Equals(product.ProductAmount, null) ? 0 : product.ProductAmount);
                    paypalItems.Add(item);
                }
            }

            if (!string.IsNullOrEmpty(paymentModel.Discount) && Convert.ToDecimal(paymentModel.Discount) > 0)
            {
                // Add the discount field as shopping cart item with negative value.
                item = new PaymentDetailsItemType();
                item.Name = "Discount";
                item.Description = string.Empty;
                item.Number = string.Empty;
                item.Quantity = "1";
                item.Amount = this.GetAmount(-Convert.ToDecimal(paymentModel.Discount));
                paypalItems.Add(item);
            }

            if (!string.IsNullOrEmpty(paymentModel.GiftCardAmount) && Convert.ToDecimal(paymentModel.GiftCardAmount) > 0)
            {
                // Add the gift card amount as shopping cart item with negative value.
                item = new PaymentDetailsItemType();
                item.Name = "Gift Card";
                item.Description = string.Empty;
                item.Number = string.Empty;
                item.Quantity = "1";
                item.Amount = this.GetAmount(-Convert.ToDecimal(paymentModel.GiftCardAmount));
                paypalItems.Add(item);
            }
            return paypalItems;
        }

        /// <summary>
        /// To set Customer Billing & Shipping Addres to show on Paypal Express Checkout Page
        /// </summary>
        /// <param name="expCheckoutRequest">Set SetExpressCheckoutRequestType to current model</param>
        /// <param name="paymentModel">PaymentModel to get billing shipping details</param>
        private void SetBillingShippingDetails(SetExpressCheckoutRequestType expCheckoutRequest, PaymentModel paymentModel)
        {
            if (!Equals(paymentModel, null) && !string.IsNullOrEmpty(paymentModel.ShippingCountryCode) && !string.IsNullOrEmpty(paymentModel.BillingEmailId))
            {
                //Shipping Information
                expCheckoutRequest.SetExpressCheckoutRequestDetails.AddressOverride = "true";
                expCheckoutRequest.SetExpressCheckoutRequestDetails.Address = new AddressType();
                expCheckoutRequest.SetExpressCheckoutRequestDetails.Address.Name = string.Concat(paymentModel.ShippingFirstName, " ", paymentModel.ShippingLastName);
                expCheckoutRequest.SetExpressCheckoutRequestDetails.Address.Phone = string.IsNullOrEmpty(paymentModel.ShippingPhoneNumber) ? string.Empty : paymentModel.ShippingPhoneNumber;
                expCheckoutRequest.SetExpressCheckoutRequestDetails.Address.PostalCode = string.IsNullOrEmpty(paymentModel.ShippingPostalCode) ? string.Empty : paymentModel.ShippingPostalCode;
                expCheckoutRequest.SetExpressCheckoutRequestDetails.Address.StateOrProvince = string.IsNullOrEmpty(paymentModel.ShippingStateCode) ? string.Empty : paymentModel.ShippingStateCode;
                expCheckoutRequest.SetExpressCheckoutRequestDetails.Address.Street1 = string.IsNullOrEmpty(paymentModel.ShippingStreetAddress1) ? string.Empty : paymentModel.ShippingStreetAddress1;
                expCheckoutRequest.SetExpressCheckoutRequestDetails.Address.Street2 = string.IsNullOrEmpty(paymentModel.ShippingStreetAddress2) ? string.Empty : paymentModel.ShippingStreetAddress2;
                expCheckoutRequest.SetExpressCheckoutRequestDetails.Address.CityName = string.IsNullOrEmpty(paymentModel.ShippingCity) ? string.Empty : paymentModel.ShippingCity;
                expCheckoutRequest.SetExpressCheckoutRequestDetails.Address.CountryName = string.IsNullOrEmpty(paymentModel.ShippingCountryCode) ? string.Empty : paymentModel.ShippingCountryCode;
                expCheckoutRequest.SetExpressCheckoutRequestDetails.Address.CountrySpecified = true;

                foreach (CountryCodeType codeType in Enum.GetValues(typeof(CountryCodeType)))
                {
                    if (codeType.ToString().Equals(paymentModel.ShippingCountryCode))
                    {
                        expCheckoutRequest.SetExpressCheckoutRequestDetails.Address.Country = codeType;
                        break;
                    }
                }
                //Billing Information
                expCheckoutRequest.SetExpressCheckoutRequestDetails.BillingAddress = new AddressType();
                expCheckoutRequest.SetExpressCheckoutRequestDetails.BillingAddress.Name = string.Concat(paymentModel.BillingFirstName, " ", paymentModel.BillingLastName);
                expCheckoutRequest.SetExpressCheckoutRequestDetails.BillingAddress.Street1 = string.IsNullOrEmpty(paymentModel.BillingStreetAddress1) ? string.Empty : paymentModel.BillingStreetAddress1;
                expCheckoutRequest.SetExpressCheckoutRequestDetails.BillingAddress.Street2 = string.IsNullOrEmpty(paymentModel.BillingStreetAddress2) ? string.Empty : paymentModel.BillingStreetAddress2;
                expCheckoutRequest.SetExpressCheckoutRequestDetails.BillingAddress.PostalCode = string.IsNullOrEmpty(paymentModel.BillingPostalCode) ? string.Empty : paymentModel.BillingPostalCode;
                expCheckoutRequest.SetExpressCheckoutRequestDetails.BillingAddress.Phone = string.IsNullOrEmpty(paymentModel.BillingPhoneNumber) ? string.Empty : paymentModel.BillingPhoneNumber;
                expCheckoutRequest.SetExpressCheckoutRequestDetails.BillingAddress.StateOrProvince = string.IsNullOrEmpty(paymentModel.BillingStateCode) ? string.Empty : paymentModel.BillingStateCode;
                expCheckoutRequest.SetExpressCheckoutRequestDetails.BillingAddress.CityName = string.IsNullOrEmpty(paymentModel.BillingCity) ? string.Empty : paymentModel.BillingCity;

                foreach (CountryCodeType codeType in Enum.GetValues(typeof(CountryCodeType)))
                {
                    if (codeType.ToString().Equals(paymentModel.BillingCountryCode))
                    {
                        expCheckoutRequest.SetExpressCheckoutRequestDetails.BillingAddress.Country = codeType;
                        break;
                    }
                }
                expCheckoutRequest.SetExpressCheckoutRequestDetails.BillingAddress.CountryName = paymentModel.BillingCountryCode;
                expCheckoutRequest.SetExpressCheckoutRequestDetails.BillingAddress.CountrySpecified = true;
                expCheckoutRequest.SetExpressCheckoutRequestDetails.BuyerEmail = paymentModel.BillingEmailId;
            }
        }

        private string GetQuantity(string quantity)
        {
            String[] qty = quantity.Split('.');
            return qty[0];
        }

        /// <summary>
        /// To set the PaymentStatus and Get the Gateway response
        /// </summary>
        /// <param name="payment_response">Set SetExpressCheckoutRequestType to current model</param>
        /// <param name="ECPaymentResponse">PaymentModel to get billing shipping details</param>
        /// <param name="response">Paypal response object</param>
        /// <param name="token">Token value</param>
        /// <param name="payerID">Payer ID</param>
        /// <param name="znodePaymentStatus">Payment Status Enum value</param>
        private void GetGatewayResponseModel(GatewayResponseModel payment_response, DoExpressCheckoutPaymentResponseType ECPaymentResponse, PaypalResponse response, string token, string payerID, ZnodePaymentStatus znodePaymentStatus)
        {
            payment_response.ResponseText = ECPaymentResponse.Ack.ToString();
            response.PaymentStatus = ECPaymentResponse.DoExpressCheckoutPaymentResponseDetails.PaymentInfo[0].PaymentStatus.ToString();
            payment_response.PaymentStatus = znodePaymentStatus;
            payment_response.TransactionId = ECPaymentResponse.DoExpressCheckoutPaymentResponseDetails.PaymentInfo[0].TransactionID.ToString();
            payment_response.IsSuccess = true;

            // Create Recurring Payments Profile
            if (payment_response.IsSuccess)
            {
                // Code for Creating Recurring Payments Profile
                PaymentDetailsItemType[] Ritems = new PaymentDetailsItemType[paymentModel.Subscriptions.Count()];
                int index = 0;
                decimal RecurringTotal = 0.00M;
                bool subscriptionResult = true;
                string responseText = "This gateway does not support all of the billing periods that you have set on products. Please be sure to update the billing period on all your products.";

                // Loop through the Order Line Items
                foreach (var itemsub in paymentModel.Subscriptions)
                {
                    PaypalRecurringBillingInfo recurringBilling = new PaypalRecurringBillingInfo();
                    recurringBilling.InitialAmount = Math.Round(itemsub.Amount, 2);

                    recurringBilling.Period = itemsub.Period;
                    recurringBilling.TotalCycles = itemsub.TotalCycles;
                    recurringBilling.Frequency = itemsub.Frequency;
                    recurringBilling.Amount = Math.Round(itemsub.Amount, 2);
                    recurringBilling.ProfileName = itemsub.ProfileName;
                    RecurringTotal = RecurringTotal + itemsub.Amount;

                    var val = this.CreateRecurringPaymentsProfile(token, payerID, recurringBilling, itemsub.ProfileName);

                    if (val.IsSuccess)
                        payment_response.ReferenceNumber = val.ReferenceNumber;
                    else
                    {
                        subscriptionResult &= false;
                        responseText = val.ResponseText + val.ResponseCode;
                        break;
                    }

                    if (!subscriptionResult)
                        break;

                    index++;
                }
            }
        }
        #endregion
    }
}
