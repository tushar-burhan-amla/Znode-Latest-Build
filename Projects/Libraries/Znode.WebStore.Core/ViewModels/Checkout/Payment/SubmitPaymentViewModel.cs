namespace Znode.Engine.WebStore.ViewModels
{
    public class SubmitPaymentViewModel : BaseViewModel
    {
        public int PaymentApplicationSettingId { get; set; }
        public string CustomerProfileId { get; set; }
        public string CustomerPaymentId { get; set; }
        public string CustomerGuid { get; set; }
        public string PaymentToken { get; set; }
        public string PayPalReturnUrl { get; set; }
        public string PayPalCancelUrl { get; set; }
        public string PaymentType { get; set; }
        public string AmazonPayReturnUrl { get; set; }
        public string AmazonPayCancelUrl { get; set; }
        public string AmazonOrderReferenceId { get; set; }
        public string CardType { get; set; }
        public string CcExpiration { get; set; }
        public string TransactionId { get; set; }
        public string AccountNumber { get; set; }
        public string ShippingMethod { get; set; }
        public string CustomerShippingAddressId { get; set; }
        public string CardSecurityCode { get; set; }
        public string PaymentCode { get; set; }
        public string IsSaveCreditCard { get; set; }
        public string CyberSourceToken { get; set; }
        public string Email { get; set; }
        public string CardHolderName { get; set; }
       public System.Guid PaymentGUID { get; set; }
        public string GatewayCode { get; set; }
        public string PaymentMethodNonce { get; set; } //This field is specific for braintree PCI compliance.
        public string PortalName { get; set; }
    }
}