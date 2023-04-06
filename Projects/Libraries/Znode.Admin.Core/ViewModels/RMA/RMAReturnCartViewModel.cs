using System.Collections.Generic;

namespace Znode.Engine.Admin.ViewModels
{
    public class RMAReturnCartViewModel : BaseViewModel
    {
        public List<RMAReturnCartItemViewModel> ShoppingCartItems { get; set; }
        public string ImagePath { get; set; }
        public int OmsOrderId { get; set; }
    }
}
