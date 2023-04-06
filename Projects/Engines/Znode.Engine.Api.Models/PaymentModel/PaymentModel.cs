namespace Znode.Engine.Api.Models
{
    public class PaymentModel : BaseModel
    {
        public string AuthorizationCode { get; set; }
        public AddressModel BillingAddress { get; set; }
        public AddressModel ShippingAddress { get; set; }
        public decimal NonRecurringItemsTotalAmount { get; set; }
        public string PaymentName { get; set; }
        public string PaymentDisplayName { get; set; }
        public PaymentSettingModel PaymentSetting { get; set; }
        public bool RecurringBillingExists { get; set; }
        public bool SaveCardData { get; set; }
        public bool IsPreAuthorize { get; set; }

        public string SubscriptionId { get; set; }
        public int TokenId { get; set; }
        public string TransactionId { get; set; }
        public bool UseToken { get; set; }
        public string PaymentExternalId { get; set; }
        public bool TestMode { get; set; }
        public int PaymentStatusId { get; set; }

    }
}
