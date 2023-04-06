using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class PortalApprovalViewModel : BaseViewModel
    {
        public int PortalApprovalId { get; set; }
        public bool EnableApprovalManagement { get; set; } = true;
        
        [Display(Name = ZnodeAdmin_Resources.LabelPortalApprovalType, ResourceType = typeof(Admin_Resources)),
        Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorPortalApprovalType)]
        public int PortalApprovalTypeId { get; set; }
        public string PortalApprovalTypeName { get; set; }
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorPortalApprovalLevel),
        Display(Name = ZnodeAdmin_Resources.LabelPortalApprovalLevel, ResourceType = typeof(Admin_Resources))]
        public int PortalApprovalLevelId { get; set; }
        
        public decimal? OrderLimit { get; set; }
        public int PortalId { get; set; }
        public string PortalName { get; set; }
        public List<SelectListItem> Portals { get; set; }
        public List<SelectListItem> PortalApprovalTypes { get; set; }
        public List<SelectListItem> PortalApprovalLevel { get; set; }
        public int UserId { get; set; }
        public string ApproverName { get; set; }
        public int ApproverUserId { get; set; }
        public int UserApproverId { get; set; }
        public string ApproverUser { get; set; }
        public List<UserApproverViewModel> UserApprover { get; set; }
        public string[] ApprovalUserIds { get; set; }
        public string[] PaymentTypeIds { get; set; }
        public List<SelectListItem> PaymentTypes { get; set; }
        public int? PortalPaymentGroupId { get; set; }
        public int PaymentDivCount { get; set; }
        public List<PortalPaymentApproverViewModel> PortalPaymentUserApproverList { get; set; }
    }
}
