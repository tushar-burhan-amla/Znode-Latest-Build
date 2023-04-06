using System.Collections.Generic;

namespace Znode.Engine.Admin.ViewModels
{
    public class PublishProductsViewModel : BaseViewModel
    {
        public int PublishProductId { get; set; }
        public int ParentProductId { get; set; }
        public int ZnodeCategoryIds { get; set; }
        public int ZnodeCatalogId { get; set; }
        public int LocaleId { get; set; }
        public int PortalId { get; set; }
        public int UserId { get; set; }

        public string SKU { get; set; }

        //This property is used only for Configurable Product Type.
        public string ConfigurableProductSKU { get; set; }
        public string Version { get; set; }
        public string Name { get; set; }
        public string CurrencyCode { get; set; }
        public decimal? Quantity { get; set; }
        public decimal? ReOrderLevel { get; set; }
        public decimal? SalesPrice { get; set; }
        public decimal? RetailPrice { get; set; }
        public decimal? ProductPrice { get; set; }
        public string ProductType { get; set; }
        public string AddOnProductValues { get; set; }
        public bool IsConfigurable { get; set; }
        public bool AllowAddToCart { get; set; }
        public string InventoryMessage { get; set; }
        public bool ShowAddToCart { get; set; }
        public decimal? MinQuantity { get; set; }
        public decimal? MaxQuantity { get; set; }
        public bool? AllowBackOrder { get; set; }
        public bool? TrackInventory { get; set; }
        public List<PublishAttributeViewModel> Attributes { get; set; }
        public List<ProductPromotionViewModel> Promotions { get; set; }
        public List<AddOnViewModel> AddOns { get; set; }
        public List<TierPriceViewModel> TierPriceList { get; set; }
        public ConfigurableAttributeViewModel ConfigurableData { get; set; }
        public int PromotionId { get; set; }
        public string ImageLargePath { get; set; }
        public string ImageMediumPath { get; set; }
        public string ImageThumbNailPath { get; set; }
        public string ImageSmallPath { get; set; }
        public string ImageSmallThumbnailPath { get; set; }
        public string RetailPriceWithCurrency { get; set; }
        public string SalesPriceWithCurrency { get; set; }
        public string InStockMessage { get; set; }
        public string OutOfStockMessage { get; set; }
        public string BackOrderMessage { get; set; }
        public string CategoryName { get; set; }
        public string CatalogName { get; set; }
        public bool IsDefaultConfigurableProduct { get; set; }
        public decimal PromotionalPrice { get; internal set; }
        public bool IsActive { get; set; }
        public bool IsCallForPricing { get; set; }
        public List<GroupProductViewModel> AssociatedGroupProducts { get; set; }
        public string CultureCode { get; set; }
        public bool IsQuote { get; set; }
        public decimal AssociatedQuantity { get; set; }
        public List<AssociatedPublishedBundleProductViewModel> PublishBundleProducts { get; set; }
    }
}