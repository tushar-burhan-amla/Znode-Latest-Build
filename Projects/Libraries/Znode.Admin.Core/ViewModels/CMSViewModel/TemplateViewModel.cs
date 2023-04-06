using System.ComponentModel.DataAnnotations;
using System.Web;
using Znode.Engine.Admin.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class TemplateViewModel : BaseViewModel
    {
        public int CMSTemplateId { get; set; }

        [MaxLength(100, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RangeValidatorTemplateName)]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredTemplateName)]
        public string Name { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelFileName, ResourceType = typeof(Admin_Resources))]      
        public string FileName { get; set; }
       
        [FileTypeValidation(AdminConstants.CSHTMLFileType, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorCSHTMLFile)]
        [Display(Name = ZnodeAdmin_Resources.LabelFileName, ResourceType = typeof(Admin_Resources))]
        [UIHint("FileUploader")]
        public HttpPostedFileBase FilePath { get; set; }

        [Display(Name = ZnodeAdmin_Resources.PreviewImage, ResourceType = typeof(Admin_Resources))]
        public string MediaPath { get; set; }      
        public int? MediaId { get; set; }
    }
}