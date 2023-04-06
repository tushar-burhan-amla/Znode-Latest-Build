
namespace Znode.Engine.WebStore.ViewModels
{
    public class CmsContainerWidgetConfigurationViewModel
    {
        public int CMSWidgetsId { get; set; }
        public string WidgetKey { get; set; }
        public int CMSMappingId { get; set; }
        public string TypeOFMapping { get; set; }
        public string ContainerKey { get; set; }
        public string DisplayName { get; set; }
        public int CMSContainerConfigurationId { get; set; }
        public int ContentContainerId { get; set; }
        public bool EnableCMSPreview { get; set; }
    }
}
