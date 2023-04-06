using System.ComponentModel.DataAnnotations;
using Znode.Engine.Admin.Helpers;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class SMTPViewModel : BaseViewModel
    {
        public int PortalId { get; set; }
        public string PortalName { get; set; }
        public int SmtpId { get; set; }

        [Range(0, 65535, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorSMTPPort)]
        [Display(Name = ZnodeAdmin_Resources.LabelSMTPPort, ResourceType = typeof(Admin_Resources))]
        [RegularExpression(AdminConstants.SmtpPortNumberValidation, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorSMTPPort)]
        public int? SmtpPort { get; set; } 

        [Display(Name = ZnodeAdmin_Resources.LabelSMTPServer, ResourceType = typeof(Admin_Resources))]
        [MaxLength(100, ErrorMessageResourceName = ZnodeAdmin_Resources.SMTPServerMaxLength, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string SmtpServer { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelSMTPServerUserName, ResourceType = typeof(Admin_Resources))]
        [MaxLength(100, ErrorMessageResourceName = ZnodeAdmin_Resources.SMTPUserNamePasswordError, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string SmtpUsername { get; set; }

        [DataType(DataType.Password)]
        [Display(Name = ZnodeAdmin_Resources.LabelSMTPServerPassword, ResourceType = typeof(Admin_Resources))]
        [MaxLength(100, ErrorMessageResourceName = ZnodeAdmin_Resources.SMTPUserNamePasswordError, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string SmtpPassword { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelEnableSslForSmtp, ResourceType = typeof(Admin_Resources))]
        public bool EnableSslForSmtp { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelFromDisplayName, ResourceType = typeof(Admin_Resources))]
        [MaxLength(100, ErrorMessageResourceName = ZnodeAdmin_Resources.DisplayNameLengthError, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string FromDisplayName { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelFromEmailAddress, ResourceType = typeof(Admin_Resources))]
        [RegularExpression(AdminConstants.EmailRegularExpression, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ValidateEmailAddress)]
        public string FromEmailAddress { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelBCCEmailAddress, ResourceType = typeof(Admin_Resources))]
        [RegularExpression(AdminConstants.MultipleEmailRegEx, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ValidateEmailAddress)]
        public string BccEmailAddress { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelDisableAllEmailsForSmtp, ResourceType = typeof(Admin_Resources))]
        public bool DisableAllEmailsForSmtp { get; set; }
    }
}