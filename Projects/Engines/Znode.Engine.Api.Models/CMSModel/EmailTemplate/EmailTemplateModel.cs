using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Api.Models
{
    public class EmailTemplateModel : BaseModel
    {
        public int EmailTemplateId { get; set; }
        public int EmailTemplateLocaleId { get; set; }

        [Required]
        [RegularExpression("^[A-Za-z0-9 ]+$")]
        public string TemplateName { get; set; }
        public string Descriptions { get; set; }       

        public string Html { get; set; }

        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.SubjectRequiredMessage)]
        public string Subject { get; set; }
        public int? LocaleId { get; set; }
        public string EmailTemplateTokens { get; set; }
        public string EmailTemplateTokensPartOne { get; set; }
        public string EmailTemplateTokensPartTwo { get; set; }
        public string SmsContent { get; set; }
    }
}
