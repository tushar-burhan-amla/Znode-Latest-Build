using System;
using System.Diagnostics;
using System.Linq;
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
    public class SMTPService : BaseService, ISMTPService
    {
        #region Private Variables
        private readonly IZnodeRepository<ZnodePortalSmtpSetting> _znodeSMTPRepository;
        #endregion

        #region Constructor
        public SMTPService()
        {
            _znodeSMTPRepository = new ZnodeRepository<ZnodePortalSmtpSetting>();
        }
        #endregion

        #region Public Methods
        //Get SMTP Configuration details for the Portal.
        public virtual SMTPModel GetSMTP(int portalId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Paramters portalId:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info, portalId);
            //Get SMTP Configuration details for the portal.
            if (portalId > 0)
                {
                    SMTPModel smtpModel = SMTPMap.ToModel(_znodeSMTPRepository.Table.Where(x => x.PortalId == portalId)?.FirstOrDefault());
               
                    if (HelperUtility.IsNotNull(smtpModel))
                    {
                        ZnodeRepository<ZnodePortal> _portalRepository = new ZnodeRepository<ZnodePortal>();

                        //Bind store name.
                        smtpModel.PortalName = _portalRepository.Table.FirstOrDefault(x => x.PortalId == portalId)?.StoreName;
                    }
                ZnodeLogging.LogMessage("SMTP PortalName:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info, smtpModel?.PortalName);
                return smtpModel;
                }
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return null;

        }

        //Save the SMTP Configuration details for the Portal.
        public virtual bool UpdateSMTP(SMTPModel smtpModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Input Paramters SMTPModel:", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info, smtpModel);
            if (Equals(smtpModel, null))
                    throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorModelNull);

                if (smtpModel?.PortalId > 0 && smtpModel?.SmtpId > 0)
                {
                    //If Already SMTP Configuration Exists, then Update the SMTP Configuration.
                    if (_znodeSMTPRepository.Update(SMTPMap.ToEntity(smtpModel)))
                    {

                        ZnodeLogging.LogMessage(string.Format(Admin_Resources.SuccessSmtpUpdate, smtpModel.PortalId), string.Empty, TraceLevel.Info);
                        smtpModel = SMTPMap.ToModel(_znodeSMTPRepository.Table.Where(x => x.PortalId == smtpModel.PortalId)?.FirstOrDefault());
                        return true;
                    }
                }
                else
                {
                    //Insert the SMTP Configuration Details.
                    ZnodePortalSmtpSetting smtp = _znodeSMTPRepository.Insert(SMTPMap.ToEntity(smtpModel));
                ZnodeLogging.LogMessage(string.Format(Admin_Resources.SuccessSmtpInsert, smtpModel.PortalId), string.Empty, TraceLevel.Info);
                smtpModel = SMTPMap.ToModel(smtp);
                    return true;
                }
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return false;

        }

        //Send test email .
        public virtual bool SendEmail(EmailModel emailModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            if (Equals(emailModel, null))
                    throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorModelNull);
                if (emailModel?.PortalId > 0)
                {
                ZnodeLogging.LogMessage("Parameter for ZnodeEmail.SendEmail", ZnodeLogging.Components.Portal.ToString(),TraceLevel.Verbose, new object[] { emailModel.PortalId, emailModel.ToEmailAddress, string.Empty, emailModel.BccEmailAddress, emailModel.EmailSubject, emailModel.EmailMessage, false, string.Empty, emailModel.CcEmailAddress } );
                ZnodeEmail.SendEmail(emailModel.PortalId, emailModel.ToEmailAddress, string.Empty, emailModel.BccEmailAddress, emailModel.EmailSubject, emailModel.EmailMessage, false, string.Empty,emailModel.CcEmailAddress);
                    return true;
                }
            ZnodeLogging.LogMessage("Executed.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            return false;

        }
        #endregion
    }
}
