namespace Znode.Engine.Api.Models
{
    public class LinkWidgetConfigurationModel : BaseModel
    {
        public int CMSWidgetTitleConfigurationId { get; set; }
        public int CMSWidgetsId { get; set; }
        public int PortalId { get; set; }
        public int MediaId { get; set; }
        public string Title { get; set; }
        public string Url { get; set; }
        public string MediaPath { get; set; }
        public int CMSMappingId { get; set; }
        public string WidgetsKey { get; set; }
        public string TypeOFMapping { get; set; }
        public string DisplayName { get; set; }
        public string WidgetName { get; set; }
        public bool IsActive { get; set; }
        public int LocaleId { get; set; }
        public string TitleCode { get; set; }
        public int CMSWidgetTitleConfigurationLocaleId { get; set; }
        public string Image { get; set; }

        public bool  IsNewTab { get; set; }
        public int DisplayOrder { get; set; }
        public bool EnableCMSPreview { get; set; }
    }
}
