using System.ComponentModel.DataAnnotations;
using Znode.Engine.Admin.Helpers;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class PowerBISettingsViewModel : BaseViewModel
    {
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorApplicationId)]
        [Display(Name = ZnodeAdmin_Resources.LabelApplicationId, ResourceType = typeof(Admin_Resources))]
        public string PowerBIApplicationId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorGroupId)]
        [Display(Name = ZnodeAdmin_Resources.LabelGroupId, ResourceType = typeof(Admin_Resources))]
        public string PowerBIGroupId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorReportId)]
        [Display(Name = ZnodeAdmin_Resources.LabelReportId, ResourceType = typeof(Admin_Resources))]
        public string PowerBIReportId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorTenantId)]
        [Display(Name = ZnodeAdmin_Resources.LabelTenantId, ResourceType = typeof(Admin_Resources))]
        public string PowerBITenantId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorPowerBiUsername)]
        [Display(Name = ZnodeAdmin_Resources.LabelPowerBiUsername, ResourceType = typeof(Admin_Resources))]
        [RegularExpression(AdminConstants.EmailRegularExpression, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ValidEmailAddress)]
        public string PowerBIUserName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorPowerBiPassword)]
        [Display(Name = ZnodeAdmin_Resources.LabelPowerBiPassword, ResourceType = typeof(Admin_Resources))]
        public string PowerBIPassword { get; set; }
    }
}
