using System.Collections.Generic;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.ViewModels
{
    public class CategoryProductsListViewModel : BaseViewModel
    {
        public CategoryProductsListViewModel()
        {
            CategoryProducts = new List<CategoryProductViewModel>();
        }
        public List<CategoryProductViewModel> CategoryProducts { get; set; }
        public GridModel GridModel { get; set; }
        public int PimCategoryId { get; set; }

        public int PimProductId { get; set; }
    }
}