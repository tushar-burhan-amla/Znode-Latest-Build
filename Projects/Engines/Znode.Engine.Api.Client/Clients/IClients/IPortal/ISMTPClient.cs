using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Client
{
    public interface ISMTPClient : IBaseClient
    {
        /// <summary>
        /// Get Smtp by portalId
        /// </summary>
        /// <param name="portalId">int portalId to get Smtp</param>
        /// <returns>SMTP Model</returns>
        SMTPModel GetSmtp(int portalId);

        /// <summary>
        /// Update Smtp.
        /// </summary>
        /// <param name="smtpModel">SMTPModel to update smtp.</param>
        /// <returns>SMTP Model.</returns>
        SMTPModel UpdateSmtp(SMTPModel smtpModel);

        /// <summary>
        /// Send test Email
        /// </summary>
        /// <param name="emailModel"></param>
        /// <returns>Email Model</returns>
        EmailModel SendEmail(EmailModel emailModel);
    }
}
