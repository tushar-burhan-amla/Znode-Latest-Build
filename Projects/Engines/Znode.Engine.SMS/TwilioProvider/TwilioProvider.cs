using System;
using System.Diagnostics;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

using Znode.Engine.Api.Models;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.SMS
{
    public class TwilioProvider : ZnodeSMSProviders, IZnodeSMSProvider
    {
        public TwilioProvider()
        {

        }

        /// <summary>
        /// Send Sms using Twilio Provider
        /// </summary>
        /// <param name="smsProviderSetting">smsProviderSetting Model</param>
        /// <param name="smsText">Text Message to send</param>
        /// <param name="phoneNumber">Receiver phoneNumber</param>
        public override void SendSMS(SMSPortalSettingModel smsProviderSetting, string smsText, string phoneNumber)
        {
            try
            {
                string Twilio_Account_SID = smsProviderSetting.SmsPortalAccountId;
                string Twilio_Auth_TOKEN = smsProviderSetting.AuthToken;

                TwilioClient.Init(Twilio_Account_SID, Twilio_Auth_TOKEN);

                var message = MessageResource.Create(
                    from: new Twilio.Types.PhoneNumber(smsProviderSetting.FromMobileNumber),
                    body: smsText,
                    to: new Twilio.Types.PhoneNumber(phoneNumber)
                );
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error while Sending sms using: " + smsProviderSetting.ClassName, "SendSms", TraceLevel.Error, ex);
                throw;
            }
        }
    }
}
