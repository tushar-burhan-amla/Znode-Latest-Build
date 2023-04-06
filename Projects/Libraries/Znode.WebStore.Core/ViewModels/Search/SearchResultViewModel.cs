using System;
using System.Collections.Generic;

namespace Znode.Engine.WebStore.ViewModels
{
    public class SearchResultViewModel : BaseViewModel
    {
        public List<FacetViewModel> Facets { get; set; }
        public List<SearchCategoryViewModel> Categories { get; set; }
        public List<Tuple<string, List<KeyValuePair<string,string>>>> FacetFilters { get; set; }
        public int CategoryId { get; set; }
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public string SearchTerm { get; set; }
        public string Location { get; set; }

        // Associated categories of the search response products.
        public List<CategoryHeaderViewModel> AssociatedCategoryList { get; set; }
    }
}