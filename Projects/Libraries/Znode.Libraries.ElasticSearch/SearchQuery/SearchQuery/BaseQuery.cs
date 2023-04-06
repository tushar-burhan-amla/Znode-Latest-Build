using Nest;

using System;
using System.Collections.Generic;
using System.Linq;

using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Search;

namespace Znode.Libraries.ElasticSearch
{
    public class BaseQuery : IBaseQuery
    {
        #region Constructor

        public BaseQuery()
        {

        }

        #endregion Constructor

        #region Public Methods

        public virtual AggregationBase GetCategoryAggregation() => new TermsAggregation("category_aggregation")
        {
            Field = $"categoryname.keyword",
            //This is for the no of values in Category.We are setting it 1000.
            Size = 1000,
            Aggregations = new TermsAggregation("categoryid")
            {
                Field = $"categoryid"             
            }
        };

        public virtual AggregationBase GetFacetAggregation(AggregationBase aggregationBase, List<ElasticSearchAttributes> facetableAttribute)
        {
            foreach (var aggregationterm in facetableAttribute)
            {                
                TermsAggregation termsAggregation = new TermsAggregation(aggregationterm.AttributeCode.ToLower())
                {
                    Field = $"{aggregationterm.AttributeCode.ToLower()}.keyword",
                    //This is for the no of values in facets.We are setting it 1000.
                    Size = 1000,
                    Aggregations = new CardinalityAggregation(ZnodeConstant.DocumentCountCardinalityAggregation, new Field(ZnodeConstant.SkuKeywordField))                    
                };

                aggregationBase = HelperUtility.IsNull(aggregationBase) ? termsAggregation : aggregationBase &= termsAggregation;
            }

            return aggregationBase;
        }

        // To get the distinct result count.
        public virtual AggregationBase GetResultCountCardinalityAggregation(AggregationBase aggregationBase)
        {
            CardinalityAggregation cardinalityAggregation = new CardinalityAggregation(ZnodeConstant.DocumentCountCardinalityAggregation, new Field(ZnodeConstant.SkuKeywordField));
            aggregationBase = HelperUtility.IsNull(aggregationBase) ? cardinalityAggregation : aggregationBase &= cardinalityAggregation;

            return aggregationBase;
        }

        public virtual void AddFuzzinessQuery(MatchQuery query, IZnodeSearchRequest request)
        {
            if (IsFeatureActive(request, "AutoCorrect"))
            {
                query.Fuzziness = Fuzziness.Auto;
            }
        }

        public virtual SuggestContainer GetSuggestionQuery(IZnodeSearchRequest request)
        {
            SuggestContainer suggestContainer = new SuggestContainer();
            SuggestBucket bucket = new SuggestBucket();
            bucket.Text = request.SearchText;
            bucket.Phrase = new PhraseSuggester
            {
                Field = "didyoumean",
            };
            suggestContainer.Add("suggestion", bucket);
            return suggestContainer;
        }

        public virtual List<IScoreFunction> AddFunctionToSearchQuery(IZnodeSearchRequest request)
        {
            List<IScoreFunction> scoreFunctions = new List<IScoreFunction>();
            scoreFunctions.Add(new WeightFunction { Weight = 0.0 });
            return scoreFunctions;
        }

        public virtual List<ISort> AddSortToSearchQuery(IZnodeSearchRequest request)
        {
            List<ISort> sorts = new List<ISort>();

            if (request.SortCriteria?.Count > 0)
            {
                foreach (SortCriteria sort in request.SortCriteria)
                {
                    
                    SortCriteria.SortNameEnum sortItem = sort.SortName;
                    SortOrder sortOrder = sort.SortDirection == 0 ? SortOrder.Ascending : SortOrder.Descending;
                    FieldSort sorting = new FieldSort();
                    switch (sortItem)
                    {
                        case SortCriteria.SortNameEnum.Price:
                            sorting.Field = $"{ZnodeConstant.productPrice}";
                            sorting.Order = sortOrder;
                            break;

                        case SortCriteria.SortNameEnum.ProductName:
                            sorting.Field = $"productname.lowercase";
                            sorting.Order = sortOrder;
                            break;

                        case SortCriteria.SortNameEnum.HighestRated:
                            sorting.Field = $"{ZnodeConstant.ratings}";
                            sorting.Order = SortOrder.Descending;
                            break;

                        case SortCriteria.SortNameEnum.MostReviewed:
                            sorting.Field = $"{ZnodeConstant.totalreviewcount}";
                            sorting.Order = SortOrder.Descending;                        
                            break;

                        case SortCriteria.SortNameEnum.DisplayOrder:
                            sorting.Field = $"displayorder";
                            sorting.Order = sortOrder;
                            break;

                        case SortCriteria.SortNameEnum.ProductBoost:
                            sorting.Field = $"productboost.keyword";
                            sorting.Order = SortOrder.Descending;
                            break;
                    }
                    sorts.Add(sorting);
                }

            }
            else
            {
                if (request.SortCriteria?.Count == 0 && request.FieldValueFactors?.Count > 0)
                {
                    foreach (var item in request.FieldValueFactors)
                    {
                        SortOrder sortOrder = item.Value == 1 ? SortOrder.Ascending : SortOrder.Descending;
                        FieldSort sorting = new FieldSort();
                        sorting.Field = item.Key;
                        sorting.Order = sortOrder;

                        sorts.Add(sorting);
                    }

                }
            }

            return sorts;
        }

