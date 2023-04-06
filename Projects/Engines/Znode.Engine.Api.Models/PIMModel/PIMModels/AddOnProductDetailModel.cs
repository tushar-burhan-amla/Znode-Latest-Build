namespace Znode.Engine.Api.Models
{
    public class AddOnProductDetailModel : BaseModel
    {
        public int PimAddOnProductDetailId { get; set; }
        public int PimAddOnProductId { get; set; }
        public int PimChildProductId { get; set; }
        public int DisplayOrder { get; set; }
        public bool? IsDefault { get; set; }
        public int PimProductId { get; set; }
    }
}
