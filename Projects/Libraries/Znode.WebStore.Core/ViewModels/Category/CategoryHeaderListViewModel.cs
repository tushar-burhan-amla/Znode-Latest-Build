using System.Collections.Generic;
namespace Znode.Engine.WebStore.ViewModels
{
    public class CategoryHeaderListViewModel : BaseViewModel
    {
        public CategoryHeaderListViewModel()
        {
            Categories = new List<CategoryHeaderViewModel>();
        }

        public List<CategoryHeaderViewModel> Categories { set; get; }
        public List<BrandViewModel> BrandList { get; set; }
    }
}