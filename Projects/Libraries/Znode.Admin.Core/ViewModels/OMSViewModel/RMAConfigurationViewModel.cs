using System.ComponentModel.DataAnnotations;
using Znode.Engine.Admin.Helpers;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class RMAConfigurationViewModel : BaseViewModel
    {
        [MaxLength(100, ErrorMessageResourceName = ZnodeAdmin_Resources.DisplayNameMaxLength, ErrorMessageResourceType = typeof(Admin_Resources))]
        [Display(Name = ZnodeAdmin_Resources.LabelReturnDepartmentTitle, ResourceType = typeof(Admin_Resources))]
        public string DisplayName { get; set; }

        [MaxLength(100, ErrorMessageResourceName = ZnodeAdmin_Resources.EmailMaxLength, ErrorMessageResourceType = typeof(Admin_Resources))]
        [Display(Name = ZnodeAdmin_Resources.LabelCustomerEmailId, ResourceType = typeof(Admin_Resources))]
        [RegularExpression(AdminConstants.EmailRegularExpression, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ValidEmailId)]
        public string Email { get; set; }

        [MaxLength(100, ErrorMessageResourceName = ZnodeAdmin_Resources.MailingAddressMaxLength, ErrorMessageResourceType = typeof(Admin_Resources))]
        [RegularExpression(AdminConstants.EmailRegularExpression, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ValidMailingAddress)]
        [Display(Name = ZnodeAdmin_Resources.LabelReturnAddress, ResourceType = typeof(Admin_Resources))]
        public string Address { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelShippingInstruction, ResourceType = typeof(Admin_Resources))]
        public string ShippingDirections { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelGcNotification, ResourceType = typeof(Admin_Resources))]
        public string GcNotification { get; set; }

        public int RmaConfigurationId { get; set; }

        [RegularExpression(AdminConstants.NumberExpression, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.MessageNumericValueAllowed)]
        [Display(Name = ZnodeAdmin_Resources.LabelRMAMaxDays, ResourceType = typeof(Admin_Resources))]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.rfvMaxDays)]
        public int? MaxDays { get; set; } = 7;

        [RegularExpression(AdminConstants.NumberExpression, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.MessageNumericValueAllowed)]
        [Display(Name = ZnodeAdmin_Resources.LabelGiftCardExpiration, ResourceType = typeof(Admin_Resources))]
        [Range(1, 99999, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.rnvRMAConfigIssueGiftCardRange)]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.rfvMaxDays)]
        public int? GcExpirationPeriod { get; set; } = 90;

        [Display(Name = ZnodeAdmin_Resources.LabelEnableEmailNotification, ResourceType = typeof(Admin_Resources))]
        public bool IsEmailNotification { get; set; }
    }
}