using Paymentech;
using System;
using System.Configuration;
using System.Diagnostics;
using Znode.Multifront.PaymentApplication.Data;
using Znode.Multifront.PaymentApplication.Helpers;
using Znode.Multifront.PaymentApplication.Models;

namespace Znode.Multifront.PaymentApplication.Providers
{
    public class PaymentTechProvider : BaseProvider, IPaymentProviders
    {
        private string PaymenteckSDKPath = Convert.ToString(ConfigurationManager.AppSettings["PaymentechSDKPath"]);

        /// <summary>
        /// Validate the credit card and returns the transaction details.
        /// </summary>
        /// <param name="paymentModel"></param>
        /// <returns></returns>
        public GatewayResponseModel ValidateCreditcard(PaymentModel paymentModel)
        {
            try
            {
                if (string.IsNullOrEmpty(paymentModel.CustomerProfileId))
                    return this.CreateCustomer(paymentModel);
                return this.CreateTransaction(paymentModel);
            }
            catch (Exception ex)
            {
                LoggingService.LogActivity(paymentModel.PaymentApplicationSettingId, ex.Message, paymentModel.GUID);
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                return new GatewayResponseModel { IsSuccess = false, GatewayResponseData = ex.Message };
            }
        }

        /// <summary>
        /// Refund the amount for the given transaction
        /// </summary>
        /// <param name="paymentModel"></param>
        /// <returns></returns>
        public GatewayResponseModel Refund(PaymentModel paymentModel)
        {
            GatewayResponseModel gatewayResponse = new GatewayResponseModel();

            try
            {

                Transaction transaction = new Transaction(PaymenteckSDKPath, RequestType.ECOMMERCE_REFUND);

                MapTransaction(transaction, paymentModel, "R");

                transaction["TxRefNum"] = paymentModel.TransactionId;
                //Stripe Accept Amount in Sub-divisions Multiply by 100 convert amount into subdivisions
                //Note will not work for RO,LD,JD,ID,Rls,RMB currency symbols
                transaction["Amount"] = decimal.Round((decimal.Parse(paymentModel.Total) * 100), 0).ToString();
                transaction["CustomerRefNum"] = paymentModel.CustomerPaymentProfileId;
                transaction["CustomerProfileOrderOverrideInd"] = "NO";

                Response response = transaction.Process();
                if (response["ProcStatus"].Equals("0"))
                {
                    gatewayResponse.IsSuccess = true;
                    gatewayResponse.TransactionId = response["TxRefNum"];
                    gatewayResponse.PaymentStatus = ZnodePaymentStatus.REFUNDED;
                }
                else
                {
                    gatewayResponse.IsSuccess = false;
                    gatewayResponse.GatewayResponseData = response["StatusMsg"];
                    gatewayResponse.ResponseCode = response["ProcStatus"];
                }

                return gatewayResponse;
            }
            catch (Exception ex)
            {
                LoggingService.LogActivity(paymentModel.PaymentApplicationSettingId, ex.Message, paymentModel.GUID);
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                return new GatewayResponseModel { IsSuccess = false, GatewayResponseData = ex.Message };
            }
        }

        /// <summary>
        /// Void the transaction for the given transaction details
        /// </summary>
        /// <param name="paymentModel"></param>
        /// <returns></returns>
        public GatewayResponseModel Void(PaymentModel paymentModel)
        {
            GatewayResponseModel gatewayResponse = new GatewayResponseModel();
            try
            {
                Transaction transaction = new Transaction(PaymenteckSDKPath, RequestType.REVERSE_TRANSACTION);

                MapTransaction(transaction, paymentModel);

                transaction["TxRefNum"] = paymentModel.TransactionId;
                transaction["TxRefIdx"] = "0";

                Response response = transaction.Process();
                if (response["ProcStatus"].Equals("0"))
                {
                    gatewayResponse.IsSuccess = true;
                    gatewayResponse.PaymentStatus = ZnodePaymentStatus.VOIDED;
                }
                else
                {
                    gatewayResponse.IsSuccess = false;
                    gatewayResponse.GatewayResponseData = response["StatusMsg"];
                    gatewayResponse.ResponseCode = response["ProcStatus"];
                }

                return gatewayResponse;
            }
            catch (Exception ex)
            {
                LoggingService.LogActivity(paymentModel.PaymentApplicationSettingId, ex.Message, paymentModel.GUID);
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                return new GatewayResponseModel { IsSuccess = false, GatewayResponseData = ex.Message };
            }
        }

