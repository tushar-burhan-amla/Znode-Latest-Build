using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Engine.Admin.Helpers;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
   public class EmailViewModel : BaseViewModel
    {
        public int PortalId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorEmailRequired)]
        [RegularExpression(AdminConstants.MultipleEmailRegEx, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ValidateEmailAddress)]
        [Display(Name = ZnodeAdmin_Resources.LabelToEmailAddress, ResourceType = typeof(Admin_Resources))]
        public string ToEmailAddress { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelCCEmailAddress, ResourceType = typeof(Admin_Resources))]
        [RegularExpression(AdminConstants.MultipleEmailRegEx, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ValidateEmailAddress)]
        public string CcEmailAddress { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelBCCEmail, ResourceType = typeof(Admin_Resources))]
        [RegularExpression(AdminConstants.MultipleEmailRegEx, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ValidateEmailAddress)]
        public string BccEmailAddress { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelSubject, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredEmailSubject)]
        public string EmailSubject { get; set; }

        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredEmailMessage)]
        [Display(Name = ZnodeAdmin_Resources.LabelBody, ResourceType = typeof(Admin_Resources))]
        [AllowHtml]
        [UIHint("RichTextBox")]
        public string EmailMessage { get; set; }


    }
}
