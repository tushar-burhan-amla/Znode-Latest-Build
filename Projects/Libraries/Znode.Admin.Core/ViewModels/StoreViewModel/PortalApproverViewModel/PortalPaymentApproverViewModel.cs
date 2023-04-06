using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class PortalPaymentApproverViewModel: BaseViewModel
    {
        public List<UserApproverViewModel> UserApprover { get; set; }
        public string PortalPaymentGroupCode { get; set; }
        
        public int? PortalPaymentGroupId { get; set; }
        [Display(Name = ZnodeAdmin_Resources.LabelPaymentName, ResourceType = typeof(Admin_Resources))]
        public string[] PaymentSettingIds { get; set; }
        public string[] ApprovalUserIds { get; set; }
        public int PaymentDivCount { get; set; }
        public List<SelectListItem> PaymentTypes { get; set; }
    }
}
