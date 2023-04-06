namespace Znode.Engine.Admin.ViewModels
{
    public class IssuedGiftCardViewModel
    {
        public string CardNumber { get; set; }
        public decimal Amount { get; set; }
        public string ExpirationDate { get; set; }
        public string CurrencyCode { get; set; }
        public string CultureCode { get; set; }
    }
}