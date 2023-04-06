namespace Znode.Engine.Api.Models
{
    public class PaymentSettingPortalModel : BaseModel
    {
        public int PortalId { get; set; }
        public int PaymentSettingId { get; set; }
        public string PaymentDisplayName { get; set; }
        public string PaymentExternalId { get; set; }
        public bool IsApprovalRequired { get; set; }
        public bool IsOABRequired { get; set; }
    public string PublishState { get; set; }
  }
}
