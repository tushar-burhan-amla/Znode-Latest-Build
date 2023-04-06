using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Services.Maps;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;

namespace Znode.Engine.Services
{
    public class SMSService: BaseService, ISMSService
    {
        #region Private Variables
        protected readonly IZnodeRepository<ZnodePortalSmsSetting> _znodePortalSmsSettingRepository;
        protected readonly IZnodeRepository<ZnodeSmsProvider> _znodeSMSProviderRepository;
        #endregion

        #region Constructor
        public SMSService()
        {
            _znodePortalSmsSettingRepository = new ZnodeRepository<ZnodePortalSmsSetting>();
            _znodeSMSProviderRepository = new ZnodeRepository<ZnodeSmsProvider>();
        }
        #endregion
        #region Public Methods
        //Get SMS Configuration details for the Portal.
        public virtual SMSModel GetSMSDetails(int portalId,bool isSMSSettingEnabled = false)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Paramters portalId:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info, portalId);
            //Get SMS Configuration details for the portal.
            if (isSMSSettingEnabled)
            {
                SMSModel smsModel = SMSMap.ToModel(_znodePortalSmsSettingRepository.Table.Where(x => x.PortalId == portalId && x.IsSMSSettingEnabled == true)?.FirstOrDefault());
                ZnodeLogging.LogMessage("SMS PortalName:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info, smsModel?.PortalId);
                return smsModel;
            }
            else
            {
                if (portalId > 0)
                {
                    SMSModel smsModel = SMSMap.ToModel(_znodePortalSmsSettingRepository.Table.Where(x => x.PortalId == portalId)?.FirstOrDefault());
                    if (HelperUtility.IsNull(smsModel))
                    {
                        smsModel = new SMSModel();
                        smsModel.SMSProviderId = _znodeSMSProviderRepository.Table.FirstOrDefault().SmsProviderId;
                    }
                    ZnodeLogging.LogMessage("SMS PortalName:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info, smsModel?.PortalId);
                    return smsModel;
                }
            }
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return null;

        }
        //Save the SMS Configuration details for the Portal.
        public virtual bool InsertUpdateSMSSetting(SMSModel smsModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Paramters SMSModel:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info, smsModel);
            if (Equals(smsModel, null))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorModelNull);

            try
            {
                if(HelperUtility.IsNull(smsModel?.AuthToken) && HelperUtility.IsNull(smsModel?.SmsPortalAccountId))
                    return true;
                
                if (smsModel?.PortalId > 0 && smsModel?.PortalSmsSettingId > 0)
                {
                    //If Already SMS Configuration Exists, then Update the SMS Configuration.
                    if (_znodePortalSmsSettingRepository.Update(SMSMap.ToEntity(smsModel)))
                    {
                        ZnodeLogging.LogMessage(string.Format(Admin_Resources.SuccessSmsUpdate, smsModel.PortalId), string.Empty, TraceLevel.Info);
                        smsModel = SMSMap.ToModel(_znodePortalSmsSettingRepository.Table.Where(x => x.PortalId == smsModel.PortalId)?.FirstOrDefault());
                        return true;
                    }
                }
                else
                {
                    //Insert the SMS Configuration Details.
                    ZnodePortalSmsSetting sms = _znodePortalSmsSettingRepository.Insert(SMSMap.ToEntity(smsModel));
                    ZnodeLogging.LogMessage(string.Format(Admin_Resources.SuccessSmsInsert, smsModel.PortalId), string.Empty, TraceLevel.Info);
                    smsModel = SMSMap.ToModel(sms);
                    return true;
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Reports.ToString(), TraceLevel.Error);
                return false;
            }
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return true;

        }
        public virtual List<SMSProviderModel> GetSmsProviderList() => SMSMap.ToSmsProviderModel(_znodeSMSProviderRepository.GetEntityList(string.Empty));
        #endregion
    }
}
