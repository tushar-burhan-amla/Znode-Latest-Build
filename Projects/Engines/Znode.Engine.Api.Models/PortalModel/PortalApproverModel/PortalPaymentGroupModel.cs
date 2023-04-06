namespace Znode.Engine.Api.Models
{
    public class PortalPaymentGroupModel : BaseModel
    {
        public int PortalPaymentGroupId { get; set; }
        public string PaymentGroupCode { get; set; }
        public int PortalApprovalId { get; set; }
    }
}
