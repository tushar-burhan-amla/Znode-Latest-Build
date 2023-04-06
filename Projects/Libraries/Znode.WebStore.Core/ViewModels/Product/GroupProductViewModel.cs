using System.Collections.Generic;

namespace Znode.Engine.WebStore.ViewModels
{
    public class GroupProductViewModel : BaseViewModel
    {
        public int PublishProductId { get; set; }
        public int ParentPublishProductId { get; set; }
        public string Name { get; set; }
        public int LocaleId { get; set; }
        public string SKU { get; set; }
        public decimal? SalesPrice { get; set; }
        public decimal? RetailPrice { get; set; }
        public string CurrencyCode { get; set; }
        public decimal? Quantity { get; set; }
        public string ChangedQuantity { get; set; }
        public decimal? TotalPrice { get; set; }
        public decimal? ReOrderLevel { get; set; }
        public string InventoryMessage { get; set; }
        public bool ShowAddToCart { get; set; }
        public bool IsCallForPricing { get; set; }
        public string ImageLargePath { get; set; }
        public string ImageMediumPath { get; set; }
        public string ImageThumbNailPath { get; set; }
        public string ImageSmallPath { get; set; }
        public string ImageSmallThumbnailPath { get; set; }
        public List<AttributesViewModel> Attributes { get; set; }
        public string InStockMessage { get; set; }
        public string OutOfStockMessage { get; set; }
        public string BackOrderMessage { get; set; }
        public int AssociatedProductDisplayOrder { get; set; }
        public decimal? ShoppingCartQuantity { get; set; }
        public string CultureCode { get; set; }

        //Property to map DefaultWarehouseCount of product
        public string DefaultWarehouseCount { get; set; }

        //Property to map DefaultWarehouseName of product
        public string DefaultWarehouseName { get; set; }
    }
}