        //Add query for exact match result first then partial match.
        public virtual void CheckAndAddBoostQueryForExactMatchResult(IZnodeSearchRequest request)
        {
            /*If boost bury conditions are present then return 
             and do not add condition for exact match result first*/
            if (request.BoostAndBuryItemLists?.Count > 0)
                return;

            if (HelperUtility.IsNull(request.BoostAndBuryItemLists))
                request.BoostAndBuryItemLists = new List<ElasticSearchBoostAndBuryItemList>();

            string searchField = string.Empty;

            if (request.SearchableAttribute?.Count > 0)
            {
                searchField = request.SearchableAttribute?.FirstOrDefault().AttributeCode.ToLower();
            }
            else
            {
                searchField = "productname";
            }

            SetSearchItemConditionForBoostAndBury(request, searchField, ZnodeConstant.Is, 200);
            if (!string.IsNullOrEmpty(request.SearchText))
                SetSearchItemConditionForBoostAndBury(request, searchField, ZnodeConstant.StartWith, 200);
        }

        //Add query for exact match result first then partial match for multi match query type.
        public virtual void CheckAndAddBoostQueryForMultiMatchQuery(IZnodeSearchRequest request)
        {
            /*If boost bury conditions are present then return 
             and do not add condition for exact match result first*/
            if (request.BoostAndBuryItemLists?.Count > 0)
                return;

            if (HelperUtility.IsNull(request.BoostAndBuryItemLists))
                request.BoostAndBuryItemLists = new List<ElasticSearchBoostAndBuryItemList>();

            foreach (ElasticSearchAttributes item in request.SearchableAttribute)
            {
                SetSearchItemConditionForBoostAndBury(request, item.AttributeCode.ToLower(), ZnodeConstant.Is, 200);
                if (!string.IsNullOrEmpty(request.SearchText))
                    SetSearchItemConditionForBoostAndBury(request, item.AttributeCode.ToLower(), ZnodeConstant.StartWith, 200);
            }
        }

        // To set the search item condition for boost and bury.
        // Based on search item conditions boosting query will be generated in the GetBoostOrBuryItem method.
        protected virtual void SetSearchItemConditionForBoostAndBury(IZnodeSearchRequest request, string attributeCode, string searchItemCondition, int boostingValue)
        {
            request.BoostAndBuryItemLists.Add(new ElasticSearchBoostAndBuryItemList
            {
                SearchItemKeyword = attributeCode,
                SearchItemCondition = searchItemCondition,
                SearchItemValue = request.SearchText,
                SearchItemBoostValue = boostingValue
            });
        }

