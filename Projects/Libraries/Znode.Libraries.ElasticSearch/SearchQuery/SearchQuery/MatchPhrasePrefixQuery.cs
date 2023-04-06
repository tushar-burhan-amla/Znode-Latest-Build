using Nest;

using System.Collections.Generic;
using System.Linq;

using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Search;

namespace Znode.Libraries.ElasticSearch
{
    public class MatchPhrasePrefixQueryBuilder : BaseQuery, ISearchQuery
    {
        #region Protected Fields
        protected readonly FunctionScoreQuery functionScoreQuery = new FunctionScoreQuery();
        protected readonly BoolQuery finalBoolQuery = new BoolQuery();
        #endregion

        #region Public Methods

        //Generate query.
        public virtual SearchRequest<dynamic> GenerateQuery(IZnodeSearchRequest request)
        {

            //Match phrase query for partial match after exact match.
            MatchPhrasePrefixQuery partialMatchPhrasePrefixQuery = BuildMatchPhrasePrefixQuery(request);

            finalBoolQuery.Must = new List<QueryContainer> { partialMatchPhrasePrefixQuery };

            //If boost and bury conditions are not present then add condition for exact match first.
            CheckAndAddBoostQueryForExactMatchResult(request);

            //set conditions for boost or bury the products.
            BoolQuery boostAndBuryCondition = GetBoostOrBuryItem(request);

            finalBoolQuery.Should = new List<QueryContainer> { boostAndBuryCondition };

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

        //Build match phrase prefix query.
        protected virtual MatchPhrasePrefixQuery BuildMatchPhrasePrefixQuery(IZnodeSearchRequest request)
        {
            MatchPhrasePrefixQuery matchPhrasePrefixQuery = new MatchPhrasePrefixQuery();

            string searchField = "";

            if (request.SearchableAttribute?.Count > 0)
            {
                searchField = request.SearchableAttribute?.FirstOrDefault().AttributeCode.ToLower();
            }
            else
            {
                searchField = "productname";
            }

            matchPhrasePrefixQuery.Query = request.SearchText;

            matchPhrasePrefixQuery.Field = searchField; 

            return matchPhrasePrefixQuery;
        }       

        #endregion
    }
}
