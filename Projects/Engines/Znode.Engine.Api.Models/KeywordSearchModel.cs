using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class KeywordSearchModel : BaseModel
    {
        public int CategoryId { get; set; }
        public string Category { get; set; }
        public string Keyword { get; set; }
        public bool ExternalIdEnabled { get; set; }
        public bool ExternalIdNotNullCheck { get; set; }
        public List<string> InnerSearchKeywords { get; set; }
        public Dictionary<string, List<string>> RefineBy { get; set; }

        public List<SearchFacetModel> Facets { get; set; }
        public List<SearchProductModel> Products { get; set; }
        public List<SearchCategoryModel> Categories { get; set; }
        public int CatalogId { get; set; }
        public int LocaleId { get; set; }
        public int? ProfileId { get; set; }
        public int? PageNumber { get; set; }
        public int? PageSize { get; set; }
        public List<string> HighlightList { get; set; }
        public string[] SearchableFields { get; set; }
        public string[] FacetableFields { get; set; }
        public string HighlightFieldName { get; set; }
        public string PostTags { get; set; }
        public string PreTags { get; set; }
        public string IndexName { get; set; }
        public int NumberOfAggregationSize { get; set; }
        public int PortalId { get; set; }

        public bool IsBrandSearch { get; set; }
        public bool IsFacetList { get; set; }
        public string SuggestTerm { get; set; }
        public bool IsSearchFromSuggestion { get; set; }

        public long TotalProductCount { get; set; }

        public Dictionary<string, List<string>> FilterList { get; set; }

        public Dictionary<string, List<string>> CatalogIdLocalIdDictionary { get; set; }

        public bool UseSuggestion { get; set; }

        public string Location { get; set; }

        public int SearchProfileId { get; set; }

        public int TotalCMSPageCount { get; set; }

        // Retrieve associated category Ids.
        public List<int> AssociatedCategoryIds { get; set; }
    }
}
