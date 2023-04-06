using Znode.Engine.Admin.ViewModels;

namespace Znode.Engine.Admin.Agents
{
    public interface ISMTPAgent
    {
        /// <summary>
        /// Get SMTP for specified portal Id.
        /// </summary>
        /// <param name="portalId">int portalId to get smtp.</param>
        /// <returns>SMTP ViewModel.</returns>
        SMTPViewModel GetSmtp(int portalId);

        /// <summary>
        ///  Update Smtp associated to portal.
        /// </summary>
        /// <param name="smtpViewModel">SMTPViewModel to update smtp.</param>
        /// <returns>SMTP ViewModel.</returns>
        SMTPViewModel UpdateSmtp(SMTPViewModel smtpViewModel);

        /// <summary>
        /// Used to send test mail
        /// </summary>
        /// <param name="emailViewModel"></param>
        /// <returns>Email ViewModel</returns>
        EmailViewModel SendTestEmail(EmailViewModel emailViewModel);
    }
}
