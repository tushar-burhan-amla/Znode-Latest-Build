namespace Znode.Engine.Api.Models
{
    public class CMSTextWidgetConfigurationModel : BaseModel
    {
        public int CMSTextWidgetConfigurationId { get; set; }
        public int LocaleId { get; set; }
        public int CMSWidgetsId { get; set; }
        public string WidgetsKey { get; set; }
        public int CMSMappingId { get; set; }
        public string TypeOFMapping { get; set; }
        public string Text { get; set; }
        public bool EnableCMSPreview { get; set; }
    }
}
