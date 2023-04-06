using Nest;

using System.Collections.Generic;

using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Search;

namespace Znode.Libraries.ElasticSearch
{
    public class MultiMatchQueryBuilder : BaseQuery, ISearchQuery
    {
        #region Protected Variables
        protected readonly FunctionScoreQuery functionScoreQuery = new FunctionScoreQuery();
        protected readonly BoolQuery finalBoolQuery = new BoolQuery();
        #endregion

        #region Public Methods

        public virtual SearchRequest<dynamic> GenerateQuery(IZnodeSearchRequest request)
        {
          MultiMatchQuery multiMatchQuery = new MultiMatchQuery();
            List<Field> multipleFields = new List<Field>();

            foreach (ElasticSearchAttributes item in request.SearchableAttribute)
            {
                multipleFields.Add(new Field(item.AttributeCode.ToLower(), item.BoostValue));
            }

            multiMatchQuery.Query = request.SearchText;
            multiMatchQuery.Fields = multipleFields.ToArray();

            if (!string.IsNullOrEmpty(GetFeatureValue(request, "MinimumShouldMatch")))
                multiMatchQuery.MinimumShouldMatch = GetFeatureValue(request, "MinimumShouldMatch");

            multiMatchQuery.Operator = GetOperator(request);

            switch (request.SubQueryType?.Replace(" ", "").ToLower())
            {
                case ZnodeConstant.MultiMatchBest:
                    multiMatchQuery.Type = TextQueryType.BestFields;
                    break;

                case ZnodeConstant.Most:
                    multiMatchQuery.Type = TextQueryType.MostFields;
                    break;

                case ZnodeConstant.MultiMatchCross:
                    multiMatchQuery.Type = TextQueryType.CrossFields;
                    break;

                case ZnodeConstant.Phrase:
                    multiMatchQuery.Type = TextQueryType.Phrase;
                    break;
            }

            //Check if auto correct is active.
            if (IsFeatureActive(request, "AutoCorrect"))
            {
                multiMatchQuery.Fuzziness = Fuzziness.Auto;
            }

            functionScoreQuery.ScoreMode = FunctionScoreMode.Sum;
            functionScoreQuery.BoostMode = FunctionBoostMode.Sum;

            functionScoreQuery.Functions = AddFunctionToSearchQuery(request);

            finalBoolQuery.Must = new List<QueryContainer> { multiMatchQuery };

            //If boost and bury conditions are not present then add condition for exact Match first.
            CheckAndAddBoostQueryForMultiMatchQuery(request);

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

            aggregationBase = GetResultCountCardinalityAggregation(aggregationBase);

            List<ISort> SortSettings = AddSortToSearchQuery(request);

            SearchRequest<dynamic> searchRequest = new SearchRequest<dynamic>(request.IndexName);
            searchRequest.Query = functionScoreQuery;
            if (HelperUtility.IsNull(request.Source))
            {
                if (HelperUtility.IsNull(searchRequest.Collapse))
                    searchRequest.Collapse = new FieldCollapse();
                searchRequest.Collapse.Field = new Field(ZnodeConstant.SkuKeywordField);
            }
            searchRequest.From = request.StartIndex;
            searchRequest.Size = request.PageSize;
            searchRequest.Source = new Union<bool, ISourceFilter>(new SourceFilter {Includes = request.Source});
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
    }
}
