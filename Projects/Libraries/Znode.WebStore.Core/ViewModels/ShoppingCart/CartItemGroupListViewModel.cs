using System.Collections.Generic;

namespace Znode.Engine.WebStore.ViewModels
{
    public class CartItemGroupListViewModel :BaseViewModel
    {
        public string SKU { get; set; }

        public string GroupId { get; set; }

        public int Sequence { get; set; }
        public List<CartItemViewModel> Children { get; set; }
    }
}
