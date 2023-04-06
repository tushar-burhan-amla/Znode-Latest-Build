using System.Collections.Generic;

using Znode.Engine.klaviyo.Models;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Services.Maps
{
    public static class KlaviyoMap
    {
        public static KlaviyoModel ToModel(ZnodePortalKlaviyoSetting klaviyoEntity)
        {
            if (HelperUtility.IsNull(klaviyoEntity))
                return null;

            ZnodeEncryption encryption = new ZnodeEncryption();

            KlaviyoModel klaviyoModel = new KlaviyoModel()
            {
                PortalKlaviyoSettingId = klaviyoEntity.PortalKlaviyoSettingId,
                PortalId = klaviyoEntity.PortalId,
                KlaviyoCode = klaviyoEntity.KlaviyoCode,
                PublicApiKey = klaviyoEntity.PublicApiKey,
                KlaviyoUserName = encryption.DecryptData(klaviyoEntity.UserName),
                KlaviyoPassword = klaviyoEntity.Password,
                IsActive = klaviyoEntity.IsActive
            };
            return klaviyoModel;
        }

        public static ZnodePortalKlaviyoSetting ToEntity(KlaviyoModel klaviyoModel)
        {

            if (HelperUtility.IsNull(klaviyoModel))
                return null;

            ZnodeEncryption encryption = new ZnodeEncryption();
            ZnodePortalKlaviyoSetting klaviyoEntity = new ZnodePortalKlaviyoSetting()
            {
                PortalKlaviyoSettingId = klaviyoModel.PortalKlaviyoSettingId,
                PortalId = klaviyoModel.PortalId.GetValueOrDefault(),
                KlaviyoCode = klaviyoModel.KlaviyoCode,
                PublicApiKey = klaviyoModel.PublicApiKey,
                UserName = encryption.EncryptData(klaviyoModel.KlaviyoUserName),
                Password = klaviyoModel.KlaviyoPassword,
                IsActive = klaviyoModel.IsActive

            };
            return klaviyoEntity;
        }

        //Map list of ZnodeEmailProvider entity to list of EmailProviderModel.
        public static List<EmailProviderModel> ToEmailProviderModel(IList<ZnodeEmailProvider> emailProviderListEntity)
        {
            List<EmailProviderModel> emailProviderList = new List<EmailProviderModel>();
            if (emailProviderListEntity?.Count > 0)
            {
                foreach (ZnodeEmailProvider emailProviderEntity in emailProviderListEntity)
                {
                    emailProviderList.Add(ToEmailProviderModel(emailProviderEntity));
                }
            }
            return emailProviderList;
        }
        //Map ZnodeEmailProvider entity to EmailProviderModel.
        public static EmailProviderModel ToEmailProviderModel(ZnodeEmailProvider emailProvider)
        {

            if (HelperUtility.IsNull(emailProvider))
                return null;
            return new EmailProviderModel
            {
                EmailProviderId = emailProvider.EmailProviderId,
                ProviderCode = emailProvider.ProviderCode,
                ClassName = emailProvider.ClassName,
                ProviderName = emailProvider.ProviderName,

            };
        }
    }
}
