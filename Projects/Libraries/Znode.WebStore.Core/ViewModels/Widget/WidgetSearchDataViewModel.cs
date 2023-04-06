using System;
using System.Collections.Generic;

namespace Znode.Engine.WebStore.ViewModels
{
    public class WidgetSearchDataViewModel : BaseViewModel
    {
        public List<FacetViewModel> Facets { get; set; }
        public List<Tuple<string, List<KeyValuePair<string, string>>>> FacetFilters { get; set; }
        public List<ProductViewModel> Products { get; set; }
        public string SearchResultCountText { get; set; }
    }
}
