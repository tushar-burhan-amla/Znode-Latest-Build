using Nest;

using System;
using System.Collections.Generic;
using System.Diagnostics;

using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Search;
using System.Linq;

using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Libraries.ElasticSearch
{
    public class DefaultDataService : IDefaultDataService
    {
        #region Protected Variables

        protected readonly ElasticClient elasticClient;
        protected readonly IElasticSearchIndexerService elasticSearchIndexerService;

        #endregion Protected Variables

        #region Constructor

        public DefaultDataService()
        {
            ConnectionSettings settings = ElasticConnectionHelper.GetElasticConnectionSetting().DefaultMappingFor<SearchProduct>(m => m.IdProperty(p => p.indexid)).RequestTimeout(TimeSpan.FromMinutes(20));
            elasticClient = new ElasticClient(settings);

            //Elasticsearch implementation.
            elasticSearchIndexerService = GetService<IElasticSearchIndexerService>();
        }

        #endregion Constructor

        #region Public Methods

        //Populate the default data.
        public virtual void IndexingDefaultData(string indexName, SearchParameterModel searchParameterModel)
        {
            try
            {
                //Call to Create index in elastic search.
                elasticSearchIndexerService.CreateIndex(elasticClient, indexName, searchParameterModel);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //Create document in given index.
        public virtual void CreateDocuments(string indexName, List<ZnodePublishProductEntity> elasticProducts)
            => elasticSearchIndexerService.CreateDocuments(elasticProducts, elasticClient, indexName);


        //Populate the default data.
        public virtual bool DeleteIndexData(string indexName, long indexStartTime)
        {
            try
            {
                //Call to delete index data in elastic search.
                return elasticSearchIndexerService.DeleteProductData(elasticClient, indexName, indexStartTime);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
                throw;
            }
        }

        //Populate the default data.
        public virtual bool DeleteProductDataByRevisionType(string indexName, string revisionType, long indexStartTime)
        {
            return elasticSearchIndexerService.DeleteProductDataByRevisionType(elasticClient, indexName, revisionType, indexStartTime);
        }

        //Delete product/document from given index. 
        public virtual bool DeleteProduct(string indexName, string znodeProductIds, string revisionType)
         => elasticSearchIndexerService.DeleteProduct(elasticClient, indexName, znodeProductIds, revisionType);

        //Delete product/document from given index. 
        public virtual bool DeleteProduct(string indexName, IEnumerable<object> productIds, string revisionType, string versionId)
            => elasticSearchIndexerService.DeleteProducts(elasticClient, indexName, productIds, revisionType, versionId);

        //Delete category/document from given index. 
        public virtual bool DeleteCategoryForGivenIndex(string indexName, int categoryId)
            => elasticSearchIndexerService.DeleteCategoryForGivenIndex(elasticClient, indexName, categoryId);

        //Delete catalog category products/documents from given index. 
        public virtual bool DeleteCatalogCategoryProducts(string indexName, int publishCatalogId, List<int> publishCategoryIds, string revisionType, string versionId)
            => elasticSearchIndexerService.DeleteCatalogCategoryProducts(elasticClient, indexName, publishCatalogId, publishCategoryIds, revisionType, versionId);

        //Checks if the index exists on the local system.
        public virtual bool IsIndexExists(string indexName)
                => elasticSearchIndexerService.IsIndexExists(elasticClient, indexName);

        public virtual bool WriteSynonymsFile(int publishCatalogId, string indexName, bool isSynonymsFile, bool isAllDeleted = false) =>
            elasticSearchIndexerService.WriteSynonymsFile(elasticClient, publishCatalogId, indexName, isSynonymsFile, isAllDeleted);

        public virtual bool DeleteIndex(string indexName)
        {
            try
            {
                //Call to delete index data in elastic search.
                return elasticSearchIndexerService.DeleteIndex(elasticClient, indexName);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
                throw;
            }
        }

        public virtual bool RenameIndexData(int catalogIndexId, string oldIndexName, string newIndexName)
        {
            return elasticSearchIndexerService.RenameIndexData(elasticClient, catalogIndexId, oldIndexName, newIndexName);
        }

        //Purge all indexed data from elastic search
        public virtual void DeleteElasticSearchIndex(List<string> indexName)
        {
            try
            {
                foreach (string name in indexName)
                {
                    if (!string.IsNullOrEmpty(name))
                    {
                        bool status = elasticSearchIndexerService.DeleteElasticSearchIndex(elasticClient, name);
                    }
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Search.ToString(), TraceLevel.Error);
            }
        }

        // To get the index names which are pointed by the specified alias.
        public virtual List<string> GetIndicesPointingToAlias(string alias)
            => elasticSearchIndexerService.GetIndicesPointingToAlias(elasticClient, alias)?.ToList();

        #endregion
    }
}
