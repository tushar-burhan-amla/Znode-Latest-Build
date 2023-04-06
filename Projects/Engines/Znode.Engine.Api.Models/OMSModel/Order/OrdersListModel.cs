using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class OrdersListModel : BaseListModel
    {
        public List<OrderModel> Orders { get; set; }
        public string CustomerName { get; set; }
        public bool HasParentAccounts { get; set; }

        public OrdersListModel()
        {
            Orders = new List<OrderModel>();
        }
    }
}
