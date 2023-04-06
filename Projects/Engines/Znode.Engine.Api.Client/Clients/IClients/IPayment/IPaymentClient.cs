using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IPaymentClient : IBaseClient
    {
        /// <summary>
        /// Get all Payment Gateways
        /// </summary>
        /// <returns>Payment Gateway ListModel</returns>
        PaymentGatewayListModel GetGateways();

        /// <summary>
        /// Get All Payment Types
        /// </summary>
        /// <returns>Payment Type ListModel</returns>
        PaymentTypeListModel GetPaymentTypes();

        /// <summary>
        /// Create Payment setting
        /// </summary>
        /// <param name="model">Model of Payment setting</param>
        /// <param name="isPaymentApplication">True to call payment application Api</param>
        /// <returns>Payment Setting Model</returns>
        PaymentSettingModel CreatePaymentSetting(PaymentSettingModel model, bool isPaymentApplication = false);

        /// <summary>
        /// Update Payment setting
        /// </summary>
        /// <param name="model">Model of Payment setting</param>
        /// <param name="isPaymentApplication">True to call payment application Api</param>
        /// <returns>String Response</returns>
        PaymentSettingModel UpdatePaymentSetting(PaymentSettingModel model, bool isPaymentApplication = false);

        /// <summary>
        /// Get Payment Setting
        /// </summary>
        /// <param name="paymentsettingId">Id To get Payment Setting</param>
        /// <param name="isPaymentApplication">True to call payment application Api</param>
        /// <param name="expands">Expands to be retrieved along with Payment Settings list.</param>
        /// <param name="portalId">Optional portalId</param>
        /// <returns>payment Setting Model</returns>
        PaymentSettingModel GetPaymentSetting(int paymentsettingId, bool isPaymentApplication = false, ExpandCollection expands = null, int portalId = 0);

        /// <summary>
        /// Delete payment setting from payment application
        /// </summary>
        /// <param name="paymentSettingIds">Ids To delete Payment Setting</param>
        /// <param name="isPaymentApplication">True to call payment application Api</param>
        /// <returns>True if deleted successfully else return false</returns>
        bool DeletePaymentSetting(string paymentSettingIds, bool isPaymentApplication = false);

        /// <summary>
        /// Get All Payment Settings
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with Payment Settings list.</param>
        /// <param name="filters">Filters to be applied on Payment Settings list.</param>
        /// <param name="sorts">Sorting to be applied on Payment Settings list.</param>
        /// <param name="pageIndex">Start page index of Payment Settings list.</param>
        /// <param name="pageSize">Page size of Payment Settings list.</param>
        /// <returns>Payment Setting List Model </returns>
        PaymentSettingListModel GetPaymentSettings(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get Payment Setting Credentials by PaymentsettingId and test mode status
        /// </summary>
        /// <param name="paymentsettingId">payment setting Id</param>
        /// <param name="isTestMode">true for test mode else set false</param>
        /// <returns>Payment Setting Credentials Model</returns>
        PaymentSettingCredentialModel GetPaymentSettingCredentials(int paymentsettingId, bool isTestMode);

        /// <summary>
        /// Check Whether Active payment seting present for given Profile and paymentType.
        /// </summary>
        /// <param name="paymentSettingModel">payment Settings Model</param>
        /// <returns>True if payment setting present else false</returns>
        bool IsActivePaymentSettingPresent(PaymentSettingModel paymentSettingModel);

        /// <summary>
        /// Check whether active payment seting present for given profile and paymentType by paymentCode.
        /// </summary>
        /// <param name="paymentSettingModel">payment Settings Model</param>
        /// <returns>True if payment setting present else false</returns>
        bool IsActivePaymentSettingPresentByPaymentCode(PaymentSettingModel paymentSettingModel);

        /// <summary>
        /// Call PayNow method in Payment Application
        /// </summary>
        /// <param name="submitPaymentModel">Submit Payment Model</param>
        /// <returns>Gateway Response Model</returns>
        GatewayResponseModel PayNow(SubmitPaymentModel submitPaymentModel);

        /// <summary>
        /// Call PayPal method in Payment Application
        /// </summary>
        /// <param name="submitPaymentModel">Submit Payment Model</param>
        /// <returns>Gateway Response Model</returns>
        GatewayResponseModel PayPal(SubmitPaymentModel submitPaymentModel);

        /// <summary>
        /// Call Amazon pay method in payment application.
        /// </summary>
        /// <param name="model">Submit Payment Model</param>
        /// <returns>Gateway Response Model</returns>
        GatewayResponseModel AmazonPay(SubmitPaymentModel model);

        /// <summary>
        /// Capture payment 
        /// </summary>
        /// <param name="paymentTransactionToken">paymentTransactionToken</param>
        /// <returns>BooleanModel</returns>
        BooleanModel CapturePayment(string paymentTransactionToken);

        /// <summary>
        ///Amazon Capture payment.
        /// </summary>
        /// <param name="paymentTransactionToken"></param>
        /// <returns>BooleanModel</returns>
        BooleanModel AmazonCapturePayment(string paymentTransactionToken);

        /// <summary>
        /// Void payment 
        /// </summary>
        /// <param name="paymentTransactionToken">paymentTransactionToken</param>
        /// <returns>BooleanModel</returns>
        BooleanModel VoidPayment(string paymentTransactionToken);

        /// <summary>
        ///  Refund Payment
        /// </summary>
        /// <param name="model">RefundPaymentModel</param>
        /// <returns>BooleanModel</returns>
        BooleanModel RefundPayment(RefundPaymentModel model);

        /// <summary>
        /// Get saved credit card details 
        /// </summary>
        /// <param name="paymentSettingId">paymentSettingId</param>
        /// <param name="customersGUID">customersGUID</param>
        /// <returns>PaymentMethodCCDetailsListModel</returns>
        PaymentMethodCCDetailsListModel GetPaymentCreditCardDetails(int paymentSettingId, string customersGUID);

        /// <summary>
        /// Get saved credit card details 
        /// </summary>
        /// <param name="paymentSettingId">paymentSettingId</param>
        /// <param name="customersGUID">customersGUID</param>
        /// <returns>Count of saved credit card.</returns>
        int GetSaveCreditCardCount(int paymentSettingId, string customersGUID);

        /// <summary>
        /// Delete saved credit cards.
        /// </summary>
        /// <param name="paymentGUID">paymentGUID</param>
        /// <returns>true or false</returns>
        bool DeleteSavedCreditCardDetail(string paymentGUID);

        /// <summary>
        /// Call PayPal method in Payment Application
        /// </summary>
        /// <param name="submitPaymentModel">Submit paypal Model</param>
        /// <returns>Response from Gateway API</returns>
        GatewayResponseModel FinalizePayPalProcess(SubmitPaymentModel submitPaymentModel);

        /// <summary>
        /// Get amazon pay address details.
        /// </summary>
        /// <param name="model">SubmitPaymentModel</param>
        /// <returns>SubmitPaymentModel</returns>
        SubmitPaymentModel GetAmazonPayAddressDetails(SubmitPaymentModel model);

        /// <summary>
        ///  Refund Amazon Payment.
        /// </summary>
        /// <param name="model">RefundPaymentModel</param>
        /// <returns>BooleanModel</returns>
        BooleanModel AmazonPayRefund(RefundPaymentModel model);

        // <summary>
        /// Amazon void payment. 
        /// </summary>
        /// <param name="paymentTransactionToken">paymentTransactionToken</param>
        /// <returns>BooleanModel</returns>
        BooleanModel AmazonVoidPayment(string paymentTransactionToken);

        /// <summary>
        /// Get the captured payment details for providing it to ERP.
        /// </summary>
        /// <param name="omsOrderId">Order Id.</param>
        /// <returns>Returns true if erp receives the payment information and processes it successfully.</returns>
        bool GetCapturedPaymentDetails(int omsOrderId);

        /// <summary>
        /// Get Payment Setting by paymentCode
        /// </summary>
        /// <param name="paymentCode">Code to get payment setting</param>
        /// <returns>returns payment Setting Model</returns>
        PaymentSettingModel GetPaymentSettingByPaymentCode(string paymentCode);

        /// <summary>
        /// Check whether to call payment API by paymentTypeCode.
        /// </summary>
        /// <param name="paymentTypeCode">paymentType Code</param>
        /// <returns>True if payment IsCallToPaymentAPI is set to true else false</returns>
        bool CallToPaymentAPI(string paymentTypeCode);

        /// <summary>
        /// Get Payment Setting Credentials by paymentCode and test mode status
        /// </summary>
        /// <param name="paymentCode">payment code</param>
        /// <param name="isTestMode">true for test mode else set false</param>
        /// <returns>Payment Setting Credentials Model</returns>
        PaymentSettingCredentialModel GetPaymentSettingCredentialsByPaymentCode(string paymentCode, bool isTestMode);

        /// <summary>
        /// Get saved card details by customers GUID
        /// </summary>
        /// <param name="customersGUID">string customersGUID</param>
        /// <returns>PaymentMethodCCDetailsListModel</returns>
        PaymentMethodCCDetailsListModel GetSavedCardDetailsByCustomerGUID(string customersGUID);

        /// <summary>
        /// Get PaymentAuthToken based on random GUID 
        /// </summary>
        /// <returns>Payment token string</returns>
        string GetPaymentAuthToken(string userOrSessionId, bool fromAdminApp = false);

        #region Portal/Profile
        /// <summary>
        /// Associate payment settings to portal.
        /// </summary>
        /// <param name="model">Association model.</param>
        /// <returns>Returns true if associated successfully else false.</returns>
        bool AssociatePaymentSettings(PaymentSettingAssociationModel model);

        /// <summary>
        /// Remove associated payment settings to portal.
        /// </summary>
        /// <param name="model">Association model.</param>
        /// <returns>Returns true if association removed successfully else false.</returns>
        bool RemoveAssociatedPaymentSettings(PaymentSettingAssociationModel model);

        /// <summary>
        /// Update portal payment settings.
        /// </summary>
        /// <param name="model">Payment setting portal model.</param>
        /// <returns>Returns true if updated successfully else false.</returns>
        bool UpdatePortalPaymentSettings(PaymentSettingPortalModel model);

        /// <summary>
        /// Get payment transaction status details.
        /// </summary>
        /// <param name="transactionId">Transaction Id</param>
        /// <returns>TransactionDetailsModel</returns>
        TransactionDetailsModel GetTransactionStatusDetails(string transactionId);

        /// <summary>
        /// Update profile payment setting
        /// </summary>
        /// <param name="model">Payment Setting Association Model</param>
        /// <returns>Returns true if updated successfully else false.</returns>
        bool UpdateProfilePaymentSetting(PaymentSettingAssociationModel model);

        #endregion

        /// <summary>
        /// Is payment display name already exist
        /// </summary>
        /// <param name="paymentSettingValidationModel"></param>
        /// <returns>Return True False Response</returns>
        bool IsPaymentDisplayNameExists(PaymentSettingValidationModel paymentSettingValidationModel);

        /// <summary>
        /// Get all payment settings by userId and portalId using UserPaymentSettingModel.
        /// </summary>
        /// <param name="UserPaymentSettingModel">Current UserId and PortalId is binded.</param>    
        /// <returns>Payment Setting List Model</returns>
        PaymentSettingListModel GetPaymentSettingByUserDetails(UserPaymentSettingModel userPaymentSettingModel);

        /// <summary>
        /// Get all Payment Gateways for ACH
        /// </summary>
        /// <returns>Payment Gateway ListModel</returns>
        PaymentGatewayListModel GetGatewaysForACH();

        /// <summary>
        /// Associate Payment Settings For Invoice
        /// </summary>
        /// <param name="PaymentSettingAssociationModel"></param>
        /// <returns>Return True False Response</returns>
        bool AssociatePaymentSettingsForInvoice(PaymentSettingAssociationModel model);

       
        /// <summary>
        /// Get Payment Gateway Token
        /// </summary>
        /// <param name="paymentTokenModel">Payment Gateway Token Model</param>
        /// <returns>Payment Gateway Token Model</returns>
        PaymentGatewayTokenModel GetPaymentGatewayToken(PaymentGatewayTokenModel paymentTokenModel);


        /// <summary>
        /// Get refund transaction id.
        /// </summary>
        /// <param name="transactionId">Transaction Id</param>
        /// <returns>transactionId</returns>
        string GetRefundTransactionId(string transactionId);
    }
}
