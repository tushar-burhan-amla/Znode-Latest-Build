using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.SMS
{
    public class ZnodeSMSManager : ZnodeSMSProviders, IZnodeSMSManager
    {

        /// <summary>
        /// This Method is used to send SMS based on Portal and Template setting
        /// </summary>
        /// <param name="znodeSmsContext"></param>
        public virtual void SendSMS(ZnodeSMSContext znodeSmsContext)
        {
            try
            {
                SMSPortalSettingModel smsProviderSetting = GetSMSProviderSetting(znodeSmsContext.PortalId);
                if (HelperUtility.IsNotNull(smsProviderSetting) && smsProviderSetting.IsSmsProviderEnabled && znodeSmsContext.SMSOptIn)
                {
                    SMSTemplateSettingModel smsTemplate = GetSMSTemplateSetting(znodeSmsContext.SmsTemplateName, znodeSmsContext.PortalId);
                    if (HelperUtility.IsNotNull(smsTemplate) && smsTemplate.IsSmsNotificationActive)
                    {
                        string messageBody = ReplaceMacros(smsTemplate.SmsContent, znodeSmsContext.MacroValues);
                        if (!string.IsNullOrEmpty(znodeSmsContext.ReceiverPhoneNumber))
                        {
                            base.SendSMS(smsProviderSetting, messageBody, znodeSmsContext.ReceiverPhoneNumber);
                        }
                        else
                        {
                            ZnodeLogging.LogMessage("Receiver Phone Number is blank : " + znodeSmsContext, "SendSms", TraceLevel.Warning);
                        }

                    }
                }

            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("Error while Sending sms for : " + znodeSmsContext, "SendSms", TraceLevel.Error, ex);
                throw;
            }

        }

        /// <summary>
        /// Return SMS Provider Setting for the given Portal Id
        /// </summary>
        /// <param name="portalId"></param>
        /// <returns></returns>
        
        //TODO - Need to be implement cache.
        protected virtual SMSPortalSettingModel GetSMSProviderSetting(int portalId)
        {
            SMSPortalSettingModel smsProviderSetting = null;

            if (portalId > 0)
            {
                IZnodeRepository<ZnodePortalSmsSetting> _portalSmsRepository = new ZnodeRepository<ZnodePortalSmsSetting>();
                IZnodeRepository<ZnodeSmsProvider> _znodeSmsProvider = new ZnodeRepository<ZnodeSmsProvider>();
                smsProviderSetting = (from portalSms in _portalSmsRepository.Table
                                      join smsProvider in _znodeSmsProvider.Table on portalSms.SmsProviderId equals smsProvider.SmsProviderId
                                      where portalSms.PortalId == portalId
                                      select new SMSPortalSettingModel
                                      {
                                          SmsPortalAccountId = portalSms.SmsPortalAccountId,
                                          AuthToken = portalSms.AuthToken,
                                          FromMobileNumber = portalSms.FromMobileNumber,
                                          ClassName = smsProvider.ClassName,
                                          IsSmsProviderEnabled = portalSms.IsSMSSettingEnabled
                                      }
                                       )?.FirstOrDefault();
            }
            return smsProviderSetting;
        }

        /// <summary>
        /// Return SMS Template Setting for the given template
        /// </summary>
        /// <param name="templateName"></param>
        /// <returns></returns>
        protected virtual SMSTemplateSettingModel GetSMSTemplateSetting(string templateName, int portalId)
        {
            SMSTemplateSettingModel smsTemplate = null;
            if (!string.IsNullOrEmpty(templateName))
            {
                IZnodeRepository<ZnodeEmailTemplate> _ZnodeEmailTemplateRepository = new ZnodeRepository<ZnodeEmailTemplate>();
                IZnodeRepository<ZnodeEmailTemplateLocale> _ZnodeEmailTemplateLocaleRepository = new ZnodeRepository<ZnodeEmailTemplateLocale>();
                IZnodeRepository<ZnodeEmailTemplateMapper> _ZnodeEmailTemplateMapperRepository = new ZnodeRepository<ZnodeEmailTemplateMapper>();
                smsTemplate = (from template in _ZnodeEmailTemplateRepository.Table
                               join templateLocale in _ZnodeEmailTemplateLocaleRepository.Table on template.EmailTemplateId equals templateLocale.EmailTemplateId
                               join templateMapper in _ZnodeEmailTemplateMapperRepository.Table on templateLocale.EmailTemplateId equals templateMapper.EmailTemplateId
                               where template.TemplateName == templateName && templateMapper.PortalId == portalId
                               select new SMSTemplateSettingModel
                               {
                                   SmsContent = templateLocale.SmsContent,
                                   IsSmsNotificationActive = templateMapper.IsSmsNotificationActive
                               })?.FirstOrDefault();
            }
            if(string.IsNullOrEmpty(smsTemplate?.SmsContent))
                ZnodeLogging.LogMessage("Sms Content is not set for : " + templateName, "GetSMSTemplateSetting", TraceLevel.Info);
            return smsTemplate;
        }

        /// <summary>
        /// Replace macro values in Sms Template
        /// </summary>
        /// <param name="smsTemplate"></param>
        /// <param name="macroValues">Accept Macro Name and its Value</param>
        /// <returns></returns>
        protected virtual string ReplaceMacros(string smsTemplate, Dictionary<string, string> macroValues)
        {
            smsTemplate = HelperUtility.ReplaceMultipleTokenWithMessageText(macroValues, smsTemplate);

            return smsTemplate;
        }


    }
}
