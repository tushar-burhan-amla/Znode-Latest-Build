namespace Znode.Engine.Api.Models
{
    public class ProductTypeAssociationModel : BaseModel
    {
        public int PimProductTypeAssociationId { get; set; }
        public int? PimParentProductId { get; set; }
        public int? PimProductId { get; set; }
        public int? PimAttributeId { get; set; }
        public int? DisplayOrder { get; set; } = 99;
        public bool IsDefault { get; set; }
        public decimal? BundleQuantity { get; set; } = 1;
    }
}
