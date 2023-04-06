using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using Znode.Libraries.Search;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Libraries.ElasticSearch
{
    public static class ElasticProductMapper
    {
        // Maps elastic search response's documents in ElasticSearchResponse model. 
        public static ElasticSearchResponse ElasticProductMapToZNodeSearchResponse(ISearchResponse<dynamic> searchResponse, string highlightFieldName, IZnodeSearchRequest request)/*, int totalCount, TermsAggregate<string> categoryAggregate)*/
        {
            List<IZNodeSearchFacet> Facets = new List<IZNodeSearchFacet>();
            List<IZNodeSearchCategoryItem> categoryItems = new List<IZNodeSearchCategoryItem>();
            ElasticSearchResponse elasticSearchResponse = new ElasticSearchResponse();

            ValueAggregate documentCountCardinalityAggregation = new ValueAggregate();

            foreach (var aggregate in searchResponse.Aggregations)
            {
                switch (aggregate.Key)
                {
                    case "category_aggregation":
                        categoryItems = MapCategoryAggregation(aggregate, searchResponse);
                        break;
                    case "Count_Aggregation":
                        ValueAggregate valueAggregate = searchResponse.Aggregations.Cardinality("Count_Aggregation");
                        break;
                    case ZnodeConstant.DocumentCountCardinalityAggregation:
                        documentCountCardinalityAggregation = searchResponse.Aggregations.Cardinality(ZnodeConstant.DocumentCountCardinalityAggregation);
                        break;
                    default:
                        var model = MapFacetAggregation(aggregate, searchResponse, request);
                        if (!Equals(model, null))
                            Facets.Add(model);
                        break;
                }
            }
            elasticSearchResponse.Facets = Facets;
            elasticSearchResponse.TotalProductCount = GetDocumentCountFromAggregation(documentCountCardinalityAggregation);
            elasticSearchResponse.ProductDetails = searchResponse.Documents.ToList();

            elasticSearchResponse.CategoryItems = categoryItems;

            string debugInformation = searchResponse.DebugInformation;
            string startTag = "# Request:";
            int startIndex = debugInformation.IndexOf(startTag) + startTag.Length;
            int endIndex = debugInformation.IndexOf("# Response:", startIndex);
            string requestbody = debugInformation.Substring(startIndex, endIndex - startIndex);
            elasticSearchResponse.RequestBody = requestbody;
            if (searchResponse.Documents.Count <= 0)
                elasticSearchResponse.SuggestionTerm = searchResponse.Suggest?.Values?.FirstOrDefault()?.OrderByDescending(option => option)?.FirstOrDefault()?.Text;

            return elasticSearchResponse;
        }

        public static ElasticSearchResponse MapSuggestionResponse(ISearchResponse<dynamic> searchResponse, string highlightFieldName, IZnodeSearchRequest request)
        {
            ElasticSearchResponse elasticSearchResponse = new ElasticSearchResponse();
            elasticSearchResponse.TotalProductCount = searchResponse.Total;
            elasticSearchResponse.ProductDetails = searchResponse.Documents.ToList();
            return elasticSearchResponse;
        }

        private static IZNodeSearchFacet MapFacetAggregation(KeyValuePair<string, IAggregate> aggregate, ISearchResponse<dynamic> searchResponse, IZnodeSearchRequest request)
        {

            IZNodeSearchFacet facet = request.AttributeList.Where(x => x.attributecode.Equals(aggregate.Key, StringComparison.InvariantCultureIgnoreCase))
                                         .Select(m => new ElasticSearchFacet { DisplayOrder = m.displayorder, AttributeName = m.attributename,
                                             AttributeValues = m.selectvalues.Select(n=>new ElasticSearchFacetValue() {
                                                 displayorder=n.displayorder,
                                                 AttributeValue=n.value
                                             }).ToList() })?.FirstOrDefault();
            if (!Equals(facet, null))
            {
                facet.AttributeCode = aggregate.Key;

                TermsAggregate<string> termAggregate4 = searchResponse.Aggregations.Terms(aggregate.Key);
                var facetBucket = termAggregate4.Buckets;
                List<ElasticSearchFacetValue> list = new List<ElasticSearchFacetValue>();

                foreach (var attributeValues in facetBucket)
                {
                    ElasticSearchFacetValue facetValues = new ElasticSearchFacetValue();
                    facetValues.AttributeValue = attributeValues.Key;
                    facetValues.FacetCount = GetDocumentCountFromAggregation((ValueAggregate)attributeValues?.FirstOrDefault(x => string.Equals(x.Key, ZnodeConstant.DocumentCountCardinalityAggregation, StringComparison.InvariantCultureIgnoreCase)).Value);
                    facetValues.displayorder = facet.AttributeValues.FirstOrDefault(w => w.AttributeValue == attributeValues.Key.ToString())?.displayorder;
                    list.Add(facetValues);
                }
                facet.AttributeValues = list;
            }
            return facet;
        }

        // To get the distinct document count from cardinality aggregation results.
        private static long GetDocumentCountFromAggregation(ValueAggregate valueAggregate)
        {
            long distinctDocumentCount;
            long.TryParse(valueAggregate.Value.ToString(), out distinctDocumentCount);
            return distinctDocumentCount;
        }

        private static List<IZNodeSearchCategoryItem> MapCategoryAggregation(KeyValuePair<string, IAggregate> aggregate, ISearchResponse<dynamic> searchResponse)
        {
            List<IZNodeSearchCategoryItem> categoryItems = new List<IZNodeSearchCategoryItem>();
            TermsAggregate<string> category = searchResponse.Aggregations.Terms("category_aggregation");

            var categoryBucket = category.Buckets;

            foreach (var bucket in categoryBucket)
            {
                if (bucket.Terms("categoryid").Buckets.Count > 0)
                {
                    IZNodeSearchCategoryItem categoryFacet = new ElasticSearchCategoryItem();
                    string categoryid = bucket.Terms("categoryid").Buckets.FirstOrDefault().Key;
                    categoryFacet.Name = bucket.Key;
                    categoryFacet.Count = bucket.DocCount;
                    categoryFacet.CategoryID = Convert.ToInt32(categoryid);
                    categoryItems.Add(categoryFacet);
                }
            }

            return categoryItems;
        }

        // Gets search category item from product response's elastic categories.
        public static List<IZNodeSearchCategoryItem> ElasticCategoryToSearchCategoryItem(List<SearchCategory> elasticCategories)
        {
            List<IZNodeSearchCategoryItem> searchCategory = new List<IZNodeSearchCategoryItem>();
            foreach (SearchCategory item in elasticCategories)
            {
                searchCategory.Add(new ElasticSearchCategoryItem
                {
                    SEOUrl = item.seourl,
                    CategoryID = item.categoryid,
                    Name = item.categoryname,
                    ParentCategory = (ECommerce.Utilities.HelperUtility.IsNull(item.parentcategories)) ? null : ElasticCategoryToSearchCategoryItem(item.parentcategories)
                });
            }
            return searchCategory;
        }
    }
}
