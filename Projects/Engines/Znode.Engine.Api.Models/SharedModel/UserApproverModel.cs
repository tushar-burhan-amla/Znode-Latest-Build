using System;

namespace Znode.Engine.Api.Models
{
    public class UserApproverModel : BaseModel
    {
        public int UserApproverId { get; set; }
        public int? UserId { get; set; }
        public int ApproverLevelId { get; set; }
        public int? PortalPaymentGroupId { get; set; }
        public int ApproverUserId { get; set; }
        public int ApproverOrder { get; set; }
        public bool IsNotifyEmail { get; set; }
        public bool IsMandatory { get; set; }
        public Nullable<decimal> ToBudgetAmount { get; set; }
        public Nullable<decimal> FromBudgetAmount { get; set; }
        public int OmsOrderStateId { get; set; }
        public int OmsQuoteId { get; set; }
        public string ApproverName { get; set; }
        public string OmsOrderState { get; set; }
        public string ApproverLevelName { get; set; }
        public Nullable<DateTime> StatusModifiedDate { get; set; }
        public Nullable<bool> IsNoLimit { get; set; }
        public Nullable<bool> IsActive { get; set; }
        public Nullable<int> PortalApprovalId { get; set; }
        public string FullName { get; set; }
    }
}
