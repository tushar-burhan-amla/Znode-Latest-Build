using System.Collections.Generic;
using System.Web.Mvc;

namespace Znode.Engine.Api.Models
{
    public class PortalApprovalModel : BaseModel
    {
        public int PortalApprovalId { get; set; }
        public bool EnableApprovalManagement { get; set; }
        public int PortalApprovalTypeId { get; set; }
        public int PortalApprovalLevelId { get; set; }
        public string PortalApprovalLevelName { get; set; }
        public decimal? OrderLimit { get; set; }
        public int PortalId { get; set; }
        public string[] ApprovalUserIds { get; set; }
        public List<UserApproverModel> UserApprover { get; set; }
        public List<PortalPaymentApproverModel> PortalPaymentUserApproverList { get; set; }
        public string[] PaymentTypeIds { get; set; }
        public int PortalPaymentGroupId { get; set; }
        public List<SelectListItem> PortalApprovalTypes { get; set; }
        public List<SelectListItem> PortalApprovalLevel { get; set; }
        public int UserId { get; set; }
        public string ApproverName { get; set; }
        public int ApproverUserId { get; set; }
        public int UserApproverId { get; set; }
        public string ApproverUser { get; set; }
        public string PortalApprovalTypeName { get; set; }
    }
}
