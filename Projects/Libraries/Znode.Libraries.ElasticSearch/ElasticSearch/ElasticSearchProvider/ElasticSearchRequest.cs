using Nest;

using System;
using System.Collections.Generic;

using Znode.Libraries.Search;

namespace Znode.Libraries.ElasticSearch
{
    public class ElasticSearchRequest : IZnodeSearchRequest
    {
        public ElasticSearchRequest()
        {
            HighlightFieldName = "name";
        }

        public string SearchText { get; set; }
        public string Category { get; set; }
        public string SearchType { get; set; }
        public int PortalCatalogID { get; set; }

        public Dictionary<string, List<string>> Facets { get; set; }
        public int SortOrder { get; set; }
        public List<SortCriteria> SortCriteria { get; set; }
        public bool GetFacets { get; set; }
        // If true Returns externalIds instead of ProductId 
        public bool ExternalIdEnabled { get; set; }
        public bool UseSuggestion { get; set; }
        public bool GetCategoriesHierarchy { get; set; }

        public bool ExternalIdNotNull { get; set; }

        public List<string> InnerSearchKeywords { get; set; }

        public string Tags { get; set; }

        public int CategoryId { get; set; }

        public int CatalogId { get; set; }

        public int LocaleId { get; set; }

        public int PageSize { get; set; } = 10;
        public int PageFrom { get; set; } = 0;
        public int StartIndex { get; set; }

        public int ProfileId { get; set; }


        public List<string> SearchableFields { get; set; }
        public string[] FacetableFields { get; set; }
        public string HighlightFieldName { get; set; }
        public string PostTags { get; set; }
        public string PreTags { get; set; }
        public string IndexName { get; set; }
        public int NumberOfAggregationSize { get; set; }
        public string boostField { get; set; }

        public Dictionary<string, List<string>> FilterDictionaryInAndClause { get; set; }
        public Dictionary<string, List<string>> FilterDictionaryInOrClause { get; set; }

        public List<Tuple<string, List<string>, double>> SearchableFieldsDictionary { get; set; }
        public Dictionary<string, List<string>> CatalogIdLocalIdDictionary { get; set; }

        public bool IsBrandSearch { get; set; }
        public bool IsAllowIndexing { get; set; }

        public int SuggestionTermCount { get; set; }
        public int SearchProfileId { get; set; }
        //
        public Dictionary<string, double> BoostingFields { get; set; }

        public string QueryTypeName { get; set; }
        public string SubQueryType { get; set; }

        public string Operator { get; set; }
        public List<QueryContainer> FilterValues { get; set; }
        public List<ElasticSearchAttributes> SearchableAttribute { get; set; }

        public List<ElasticSearchAttributes> FacetableAttribute { get; set; }

        public List<SearchAttributes> AttributeList { get; set; }

        public List<ElasticSearchFeature> FeatureList { get; set; }
        public List<ElasticSearchBoostAndBuryItemList> BoostAndBuryItemLists { get; set; }
        public string QueryClass { get; set; }
        public List<KeyValuePair<string, int>> FieldValueFactors { get; set; }

        public int CMSSearchPageCount { get; set; }

        // Retrieve specific properties from Elastic Response.
        public string[] Source { get; set; }
    }
}
