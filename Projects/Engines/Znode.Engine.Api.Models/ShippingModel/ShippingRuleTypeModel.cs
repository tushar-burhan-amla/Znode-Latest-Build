namespace Znode.Engine.Api.Models
{
    public class ShippingRuleTypeModel : BaseModel
    {
        public int ShippingRuleTypeId { get; set; }
        public string ClassName { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
    }
}
