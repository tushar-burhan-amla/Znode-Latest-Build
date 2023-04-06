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

namespace Znode.Engine.Admin.Agents
{
    public class SMTPAgent : BaseAgent, ISMTPAgent
    {
        #region Private Variables
        private readonly ISMTPClient _smtpClient;
        #endregion

        #region Constructor
        public SMTPAgent(ISMTPClient smtpClient)
        {
            _smtpClient = GetClient<ISMTPClient>(smtpClient);
        }
        #endregion


        public virtual SMTPViewModel GetSmtp(int portalId)
            => portalId > 0 ? _smtpClient.GetSmtp(portalId)?.ToViewModel<SMTPViewModel>() : new SMTPViewModel();

        public virtual SMTPViewModel UpdateSmtp(SMTPViewModel smtpViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            try
            {
                SMTPModel smtpModel = _smtpClient.UpdateSmtp(smtpViewModel?.ToModel<SMTPModel>());
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
                return HelperUtility.IsNotNull(smtpModel) ? smtpModel?.ToViewModel<SMTPViewModel>() : new SMTPViewModel();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Warning);
                return new SMTPViewModel { HasError = true, ErrorMessage = ex.ErrorMessage };
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                return (SMTPViewModel)GetViewModelWithErrorMessage(smtpViewModel, Admin_Resources.UpdateErrorMessage);
            }
        }

        // Methood to Send Email
        public virtual EmailViewModel SendTestEmail(EmailViewModel emailViewModel)
        {
            ZnodeLogging.LogMessage("Agent method execution started.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
            try
            {
                EmailModel emailModel = _smtpClient.SendEmail(emailViewModel?.ToModel<EmailModel>());
                ZnodeLogging.LogMessage("Agent method execution done.", ZnodeLogging.Components.Portal.ToString(), TraceLevel.Info);
                return HelperUtility.IsNotNull(emailModel) ? emailModel?.ToViewModel<EmailViewModel>() : new EmailViewModel();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Warning);
                return new EmailViewModel { HasError = true, ErrorMessage = ex.ErrorMessage };
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                return (EmailViewModel)GetViewModelWithErrorMessage(emailViewModel, Admin_Resources.EmailError);
            }
        }
    }
}