using Nest;

using System;
using System.Collections.Generic;
using System.Linq;

using Znode.Libraries.Search;

namespace Znode.Libraries.ElasticSearch
{
    // Implementation to get type ahead feature.   
    public class ElasticSuggestionsService : ElasticSearchBaseService, IElasticSuggestionsService
    {
        #region Constructor

        public ElasticSuggestionsService()
        {

        }

        #endregion Constructor

        #region Public Methods

        public virtual IZnodeSearchResponse SuggestTermsFromExistingIndex(IZnodeSearchRequest request)
        {
            request.PageSize = 10;
            request.StartIndex = 0;
            var elasticSearchClient = ElasticConnectionHelper.ElasticSearchClient;
            List<QueryContainer> catalogIdLocaleIdContainerList = GetDefaultFilters(request);

            //Checks if the index is present or not.
            if (string.IsNullOrEmpty(request.IndexName))
                throw new Exception("Index name cannot be blank.");
            if (!elasticSearchClient.Indices.Exists(request.IndexName).Exists)
                throw new Exception("Search index does not exist.");

            SetDefaultFilters(request);

            ISearchResponse<dynamic> suggestions = GetAutoSuggestions(request, catalogIdLocaleIdContainerList);
            // Get Suggestion by suggested term if no document is returned
            if (suggestions.Documents.Count == 0 && suggestions.Suggest?.Values?.FirstOrDefault()?.FirstOrDefault().Options.Count > 0)
                suggestions = GetSuggestionBySuggestedTerm(request, catalogIdLocaleIdContainerList, suggestions);

            return ElasticProductMapper.MapSuggestionResponse(suggestions, request.HighlightFieldName, request);
        }

        //Get suggestions for search index.
        public virtual ISearchResponse<dynamic> GetAutoSuggestions(IZnodeSearchRequest request, List<QueryContainer> catalogIdLocaleIdContainerList)
        {
            var suggestions = GetSearchResponse(request);

            return suggestions;
        }

        #endregion Public Methods

        #region Protected Methods

        //Gets default filters for catalog, locale ID, etc.
        protected virtual List<QueryContainer> GetDefaultFilters(IZnodeSearchRequest request)
        {
            List<QueryContainer> catalogIdLocaleIdContainerList = new List<QueryContainer>();
            // Loop to add catalogId, localId and categoryId 
            foreach (var andItem in request.CatalogIdLocalIdDictionary ?? new Dictionary<string, List<string>>())
            {
                foreach (var item in andItem.Value)
                    // Added item to list to create "AND" query.
                    catalogIdLocaleIdContainerList.Add(GetTermQuery(andItem.Key.ToString(), item));
            }

            return catalogIdLocaleIdContainerList;
        }

        //Gets the term query.
        protected virtual QueryContainer GetTermQuery(string andItem, string itemValue, double boost = 0.0, bool isSortEnabled = false)
        {
            boost = isSortEnabled ? boost : 0.0;
            return new QueryContainerDescriptor<SearchProduct>().Term(s => s.Field(andItem).Value(itemValue.ToLower()).Boost(boost));
        }

        //Get auto suggestion by suggest term.
        protected virtual ISearchResponse<dynamic> GetSuggestionBySuggestedTerm(IZnodeSearchRequest request, List<QueryContainer> catalogIdLocaleIdContainerList, ISearchResponse<dynamic> suggestions)
        {
            string suggestedTerm = suggestions.Suggest?.Values?.FirstOrDefault()?.FirstOrDefault().Options.OrderByDescending(m => m.Score)?.FirstOrDefault().Text;

            request.SearchText = suggestedTerm;

            suggestions = GetAutoSuggestions(request, catalogIdLocaleIdContainerList);

            return suggestions;
        }

        #endregion
    }
}