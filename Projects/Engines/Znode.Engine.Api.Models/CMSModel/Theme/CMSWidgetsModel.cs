namespace Znode.Engine.Api.Models
{
    public class CMSWidgetsModel : BaseModel
    {
        public int CMSWidgetsId { get; set; }
        public int? CMSAreaId { get; set; }
        public string TemplateName { get; set; }
        public string Code { get; set; }
        public string DisplayName { get; set; }
        public bool IsConfigurable { get; set; }
        public string FileName { get; set; }
    }
}
