using System.Collections.Generic;

namespace Znode.Engine.WebStore.ViewModels
{
    public class SearchRequestViewModel : BaseViewModel
    {
        // Request
        public string Category { get; set; }
        public int CategoryId { get; set; }
        public string InnerSearchKeywords { get; set; }
        public string FacetValue { get; set; }
        public string SearchTerm { get; set; }
        public string FacetGroup { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        public List<KeyValuePair<string, IEnumerable<string>>> RefineBy { get; set; }
        public int Sort { get; set; }
        public int LocaleId { get; set; }
        public int CatalogId { get; set; }
        public int? ProfileId { get; set; }
        public bool IsRemove { get; set; }
        public bool IsRemoveAll { get; set; }
        public int PortalId { get; set; }
        public Dictionary<string,List<string>> FilterList { get; set; }
        
        public bool IsBrandSearch { get; set; }
        public int BrandId { get; set; }
        public int PageId { get; set; }
        public string BrandName { get; set; }
        public bool IsFacetList { get; set; }

        public Dictionary<string, string> FilterDictionaryInAndClause { get; set; }
        public Dictionary<string, string> FilterDictionaryInOrClause { get; set; }
        public ProductListViewModel ProductListViewModel { get; set; }

        public bool UseSuggestion { get; set; } = true;
        public bool IsProductInheritanceEnabled { get; set; }
    }
}