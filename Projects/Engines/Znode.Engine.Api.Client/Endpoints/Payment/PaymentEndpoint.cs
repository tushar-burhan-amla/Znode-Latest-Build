namespace Znode.Engine.Api.Client.Endpoints
{
    public class PaymentEndpoint : BaseEndpoint
    {
        //Get List of Payment Settings
        public static string List() => $"{ApiRoot}/payment/list";

        //Get of Payment setting
        public static string Get(int paymentsettingId, int portalId = 0) => $"{ApiRoot}/payment/{paymentsettingId}/{portalId}";

        //Add new Payment Settings
        public static string Create() => $"{ApiRoot}/payment/create";

        //Update Payment Settings
        public static string Update() => $"{ApiRoot}/payment/update";

        //Delete Payment Settings
        public static string Delete() => $"{ApiRoot}/payment/delete";

        //Check active Payment Settings present or not
        public static string IsActivePaymentSettingPresent() => $"{ApiRoot}/payment/isactivepaymentsettingpresent";

        //Check active Payment Settings present or not by paymentCode
        public static string IsActivePaymentSettingPresentByPaymentCode() => $"{ApiRoot}/payment/isactivepaymentsettingpresentbypaymentcode";

        //Check whether to call payment API by paymentTypeCode.
        public static string CallToPaymentAPI(string paymentTypeCode) => $"{ApiRoot}/payment/calltopaymentapi/{paymentTypeCode}";

        #region Payment Application endpoints

        //Get Payment Gateways from Payment Application
        public static string GetGatewaysFromPaymentApplication() => $"{PaymentApiRoot}/payment/getgateways";

        //Get Payment types from Payment Application
        public static string GetPaymentTypesFromPaymentApplication() => $"{PaymentApiRoot}/payment/getpaymenttypes";

        //Delete Payment setting from Payment Application
        public static string DeleteFromPaymentApplication() => $"{PaymentApiRoot}/payment/delete";

        //Create Payment Setting from Payment Application
        public static string CreateOnPaymentApplication() => $"{PaymentApiRoot}/payment/addpaymentsettings";

        //Get of Payment setting from Payment Application
        public static string GetFromPaymentApplication(int paymentsettingId) => $"{PaymentApiRoot}/payment/getpaymentsettingdetails/{paymentsettingId}";

        //Update Payment Settings in Payment Application
        public static string UpdateOnPaymentApplication() => $"{PaymentApiRoot}/payment/updatepaymentsettings";

        //Get Payment Setting credentials from Payment Application
        public static string GetCredentialsFromPaymentApplication(int paymentsettingId, bool isTestMode)
                   => $"{PaymentApiRoot}/payment/getpaymentsettingcredentials/{paymentsettingId}/{isTestMode}";

        //Get Payment Setting credentials from Payment Application by paymentCode
        public static string GetCredentialsFromPaymentApplicationByPaymentCode(string paymentCode, bool isTestMode)
                   => $"{PaymentApiRoot}/payment/getpaymentsettingcredentialsbypaymentcode/{paymentCode}/{isTestMode}";

        // Process Payment operation
        public static string PayNow() => $"{PaymentApiRoot}/payment/paynow";

        // Process Payment operation
        public static string PayPal() => $"{PaymentApiRoot}/payment/paypal";

        // Finalize paypal Payment operation
        public static string FinalizePayPalProcess() => $"{PaymentApiRoot}/payment/finalizepaypalprocess";

        // Process AmazonPay operation
        public static string AmazonPay() => $"{PaymentApiRoot}/payment/amazonpay";

        // Process AmazonPay operation
        public static string GetAmazonPayAddress() => $"{PaymentApiRoot}/payment/getamazonpayaddress";
        //Capture Payment
        public static string CapturePayment(string token) => $"{PaymentApiRoot}/payment/capture/{token}";

        public static string AmazonCapturePayment(string token) => $"{PaymentApiRoot}/payment/amazonpaycapture/{token}";
        //Void Payment
        public static string VoidPayment(string token) => $"{PaymentApiRoot}/payment/void/{token}";

        //Amazon void Payment
        public static string AmazonVoidPayment(string token) => $"{PaymentApiRoot}/payment/amazonvoid/{token}";

        //Refund Payment
        public static string RefundPayment() => $"{PaymentApiRoot}/payment/refund";

        //Refund amazon payment
        public static string AmazonPayRefundPayment() => $"{PaymentApiRoot}/payment/amazonpayrefund";

        //Get Payment Setting credentials from Payment Application
        public static string GetPaymentCreditCardDetails(int paymentsettingId, string customersGUID)
                   => $"{PaymentApiRoot}/payment/GetPaymentCreditCardDetails/{paymentsettingId}/{customersGUID}";

        //Get Saved credit card Count
        public static string GetSaveCreditCardCount(int paymentsettingId, string customersGUID)
                   => $"{PaymentApiRoot}/payment/GetPaymentCreditCardCount/{paymentsettingId}/{customersGUID}";

        //Delete Saved credit card Count
        public static string DeleteSavedCreditCardDetail(string paymentGUID)
                   => $"{PaymentApiRoot}/payment/DeleteSavedCreditCardDetail/{paymentGUID}";

        //Get captured payment details.
        public static string GetCapturedPaymentDetails(int omsOrderId) => $"{ApiRoot}/payment/paymentcaptured/{omsOrderId}";

        //Get of Payment setting from Payment Application by paymentCode
        public static string GetPaymentApplicationSettingByPaymentCode(string paymentCode) => $"{PaymentApiRoot}/payment/getpaymentsettingbypaymentcode/{paymentCode}";

        //Delete payment setting from Payment Application by paymentCode
        public static string DeletePaymentSettingByPaymentCode() => $"{PaymentApiRoot}/payment/deletepaymentsettingbypaymentcode";

        //Get saved credit card details from payment application by customers GUID
        public static string GetSavedCardDetailsByCustomerGUID(string customersGUID)
                   => $"{PaymentApiRoot}/payment/getsavedcarddetailsbycustomerguid/{customersGUID}";

        //Get transaction status details.
        public static string GetTransactionStatusDetails(string transactionId) => $"{PaymentApiRoot}/payment/GetTransactionStatusDetails/{transactionId}";

        //Get AuthToken for payment process
        public static string GetPaymentAuthToken(string userOrSessionId, bool fromAdminApp = false) => $"{PaymentApiRoot}/authtoken/paymentaccesstokengenerator/{userOrSessionId}/{fromAdminApp}";
        #endregion

        #region Portal/Profile Endpoint
        // Associate payment settings.
        public static string AssociatePaymentSettings() => $"{ApiRoot}/payment/associatepaymentsettings";

        // Associate payment settings for offline payment.
        public static string AssociatePaymentSettingsForInvoice() => $"{ApiRoot}/payment/associatepaymentsettingsforinvoice";

        // Remove associated payment settings.
        public static string RemoveAssociatedPaymentSettings() => $"{ApiRoot}/payment/removeassociatedpaymentsettings";

        // Update portal payment settings.
        public static string UpdatePortalPaymentSettings() => $"{ApiRoot}/payment/updateportalpaymentsettings";

        // Update profile payment settings.
        public static string UpdateProfilePaymentSettings() => $"{ApiRoot}/payment/updateprofilepaymentsettings";

        #endregion

        // Is payment display name already exist
        public static string IsPaymentDisplayNameExists() => $"{ApiRoot}/payment/IsPaymentDisplayNameExists";

        //Get List of Payment Settings by userId and portalId using UserPaymentSettingModel
        public static string GetPaymentSettingByUserDetails() => $"{ApiRoot}/payment/getpaymentsettingbyuserdetails";

        //Get Payment Gateways from Payment Application for ACH.
        public static string GetGatewaysFromPaymentApplicationForACH() => $"{PaymentApiRoot}/payment/getachgateways";

        //Get Payment Gateway Token
        public static string GetPaymentGatewayToken() => $"{PaymentApiRoot}/payment/generategatewaytoken";

        //Get Payment refund transaction id
        public static string GetRefundTransactionId(string transactionId) => $"{PaymentApiRoot}/payment/GetRefundTransactionId/{transactionId}";
    }
}
