namespace Znode.Engine.Api.Models
{
    public class ImportTemplateModel: BaseModel
    {
        public int ImportTemplateId { get; set;}
        public int ImportHeadId { get; set; }
        public string TemplateName { get; set; }
        public string TemplateVersion { get; set; }
    }
}
