using System.Collections.Generic;

namespace Znode.Engine.WebStore.ViewModels
{
    public class ProductPriceViewModel
    {
        public string sku { get; set; }
        public string type { get; set; }
        public decimal? SalesPrice { get; set; }
        public decimal? RetailPrice { get; set; }
        public string DisplaySalesPrice { get; set; }
        public string DisplayRetailPrice { get; set; }
        public string CurrencyCode { get; set; }
        public string ProductType { get; set; }
        public bool PriceView { get; set; }
        public string ObsoleteClass { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? ReOrderLevel { get; set; }
        public string CultureCode { get; set; }
        public decimal PromotionalPrice { get; set; }
        public int PublishProductId { get; set; }
        public string MinQuantity { get; set; }
        public decimal? ProductPrice { get; set; }
        public List<TierPriceViewModel> TierPriceList { get; set; }
    }
}