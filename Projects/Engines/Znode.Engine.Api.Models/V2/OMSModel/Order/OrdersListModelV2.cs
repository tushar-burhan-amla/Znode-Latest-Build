using System.Collections.Generic;

namespace Znode.Engine.Api.Models.V2
{
    public class OrdersListModelV2 : BaseListModel
    {
        public List<OrderModelV2> Orders { get; set; }
    }
}
