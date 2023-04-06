using System;
using System.Diagnostics;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.SMS
{
    public class ZnodeSMSProviders : IZnodeSMSProvider
    {
        #region Variable
        private int portalId;
        private ZnodeSMSContext znodeSmsContext;
        #endregion

        #region Constructor
        public ZnodeSMSProviders(ZnodeSMSContext znodeSmsContext)
        {
            portalId = znodeSmsContext.PortalId;
            this.znodeSmsContext = znodeSmsContext;
        }

        public ZnodeSMSProviders()
        {

        }

        #endregion

        /// <summary>
        /// This method is used to send the SMS using Specific Provider
        /// </summary>
        /// <param name="smsProviderSetting">smsProviderSetting Model</param>
        /// <param name="smsText">Text Message to send</param>
        /// <param name="phoneNumber">Receiver phoneNumber</param>
        public virtual void SendSMS(SMSPortalSettingModel smsProviderSetting, string smsText, string phoneNumber)
        {
            if (HelperUtility.IsNotNull(smsProviderSetting))
            {
                try
                {
                    IZnodeSMSProvider znodeSmsProvider = GetSmsProviderInstance<IZnodeSMSProvider>(smsProviderSetting.ClassName);
                    znodeSmsProvider.SendSMS(smsProviderSetting, smsText, phoneNumber);
                }
                catch (Exception ex)
                {
                    //Log exception if occur.
                    ZnodeLogging.LogMessage("Error while instantiating Sms Provider type: " + smsProviderSetting.ClassName, "SmsProvider", TraceLevel.Error, ex);
                }
            }
        }

        /// <summary>
        /// Create and return instance for SMS provider classes
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="className"></param>
        /// <returns></returns>
        protected virtual T GetSmsProviderInstance<T>(string className) where T : class
        {
            if (!string.IsNullOrEmpty(className))
                return (T)GetKeyedService<T>(className);
            return null;
        }
    }
}
