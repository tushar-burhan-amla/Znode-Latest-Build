using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class WebStoreConfigurableProductModel : BaseModel
    {
        public int PublishProductId { get; set; }
        public string SKU { get; set; }
        public string Name { get; set; }
        public int LocaleId { get; set; }
        public string CurrencySuffix { get; set; }
        public string ImageLargePath { get; set; }
        public string ImageName { get; set; }
        public string Attributes { get; set; }
        public List<PublishAttributeModel> ProductAttributes { get; set; }
        public string AlternateImageName { get; set; }
        public string InStockMessage { get; set; }
        public string OutOfStockMessage { get; set; }
        public string BackOrderMessage { get; set; }
        public int AssociatedProductDisplayOrder { get; set; }
        public string WarehouseName { get; set; }
        public string DefaultInventoryCount { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? ReOrderLevel { get; set; }
        public decimal? SalesPrice { get; set; }
        public decimal? RetailPrice { get; set; }
        public string CurrencyCode { get; set; }
        public string ConfigurableAttributeCodes { get; set; }
        public List<string> ConfigurableAttributeCodeList { get; set; }
    }
}
