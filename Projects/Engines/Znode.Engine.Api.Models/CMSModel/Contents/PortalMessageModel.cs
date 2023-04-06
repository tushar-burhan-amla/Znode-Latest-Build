namespace Znode.Engine.Api.Models
{
    public class PortalMessageModel : BaseModel
    {
        public int CMSPortalMessageId { get; set; }
        public int PortalId { get; set; }
        public int CMSMessageKeyId { get; set; }
        public int CMSMessageId { get; set; }
    }
}
