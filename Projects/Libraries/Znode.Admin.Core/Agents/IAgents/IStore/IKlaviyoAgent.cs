using System.Collections.Generic;

using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;

namespace Znode.Engine.Admin.Agents
{
    public interface IKlaviyoAgent
    {
        /// <summary>
        /// Get Klaviyo for specified portal Id.
        /// </summary>
        /// <param name="portalId">int portalId to get Klaviyo.</param>
        /// <returns>KlaviyoViewModel.</returns>
        KlaviyoViewModel GetKlaviyo(int portalId, bool isActive);

        /// <summary>
        ///  Update Klaviyo associated to portal.
        /// </summary>
        /// <param name="klaviyoViewModel">KlaviyoViewModel to update klaviyo.</param>
        /// <returns>KlaviyoViewModel.</returns>
        KlaviyoViewModel UpdateKlaviyo(KlaviyoViewModel klaviyoViewModel);
        /// <summary>
        ///  To get Email Provider List.
        /// </summary>
        /// <param name="BaseDropDownOptions">To get Email Provider list for klaviyo configuration.</param>
        /// <returns>BaseDropDownOptions.</returns>
        List<BaseDropDownOptions> GetEmailProviderList();

        /// <summary>
        ///  To get data for klaviyo configuration.
        /// </summary>
        /// <param name="klaviyoViewModel">KlaviyoViewModel to get klaviyo details.</param>
        /// <returns>KlaviyoViewModel.</returns>
        KlaviyoViewModel GetEmailSettingViewData(string providerName, int portalId, bool isActive = false);
    }
}
