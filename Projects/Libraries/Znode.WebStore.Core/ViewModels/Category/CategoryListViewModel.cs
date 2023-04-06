using System.Collections.Generic;

namespace Znode.Engine.WebStore.ViewModels
{
    public class CategoryListViewModel:BaseViewModel
    {
        public List<CategoryViewModel> Categories { get; set; }

        public CategoryListViewModel()
        {
            Categories = new List<CategoryViewModel>();
        }

    }
}