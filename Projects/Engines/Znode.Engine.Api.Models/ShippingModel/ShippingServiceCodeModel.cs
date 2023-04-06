namespace Znode.Engine.Api.Models
{
    public class ShippingServiceCodeModel : BaseModel
    {
        public int ShippingServiceCodeId { get; set; }
        public int ShippingTypeId { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public int? DisplayOrder { get; set; }
        public bool IsActive { get; set; }
    }
}
