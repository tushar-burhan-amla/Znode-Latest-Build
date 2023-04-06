using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
	public class PublishProductDTO : BaseModel
	{
		public int PublishProductId { get; set; }
		public int ParentPublishProductId { get; set; }
		public int PublishedCatalogId { get; set; }
		public int ZnodeCategoryIds { get; set; }
		public int ZnodeCatalogId { get; set; }
		public int LocaleId { get; set; }
		
		public ProductStoreSettingsDTO StoreSettings { get; set; }
		public ProductImageDTO ProductImage { get; set; }
		public ProductSeoDTO SEO { get; set; }
		public ProductPricingDTO Pricing { get; set; }
		public ProductInventoryDTO InventoryDetails { get; set; }
		public List<PublishCategoryModel> CategoryHierarchy { get; set; }
		public List<PublishAttributeModel> Attributes { get; set; }
		public List<ProductPromotionModel> Promotions { get; set; }
		public List<WebStoreAddOnModel> AddOns { get; set; }
		public ProductReviewsDTO CustomerReviews { get; set; }
		public ProductBrandDTO Brand { get; set; }
		public ProductAssociationsDTO ProductAssociations { get; set; }
		public ProductMiscellaneousDetailsDTO MiscellaneousDetails { get; set; }

		public string SKU { get; set; }

		public string ConfigurableProductSKU { get; set; }
		public string Version { get; set; }
		public string Name { get; set; }
		public string ProductTemplateName { get; set; }

		public int PromotionId { get; set; }
	}

	public class ProductImageDTO
	{
		public string ProductImagePath { get; set; }
		public string ImageLargePath { get; set; }
		public string ImageMediumPath { get; set; }
		public string ImageThumbNailPath { get; set; }
		public string ImageSmallPath { get; set; }
		public string ImageSmallThumbnailPath { get; set; }
		public string OriginalImagepath { get; set; }

		public List<ProductAlterNateImageModel> AlternateImages { get; set; }
	}

	public class ProductSeoDTO
	{
		public string SEODescription { get; set; }
		public string SEOKeywords { get; set; }
		public string SEOTitle { get; set; }
		public string SEOUrl { get; set; }
		public string ParentSEOCode { get; set; }
		public string SEOCode { get; set; }
	}

	public class ProductPricingDTO
	{
		public decimal? RetailPrice { get; set; }
		public string CurrencyCode { get; set; }
		public decimal? ProductPrice { get; set; }
		public List<PriceTierModel> TierPriceList { get; set; }
		public decimal? UnitPrice { get; set; }
		public string CultureCode { get; set; }
		public decimal? SalesPrice { get; set; }
		public string CurrencySuffix { get; set; }
		public decimal? PromotionalPrice { get; set; }
		public Dictionary<string, decimal> AdditionalCost { get; set; }
		public decimal? ShippingCost { get; set; }
	}

	public class ProductInventoryDTO
	{
		public decimal? Quantity { get; set; }
		public decimal? ReOrderLevel { get; set; }
		public string InventoryMessage { get; set; }
		public bool ShowAddToCart { get; set; }
		public List<InventorySKUModel> Inventory { get; set; }
		public string GroupProductPriceMessage { get; set; }
	}

	public class ProductStoreSettingsDTO
	{
		public string InStockMessage { get; set; }
		public string OutOfStockMessage { get; set; }
		public string BackOrderMessage { get; set; }
	}

	public class ProductMiscellaneousDetailsDTO
	{
		public bool IsDefaultConfigurableProduct { get; set; }
		public bool IsSimpleProduct { get; set; }
		public bool IsConfigurableProduct { get; set; }
		public string ParentConfiguarableProductName { get; set; }
		public int ConfigurableProductId { get; set; }		
		public int PortalId { get; set; }		
		public string CategoryName { get; set; }
		public string CatalogName { get; set; }
		public string ProductName { get; set; }
		public string ProductType { get; set; }
		public bool IsActive { get; set; }
		public string IsPublish { get; set; }
		public string PublishStatus { get; set; }
		public int PimProductId { get; set; }
	}

	public class ProductReviewsDTO
	{
		public decimal? Rating { get; set; }
		public List<CustomerReviewModel> ProductReviews { get; set; }
		public int? TotalReviews { get; set; }
	}

	public class ProductBrandDTO
	{
		public int BrandId { get; set; }
		public string BrandSeoUrl { get; set; }
		public bool IsBrandActive { get; set; }
	}

	public class ProductAssociationsDTO
	{
		public string AddonProductSKUs { get; set; }
		public string BundleProductSKUs { get; set; }
		public string ConfigurableProductSKUs { get; set; }
		public List<AssociatedProductModel> GroupProductSKUs { get; set; }
		public List<AssociatedProductModel> AssociatedAddOnProducts { get; set; }
		public List<WebStoreGroupProductModel> AssociatedGroupProducts { get; set; }
		public List<AssociatedProductsModel> AssociatedProducts { get; set; }
	}
}
