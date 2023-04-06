using Znode.Engine.Api.Models.V2;

namespace Znode.Engine.Api.Models.Responses.V2
{
    public class OrderListResponseV2 : BaseListResponse
    {
        public OrdersListModelV2 OrderList { get; set; }        
    }
}
