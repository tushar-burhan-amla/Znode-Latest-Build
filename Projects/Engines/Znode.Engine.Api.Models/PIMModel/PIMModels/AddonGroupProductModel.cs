namespace Znode.Engine.Api.Models
{
    public class AddonGroupProductModel : BaseModel
    {
        public int PimAddonGroupProductId { get; set; }
        public int PimAddonGroupId { get; set; }
        public int PimChildProductId { get; set; }
        public int DisplayOrder { get; set; }
    }
}
