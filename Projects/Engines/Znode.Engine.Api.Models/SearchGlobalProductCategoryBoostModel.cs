namespace Znode.Engine.Api.Models
{
    public class SearchGlobalProductCategoryBoostModel : BaseModel
    {
        public int SearchGlobalProductCategoryBoostId { get; set; }
        public int PublishCatalogId { get; set; }
        public int PublishCategoryProductId { get; set; }
        public int PublishCategoryId { get; set; }
        public int PublishProductId { get; set; }
        public string PublishCategoryName { get; set; }
        public string ProductName { get; set; }
        public decimal Boost { get; set; }
        public string SKU { get; set; }
    }
}
