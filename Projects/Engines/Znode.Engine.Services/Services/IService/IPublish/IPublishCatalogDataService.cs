using System.Collections.Generic;

namespace Znode.Engine.Services
{
    public interface IPublishCatalogDataService
    {
        /// <summary>
        /// Perform complete catalog publish operation with associated productstatus matched with isDraftProductsOnly flag passed.
        /// </summary>
        /// <param name="pimCatalogId">Product Category Id</param>
        /// <param name="revisionType">Revision Type</param>
        /// <param name="jobId">Job Id</param>
        /// <param name="publishCatalogId">Publish Catalog Id</param>
        /// <param name="isDraftProductsOnly">Publish draft status/all Products Only.</param>
        /// <returns></returns>
        bool ProcessCatalogPublish(int pimCatalogId, string revisionType, string jobId, bool isDraftProductsOnly, out int publishCatalogId);

        /// <summary>
        /// Perform complete catalog publish catalog operation with associated product which has draft status.
        /// </summary>
        /// <param name="pimCatalogId">Product Category Id</param>
        /// <param name="revisionType">Revision Type</param>
        /// <param name="jobId">Job Id</param>
        /// <param name="publishCatalogId">Publish Catalog Id</param>
        /// <returns></returns>
        bool ProcessCatalogPublish(int pimCatalogId, string revisionType, string jobId, out int publishCatalogId);

        /// <summary>
        /// Clean up all previous version data of current publish catalog.
        /// </summary>
        /// <param name="publishCatalogId">Publish CatalogId</param>
        /// <param name="jobId">jobId</param>
        void PurgePreviouslyPublishedCatalogDetails(int publishCatalogId, string jobId);

        /// <summary>
        /// Revert all inserted data of processing catalog in case of any failure.
        /// </summary>
        /// <param name="publishCatalogId">publish Catalog Id</param> 
        /// <param name="jobId">jobId</param>
        void RevertInProgressCatalogData(int publishCatalogId, string jobId);

        /// <summary>
        /// Call update store procedure to update associated & linked products data
        /// </summary>
        void UpdatePublishedProductAssociatedData();

        /// <summary>
        /// Fetch appropriate revision type(s) for elastic search based on given revision type
        /// </summary>
        /// <param name="revisionType"> given revision type</param>
        /// <returns>List of necessary revision type</returns>
        List<string> GetRevisionTypesForElasticIndex(string revisionType);

        /// <summary>
        /// Check any import process is in progress or not.
        /// </summary>
        /// <returns>True if any import process in progress, other wise false</returns>
        bool IsImportProcessInProgress();
    }
}
