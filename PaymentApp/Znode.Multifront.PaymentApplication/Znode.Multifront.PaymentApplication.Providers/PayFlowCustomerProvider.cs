using PayPal.Payments.Common.Utility;
using PayPal.Payments.DataObjects;
using PayPal.Payments.Transactions;
using System;
using Znode.Multifront.PaymentApplication.Data;
using System.Configuration;
using Znode.Multifront.PaymentApplication.Helpers;
using Znode.Multifront.PaymentApplication.Models;
using System.Diagnostics;


namespace Znode.Multifront.PaymentApplication.Providers
{
    /// <summary>
    /// This is the class responsible for the payment related operations of braintree payment provider
    /// </summary>
    public class PayFlowCustomerProvider : BaseProvider, IPaymentProviders
    {
        /// <summary>
        /// Validate the credit card and returns the transaction details.
        /// </summary>
        /// <param name="paymentModel">PaymentModel</param>
        /// <returns>gateway response model</returns>
        public GatewayResponseModel ValidateCreditcard(PaymentModel paymentModel)
        {
            GatewayResponseModel response = new GatewayResponseModel();

            try
            {
                if (!string.IsNullOrEmpty(paymentModel.CustomerProfileId))
                {
                    paymentModel.TransactionId = string.IsNullOrEmpty(paymentModel.TransactionId) ? paymentModel.CustomerPaymentProfileId : paymentModel.TransactionId;
                    return CreateTransaction(paymentModel);
                }
                //To Create Customer Profile based on user input.
                response = CreateCustomer(paymentModel);
                return response;

            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(null, ex.Message);
                return new GatewayResponseModel { IsSuccess = false, GatewayResponseData = ex.Message };
            }
        }

        /// <summary>
        /// Refund the amount for the given transaction
        /// </summary>
        /// <param name="paymentModel">PaymentModel</param>
        /// <returns>gateway response model</returns>
        public GatewayResponseModel Refund(PaymentModel paymentModel)
        {
            try
            {
                GatewayResponseModel gatewayResponse = new GatewayResponseModel();

                paymentModel.Vendor = string.IsNullOrEmpty(paymentModel.Vendor) ? paymentModel.GatewayLoginName : paymentModel.Vendor;
                
                // Create the User data object with the required user details.
                UserInfo User = new UserInfo(paymentModel.GatewayLoginName, paymentModel.Vendor, paymentModel.Partner, paymentModel.GatewayLoginPassword);

                // Create the Payflow  Connection data object with the required connection details.
                PayflowConnectionData Connection = new PayflowConnectionData();

                Invoice Inv = new Invoice();// Set Amount.
                Currency Amt = new Currency(decimal.Parse(paymentModel.Total));
                Inv.Amt = Amt;

                // The ORIGID is the PNREF no. for a previous transaction.  
                // Create a new Refund Transaction.
                //CreditTransaction Trans = new CreditTransaction(paymentModel.TransactionId, User, Connection, PayflowUtility.RequestId);

                CreditTransaction Trans = new CreditTransaction(paymentModel.CaptureTransactionId, User, Connection, Inv, PayflowUtility.RequestId);

                // Submit the Transaction
                Response Resp = Trans.SubmitTransaction();

                // Display the transaction response parameters.
                if (Resp != null)
                {
                    // Get the Transaction Response parameters.
                    TransactionResponse transactionResponse = Resp.TransactionResponse;

                    if (transactionResponse != null)
                    {
                        gatewayResponse.IsSuccess = transactionResponse.Result == 0 ? true : false;
                        gatewayResponse.ResponseText = transactionResponse.RespMsg;
                        gatewayResponse.TransactionId = transactionResponse.Pnref;
                        gatewayResponse.ResponseCode = Convert.ToString(transactionResponse.Result);
                        gatewayResponse.PaymentStatus = ZnodePaymentStatus.REFUNDED;
                    }

                }
                return gatewayResponse;
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(null, ex.Message);
                return new GatewayResponseModel { IsSuccess = false, GatewayResponseData = ex.Message };
            }
        }

