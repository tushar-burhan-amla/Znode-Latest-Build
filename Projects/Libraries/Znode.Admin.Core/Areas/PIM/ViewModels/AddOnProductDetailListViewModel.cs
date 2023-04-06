using System.Collections.Generic;

namespace Znode.Engine.Admin.ViewModels
{
    public class AddOnProductDetailListViewModel : BaseViewModel
    {
        public List<AddOnProductDetailViewModel> AddOnProductDetails { get; set; }

        public AddOnProductDetailListViewModel()
        {
            AddOnProductDetails = new List<AddOnProductDetailViewModel>();
        }
    }
}