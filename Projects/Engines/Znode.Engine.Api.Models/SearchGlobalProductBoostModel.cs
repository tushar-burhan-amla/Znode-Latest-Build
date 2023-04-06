namespace Znode.Engine.Api.Models
{
    public class SearchGlobalProductBoostModel : BaseModel
    {
        public int SearchGlobalProductBoostId { get; set; }
        public int PublishCatalogId { get; set; }
        public int PublishProductId { get; set; }
        public string ProductName { get; set; }
        public decimal Boost { get; set; }
        public string SKU{ get; set; }
    }
}