        /// <summary>
        /// Recurring subscription
        /// </summary>
        /// <param name="paymentModel"></param>
        /// <returns></returns>
        public GatewayResponseModel Subscription(PaymentModel paymentModel)
        {
            GatewayResponseModel gatewayResponse = new GatewayResponseModel();
            try
            {
                Transaction transaction = new Transaction(PaymenteckSDKPath, RequestType.NEW_ORDER_TRANSACTION);

                MapTransaction(transaction, paymentModel, "AC");

                transaction["IndustryType"] = "RC";
                transaction["Amount"] = decimal.Round((paymentModel.Subscription.Amount * 100), 0).ToString();
                transaction["TxRefNum"] = paymentModel.TransactionId;
                transaction["CustomerProfileFromOrderInd"] = "S";
                transaction["CustomerRefNum"] = paymentModel.CustomerProfileId;
                transaction["CustomerProfileOrderOverrideInd"] = "NO";
                transaction["Status"] = "A";
                transaction["MBType"] = "R";
                transaction["MBOrderIdGenerationMethod"] = "DI";
                transaction["MBRecurringMaxBillings"] = paymentModel.Subscription.TotalCycles.ToString();
                transaction["MBRecurringFrequency"] = "1 * ?";
                transaction["Exp"] = paymentModel.CardExpirationMonth + paymentModel.CardExpirationYear.Substring(2);
                transaction["AccountNum"] = paymentModel.CardNumber;
                transaction["CardSecVal"] = paymentModel.CardSecurityCode;

                transaction["IndustryType"] = "RC";
                transaction["Amount"] = decimal.Round((paymentModel.Subscription.Amount * 100), 0).ToString();
                transaction["TxRefNum"] = paymentModel.TransactionId;
                transaction["CustomerProfileFromOrderInd"] = "S";
                transaction["CustomerRefNum"] = paymentModel.CustomerProfileId;
                transaction["CustomerProfileOrderOverrideInd"] = "NO";
                transaction["Status"] = "A";
                transaction["MBType"] = "R";
                transaction["MBOrderIdGenerationMethod"] = "DI";
                transaction["MBRecurringMaxBillings"] = paymentModel.Subscription.TotalCycles.ToString();
                transaction["MBRecurringFrequency"] = "1 * ?";
                transaction["Exp"] = paymentModel.CardExpirationMonth + paymentModel.CardExpirationYear.Substring(2);
                transaction["AccountNum"] = paymentModel.CardNumber;
                transaction["CardSecVal"] = paymentModel.CardSecurityCode;

                Response response = transaction.Process();
                if (response["ProcStatus"].Equals("0"))
                {
                    gatewayResponse.IsSuccess = true;
                    gatewayResponse.TransactionId = ((Paymentech.Response)(response)).TxRefNum;
                }
                else
                {
                    gatewayResponse.IsSuccess = false;
                    gatewayResponse.GatewayResponseData = response["StatusMsg"];
                    gatewayResponse.ResponseCode = response["ProcStatus"];
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

        /// <summary>
        /// Create payment transaction based on created customer token.
        /// </summary>
        /// <param name="paymentModel">PaymentModel paymentModel</param>
        /// <returns>returns paymenttech response</returns>
        private GatewayResponseModel CreateTransaction(PaymentModel paymentModel)
        {
            try
            {
                GatewayResponseModel gatewayResponse = new GatewayResponseModel();
                Transaction transaction = null;
                if (paymentModel.IsCapture)
                {
                    // Create an Capture transaction
                    transaction = new Transaction(PaymenteckSDKPath, RequestType.CC_CAPTURE);
                    MapTransaction(transaction, paymentModel);
                    transaction["TxRefNum"] = paymentModel.TransactionId;
                }
                else
                {
                    paymentModel.OrderId = null;

                    // Create an Authorize transaction
                    transaction = new Transaction(PaymenteckSDKPath, RequestType.CC_AUTHORIZE);
                    MapTransaction(transaction, paymentModel, "A");
                    transaction["CustomerRefNum"] = paymentModel.CustomerPaymentProfileId;
                }
                paymentModel.Total = String.Format("{0:0.00}", Convert.ToDecimal(paymentModel.Total));

                //Stripe Accept Amount in Sub-divisions Multiply by 100 convert amount into subdivisions
                //Note will not work for RO,LD,JD,ID,Rls,RMB currency symbols
                transaction["Amount"] = decimal.Round((decimal.Parse(paymentModel.Total) * 100), 0).ToString();

                Response response = transaction.Process();
                if (response["ProcStatus"].Equals("0"))
                {
                    gatewayResponse.IsSuccess = response.Approved;
                    gatewayResponse.TransactionId = response["TxRefNum"];
                    gatewayResponse.PaymentStatus = paymentModel.IsCapture ? ZnodePaymentStatus.CAPTURED : ZnodePaymentStatus.AUTHORIZED;
                    gatewayResponse.CustomerProfileId = paymentModel.CustomerProfileId;
                    gatewayResponse.CustomerPaymentProfileId = paymentModel.CustomerPaymentProfileId;
                    gatewayResponse.IsGatewayPreAuthorize = paymentModel.GatewayPreAuthorize;
                    gatewayResponse.Token = response["OrderID"];
                    gatewayResponse.ResponseText = response.Message;
                }
                else
                {
                    gatewayResponse.IsSuccess = false;
                    gatewayResponse.GatewayResponseData = response["StatusMsg"];
                    gatewayResponse.ResponseCode = response["ProcStatus"];
                }
                return gatewayResponse;
            }
            catch (Exception ex)
            {
                LoggingService.LogActivity(paymentModel.PaymentApplicationSettingId, ex.Message, paymentModel.GUID);
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                return new GatewayResponseModel { IsSuccess = false, GatewayResponseData = ex.Message };
            }
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
                        response = CreateCustomerPayment(paymentModel);
                        if (response.IsSuccess)
                            isSuccess = gatewayConnector.SaveCustomerDetails(paymentModel);
                    }
                    else
                        return CreateCustomerPayment(paymentModel);
                }
                else
                {
                    if (Equals(paymentModel.IsSaveCreditCard, true))
                    {
                        response = CreatePaymentGatewayVault(paymentModel);
                        if (response.IsSuccess)
                            isSuccess = gatewayConnector.SaveCustomerDetails(paymentModel);
                    }
                    else if (!string.IsNullOrEmpty(paymentModel.PaymentToken))
                        isSuccess = gatewayConnector.SaveCustomerDetails(paymentModel);
                    else
                        return CreateCustomerPayment(paymentModel);
                }

                response.CustomerGUID = paymentModel.CustomerGUID;
                response.PaymentToken = paymentModel.CustomerPaymentProfileId;
                response.CustomerProfileId = paymentModel.CustomerProfileId;
                response.CustomerPaymentProfileId = paymentModel.CustomerPaymentProfileId;
                response.IsSuccess = isSuccess;
                return response;
            }
            else
                return CreateCustomerPayment(paymentModel);
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
            {
                paymentModel.CustomerProfileId = payment.CustomerProfileId;
                response = CreateCustomerPayment(paymentModel);
            }
            else
                response = CreateCustomerPayment(paymentModel);
            return Equals(response, null) ? new GatewayResponseModel() : response;
        }

