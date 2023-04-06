using System.ComponentModel.DataAnnotations;

namespace Znode.Engine.Api.Models
{
    public class EmailTemplateMapperModel : BaseModel
    {
        public int EmailTemplateMapperId { get; set; }
        public int EmailTemplateId { get; set; }
        public int EmailTemplateAreasId { get; set; }
        public string Name { get; set; }
        public string Code { get; set; }

        public string TemplateName { get; set; }
        public string Descriptions { get; set; }

        [Required]
        [Display(Name = "Content")]
        [UIHint("RichTextBox")]
        public string Html { get; set; }

        [Required]
        public string Subject { get; set; }

        public bool  IsEnableBcc { get; set; }
    }
}
