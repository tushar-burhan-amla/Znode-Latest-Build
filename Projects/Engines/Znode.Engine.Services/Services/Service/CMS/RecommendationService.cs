using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using Znode.Libraries.Resources;
using System.Diagnostics;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Observer;
using System.Collections.Generic;
using Znode.Engine.Recommendations;
using Znode.Engine.Recommendations.Models;
using Znode.Engine.Recommendations.DataModel;
using System;
using System.Runtime.Remoting.Messaging;
using System.Web;
using System.Data;
using Znode.Libraries.Data.Helpers;
using System.Collections.Specialized;

namespace Znode.Engine.Services
{
    public class RecommendationService : BaseService, IRecommendationService
    {
        #region Private Variables
        private readonly IPublishProductService _publishProductService;
        private readonly IZnodeRepository<ZnodePortalRecommendationSetting> _portalRecommendationSettingRepository;
        private readonly IRecommendationEngine _recommendationEngine;
        private readonly int processingTimeLimit;
        private readonly int chunkSize;
        #endregion

        public RecommendationService(IPublishProductService publishProductService, IRecommendationEngine recommendationEngine)
        {
            _publishProductService = publishProductService;
            _recommendationEngine = recommendationEngine;
            _portalRecommendationSettingRepository = new ZnodeRepository<ZnodePortalRecommendationSetting>();            
            processingTimeLimit = ZnodeApiSettings.RecommendationModelTimeLimit;
            chunkSize = ZnodeApiSettings.RecommendationModelChunkSize;
        }

        #region Admin - product recommendation section
        //To get product recommendation setting against the portal Id.
        public RecommendationSettingModel GetRecommendationSetting(int portalId, string touchPointName)
        {
            IZnodeRepository<ZnodePortal> _portalRepository = new ZnodeRepository<ZnodePortal>();            

            ZnodeLogging.LogMessage("Get recommendation settings for portalId: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, portalId);

            if (portalId < 1)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.PortalIdNotLessThanOne);

            RecommendationSettingModel recommendationSettingModel = new RecommendationSettingModel();

            //To get record from ZnodePortalRecommendationSetting entity.
            ZnodePortalRecommendationSetting recommendationSettingEntity = _portalRecommendationSettingRepository.Table.FirstOrDefault(x => x.PortalId == portalId);

            if(IsNotNull(recommendationSettingEntity))
                recommendationSettingModel = recommendationSettingEntity.ToModel<RecommendationSettingModel>();

            //Portal id will be used in save event.
            recommendationSettingModel.PortalId = portalId;

            //Portal name to be displayed on the page.
            recommendationSettingModel.PortalName = _portalRepository.Table.FirstOrDefault(x => x.PortalId == portalId)?.StoreName;
            
            recommendationSettingModel = SetSchedulerDetails(recommendationSettingModel, touchPointName);

            return recommendationSettingModel;
        }

        //To set scheduler details in RecommendationSettingModel.
        protected virtual RecommendationSettingModel SetSchedulerDetails(RecommendationSettingModel recommendationSettingModel, string touchPointName)
        {
            IZnodeRepository<ZnodeERPTaskScheduler> _erpTaskSchedulerRepository = new ZnodeRepository<ZnodeERPTaskScheduler>();

            ZnodeERPTaskScheduler znodeERPTaskScheduler = _erpTaskSchedulerRepository.Table
                .FirstOrDefault(x => x.TouchPointName == touchPointName && !string.IsNullOrEmpty(x.SchedulerName));
            
            recommendationSettingModel.ERPTaskSchedulerId = IsNotNull(znodeERPTaskScheduler) ? znodeERPTaskScheduler.ERPTaskSchedulerId : 0;
            recommendationSettingModel.TouchPointName = touchPointName;

            return recommendationSettingModel;
        }

