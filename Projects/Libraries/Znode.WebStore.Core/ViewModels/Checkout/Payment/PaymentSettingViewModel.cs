namespace Znode.Engine.WebStore.ViewModels
{
    public class PaymentSettingViewModel : BaseViewModel
    {
        public int PaymentApplicationSettingId { get; set; }
        public int PaymentSettingId { get; set; }
        public int? ProfileId { get; set; }
        public int PaymentTypeId { get; set; }
        public bool PreAuthorize { get; set; }
        public string PaymentTypeName { get; set; }
        public string PaymentDisplayName { get; set; }
        public string GatewayPassword { get; set; }
        public string TransactionKey { get; set; }
        public string GatewayUsername { get; set; }
        public bool TestMode { get; set; }
        public string PaymentCode { get; set; }
        public int? PaymentGatewayId { get; set; }
    }
}