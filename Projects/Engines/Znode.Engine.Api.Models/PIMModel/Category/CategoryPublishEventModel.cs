namespace Znode.Engine.Api.Models
{
    public class CategoryPublishEventModel : BaseModel
    {
        public int PortalId { get; set; }
        public int CatalogId { get; set; }
        public int CategoryId { get; set; }
        public string SeoUrl { get; set; }
    }
}
