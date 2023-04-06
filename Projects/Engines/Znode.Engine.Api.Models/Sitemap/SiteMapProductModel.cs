namespace Znode.Engine.Api.Models
{
    public class SiteMapProductModel : BaseModel
    {
        public string SEOUrl { get; set; }
        public int ZnodeProductId { get; set; }
        public string Name { get; set; }
        public string CategoryName { get; set; }
    }
}
