using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class OrderHistoryListViewModel : BaseViewModel
    {
        public OrderHistoryListViewModel()
        {
            List = new List<OrderHistoryViewModel>();
            GridModel = new GridModel();
        }

        public List<OrderHistoryViewModel> List { get; set; }
        public GridModel GridModel { get; set; }
    }
}
