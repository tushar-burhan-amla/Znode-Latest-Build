using System.Collections.Generic;

namespace Znode.Engine.Admin.ViewModels
{
    public class LinkProductDetailListViewModel : BaseViewModel
    {
        public List<LinkProductDetailViewModel> LinkProducts { get; set; }

        public LinkProductDetailListViewModel()
        {
            LinkProducts = new List<LinkProductDetailViewModel>();
        }
    }
}