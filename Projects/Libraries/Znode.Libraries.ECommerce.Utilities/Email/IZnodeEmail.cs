using MimeKit;
using Znode.Libraries.Data.DataModel;

namespace Znode.Libraries.ECommerce.Utilities
{
    public interface IZnodeEmail
    {
        /// <summary>
        /// This method is responsible to send emails using MailKit SMTPClient.
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="znodePortalSmtpSetting">Portal SMTP settings</param>
        /// <returns>Status</returns>
        string SendSMTPEmail(MimeMessage message, ZnodePortalSmtpSetting znodePortalSmtpSetting);
    }
}
