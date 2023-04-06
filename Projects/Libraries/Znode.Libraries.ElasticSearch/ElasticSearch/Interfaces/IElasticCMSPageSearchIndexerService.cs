using Nest;

using System.Collections.Generic;

using Znode.Libraries.Search;

namespace Znode.Libraries.ElasticSearch
{
    public interface IElasticCMSPageSearchIndexerService
    {
        /// <summary>
        /// Check index already exist or not
        /// </summary>
        /// <param name="elasticClient">elasticClient</param>
        /// <param name="indexName">indexName</param>
        /// <returns>Return true or false based on response by elastic client</returns>
        bool IsIndexExists(ElasticClient elasticClient, IndexName indexName);

        /// <summary>
        /// Rename the elastic search index
        /// </summary>
        /// <param name="elasticClient">elasticClient</param>
        /// <param name="cmsPagesIndexId">cmsPagesIndexId</param>
        /// <param name="oldIndexName">oldIndexName</param>
        /// <param name="newIndexName">newIndexName</param>
        /// <returns>Return true or false based on response by elastic client</returns>
        bool RenameIndexData(ElasticClient elasticClient, int cmsPagesIndexId, string oldIndexName, string newIndexName);

        /// <summary>
        /// Create the elastic search index
        /// </summary>
        /// <param name="elasticClient">elasticClient</param>
        /// <param name="indexName">indexName</param>
        /// <param name="searchCmsPagesParameterModel">searchCmsPagesParameterModel</param>
        void CreateIndex(ElasticClient elasticClient, IndexName indexName, SearchCMSPagesParameterModel searchCmsPagesParameterModel);

        /// <summary>
        /// Get cms pages list base on version
        /// </summary>
        /// <param name="pagedAllCmsPages">pagedAllCmsPages</param>
        /// <param name="versionId">versionId</param>
        /// <param name="start">start</param>
        /// <param name="pageLength">pageLength</param>
        /// <param name="totalPages">totalPages</param>
        /// <returns>Return cms pages</returns>
        List<SearchCMSPages> GetPagedCMSPagesListByVersionId(List<SearchCMSPages> pagedAllCmsPages, int versionId, int start, int pageLength, out decimal totalPages);

        /// <summary>
        /// Create elastic search index schema
        /// </summary>
        /// <param name="elasticClient">elasticClient</param>
        /// <param name="indexName">indexName</param>
        /// <param name="portalId">portalId</param>
        /// <returns>Return response of elasticsearch</returns>
        CreateIndexDescriptor AddPagesDescriptor(ElasticClient elasticClient, IndexName indexName, int portalId);

        /// <summary>
        /// Delete cms pages data from elastic search based on revision type
        /// </summary>
        /// <param name="elasticClient">elasticClient</param>
        /// <param name="indexName">indexName</param>
        /// <param name="revisionType">revisionType</param>
        /// <param name="indexStartTime">indexStartTime</param>
        /// <returns>Return true or false based on response by elastic client</returns>
        bool DeleteCmsPagesDataByRevisionType(ElasticClient elasticClient, IndexName indexName, string revisionType, long indexStartTime);

        /// <summary>
        /// Set all configuration for elasticsearch index schema creation
        /// </summary>
        /// <param name="indexName">indexName</param>
        /// <param name="settings">settings</param>
        /// <returns>Return response of elastic client</returns>
        CreateIndexDescriptor ConfigureIndex(IndexName indexName, IndexSettings settings);

        /// <summary>
        /// Set required analyzers for schema creation
        /// </summary>
        /// <param name="tokenFilters">tokenFilters</param>
        /// <param name="indexsettings">indexsettings</param>
        void SetAnalyzers(List<string> tokenFilters, IndexSettings indexsettings);

        /// <summary>
        /// Set required search time analyzers for schema creation
        /// </summary>
        /// <param name="tokenFilters">tokenFilters</param>
        /// <param name="indexsettings">indexsettings</param>
        void SetSearchTimeAnalyzers(List<string> tokenFilters, IndexSettings indexsettings);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="tokenFilters"></param>
        /// <param name="indexsettings"></param>
        void SetIndexTimeAnalyzers(List<string> tokenFilters, IndexSettings indexsettings);

        /// <summary>
        ///  Set required filter for analyzers
        /// </summary>
        /// <param name="tokenFilters">tokenFilters</param>
        /// <param name="indexsettings">indexsettings</param>
        void SetFilterForAnalyzers(List<string> tokenFilters, IndexSettings indexsettings);

        /// <summary>
        /// Get create index descriptor
        /// </summary>
        /// <param name="indexName">indexName</param>
        /// <returns>Return create index descriptor setting</returns>
        CreateIndexDescriptor GetCreateIndexDescriptor(IndexName indexName);

        /// <summary>
        /// Get create index descriptor
        /// </summary>
        /// <returns>Return create index descriptor</returns>
        IndexSettings CreateIndexSettings();

    }
}
