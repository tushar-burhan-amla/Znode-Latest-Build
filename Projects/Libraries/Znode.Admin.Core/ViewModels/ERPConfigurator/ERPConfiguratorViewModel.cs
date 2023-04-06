using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Engine.Admin.Helpers;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class ERPConfiguratorViewModel : BaseViewModel 
    {
        public int ERPConfiguratorId { get; set; }

        [Display(Name = ZnodeERP_Resources.LabelERPConnectorPackageName, ResourceType = typeof(ERP_Resources))]
        [Required(ErrorMessageResourceType = typeof(ERP_Resources), ErrorMessageResourceName = ZnodeERP_Resources.ERPConnectorPackageNameRequiredMessage)]
        [MaxLength(80, ErrorMessageResourceType = typeof(ERP_Resources), ErrorMessageResourceName = ZnodeERP_Resources.ERPTaskSchedulerSchedulerNameLengthErrorMessage)]
        public string ClassName { get; set; }

        [Display(Name = ZnodeERP_Resources.LabelERPConnectorERPName, ResourceType = typeof(ERP_Resources))]
        [Required(ErrorMessageResourceType = typeof(ERP_Resources), ErrorMessageResourceName = ZnodeERP_Resources.ERPConnectorERPNameRequiredMessage)]
        [MaxLength(100, ErrorMessageResourceType = typeof(ERP_Resources), ErrorMessageResourceName = ZnodeERP_Resources.ERPConnectorNameLengthErrorMessage)]
        public string ERPName { get; set; }

        [Display(Name = ZnodeERP_Resources.LabelERPConnectorDescription, ResourceType = typeof(ERP_Resources))]
        public string Description { get; set; }

        [Display(Name = ZnodeERP_Resources.LabelERPConnectorIsActive, ResourceType = typeof(ERP_Resources))]
        public bool IsActive { get; set; }
        public bool Status { get; set; }

        [Display(Name = ZnodeERP_Resources.LabelERPConnectorEmail, ResourceType = typeof(ERP_Resources))]
        [Required(ErrorMessageResourceType = typeof(ERP_Resources), ErrorMessageResourceName = ZnodeERP_Resources.RequiredEmailID)]
        [RegularExpression(AdminConstants.MultipleEmailRegEx, ErrorMessageResourceType = typeof(ERP_Resources), ErrorMessageResourceName = ZnodeERP_Resources.ValidEmailAddress)]
        public string Email { get; set; }

        public List<SelectListItem> ERPConfiguratorClasses { get; set; }

        public string ERPClassName { get; set; }

        public string JsonSetting { get; set; }
    }
}