        /// <summary>
        /// Void the transaction for the given transaction details
        /// </summary>
        /// <param name="paymentModel">PaymentModel</param>
        /// <returns>gateway response model</returns>
        public GatewayResponseModel Void(PaymentModel paymentModel)
        {
            try
            {
                GatewayResponseModel gatewayResponse = new GatewayResponseModel();

                paymentModel.Vendor = string.IsNullOrEmpty(paymentModel.Vendor) ? paymentModel.GatewayLoginName : paymentModel.Vendor;
                
                // Create the User data object with the required user details.
                UserInfo User = new UserInfo(paymentModel.GatewayLoginName, paymentModel.Vendor, paymentModel.Partner, paymentModel.GatewayLoginPassword);

                // Create the Payflow  Connection data object with the required connection details.
                PayflowConnectionData Connection = new PayflowConnectionData();

                // Create a new Void Transaction.
                // The ORIGID is the PNREF no. for a previous transaction.
                VoidTransaction Trans = new VoidTransaction(paymentModel.TransactionId, User, Connection, PayflowUtility.RequestId);

                // Submit the Transaction
                Response Resp = Trans.SubmitTransaction();

                // Display the transaction response parameters.
                if (Resp != null)
                {
                    // Get the Transaction Response parameters.
                    TransactionResponse transactionResponse = Resp.TransactionResponse;

                    if (transactionResponse != null)
                    {
                        gatewayResponse.IsSuccess = transactionResponse.Result == 0 ? true : false;
                        gatewayResponse.ResponseCode = Convert.ToString(transactionResponse.Result);
                        gatewayResponse.ResponseText = transactionResponse.RespMsg;
                        gatewayResponse.TransactionId = transactionResponse.Pnref;
                        gatewayResponse.PaymentStatus = ZnodePaymentStatus.VOIDED;
                    }
                }
                return gatewayResponse;
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(null, ex.Message);
                return new GatewayResponseModel { IsSuccess = false, GatewayResponseData = ex.Message };
            }
        }

        /// <summary>
        /// Recurring subscription
        /// </summary>
        /// <param name="paymentModel">PaymentModel</param>
        /// <returns>gateway response model</returns>
        public GatewayResponseModel Subscription(PaymentModel paymentModel)
        {
            try
            {
                GatewayResponseModel gatewayResponse = new GatewayResponseModel();

                // Populate the invoice
                Invoice invoice = new Invoice();
                invoice.BillTo = SetBillingAddress(paymentModel);
                invoice.ShipTo = SetShippingAddress(paymentModel);
                invoice.InvNum = "INV" + DateTime.Now.Ticks.ToString();
                invoice.Amt = new Currency(decimal.Parse(paymentModel.Total));

                // Populate the Credit Card details.            
                // Set Credit Card Security Code                         
                CreditCard Card = new CreditCard(paymentModel.CardNumber, paymentModel.CardExpirationMonth + paymentModel.CardExpirationYear.Substring(2));
                Card.Cvv2 = paymentModel.CardSecurityCode;


                // Create the Tender.
                CardTender tender = new CardTender(Card);
                tender.ChkNum = "1";

                paymentModel.Vendor = string.IsNullOrEmpty(paymentModel.Vendor) ? paymentModel.GatewayLoginName : paymentModel.Vendor;
                
                // Merchant Account Details
                // Create the User data object with the required user details.
                UserInfo User = new UserInfo(paymentModel.GatewayLoginName, paymentModel.Vendor, paymentModel.Partner, paymentModel.GatewayLoginPassword);


                // Create the Payflow  Connection data object with the required connection details.
                PayflowConnectionData Connection = new PayflowConnectionData();

                // Recurring Billing settings
                RecurringInfo subscriptionInfo = new RecurringInfo();
                subscriptionInfo.ProfileName = paymentModel.Subscription.ProfileName;

                invoice.Amt = new Currency(paymentModel.Subscription.Amount);

                subscriptionInfo.PayPeriod = paymentModel.Subscription.Period;

                subscriptionInfo.Term = long.Parse(paymentModel.Subscription.TotalCycles.ToString());
                subscriptionInfo.OptionalTrx = "A"; // Authorization for validating the account information
                DateTime startDate = DateTime.Today.Date.AddDays(1);

                if (paymentModel.Subscription.Amount > 0)
                {
                    subscriptionInfo.OptionalTrxAmt = new Currency(paymentModel.Subscription.Amount);

                    long term = long.Parse(paymentModel.Subscription.Period);
                    startDate = DateTime.Today.Date.AddDays(term);
                }

                // Format: MMDDYYYY            
                subscriptionInfo.Start = startDate.ToString("MMddyyyy");

                // Create the transaction.
                PayPal.Payments.DataObjects.Response RespAuth;

                RecurringAddTransaction recurringBilling = new RecurringAddTransaction(User, Connection, invoice, tender, subscriptionInfo, PayflowUtility.RequestId);

                // Submit request transaction
                RespAuth = recurringBilling.SubmitTransaction();

                if (RespAuth.TransactionResponse.Result == 0)
                {
                    gatewayResponse.ResponseCode = "0";
                    gatewayResponse.IsSuccess = true;
                    gatewayResponse.ResponseText = RespAuth.TransactionResponse.RespMsg;
                    gatewayResponse.CustomerProfileId = RespAuth.RecurringResponse.ProfileId;
                    gatewayResponse.CardAuthCode = RespAuth.RecurringResponse.RPRef;
                    gatewayResponse.TransactionId = RespAuth.RecurringResponse.TrxPNRef;
                }
                else
                {
                    gatewayResponse.IsSuccess = true;
                    gatewayResponse.ResponseCode = RespAuth.TransactionResponse.Result.ToString();
                    gatewayResponse.ResponseCode = Convert.ToString(RespAuth.TransactionResponse.Result);
                    gatewayResponse.ResponseText = RespAuth.TransactionResponse.RespMsg;
                }
                return gatewayResponse;
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(null, ex.Message);
                return new GatewayResponseModel { IsSuccess = false, GatewayResponseData = ex.Message };
            }
        }

