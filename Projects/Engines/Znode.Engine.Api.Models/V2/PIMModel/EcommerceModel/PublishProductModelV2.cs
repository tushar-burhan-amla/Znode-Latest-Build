using System.Collections.Generic;

namespace Znode.Engine.Api.Models.V2
{
    public class PublishProductModelV2 : BaseModel
    {
        public PublishProductModelV2()
        {
            AssociatedProducts = new List<AssociatedProductsModel>();
            AddOns = new List<WebStoreAddOnModel>();
        }

        public int PublishProductId { get; set; }
        public int ZnodeCategoryId { get; set; }
        public int ZnodeCatalogId { get; set; }
        public int PublishedCatalogId { get; set; }        

        public decimal? RetailPrice { get; set; }
        public decimal? SalesPrice { get; set; }
        public decimal? Rating { get; set; }

        public string SKU { get; set; }
        public string ConfigurableProductSKU { get; set; }
        public string Name { get; set; }
        public string CurrencySuffix { get; set; }
        public string SeoDescription { get; set; }
        public string SeoKeywords { get; set; }
        public string SeoTitle { get; set; }
        public string SeoUrl { get; set; }
        public string ImageLargePath { get; set; }
        public string ImageMediumPath { get; set; }
        public string ImageThumbnailPath { get; set; }
        public string ImageSmallPath { get; set; }
        public string ImageSmallThumbnailPath { get; set; }
        public string OriginalImagePath { get; set; }
        public string AddOnProductSKUs { get; set; }
        public string BundleProductSKUs { get; set; }
        public string ConfigurableSKUs { get; set; }
        public string GroupProductSKUs { get; set; }

        public List<PublishAttributeModel> Attributes { get; set; }
        public List<PriceTierModel> TierPriceList { get; set; }
        public List<AssociatedProductsModel> AssociatedProducts { get; set; }
        public List<WebStoreAddOnModel> AddOns { get; set; }
    }
}
