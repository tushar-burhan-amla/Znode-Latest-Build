using Znode.Engine.Admin.Models;
namespace Znode.Engine.Admin.ViewModels
{
    public class OrderTotalViewModel : BaseViewModel
    {
        public decimal? Total { get; set; }
        public decimal? SubTotal { get; set; }
        public decimal? ShippingCost { get; set; }
        public decimal? DiscountAmount { get; set; }
        public decimal? CSRDiscountAmount { get; set; }
        public decimal? TaxCost { get; set; }
        public decimal? GiftCardAmount { get; set; }
        public string CurrencyCode { get; set; }
    }
}
