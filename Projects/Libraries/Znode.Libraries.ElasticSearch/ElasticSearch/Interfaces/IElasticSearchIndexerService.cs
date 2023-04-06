using Nest;

using System.Collections.Generic;
using System.Data;
using Znode.Engine.Api.Models;

using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Search;

namespace Znode.Libraries.ElasticSearch
{
    public interface IElasticSearchIndexerService
    {
        /// <summary>
        /// Add the product descriptor
        /// </summary>
        /// <param name="elasticClient">elasticClient</param>
        /// <param name="indexName">indexName</param>
        /// <param name="publishCatalogId">publishCatalogId</param>
        /// <returns>Return response from elasticsearch</returns>
        CreateIndexDescriptor AddProductDocuments(ElasticClient elasticClient, IndexName indexName, int publishCatalogId);

        /// <summary>
        /// Create elastic index for the catalog data
        /// </summary>
        /// <param name="elasticClient">elasticClient</param>
        /// <param name="indexName">indexName</param>
        /// <param name="searchParameterModel">searchParameterModel</param>
        void CreateIndex(ElasticClient elasticClient, IndexName indexName, SearchParameterModel searchParameterModel);

        /// <summary>
        /// Get publish product data in the require form for the elastic search indexing process
        /// </summary>
        /// <param name="searchParameterModel">searchParameterModel</param>
        /// <param name="latestVersionIds">latestVersionIds</param>
        /// <param name="publishcategoryIds">publishcategoryIds</param>
        /// <param name="pageIndex">pageIndex</param>
        /// <param name="pageSize">pageSize</param>
        /// <param name="isPublishDraftProductsOnly">To specify whether to publish only draft products or not.</param>
        /// <returns>Return product data list</returns>
        List<dynamic> GetPublishProductData(SearchParameterModel searchParameterModel, int[] latestVersionIds, IEnumerable<int> publishcategoryIds, int pageIndex, int pageSize, bool isPublishDraftProductsOnly);

        /// <summary>
        /// Generate elastic search index
        /// </summary>
        /// <param name="elasticClient">elasticClient</param>
        /// <param name="searchParameterModel">searchParameterModel</param>
        /// <param name="indexName">indexName</param>
        /// <param name="productsData">productsData</param>
        /// <param name="isPublishDraftProductsOnly">To specify whether to publish only draft products or not.</param>
        void GenerateElasticSearchIndex(ElasticClient elasticClient, SearchParameterModel searchParameterModel, IndexName indexName, List<dynamic> productsData, bool isPublishDraftProductsOnly);

        /// <summary>
        /// Generate elastic search index
        /// </summary>
        /// <param name="pagedProducts">pagedProducts</param>
        /// <param name="elasticClient">elasticClient</param>
        /// <param name="indexName">indexName</param>
        void CreateDocuments(List<ZnodePublishProductEntity> pagedProducts, ElasticClient elasticClient, string indexName);

        /// <summary>
        /// Convert product data into respective elasticsearch model
        /// </summary>
        /// <param name="pagedProducts">pagedProducts</param>
        /// <returns>Return data list</returns>
        List<dynamic> ConvertProductDataToElasticModel(List<SearchProduct> pagedProducts);

        /// <summary>
        /// Delete elastic search index
        /// </summary>
        /// <param name="elasticClient">elasticClient</param>
        /// <param name="indexName">indexName</param>
        /// <returns>Return true or false based on response by elastic client</returns>
        bool DeleteIndex(ElasticClient elasticClient, string indexName);

        /// <summary>
        /// Write in synonyms file
        /// </summary>
        /// <param name="elasticClient">elasticClient</param>
        /// <param name="publishCatalogId">publishCatalogId</param>
        /// <param name="indexName">indexName</param>
        /// <param name="isSynonymsFile">isSynonymsFile</param>
        /// <param name="isAllDeleted">isAllDeleted</param>
        /// <returns>Return true or false</returns>
        bool WriteSynonymsFile(ElasticClient elasticClient, int publishCatalogId, string indexName, bool isSynonymsFile, bool isAllDeleted = false);

        /// <summary>
        /// Delete unaffected products from search index
        /// </summary>
        /// <param name="elasticClient">elasticClient</param>
        /// <param name="indexName">indexName</param>
        /// <param name="indexStartTime">indexStartTime</param>
        /// <returns>Return true or false</returns>
        bool DeleteProductData(ElasticClient elasticClient, IndexName indexName, long indexStartTime);

