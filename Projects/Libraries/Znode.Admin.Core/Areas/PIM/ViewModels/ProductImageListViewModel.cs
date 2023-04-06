using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class ProductImageListViewModel : BaseViewModel
    {
        public List<ProductImageViewModel> ProductImages { get; set; }

        public GridModel GridModel { get; set; }

        public ProductImageListViewModel()
        {
            ProductImages = new List<ProductImageViewModel>();
        }
    }
}