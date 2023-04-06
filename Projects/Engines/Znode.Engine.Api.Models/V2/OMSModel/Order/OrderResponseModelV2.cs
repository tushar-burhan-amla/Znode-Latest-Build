namespace Znode.Engine.Api.Models.V2
{
    public class OrderResponseModelV2 : BaseModel
    {
        public string OrderNumber { get; set; }

        public int OmsOrderId { get; set; }

        public string UserEmailId { get; set; }
    }
}
