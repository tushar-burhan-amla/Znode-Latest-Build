using Nest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Search;
using Elasticsearch.Net;

using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Libraries.ElasticSearch
{
    public class ElasticSearchBaseService : IElasticSearchBaseService
    {
        #region Constructor

        public ElasticSearchBaseService()
        {
        }

        #endregion Constructor

        #region Elastic search

        #region Text search

        //Full text search(which contains the text of several text fields at once)
        public virtual IZnodeSearchResponse FullTextSearch(IZnodeSearchRequest request)
        {
            bool isSortEnabled = request.SortCriteria?.Count > 0;

            //Checks if the index is present or not.
            CheckIsIndexExists(request.IndexName);

            //Get paging params
            request = GetPagingParameters(request);

            SetDefaultFilters(request);

            ISearchResponse<dynamic> searchResponse = GetSearchResponse(request);

            return ElasticProductMapper.ElasticProductMapToZNodeSearchResponse(searchResponse, request.HighlightFieldName, request);
        }

        //Get elasticsearch response based on search request
        public virtual ISearchResponse<dynamic> GetSearchResponse(IZnodeSearchRequest request)
        {
            Assembly assembly = Assembly.Load("Znode.Libraries.ElasticSearch");
            Type className = assembly.GetTypes().FirstOrDefault(g => g.Name == request.QueryClass);
            var elasticSearchClient = ElasticConnectionHelper.ElasticSearchClient;

            //Create Instance of active class
            ISearchQuery instance = (ISearchQuery)Activator.CreateInstance(className);

            var _query = instance.GenerateQuery(request);

            var searchResponse = elasticSearchClient.Search<dynamic>(_query);

            return searchResponse;
        }

        public virtual List<string> FieldValueList(string indexName, string fieldType = "")
        {
            var elasticSearchClient = ElasticConnectionHelper.ElasticSearchClient;
            //Checks if the index is present or not.
            CheckIsIndexExists(indexName);

            //Get Index Mapping request.
            GetMappingRequest getMappingRequest = new GetMappingRequest(indexName);

            GetMappingResponse fieldMappings = elasticSearchClient.Indices.GetMapping(getMappingRequest);

            List<string> listOfField = new List<string>();
            listOfField.Add("profileids");
            listOfField.Add("productindex");
            listOfField.Add("versionid");
            listOfField.Add("localeid");
            listOfField.Add("timestamp");
            listOfField.Add("znodeproductid");
            listOfField.Add("categoryid");
            listOfField.Add("znodecatalogid");
            listOfField.Add("attributes");
            listOfField.Add("outofstockoptions");
            listOfField.Add("isactive");
            listOfField.Add("callforpricing");
            listOfField.Add("producttype");
            listOfField.Add("displayorder");

            switch (fieldType)
            {
                case "number":
                    if (fieldMappings.GetMappingFor(indexName)?.Properties?.Count > 0)
                        return fieldMappings.GetMappingFor(indexName).Properties.Where(x => x.Value.Type == "float" || x.Value.Type == "double" || x.Value.Type == "long").Select(y => y.Value.Name).Select(z => z.Name).Where(y => !listOfField.Contains(y)).ToList();
                    break;
                case "all":
                    if (fieldMappings.GetMappingFor(indexName)?.Properties?.Count > 0)
                        return fieldMappings.GetMappingFor(indexName).Properties.Select(y => y.Value.Name).Select(z => z.Name).Where(y => !listOfField.Contains(y)).ToList();
                    break;
            }

            return new List<string>();
        }


        //Get Auto suggestion for boost and bury.
        public virtual List<string> GetBoostAndBuryAutoSuggestion(string indexName, int publishCatalogId, string fieldName, string searchTerm, int? catalogVersionId ,int localeId)
        {
            var elasticSearchClient = ElasticConnectionHelper.ElasticSearchClient;
            //Checks if the index is present or not.
            CheckIsIndexExists(indexName);

            BoolQuery finalBoolQuery = new BoolQuery();

            finalBoolQuery.Filter = GetBoostAndBuryAutoSuggestionFilters(publishCatalogId, catalogVersionId, localeId);
     
            SearchRequest<dynamic> searchRequest = new SearchRequest<dynamic>(indexName);
            searchRequest.From = 1;
            searchRequest.Size = 12;

            PrefixQuery prefixQuery = new PrefixQuery();
            prefixQuery.Field = $"{fieldName.ToLower()}.autocomplete";
            prefixQuery.Value = searchTerm.ToLower();

            finalBoolQuery.Must = new List<QueryContainer> { prefixQuery };

            searchRequest.Query = finalBoolQuery;

            searchRequest.Aggregations = new TermsAggregation("autocomplete")
            {
                Field = $"{fieldName.ToLower()}.autocomplete",
                Include = new TermsInclude($"{searchTerm.ToLower()}.*"),
                Order = new List<TermsOrder> { TermsOrder.CountDescending },
                Aggregations = new TermsAggregation("name")
                {
                    Size = 1000 ,
                    Field = $"{fieldName.ToLower()}.keyword"
                }
            };

            var searchResponse = elasticSearchClient.Search<dynamic>(searchRequest);

            List<string> suggestionList = new List<string>();

            TermsAggregate<string> suggestions = searchResponse.Aggregations.Terms("autocomplete");

            var suggestionBucket = suggestions.Buckets;

            foreach (var bucket in suggestionBucket)
            {
                if (bucket.Terms("name").Buckets.Count > 0)
                {
                    string value = bucket.Terms("name").Buckets.FirstOrDefault().Key;
                    suggestionList.Add(value);
                }
            }

            return suggestionList.Distinct().ToList();
        }

        // Method to check the cluster health of the elastic search index.
        public virtual string CheckElasticSearch()
        {
            var elasticSearchClient = ElasticConnectionHelper.ElasticSearchClient;
            // To get the cluster health details.
            ClusterHealthResponse clusterHealthResponse = elasticSearchClient?.Cluster?.Health();

            // If the API call fails, the service might not be in the running state.
            if (!(clusterHealthResponse?.ApiCall?.Success).GetValueOrDefault())
                return Health.Red.ToString();

            return clusterHealthResponse?.Status.ToString();
        }

        #endregion Text search

        #region Create query

        protected virtual QueryContainer GetMatchQuery(string andItem, string itemValue, bool isKey = false) =>
         new QueryContainerDescriptor<SearchProduct>().Match(match => match.Field(andItem).Query(itemValue.ToLower()).Operator(Operator.And));

        // Get term query.
        protected virtual QueryContainer GetTermQuery(string andItem, string itemValue, double boost = 0.0, bool isSortEnabled = false)
        {
            boost = isSortEnabled ? 0.0 : boost;
            return new QueryContainerDescriptor<SearchProduct>().Term(s => s.Field(andItem).Value(itemValue.ToLower()).Boost(boost));
        }

        // Get terms query to find multiple values for a single field.
        protected virtual QueryContainer GetTermContainsQuery(string andItem, List<string> itemValues, bool isSortEnabled = false)
        => new QueryContainerDescriptor<SearchProduct>().Terms(s => s.Field(andItem).Terms(itemValues.ConvertAll(d => d.ToLower())));

        //Get query filters for Boost and bury AutoSuggestion.
        protected virtual List<QueryContainer> GetBoostAndBuryAutoSuggestionFilters(int publishCatalogId, int? catalogVersionId, int localeId)
        {
            List<QueryContainer> catalogIdLocaleIdContainerList = new List<QueryContainer>();
            catalogIdLocaleIdContainerList.Add(GetTermQuery("localeid", localeId.ToString()));
            if(publishCatalogId > 0)
            {
                catalogIdLocaleIdContainerList.Add(GetTermQuery("znodecatalogid", publishCatalogId.ToString()));
            }           
            catalogIdLocaleIdContainerList.Add(GetTermQuery("isactive", "True"));
            catalogIdLocaleIdContainerList.Add(GetTermQuery("productindex", "1"));
            if(HelperUtility.IsNotNull( catalogVersionId))
            {
                catalogIdLocaleIdContainerList.Add(GetTermQuery("versionid", catalogVersionId.ToString()));
            }
            

            return catalogIdLocaleIdContainerList;
        }

        #endregion Create query

        #region Get paging params.

        //Get paging params.
        protected virtual IZnodeSearchRequest GetPagingParameters(IZnodeSearchRequest request)
        {
            //Condition to get all products.
            if (request.PageSize == -1)
            {
                //Maximum number of records elastic search can fetch.
                request.PageSize = 10000;
                request.StartIndex = 0;
                return request;
            }

            request.StartIndex = request.PageSize * (request.PageFrom - 1);
            return request;
        }

        #endregion Get paging params.

        #region Index Validation

        //Checks if an index exists.
        protected virtual void CheckIsIndexExists(string indexName)
        {
            var elasticSearchClient = ElasticConnectionHelper.ElasticSearchClient;
            if (string.IsNullOrEmpty(indexName))
                throw new ZnodeException(ErrorCodes.InvalidData, "Index name cannot be blank.");
            if (!elasticSearchClient.Indices.Exists(indexName).Exists)
                throw new ZnodeException(ErrorCodes.NotFound, "Search index does not exist.");
        }

        //Checks if an index exists.
        protected virtual void CheckIsCMSPageIndexExists(string indexName)
        {
            if (string.IsNullOrEmpty(indexName))
                throw new ZnodeException(ErrorCodes.InvalidData, "CMS page search index name cannot be blank");
            if (!IsIndexExist(indexName))
                throw new ZnodeException(ErrorCodes.NotFound, "CMS page search index does not exist in elastic search indices.");
        }

        //Check current index exist in elastic search node
        protected virtual bool IsIndexExist(string indexName)
        {
            return ElasticConnectionHelper.ElasticSearchClient.Indices.Exists(indexName).Exists;
        }

        #endregion Index Validation

        //Gets default filters for catalog, locale ID, etc.
        protected virtual List<QueryContainer> SetDefaultFilters(IZnodeSearchRequest request)
        {
            List<QueryContainer> catalogIdLocaleIdContainerList = new List<QueryContainer>();
            // Loop to add catalogId, localId and categoryId
            foreach (var andItem in request.CatalogIdLocalIdDictionary ?? new Dictionary<string, List<string>>())
            {// Added item to list to create "AND" query.
                if(string.Equals(andItem.Key, ZnodeConstant.ParentCategoryIds, StringComparison.InvariantCultureIgnoreCase))
                    catalogIdLocaleIdContainerList.Add(GetTermContainsQuery(andItem.Key.ToString(), andItem.Value));
                else if (andItem.Value.Count == 1)
                    catalogIdLocaleIdContainerList.Add(GetTermQuery(andItem.Key.ToString(), andItem.Value.FirstOrDefault()));
                else if (andItem.Value.Count > 1)
                    catalogIdLocaleIdContainerList.Add(GetTermContainsQuery(andItem.Key.ToString(), andItem.Value));
            }

            foreach (var andItem in request.Facets ?? new Dictionary<string, List<string>>())
            {
                List<QueryContainer> fieldValuesQuery = new List<QueryContainer>();

                foreach (var item in andItem.Value)
                {
                    fieldValuesQuery.Add(new QueryContainerDescriptor<SearchProduct>()
                    .Term(matchphrase => matchphrase.Field($"{andItem.Key.ToLower()}.keyword").Value(item)));
                }

                QueryContainer orQueryContainer = new QueryContainer();

                // Loop on item to get "OR" query which used for searchable field.
                foreach (QueryContainer item in fieldValuesQuery)
                    orQueryContainer |= +item;

                // Added combination of "AND" and "OR" query to create final query.
                catalogIdLocaleIdContainerList.Add(orQueryContainer);
            }

            request.FilterValues = catalogIdLocaleIdContainerList;
            return catalogIdLocaleIdContainerList;
        }

        #endregion Elastic search

        #region CMS Page Elastic Search

        //Get cms page count from elastic search count api.
        public virtual CountResponse GetCountSearchResponse(IZnodeSearchRequest request)
        {
            var elasticSearchClient = ElasticConnectionHelper.ElasticSearchClient;
            CountRequest<dynamic> _query = GetService<ICountQuery>().GenerateCountQuery(request);

            CountResponse searchResponse = elasticSearchClient.Count(_query);

            return searchResponse;
        }

        //Get default filters for portalId, locale ID, isActive, profileId, VersionId  etc.
        public virtual List<QueryContainer> SetDefaultFilters(IZnodeCMSPageSearchRequest request)
        {
            List<QueryContainer> filterContainerList = new List<QueryContainer>();

            foreach (KeyValuePair<string, List<string>> filterItem in request.FilterDictionary ?? new Dictionary<string, List<string>>())
            {
                // Added item to list to create "AND" query.
                if (filterItem.Value.Count == 1)
                    filterContainerList.Add(GetTermQuery(filterItem.Key.ToString(), filterItem.Value.FirstOrDefault()));
                else if (filterItem.Value.Count > 1)
                    filterContainerList.Add(GetTermContainsQuery(filterItem.Key.ToString(), filterItem.Value));
            }

            request.FilterValues = filterContainerList;
            return filterContainerList;
        }

        //Get paging params.
        protected virtual IZnodeCMSPageSearchRequest GetPagingParameters(IZnodeCMSPageSearchRequest request)
        {
            //Condition to get all Cms pages.
            if (request.PageSize == -1)
            {
                //Maximum number of records elastic search can fetch.
                request.PageSize = 10000;
                request.StartIndex = 0;
                return request;
            }

            request.StartIndex = request.PageSize * (request.PageFrom - 1);
            return request;
        }
        #endregion
    }
}