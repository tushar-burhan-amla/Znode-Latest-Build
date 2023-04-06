namespace Znode.Multifront.PaymentApplication.Models
{
    public class PaymentCreditCardModel : BaseModel
    {
        public string gateway { get; set; }
        public string twoCoUrl { get; set; }
        public string customerGUID { get; set; }
        public int? profileId { get; set; }
        public int paymentSettingId { get; set; }
        public string CurrencyCode { get; set; }
        public string PaymentCode { get; set; }
        public int PublishStateId { get; set; }
    }
}