        //To save the product recommendation setting.
        public RecommendationSettingModel SaveRecommendationSetting(RecommendationSettingModel recommendationSettingModel)
        {
            if (IsNull(recommendationSettingModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ErrorRecommendationSettingModelNull);

            //To check recommendation setting is present for portal or not, if present update the record else insert.
            ZnodePortalRecommendationSetting recommendationSettingEntity = _portalRecommendationSettingRepository.Table.FirstOrDefault(x => x.PortalId == recommendationSettingModel.PortalId);
            if (IsNotNull(recommendationSettingEntity))
            {
                //Because of ajax request might be possible that model does not contain value for the PortalRecommendationSettingId property. 
                recommendationSettingModel.PortalRecommendationSettingId = recommendationSettingEntity.PortalRecommendationSettingId;
                UpdateRecommendationSetting(recommendationSettingModel);
                return recommendationSettingModel;
            }                       
            else
                return InsertRecommendationSetting(recommendationSettingModel);            
        }

        //To update the recommendation setting.
        protected virtual bool UpdateRecommendationSetting(RecommendationSettingModel recommendationSettingModel)
        {
            bool isUpdated = _portalRecommendationSettingRepository.Update(recommendationSettingModel.ToEntity<ZnodePortalRecommendationSetting>());

            //Clear webstore cache if product recommendation setting updated successfully.
            if (isUpdated)
                ClearWebstoreCache(recommendationSettingModel);

            return isUpdated;
        }

        //To insert the recommendation setting.
        protected virtual RecommendationSettingModel InsertRecommendationSetting(RecommendationSettingModel recommendationSettingModel)
        {
            //Used recommendationSettingEntity variable to hold inserted entity for better readability.
            ZnodePortalRecommendationSetting recommendationSettingEntity = _portalRecommendationSettingRepository.Insert(recommendationSettingModel?.ToEntity<ZnodePortalRecommendationSetting>());

            //Clear webstore cache if product recommendation setting saved successfully.
            if (recommendationSettingEntity?.PortalRecommendationSettingId > 0)
                ClearWebstoreCache(recommendationSettingModel);

            return recommendationSettingEntity?.ToModel<RecommendationSettingModel>();
        }

        //To clear webstore cache using ZnodeEventNotifier and ZnodeEventObserver.
        protected virtual void ClearWebstoreCache(RecommendationSettingModel recommendationSettingModel)
        {
            //Initialization to notify event. 
            var clearCacheInitializer = new ZnodeEventNotifier<RecommendationSettingModel>(recommendationSettingModel);
        }
        #endregion

        #region Recommendations

        #region Get recommendations
        #endregion
        //To get the recommendations based on recommendation request.
        public virtual RecommendationModel GetRecommendation(RecommendationRequestModel recommendationRequestModel)
        {
            List<string> recommendedProductSkus = _recommendationEngine.GetRecommendations(recommendationRequestModel?.ToModel<RecommendationContext, RecommendationRequestModel>());
 
            return new RecommendationModel
            {
                RecommendedProducts = PublishProductModels(recommendedProductSkus, recommendationRequestModel)
            };
        }
        
        #region Initiate recommendation engine internal model creation
        //Will initiate the internal model creation process. 
        public virtual RecommendationGeneratedDataModel GenerateRecommendationData(int? portalId, bool isBuildPartial)
        {
            if (_recommendationEngine.CheckAnyLogExistByStatus(ZnodeConstant.Processing))
                throw new ZnodeException(ErrorCodes.AlreadyExist, Admin_Resources.RecommendationDataGenerationInProgress);
            ZnodeRecommendationProcessingLog completedLog = SetLastProcessedOrderIdWithdate(isBuildPartial, portalId);
            //Stop processing if there is no orders data available for processing.
            if (!CheckOrdersAvailableForProcessing(portalId, completedLog.LastProcessedOrderId))
                throw new ZnodeException(ErrorCodes.NotFound, Admin_Resources.RecommendationsOrderDetailsNotPresent);
            ZnodeRecommendationProcessingLog processingLog = CreateLogWithProcessingStatus(completedLog, portalId);
            
            HttpContext httpContext = HttpContext.Current;
            Action threadWorker = delegate ()
            {
                HttpContext.Current = httpContext;
                Stopwatch stopwatch = new Stopwatch();
                stopwatch.Start();
                try
                {
                    while (!IsProcessingTimeElapsed(stopwatch))
                    {
                        RecommendationOrderModel ordersDataModel = GetOrdersData(portalId, processingLog.LastProcessedOrderId, chunkSize);
                        if (IsNull(ordersDataModel) || IsProcessingTimeElapsed(stopwatch)) break;
                        if (ordersDataModel.LineItemsSkuList?.Count > 0)
                        {
                            List<RecommendationBaseProductModel> generatedInternalModel = _recommendationEngine.GenerateInternalModel(ordersDataModel.LineItemsSkuList, portalId, processingLog.RecommendationProcessingLogsId);
                            if (IsProcessingTimeElapsed(stopwatch)) break;
                            List<RecommendationBaseProductModel> processedData = GetProcessedRecommendationBaseProducts(portalId, isBuildPartial);
                            List<RecommendationBaseProductModel> mergedInternalModel = _recommendationEngine.GetMergedRecommendedBaseProducts(processedData, generatedInternalModel, portalId, processingLog.RecommendationProcessingLogsId);
                            if (IsProcessingTimeElapsed(stopwatch)) break;
                            if (SaveRecommendationsData(mergedInternalModel, stopwatch) != 1) break;
                        }
                        //In next iteration updated order Id will be considered.
                        processingLog = UpdateLastProcessedOrderDetails(processingLog, ordersDataModel);
                    }
                    bool isprocessingLogUpdated = StopTimerAndUpdateLog(stopwatch, processingLog, ZnodeConstant.CompletedStatus);
                    if (isprocessingLogUpdated)
                        _recommendationEngine.DeleteDataAssociatedToProcessLog(portalId, processingLog.RecommendationProcessingLogsId, ZnodeConstant.CompletedStatus, ZnodeConstant.RecommendationDataProcessingFailed);
                }
                catch (Exception ex)
                {
                    StopTimerAndUpdateLog(stopwatch, processingLog, ZnodeConstant.RecommendationDataProcessingFailed);
                    ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                    throw new ZnodeException(ErrorCodes.ProcessingFailed, Admin_Resources.RecommendationDataProcessingFailed);
                }
            };
            AsyncCallback cb = new AsyncCallback(RecommendationModelCreationCallBack);
            threadWorker.BeginInvoke(cb, null);

            return new RecommendationGeneratedDataModel() { IsDataGenerationStarted = true};
        }

        protected virtual void RecommendationModelCreationCallBack(IAsyncResult ar)
        {
            AsyncResult result = ar as AsyncResult;
        }
        #endregion

        #region Order operations
        //To check placed order details are present after specific order Id for a portal.  
        protected virtual bool CheckOrdersAvailableForProcessing(int? portalId, int lastProcessedOrderId)
        {
            IZnodeRepository<ZnodeOmsOrder> _orderRepository = new ZnodeRepository<ZnodeOmsOrder>(); 
            IZnodeRepository<ZnodeOmsOrderDetail> _orderDetailsRepository = new ZnodeRepository<ZnodeOmsOrderDetail>();            

            List<ZnodeOmsOrderDetail> orderDetailsList = (from order in _orderRepository.Table.Where(x => x.OmsOrderId > lastProcessedOrderId)
                                                              join orderDetails in _orderDetailsRepository.Table.Where(x => portalId == null ? true : x.PortalId == portalId)
                                                              on order.OmsOrderId equals orderDetails.OmsOrderId into data
                                                              select data).SelectMany(x => x).ToList();
            return orderDetailsList.Any();
        }

        public virtual RecommendationOrderModel GetOrdersData(int? portalId, int lastProcessedOrderId, int chunkSize)
        {
            IZnodeRepository<ZnodeOmsOrder> _orderRepository = new ZnodeRepository<ZnodeOmsOrder>();
            IZnodeRepository<ZnodeOmsOrderDetail> _orderDetailsRepository = new ZnodeRepository<ZnodeOmsOrderDetail>();
            IZnodeRepository<ZnodeOmsOrderLineItem> _orderLineItemsRepository = new ZnodeRepository<ZnodeOmsOrderLineItem>();

            
            List<ZnodeOmsOrder> ordersList = _orderRepository.Table.Where(x => x.OmsOrderId > lastProcessedOrderId)?.Take(chunkSize)?.ToList();

            RecommendationOrderModel ordersDataModel = ordersList?.OrderByDescending(a => a.CreatedDate).Select(a => new RecommendationOrderModel
            {
                LastProcessedOrderDate = a.CreatedDate,
                LastProcessedOrderId = a.OmsOrderId
            })?.FirstOrDefault();

            if (IsNotNull(ordersDataModel))
            {
                List<List<RecommendationLineItemModel>> lineItemsSkuList = new List<List<RecommendationLineItemModel>>();
                
                var orderLineItemsListForEachOrderDetail = (from order in ordersList
                                                            join orderDetails in _orderDetailsRepository.Table.Where(x => portalId == null ? true : x.PortalId == portalId) on order.OmsOrderId equals orderDetails.OmsOrderId
                                                            join orderLineItems in _orderLineItemsRepository.Table.Select(x=> new { x.OmsOrderDetailsId, x.Sku, x.Quantity }) on orderDetails.OmsOrderDetailsId equals orderLineItems.OmsOrderDetailsId
                                                            group orderLineItems by orderLineItems.OmsOrderDetailsId into data
                                                            select data).ToList();

                //To get SKU and Quantity of products of every order.
                orderLineItemsListForEachOrderDetail.ForEach(x => {
                    lineItemsSkuList.Add(x.Select(y => new RecommendationLineItemModel { SKU = y.Sku, Quantity = y.Quantity }).DistinctBy(z => z.SKU).ToList());
                });

                ZnodeLogging.LogMessage("lineItemsSkuList count: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, lineItemsSkuList?.Count);

                ordersDataModel.LineItemsSkuList = lineItemsSkuList;
            }

            //Note: there may happen that entries will be present in order table and related entries will not be present in orderdetails table.
            //thats why we cannot put null check for LineItemsSkuList
            //May happen that some orders are still left to be processed yet which are having related order details
            return ordersDataModel;
        }
        #endregion

        #region Data saving
        //This method will save the recommendations data and returns 1 if data saved successfully else returns 0.
        protected virtual int SaveRecommendationsData(List<RecommendationBaseProductModel> recommendationsData, Stopwatch stopwatch)
        {
            string hashTableName;
            DataTable processedDataTable = Utilities.MapProcessedDataToDataTable(recommendationsData);
            hashTableName = Utilities.CreateDoubleHashTable();
            Utilities.SaveDataInTempTable(hashTableName, processedDataTable);

            int remainingTimeLimit = RemainingProcessingTimeLimit(stopwatch);

            IList<View_ReturnBoolean> result;
            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>(new Znode_Recommendation_Entities());
            //TODO: change with UserId
            objStoredProc.SetParameter("UserId", HelperMethods.GetLoginUserId(), ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("TableName", hashTableName, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("ProcessingTimeLimit", remainingTimeLimit, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Boolean);
            int status = 0;
            result = objStoredProc.ExecuteStoredProcedureList("Znode_RecommendationProcessedData @UserId, @TableName, @ProcessingTimeLimit, @status OUT", 3, out status);

            return status;
        }
        #endregion

        #region Processing time operations
        //To find weather processing time elapsed or not.
        protected virtual bool IsProcessingTimeElapsed(Stopwatch stopwatch)
        => stopwatch.ElapsedMilliseconds >= processingTimeLimit;

        //Returns the remaining processing time limit.
        protected virtual int RemainingProcessingTimeLimit(Stopwatch stopwatch)
        => processingTimeLimit - (int)stopwatch.ElapsedMilliseconds;
        #endregion

        #region Process log operations
        //To stop the timer and update the status of processing log based on parameter.
        protected virtual bool StopTimerAndUpdateLog(Stopwatch stopwatch, ZnodeRecommendationProcessingLog processingLog, string status)
        {
            ZnodeLogging.LogMessage("Elapsed processing time: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, stopwatch.ElapsedMilliseconds);
            stopwatch.Stop();

            if (IsNull(processingLog))
                return false;

            processingLog.Status = status;
            return _recommendationEngine.UpdateRecommendationProcessingLog(processingLog);
        }

        //To create new log with processing status.
        protected virtual ZnodeRecommendationProcessingLog CreateLogWithProcessingStatus(ZnodeRecommendationProcessingLog recommendationProcessingLog, int? portalId)
        {
            recommendationProcessingLog.Status = ZnodeConstant.Processing;
            recommendationProcessingLog.PortalId = portalId;
            ZnodeRecommendationProcessingLog processingLog = _recommendationEngine.CreateRecommendationProcessingLog(recommendationProcessingLog);

            if (IsNull(processingLog))
                throw new ZnodeException(ErrorCodes.CreationFailed, Admin_Resources.RecommendationProcessLogCreationFailed);

            return processingLog;
        }

        //To get the last processed order Id.
        protected virtual ZnodeRecommendationProcessingLog SetLastProcessedOrderIdWithdate(bool isBuildPartial, int? portalId)
        {
            ZnodeRecommendationProcessingLog recommendationProcessingLog = _recommendationEngine.GetRecommendationProcessLog(portalId, ZnodeConstant.CompletedStatus);

            if (!isBuildPartial || IsNull(recommendationProcessingLog))
            {
                recommendationProcessingLog = new ZnodeRecommendationProcessingLog();
                recommendationProcessingLog.LastProcessedOrderId = 0;
                recommendationProcessingLog.LastProcessedOrderDate = new DateTime(1900, 1, 1);
            }

            return recommendationProcessingLog;
        }

        protected virtual ZnodeRecommendationProcessingLog UpdateLastProcessedOrderDetails(ZnodeRecommendationProcessingLog processingLog, RecommendationOrderModel ordersDataModel)
        {
            processingLog.LastProcessedOrderId = ordersDataModel.LastProcessedOrderId;
            processingLog.LastProcessedOrderDate = ordersDataModel.LastProcessedOrderDate;
            return processingLog;
        }
        #endregion

        #region Stored data operations
        //To get the recommendation base products
        protected virtual List<RecommendationBaseProductModel> GetProcessedRecommendationBaseProducts(int? portalId, bool isBuildPartial)
        {
            List<RecommendationBaseProductModel> processedData = _recommendationEngine.GetRecommendationBaseProducts(portalId, ZnodeConstant.Processing)?.
                ToModel<RecommendationBaseProductModel>()?.ToList();

            if ((IsNull(processedData) || processedData?.Count <= 0) && isBuildPartial)
            {
                processedData = _recommendationEngine.GetRecommendationBaseProducts(portalId, ZnodeConstant.CompletedStatus)?.
                ToModel<RecommendationBaseProductModel>()?.ToList();

                processedData?.ForEach(x =>
                {
                    x.RecommendationBaseProductsId = 0;
                    x.RecommendedProducts.ForEach(y=>y.RecommendedProductsId = 0);
                });
            }
            return processedData;
        }
        #endregion
                
        #region Published operations
        //To get the recommended published products.
        protected virtual List<PublishProductModel> PublishProductModels(List<string> productSkus, RecommendationRequestModel recommendationRequestModel)
        {
          if (productSkus?.Count > 0)
            {
                ZnodeLogging.LogMessage("Count of recommended products derived by engine: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, productSkus.Count);

                //For getting price add pricing in expands.Also pass expands in GetPublishProductList method.
                NameValueCollection expands = new NameValueCollection();
                expands.Add(ZnodeConstant.Pricing, ZnodeConstant.Pricing);

                //DefaultGlobalConfigSettingHelper.DefaultProductLimitForRecommendations is used to get the top ranked products according to the product limit specified in global setting.
                FilterCollection filters = GetRequiredFilters(productSkus, recommendationRequestModel);
                
                PublishProductListModel listModel = _publishProductService.GetPublishProductList(expands, filters, null, null);

                if (listModel?.PublishProducts?.Count > 0)
                {
                    //Distinct clause is used to avoid duplicate products in case of same product is associated to many categories and
                    //OrderBy clause is used to change the order of published products list according to recommended products SKU list(ordered by ranking).
                    listModel.PublishProducts = listModel.PublishProducts.DistinctBy(product => product.SKU).OrderBy(product => productSkus.IndexOf(product.SKU)).Take(DefaultGlobalConfigSettingHelper.DefaultProductLimitForRecommendations).ToList();

                    ZnodeLogging.LogMessage("Recommended products count: ", ZnodeLogging.Components.CMS.ToString(), TraceLevel.Verbose, listModel.PublishProducts?.Count);
                    return listModel.PublishProducts;
                }
            }
            return new List<PublishProductModel>();
        }

        //Filters to get the desired publish products.
        protected virtual FilterCollection GetRequiredFilters(List<string> productSkus, RecommendationRequestModel recommendationRequestModel)
        {          
               FilterCollection filters = new FilterCollection();
            filters.Add(WebStoreEnum.ZnodeCatalogId.ToString(), FilterOperators.Equals, Convert.ToString(recommendationRequestModel.CatalogId));
            filters.Add(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, recommendationRequestModel.LocaleId.ToString());
            filters.Add(WebStoreEnum.IsActive.ToString(), FilterOperators.Equals, ZnodeConstant.TrueValue);
            filters.Add(FilterKeys.PortalId, FilterOperators.Equals, recommendationRequestModel.PortalId.ToString());
            filters.Add(ZnodeConstant.SkuLower, FilterOperators.In, String.Join(",", productSkus.ConvertAll(sku => sku.ToLower()).Select(x => $"\"{x}\"")));
            //To avoid those products which are not available in any category.
            filters.Add(FilterKeys.ZnodeCategoryIds.ToString(), FilterOperators.NotEquals, ZnodeConstant.Zero);

            return filters;
        }
        #endregion

        #endregion
    }
}
