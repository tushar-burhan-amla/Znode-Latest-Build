using System.Collections.Generic;

namespace Znode.Engine.WebStore.ViewModels
{
    public class CategoryViewModel : BaseViewModel
    {
        public int CategoryId { get; set; }
        public string CategoryName { get; set; }

        public SEOViewModel SEODetails { get; set; }
        public List<CategoryViewModel> Subcategories { get; set; }
        public CategoryViewModel ParentCategories { get; set; }
        public List<ProductViewModel> ProductList { get; set; }
        public List<FacetViewModel> Facets { get; set; }
    
        public List<AttributesViewModel> Attributes { get; set; }

        public string ImageLargePath { get; set; }
        public string ImageMediumPath { get; set; }
        public string ImageThumbNailPath { get; set; }
        public string ImageSmallPath { get; set; }

        public List<CategoryViewModel> ParentCategory { get; set; }

        public ProductListViewModel ProductListViewModel { get; set; }
        public string BreadCrumbHtml { get; set; }

        public CategoryViewModel()
        {
            Facets = new List<FacetViewModel>();
        }
    }
}

