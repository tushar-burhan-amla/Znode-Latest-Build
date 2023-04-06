using System.Collections.Generic;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;

namespace Znode.Engine.Api.Cache
{
    public class SMSCache : BaseCache,ISMSCache
    {
        #region Private Variable
        private readonly ISMSService _service;
        #endregion

        #region Constructor
        public SMSCache(ISMSService smsService)
        {
            _service = smsService;
        }
        #endregion
        #region Public Method
        public virtual string GetSMSDetails(int portalId,string routeUri, string routeTemplate, bool isSMSSettingEnabled = false)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                SMSModel smsDetails = _service.GetSMSDetails(portalId,isSMSSettingEnabled);
                if (!Equals(smsDetails, null))
                {
                    SMSResponse smtpResponse = new SMSResponse { Sms = smsDetails };
                    data = InsertIntoCache(routeUri, routeTemplate, smtpResponse);
                }
            }
            return data;
        }
        //TO get SMS ProvideList by PortalId.
        public virtual string GetSmsProviderList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                List<SMSProviderModel> smsProvider = _service.GetSmsProviderList();
                if (!Equals(smsProvider, null))
                {
                    SMSResponse smsResponse = new SMSResponse { SmsProviderModelList = smsProvider };
                    data = InsertIntoCache(routeUri, routeTemplate, smsResponse);
                }
            }
            return data;
        }
        #endregion
    }
}
