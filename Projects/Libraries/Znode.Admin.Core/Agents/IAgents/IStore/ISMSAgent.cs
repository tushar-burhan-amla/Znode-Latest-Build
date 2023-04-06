using System.Collections.Generic;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;

namespace Znode.Engine.Admin.Agents
{
    public interface ISMSAgent
    {
        /// <summary>
        /// Get SMS for specified portal Id.
        /// </summary>
        /// <param name="portalId">int portalId to get sms.</param>
        /// <returns>SMS ViewModel.</returns>
       // PortalSmsViewModel GetSmsDetails(int portalId);

        /// <summary>
        /// Get SMS Portal List for specified portal Id.
        /// </summary>
        /// <param name="portalId">int portalId to get sms.</param>
        /// <returns>SMS ViewModel.</returns>
        List<BaseDropDownOptions> GetSmsProviderList();
        /// <summary>
        /// Insert and Update SMS setting.
        /// </summary>
        /// <param name="smsViewModel"></param>
        /// <returns>SMS ViewModel.</returns>
        PortalSMSViewModel InsertUpdateSMSSetting(PortalSMSViewModel smsViewModel);
        /// <summary>
        /// Get SMS Portal List for specified portal Id and Enable SMS Setting.
        /// </summary>
        /// <param name="providerName"></param>
        /// <param name="portalId">int portalId to get sms.</param>
        /// <param name="isSMSSettingEnabled"></param>
        /// <returns>SMS ViewModel.</returns>
        PortalSMSViewModel GetSmsSettingViewData(string providerName,int portalid, bool isSMSSettingEnabled = false);
    }
}
