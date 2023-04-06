namespace Znode.Engine.Api.Models
{
    public class PaymentTypeModel
    {
        public int PaymentTypeId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Code { get; set; }
        public string BehaviorType { get; set; }
        public bool IsActive { get; set; }
    }
}
