namespace Znode.Engine.Api.Models
{
    public class ManageMessageKeyModel : BaseModel
    {
        public int CMSMessageKeyId { get; set; }
        public int CMSAreaId { get; set; }
        public string MessageKey { get; set; }
        public string MessageTag { get; set; }
    }
}
