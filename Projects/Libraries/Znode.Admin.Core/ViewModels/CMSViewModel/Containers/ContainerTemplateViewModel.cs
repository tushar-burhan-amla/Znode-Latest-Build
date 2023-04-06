using System.ComponentModel.DataAnnotations;
using System.Web;
using Znode.Engine.Admin.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class ContainerTemplateViewModel : BaseViewModel
    {
        public int ContainerTemplateId { get; set; }


        [Required(ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredTemplateName, ErrorMessageResourceType = typeof(Admin_Resources))]
        [Display(Name = ZnodeAdmin_Resources.LabelContainerTemplate, ResourceType = typeof(Admin_Resources))]
        [MaxLength(100, ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorTemplateNameLimit, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string Name { get; set; }

       
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredContainerTemplateCode, ErrorMessageResourceType = typeof(Admin_Resources))]
        [Display(Name = ZnodeAdmin_Resources.TemplateCode, ResourceType = typeof(Admin_Resources))]
        [RegularExpression(@"^[A-Za-z][a-zA-Z0-9]*$", ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorAlphanumericOnly, ErrorMessageResourceType = typeof(Admin_Resources))]
        [MaxLength(100, ErrorMessageResourceName = ZnodeAdmin_Resources.TemplateCodeLimit, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string Code { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelFileName, ResourceType = typeof(Admin_Resources))]
        public string FileName { get; set; }
        public int? MediaId { get; set; }

        [FileTypeValidation(AdminConstants.CSHTMLFileType, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorUploadCSHTMLFile)]
        [Display(Name = ZnodeAdmin_Resources.LabelFileName, ResourceType = typeof(Admin_Resources))]
        [UIHint("FileUploader")]
        public HttpPostedFileBase FilePath { get; set; }

        [Display(Name = ZnodeAdmin_Resources.PreviewImage, ResourceType = typeof(Admin_Resources))]
        public string MediaPath { get; set; }
        public string CreatedByName { get; set; }
        public string ModifiedByName { get; set; }
    }
}
