using System;

namespace Znode.Engine.Api.Models
{
    public class SMSModel : BaseModel
    {
        public int PortalSmsSettingId { get; set; }
        public int PortalId { get; set; }
        public int SMSProviderId { get; set; }
        public string SmsPortalAccountId { get; set; }
        public string AuthToken { get; set; }
        public string FromMobileNumber { get; set; }
        public bool IsSMSSettingEnabled { get; set; }
    }
}
