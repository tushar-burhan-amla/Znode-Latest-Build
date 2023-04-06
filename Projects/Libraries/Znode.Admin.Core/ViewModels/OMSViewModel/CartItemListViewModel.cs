using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class CartItemListViewModel : BaseViewModel
    {
        public List<CartItemViewModel> CartItemList { get; set; }
        public GridModel GridModel { get; set; }
        public OrderTotalViewModel OrderTotal { get; set; }

        public CartItemListViewModel()
        {
            CartItemList = new List<CartItemViewModel>();
            GridModel = new GridModel();
            OrderTotal = new OrderTotalViewModel();
        }
    }
}
