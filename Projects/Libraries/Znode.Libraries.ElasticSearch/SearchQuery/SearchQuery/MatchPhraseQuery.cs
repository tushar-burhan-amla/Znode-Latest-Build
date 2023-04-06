using Nest;

using System.Collections.Generic;
using System.Linq;

using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Search;

namespace Znode.Libraries.ElasticSearch
{
    public class MatchPhraseQueryBuilder : BaseQuery, ISearchQuery
    {
        #region Protected Variables
        protected readonly FunctionScoreQuery functionScoreQuery = new FunctionScoreQuery();
        protected readonly BoolQuery finalBoolQuery = new BoolQuery();
        #endregion

        #region Public Methods
        //Generate query.
        public virtual SearchRequest<dynamic> GenerateQuery(IZnodeSearchRequest request)
        {
            //Match phrase query for partial match after the exact match.
            MatchPhraseQuery partialMatchPhraseQuery = BuildMatchPhraseQuery(request);

            functionScoreQuery.ScoreMode = FunctionScoreMode.Sum;
            functionScoreQuery.BoostMode = FunctionBoostMode.Sum;
            functionScoreQuery.Functions = AddFunctionToSearchQuery(request);

            finalBoolQuery.Must = new List<QueryContainer> { partialMatchPhraseQuery };

            //If boost and bury conditions are not present then add condition for exact Match first.
            CheckAndAddBoostQueryForExactMatchResult(request);

            //set conditions for boost or bury the products.
            BoolQuery boostAndBuryCondition = GetBoostOrBuryItem(request);

            finalBoolQuery.Should = new List<QueryContainer> { boostAndBuryCondition };

            finalBoolQuery.Filter = request.FilterValues;

            functionScoreQuery.Query = finalBoolQuery;
            AggregationBase aggregationBase = null;

            if (request.GetCategoriesHierarchy)
                aggregationBase = GetCategoryAggregation();

            if (request.GetFacets && request.FacetableAttribute?.Count > 0)
                aggregationBase = GetFacetAggregation(aggregationBase, request.FacetableAttribute);

            List<ISort> SortSettings = AddSortToSearchQuery(request);

            SearchRequest<dynamic> searchRequest = new SearchRequest<dynamic>(request.IndexName);
            searchRequest.Query = functionScoreQuery;
            searchRequest.From = request.StartIndex;
            searchRequest.Size = request.PageSize;
            if (SortSettings.Count > 0)
                searchRequest.Sort = SortSettings;

            if (HelperUtility.IsNotNull(aggregationBase))
                searchRequest.Aggregations = aggregationBase;
           
            searchRequest.Suggest = GetSuggestionQuery(request);

            if (IsFeatureActive(request, "DfsQueryThenFetch"))
            {
                searchRequest.SearchType = Elasticsearch.Net.SearchType.DfsQueryThenFetch;
            }

            return searchRequest;

        }
        #endregion

        #region Protected Method
        //Build Match Phrase Query
        protected virtual MatchPhraseQuery BuildMatchPhraseQuery(IZnodeSearchRequest request)
        {
            MatchPhraseQuery matchPhraseQuery = new MatchPhraseQuery();

            string searchField = "";

            if (request.SearchableAttribute?.Count > 0)
            {
                searchField = request.SearchableAttribute?.FirstOrDefault().AttributeCode.ToLower();
            }
            else
            {
                searchField = "productname";
            }

            matchPhraseQuery.Query = request.SearchText;
            matchPhraseQuery.Field = searchField;

            return matchPhraseQuery;
        }
        #endregion
    }
}
