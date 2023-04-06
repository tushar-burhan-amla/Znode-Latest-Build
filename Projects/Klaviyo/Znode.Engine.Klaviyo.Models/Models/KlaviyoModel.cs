namespace Znode.Engine.klaviyo.Models
{
    public class KlaviyoModel : KlaviyoBaseModel
    {
        public int PortalKlaviyoSettingId { get; set; }
        public string KlaviyoCode { get; set; }
        public string PublicApiKey { get; set; }
        public string KlaviyoUserName { get; set; }
        public string KlaviyoPassword { get; set; }
        public string PortalName { get; set; }
        public bool IsActive { get; set; }
     }
     
}
