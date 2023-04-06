using System.Collections.Generic;

namespace Znode.Engine.WebStore.ViewModels
{
    public class AddOnValuesViewModel : BaseViewModel
    {
        public int PublishProductId { get; set; }
        public int LocaleId { get; set; }

        public string SKU { get; set; }
        public string Name { get; set; }

        public decimal? Quantity { get; set; }
        public decimal? ReOrderLevel { get; set; }
        public decimal? SalesPrice { get; set; }
        public decimal? RetailPrice { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencySuffix { get; set; }
        public bool IsDefault { get; set; }
        public string CultureCode { get; set; }

        public List<AttributesViewModel> Attributes { get; set; }
    }
}