using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IPaymentSettingService
    {
        /// <summary>
        /// Create Payment Setting
        /// </summary>
        /// <param name="paymentSettingsModel">paymentSettingsModel</param>
        /// <returns>Payment Settings Model</returns>
        PaymentSettingModel CreatePaymentSetting(PaymentSettingModel paymentSettingsModel);

        /// <summary>
        /// Update Payment Setting
        /// </summary>
        /// <param name="paymentSettingsModel">paymentSettingsModel</param>
        /// <returns>Update Status</returns>
        bool UpdatePaymentSetting(PaymentSettingModel paymentSettingsModel);

        /// <summary>
        /// Get Payment Setting by paymentSettingId
        /// </summary>
        /// <param name="paymentSettingId">paymentSettingId</param>
        /// <param name="expands">expand collection list </param>
        /// <param name="portalId">optional portalId </param>
        /// <returns>Payment Settings Model</returns>
        PaymentSettingModel GetPaymentSetting(int paymentSettingId, NameValueCollection expands, int portalId = 0);

        /// <summary>
        /// Get Payment Setting List
        /// </summary>
        /// <param name="expands">expand collection list </param>
        /// <param name="filters">filter list</param>
        /// <param name="sorts">sort list</param>
        /// <param name="page">paging parameters </param>
        /// <returns>Payment Setting List Model</returns>
        PaymentSettingListModel GetPaymentSettingList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);


        /// <summary>
        /// Associate Payment Settings For Invoice
        /// </summary>
        /// <param name="associationModel">associationModel</param>
        /// <returns>Check payment is associated successfully.</returns>
        bool AssociatePaymentSettingsForInvoice(PaymentSettingAssociationModel associationModel);

        /// <summary>
        /// Delete Payment Setting
        /// </summary>
        /// <param name="paymentSettingId">paymentSettingId</param>
        /// <returns>Delete Status</returns>
        bool DeletePaymentSetting(ParameterModel paymentSettingId);

        /// <summary>
        /// Check Whether Active payment seting present for given Profile and paymentType.
        /// </summary>
        /// <param name="paymentSettingsModel">paymentSettingsModel</param>
        /// <returns>True if payment setting present else false</returns>
        bool IsActivePaymentSettingPresent(PaymentSettingModel paymentSettingsModel);

        /// <summary>
        /// Check Whether Active payment seting present for given Profile and paymentType by paymentCode. 
        /// </summary>
        /// <param name="paymentSettingsModel"></param>
        /// <returns>True if payment setting present else false</returns>
        bool IsActivePaymentSettingPresentByPaymentCode(PaymentSettingModel paymentSettingsModel);

        /// <summary>
        /// Get captured payment details to provide to erp.
        /// </summary>
        /// <param name="omsOrderId">Order Id.</param>
        /// <returns>Returns true if erp receives the payment information and processes it successfully.</returns>
        bool GetCapturedPaymentDetails(int omsOrderId);

        /// <summary>
        /// //Check whether to call payment API by paymentType Code.
        /// </summary>
        /// <param name="paymentTypeCode">paymentType Code</param>
        /// <returns>Returns true if value is set to true else false.</returns>
        bool CallToPaymentAPI(string paymentTypeCode);

        #region Portal/Profile
        /// <summary>
        /// Associate payment settings to portal.
        /// </summary>
        /// <param name="associationModel">Association model.</param>
        /// <returns>Returns true if associated successfully else false.</returns>
        bool AssociatePaymentSettings(PaymentSettingAssociationModel associationModel);

        /// <summary>
        /// Remove associated payment settings to portal.
        /// </summary>
        /// <param name="associationModel">Association model.</param>
        /// <returns>Returns true if association removed successfully else false.</returns>
        bool RemoveAssociatedPaymentSettings(PaymentSettingAssociationModel associationModel);

        /// <summary>
        /// Update portal payment settings.
        /// </summary>
        /// <param name="model">Payment setting portal model.</param>
        /// <returns>Returns true if updated successfully else false.</returns>
        bool UpdatePortalPaymentSettings(PaymentSettingPortalModel model);

        /// <summary>
        /// Update profile payment settings.
        /// </summary>
        /// <param name="model">PaymentSettingAssociationModel</param>
        /// <returns>Returns true if updated successfully else false.</returns>
        bool UpdateProfilePaymentSettings(PaymentSettingAssociationModel model);

        #endregion

        /// <summary>
        /// Is payment display name already exist
        /// </summary>
        /// <param name="paymentSettingValidationModel"></param>
        /// <returns>Return True False Response</returns>
        bool IsPaymentDisplayNameExists(PaymentSettingValidationModel paymentSettingValidationModel);

        /// <summary>
        /// Get list of payment settings by userId and portalId using UserPaymentSettingModel
        /// </summary>
        /// <param name="UserPaymentSettingModel">UserPaymentSettingModel contains userId and portalId</param>
        /// <returns>Returns Payment Setting list.</returns>
        PaymentSettingListModel GetPaymentSettingByUserDetails(UserPaymentSettingModel userPaymentSettingModel);

    }
}
