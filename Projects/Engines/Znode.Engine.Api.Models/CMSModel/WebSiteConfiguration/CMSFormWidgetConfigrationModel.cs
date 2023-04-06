namespace Znode.Engine.Api.Models
{
    public class CMSFormWidgetConfigrationModel : BaseModel
    {
        public int CMSFormWidgetConfigurationId { get; set; }
        public int LocaleId { get; set; }
        public int CMSWidgetsId { get; set; }
        public string WidgetsKey { get; set; }
        public int CMSMappingId { get; set; }
        public string TypeOFMapping { get; set; }
        public int? FormBuilderId { get; set; }
        public string FormTitle { get; set; }
        public string ButtonText { get; set; }
        public bool IsTextMessage { get; set; }
        public string TextMessage { get; set; }
        public string RedirectURL { get; set; }
        public bool IsShowCaptcha { get; set; }
        public bool EnableCMSPreview { get; set; }
    }
}
