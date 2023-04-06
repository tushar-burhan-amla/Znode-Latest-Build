namespace Znode.Engine.Api.Models
{
    public class SMSPortalSettingModel
    {
        public string SmsPortalAccountId { get; set; }
        public string AuthToken { get; set; }
        public string FromMobileNumber { get; set; }
        public string ClassName { get; set; }
        public bool IsSmsProviderEnabled { get; set; }
    }
}
