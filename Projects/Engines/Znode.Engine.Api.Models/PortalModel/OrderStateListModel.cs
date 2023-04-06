using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class OrderStateListModel : BaseListModel
    {
        public List<OrderStateModel> OrderStates { get; set; }

        public OrderStateListModel()
        {
            OrderStates = new List<OrderStateModel>();
        }
    }
}
