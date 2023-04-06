using System.Collections.Generic;

namespace Znode.Engine.WebStore.ViewModels
{
    public class ReturnItemGroupListViewModel : BaseViewModel
    {
        public string SKU { get; set; }

        public string GroupId { get; set; }

        public int Sequence { get; set; }
        public List<RMAReturnCartItemViewModel> Children { get; set; }
    }
}
