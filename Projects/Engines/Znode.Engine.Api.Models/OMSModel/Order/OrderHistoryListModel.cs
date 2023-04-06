using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class OrderHistoryListModel:BaseListModel
    {
        public List<OrderHistoryModel> OrderHistoryList { get; set; }

        public OrderHistoryListModel()
        {
            OrderHistoryList = new List<OrderHistoryModel>();
        }
    }
}