        public TransactionDetailsModel GetTransactionDetails(PaymentModel paymentModel)
        {
            return new TransactionDetailsModel();
        }

        #region Private Methods

        private GatewayResponseModel CreateCustomer(PaymentModel paymentModel)
        {
            return AuthorizePayment(paymentModel);
        }

        private GatewayResponseModel CreateTransaction(PaymentModel paymentModel)
        {
            if (paymentModel.IsCapture)
                return CapturePayment(paymentModel);
            else
            {
                GatewayResponseModel gatewayResponse = new GatewayResponseModel();
                gatewayResponse.IsSuccess = true;
                gatewayResponse.TransactionId = paymentModel.TransactionId;
                gatewayResponse.PaymentStatus = ZnodePaymentStatus.AUTHORIZED;
                gatewayResponse.CustomerProfileId = paymentModel.CustomerProfileId;
                gatewayResponse.CustomerPaymentProfileId = gatewayResponse.CustomerPaymentProfileId;
                gatewayResponse.IsGatewayPreAuthorize = paymentModel.GatewayPreAuthorize;
                return gatewayResponse;
            }
        }

        /// <summary>
        /// Capture the transaction for the given transaction details
        /// </summary>
        /// <param name="paymentModel">PaymentModel</param>
        /// <returns>gateway response model</returns>
        private GatewayResponseModel CapturePayment(PaymentModel paymentModel)
        {
            GatewayResponseModel gatewayResponse = new GatewayResponseModel();
            try
            {
                paymentModel.Vendor = string.IsNullOrEmpty(paymentModel.Vendor) ? paymentModel.GatewayLoginName : paymentModel.Vendor;
                
                // Create the User data object with the required user details.
                UserInfo User = new UserInfo(paymentModel.GatewayLoginName, paymentModel.Vendor, paymentModel.Partner, paymentModel.GatewayLoginPassword);

                // Create the Payflow  Connection data object with the required connection details.
                PayflowConnectionData Connection = new PayflowConnectionData();

                // Create a new Capture Transaction for the original amount of the authorization.  See above if you
                CaptureTransaction Trans = new CaptureTransaction(paymentModel.TransactionId, User, Connection, PayflowUtility.RequestId);
                // Indicates if this Delayed Capture transaction is the last capture you intend to make.
                // The values are: Y (default) / N
                // NOTE: If CAPTURECOMPLETE is Y, any remaining amount of the original reauthorized transaction
                // is automatically voided.  Also, this is only used for UK and US accounts where PayPal is acting
                // as your bank.
                // Trans.CaptureComplete = "N";

                // Submit the Transaction
                Response Resp = Trans.SubmitTransaction();

                // Display the transaction response parameters.
                if (Resp != null)
                {
                    // Get the Transaction Response parameters.
                    TransactionResponse transactionResponse = Resp.TransactionResponse;

                    if (transactionResponse != null)
                    {
                        gatewayResponse.IsSuccess = transactionResponse.Result == 0 ? true : false;
                        gatewayResponse.TransactionId = transactionResponse.Pnref;
                        gatewayResponse.ResponseText = transactionResponse.RespMsg;
                        gatewayResponse.ResponseCode = Convert.ToString(transactionResponse.Result);
                        gatewayResponse.PaymentStatus = ZnodePaymentStatus.CAPTURED;
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(null, ex.Message);
                gatewayResponse.IsSuccess = false;
                gatewayResponse.GatewayResponseData = ex.Message;
            }
            return gatewayResponse;
        }

        /// <summary>
        /// AuthorizePayment the transaction for the given transaction details
        /// </summary>
        /// <param name="paymentModel">PaymentModel</param>
        /// <returns>gateway response model</returns>
        private GatewayResponseModel AuthorizePayment(PaymentModel paymentModel)
        {
            var billing = SetBillingAddress(paymentModel);
            var shipping = SetShippingAddress(paymentModel);
            if (!Equals(shipping, null))
            {
                try
                {
                    GatewayResponseModel gatewayResponse = new GatewayResponseModel();

                    paymentModel.Vendor = string.IsNullOrEmpty(paymentModel.Vendor) ? paymentModel.GatewayLoginName : paymentModel.Vendor;
                    
                    // Create the User data object with the required user details.
                    UserInfo User = new UserInfo(paymentModel.GatewayLoginName, paymentModel.Vendor, paymentModel.Partner, paymentModel.GatewayLoginPassword);

                    // Create the Payflow  Connection data object with the required connection details.
                    PayflowConnectionData Connection = new PayflowConnectionData();

                    // Create a new Invoice data object with the Amount, Billing Address etc. details.
                    Invoice Inv = new Invoice();

                    GetTotalAmount(paymentModel);
                    // Set Amount.
                    Currency Amt = new Currency(decimal.Parse(paymentModel.Total));
                    Inv.Amt = Amt;
                    Inv.PoNum = "PO" + DateTime.Now.Ticks.ToString();
                    Inv.InvNum = paymentModel.OrderId; //"INV" + DateTime.Now.Ticks.ToString();
                    Inv.BrowserInfo = new BrowserInfo { ButtonSource = ConfigurationManager.AppSettings["PaypalBNCode"] };

                    Inv.BillTo = SetBillingAddress(paymentModel);
                    Inv.ShipTo = SetShippingAddress(paymentModel);
                    // Create a new Payment Device - Credit Card data object.
                    // The input parameters are Credit Card Number and Expiration Date of the Credit Card.
                    CreditCard CC = new CreditCard(paymentModel.CardNumber, paymentModel.CardExpirationMonth + paymentModel.CardExpirationYear.Substring(2));
                    CC.Cvv2 = paymentModel.CardSecurityCode;

                    // Create a new Tender - Card Tender data object.
                    CardTender Card = new CardTender(CC);

                    // Create a new Auth Transaction.
                    AuthorizationTransaction Trans = new AuthorizationTransaction(User, Connection, Inv, Card, PayflowUtility.RequestId);

                    // Submit the Transaction
                    Response Resp = Trans.SubmitTransaction();

                    // Display the transaction response parameters.
                    if (Resp != null)
                    {
                        // Get the Transaction Response parameters.
                        TransactionResponse transactionResponse = Resp.TransactionResponse;

                        if (transactionResponse != null)
                        {
                            if (transactionResponse.Result.Equals(126))
                            {
                                gatewayResponse.IsSuccess = true;
                            }
                            else
                            {
                                gatewayResponse.IsSuccess = transactionResponse.Result == 0 ? true : false;
                            }

                            if (transactionResponse.Result.Equals(12))
                                gatewayResponse.ResponseText = transactionResponse.RespMsg + ". Check the credit card number, expiration date, and transaction information to make sure they were entered correctly.";
                            else
                                gatewayResponse.ResponseText = transactionResponse.RespMsg;

                            gatewayResponse.ResponseCode = Convert.ToString(transactionResponse.Result);
                            gatewayResponse.TransactionId = transactionResponse.Pnref;
                            gatewayResponse.CardAuthCode = transactionResponse.AuthCode;
                            gatewayResponse.CustomerProfileId = transactionResponse.AuthCode;
                            gatewayResponse.CustomerPaymentProfileId = transactionResponse.Pnref;
                            gatewayResponse.PaymentStatus = ZnodePaymentStatus.AUTHORIZED;
                        }
                    }
                    return gatewayResponse;
                }
                catch (Exception ex)
                {
                    Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                }
                finally
                {

                }
            }
            else
            {
                if (Equals(shipping, null))
                {
                    Logging.LogMessage("Shipping Address : Is Null", Logging.Components.Payment.ToString(), TraceLevel.Error);
                }
                else
                {
                    Logging.LogMessage("Billing Address : Is Null", Logging.Components.Payment.ToString(), TraceLevel.Error);
                }
            }
            return new GatewayResponseModel();

        }

        /// <summary>
        /// CreateTransaction the transaction for the given transaction details
        /// </summary>
        /// <param name="paymentModel">PaymentModel</param>
        /// <returns>gateway response model</returns>
        private GatewayResponseModel SaleTransaction(PaymentModel paymentModel)
        {
            // Create the User data object with the required user details.
            GatewayResponseModel gatewayResponse = new GatewayResponseModel();
            try
            {
                UserInfo User = new UserInfo(paymentModel.GatewayLoginName, paymentModel.GatewayLoginName, paymentModel.Partner, paymentModel.GatewayLoginPassword);

                // Create the Payflow  Connection data object with the required connection details.
                PayflowConnectionData Connection = new PayflowConnectionData();

                // Create a new Invoice data object with the Amount, Billing Address etc. details.
                Invoice Inv = new Invoice();

                // Set Amount.
                decimal total = decimal.Parse(paymentModel.Total);
                Currency Amt = new Currency(total);
                Inv.Amt = Amt;
                Inv.PoNum = "PO" + DateTime.Now.Ticks.ToString();
                Inv.InvNum = "INV" + DateTime.Now.Ticks.ToString();

                Inv.BillTo = SetBillingAddress(paymentModel);

                // Create a new Payment Device - Credit Card data object.
                CreditCard CC = new CreditCard(paymentModel.CardNumber, paymentModel.CardExpirationMonth + paymentModel.CardExpirationYear.Substring(2));

                // Create a new Tender - Card Tender data object.
                CardTender Card = new CardTender(CC);
                ///////////////////////////////////////////////////////////////////

                // Create a new Credit Transaction.
                SaleTransaction Trans = new SaleTransaction(User, Connection, Inv, Card, PayflowUtility.RequestId);

                // Submit the Transaction
                Response Resp = Trans.SubmitTransaction();

                // Display the transaction response parameters.
                if (Resp != null)
                {
                    // Get the Transaction Response parameters.
                    TransactionResponse transactionResponse = Resp.TransactionResponse;

                    if (transactionResponse != null)
                    {
                        gatewayResponse.IsSuccess = transactionResponse.Result == 0 ? true : false;
                        gatewayResponse.ResponseCode = Convert.ToString(transactionResponse.Result);
                        gatewayResponse.ResponseText = transactionResponse.RespMsg;
                        gatewayResponse.TransactionId = transactionResponse.Pnref;
                        gatewayResponse.CardAuthCode = transactionResponse.AuthCode;
                        gatewayResponse.CustomerProfileId = transactionResponse.AuthCode;
                        gatewayResponse.PaymentStatus = ZnodePaymentStatus.CAPTURED;
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(null, ex.Message);
                gatewayResponse.IsSuccess = false;
                gatewayResponse.GatewayResponseData = ex.Message;
            }
            return gatewayResponse;
        }

        private GatewayResponseModel UpdateTransactionAmount(PaymentModel paymentModel)
        {
            GatewayResponseModel gatewayResponse = new GatewayResponseModel();
            try
            {
                UserInfo User = new UserInfo(paymentModel.GatewayLoginName, paymentModel.GatewayLoginName, paymentModel.Partner, paymentModel.GatewayLoginPassword);

                // Create the Payflow  Connection data object with the required connection details.
                PayflowConnectionData Connection = new PayflowConnectionData();

                // Create a new Invoice data object with the Amount, Billing Address etc. details.
                Invoice Inv = new Invoice();

                // Set Amount.
                decimal total = decimal.Parse(paymentModel.Total);
                Currency Amt = new Currency(total);
                Inv.Amt = Amt;

                // Create a new Credit Transaction from the original transaction.  See above if you
                // need to change the amount.
                CreditTransaction Trans = new CreditTransaction(paymentModel.TransactionId, User, Connection, Inv, PayflowUtility.RequestId);

                // Submit the Transaction
                Response Resp = Trans.SubmitTransaction();

                // Display the transaction response parameters.
                if (Resp != null)
                {
                    // Get the Transaction Response parameters.
                    TransactionResponse transactionResponse = Resp.TransactionResponse;

                    if (transactionResponse != null)
                    {
                        gatewayResponse.IsSuccess = transactionResponse.Result == 0 ? true : false;
                        gatewayResponse.ResponseCode = Convert.ToString(transactionResponse.Result);
                        gatewayResponse.ResponseText = transactionResponse.RespMsg;
                        gatewayResponse.TransactionId = transactionResponse.Pnref;
                        gatewayResponse.CardAuthCode = transactionResponse.AuthCode;
                    }
                }
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(null, ex.Message);
                gatewayResponse.IsSuccess = false;
                gatewayResponse.GatewayResponseData = ex.Message;
            }
            return gatewayResponse;
        }

        /// <summary>
        /// To check Total Amount is in encrypted format or not
        /// </summary>
        /// <param name="paymentModel"></param>
        private void GetTotalAmount(PaymentModel paymentModel)
        {
            try
            {
                paymentModel.Total = DecryptPaymentToken(paymentModel.Total);
            }
            catch (Exception ex)
            {
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                LoggingService.LogActivity(null, ex.Message);
                throw ex;
            }
        }

        // to Populate the Billing address details.
        private BillTo SetBillingAddress(PaymentModel paymentModel)
        {
            BillTo Bill = new BillTo();
            Bill.BillToFirstName = paymentModel.BillingFirstName;
            Bill.BillToLastName = paymentModel.BillingLastName;
            Bill.BillToStreet = paymentModel.BillingStreetAddress1;
            Bill.BillToStreet2 = paymentModel.BillingStreetAddress2;
            Bill.BillToCity = paymentModel.BillingCity;
            Bill.BillToZip = paymentModel.BillingPostalCode;
            Bill.BillToState = paymentModel.BillingStateCode;
            Bill.BillToEmail = paymentModel.BillingEmailId;
            return Bill;
        }

        // to Populate the Shipping address details.
        private ShipTo SetShippingAddress(PaymentModel paymentModel)
        {
            ShipTo Ship = new ShipTo();
            Ship.ShipToFirstName = paymentModel.ShippingFirstName;
            Ship.ShipToLastName = paymentModel.ShippingLastName;
            Ship.ShipToStreet = paymentModel.ShippingStreetAddress1;
            Ship.ShipToStreet2 = paymentModel.ShippingStreetAddress2;
            Ship.ShipToCity = paymentModel.ShippingCity;
            Ship.ShipToZip = paymentModel.ShippingPostalCode;
            Ship.ShipToState = paymentModel.ShippingStateCode;
            Ship.ShipToEmail = paymentModel.ShippingEmailId;
            Ship.ShipToCountry = paymentModel.ShippingCountryCode;

            return string.IsNullOrEmpty(paymentModel.ShippingCity) && string.IsNullOrEmpty(paymentModel.ShippingPostalCode) && string.IsNullOrEmpty(paymentModel.ShippingStateCode) && string.IsNullOrEmpty(paymentModel.ShippingCountryCode) ? null : Ship;
        }

        public PaymentGatewayTokenModel TokenGenerator(PaymentGatewayTokenModel paymentModel)
        {
            throw new NotImplementedException();
        }
        #endregion
    }
}
