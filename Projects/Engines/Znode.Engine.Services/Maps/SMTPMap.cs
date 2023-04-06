using Znode.Engine.Api.Models;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Services.Maps
{
    public static class SMTPMap
    {
        public static SMTPModel ToModel(ZnodePortalSmtpSetting smtpEntity)
        {
            if (Equals(smtpEntity, null))
                return null;

            ZnodeEncryption encryption = new ZnodeEncryption();

            SMTPModel smtpModel = new SMTPModel()
            {
                SmtpId = smtpEntity.PortalSmtpSettingId,
                PortalId = smtpEntity.PortalId,
                SmtpPort = smtpEntity.Port,
                SmtpServer = smtpEntity.ServerName,
                SmtpUsername = encryption.DecryptData(smtpEntity.UserName),
                SmtpPassword = smtpEntity.Password,
                EnableSslForSmtp = smtpEntity.IsEnableSsl,
                FromDisplayName = smtpEntity.FromDisplayName,
                BccEmailAddress = smtpEntity.BccEmailAddress,
                FromEmailAddress = smtpEntity.FromEmailAddress,
                DisableAllEmailsForSmtp = smtpEntity.DisableAllEmails
            };
            return smtpModel;
        }

        public static ZnodePortalSmtpSetting ToEntity(SMTPModel smtpModel)
        {
            if (Equals(smtpModel, null))
                return null;
            ZnodeEncryption encryption = new ZnodeEncryption();
            ZnodePortalSmtpSetting smtpEntity = new ZnodePortalSmtpSetting()
            {
                PortalSmtpSettingId = smtpModel.SmtpId,
                PortalId = smtpModel.PortalId,
                Port = smtpModel.SmtpPort,
                ServerName = smtpModel.SmtpServer,
                UserName = encryption.EncryptData(smtpModel.SmtpUsername),
                Password = smtpModel.SmtpPassword,
                IsEnableSsl = smtpModel.EnableSslForSmtp,
                FromDisplayName = smtpModel.FromDisplayName,
                BccEmailAddress = smtpModel.BccEmailAddress,
                FromEmailAddress = smtpModel.FromEmailAddress,
                DisableAllEmails = smtpModel.DisableAllEmailsForSmtp     
            };
            return smtpEntity;
        }
    }
}
