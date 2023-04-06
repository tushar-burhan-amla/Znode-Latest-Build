using System.Collections.Generic;

namespace Znode.Engine.Api.Models.Responses
{
    public class OrderStateListResponse : BaseListResponse
    {
        public List<OrderStateModel> OrderStates { get; set; }
    }
}
