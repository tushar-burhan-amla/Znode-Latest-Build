using Znode.Engine.Api.Models;

namespace Znode.Engine.Services
{
    public interface ISMTPService 
    {
        /// <summary>
        ///  Get SMTP by portal Id.
        /// </summary>
        /// <param name="portalId">int smtpId to get the smtp.</param>
        /// <returns>DomainModel</returns>
        SMTPModel GetSMTP(int portalId);

        /// <summary>
        /// Update SMTP associated to portal.
        /// </summary>
        /// <param name="smtpModel">SMTPModel to update data.</param>
        /// <returns>returns true if smtp updated else returns false.</returns>
        bool UpdateSMTP(SMTPModel smtpModel);

        /// <summary>
        /// Used to send email 
        /// </summary>
        /// <param name="emailModel">emailModel is use to send email</param>
        /// <returns></returns>
        bool SendEmail(EmailModel emailModel);
    }
}
