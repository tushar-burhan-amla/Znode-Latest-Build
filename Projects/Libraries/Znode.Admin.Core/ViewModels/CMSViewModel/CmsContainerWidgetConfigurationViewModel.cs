using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class CmsContainerWidgetConfigurationViewModel : BaseViewModel
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
        public string PortalName { get; set; }     
        public string WidgetName { get; set; }
        public string FileName { get; set; }
        public string WidgetCode { get; set; }

        public GridModel GridModel { get; set; }
    }
}