        /// <summary>
        /// Delete unaffected products from search index based on revision type
        /// </summary>
        /// <param name="elasticClient">elasticClient</param>
        /// <param name="indexName">indexName</param>
        /// <param name="revisionType">revisionType</param>
        /// <param name="indexStartTime">indexStartTime</param>
        /// <returns>Return true or false</returns>
        bool DeleteProductDataByRevisionType(ElasticClient elasticClient, IndexName indexName, string revisionType, long indexStartTime);

        /// <summary>
        ///  Rename index name in elastic search
        /// </summary>
        /// <param name="elasticClient">elasticClient</param>
        /// <param name="catalogIndexId">catalogIndexId</param>
        /// <param name="oldIndexName">oldIndexName</param>
        /// <param name="newIndexName">newIndexName</param>
        /// <returns>Return true or false</returns>
        bool RenameIndexData(ElasticClient elasticClient, int catalogIndexId, string oldIndexName, string newIndexName);

        /// <summary>
        /// Delete unaffected products from search index
        /// </summary>
        /// <param name="elasticClient">elasticClient</param>
        /// <param name="indexName">indexName</param>
        /// <param name="znodeProductIds">znodeProductIds</param>
        /// <param name="revisionType">revisionType</param>
        /// <returns>Return true or false</returns>
        bool DeleteProduct(ElasticClient elasticClient, IndexName indexName, string znodeProductIds, string revisionType);

        /// <summary>
        /// Delete unaffected products from search index
        /// </summary>
        /// <param name="elasticClient">elasticClient</param>
        /// <param name="indexName">indexName</param>
        /// <param name="productIds">productIds</param>
        /// <param name="revisionType">revisionType</param>
        /// <param name="versionId">versionId</param>
        /// <returns>Return true or false</returns>
        bool DeleteProducts(ElasticClient elasticClient, IndexName indexName, IEnumerable<object> productIds, string revisionType, string versionId);

        /// <summary>
        /// Delete unaffected category name from search index
        /// </summary>
        /// <param name="elasticClient">elasticClient</param>
        /// <param name="indexName">indexName</param>
        /// <param name="categoryId">categoryId</param>
        /// <returns>Return true or false</returns>
        bool DeleteCategoryForGivenIndex(ElasticClient elasticClient, IndexName indexName, int categoryId);

        /// <summary>
        /// Delete unaffected catalog category products from search index
        /// </summary>
        /// <param name="elasticClient">elasticClient</param>
        /// <param name="indexName">indexName</param>
        /// <param name="publishCatalogId">publishCatalogId</param>
        /// <param name="publishCategoryIds">publishCategoryIds</param>
        /// <param name="revisionType">revisionType</param>
        /// <param name="versionId">versionId</param>
        /// <param name="indexStartTime">indexStartTime</param>
        /// <returns>Return true or false</returns>
        bool DeleteCatalogCategoryProducts(ElasticClient elasticClient, IndexName indexName, int publishCatalogId, List<int> publishCategoryIds, string revisionType, string versionId, long indexStartTime = 0);

        /// <summary>
        /// Checks if the index exists
        /// </summary>
        /// <param name="elasticClient">elasticClient</param>
        /// <param name="indexName">indexName</param>
        /// <returns>Return true or false</returns>
        bool IsIndexExists(ElasticClient elasticClient, IndexName indexName);

        /// <summary>
        /// Set Analyzer for search settings
        /// </summary>
        /// <param name="tokenFilters">tokenFilters</param>
        /// <param name="indexsettings">indexsettings</param>
        void SetAnalyzers(List<string> tokenFilters, IndexSettings indexsettings);

        /// <summary>
        /// Set search time analyzers for index
        /// </summary>
        /// <param name="tokenFilters">tokenFilters</param>
        /// <param name="indexsettings">indexsettings</param>
        void SetSearchTimeAnalyzers(List<string> tokenFilters, IndexSettings indexsettings);

        /// <summary>
        /// Set index time analyzers for index
        /// </summary>
        /// <param name="tokenFilters">tokenFilters</param>
        /// <param name="indexsettings">indexsettings</param>
        void SetIndexTimeAnalyzers(List<string> tokenFilters, IndexSettings indexsettings);

