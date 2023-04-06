using Znode.Libraries.Search;

namespace Znode.Libraries.ElasticSearch
{
    public interface ICMSPageDefaultDataService
    {
        /// <summary>
        /// Populate the default data
        /// </summary>
        /// <param name="indexName">indexName</param>
        /// <param name="searchCmsPagesParameterModel">searchCmsPagesParameterModel</param>
        void IndexingDefaultData(string indexName, SearchCMSPagesParameterModel searchCmsPagesParameterModel);

        /// <summary>
        /// Checks if the index exists on the local system
        /// </summary>
        /// <param name="indexName">indexName</param>
        /// <returns>Return true or false based on elasticsearch response</returns>
        bool IsIndexExists(string indexName);

        /// <summary>
        /// Rename the cms index name
        /// </summary>
        /// <param name="cmsSearchIndexId">cmsSearchIndexId</param>
        /// <param name="oldIndexName">oldIndexName</param>
        /// <param name="newIndexName">newIndexName</param>
        /// <returns>Return true or false based on elasticsearch response</returns>
        bool RenameCmsPageIndex(int cmsSearchIndexId, string oldIndexName, string newIndexName);

        /// <summary>
        /// Delete the cms page index from elasticsearch
        /// </summary>
        /// <param name="indexName">indexName</param>
        /// <param name="revisionType">revisionType</param>
        /// <param name="indexStartTime">indexStartTime</param>
        /// <returns>Return true or false based on elasticsearch response</returns>
        bool DeleteCmsPagesDataByRevisionType(string indexName, string revisionType, long indexStartTime);
    }
}
