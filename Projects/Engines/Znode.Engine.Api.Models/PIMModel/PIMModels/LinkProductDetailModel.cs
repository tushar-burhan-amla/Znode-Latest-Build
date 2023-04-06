namespace Znode.Engine.Api.Models
{
    public class LinkProductDetailModel : BaseModel
    {
        public int PimLinkProductDetailId { get; set; }
        public int? PimParentProductId { get; set; }
        public int? PimProductId { get; set; }
        public int PimAttributeId { get; set; }
        public int? DisplayOrder { get; set; }
        public virtual ProductModel ZnodePimProduct { get; set; }
    }
}
