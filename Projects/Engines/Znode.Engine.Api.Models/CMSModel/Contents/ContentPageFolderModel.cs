namespace Znode.Engine.Api.Models
{
    public class ContentPageFolderModel : BaseModel
    {
        public int CMSContentPageGroupId { get; set; }
        public int ParentCMSContentPageGroupId { get; set; }
        public string Code { get; set; }
        public int LocaleId { get; set; }
    }
}