        /// <summary>
        /// Set filter for analyzers
        /// </summary>
        /// <param name="tokenFilters">tokenFilters</param>
        /// <param name="indexsettings">indexsettings</param>
        /// <param name="publishCatalogId">publishCatalogId</param>
        /// <param name="publishSearchProfileModel">PublishSearchProfileModel</param>
        void SetFilterForAnalyzers(List<string> tokenFilters, IndexSettings indexsettings, int publishCatalogId, PublishSearchProfileModel publishSearchProfileModel);

        /// <summary>
        /// Set Stopwords filter for analyzers
        /// </summary>
        /// <param name="tokenFilters">tokenFilters</param>
        /// <param name="indexsettings">indexsettings</param>
        void SetStopWordsFilter(List<string> tokenFilters, IndexSettings indexsettings);

        /// <summary>
        /// Set  edge ngram token filter for autocomplete filter
        /// </summary>
        /// <param name="indexsettings">indexsettings</param>
        void SetAutoCompleteFilter(IndexSettings indexsettings);

        /// <summary>
        /// Delete elastic search index
        /// </summary>
        /// <param name="elasticClient">elasticClient</param>
        /// <param name="indexName">indexName</param>
        /// <returns>Return true or false</returns>
        bool DeleteElasticSearchIndex(ElasticClient elasticClient, string indexName);

        /// <summary>
        /// Convert product data into data table
        /// </summary>
        /// <param name="products">products</param>
        /// <returns>Return data table</returns>
        DataTable ToDataTable(List<SearchProduct> products);

        /// <summary>
        /// Configure the elastic search index setting
        /// </summary>
        /// <param name="indexName">indexName</param>
        /// <param name="settings">settings</param>
        /// <param name="publishSearchProfileModel">PublishSearchProfileModel</param>
        /// <returns>Return setting</returns>
        CreateIndexDescriptor ConfigureIndex(IndexName indexName, IndexSettings settings, PublishSearchProfileModel publishSearchProfileModel);

        /// <summary>
        /// Creates create index descriptor
        /// </summary>
        /// <param name="indexName">indexName</param>
        /// <param name="publishCatalogId">publishCatalogId</param>
        /// <returns>Return create index descriptor setting</returns>
        CreateIndexDescriptor GetCreateIndexDescriptor(IndexName indexName, int publishCatalogId);

        /// <summary>
        /// Creates index setting
        /// </summary>
        /// <param name="publishCatalogId">publishCatalogId</param>
        /// <param name="publishSearchProfileModel">PublishSearchProfileModel</param>
        /// <returns>Return index setting object</returns>
        IndexSettings CreateIndexSettings(int publishCatalogId, PublishSearchProfileModel publishSearchProfileModel);

        /// <summary>
        /// Set synonym token filter
        /// </summary>
        /// <param name="tokenFilters">tokenFilters</param>
        /// <param name="indexsettings">indexsettings</param>
        /// <param name="publishCatalogId">publishCatalogId</param>
        /// <param name="isPublishSynonyms">isPublishSynonyms</param>
        /// <param name="isAllDeleted">isAllDeleted</param>
        void SetSynonymTokenFilter(List<string> tokenFilters, IndexSettings indexsettings, int publishCatalogId = 0, bool isPublishSynonyms = false, bool isAllDeleted = false);

        /// <summary>
        /// Write synonyms file
        /// </summary>
        /// <param name="elasticClient">elasticClient</param>
        /// <param name="publishCatalogId">publishCatalogId</param>
        /// <param name="indexName">indexName</param>
        /// <param name="configPath">configPath</param>
        /// <param name="isAllDeleted">isAllDeleted</param>
        /// <returns>Return true or false</returns>
        bool WriteSynonymsTextFile(ElasticClient elasticClient, int publishCatalogId, string indexName, string configPath, bool isAllDeleted = false);

        /// <summary>
        /// To get the index names which are pointed by the specified alias.
        /// </summary>
        /// <param name="elasticClient">ElasticClient</param>
        /// <param name="alias">alias</param>
        /// <returns>IReadOnlyCollection<string></returns>
        IReadOnlyCollection<string> GetIndicesPointingToAlias(ElasticClient elasticClient, string alias);
    }
}
