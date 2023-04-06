using System.Collections.Generic;
using System.Data;
using Znode.Libraries.Data.DataModel;
namespace Znode.Engine.Services
{
    public interface IPublishProductDataService 
    {
        /// <summary>
        /// Check requested product for publish is associated with publish category or not
        /// </summary>
        /// <param name="pimProductId"></param>
        /// <returns>True if its associated, else False</returns>
        bool IsProductAssociateWithPublishCategory(int pimProductId);

        /// <summary>
        /// Perform single product publish operation.
        /// </summary>
        /// <param name="pimProductId">pimProductId</param>
        /// <param name="revisionType">revisionType</param>
        /// <param name="status">status</param>
        /// <returns>Return dataset with status</returns>
        DataSet ProcessSingleProductPublish(int pimProductId, string revisionType, out bool status);

        /// <summary>
        /// Fetch appropriate revision type data for elastic search based on given revision type
        /// </summary>
        /// <param name="revisionType"> given revision type</param>
        /// <returns>List of necessary revision type</returns>
        List<string> GetRevisionTypesForElasticIndex(string revisionType);

        /// <summary>
        /// Create product(s) elastic index and delete old index of product(s)
        /// </summary>
        /// <param name="productEntities">productEntities</param>
        /// <param name="revisionType">revisionType</param>
        void CreateProductElasticIndex(List<ZnodePublishProductEntity> productEntities, string revisionType);

        /// <summary>
        /// Check export status
        /// </summary>
        /// <returns>bool</returns>
        bool IsExportPublishInProgress();
    }
}
