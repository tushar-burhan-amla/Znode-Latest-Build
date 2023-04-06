namespace Znode.Engine.Admin.ViewModels
{
    public class PortalPaymentGroupViewModel : BaseViewModel
    {
        public int PortalPaymentGroupId { get; set; }
        public string PaymentGroupCode { get; set; }
        public int PortalApprovalId { get; set; }
        public PaymentSettingViewModel PaymentSetting { get; set; }
        public UserApproverViewModel UserApprover { get; set; }
    }
}
