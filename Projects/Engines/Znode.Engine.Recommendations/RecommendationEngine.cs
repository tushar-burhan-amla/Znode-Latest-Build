using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Exceptions;
using Znode.Engine.Recommendations.DataModel;
using Znode.Engine.Recommendations.Models;
using Znode.Libraries.Data;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Recommendations
{
    public class RecommendationEngine : IRecommendationEngine
    {
        #region Private Variables
        private readonly IZnodeRepository<ZnodeRecommendationBaseProduct> _recommendationBaseProductRepository;
        private readonly IZnodeRepository<ZnodeRecommendedProduct> _recommendedProductRepository;
        private readonly IZnodeRepository<ZnodeRecommendationProcessingLog> _recommendationProcessingLogRepository;
        private readonly Dictionary<string, Dictionary<string, decimal?>> internalModel = new Dictionary<string, Dictionary<string, decimal?>>();
        #endregion

        #region Constructor
        public RecommendationEngine()
        {
            _recommendationBaseProductRepository = new ZnodeRepository<ZnodeRecommendationBaseProduct>(Utilities.CurrentContext);
            _recommendedProductRepository = new ZnodeRepository<ZnodeRecommendedProduct>(Utilities.CurrentContext);
            _recommendationProcessingLogRepository = new ZnodeRepository<ZnodeRecommendationProcessingLog>(Utilities.CurrentContext);
        }
        #endregion

        #region Get Recommendations
        //To get the recommended product SKUs based on the recommendation context.
        public List<string> GetRecommendations(RecommendationContext recommendationContext)
        {
            if (IsNull(recommendationContext))
            {
                ZnodeLogging.LogMessage(Admin_Resources.InfoEmptyRecommendationContext, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.InfoEmptyRecommendationContext);
            }

            List<RecommendedProductModel> recommendedProductsList = null;

            switch (recommendationContext.WidgetCode)
            {
                case ZnodeConstant.HomeRecommendations:
                    recommendedProductsList = GetRecommendedProducts(recommendationContext.RecentlyViewedProductSkus, recommendationContext.PortalId);
                    return GetSortedRecommendedProductSku(recommendedProductsList);
                case ZnodeConstant.PDPRecommendations:
                    recommendedProductsList = GetRecommendedProducts(new List<string>() { recommendationContext.ProductSkuCurrentlyBeingViewed },
                    recommendationContext.PortalId);
                    return GetSortedRecommendedProductSku(recommendedProductsList);
                case ZnodeConstant.CartRecommendations:
                    recommendedProductsList = GetRecommendedProducts(recommendationContext.ProductSkusInCart, recommendationContext.PortalId);
                    return GetSortedRecommendedProductSku(recommendedProductsList);
                default:
                    throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.InvalidRecommendationWidgetCode);
            }
        }

        //Used to get the recommended products against the base product SKUs passed to it.
        public List<RecommendedProductModel> GetRecommendedProducts(List<string> baseProductSKUs, int? portalId)
        {
            ZnodeRecommendationProcessingLog processLog = _recommendationProcessingLogRepository.Table.
                FirstOrDefault(x => x.PortalId == portalId && x.Status.Equals(ZnodeConstant.CompletedStatus));

            List<RecommendedProductModel> recommendations = new List<RecommendedProductModel>();

            if (IsNotNull(processLog))
            {
                List<ZnodeRecommendationBaseProduct> baseProducts = _recommendationBaseProductRepository.Table.
                 Where(x => x.PortalId == portalId && baseProductSKUs.Contains(x.SKU) && x.RecommendationProcessingLogsId == processLog.RecommendationProcessingLogsId).ToList();

                List<ZnodeRecommendedProduct> recommendedProducts = null; 

                if (baseProducts?.Count > 0)
                {
                    List<long> baseProductIds = baseProducts.Select(y => y.RecommendationBaseProductsId).ToList();
                    //Base product itself will not be shown in the recommendations
                    recommendedProducts = _recommendedProductRepository.Table.
                        Where(x => baseProductIds.Contains(x.RecommendationBaseProductsId) && !baseProductSKUs.Contains(x.SKU)).ToList();

                }                    

                recommendations = recommendedProducts?.GroupBy(x => x.SKU).Select(x => new RecommendedProductModel()
                {
                    SKU = x.Key,
                    Quantity = x.Sum(y => y.Quantity),
                    Occurrence = x.Sum(y => y.Occurrence)
                }).ToList();
            }                      

            return recommendations;
        }

        //Get recommended products in sorted order by occurrence and quantity.
        protected virtual List<string> GetSortedRecommendedProductSku(List<RecommendedProductModel> recommendedProducts)
        => recommendedProducts?.OrderByDescending(product => product.Occurrence).ThenByDescending(product => product.Quantity).Select(product => product.SKU).ToList();
        #endregion       

        #region Internal model creation logic
        //To generate recommendation engine's internal model.
        public List<RecommendationBaseProductModel> GenerateInternalModel(List<List<RecommendationLineItemModel>> cartLineItemsOfOrders, int? portalId, int processingLogsId)
        {
            List<RecommendationBaseProductModel> recommendedBaseProductList = new List<RecommendationBaseProductModel>();

            List<string> uniqueSKUs = cartLineItemsOfOrders?.SelectMany(t => t).Select(x => x.SKU).Distinct().ToList();

            uniqueSKUs?.ForEach(baseProductSku =>
            {
                List<List<RecommendationLineItemModel>> ordersHavingBaseSku = cartLineItemsOfOrders.Where(group => group.Any(product => product.SKU == baseProductSku))?.ToList();
                List<RecommendedProductModel> recommendedProductList = GetRecommendedProduct(ordersHavingBaseSku, baseProductSku);
                recommendedBaseProductList.Add(new RecommendationBaseProductModel
                {
                    SKU = baseProductSku,
                    PortalId = portalId,
                    RecommendationProcessingLogsId = processingLogsId,
                    RecommendedProducts = recommendedProductList
                });
            });

            return recommendedBaseProductList;
        }

        //Get recommended base product details against the SKU passed to it.
        protected virtual List<RecommendedProductModel> GetRecommendedProduct(List<List<RecommendationLineItemModel>> ordersHavingBaseSku, string baseProductSku)
        {
            List<RecommendedProductModel> recommendedProductList = ordersHavingBaseSku?.SelectMany(order => order).
                GroupBy(x => x.SKU).
                Where(x => !x.Key.Equals(baseProductSku))?.
                Select(x => new RecommendedProductModel
                {
                    SKU = x.Key,
                    Quantity = x.Sum(y => y.Quantity),
                    Occurrence = x.Count()
                })?.ToList();

            return recommendedProductList;
        }
        #endregion        

        #region Internal model merging logic        
        //To get the currently processed internal model with the internal model present in database.
        public List<RecommendationBaseProductModel> GetMergedRecommendedBaseProducts(List<RecommendationBaseProductModel> processedData, List<RecommendationBaseProductModel> dataToBeMerged, int? portalId, int processLogId)
        {
            RecommendationEngine recommendationEngine = new RecommendationEngine();

            if (processedData?.Count > 0 && dataToBeMerged?.Count > 0)
            {
                processedData.AddRange(dataToBeMerged);
                var baseProductsGrpBySku = processedData.GroupBy(x => x.SKU).ToList();
                List<string> uniqueSKUs = baseProductsGrpBySku?.SelectMany(t => t).Select(x => x.SKU).Distinct().ToList();
                List<RecommendationBaseProductModel> recommendedBaseProductList = new List<RecommendationBaseProductModel>();

                uniqueSKUs?.ForEach(baseProductSku =>
                {
                    List<RecommendationBaseProductModel> recommendedBaseProducts = baseProductsGrpBySku?.Where(group => group.Any(product => product.SKU == baseProductSku))?.SelectMany(baseProduct => baseProduct)?.ToList();
                    //Because each group having existing model data(with Id) and new data.
                    RecommendationBaseProductModel recommendedBaseProduct = recommendedBaseProducts?.FirstOrDefault(x => x.RecommendationBaseProductsId > 0 || x.RecommendationProcessingLogsId > 0);
                    List<RecommendedProductModel> recommendedProductList = GetMergedRecommendedProducts(recommendedBaseProducts, baseProductSku);
                    recommendedBaseProductList.Add(new RecommendationBaseProductModel
                    {
                        RecommendationBaseProductsId = recommendedBaseProduct != null ? recommendedBaseProduct.RecommendationBaseProductsId : 0,
                        SKU = baseProductSku,
                        PortalId = portalId,
                        RecommendationProcessingLogsId = processLogId,
                        RecommendedProducts = recommendedProductList
                    });
                });

                return recommendedBaseProductList;
            }

            return dataToBeMerged;
        }
        
        //To get the merged recommended products.
        protected virtual List<RecommendedProductModel> GetMergedRecommendedProducts(List<RecommendationBaseProductModel> recommendedBaseProducts, string baseProductSku)
        {
            var recommendedProductGrpBySku = recommendedBaseProducts?.Select(x => x.RecommendedProducts)?.SelectMany(recommendedProduct => recommendedProduct)?.
                GroupBy(x => x.SKU);

            List<RecommendedProductModel> recommendedProductList = recommendedProductGrpBySku?.
                Select(x => new RecommendedProductModel
                {
                    //RecommendedProductsID = recommendedProductModel.RecommendedProductsID,
                    RecommendedProductsId = x.FirstOrDefault().RecommendedProductsId,
                    RecommendationBaseProductsId = x.FirstOrDefault().RecommendationBaseProductsId,
                    SKU = x.Key,
                    Quantity = x.Sum(y => y.Quantity),
                    Occurrence = x.Sum(y => y.Occurrence),
                })?.ToList();

            return recommendedProductList;
        }
        #endregion

        #region Recommendation processing log operations
        //To check if recommendation processing log is present for the given status.
        public bool CheckAnyLogExistByStatus(string status)
        => _recommendationProcessingLogRepository.Table.Any(log => log.Status.Equals(status));

        //To create recommendation processing log.
        public ZnodeRecommendationProcessingLog CreateRecommendationProcessingLog(ZnodeRecommendationProcessingLog znodeRecommendationProcessingLog)
        => _recommendationProcessingLogRepository.Insert(znodeRecommendationProcessingLog);

        //To get recommendation processing log by portal Id and status.
        public ZnodeRecommendationProcessingLog GetRecommendationProcessLog(int? portalId, string status)
        => _recommendationProcessingLogRepository.Table.FirstOrDefault(x => x.PortalId == portalId && x.Status.Equals(status));

        //To update recommendation processing log.
        public bool UpdateRecommendationProcessingLog(ZnodeRecommendationProcessingLog znodeRecommendationProcessingLog)
        => _recommendationProcessingLogRepository.Update(znodeRecommendationProcessingLog);
        #endregion

        #region Recommendation base product operations
        //To save list of ZnodeRecommendationBaseProduct.
        public List<ZnodeRecommendationBaseProduct> SaveRecommendationBaseProducts(List<ZnodeRecommendationBaseProduct> recommendationBaseProducts)
        => _recommendationBaseProductRepository.Insert(recommendationBaseProducts).ToList();

        //To get the recommendation base products list based on portal Id and processing log status passed to it.
        public List<ZnodeRecommendationBaseProduct> GetRecommendationBaseProducts(int? portalId, string status)
        {
            ZnodeRecommendationProcessingLog recommendationProcessingLog = GetRecommendationProcessLog(portalId, status);

            if (recommendationProcessingLog == null)
                return new List<ZnodeRecommendationBaseProduct>();
           
            List <ZnodeRecommendationBaseProduct> existingBaseProducts = _recommendationBaseProductRepository.Table
                .Where(x => x.PortalId == portalId && x.RecommendationProcessingLogsId == recommendationProcessingLog.RecommendationProcessingLogsId).Include("ZnodeRecommendedProducts").ToList();

            return existingBaseProducts;
        }
        #endregion

        #region Delete operations
        // To delete all the records which are associated to specific logs, except the log id passed to method.
        // Logs with different statuses can be deleted at a time.
        public bool DeleteDataAssociatedToProcessLog(int? portalId, int deleteExceptId, params string[] statuses)
        {
            List<ZnodeRecommendationProcessingLog> processLogList = _recommendationProcessingLogRepository.Table.
                Where(log => log.PortalId == portalId && statuses.Contains(log.Status) && log.RecommendationProcessingLogsId != deleteExceptId).ToList();

            if(processLogList?.Count > 0)
            {
                List<int> processLogIds = processLogList.Select(x => x.RecommendationProcessingLogsId).ToList();

                List<ZnodeRecommendationBaseProduct> recommendedBaseProducts = _recommendationBaseProductRepository.Table.
                    Where(baseProduct => processLogIds.Contains(baseProduct.RecommendationProcessingLogsId))?.ToList();

                List<long> recommendedBaseProductIds = recommendedBaseProducts.Select(x => x.RecommendationBaseProductsId).ToList();
                
                List<ZnodeRecommendedProduct> recommendedProducts = _recommendedProductRepository.Table.
                    Where(x => recommendedBaseProductIds.Contains(x.RecommendationBaseProductsId)).ToList();
                    
                bool isLogAssociatedDataDeleted = false;

                //If recommended products are deleted then only delete the associated base products.
                if (recommendedProducts?.Count > 0 && _recommendedProductRepository.Delete(recommendedProducts))
                    isLogAssociatedDataDeleted = _recommendationBaseProductRepository.Delete(recommendedBaseProducts);

                //If base products are deleted then only delete the process log records.
                if (isLogAssociatedDataDeleted)
                    return _recommendationProcessingLogRepository.Delete(processLogList);

                return isLogAssociatedDataDeleted;
            }
            return false;
        }
        #endregion                       
    }
}