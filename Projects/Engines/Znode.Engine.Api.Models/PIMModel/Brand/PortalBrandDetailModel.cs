 namespace Znode.Engine.Api.Models
{
   public class PortalBrandDetailModel : BaseModel
    {
        public int PortalBrandId { get; set; }
        public int PortalId { get; set; }
        public int BrandId { get; set; }
        public int DisplayOrder { get; set; }
    }
}
