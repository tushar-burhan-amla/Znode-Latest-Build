namespace Znode.Engine.Api.Models
{
    public class CMSContentPageTemplateModel : BaseModel
    {
        public int CMSTemplateId { get; set; }
        public string Name { get; set; }
        public string FileName { get; set; }
    }
}
