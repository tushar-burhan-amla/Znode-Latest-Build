using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Api.Models
{
    public class ContainerTemplateUpdateModel : BaseModel
    {
        public int ContainerTemplateId { get; set; }
        public int? MediaId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = ZnodeApi_Resources.RequiredContainerTemplateCode, ErrorMessageResourceType = typeof(Api_Resources))]
        [RegularExpression(@"^[A-Za-z][a-zA-Z0-9]*$", ErrorMessageResourceName = ZnodeApi_Resources.AlphaNumericOnly, ErrorMessageResourceType = typeof(Api_Resources))]
        [MaxLength(100, ErrorMessageResourceName = ZnodeApi_Resources.TemplateCodeLimit, ErrorMessageResourceType = typeof(Api_Resources))]
        public string Code { get; set; }

        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.TemplateNameRequired)]
        public string Name { get; set; }

        [Required(ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.RequiredFileNameErrorMessage)]
        public string FileName { get; set; }
    }
}
