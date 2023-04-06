namespace Znode.Engine.Api.Models
{

    public class CMSMediaWidgetConfigurationModel : BaseModel
    {
        public int CMSWidgetsId { get; set; }
        public string WidgetsKey { get; set; }
        public int CMSMappingId { get; set; }
        public string TypeOFMapping { get; set; }
        public int MediaId { get; set; }
        public string DisplayName { get; set; }
        public int CMSMediaConfigurationId { get; set; }
        public string MediaPath { get; set;}
        public bool EnableCMSPreview { get; set; }
    }
}
