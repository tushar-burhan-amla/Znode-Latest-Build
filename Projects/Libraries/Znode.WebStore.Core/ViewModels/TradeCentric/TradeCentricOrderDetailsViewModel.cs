namespace Znode.Engine.WebStore.ViewModels
{
    public class TradeCentricOrderDetailsViewModel
    {
        public string currency { get; set; }
        public string total { get; set; }
        public string shipping { get; set; }
        public string shipping_title { get; set; }
        public string tax { get; set; }
        public string Tax_title { get; set; }
        public string Discount { get; set; }
        public string Discount_title { get; set; }
        public TradeCentricAddressDetailsViewModel Bill_to { get; set; }
        public TradeCentricAddressDetailsViewModel Ship_to { get; set; }
        public TradeCentricContactViewModel Contact { get; set; }
    }
}
