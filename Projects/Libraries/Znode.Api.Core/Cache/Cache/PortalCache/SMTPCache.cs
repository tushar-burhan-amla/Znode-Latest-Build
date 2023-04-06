using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;

namespace Znode.Engine.Api.Cache
{
    public class SMTPCache : BaseCache, ISMTPCache
    {
        #region Private Variable
        private readonly ISMTPService _service;
        #endregion

        #region Constructor
        public SMTPCache(ISMTPService smtpService)
        {
            _service = smtpService;
        }
        #endregion

        #region Public Method
        public virtual string GetSMTP(int portalId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                SMTPModel smtp = _service.GetSMTP(portalId);
                if (!Equals(smtp, null))
                {
                    SMTPResponse smtpResponse = new SMTPResponse { Smtp = smtp };
                    data = InsertIntoCache(routeUri, routeTemplate, smtpResponse);
                }
            }
            return data;
        }
        #endregion
    }
}