using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class SearchRequestModel : BaseModel
    {
        // Request
        public string Category { get; set; }
        public int CategoryId { get; set; }
        public string Keyword { get; set; }
        public List<string> InnerSearchKeywords { get; set; }
        public string FacetValue { get; set; }
        public string SearchTerm { get; set; }
        public string FacetGroup { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        public Dictionary<string, List<string>> RefineBy { get; set; }
        public int Sort { get; set; }
        public int LocaleId { get; set; }
        public int CatalogId { get; set; }
        public int? ProfileId { get; set; }
        public bool IsRemove { get; set; }
        public bool IsRemoveAll { get; set; }
        public int PortalId { get; set; }
        public Dictionary<string, List<string>> FilterList { get; set; }
        public bool IsBrandSearch { get; set; }
        public int BrandId { get; set; }
        public string BrandName { get; set; }
        public bool IsFacetList { get; set; }
        public bool IsSearchWidget { get; set; }
        public bool IsAutocomplete { get; set; }
        public bool UseSuggestion { get; set; } = true;
        public bool ExternalIdEnabled { get; set; }
        public bool ExternalIdNotNullCheck { get; set; }
        public string[] FacetableFields { get; set; }
        public int NumberOfAggregationSize { get; set; }
        public List<int> ParentCategoryIds { get; set; }
        public bool IsProductInheritanceEnabled { get; set; }
    }
}
