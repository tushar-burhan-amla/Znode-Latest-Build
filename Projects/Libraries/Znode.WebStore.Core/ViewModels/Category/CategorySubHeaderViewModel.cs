using System.Collections.Generic;

namespace Znode.Engine.WebStore.ViewModels
{
    /// <summary>
    /// This is the model for CategorySubHeaderViewModel to show the Sub Menus
    /// </summary>
    public class CategorySubHeaderViewModel : BaseViewModel
    {
        public CategorySubHeaderViewModel()
        {
            ChildCategoryItems = new List<CategorySubHeaderViewModel>();
            ParentCategories = new List<CategoryViewModel>();
        }

        public string CategoryName { set; get; }
        public int CategoryId { set; get; }
        public string SEOPageName { set; get; }
        public List<CategorySubHeaderViewModel> ChildCategoryItems { get; set; }
        public List<CategoryViewModel> ParentCategories { get; set; }
        public List<AttributesViewModel> Attributes { get; set; }

        // Unique Identifier for category.
        public string CategoryCode { get; set; }
    }
}