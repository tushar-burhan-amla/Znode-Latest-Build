using Znode.Engine.Api.Client;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Admin.Extensions;
using Znode.Libraries.ECommerce.Utilities;
using System.Diagnostics;
using Znode.Libraries.Framework.Business;
using System;
using Znode.Libraries.Resources;
using Znode.Engine.klaviyo.Models;
using Znode.Engine.Klaviyo.IClient;
using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.Agents
{
    public class KlaviyoAgent : BaseAgent, IKlaviyoAgent
    {
        #region Private Variables
        private readonly IKlaviyoClient _klaviyoClient;
        #endregion

        #region Constructor
        public KlaviyoAgent(IKlaviyoClient klaviyoClient)
        {
            _klaviyoClient = GetComponentClient<IKlaviyoClient>(klaviyoClient);
        }
        #endregion

        #region Public Method
        // Get Klaviyo Details
        public virtual KlaviyoViewModel GetKlaviyo(int portalId, bool isActive)
            => portalId > 0 ? MapKlaviyoModel(_klaviyoClient.GetKlaviyo(portalId)) : new KlaviyoViewModel();

        //Get Email Providers List
        public virtual List<BaseDropDownOptions> GetEmailProviderList()
        {
            List<EmailProviderModel> emailProviderList = _klaviyoClient.GetEmailProviderList();

            List<BaseDropDownOptions> providerlist = new List<BaseDropDownOptions>();

            if (emailProviderList?.Count > 0)
            {
                emailProviderList.ForEach(x =>
                {
                    providerlist.Add(new BaseDropDownOptions()
                    {
                        Id = Convert.ToString(x.EmailProviderId),
                        Text = x.ProviderName,
                        Value = x.ProviderCode,
                        Type = x.ClassName
                    });
                });
            }
            return providerlist;
        }

        public virtual KlaviyoViewModel GetEmailSettingViewData(string providerName, int portalId, bool isActive = false)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
            try
            {
                KlaviyoViewModel klaviyoViewModel = GetKlaviyo(portalId, isActive);
                klaviyoViewModel = HelperUtility.IsNull(klaviyoViewModel) ? new KlaviyoViewModel() : klaviyoViewModel;
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Admin.ToString(), TraceLevel.Info);
                return klaviyoViewModel;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return null;
            }
        }

        protected virtual KlaviyoViewModel MapKlaviyoModel(KlaviyoModel model)
        {
            if (HelperUtility.IsNull(model))
                return new KlaviyoViewModel();
            return new KlaviyoViewModel
            {
                PortalId = Convert.ToInt32(model.PortalId),
                PortalName = model.PortalName,
                PortalKlaviyoSettingId = model.PortalKlaviyoSettingId,
                KlaviyoCode = model.KlaviyoCode,
                PublicApiKey = model.PublicApiKey,
                KlaviyoUserName = model.KlaviyoUserName,
                KlaviyoPassword = model.KlaviyoPassword,
                IsActive = model.IsActive
            };
            
        }
        
        //Update klaviyo Details
        public virtual KlaviyoViewModel UpdateKlaviyo(KlaviyoViewModel klaviyoViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            try
            {
                KlaviyoModel klaviyoModel = _klaviyoClient.UpdateKlaviyo(klaviyoViewModel?.ToModel<KlaviyoModel>());
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
                return HelperUtility.IsNotNull(klaviyoModel) ? MapKlaviyoModel(klaviyoModel) : new KlaviyoViewModel();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Warning);
                return new KlaviyoViewModel { HasError = true, ErrorMessage = ex.ErrorMessage };
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                return (KlaviyoViewModel)GetViewModelWithErrorMessage(klaviyoViewModel, Admin_Resources.UpdateErrorMessage);
            }
        }
        #endregion
    }
}
