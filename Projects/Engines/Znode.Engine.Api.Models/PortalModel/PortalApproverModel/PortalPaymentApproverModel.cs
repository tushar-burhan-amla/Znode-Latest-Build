using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class PortalPaymentApproverModel : BaseModel
    {
        public int PortalPaymentApprovalId { get; set; }
        public int PaymentTypeID { get; set; }
        public string[] PaymentSettingIds { get; set; }
        public string[] ApprovalUserIds { get; set; }
        public int? PortalPaymentGroupId { get; set; }
        public string PortalPaymentGroupCode { get; set; }
        public List<UserApproverModel> UserApprover { get; set; }
    }
}
