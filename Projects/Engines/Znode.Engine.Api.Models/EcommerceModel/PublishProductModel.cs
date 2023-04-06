using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class PublishProductModel : ProductInventoryPriceModel
    {
        public int PublishProductId { get; set; }
        public int ParentPublishProductId { get; set; }
        public string ParentPublishProductSKU { get; set; }

        public int ZnodeCategoryIds { get; set; }
        public int ZnodeCatalogId { get; set; }
        public int LocaleId { get; set; }
        public int? TotalReviews { get; set; }

        //This property is used only for Configurable Product Type.
        public string ConfigurableProductSKU { get; set; }
        public string Version { get; set; }
        public string Name { get; set; }
        public string ProductTemplateName { get; set; }

        public string CurrencySuffix { get; set; }
        public decimal? ShippingCost { get; set; }

        public string InventoryMessage { get; set; }
        public bool ShowAddToCart { get; set; }        

        public string SEODescription { get; set; }
        public string SEOKeywords { get; set; }
        public string SEOTitle { get; set; }
        public string SEOUrl { get; set; }

        public string ImageLargePath { get; set; }
        public string ImageMediumPath { get; set; }
        public string ImageThumbNailPath { get; set; }
        public string ImageSmallPath { get; set; }
        public string ImageSmallThumbnailPath { get; set; }
        public string ParentProductImageSmallPath { get; set; }

        public string OriginalImagepath { get; set; }

        public string AddonProductSKUs { get; set; }
        public string BundleProductSKUs { get; set; }
        public string ConfigurableProductSKUs { get; set; }
        public List<AssociatedProductModel> GroupProductSKUs { get; set; }
        public int PublishedCatalogId { get; set; }
        public List<WebStoreGroupProductModel> AssociatedGroupProducts { get; set; }
        public decimal? Rating { get; set; }
        public List<PriceTierModel> TierPriceList { get; set; }
        public List<PublishAttributeModel> Attributes { get; set; }
        public List<ProductPromotionModel> Promotions { get; set; }
        public List<CustomerReviewModel> ProductReviews { get; set; }
        public List<WebStoreAddOnModel> AddOns { get; set; }
        public List<ProductAlterNateImageModel> AlternateImages { get; set; }
        public int PromotionId { get; set; }
        public bool IsDefaultConfigurableProduct { get; set; }
        public bool IsSimpleProduct { get; set; }

        public List<PublishCategoryModel> CategoryHierarchy { get; set; }
        public string InStockMessage { get; set; }
        public string OutOfStockMessage { get; set; }
        public string BackOrderMessage { get; set; }

        public bool IsConfigurableProduct { get; set; }
        public string ParentConfiguarableProductName { get; set; }
        public int ConfigurableProductId { get; set; }
        public int BrandId { get; set; }
        public int PortalId { get; set; }
        public string BrandSeoUrl { get; set; }
        public bool IsBrandActive { get; set; }
        public string CategoryName { get; set; }
        public string CatalogName { get; set; }
        public string ProductName { get; set; }
        public string ProductType { get; set; }
        public bool IsActive { get; set; }
        public string IsPublish { get; set; }
        public List<AssociatedProductsModel> AssociatedProducts { get; set; }
        public string PublishStatus { get; set; }
        public Dictionary<string, decimal> AdditionalCost { get; set; }
        public string ParentSEOCode { get; set; }
        public string SEOCode { get; set; }
        public string ProductImagePath { get; set; }
        public int PimProductId { get; set; }
        public List<AssociatedProductModel> AssociatedAddOnProducts { get; set; }
        public List<int> ZnodeProductCategoryIds { get; set; }
        public int? DisplayOrder { get; set; }
        public string SKU { get; set; }
        public string CanonicalURL { get; set; }
        public string RobotTagValue { get; set; }
        public string DefaultWarehouseName { get; set; }
        public string DefaultInventoryCount { get; set; }
        public List<OrderDiscountModel> OrdersDiscount { get; set; }
        public decimal HST { get; set; }
        public decimal GST { get; set; }
        public decimal PST { get; set; }
        public decimal VAT { get; set; }
        public decimal ImportDuty { get; set; }
        public decimal SalesTax { get; set; }
        public decimal DiscountAmount { get; set; }
        public decimal? AssociatedQuantity { get; set; }
        public List<AssociatedPublishedBundleProductModel> PublishBundleProducts { get; set; }
    }
}
