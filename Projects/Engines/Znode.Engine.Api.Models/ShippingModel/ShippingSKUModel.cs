namespace Znode.Engine.Api.Models
{
    public class ShippingSKUModel : BaseModel
    {
        public int ShippingSKUId { get; set; }
        public int? ShippingRuleId { get; set; }
        public string SKU { get; set; }
        public int? ShippingTypeId { get; set; }
        public string ClassName { get; set; }
        public string SKUs { get; set; }
        public string ProductName { get; set; }
        public int ShippingId { get; set; }
    }
}
