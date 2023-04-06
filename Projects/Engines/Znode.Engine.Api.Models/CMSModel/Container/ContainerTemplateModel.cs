

namespace Znode.Engine.Api.Models
{
    public class ContainerTemplateModel : BaseModel
    {
        public int ContainerTemplateId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }
        public string FileName { get; set; }
        public int? MediaId { get; set; }
        public string MediaPath { get; set; }
        public string MediaThumbNailPath { get; set; }
        public int Version { get; set; }
        public string CreatedByName { get; set; }
        public string ModifiedByName { get; set; }
    }
}
