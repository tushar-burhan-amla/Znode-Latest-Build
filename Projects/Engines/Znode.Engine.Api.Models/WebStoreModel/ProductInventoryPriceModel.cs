using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class ProductInventoryPriceModel : BaseModel
    {
        public string SKU { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? ReOrderLevel { get; set; }
        public decimal? SalesPrice { get; set; }
        public decimal? RetailPrice { get; set; }
        public decimal? PromotionalPrice { get; set; }
        public string CurrencyCode { get; set; }
        public string GroupProductPriceMessage { get; set; }
        public decimal? ProductPrice { get; set; }
        public List<InventorySKUModel> Inventory { get; set; }
        public string CultureCode { get; set; }
        public decimal? AllLocationQuantity { get; set; }
        public int PortalId { get; set; }
        public int ProductId { get; set; }
        public bool PriceView { get; set; }
        public string ObsoleteClass { get; set; }
        public string ProductType { get; set; }
        public string MinQuantity { get; set; }
        public List<PriceTierModel> TierPriceList { get; set; }

        //Property to map DefaultWarehouseCount of product
        public string DefaultWarehouseCount { get; set; }

        //Property to map DefaultWarehouseName of product
        public string DefaultWarehouseName { get; set; }
    }
}
