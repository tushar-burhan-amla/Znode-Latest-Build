namespace Znode.Engine.Admin.ViewModels
{
    public class PaymentDetailsViewModel : BaseViewModel
    {
        public string PaymentCode { get; set; }
        public string GatewayCode { get; set; }        
        public int? PaymentProfileId { get; set; }
        public string Total { get; set; }
        public int PaymentApplicationSettingId { get; set; }
        public int IsCreditCardEnabled { get; set; }
    }
}