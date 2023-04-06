using System.Collections.Generic;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Services.Maps
{
    public static class SMSMap
    {
        public static SMSModel ToModel(ZnodePortalSmsSetting smsEntity)
        {
            if (Equals(smsEntity, null))
                return null;

            ZnodeEncryption encryption = new ZnodeEncryption();

            SMSModel smsModel = new SMSModel()
            {
                PortalSmsSettingId = smsEntity.PortalSmsSettingId,
                PortalId = smsEntity.PortalId,
                SMSProviderId = smsEntity.SmsProviderId,
                SmsPortalAccountId = smsEntity.SmsPortalAccountId,
                AuthToken = smsEntity.AuthToken,
                FromMobileNumber = smsEntity.FromMobileNumber,
                IsSMSSettingEnabled = smsEntity.IsSMSSettingEnabled
            };
            return smsModel;
        }

        public static ZnodePortalSmsSetting ToEntity(SMSModel smsModel)
        {
            if (Equals(smsModel, null))
                return null;
            ZnodePortalSmsSetting smsEntity = new ZnodePortalSmsSetting()
            {
                PortalSmsSettingId = smsModel.PortalSmsSettingId,
                PortalId = smsModel.PortalId,
                SmsProviderId = smsModel.SMSProviderId,
                SmsPortalAccountId = smsModel.SmsPortalAccountId,
                AuthToken = smsModel.AuthToken,
                FromMobileNumber = smsModel.FromMobileNumber,
                IsSMSSettingEnabled = smsModel.IsSMSSettingEnabled
            };
            return smsEntity;
        }

        //Map list of ZnodeSMSProvider entity to list of SMSProviderModel.
        public static List<SMSProviderModel> ToSmsProviderModel(IList<ZnodeSmsProvider> smsProviderListEntity)
        {
            List<SMSProviderModel> smsProviderList = new List<SMSProviderModel>();
            if (smsProviderListEntity?.Count > 0)
            {
                foreach (ZnodeSmsProvider smsProviderEntity in smsProviderListEntity)
                {
                    smsProviderList.Add(ToSmsProviderModel(smsProviderEntity));
                }
            }
            return smsProviderList;
        }
        //Map ZnodeSMSProvider entity to SMSProviderModel.
        public static SMSProviderModel ToSmsProviderModel(ZnodeSmsProvider smsProvider)
        {
            if (IsNull(smsProvider))
                return null;

            return new SMSProviderModel
            {
                SMSProviderId = smsProvider.SmsProviderId,
                ProviderCode = smsProvider.ProviderCode,
                ClassName = smsProvider.ClassName,
                ProviderName = smsProvider.ProviderName,
            };
        }
    }
}
