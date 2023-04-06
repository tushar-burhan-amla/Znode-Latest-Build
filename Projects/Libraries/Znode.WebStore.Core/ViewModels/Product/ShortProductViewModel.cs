using System.Collections.Generic;
using Znode.Engine.Api.Models;

namespace Znode.Engine.WebStore.ViewModels
{
    public class ShortProductViewModel : BaseModel
	{
		public int PublishProductId { get; set; }
		public int ParentProductId { get; set; }
		public string SKU { get; set; }
		public string ConfigurableProductSKU { get; set; }
		public string Name { get; set; }
		public bool IsQuickView { get; set; }
		public ConfigurableAttributeViewModel ConfigurableData { get; set; }
		public bool IsCallForPricing { get; set; }
		public bool IsConfigurable { get; set; }
		public int CategoryId { get; set; }
		public bool IsAddedInWishList { get; set; }
		public int WishListId { get; set; }
		
		public ProductStoreSettingsViewModel StoreSettings { get; set; }
		public ProductDefaultImageViewModel ProductImage { get; set; }
		public ProductSEODetailsViewModel SEO { get; set; }
		public ProductBrandViewModel Brand { get; set; }
		public List<ProductPromotionViewModel> Promotions { get; set; }
		public ProductPricingViewModel Pricing { get; set; }
		public ProductInventoryViewModel InventoryDetails { get; set; }
		public List<CategoryViewModel> CategoryHierarchy { get; set; }
		public List<AttributesViewModel> Attributes { get; set; }		
		public Dictionary<string, string> CartParameter { get; set; }
		public List<AddOnViewModel> AddOns { get; set; }
		public ProductReviewsViewModel CustomerReviews { get; set; }
		public ProductAssociationsViewModel ProductAssociations { get; set; }
		public ProductMiscellaneousDetailsViewModel MiscellaneousDetails { get; set; }
		
		public bool EnableProductCompare { get; set; }
		public List<AttributeValueSwatchViewModel> SwatchAttributesValues { get; set; }		
        public string GroupProductPriceMessage { get; set; }
        public bool IsProductEdit { get; set; }        
    }

	public class ProductDefaultImageViewModel
	{
		public string ImageLargePath { get; set; }
		public string ImageMediumPath { get; set; }
		public string ImageThumbNailPath { get; set; }
		public string ImageSmallPath { get; set; }
		public string ImageSmallThumbnailPath { get; set; }
		public string OriginalImagepath { get; set; }
		public List<ProductAlterNateImageViewModel> AlternateImages { get; set; }
	}

	public class ProductSEODetailsViewModel
	{
		public string SEODescription { get; set; }
		public string SEOKeywords { get; set; }
		public string SEOTitle { get; set; }
		public string SEOUrl { get; set; }
	}

	public class ProductPricingViewModel
	{
		public decimal? RetailPrice { get; set; }
		public string CurrencyCode { get; set; }
		public decimal? ProductPrice { get; set; }
		public List<TierPriceViewModel> TierPriceList { get; set; }
		public decimal? UnitPrice { get; set; }
		public string CultureCode { get; set; }
		public decimal? SalesPrice { get; set; }
		public decimal? PromotionalPrice { get; set; }
		public Dictionary<string, decimal> AdditionalCost { get; set; }
	}

	public class ProductInventoryViewModel
	{
		public decimal? Quantity { get; set; }
		public decimal? ReOrderLevel { get; set; }
		public string InventoryMessage { get; set; }
		public bool ShowAddToCart { get; set; }
		public decimal CartQuantity { get; set; }
		public List<InventorySKUViewModel> Inventory { get; set; }
	}

	public class ProductStoreSettingsViewModel
	{
		public string InStockMessage { get; set; }
		public string OutOfStockMessage { get; set; }
		public string BackOrderMessage { get; set; }
	}

	public class ProductMiscellaneousDetailsViewModel
	{
		public bool IsDefaultConfigurableProduct { get; set; }
		public string ParentConfiguarableProductName { get; set; }
		public int ConfigurableProductId { get; set; }
		public string CategoryName { get; set; }
		public string ProductType { get; set; }
	}

	public class ProductBrandViewModel
	{
		public int BrandId { get; set; }
		public string BrandSeoUrl { get; set; }
		public bool IsBrandActive { get; set; }
	}

	public class ProductReviewsViewModel
	{
		public List<ProductReviewViewModel> ProductReviews { get; set; }
		public int TotalReviews { get; set; }
		public decimal Rating { get; set; }
	}

	public class ProductAssociationsViewModel
	{
		public List<GroupProductViewModel> AssociatedGroupProducts { get; set; }
		public List<AssociatedProductsViewModel> AssociatedProducts { get; set; }
	}
}