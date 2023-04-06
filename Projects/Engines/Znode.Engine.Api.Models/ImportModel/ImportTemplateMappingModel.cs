namespace Znode.Engine.Api.Models
{
    public class ImportTemplateMappingModel : BaseModel
    {
        public int ImportTemplateMappingId { get; set; }
        public int ImportTemplateId { get; set; }
        public string SourceColumnName { get; set; }
        public string TargetColumnName { get; set; }
        public int DisplayOrder { get; set; }
        public bool IsActive { get; set; }
        public bool IsAllowNull { get; set; }
    }
}
