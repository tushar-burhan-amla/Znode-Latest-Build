using Nest;

using System.Collections.Generic;
using System.Linq;

using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Search;

namespace Znode.Libraries.ElasticSearch
{
    public class MatchQueryBuilder : BaseQuery, ISearchQuery
    {
        #region Protected Variables
        protected readonly FunctionScoreQuery functionScoreQuery = new FunctionScoreQuery();
        protected readonly BoolQuery finalBoolQuery = new BoolQuery();
        #endregion

        #region Public Methods
        public virtual SearchRequest<dynamic> GenerateQuery(IZnodeSearchRequest request)
        {

            //Build Query To Get Partial Match Result after the exact match.
            MatchQuery partialMatchQuery = BuildMatchQuery(request);

            //Query with search text.
            finalBoolQuery.Must = new List<QueryContainer> { partialMatchQuery };

            CheckAndAddBoostQueryForExactMatchResult(request);

            //set conditions for boost or bury the products.
            BoolQuery boostAndBuryCondition = GetBoostOrBuryItem(request);

            finalBoolQuery.Should = new List<QueryContainer> { boostAndBuryCondition };

            //Filter for the search like version id,catalog id etc.
            finalBoolQuery.Filter = request.FilterValues;

            functionScoreQuery.ScoreMode = FunctionScoreMode.Sum;
            functionScoreQuery.BoostMode = FunctionBoostMode.Sum;
            functionScoreQuery.Functions = AddFunctionToSearchQuery(request);
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

        #region Protected Methods
        protected virtual MatchQuery BuildMatchQuery(IZnodeSearchRequest request)
        {
            MatchQuery matchQuery = new MatchQuery();

            string searchField = "";

            if (request.SearchableAttribute?.Count > 0)
            {
                searchField = request.SearchableAttribute?.FirstOrDefault().AttributeCode.ToLower();
            }
            else
            {
                searchField = "productname";
            }

            matchQuery.Query = request.SearchText;

            matchQuery.Field = searchField;

            if (!string.IsNullOrEmpty(GetFeatureValue(request, "MinimumShouldMatch")))
                matchQuery.MinimumShouldMatch = GetFeatureValue(request, "MinimumShouldMatch");

            //Add fuzziness query if feature is active
            AddFuzzinessQuery(matchQuery, request);

            matchQuery.Operator = GetOperator(request);

            return matchQuery;
        }
        #endregion
    }
}
