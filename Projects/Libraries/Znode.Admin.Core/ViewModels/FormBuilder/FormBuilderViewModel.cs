using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class FormBuilderViewModel : BaseViewModel
    {
        public int FormBuilderId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelFormCode, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.CodeRequiredMessage)]
        [RegularExpression(@"^[A-Za-z][a-zA-Z0-9]*$", ErrorMessageResourceName = ZnodeAdmin_Resources.AlphanumericStartWithAlphabet, ErrorMessageResourceType = typeof(Admin_Resources))]
        [MaxLength(200, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorFormCode)]
        public string FormCode { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelFormDescription, ResourceType = typeof(Admin_Resources))]
        [MaxLength(200, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorFormDescription)]
        public string FormDescription { get; set; }

        public bool? IsShowCaptcha { get; set; } = false;
    }
}