        /// <summary>
        /// To create customer profile for getting CustomerProfileId
        /// </summary>
        /// <param name="paymentModel">PaymentModel paymentModel</param>
        /// <returns>returns paymenttech response</returns>
        private GatewayResponseModel CreateCustomerPayment(PaymentModel paymentModel)
        {
            try
            {
                GatewayResponseModel gatewayResponse = new GatewayResponseModel();

                // Create an authorize transaction
                Transaction transaction = new Transaction(PaymenteckSDKPath, RequestType.PROFILE_MANAGEMENT);

                MapTransaction(transaction, paymentModel, string.Empty, false, "CustomerBin");

                Random random = new Random();
                string customerRefNo = random.Next(Int32.MaxValue).ToString();
                transaction["CustomerMerchantID"] = paymentModel.GatewayTransactionKey;
                transaction["CustomerProfileAction"] = "C";//"create"   => "C","retrieve" => "R","update"   => "U","delete"   => "D"
                transaction["CustomerName"] = string.Concat(paymentModel.BillingFirstName, " ", paymentModel.BillingLastName);
                transaction["CustomerRefNum"] = customerRefNo;
                transaction["CustomerAddress1"] = paymentModel.BillingStreetAddress1;
                transaction["CustomerAddress2"] = string.Concat(paymentModel.BillingStreetAddress2, customerRefNo);
                transaction["CustomerCity"] = paymentModel.BillingCity;
                transaction["CustomerState"] = paymentModel.BillingStateCode;
                transaction["CustomerZIP"] = paymentModel.BillingPostalCode;
                transaction["CustomerEmail"] = paymentModel.BillingEmailId;
                transaction["CustomerPhone"] = paymentModel.BillingPhoneNumber;
                transaction["CustomerCountryCode"] = paymentModel.BillingCountryCode;

                transaction["CCAccountNum"] = paymentModel.CardNumber;
                transaction["CCExpireDate"] = paymentModel.CardExpirationMonth + paymentModel.CardExpirationYear.Substring(2);
                transaction["CustomerProfileOrderOverrideInd"] = "NO";
                transaction["CustomerProfileFromOrderInd"] = "A";
                transaction["CustomerAccountType"] = "CC";

                transaction["OrderDefaultAmount"] = String.Format("{0:0.00}", Convert.ToDecimal(paymentModel.Total));
                transaction["Status"] = "A";

                Response response = transaction.Process();

                if (response.Status.Equals(0))
                {
                    gatewayResponse.IsSuccess = true;
                    gatewayResponse.CustomerProfileId = paymentModel.CustomerProfileId ?? Convert.ToString((new Random()).Next(100000000, 999999999));
                    gatewayResponse.CustomerPaymentProfileId = response["CustomerRefNum"];
                    gatewayResponse.GatewayResponseData = response.Message;
                    gatewayResponse.ResponseText = response.Message;
                    paymentModel.CustomerPaymentProfileId = response["CustomerRefNum"];
                    paymentModel.CustomerProfileId = gatewayResponse.CustomerProfileId;
                }
                else
                {
                    gatewayResponse.IsSuccess = false;
                    gatewayResponse.GatewayResponseData = response["StatusMsg"];
                    gatewayResponse.ResponseText = response.Message;
                }

                return gatewayResponse;
            }
            catch (Exception ex)
            {
                LoggingService.LogActivity(paymentModel.PaymentApplicationSettingId, ex.Message, paymentModel.GUID);
                Logging.LogMessage(ex, Logging.Components.Payment.ToString(), TraceLevel.Error);
                return new GatewayResponseModel { IsSuccess = false, GatewayResponseData = ex.Message };
            }
        }

        //Map Transaction Details
        private void MapTransaction(Transaction transaction, PaymentModel paymentModel, string messageType = "", bool isOrderIdRequired = true, string binName = "BIN")
        {
            // Populate the required fields for the given transaction type. You can use’
            // the Paymentech Transaction Appendix to help you populate the transaction’s
            transaction["OrbitalConnectionUsername"] = paymentModel.GatewayLoginName;
            transaction["OrbitalConnectionPassword"] = paymentModel.GatewayLoginPassword;

            transaction["MerchantID"] = paymentModel.GatewayTransactionKey;
            transaction[binName] = Convert.ToString(ConfigurationManager.AppSettings["PaymentechBIN"]);


            if (!string.IsNullOrEmpty(messageType))
                transaction["MessageType"] = messageType;

            if (isOrderIdRequired && String.IsNullOrEmpty(paymentModel.OrderId))
                paymentModel.OrderId = new Random().Next(Int32.MaxValue).ToString();

            if (isOrderIdRequired)
                transaction["OrderID"] = paymentModel.OrderId;
        }

        public PaymentGatewayTokenModel TokenGenerator(PaymentGatewayTokenModel paymentModel)
        {
            throw new NotImplementedException();
        }
    }
}