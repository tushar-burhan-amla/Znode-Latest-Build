namespace Znode.Engine.Api.Models
{
    public class ManageMessageModel : BaseModel
    {
        public int CMSMessageId { get; set; }
        public int? LocaleId { get; set; }
        public int? CMSMessageKeyId { get; set; }
        public int? CMSPortalMessageId { get; set; }
        public int? PortalId { get; set; }
        public string Message { get; set; }
        public string MessageKey { get; set; }
        public string StoreName { get; set; }
        public string[] PortalIds { get; set; }
        public string Location { get; set; }
        public string MessageTag { get; set; }
        public string PublishStatus { get; set; }
        public string IsGlobalContentBlock { get; set; }
        public bool IsPublished { get; set; }
    }
}
