namespace Znode.Engine.Api.Models
{
    public class DynamicContentModel : BaseModel
    {
        public int PortalCustomCssId { get; set; } = 0;
        public int PortalId { get; set; }
        public string DynamicCssStyle { get; set; }
        public string WYSIWYGFormatStyle { get; set; }
        public bool IsActive { get; set; }
        public byte PublishStateId { get; set; }
    }
}
