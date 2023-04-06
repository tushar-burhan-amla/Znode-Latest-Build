using System.Collections.Generic;

using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Search;

namespace Znode.Libraries.ElasticSearch
{
    public interface IDefaultDataService
    {
        /// <summary>
        /// Populate the default data
        /// </summary>
        /// <param name="indexName">indexName</param>
        /// <param name="searchParameterModel">searchParameterModel</param>
        void IndexingDefaultData(string indexName, SearchParameterModel searchParameterModel);

        /// <summary>
        /// Create document in given index
        /// </summary>
        /// <param name="indexName">indexName</param>
        /// <param name="elasticProducts">elasticProducts</param>
        void CreateDocuments(string indexName, List<ZnodePublishProductEntity> elasticProducts);

        /// <summary>
        /// Delete full-text search index
        /// </summary>
        /// <param name="indexName">indexName</param>
        /// <param name="indexStartTime">indexStartTime</param>
        /// <returns>Return true or false based on elasticsearch response</returns>
        bool DeleteIndexData(string indexName, long indexStartTime);

        /// <summary>
        /// Delete product/document from given index
        /// </summary>
        /// <param name="indexName">indexName</param>
        /// <param name="revisionType">revisionType</param>
        /// <param name="indexStartTime">indexStartTime</param>
        /// <returns>Return true or false based on elasticsearch response</returns>
        bool DeleteProductDataByRevisionType(string indexName, string revisionType, long indexStartTime);

        /// <summary>
        /// Delete product/document from given index based on revision type
        /// </summary>
        /// <param name="indexName">indexName</param>
        /// <param name="znodeProductIds">znodeProductIds</param>
        /// <param name="revisionType">revisionType</param>
        /// <returns>Return true or false based on elasticsearch response</returns>
        bool DeleteProduct(string indexName, string znodeProductIds, string revisionType);

        /// <summary>
        /// Delete product/document from given index
        /// </summary>
        /// <param name="indexName">indexName</param>
        /// <param name="productIds">productIds</param>
        /// <param name="revisionType">revisionType</param>
        /// <param name="versionId">versionId</param>
        /// <returns>Return true or false based on elasticsearch response</returns>
        bool DeleteProduct(string indexName, IEnumerable<object> productIds, string revisionType, string versionId);

        /// <summary>
        /// Delete category/document from given index
        /// </summary>
        /// <param name="indexName">indexName</param>
        /// <param name="categoryId">categoryId</param>
        /// <returns>Return true or false based on elasticsearch response</returns>
        bool DeleteCategoryForGivenIndex(string indexName, int categoryId);

        /// <summary>
        /// Delete catalog category products/documents from given index
        /// </summary>
        /// <param name="indexName">indexName</param>
        /// <param name="publishCatalogId">publishCatalogId</param>
        /// <param name="publishCategoryIds">publishCategoryIds</param>
        /// <param name="revisionType">revisionType</param>
        /// <param name="versionId">versionId</param>
        /// <returns>Return true or false based on elasticsearch response</returns>
        bool DeleteCatalogCategoryProducts(string indexName, int publishCatalogId, List<int> publishCategoryIds, string revisionType, string versionId);

        /// <summary>
        /// Checks if the index exists on the local system
        /// </summary>
        /// <param name="indexName">indexName</param>
        /// <returns>Return true or false based on elasticsearch response</returns>
        bool IsIndexExists(string indexName);

        /// <summary>
        /// Write in synonyms file
        /// </summary>
        /// <param name="publishCatalogId"></param>
        /// <param name="indexName"></param>
        /// <param name="isSynonymsFile"></param>
        /// <param name="isAllDeleted"></param>
        /// <returns>Return true or false based on elasticsearch response</returns>
        bool WriteSynonymsFile(int publishCatalogId, string indexName, bool isSynonymsFile, bool isAllDeleted = false);

        /// <summary>
        /// delete index data in elastic search
        /// </summary>
        /// <param name="indexName"></param>
        /// <returns>Return true or false based on elasticsearch response</returns>
        bool DeleteIndex(string indexName);

        /// <summary>
        /// Rename index name in elastic search
        /// </summary>
        /// <param name="catalogIndexId">catalogIndexId</param>
        /// <param name="oldIndexName">oldIndexName</param>
        /// <param name="newIndexName">newIndexName</param>
        /// <returns>Return true or false based on elasticsearch response</returns>
        bool RenameIndexData(int catalogIndexId, string oldIndexName, string newIndexName);

        /// <summary>
        /// Purge all indexed data from elastic search
        /// </summary>
        /// <param name="indexName">indexName</param>
        void DeleteElasticSearchIndex(List<string> indexName);

        /// <summary>
        /// To get the index names which are pointed by the specified alias
        /// </summary>
        /// <param name="alias">alias</param>
        /// <returns>List<string></returns>
        List<string> GetIndicesPointingToAlias(string alias);
    }
}
