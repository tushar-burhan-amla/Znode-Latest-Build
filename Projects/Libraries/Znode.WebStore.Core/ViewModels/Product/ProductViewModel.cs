using System.Collections.Generic;

namespace Znode.Engine.WebStore.ViewModels
{
    public class ProductViewModel : PublishProductViewModel
    {
        public int TotalReviews { get; set; }
        public decimal Rating { get; set; }
        public int CategoryId { get; set; }
        public string BundleProductIds { get; set; }
        public string CategoryName { get; set; }

        public string SEODescription { get; set; }
        public string SEOKeywords { get; set; }
        public string SEOTitle { get; set; }
        public string SEOUrl { get; set; }
        public string ProductTemplateName { get; set; }
        public string CurrencyCode { get; set; }
        public string AutoCompleteLabel { get; set; }

        public string ImageLargePath { get; set; }
        public string ImageMediumPath { get; set; }
        public string ImageThumbNailPath { get; set; }
        public string ImageSmallPath { get; set; }
        public string ImageSmallThumbnailPath { get; set; }
        public string OriginalImagepath { get; set; }
        public string ParentProductImageSmallPath { get; set; }

        public string InventoryMessage { get; set; }
        public bool ShowAddToCart { get; set; }
        public bool IsConfigurable { get; set; }
        public bool IsCallForPricing { get; set; }
        public bool EnableProductCompare { get; set; }
        public decimal? ProductPrice { get; set; }
        public bool IsAddedInWishList { get; set; }
        public int WishListId { get; set; }

        public string BreadCrumbHtml { get; set; }

        public List<TierPriceViewModel> TierPriceList { get; set; }
        public List<AddOnViewModel> AddOns { get; set; }
        public List<ProductAlterNateImageViewModel> AlternateImages { get; set; }
        public List<GroupProductViewModel> AssociatedGroupProducts { get; set; }
        public List<AttributeValueSwatchViewModel> SwatchAttributesValues { get; set; }

        public ConfigurableAttributeViewModel ConfigurableData { get; set; }
        public string UOM { get; set; }
        //This property is used to set the values of cart parameters.
        public Dictionary<string, string> CartParameter { get; set; }
        public decimal? UnitPrice { get; set; }
        public string InStockMessage { get; set; }
        public string OutOfStockMessage { get; set; }
        public string BackOrderMessage { get; set; }
        public int ConfigurableProductId { get; set; }
        public bool IsDefaultConfigurableProduct { get; set; }
        public ProductViewModel()
        {
            ProductReviews = new List<ProductReviewViewModel>();
        }

        public List<CategoryViewModel> CategoryHierarchy { get; set; }

        public decimal CartQuantity { get; set; }
        public int BrandId { get; set; }
        public string BrandSeoUrl { get; set; }
        public bool IsBrandActive { get; set; }
        public bool IsQuickView { get; set; }

        public string GroupProductPriceMessage { get; set; }

        public List<ProductPromotionViewModel> Promotions { get; set; }

        public bool IsProductEdit { get; set; }
        public List<AssociatedProductsViewModel> AssociatedProducts { get; set; }
        public Dictionary<string, decimal> AdditionalCost { get; set; }
        public string CultureCode { get; set; }
        public string ParentConfiguarableProductName { get; set; }
        public string CanonicalURL { get; set; }
        public string RobotTagValue { get; set; }
        public bool PriceView { get; set; }
        public string ObsoleteClass { get; set; }
        public string MinQuantity { get; set; }
        public string DefaultInventoryCount { get; set; }
        public string DefaultWarehouseName { get; set; }

        //Returns the Ecommerce Product details to be sent to the datalayer.
        public EcommerceDataViewModel GetEcommerceDetailsOfProduct() =>
            new EcommerceDataViewModel() { SKU = SKU, Name = Name, CategoryName = CategoryName, ProductPrice = RetailPrice, BrandName = Attributes?.ValueFromSelectValue(Libraries.ECommerce.Utilities.ZnodeConstant.Brand) };
        public List<HighlightsViewModel> HighlightLists { get; set; }

        public decimal AssociatedQuantity { get; set; }
        public List<AssociatedPublishBundleProductViewModel> PublishBundleProducts { get; set; }
        public bool IsAddToCartOptionForProductSlidersEnabled { get; set; }
    }
}