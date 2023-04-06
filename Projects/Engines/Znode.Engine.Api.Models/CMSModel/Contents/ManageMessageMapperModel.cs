namespace Znode.Engine.Api.Models
{
    public class ManageMessageMapperModel : BaseModel
    {
        public int CMSMessageId { get; set; }
        public int LocaleId { get; set; }
        public int CMSMessageKeyId { get; set; }
        public int PortalId { get; set; }
    }
}