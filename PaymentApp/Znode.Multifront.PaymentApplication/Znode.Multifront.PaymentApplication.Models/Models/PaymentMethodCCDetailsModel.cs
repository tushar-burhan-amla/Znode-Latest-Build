namespace Znode.Multifront.PaymentApplication.Models
{
    public class PaymentMethodCCDetailsModel : BaseModel
    {
        public System.Guid PaymentGUID { get; set; }

        public string CreditCardLastFourDigit { get; set; }
        public string CreditCardImageUrl { get; set; }
        public string CardHolderFirstName { get; set; }
        public string CardHolderLastName { get; set; }
        public string AddressLine1 { get; set; }
        public string AddressLine2 { get; set; }
        public string City { get; set; }
        public string State { get; set; }
        public string Country { get; set; }
        public string ZipCode { get; set; }
        public string CardType { get; set; }
        public string Token { get; set; }
    }
}
