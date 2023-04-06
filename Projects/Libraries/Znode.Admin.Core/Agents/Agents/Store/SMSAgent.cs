using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.Agents
{
    class SMSAgent : BaseAgent, ISMSAgent
    {
        #region Private Variables
        private readonly IPortalSMSClient _smsClient;
        #endregion

        #region Constructor
        public SMSAgent(IPortalSMSClient smsClient)
        {
            _smsClient = GetClient<IPortalSMSClient>(smsClient);
        }
        #endregion

        #region Public Methods

        //Update Sms Details
        public virtual PortalSMSViewModel InsertUpdateSMSSetting(PortalSMSViewModel smsViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            try
            {
                SMSModel smsModel = _smsClient.InsertUpdateSMSSetting(smsViewModel?.ToModel<SMSModel>());
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
                return HelperUtility.IsNotNull(smsModel) ? smsModel?.ToViewModel<PortalSMSViewModel>() : new PortalSMSViewModel();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Warning);
                return new PortalSMSViewModel { HasError = true, ErrorMessage = ex.ErrorMessage };
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                return (PortalSMSViewModel)GetViewModelWithErrorMessage(smsViewModel, Admin_Resources.UpdateErrorMessage);
            }
        }
        //Get SMS Details
        protected virtual PortalSMSViewModel GetSmsDetails(int portalId,bool isSMSSettingEnabled = false)
            => portalId > 0 ? _smsClient.GetSMSSetting(portalId,isSMSSettingEnabled)?.ToViewModel<PortalSMSViewModel>() : new PortalSMSViewModel();

        //Get SMS Providers List
        public virtual List<BaseDropDownOptions> GetSmsProviderList()
        {
            List<SMSProviderModel> smsProviderList = _smsClient.GetSmsProviderList();

            List<BaseDropDownOptions> providerlist = new List<BaseDropDownOptions>();
            
                if (smsProviderList?.Count > 0)
                {
                smsProviderList.ForEach(x =>
                    {
                        providerlist.Add(new BaseDropDownOptions()
                        {
                            Id = Convert.ToString(x.SMSProviderId),
                            Text = x.ProviderName,
                            Value = x.ProviderCode,
                            Type = x.ClassName
                        });
                    });
                }
            return providerlist;
        }
        public virtual PortalSMSViewModel GetSmsSettingViewData(string providerName, int portalId,bool isSMSSettingEnabled = false)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            try
            {
                PortalSMSViewModel portalSmsViewModel = GetSmsDetails(portalId,isSMSSettingEnabled);
                portalSmsViewModel = HelperUtility.IsNull(portalSmsViewModel) ? new PortalSMSViewModel() : portalSmsViewModel;
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                return portalSmsViewModel;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return null;
            }

        }
        #endregion
    }
}

