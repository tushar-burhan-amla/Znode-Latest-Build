using System.Collections.Generic;
using Znode.Engine.Recommendations.Models;
using Znode.Engine.Recommendations.DataModel;

namespace Znode.Engine.Recommendations
{
    public interface IRecommendationEngine
    {
        /// <summary>
        /// To generate recommendation engine's internal model.
        /// </summary>
        /// <param name="cartLineItemsOfOrders"></param>
        /// <param name="portalId">Portal Id</param>
        /// <param name="processingLogsId">Recommendation processing log Id</param>
        /// <returns></returns>
        List<RecommendationBaseProductModel> GenerateInternalModel(List<List<RecommendationLineItemModel>> cartLineItemsOfOrders, int? portalId, int processingLogsId);
        
        /// <summary>
        /// To save list of ZnodeRecommendationBaseProduct.
        /// </summary>
        /// <param name="recommendationBaseProducts">List of ZnodeRecommendationBaseProduct</param>
        /// <returns>List of ZnodeRecommendationBaseProduct</returns>
        List<ZnodeRecommendationBaseProduct> SaveRecommendationBaseProducts(List<ZnodeRecommendationBaseProduct> recommendationBaseProducts);

        /// <summary>
        /// To check if recommendation processing log is present for the given status.
        /// </summary>
        /// <param name="status">Log status</param>
        /// <returns>True if record exist else false</returns>
        bool CheckAnyLogExistByStatus(string status);

        /// <summary>
        /// To get the recommendation base products list based on portal Id and processing log status passed to it.
        /// </summary>
        /// <param name="portalId">Portal Id</param>
        /// <param name="status">Processing log status</param>
        /// <returns>List of ZnodeRecommendationBaseProduct</returns>
        List<ZnodeRecommendationBaseProduct> GetRecommendationBaseProducts(int? portalId, string status);

        /// <summary>
        /// To create recommendation processing log.
        /// </summary>
        /// <param name="znodeRecommendationProcessingLog">ZnodeRecommendationProcessingLog entity</param>
        /// <returns>ZnodeRecommendationProcessingLog</returns>
        ZnodeRecommendationProcessingLog CreateRecommendationProcessingLog(ZnodeRecommendationProcessingLog znodeRecommendationProcessingLog);

        /// <summary>
        /// To get recommendation processing log by portal Id and status.
        /// </summary>
        /// <param name="portalId">Portal Id</param>
        /// <param name="status">Processing log status</param>
        /// <returns>ZnodeRecommendationProcessingLog</returns>
        ZnodeRecommendationProcessingLog GetRecommendationProcessLog(int? portalId, string status);

        /// <summary>
        /// To update recommendation processing log.
        /// </summary>
        /// <param name="znodeRecommendationProcessingLog">ZnodeRecommendationProcessingLog</param>
        /// <returns>True if record updated successfully else returns false</returns>
        bool UpdateRecommendationProcessingLog(ZnodeRecommendationProcessingLog znodeRecommendationProcessingLog);

        /// <summary>
        /// To get the currently processed internal model with the internal model present in database.
        /// </summary>
        /// <param name="processedData">Data present in database</param>
        /// <param name="dataToBeMerged">Currently processed data</param>
        /// <param name="portalId">Portal Id</param>
        /// <param name="processLogId">Process log Id</param>
        /// <returns>List of RecommendationBaseProductModel</returns>
        List<RecommendationBaseProductModel> GetMergedRecommendedBaseProducts(List<RecommendationBaseProductModel> processedData,
            List<RecommendationBaseProductModel> dataToBeMerged, int? portalId, int processLogId);

        /// <summary>
        /// To delete all the records which are associated to specific logs, except the log id passed to method.
        /// Logs having multiple statuses can be deleted at a time.
        /// </summary>
        /// <param name="portalId">Portal Id</param>
        /// <param name="status">Status</param>
        /// <param name="deleteExceptId">Log id which will not be considered for deletion</param>
        /// <returns></returns>
        bool DeleteDataAssociatedToProcessLog(int? portalId, int deleteExceptId, params string[] statuses);

        /// <summary>
        /// To save recommendations data into the double hash table.
        /// </summary>
        /// <param name="tableName">Name of table in which data will be get inserted</param>
        /// <param name="recommendationsData">Recommendations data</param>
        //void SaveDataInTempTable(string tableName, DataTable recommendationsData);

        /// <summary>
        /// To get the recommended product SKUs based on the recommendation context.
        /// </summary>
        /// <param name="recommendationContext">Recommendation context</param>
        /// <returns>Recommended products SKU list</returns>
        List<string> GetRecommendations(RecommendationContext recommendationContext);

        /// <summary>
        /// Used to get the recommended products against the base product SKUs passed to it.
        /// </summary>
        /// <param name="baseProductSKUs"></param>
        /// <param name="portalId"></param>
        /// <returns></returns>
        List<RecommendedProductModel> GetRecommendedProducts(List<string> baseProductSKUs, int? portalId);
    }
}
