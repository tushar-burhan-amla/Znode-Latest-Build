namespace Znode.Engine.Api.Models
{
    public class WebStoreProductModel : PublishProductModel
    {
        public int CatalogId { get; set; }
        public int? DisplyOrder { get; set; }
    }
}
