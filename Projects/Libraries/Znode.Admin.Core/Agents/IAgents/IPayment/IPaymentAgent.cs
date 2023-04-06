using Znode.Engine.Api.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Expands;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;

namespace Znode.Engine.Admin.Agents
{
    public interface IPaymentAgent
    {
        /// <summary>
        ///  Bind Profiles, PaymentTypes and PaymentGateways and IsCallPaymentAPI to PaymentSettingViewModel model
        /// </summary>
        /// <param name="paymentSettingViewModel">PaymentSettingViewModel</param>
        /// <param name="paymentTypeCode">paymentTypeCode</param>
        /// <returns>PaymentSettingViewModel</returns>
        PaymentSettingViewModel GetPaymentSettingViewData(PaymentSettingViewModel paymentSettingViewModel = null, string paymentTypeCode = "");

        /// <summary>
        /// Add PaymentOption in payment Database
        /// </summary>
        /// <param name="PaymentSettingViewModel">PaymentSettingViewModel</param>
        /// <returns>Payment Setting View Model</returns>
        PaymentSettingViewModel AddPaymentSetting(PaymentSettingViewModel paymentSettingViewModel);

        /// <summary>
        /// Get Payment Setting
        /// </summary>
        /// <param name="paymenSettingId">Id To get Payment Setting</param>
        /// <param name="portalId">Optional portal Id</param>
        /// <returns>payment Setting View Model</returns>
        PaymentSettingViewModel GetPaymentSetting(int paymentSettingId, int portalId = 0);

        /// <summary>
        /// Update Payment setting
        /// </summary>
        /// <param name="model">Model of Payment setting</param>
        /// <returns>Payment Setting View Model</returns>
        PaymentSettingViewModel UpdatePaymentSetting(PaymentSettingViewModel paymentSettingViewModel);

        /// <summary>
        /// Delete Payment Setting
        /// </summary>
        /// <param name="paymentSettingIds">Ids To delete Payment Setting</param>
        /// <param name="errorMessage">error Message Occurred during delete</param> 
        /// <returns>True if deleted successfully else return false</returns>
        bool DeletePaymentSetting(string paymentSettingIds, out string errorMessage);

        /// <summary>
        /// Get All Payment Settings
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with Payment Settings list.</param>
        /// <param name="filters">Filters to be applied on Payment Settings list.</param>
        /// <param name="sorts">Sorting to be applied on Payment Settings list.</param>
        /// <param name="pageIndex">Start page index of Payment Settings list.</param>
        /// <param name="pageSize">Page size of Payment Settings list.</param>
        /// <returns>Payment Setting List Model </returns>
        PaymentSettingListViewModel List(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get Payment Type View name 
        /// </summary>
        /// <param name="paymentCode">paymentCode</param>
        /// <returns>Payment Type View name</returns>
        string GetPaymentTypeView(string paymentCode);

        /// <summary>
        ///  Get Payment Gateway View name 
        /// </summary>
        /// <param name="gatewayCode">Payment Gateway Code </param>
        /// <returns>Payment Gateway View name</returns>
        string GetPaymentGatewayView(string gatewayCode);

        /// <summary>
        /// Get Payment Setting Credentials by PaymentsettingId and test mode status
        /// </summary>
        /// <param name="paymentsettingId">payment setting Id</param>
        /// <param name="isTestMode">true for test mode else set false</param>
        /// <returns>Payment setting view model</returns>
        PaymentSettingViewModel GetPaymentSettingCredentials(int paymentsettingId, bool isTestMode);

        /// <summary>
        /// This method will use to call the payment and process the order
        /// </summary>
        /// <param name="submitPaymentModel">Submit Payment Model</param>
        /// <returns>GatewayResponseModel</returns>
        GatewayResponseModel ProcessPayNow(SubmitPaymentModel submitPaymentModel);

        /// <summary>
        /// This method will use to call the Paypal Express Checkout payment and process the order
        /// </summary>
        /// <param name="submitPaymentModel">Submit Payment Model</param>
        /// <returns>GatewayResponseModel</returns>
        GatewayResponseModel ProcessPayPal(SubmitPaymentModel submitPaymentModel);

        /// <summary>
        /// Check whether the payment name already exists.
        /// </summary>
        /// <param name="paymentCode">paymentName</param>
        /// <param name="paymentSettingId">paymentSettingId</param>
        /// <returns>returns true if exists else false.</returns>
        bool CheckPaymentCodeExist(string paymentCode, int paymentSettingId);

        /// <summary>
        /// Parse Json string to PaymentSettingViewModel
        /// </summary>
        /// <param name="paymentSetting">paymentSetting</param>
        /// <returns>PaymentSettingViewModel</returns>
        PaymentSettingViewModel ParseStringToPaymentSettingViewModel(string paymentSetting);

        /// <summary>
        /// Get Payment Setting Credentials by paymentCode and test mode status
        /// </summary>
        /// <param name="paymentCode">string paymentCode</param>
        /// <param name="isTestMode">bool isTestMode</param>
        /// <returns>returns payment setting view model</returns>
        PaymentSettingViewModel GetPaymentSettingCredentialsByPaymentCode(string paymentCode, bool isTestMode, string paymentTypeCode = "");

        /// <summary>
        /// Check whether the payment display name already exists.
        /// </summary>
        /// <param name="paymentSettingValidationViewModel"></param>
        /// <returns>Return True False Response</returns>
        bool IsPaymentDisplayNameExists(PaymentSettingValidationViewModel paymentSettingValidationViewModel);

        /// <summary>
        /// Get Payment AuthToken.
        /// </summary>
        /// <returns>AuthToken string</returns>
        string GetPaymentAuthToken(string userOrSessionId, bool fromAdminApp);
        
        /// <summary>
        /// To get the Gateway Token.
        /// </summary>
        /// <returns>PaymentTokenViewModel</returns>
        PaymentTokenViewModel GetPaymentGatewayToken(PaymentTokenViewModel paymentTokenModel);
    }
}
