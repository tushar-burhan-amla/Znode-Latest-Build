using System;
using System.ComponentModel.DataAnnotations;

using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.Models;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class ERPTaskSchedulerViewModel : BaseViewModel
    {
        public GridModel GridModel { get; set; }
        
        public int ERPTaskSchedulerId { get; set; }

        [Display(Name = ZnodeERP_Resources.LabelERPTaskSchedulerSchedulerName, ResourceType = typeof(ERP_Resources))]
        [Required(ErrorMessageResourceType = typeof(ERP_Resources), ErrorMessageResourceName = ZnodeERP_Resources.RequiredField)]
        [MaxLength(100, ErrorMessageResourceType = typeof(ERP_Resources), ErrorMessageResourceName = ZnodeERP_Resources.ERPTaskSchedulerSchedulerNameLengthErrorMessage)]
        [RegularExpression(AdminConstants.AlphanumericStartWithAlphabetValidation, ErrorMessageResourceName = ZnodeAdmin_Resources.AlphanumericStartWithAlphabet, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string SchedulerName { get; set; }

        [Display(Name = ZnodeERP_Resources.LabelERPTaskSchedulerTouchPointName, ResourceType = typeof(ERP_Resources))]
        [Required(ErrorMessageResourceType = typeof(ERP_Resources), ErrorMessageResourceName = ZnodeERP_Resources.ERPTaskSchedulerTouchPointNameRequiredMessage)]
        [MaxLength(100, ErrorMessageResourceType = typeof(ERP_Resources), ErrorMessageResourceName = ZnodeERP_Resources.ERPTaskSchedulerTouchPointNameLengthErrorMessage)]
        public string TouchPointName { get; set; }

        public string SchedulerFrequency { get; set; }

        [Display(Name = ZnodeERP_Resources.LabelERPTaskSchedulerStartDate, ResourceType = typeof(ERP_Resources))]
        public DateTime? StartDate { get; set; }

        public string StartTime { get; set; }

        public string ActiveERPClassName { get; set; }

        [Display(Name = ZnodeERP_Resources.LabelIsEnabled, ResourceType = typeof(ERP_Resources))]
        public bool IsEnabled { get; set; }

        public int PortalId { get; set; }
        public string IndexName { get; set; }
        public int CatalogId { get; set; }
        public int CatalogIndexId { get; set; }
        public bool IsAssignTouchPoint { get; set; }

        [Display(Name = ZnodeERP_Resources.LabelSchedulerType, ResourceType = typeof(ERP_Resources))]
        public string SchedulerType { get; set; }
        public string ERPClassName { get; set; }
        public string SchedulerCallFor { get; set; }
        public string DomainName { get; set; }

        [MaxLength(100)]
        [Required(ErrorMessageResourceType = typeof(ERP_Resources), ErrorMessageResourceName = ZnodeERP_Resources.ErrorInvalidCronExpression)]
        public string CronExpression { get; set; }
        public string HangfireJobId { get; set; }
    }
}