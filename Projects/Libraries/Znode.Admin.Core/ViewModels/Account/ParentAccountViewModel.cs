using System.ComponentModel.DataAnnotations;

using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class ParentAccountViewModel : BaseViewModel
    {
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorAccountCodeRequired)]
        [Display(Name = ZnodeAdmin_Resources.LabelAccountCode, ResourceType = typeof(Admin_Resources))]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessageResourceName = ZnodeAdmin_Resources.AlphanumericOnlyWithNoSpaces, ErrorMessageResourceType = typeof(Admin_Resources))]
        [MaxLength(100, ErrorMessageResourceName = ZnodeAdmin_Resources.AccountCodeMaxLength, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string AccountCode { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelAccountName, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredAccountName)]
        [MaxLength(100, ErrorMessageResourceName = ZnodeAdmin_Resources.AccountNameCodeLengthErrorMessage, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string Name { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelAccount, ResourceType = typeof(Admin_Resources))]
        public int AccountId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelParentAccount, ResourceType = typeof(Admin_Resources))]
        public int ParentAccountId { get; set; }

        public string ParentAccountName { get; set; }
    }
}
