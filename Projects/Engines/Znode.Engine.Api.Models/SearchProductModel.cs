using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class SearchProductModel : ProductInventoryPriceModel
    {

        public string Id { get; set; }
        public string ExternalId { get; set; }
        public int ZnodeProductId { get; set; }
        public int Version { get; set; }
        public int LocaleId { get; set; }
        public string Name { get; set; }
        public int CatalogId { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }        
        public string CurrencySuffix { get; set; }
        public decimal? Rating { get; set; }
        public int? TotalReviews { get; set; }
        public string SEODescription { get; set; }
        public string SEOKeywords { get; set; }
        public string SEOTitle { get; set; }
        public string SEOUrl { get; set; }
        public List<ProductPromotionModel> Promotions { get; set; }
        public List<PublishAttributeModel> Attributes { get; set; }
        public decimal Boost { get; set; }
        public List<string> HighlightList { get; set; }
        public List<WebStoreAddOnModel> AddOns { get; set; }
        public List<WebStoreGroupProductModel> AssociatedGroupProducts { get; set; }
        public List<WebStoreAttributeValueSwatchModel> SwatchAttributesValues { get; set; }
        public string ImageLargePath { get; set; }
        public string ImageMediumPath { get; set; }
        public string ImageThumbNailPath { get; set; }
        public string ImageSmallPath { get; set; }
        public string ImageSmallThumbnailPath { get; set; }
        public string InStockMessage { get; set; }
        public string OutOfStockMessage { get; set; }
        public string BackOrderMessage { get; set; }

        public List<AssociatedProductsModel> AssociatedProducts { get; set; }
        public List<int> ParentCategoryIds { get; set; }
    }
}