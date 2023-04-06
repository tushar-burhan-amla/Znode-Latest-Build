using System.Collections.Generic;

namespace Znode.Engine.WebStore.ViewModels
{
    public class LinkProductViewModel : BaseViewModel
    {
        public string AttributeName { get; set; }
        public List<ProductViewModel> PublishProduct { get; set; }

        public LinkProductViewModel()
        {
            PublishProduct = new List<ProductViewModel>();
        }
    }
}