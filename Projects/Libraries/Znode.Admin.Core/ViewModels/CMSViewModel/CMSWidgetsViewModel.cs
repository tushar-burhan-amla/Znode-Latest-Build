namespace Znode.Engine.Admin.ViewModels
{
    public class CMSWidgetsViewModel : BaseViewModel
    {
        public int CMSWidgetsId { get; set; }      
        public string TemplateName { get; set; }
        public string TemplatePath { get; set; }
        public string Code { get; set; }
        public string DisplayName { get; set; }
        public bool IsConfigurable { get; set; }
        public string FileName { get; set; }
        public string MappingKey { get; set; }
        public string WidgetActionUrl { get; set; }
        public int CMSMappingId { get; set; }

    }
}