namespace Znode.Engine.Api.Models
{
    public class RecentViewProductModel : BrandModel
    {
        public int ZnodeProductId { get; set; }
        public int ZnodeCatalogId { get; set; }
        public int ZnodeCategoryIds { get; set; }
    }
}
