namespace Znode.Engine.Api.Models
{
   public class ProductAssociationPublishModel : BaseModel
    {
        public int PublishCatalogId { get; set; }
        public int PublishCategoryId { get; set; }
        public int PublishProductId { get; set; }
        public int LocaleId { get; set; }
        public int VersionId { get; set; }
        public string SKU { get; set; }
        public string SEOUrl { get; set; }
    }
}
