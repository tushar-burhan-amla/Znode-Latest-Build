using Newtonsoft.Json;
using System.Collections.Generic;

namespace Znode.Engine.Api.Models.V2
{
    public class CategoryProductModelV2 : BaseModel
    {
        public CategoryProductModelV2()
        {
            AssociatedProducts = new List<AssociatedProductsModel>();
            Attributes = new List<PublishAttributeModel>();
        }

        public int PublishProductId { get; set; }
        public int ZnodeCategoryId { get; set; }
        public int ZnodeCatalogId { get; set; }
        [JsonIgnore]
        public int PublishedCatalogId { get; set; }
        public int? TotalReviews { get; set; }

        public decimal? RetailPrice { get; set; }
        public decimal? SalesPrice { get; set; }

        public string SKU { get; set; }
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
        public string TemplateWidth { get; set; }

        public List<PublishAttributeModel> Attributes { get; set; }

        public List<AssociatedProductsModel> AssociatedProducts { get; set; }
    }
}
