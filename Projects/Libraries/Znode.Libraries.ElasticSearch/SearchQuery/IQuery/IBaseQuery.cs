using Nest;

using System.Collections.Generic;

using Znode.Libraries.Search;

namespace Znode.Libraries.ElasticSearch
{
    public interface IBaseQuery
    {
        /// <summary>
        /// Set category aggregate value to object
        /// </summary>
        /// <returns>Return aggregate object</returns>
        AggregationBase GetCategoryAggregation();

        /// <summary>
        ///  Set facet aggregate value to object
        /// </summary>
        /// <param name="aggregationBase">aggregationBase</param>
        /// <param name="facetableAttribute">facetableAttribute</param>
        /// <returns>Return aggregate object</returns>
        AggregationBase GetFacetAggregation(AggregationBase aggregationBase, List<ElasticSearchAttributes> facetableAttribute);

        /// <summary>
        /// Add fuzziness in request query
        /// </summary>
        /// <param name="query">query</param>
        /// <param name="request">request</param>
        void AddFuzzinessQuery(MatchQuery query, IZnodeSearchRequest request);

        /// <summary>
        /// Get suggestion query to add into request
        /// </summary>
        /// <param name="request">request</param>
        /// <returns>Return suggestion query</returns>
        SuggestContainer GetSuggestionQuery(IZnodeSearchRequest request);

        /// <summary>
        /// Get score function 
        /// </summary>
        /// <param name="request">request</param>
        /// <returns>Return score function</returns>
        List<IScoreFunction> AddFunctionToSearchQuery(IZnodeSearchRequest request);

        /// <summary>
        /// Get sorting option as per search request query
        /// </summary>
        /// <param name="request">request</param>
        /// <returns>Return sorting option</returns>
        List<ISort> AddSortToSearchQuery(IZnodeSearchRequest request);

        /// <summary>
        /// Add boost and bury option in search request for exact-match result
        /// </summary>
        /// <param name="request">request</param>
        void CheckAndAddBoostQueryForExactMatchResult(IZnodeSearchRequest request);

        /// <summary>
        /// Add boost and bury option in search request for multi-match query
        /// </summary>
        /// <param name="request">request</param>
        void CheckAndAddBoostQueryForMultiMatchQuery(IZnodeSearchRequest request);

        /// <summary>
        /// Get boost or bury query option
        /// </summary>
        /// <param name="request">request</param>
        /// <returns>Return boost or bury query</returns>
        BoolQuery GetBoostOrBuryItem(IZnodeSearchRequest request);

        /// <summary>
        /// To get the distinct result count.
        /// </summary>
        /// <param name="aggregationBase">AggregationBase</param>
        /// <returns>AggregationBase</returns>
        AggregationBase GetResultCountCardinalityAggregation(AggregationBase aggregationBase);
    }
}
