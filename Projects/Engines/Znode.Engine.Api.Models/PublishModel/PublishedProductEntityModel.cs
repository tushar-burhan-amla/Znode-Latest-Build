using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class PublishedProductEntityModel : BaseModel
    {
        public int PublishProductEntityId { get; set; }
        public int VersionId { get; set; }
        public string IndexId { get; set; }
        public int ZnodeProductId { get; set; }
        public int ZnodeCatalogId { get; set; }
        public string SKU { get; set; }
        public int LocaleId { get; set; }
        public string Name { get; set; }
        public int ZnodeCategoryIds { get; set; }
        public bool IsActive { get; set; }
        public List<PublishedAttributeEntityModel> Attributes { get; set; }
        public List<PublishedBrandEntityModel> Brands { get; set; }
        public string CategoryName { get; set; }
        public string CatalogName { get; set; }
        public int DisplayOrder { get; set; }
        public string RevisionType { get; set; }
        public int AssociatedProductDisplayOrder { get; set; }
        public int ProductIndex { get; set; }
        public string SalesPrice { get; set; }
        public string RetailPrice { get; set; }
        public string CultureCode { get; set; }
        public string CurrencySuffix { get; set; }
        public string CurrencyCode { get; set; }
        public string SeoDescription { get; set; }
        public string SeoKeywords { get; set; }
        public string SeoTitle { get; set; }
        public string SeoUrl { get; set; }
        public string ImageSmallPath { get; set; }
        public string SKULower { get; set; }

        public PublishedProductEntityModel()
        {
            Attributes = new List<PublishedAttributeEntityModel>();
            Brands = new List<PublishedBrandEntityModel>();
        }
    }
}
