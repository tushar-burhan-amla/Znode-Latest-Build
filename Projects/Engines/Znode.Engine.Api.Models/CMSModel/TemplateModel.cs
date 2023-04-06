using System.ComponentModel.DataAnnotations;

namespace Znode.Engine.Api.Models
{
    public class TemplateModel : BaseModel
    {
        public int CMSTemplateId { get; set; }  
        [Required]
        public string Name { get; set; }
        public string FileName { get; set; }
        public int? MediaId { get; set; }
        public string MediaPath { get; set; }
        public string MediaThumbNailPath { get; set; }
        public int Version { get; set; }
    }
}
