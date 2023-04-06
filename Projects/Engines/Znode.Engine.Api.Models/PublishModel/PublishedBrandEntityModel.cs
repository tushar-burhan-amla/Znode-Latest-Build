namespace Znode.Engine.Api.Models
{
    public class PublishedBrandEntityModel : BaseModel
    {
        public int VersionId { get; set; }
        public int BrandId { get; set; }
        public string BrandCode { get; set; }
        public string BrandName { get; set; }
    }
}
