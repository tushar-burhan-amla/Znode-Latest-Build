
using Znode.Engine.Api.Models;

namespace Znode.Engine.SMS
{
    public interface IZnodeSMSProvider
    {
        /// <summary>
        /// This method is used to send the SMS using Specific Provider
        /// </summary>
        /// <param name="smsProviderSetting">smsProviderSetting Model</param>
        /// <param name="smsText">Text Message to send</param>
        /// <param name="phoneNumber">Receiver phoneNumber</param>
        void SendSMS(SMSPortalSettingModel smsProviderSetting, string smsText, string phoneNumber);
    }
}
