namespace Znode.Engine.Api.Models.Responses
{
    public class OrderResponse : BaseResponse
    {
        public OrderModel Order { get; set; }
        public OrderItemsRefundModel RefundPayment { get; set; }
    }
}
