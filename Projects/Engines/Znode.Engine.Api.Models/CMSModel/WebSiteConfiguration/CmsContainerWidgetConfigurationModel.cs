namespace Znode.Engine.Api.Models
{
    public class CmsContainerWidgetConfigurationModel : BaseModel
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
    public string WidgetCode { get; set; } // Added new Property WidgetCode to contain the code of widget.
    }
}
