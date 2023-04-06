using Nest;

using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data;
using System.Diagnostics;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Libraries.Admin;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Search;
using Newtonsoft.Json;

using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Libraries.ElasticSearch
{
    public class ElasticSearchIndexerService : IElasticSearchIndexerService
    {
        #region Protected Variables
        protected readonly ISearchProductService searchProductService;
        protected readonly IZnodeSearchIndexSettingHelper _searchIndexSettingHelper;
        #endregion Protected Variables

        #region Public Properties

        public int PageLength { get; set; }

        #endregion Public Properties

        #region Public constructor

        public ElasticSearchIndexerService()
        {
            searchProductService = GetService<ISearchProductService>();
            _searchIndexSettingHelper = GetService<IZnodeSearchIndexSettingHelper>();
        }

        public ElasticSearchIndexerService(int pageLength = 1000)
        {
            searchProductService = GetService<ISearchProductService>();
            _searchIndexSettingHelper = GetService<IZnodeSearchIndexSettingHelper>();
            PageLength = pageLength;
        }

        #endregion Public constructor

        #region Public Methods

        //Returns the product descriptor.
        public virtual CreateIndexDescriptor AddProductDocuments(ElasticClient elasticClient, IndexName indexName, int publishCatalogId)
        {
            CreateIndexDescriptor productDescriptor = GetCreateIndexDescriptor(indexName, publishCatalogId);
            return productDescriptor;
        }

        //Create elastic index for the catalog data.
        public virtual void CreateIndex(ElasticClient elasticClient, IndexName indexName, SearchParameterModel searchParameterModel)
        {
            ZnodeLogging.LogMessage("Create index execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);

            int pageIndex = 1;
            int totalRecordCount = 0;
            decimal totalPages = 0;
            int pageSize = ZnodeApiSettings.PublishProductFetchChunkSize;
          
            IndexName oldIndexName = indexName;

            bool isAllowIndexCreation = searchParameterModel.IsPreviewProductionEnabled ? string.Equals(searchParameterModel.revisionType, ZnodePublishStatesEnum.PRODUCTION.ToString(), StringComparison.InvariantCultureIgnoreCase) : true;

            bool isIndexExist = IsIndexExists(elasticClient, indexName);

            bool indexCreationForProductionOnly = true;

            // Restricting index creation for only preview
            if ((!searchParameterModel.IsPreviewProductionEnabled && string.Equals(searchParameterModel.revisionType, ZnodePublishStatesEnum.PREVIEW.ToString(), StringComparison.InvariantCultureIgnoreCase)) && isIndexExist)
                indexCreationForProductionOnly = false;

            if (!searchParameterModel.IsPublishDraftProductsOnly && indexCreationForProductionOnly)
            {
                if (isIndexExist && !string.IsNullOrEmpty(searchParameterModel.NewIndexName))
                    indexName = searchParameterModel.NewIndexName;

                //Ensure that the elastic index schema exist, if not exist then create it. #Step 1
                EnsureIndexSchemaExists(elasticClient, indexName, searchParameterModel, isIndexExist);
            }

            //Get all active locales. #Step 2
            int[] localeIds = searchParameterModel.ActiveLocales?.Select(x => x.LocaleId).ToArray();

            //Get all version id of the current catalog id for all locales. #Step 3
            int[] latestVersionIds = searchProductService.GetLatestVersionId(searchParameterModel.CatalogId, searchParameterModel.revisionType, localeIds);

            //Get all categories id associated to the catalog. #Step 4
            IEnumerable<int> publishcategoryIds = GetService<ISearchCategoryService>().GetPublishCategoryList(searchParameterModel.CatalogId, latestVersionIds)?.Select(x => x.ZnodeCategoryId)?.Distinct();

            //Get all publish product count. #Step 5
            totalRecordCount = searchProductService.GetAllProductCount(searchParameterModel.CatalogId, latestVersionIds);

            totalPages = totalRecordCount != 0 ? totalRecordCount < pageSize ? 1 : Math.Ceiling((decimal)totalRecordCount / pageSize) : 0;

            ZnodeLogging.LogMessage($"Total products record count : {totalRecordCount} and total page count: {totalPages} for indexing ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);

            SearchHelper searchHelper = new SearchHelper();

            try
            {
                //Save InProgress status for the server name. #Step 6
                searchHelper.UpdateSearchIndexServerStatus(new SearchIndexServerStatusModel() { SearchIndexServerStatusId = searchParameterModel.SearchIndexServerStatusId, SearchIndexMonitorId = searchParameterModel.SearchIndexMonitorId, ServerName = Environment.MachineName, Status = ZnodeConstant.SearchIndexInProgressStatus });

                while (pageIndex <= totalPages)
                {
                    List<dynamic> productsData = new List<dynamic>();

                    //Get publish product data in the require form for the elastic search indexing process. #Step 7
                    productsData = GetPublishProductData(searchParameterModel, latestVersionIds, publishcategoryIds, pageIndex, pageSize,searchParameterModel.IsPublishDraftProductsOnly);

                    //Generate elastic search index for the given data. #Step 8
                    GenerateElasticSearchIndex(elasticClient, searchParameterModel, indexName, productsData, searchParameterModel.IsPublishDraftProductsOnly);
                                        
                    pageIndex++;
                }

                if (ValidateIsIndexNameChanged(oldIndexName, indexName) && isAllowIndexCreation && !searchParameterModel.IsPublishDraftProductsOnly)
                {
                    PutAliasOnNewlyCreatedIndex(elasticClient, oldIndexName.Name, indexName.Name, searchParameterModel.CatalogId);
                }

                //Save complete status for the server name. #Step 9
                searchHelper.UpdateSearchIndexServerStatus(new SearchIndexServerStatusModel() { SearchIndexServerStatusId = searchParameterModel.SearchIndexServerStatusId, SearchIndexMonitorId = searchParameterModel.SearchIndexMonitorId, ServerName = Environment.MachineName, Status = ZnodeConstant.SearchIndexCompleteStatus });

                ZnodeLogging.LogMessage("Create index execution done", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error, ex);
                searchHelper.UpdateSearchIndexServerStatus(new SearchIndexServerStatusModel() { SearchIndexServerStatusId = searchParameterModel.SearchIndexServerStatusId, SearchIndexMonitorId = searchParameterModel.SearchIndexMonitorId, ServerName = Environment.MachineName, Status = ZnodeConstant.SearchIndexFailedStatus });
                ZnodeLogging.LogMessage("Search index server status updated to Failed", ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
                throw;
            }
        }

        // To validate is index name changed.
        protected virtual bool ValidateIsIndexNameChanged(IndexName oldIndexName, IndexName newIndexName)
            => !string.Equals(oldIndexName.Name, newIndexName.Name, StringComparison.InvariantCultureIgnoreCase);

        //Ensure that the elastic index schema exist, if not exist then create it
        protected virtual void EnsureIndexSchemaExists(ElasticClient elasticClient, IndexName indexName, SearchParameterModel searchParameterModel, bool isIndexExist)
        {
            ZnodeLogging.LogMessage("Ensure index execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);

            SearchHelper searchHelper = new SearchHelper();
            if (!(string.Equals(searchParameterModel.revisionType, ZnodePublishStatesEnum.PRODUCTION.ToString(), StringComparison.InvariantCultureIgnoreCase) && searchParameterModel.IsPreviewEnabled))
            {
                CreateIndexResponse createIndexResponse = CreateElasticSearchIndex(elasticClient, indexName, searchParameterModel);
             
                if (!createIndexResponse.IsValid)
                {
                    searchHelper.UpdateSearchIndexServerStatus(new SearchIndexServerStatusModel() { SearchIndexServerStatusId = searchParameterModel.SearchIndexServerStatusId, SearchIndexMonitorId = searchParameterModel.SearchIndexMonitorId, ServerName = Environment.MachineName, Status = ZnodeConstant.SearchIndexFailedStatus });
                    throw new Exception($"Elastic index schema creation is failed. {createIndexResponse.OriginalException.Message}");
                }
            }
            ZnodeLogging.LogMessage("Ensure index execution done", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
        }
        
        // To create an index with a new name.
        protected virtual CreateIndexResponse CreateElasticSearchIndex(ElasticClient elasticClient, IndexName indexName, SearchParameterModel searchParameterModel)
        {
            CreateIndexDescriptor createIndexDescriptor = AddProductDocuments(elasticClient, indexName, searchParameterModel.CatalogId);

            // To create a new index with specified name.
            return elasticClient.Indices.Create(indexName, i => createIndexDescriptor);
        }

        // To assign alias to the newly created index.
        protected virtual void PutAliasOnNewlyCreatedIndex(ElasticClient elasticClient, string oldIndexName, string indexName, int publishCatalogId)
        {
            bool isExistingIndexDeleted = false, isAliasAppliedOnNewIndex = false;
            ZnodeCatalogIndex catalogIndex = searchProductService.GetCatalogIndexByPublishCatalogId(publishCatalogId);

            if (UpdateIndexNameInDB(catalogIndex, indexName))
                isExistingIndexDeleted = DeleteAliasAndRespectiveIndices(elasticClient, oldIndexName);

            if (isExistingIndexDeleted)
                isAliasAppliedOnNewIndex = PutAliasOnIndex(elasticClient, indexName, oldIndexName);

            if (isAliasAppliedOnNewIndex)
                UpdateIndexNameInDB(catalogIndex, oldIndexName);
        }

        // To delete an alias and respective indices.
        protected virtual bool DeleteAliasAndRespectiveIndices(ElasticClient elasticClient, string alias)
        {
            bool isExistingAliasDeleted;
            string indexNames = string.Join(",", GetIndicesPointingToAlias(elasticClient, alias));
            isExistingAliasDeleted = !string.IsNullOrEmpty(indexNames) ? DeleteIndexAlias(elasticClient, indexNames, alias) : true;
            return DeleteElasticSearchIndex(elasticClient, string.IsNullOrEmpty(indexNames) ? alias : indexNames) && isExistingAliasDeleted;
        }

        // To update index name in ZnodeCatalogIndex Table in database.
        protected virtual bool UpdateIndexNameInDB(ZnodeCatalogIndex catalogIndex, string indexName)
        {
            catalogIndex.IndexName = indexName;
            return searchProductService.UpdateCatalogIndex(catalogIndex);
        }

        // To get the index names which are pointed by the specified alias.
        public virtual IReadOnlyCollection<string> GetIndicesPointingToAlias(ElasticClient elasticClient, string alias)
            => !string.IsNullOrEmpty(alias) ? elasticClient.GetIndicesPointingToAlias(alias) : null;

        // To put an alias on an index.
        protected virtual bool PutAliasOnIndex(ElasticClient elasticClient, string indexName, string aliasName)
        {
            return elasticClient.Indices.PutAlias(indexName, aliasName, s => s.IsWriteIndex()).IsValid;
        }

        // To delete an alias based on index names(comma separated index names can be provided) and alias name.
        protected virtual bool DeleteIndexAlias(ElasticClient elasticClient, string indexNames, string aliasName)
        {
            if (!string.IsNullOrEmpty(indexNames))
                return elasticClient.Indices.DeleteAlias(indexNames, aliasName).IsValid;
            return false;
        }
            
        //Get publish product data in the require form for the elastic search indexing process
        public virtual List<dynamic> GetPublishProductData(SearchParameterModel searchParameterModel, int[] latestVersionIds, IEnumerable<int> publishcategoryIds, int pageIndex, int pageSize, bool isPublishDraftProductsOnly)
        {
            ZnodeLogging.LogMessage("Get publish product data execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("latestVersionIds, pageIndex, pageSize : ", ZnodeLogging.Components.Search.ToString(), TraceLevel.Verbose, new object[] { latestVersionIds, pageIndex, pageSize });

            //Get all product associated to the catalog categories. #Step 1
            List<SearchProduct> searchProductList = searchProductService.GetAllProducts(searchParameterModel.CatalogId, latestVersionIds, publishcategoryIds, searchParameterModel.IndexStartTime, pageIndex, pageSize, isPublishDraftProductsOnly);
                        
            //Convert product list to appropriate model for elastic search. #Step 2
            List<dynamic> productsData = ConvertProductDataToElasticModel(searchProductList);

            ZnodeLogging.LogMessage("Get publish product data execution done", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);

            return productsData;
        }

        //Generate elastic search index for the data
        public virtual void GenerateElasticSearchIndex(ElasticClient elasticClient, SearchParameterModel searchParameterModel, IndexName indexName, List<dynamic> productsData, bool isPublishDraftProductsOnly)
        {
            ZnodeLogging.LogMessage("Elastic search index process execution started.", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);

            try
            {
                DeleteBulkProduct(elasticClient, indexName, productsData);
                //insert/update product in the elasticsearch 
                InsertBulkProduct(elasticClient, indexName, productsData, isPublishDraftProductsOnly);

                ZnodeLogging.LogMessage("Elastic search index process execution done", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error, ex);
                throw new Exception("Elastic search index creation process is failed", ex);
            }
        }

        public virtual void CreateDocuments(List<ZnodePublishProductEntity> pagedProducts, ElasticClient elasticClient, string indexName)
        {
            if (pagedProducts?.Count > 0)
            {
                long indexStartTime = DateTime.Now.Ticks;
                List<dynamic> productsData = ConvertProductDataToElasticModel(searchProductService.GetElasticProducts(pagedProducts, indexStartTime));
                if (productsData?.Count > 0)
                {
                    List<List<dynamic>> productDataInChunks = HelperUtility.SplitCollectionIntoChunks<dynamic>(productsData, Convert.ToInt32(ZnodeApiSettings.SearchIndexChunkSize));

                    Parallel.For(0, productDataInChunks.Count(), index =>
                    {
                        InsertBulkProduct(elasticClient, indexName, productDataInChunks[index],false);
                    });
                }
            }
        }


        public virtual List<dynamic> ConvertProductDataToElasticModel(List<SearchProduct> pagedProducts)
        {
            List<dynamic> productsData = new List<dynamic>();

            if (pagedProducts?.Count > 0)
            {
                DataTable products = ToDataTable(pagedProducts.OrderBy(x => x.name).ToList());

                foreach (var item in products.AsEnumerable())
                {
                    // Expando objects are IDictionary<string, object>
                    IDictionary<string, object> dn = new ExpandoObject();

                    foreach (var column in products.Columns.Cast<DataColumn>())
                        dn[column.ColumnName] = !string.IsNullOrEmpty(item[column].ToString()) ? item[column] : null;

                    productsData.Add(dn);
                }
            }
            return productsData;
        }

        //Delete elastic search index.
        public virtual bool DeleteIndex(ElasticClient elasticClient, string indexName)
        {
            DeleteIndexResponse response = elasticClient.Indices.Delete(indexName);
            if (response.IsValid)
                return true;
            else
            {
                ZnodeLogging.LogMessage(response.ServerError.Error.ToString(), ZnodeLogging.Components.Search.ToString());
                if (response.ServerError.Status == 404)
                    throw new ZnodeException(ErrorCodes.NotFound, "Index does not exist.");
                return false;
            }
        }

        public virtual bool WriteSynonymsFile(ElasticClient elasticClient, int publishCatalogId, string indexName, bool isSynonymsFile, bool isAllDeleted = false)
        {
            bool result = true;
            NodesInfoResponse nodeInfo = elasticClient.Nodes.Info();
            var pathDynamicValues = nodeInfo.Nodes[(nodeInfo.Nodes.FirstOrDefault().Key)].Settings["path"];
            string configPath = pathDynamicValues["home"].Value.ToString() + Path.DirectorySeparatorChar + "config";

            if (isSynonymsFile)
                result = WriteSynonymsTextFile(elasticClient, publishCatalogId, indexName, configPath, isAllDeleted);
            return result;
        }

        //Delete unaffected products from search index
        public virtual bool DeleteProductData(ElasticClient elasticClient, IndexName indexName, long indexStartTime)
        {
            //Delete request for delete documents which does not match to current create index time.
            DeleteByQueryRequest request = new DeleteByQueryRequest<dynamic>(indexName)
            {
                Query = new QueryContainer(new BoolQuery
                {
                    MustNot = new QueryContainer[] { new TermQuery { Field = "timestamp", Value = indexStartTime.ToString() } },
                })
            };

            return elasticClient.DeleteByQuery(request)?.IsValid ?? false;
        }

        //Delete unaffected products from search index
        public virtual bool DeleteProductDataByRevisionType(ElasticClient elasticClient, IndexName indexName, string revisionType, long indexStartTime)
        {
            //Delete request for delete documents which does not match to current create index time.
            DeleteByQueryRequest request = new DeleteByQueryRequest<dynamic>(indexName)
            {
                Query = new QueryContainer(new BoolQuery
                {
                    Must = new QueryContainer[] { new MatchQuery { Field = "revisiontype", Query = revisionType } },
                    MustNot = new QueryContainer[] { new TermQuery { Field = "timestamp", Value = indexStartTime.ToString() } },
                }
                )
            };

            var response = elasticClient.DeleteByQuery(request);

            return response?.IsValid ?? false;
        }

        public virtual bool RenameIndexData(ElasticClient elasticClient, int catalogIndexId, string oldIndexName, string newIndexName)
        {
            if (IsIndexExists(elasticClient, oldIndexName))
                return DeleteAliasAndRespectiveIndices(elasticClient, oldIndexName);
            else
                return false;
        }

        //Delete unaffected products from search index
        public virtual bool DeleteProduct(ElasticClient elasticClient, IndexName indexName, string znodeProductIds, string revisionType)
        {
            //Delete request for delete documents which does not match to current create index time.
            var request = new DeleteByQueryRequest<dynamic>(indexName)
            {
                Query = new QueryContainer(new BoolQuery
                {
                    Must = new QueryContainer[] { new TermQuery { Field = ZnodeConstant.ZnodeProductId, Value = znodeProductIds } },
                })
            };

            return elasticClient.DeleteByQuery(request)?.IsValid ?? false;
        }

        //Delete unaffected products from search index
        public virtual bool DeleteProducts(ElasticClient elasticClient, IndexName indexName, IEnumerable<object> productIds, string revisionType, string versionId)
        {
            ///Delete request for delete documents which does not match to current create index time.
            var request = new DeleteByQueryRequest<dynamic>(indexName)
            {
                Query = new QueryContainer(new BoolQuery
                {
                    Must = new QueryContainer[] { new TermsQuery { Field = ZnodeConstant.ZnodeProductId, Terms = productIds } ,
                    new MatchQuery { Field = "revisiontype", Query = revisionType } ,
                    new MatchQuery { Field = "versionid", Query = versionId } },
                })
            };
            var response = elasticClient.DeleteByQuery(request);
            return response?.IsValid ?? false;
        }

        //Delete unaffected category name from search index
        public virtual bool DeleteCategoryForGivenIndex(ElasticClient elasticClient, IndexName indexName, int categoryId)
        {
            //Delete request for delete documents which does not match to current create index time.
            var request = new DeleteByQueryRequest<dynamic>(indexName)
            {
                Query = new QueryContainer(new BoolQuery
                {
                    Must = new QueryContainer[] { new TermQuery { Field = ZnodeConstant.PublishCategoryId.ToLower(), Value = categoryId.ToString() } },
                })
            };
            return elasticClient.DeleteByQuery(request)?.IsValid ?? false;
        }

        //Delete unaffected catalog category products from search index
        public virtual bool DeleteCatalogCategoryProducts(ElasticClient elasticClient, IndexName indexName, int publishCatalogId, List<int> publishCategoryIds, string revisionType, string versionId, long indexStartTime = 0)
        {
            IEnumerable<object> categoryIds = publishCategoryIds.Cast<object>();

            //Delete request for delete documents which does not match to current create index time.
            var request = new DeleteByQueryRequest<dynamic>(indexName)
            {
                Query = new QueryContainer(new BoolQuery
                {
                    Must = new QueryContainer[] { new MatchQuery { Field = "versionid", Query = versionId }, new MatchQuery { Field = "revisiontype", Query = revisionType }, new TermsQuery { Field = ZnodeConstant.PublishCategoryId, Terms = categoryIds } },
                    MustNot = new QueryContainer[] { new TermQuery { Field = "timestamp", Value = indexStartTime.ToString() } },
                })
            };
            DeleteByQueryResponse deleteByQueryResponse = elasticClient.DeleteByQuery(request);
            return deleteByQueryResponse?.IsValid ?? false;
        }


        //Checks if the index exists.
        public virtual bool IsIndexExists(ElasticClient elasticClient, IndexName indexName)
        {
            IndexExistsRequest request = new IndexExistsRequest(indexName);
            ExistsResponse result = elasticClient.Indices.Exists(indexName);
            return result.Exists;
        }

        //Set Analyzer for search settings
        public virtual void SetAnalyzers(List<string> tokenFilters, IndexSettings indexsettings)
        {
            SetSearchTimeAnalyzers(tokenFilters, indexsettings);
            SetIndexTimeAnalyzers(tokenFilters, indexsettings);
        }

        //Set search time analyzers for index.
        public virtual void SetSearchTimeAnalyzers(List<string> tokenFilters, IndexSettings indexsettings)
        {
            List<string> searchTimeTokenFilters = GetTokenFiltersForSearchTimeAnalyzer(tokenFilters);
            indexsettings.Analysis.Analyzers.Add(ZnodeConstant.StandardSearchSynonymAnalyzer, _searchIndexSettingHelper.SetCustomAnalyzer("standard", searchTimeTokenFilters, new List<string> { ZnodeConstant.MappingCharacterFilter }));
            indexsettings.Analysis.Analyzers.Add(ZnodeConstant.AutocompleteSearchAnalyzer, _searchIndexSettingHelper.SetCustomAnalyzer("keyword", new List<string> { "lowercase" }));
        }

        //Set index time analyzers for index.
        public virtual void SetIndexTimeAnalyzers(List<string> tokenFilters, IndexSettings indexsettings)
        {
            List<string> indexTimeNgramAnalyzerTokenFilters = GetTokenFiltersForIndexTimeAnalyzer(tokenFilters);
            List<string> indexTimeAnalyzerTokenFilters = indexTimeNgramAnalyzerTokenFilters.ToList();
            indexsettings.Analysis.Analyzers.Add(ZnodeConstant.IndexTimeNgramAnalyzer, _searchIndexSettingHelper.SetCustomAnalyzer("standard", indexTimeNgramAnalyzerTokenFilters, new List<string> { ZnodeConstant.MappingCharacterFilter }));
            indexsettings.Analysis.Analyzers.Add(ZnodeConstant.DidYouMeanAnalyzer, _searchIndexSettingHelper.SetCustomAnalyzer("standard", indexTimeNgramAnalyzerTokenFilters, new List<string> { "html_strip", ZnodeConstant.MappingCharacterFilter }));
            indexTimeAnalyzerTokenFilters?.Remove(ZnodeConstant.NGramTokenFilter);
            indexsettings.Analysis.Analyzers.Add(ZnodeConstant.IndexTimeAnalyzer, _searchIndexSettingHelper.SetCustomAnalyzer("standard", indexTimeAnalyzerTokenFilters, new List<string> { ZnodeConstant.MappingCharacterFilter }));
            indexsettings.Analysis.Analyzers.Add(ZnodeConstant.LowercaseAnalyzer, _searchIndexSettingHelper.SetCustomAnalyzer("keyword", new List<string> { "lowercase" }));
            indexsettings.Analysis.Analyzers.Add(ZnodeConstant.AutocompleteAnalyzer, _searchIndexSettingHelper.SetCustomAnalyzer("ngram", new List<string> { "lowercase" }));
        }

        //Set filter for analyzers
        public virtual void SetFilterForAnalyzers(List<string> tokenFilters, IndexSettings indexsettings, int publishCatalogId, PublishSearchProfileModel publishSearchProfileModel)
        {
            SetSynonymTokenFilter(tokenFilters, indexsettings, publishCatalogId);

            SetStopWordsFilter(tokenFilters, indexsettings);

            _searchIndexSettingHelper.SetNGramTokenFilter(indexsettings, publishSearchProfileModel);

            _searchIndexSettingHelper.SetPorterStemTokenFilter(indexsettings);

            _searchIndexSettingHelper.SetMappingCharacterFilter(indexsettings, publishSearchProfileModel);

            SetAutoCompleteFilter(indexsettings);
        }

        //Set Stopwords filter for analyzers.
        public virtual void SetStopWordsFilter(List<string> tokenFilters, IndexSettings indexsettings)
        {
            if (ZnodeApiSettings.EnableStopWordsForSearchIndex)
            {
                indexsettings.Analysis.TokenFilters.Add(ZnodeConstant.StopwordsTokenFilter, new StopTokenFilter
                {
                    IgnoreCase = true,
                    StopWords = "_english_"
                });
                tokenFilters.Add(ZnodeConstant.StopwordsTokenFilter);
            }
        }

        //Set  edge ngram token filter for autocomplete filter.
        public virtual void SetAutoCompleteFilter(IndexSettings indexsettings)
        {
            indexsettings.Analysis.TokenFilters.Add("autocomplete_filter", new EdgeNGramTokenFilter
            {
                MinGram = 1,
                MaxGram = 10
            });
        }


        //Delete elastic search index.
        public virtual bool DeleteElasticSearchIndex(ElasticClient elasticClient, string indexName)
        {
            if (IsIndexExists(elasticClient, indexName))
            {
                DeleteIndexResponse response = elasticClient.Indices.Delete(indexName);

                if (response.IsValid)
                    ZnodeLogging.LogMessage($"{indexName} index deleted successfully", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
                else
                    ZnodeLogging.LogMessage($"{indexName} index deletion failed", ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);

                return response.IsValid;
            }
            else
            {
                ZnodeLogging.LogMessage($"{indexName} index does not exist", ZnodeLogging.Components.Search.ToString(), TraceLevel.Info);
            }
            return true;
        }
        //Creates create index descriptor.
        public virtual CreateIndexDescriptor GetCreateIndexDescriptor(IndexName indexName, int publishCatalogId)
        {
            PublishSearchProfileModel publishSearchProfileModel = searchProductService.GetPublishSearchProfile(publishCatalogId);
            
            IndexSettings settings = CreateIndexSettings(publishCatalogId, publishSearchProfileModel);

            //We have set the field limit to 5000.
            settings.Add("index.mapping.total_fields.limit", 5000);

            return ConfigureIndex(indexName, settings, publishSearchProfileModel);
        }

        // To configure the index settings.
        public virtual CreateIndexDescriptor ConfigureIndex(IndexName indexName, IndexSettings settings, PublishSearchProfileModel publishSearchProfileModel)
        {   
            CreateIndexDescriptor createIndexDescriptor = new CreateIndexDescriptor(indexName)
                      .InitializeUsing(new IndexState() { Settings = settings })
                      .Mappings(mappings => mappings.Map<dynamic>("_default_", s => s.DateDetection(false)
                      .DynamicTemplates(dynamicTemplates => AddDynamicTemplates(dynamicTemplates))
                      .Properties(properties => ProductAttributeSettings(properties, publishSearchProfileModel))));

            return createIndexDescriptor;
        }

        // To update dynamic templates in index settings.
        protected virtual DynamicTemplateContainerDescriptor<dynamic> AddDynamicTemplates(DynamicTemplateContainerDescriptor<dynamic> dynamicTemplates)
        {
            dynamicTemplates.DynamicTemplate("strings", dt => dt.Match("*")
                      .PathUnmatch("attributes.*")
                      .MatchMappingType("string").Mapping(t => t
                      .Text(d => d
                      .Fields(fi => fi.Text(te => te.Name("lowercase")
                      .Analyzer(ZnodeConstant.LowercaseAnalyzer)).Keyword(ss => ss.Name("keyword")).Text(at => at.Name("autocomplete").Analyzer(ZnodeConstant.AutocompleteAnalyzer).Fielddata(true)))
                      .Analyzer(ZnodeConstant.IndexTimeAnalyzer)
                      .SearchAnalyzer(ZnodeConstant.StandardSearchSynonymAnalyzer))))
                      .DynamicTemplate("nested", nest => nest.PathMatch("attributes.*").MatchMappingType("string").Mapping(x => x.Text(te => te.Index(false))));

            return dynamicTemplates;
        }

        // To updated the index setting properties based on searchable attributes configured in the search profile.
        protected PropertiesDescriptor<dynamic> ProductAttributeSettings(PropertiesDescriptor<dynamic> properties, PublishSearchProfileModel publishSearchProfileModel)
        {
            List<SearchAttributesModel> searchAttributes = JsonConvert.DeserializeObject<List<SearchAttributesModel>>(publishSearchProfileModel?.SearchProfileAttributeMappingJson)?.ToList();

            searchAttributes = searchAttributes?.Where(x => x.IsNgramEnabled)?.ToList();

            if (!(searchAttributes?.Any(x => string.Equals(x.AttributeCode, ZnodeConstant.ProductName, StringComparison.InvariantCultureIgnoreCase))).GetValueOrDefault())
                searchAttributes?.Add(new SearchAttributesModel()
                {
                    AttributeCode = ZnodeConstant.ProductName,
                    IsNgramEnabled = false
                });            

            foreach (SearchAttributesModel searchAttribute in searchAttributes)
                AddAttributeSetting(properties, searchAttribute);
            return properties.Text(did => did.Name("didyoumean").Analyzer(ZnodeConstant.DidYouMeanAnalyzer));
        }

        // To add the property in index settings for a searchable attribute.
        protected void AddAttributeSetting(PropertiesDescriptor<dynamic> properties, SearchAttributesModel searchAttribute)
        {
            bool isFieldData = string.Equals(searchAttribute.AttributeCode, ZnodeConstant.ProductName, StringComparison.InvariantCultureIgnoreCase);
            properties.Text(productname => productname.Name(searchAttribute.AttributeCode.ToLower())
            .Fields(f => SetPropertyFields(f, searchAttribute).Text(te => te.Name("lowercase")
            .Analyzer(ZnodeConstant.LowercaseAnalyzer).Fielddata(isFieldData))
            .Text(at => at.Name("autocomplete").Analyzer(ZnodeConstant.AutocompleteAnalyzer).Fielddata(true)))
            .Analyzer(searchAttribute.IsNgramEnabled ? ZnodeConstant.IndexTimeNgramAnalyzer : ZnodeConstant.IndexTimeAnalyzer)
            .SearchAnalyzer(ZnodeConstant.StandardSearchSynonymAnalyzer));
        }

        // To set the property fields.
        protected PropertiesDescriptor<dynamic> SetPropertyFields(PropertiesDescriptor<dynamic> propertyDescriptor, SearchAttributesModel searchAttribute)
        {
            if (string.Equals(searchAttribute.AttributeCode, ZnodeConstant.ProductSKU, StringComparison.InvariantCultureIgnoreCase) ||
                string.Equals(searchAttribute.AttributeCode, ZnodeConstant.Brand, StringComparison.InvariantCultureIgnoreCase))
                return propertyDescriptor.Keyword(property => property.Name("keyword"));

            return propertyDescriptor.Text(ss => ss.Name("keyword"));
        }

        //Creates index setting.
        public virtual IndexSettings CreateIndexSettings(int publishCatalogId, PublishSearchProfileModel publishSearchProfileModel)
        {
            //Get default token filters.
            List<string> tokenFilters = ZnodeApiSettings.DefaultTokenFilters.Split(',').ToList();

            //create analysis object
            IndexSettings indexsettings = new IndexSettings
            {
                Analysis = new Analysis
                {
                    TokenFilters = new TokenFilters(),
                    Analyzers = new Analyzers(),
                    Tokenizers = new Tokenizers()
                }
            };

            int minGram = ZnodeConstant.ngramMinimumTokenLength, maxGram = ZnodeConstant.ngramMaximumTokenLength;

            if (HelperUtility.IsNotNull(publishSearchProfileModel))
            {
                Int32.TryParse(_searchIndexSettingHelper.GetSearchFeatureByFeatureCode(publishSearchProfileModel?.FeaturesList, ZnodeConstant.MinGram), out minGram);
                Int32.TryParse(_searchIndexSettingHelper.GetSearchFeatureByFeatureCode(publishSearchProfileModel?.FeaturesList, ZnodeConstant.MaxGram), out maxGram);
            }

            //The default difference between MinNGram and MaxNGram is set as 1 to set max difference need to add difference in index setting. 
            indexsettings.Add(UpdatableIndexSettings.MaxNGramDiff, maxGram - minGram);

            //The default difference between MaxShingleDiff and MaxShingleDiff is set as 1 to set max difference need to add difference in index setting. 
            
            SetFilterForAnalyzers(tokenFilters, indexsettings, publishCatalogId, publishSearchProfileModel);

            _searchIndexSettingHelper.SetTokenizers(indexsettings, publishSearchProfileModel);

            SetAnalyzers(tokenFilters, indexsettings);

            return indexsettings;
        }

        //Gets synonym token filter.
        public virtual void SetSynonymTokenFilter(List<string> tokenFilters, IndexSettings indexsettings, int publishCatalogId = 0, bool isPublishSynonyms = false, bool isAllDeleted = false)
        {
            List<ZnodeSearchSynonym> synonymList = searchProductService.GetSynonymsData(publishCatalogId);

            List<string> formattedSynonymList = new List<string>();

            if (!isAllDeleted && isPublishSynonyms && synonymList.Count <= 0)
                throw new ZnodeException(ErrorCodes.ProcessingFailed, "no record found for processing synonyms.");

            if (isAllDeleted)
                formattedSynonymList.Add("");

            if (synonymList.Count > 0)
            {
                foreach (ZnodeSearchSynonym item in synonymList)
                {
                    if (item.IsBidirectional.GetValueOrDefault())
                        SetBidirectionalSynonyms(formattedSynonymList, item.OriginalTerm, item.ReplacedBy);
                    else
                        SetUnidirectionalSynonyms(formattedSynonymList, item.OriginalTerm, item.ReplacedBy);
                }
            }

            if (formattedSynonymList?.Count > 0 && Equals(ZnodeApiSettings.UseSynonym, "true") && formattedSynonymList?.Count > 0)
            {
                //Create synonym token filter.
                SynonymTokenFilter synonymFilter = new SynonymTokenFilter();
                synonymFilter.Synonyms = formattedSynonymList;
                //Add synonym token filter in indexsetting.
                indexsettings.Analysis.TokenFilters = new TokenFilters();
                indexsettings.Analysis.TokenFilters.Add(ZnodeConstant.SynonymTokenFilter, synonymFilter);
                tokenFilters.Add(ZnodeConstant.SynonymTokenFilter);
            }
        }

        // To set bidirectional synonyms.
        protected virtual void SetBidirectionalSynonyms(List<string> synonymsList, string originalTerms, string replacedByTerms)
        {
            if (HelperUtility.IsNotNull(synonymsList))
            {
                // To add bidirectional synonyms.
                synonymsList.Add($"{originalTerms},{replacedByTerms}");
            }
        }

        // To set unidirectional synonyms.
        protected virtual void SetUnidirectionalSynonyms(List<string> synonymsList, string originalTerms, string replacedByTerms)
        {
            // If more than one replaced by terms are available then for each replaced by term separate mapping will be added in the index.
            List<string> replacedByTermList = replacedByTerms?.Split(',')?.ToList();
            if (HelperUtility.IsNotNull(synonymsList) && HelperUtility.IsNotNull(replacedByTermList))
            {
                foreach (string replacedByTerm in replacedByTermList)
                {
                    // To add unidirectional synonyms.
                    synonymsList.Add($"{replacedByTerm} => {originalTerms}, {replacedByTerm}");
                }
            }
        }

        //Write Synonyms file
        public virtual bool WriteSynonymsTextFile(ElasticClient elasticClient, int publishCatalogId, string indexName, string configPath, bool isAllDeleted = false)
        {
            try
            {
                if (IsIndexExists(elasticClient, indexName))
                {
                    IndexSettings updatedIndexSettings = GetNewlyInitializedIndexSettingsInstance();

                    List<string> tokenFilters = new List<string>();

                    SetSynonymTokenFilter(tokenFilters, updatedIndexSettings, publishCatalogId, true, isAllDeleted);

                    IIndexSettings existingIndexSettings = GetSettingsFromIndex(elasticClient, indexName);

                    CustomAnalyzer indexTimeNgramAnalyzer = GetAnalyzerFromIndexSettings(existingIndexSettings, ZnodeConstant.IndexTimeNgramAnalyzer);
                    SetSynonymsTokenFilterInAnalyzer(indexTimeNgramAnalyzer);

                    CustomAnalyzer indexTimeAnalyzer = GetAnalyzerFromIndexSettings(existingIndexSettings, ZnodeConstant.IndexTimeAnalyzer);
                    SetSynonymsTokenFilterInAnalyzer(indexTimeAnalyzer);

                    CustomAnalyzer didYouMeanAnalyzer = GetAnalyzerFromIndexSettings(existingIndexSettings, ZnodeConstant.DidYouMeanAnalyzer);
                    SetSynonymsTokenFilterInAnalyzer(didYouMeanAnalyzer);

                    // To set updated analyzers(having synonyms token filter) in the IndexSettings instance.
                    updatedIndexSettings.Analysis.Analyzers.Add(ZnodeConstant.IndexTimeNgramAnalyzer, indexTimeNgramAnalyzer);
                    updatedIndexSettings.Analysis.Analyzers.Add(ZnodeConstant.IndexTimeAnalyzer, indexTimeAnalyzer);
                    updatedIndexSettings.Analysis.Analyzers.Add(ZnodeConstant.DidYouMeanAnalyzer, didYouMeanAnalyzer);

                    return UpdateIndexSettings(elasticClient, indexName, updatedIndexSettings);
                }
                else
                    throw new ZnodeException(ErrorCodes.NotFound, "Search index does not exist.");
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex.Message, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
                throw;
            }
        }

        // To get the newly initialized instance of IndexSettings class.
        protected virtual IndexSettings GetNewlyInitializedIndexSettingsInstance()
        {
            return new IndexSettings
            {
                Analysis = new Analysis
                {
                    TokenFilters = new TokenFilters(),
                    Analyzers = new Analyzers(),
                }
            };
        }

        // To update the index settings of a particular index.
        protected virtual bool UpdateIndexSettings(ElasticClient elasticClient, string indexName, IndexSettings updatedIndexSettings)
        {
            // To close the index before updating the index settings. Elastic search will not allow modifications in the settings of an open index.
            elasticClient?.Indices?.Close(indexName);

            UpdateIndexSettingsResponse updateIndexSettingsResponse = elasticClient?.Indices?.UpdateSettings(new UpdateIndexSettingsRequest(indexName) { IndexSettings = updatedIndexSettings });

            // To open the closed index as a closed index will not allow read or write operations.
            elasticClient?.Indices?.Open(indexName);

            // Returns true if response returned a valid HTTP status code else false will be returned.
            return (updateIndexSettingsResponse?.IsValid).GetValueOrDefault();
        }

        // To get the index settings of a particular index.
        protected virtual IIndexSettings GetSettingsFromIndex(ElasticClient elasticClient, string indexName)
        {
            GetIndexSettingsResponse indexResponse = elasticClient?.Indices?.GetSettings(Indices.Parse(indexName));

            return indexResponse?.Indices?.Values?.FirstOrDefault()?.Settings;
        }

        // To get the search analyzer from index settings based on the name of the analyzer.
        protected virtual CustomAnalyzer GetAnalyzerFromIndexSettings(IIndexSettings indexSettings, string analyzerName)
        {
            CustomAnalyzer indexedAnalyzer = new CustomAnalyzer();
            if (HelperUtility.IsNotNull(indexSettings?.Analysis?.Analyzers?.FirstOrDefault(x => string.Equals(x.Key, analyzerName, StringComparison.InvariantCultureIgnoreCase))))
                indexedAnalyzer = (CustomAnalyzer)indexSettings.Analysis.Analyzers.FirstOrDefault(x => string.Equals(x.Key, analyzerName, StringComparison.InvariantCultureIgnoreCase)).Value;

            return indexedAnalyzer;
        }

        // To set the synonyms token filter in the analyzer.
        protected virtual void SetSynonymsTokenFilterInAnalyzer(CustomAnalyzer analyzer)
        {
            if (HelperUtility.IsNotNull(analyzer))
            {
                List<string> tokenFilters = analyzer.Filter?.ToList();

                if (HelperUtility.IsNull(tokenFilters))
                    tokenFilters = new List<string> { ZnodeConstant.SynonymTokenFilter };
                else if (!tokenFilters.Contains(ZnodeConstant.SynonymTokenFilter))
                    tokenFilters?.Insert(tokenFilters.FindIndex(x => x == ZnodeConstant.StopwordsTokenFilter), ZnodeConstant.SynonymTokenFilter);

                analyzer.Filter = tokenFilters;
            }
        }

        public virtual DataTable ToDataTable(List<SearchProduct> products)
        {
            DataTable tmp = new DataTable();

            //Tables created with columns
            foreach (SearchProduct product in products)
            {
                foreach (SearchAttributes attribute in product.searchableattributes)
                {
                    DataColumnCollection columns = tmp.Columns;
                    if (!columns.Contains(attribute.attributecode))
                    {
                        if (attribute.isfacets)
                        {
                            tmp.Columns.Add(attribute.attributecode.ToLower(), typeof(string[]));
                            continue;
                        }
                        else if (attribute.attributetypename == "Number")
                        {
                            tmp.Columns.Add(attribute.attributecode.ToLower(), typeof(double));
                            continue;
                        }
                        else
                            tmp.Columns.Add(attribute.attributecode.ToLower());
                    }
                }
            }
            tmp.Columns.Add("id", typeof(string));
            tmp.Columns.Add("znodecatalogid", typeof(int));
            tmp.Columns.Add(ZnodeConstant.ParentCategoryIds, typeof(List<int>));     
            tmp.Columns.Add("localeid", typeof(int));
            tmp.Columns.Add("categoryid", typeof(int));
            tmp.Columns.Add("categoryname");
            tmp.Columns.Add("znodeproductid", typeof(int));
            tmp.Columns.Add(ZnodeConstant.productPrice, typeof(decimal));
            tmp.Columns.Add("isactive", typeof(bool));
            tmp.Columns.Add("productindex", typeof(int));
            tmp.Columns.Add("displayorder", typeof(int));
            tmp.Columns.Add("timestamp", typeof(long));
            tmp.Columns.Add("autocompleteproductname");
            tmp.Columns.Add("autocompletesku");
            tmp.Columns.Add("didyoumean");
            tmp.Columns.Add("name");
            tmp.Columns.Add("versionid", typeof(int));
            tmp.Columns.Add("rating", typeof(decimal));
            tmp.Columns.Add("totalreviewcount", typeof(int));
            tmp.Columns.Add("attributes", typeof(object));
            tmp.Columns.Add("revisiontype", typeof(string));
            tmp.Columns.Add("brands", typeof(string[]));
            tmp.Columns.Add("salesprice");
            tmp.Columns.Add("retailprice");
            tmp.Columns.Add("culturecode");
            tmp.Columns.Add("currencycode");
            tmp.Columns.Add("currencysuffix");
            tmp.Columns.Add("seodescription");
            tmp.Columns.Add("seokeywords");
            tmp.Columns.Add("seotitle");
            tmp.Columns.Add("seourl");
            tmp.Columns.Add("imagesmallpath");
            tmp.Columns.Add("ElasticSearchEvent", typeof(int));
            //Add data of each product
            foreach (SearchProduct product in products)
            {
                try
                {
                    DataRow dr = tmp.NewRow();
                    foreach (SearchAttributes attribute in product.searchableattributes)
                    {
                        try
                        {
                            foreach (DataColumn column in tmp.Columns)
                            {
                                if (
                                    (attribute.attributecode.Equals(ZnodeConstant.OutOfStockOptions, StringComparison.InvariantCultureIgnoreCase) && column.ColumnName.Equals(ZnodeConstant.OutOfStockOptions, StringComparison.InvariantCultureIgnoreCase))
                                    ||
                                    (attribute.attributecode.Equals(ZnodeConstant.ProductType, StringComparison.InvariantCultureIgnoreCase) && column.ColumnName.Equals(ZnodeConstant.ProductType, StringComparison.InvariantCultureIgnoreCase)))
                                {
                                    if (attribute.selectvalues.Count > 0)
                                        dr[column.ColumnName] = attribute.selectvalues.FirstOrDefault()?.code;
                                    continue;
                                }
                                if (attribute.attributecode.Equals(ZnodeConstant.Brand, StringComparison.InvariantCultureIgnoreCase)
                                    && column.ColumnName.Equals(ZnodeConstant.Brand, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    dr[column.ColumnName] = attribute.selectvalues?.Select(y => y.value).ToArray();
                                    continue;
                                }// Added if condition to add brandcode in place of brandname to fetch product using brandcode 

                                if (attribute.attributecode.Equals(column.ColumnName, StringComparison.InvariantCultureIgnoreCase))
                                {
                                    if (attribute.selectvalues?.Count > 0)
                                    {
                                        if (column.DataType == typeof(string[]))
                                            dr[column.ColumnName] = attribute.selectvalues.Select(x => x.value).ToArray();
                                        else
                                            dr[column.ColumnName] = attribute.selectvalues.FirstOrDefault().value;
                                    }
                                    else if (column.DataType == typeof(double))
                                        dr[column.ColumnName] = string.IsNullOrEmpty(attribute.attributevalues) ? 0 : Convert.ToDouble(attribute.attributevalues);
                                    else
                                        dr[column.ColumnName] = string.IsNullOrEmpty(attribute.attributevalues) ? null : attribute.attributevalues.TrimEnd();
                                    continue;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            ZnodeLogging.LogMessage($"Error occurred for product sku: {product.sku} and attribute code : {attribute.attributecode}, error details are {ex.Message}", "Elasticsearch", TraceLevel.Error);
                            ZnodeLogging.LogMessage(ex.Message, "Elasticsearch", TraceLevel.Error, ex);
                        }
                    }

                    if (product.brands.Count > 0) dr["brands"] = product.brands.Select(x => x.brandcode).ToArray();
                    dr["id"] = product.publishproductentityid;
                    dr["znodecatalogid"] = product.catalogid;
                    dr[ZnodeConstant.ParentCategoryIds] = product.parentcategoryids;        
                    dr["localeid"] = product.localeid;
                    dr["categoryid"] = product.categoryid;
                    dr["categoryname"] = product.categoryname;
                    dr["znodeproductid"] = product.znodeproductid;
                    dr["isactive"] = product.isactive;
                    dr["productindex"] = product.productindex;
                    dr["rating"] = product.rating;
                    dr["totalreviewcount"] = product.totalreviewcount;
                    dr["displayorder"] = product.displayorder;
                    dr["name"] = product.name;
                    dr["timestamp"] = product.timestamp;
                    dr["autocompleteproductname"] = product.name;
                    dr["autocompletesku"] = product.sku;
                    dr["didyoumean"] = product.name;
                    dr["versionid"] = product.version;
                    dr["attributes"] = product.attributes;
                    dr["revisiontype"] = product.revisionType;
                    dr[ZnodeConstant.productPrice] = product.productprice;
                    dr["salesprice"] = product.salesprice;
                    dr["retailprice"] = product.retailprice;
                    dr["culturecode"] = product.culturecode;
                    dr["currencycode"] = product.currencycode;
                    dr["currencysuffix"] = product.currencysuffix;
                    dr["seodescription"] = product.seodescription;
                    dr["seokeywords"] = product.seokeywords;
                    dr["seotitle"] = product.seotitle;
                    dr["seourl"] = product.seourl;
                    dr["imagesmallpath"] = product.imagesmallpath;
                    dr["ElasticSearchEvent"] = (product.ElasticSearchEvent == null ? 1: product?.ElasticSearchEvent);
                    tmp.Rows.Add(dr);
                }
                catch (Exception ex)
                {
                    ZnodeLogging.LogMessage($"Error occurred for product sku: {product.sku}, error details are {ex.Message}", "Elasticsearch", TraceLevel.Error);
                    ZnodeLogging.LogMessage(ex.Message, "Elasticsearch", TraceLevel.Error, ex);
                }
            }

            return tmp;
        }

        #endregion Public Methods

        #region Protected methods
        // To get the list of token filters for search time analyzers.
        protected virtual List<string> GetTokenFiltersForSearchTimeAnalyzer(List<string> tokenFilters)
        {
            List<string> searchTimeTokenFilters = tokenFilters.ToList();

            // Tokens will be generated based on the sequence of configured token filters.
            searchTimeTokenFilters?.Insert(searchTimeTokenFilters.FindIndex(x => x == ZnodeConstant.StopwordsTokenFilter), ZnodeConstant.PorterStemTokenFilter);

            // To avoid using synonyms token filter for search time analyzer.
            searchTimeTokenFilters.Remove(ZnodeConstant.SynonymTokenFilter);

            return searchTimeTokenFilters;
        }

        // To get the list of token filters for index time analyzers.
        protected virtual List<string> GetTokenFiltersForIndexTimeAnalyzer(List<string> tokenFilters)
        {
            List<string> indexTimeTokenFilters = tokenFilters.ToList();

            // Tokens will be generated based on the sequence of configured token filters.
            indexTimeTokenFilters?.Insert(indexTimeTokenFilters.FindIndex(x => x == ZnodeConstant.StopwordsTokenFilter), ZnodeConstant.PorterStemTokenFilter);

            // Tokens will be generated based on the sequence of configured token filters.
            // This code statement has been added to keep the sequence as it is after removing the Edge NGram token filter from index time analyzers. 
            indexTimeTokenFilters?.Insert(indexTimeTokenFilters.FindIndex(x => x == ZnodeConstant.StopwordsTokenFilter), ZnodeConstant.NGramTokenFilter);

            return indexTimeTokenFilters;
        }

        protected virtual BulkResponse DeleteBulkProduct(ElasticClient elasticClient, IndexName indexName, List<dynamic> productsData)
        {
            BulkDescriptor deletDescriptor = new BulkDescriptor();
            BulkResponse _response = new BulkResponse();
            List<dynamic> deleteProductList = productsData.Where(a => a.ElasticSearchEvent == 2).ToList();
            //Adding product list in the BulkDescriptor
            foreach (var doc1 in deleteProductList)
            {
                deletDescriptor.Delete<object>(i => i
                    .Index(indexName)
                    .Id((Id)doc1.id));
            }
            //Inserting the the bulk product data in the elasticsearch
            if (deleteProductList.Count > 0)
            {
                _response = elasticClient.Bulk(deletDescriptor);
                if (!_response.IsValid)
                    throw new Exception($"Product insertion failed. {_response.ItemsWithErrors?.FirstOrDefault()?.Error?.Reason}");
            }
            return _response;
        }

        protected virtual BulkResponse InsertBulkProduct(ElasticClient elasticClient, IndexName indexName, List<dynamic> productsData, bool isPublishDraftProductsOnly)
        {
            // Insert all product document in elastic search/
             BulkDescriptor insertOrUpdateDescriptor = new BulkDescriptor();
            BulkResponse _response = new BulkResponse();

            List<dynamic> insertUpdateProductList = isPublishDraftProductsOnly?productsData.Where(a => a.ElasticSearchEvent == 1).ToList(): productsData?.Where(a => a.ElasticSearchEvent != 2).ToList();

            //Adding product list in the BulkDescriptor
            foreach (var doc in insertUpdateProductList)
            {
                insertOrUpdateDescriptor.Index<object>(i => i
                    .Index(indexName)
                    .Id((Id)doc.id)
                    .Document((object)doc)).Refresh(Elasticsearch.Net.Refresh.True);
            }
            //Inserting the the bulk product data in the elasticsearch
            if (insertUpdateProductList.Count > 0)
            {
                _response = elasticClient.Bulk(insertOrUpdateDescriptor);
                if (!_response.IsValid)
                {
                    ZnodeLogging.LogMessage(_response.ItemsWithErrors?.First()?.Error?.CausedBy?.Reason, "Elasticsearch", TraceLevel.Error, _response.ItemsWithErrors?.First()?.Error);
                    throw new Exception($"Create Index failed. {_response.ItemsWithErrors?.FirstOrDefault()?.Error?.Reason}");
                }
            }

            return _response;
        }
        #endregion Protected methods
    }
}