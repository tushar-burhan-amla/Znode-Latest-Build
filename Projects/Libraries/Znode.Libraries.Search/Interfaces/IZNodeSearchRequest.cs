using Nest;

using System.Collections.Generic;

namespace Znode.Libraries.Search
{
    public interface IZnodeSearchRequest
    {
        int CatalogId { get; set; }

        int PortalCatalogID { get; set; }

        string SearchText { get; set; }
        int CategoryId { get; set; }
        string Category { get; set; }
        string SearchType { get; set; }

        // If true , returns External Ids instead of product Id 
        bool ExternalIdEnabled { get; set; }
        // if true , returns only documents that has external Id populated
        bool ExternalIdNotNull { get; set; }
        bool GetFacets { get; set; }
        bool GetCategoriesHierarchy { get; set; }
        bool UseSuggestion { get; set; }

        List<string> InnerSearchKeywords { get; set; }
        Dictionary<string, List<string>> Facets { get; set; }
        List<SortCriteria> SortCriteria { get; set; }

        int LocaleId { get; set; }
        int PageFrom { get; set; }
        int PageSize { get; set; }
        int StartIndex { get; set; }

        string[] FacetableFields { get; set; }
        string HighlightFieldName { get; set; }
        string PostTags { get; set; }
        string PreTags { get; set; }
        string IndexName { get; set; }
        int NumberOfAggregationSize { get; set; }
        string boostField { get; set; }

        bool IsBrandSearch { get; set; }
        bool IsAllowIndexing { get; set; }
        Dictionary<string, List<string>> CatalogIdLocalIdDictionary { get; set; }

        int SuggestionTermCount { get; set; }
        int SearchProfileId { get; set; }

        //
        Dictionary<string, double> BoostingFields { get; set; }

        string QueryClass { get; set; }
        string QueryTypeName { get; set; }
        string SubQueryType { get; set; }
        string Operator { get; set; }

        List<QueryContainer> FilterValues { get; set; }

        List<ElasticSearchAttributes> SearchableAttribute { get; set; }

        List<ElasticSearchAttributes> FacetableAttribute { get; set; }

        List<SearchAttributes> AttributeList { get; set; }

        List<ElasticSearchFeature> FeatureList { get; set; }

        List<ElasticSearchBoostAndBuryItemList> BoostAndBuryItemLists { get; set; }

        List<KeyValuePair<string, int>> FieldValueFactors { get; set; }

        int CMSSearchPageCount { get; set; }

        // Retrieves required field in search response
        string[] Source { get; set; }
    }
}