        //Get boost and Bury Condition for search result.
        public virtual BoolQuery GetBoostOrBuryItem(IZnodeSearchRequest request)
        {
            BoolQuery finalBoolQuery = new BoolQuery();
            List<QueryContainer> queryContainers = new List<QueryContainer>();

            if (request.BoostAndBuryItemLists?.Count > 0)
            {
                var boostAndBuryConditions = request.BoostAndBuryItemLists.GroupBy(x => x.SearchCatalogRuleId);

                foreach (var conditions in boostAndBuryConditions)
                {                   
                    QueryContainer query = new QueryContainer();
                    QueryContainer childquery = new QueryContainer();
                    
                    foreach (var item in conditions)
                    {                        
                        BoostingQuery boost = new BoostingQuery();
                        boost.Boost = 300;
                        if (!string.IsNullOrEmpty(item.SearchItemValue))
                        boost.PositiveQuery = new MatchAllQuery {};
                        switch (item.SearchItemCondition.ToLower())
                        {         
                            case "is":
                                MatchQuery isQuery = new MatchQuery();                               
                                isQuery.Field = item.SearchItemKeyword.ToLower() + ".lowercase";
                                isQuery.Query = item.SearchItemValue.ToLower();
                                boost.NegativeQuery = isQuery;                                
                                boost.NegativeBoost = Convert.ToDouble(item.SearchItemBoostValue);
                                break;
                            case "contains":
                                WildcardQuery wildcardContainsQuery = new WildcardQuery();
                                wildcardContainsQuery.Field = item.SearchItemKeyword.ToLower() + ".lowercase";
                                wildcardContainsQuery.Value = "*" + item.SearchItemValue.ToLower() + "*";
                                boost.NegativeQuery = wildcardContainsQuery;
                                boost.NegativeBoost = Convert.ToDouble(item.SearchItemBoostValue);
                                break;
                            case "startwith":
                                WildcardQuery wildcardStartWithQuery = new WildcardQuery();
                                wildcardStartWithQuery.Field = item.SearchItemKeyword.ToLower() + ".lowercase";
                                wildcardStartWithQuery.Value = item.SearchItemValue.ToLower() + "*";
                                boost.NegativeQuery = wildcardStartWithQuery;
                                boost.NegativeBoost = Convert.ToDouble(item.SearchItemBoostValue);
                                break;
                            case "endwith":
                                WildcardQuery wildcardEndsWithQuery = new WildcardQuery();
                                wildcardEndsWithQuery.Field = item.SearchItemKeyword.ToLower() + ".lowercase";
                                wildcardEndsWithQuery.Value = "*" + item.SearchItemValue.ToLower();
                                boost.NegativeQuery = wildcardEndsWithQuery;
                                boost.NegativeBoost = Convert.ToDouble(item.SearchItemBoostValue);
                                break;
                            //greater than Query for numeric field.
                            case "gt":
                                NumericRangeQuery greaterThanQuery = new NumericRangeQuery();
                                greaterThanQuery.Field = item.SearchItemKeyword.ToLower();
                                greaterThanQuery.GreaterThan = Convert.ToDouble(item.SearchItemValue);
                                boost.NegativeQuery = greaterThanQuery;
                                boost.NegativeBoost = Convert.ToDouble(item.SearchItemBoostValue);
                                break;
                            //greater than or Equal Query for numeric field.
                            case "gte":
                                NumericRangeQuery greaterThanOrEqualToQuery = new NumericRangeQuery();
                                greaterThanOrEqualToQuery.Field = item.SearchItemKeyword.ToLower();
                                greaterThanOrEqualToQuery.GreaterThanOrEqualTo = Convert.ToDouble(item.SearchItemValue);
                                boost.NegativeQuery = greaterThanOrEqualToQuery;
                                boost.NegativeBoost = Convert.ToDouble(item.SearchItemBoostValue);
                                break;
                            //less than Query for numeric field.
                            case "lt":
                                NumericRangeQuery lessThanQuery = new NumericRangeQuery();
                                lessThanQuery.Field = item.SearchItemKeyword.ToLower();
                                lessThanQuery.LessThan = Convert.ToDouble(item.SearchItemValue);
                                boost.NegativeQuery = lessThanQuery;
                                boost.NegativeBoost = Convert.ToDouble(item.SearchItemBoostValue);
                                break;
                            //less than or Equal Query for numeric field.
                            case "lte":
                                NumericRangeQuery lessThanOrEqualTo = new NumericRangeQuery();
                                lessThanOrEqualTo.Field = item.SearchItemKeyword.ToLower();
                                lessThanOrEqualTo.LessThanOrEqualTo = Convert.ToDouble(item.SearchItemValue);
                                boost.NegativeQuery = lessThanOrEqualTo;
                                boost.NegativeBoost = Convert.ToDouble(item.SearchItemBoostValue);
                                break;
                            //Equal Query for numeric field.
                            case "et":
                                TermQuery equalQuery = new TermQuery();
                                equalQuery.Field = item.SearchItemKeyword.ToLower();
                                equalQuery.Value = Convert.ToDouble(item.SearchItemValue);
                                boost.NegativeQuery = equalQuery;
                                boost.NegativeBoost = Convert.ToDouble(item.SearchItemBoostValue);
                                break;
                        }

                        //If IsItemForAll is true then all the condition will be in and condition.
                        if (conditions.FirstOrDefault().IsItemForAll)
                            query &= boost;
                        else
                            query |= boost;
                    }

                    queryContainers.Add(query);
                }
                finalBoolQuery.Should = queryContainers;
            }

            return finalBoolQuery;
        }

        #endregion Public Methods

        #region Protected Methods

        protected virtual Operator GetOperator(IZnodeSearchRequest request)
          => string.Equals(request.Operator, "and", StringComparison.InvariantCultureIgnoreCase) ? Operator.And : Operator.Or;

        protected virtual string GetFeatureValue(IZnodeSearchRequest request, string featureCode) =>
          request?.FeatureList?.Find(x => x.FeatureCode == featureCode)?.SearchFeatureValue;

        //Check if the feature is active.
        protected virtual bool IsFeatureActive(IZnodeSearchRequest request, string featureCode)
        {
            bool isActive;
            bool.TryParse(GetFeatureValue(request, featureCode), out isActive);
            return isActive;
        }

        #endregion Protected Methods
    }
}
