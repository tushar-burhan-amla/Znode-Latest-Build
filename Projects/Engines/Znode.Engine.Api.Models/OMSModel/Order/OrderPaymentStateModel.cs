namespace Znode.Engine.Api.Models
{
    public class OrderPaymentStateModel : BaseModel
    {
        public int? OmsPaymentStateId { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
