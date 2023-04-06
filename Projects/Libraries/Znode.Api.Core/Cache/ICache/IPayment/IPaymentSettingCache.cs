using Znode.Engine.Api.Models;
namespace Znode.Engine.Api.Cache
{
    public interface IPaymentSettingCache
    {
        /// <summary>
        /// Get Payment Setting list.
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns Payment Setting list.</returns>
        string GetPaymentSettings(string routeUri, string routeTemplate);

        /// <summary>
        /// Get Payment Setting on the basis of paymentSettingId.
        /// </summary>
        /// <param name="paymentSettingId">payment Setting Id.</param>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <param name="portalId">optional portalId.</param>
        /// <returns>Returns GiftCard.</returns>
        string GetPaymentSetting(int paymentSettingId, string routeUri, string routeTemplate, int portalId = 0);

        /// <summary>
        ///  Get list of payment settings by userId and portalId using UserPaymentSettingModel
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <param name="userPaymentSettingModel">UserPaymentSettingModel</param>
        /// <returns>Returns Payment Setting list.</returns>
        string GetPaymentSettingByUserDetails(string routeUri, string routeTemplate, UserPaymentSettingModel userPaymentSettingModel);

    }
}
