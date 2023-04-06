using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Hangfire;
using Znode.Libraries.Admin;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;

using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Services
{
    public class ERPTaskSchedulerService : BaseService, IERPTaskSchedulerService
    {
        #region Private Variables
        private readonly IZnodeRepository<ZnodeERPTaskScheduler> _erpTaskSchedulerRepository;
        private readonly IERPConfiguratorService _service;
        private readonly IZnodeRepository<ZnodeERPConfigurator> _eRPConfiguratorRepository;
        private readonly IZnodeRepository<ZnodeSearchIndexMonitor> _searchIndexMonitorRepository;
        private readonly IERPJobs _eRPJob;
        #endregion

        #region Constructor
        public ERPTaskSchedulerService()
        {
            _erpTaskSchedulerRepository = new ZnodeRepository<ZnodeERPTaskScheduler>();
            _service = GetService<IERPConfiguratorService>();
            _eRPConfiguratorRepository = new ZnodeRepository<ZnodeERPConfigurator>();
            _searchIndexMonitorRepository = new ZnodeRepository<ZnodeSearchIndexMonitor>();
            _eRPJob = GetService<IERPJobs>();
        }
        #endregion

        #region Public Methods
        // Get ERPTaskScheduler list from database.
        public virtual ERPTaskSchedulerListModel GetERPTaskSchedulerList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            //Bind the Filter, sorts & Paging details.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            filters.Add(ZnodeERPTaskSchedulerEnum.ERPConfiguratorId.ToString(), ProcedureFilterOperators.Equals, _service.GetActiveERPClassId().ToString());
            //maps the entity list to model
            ZnodeLogging.LogMessage("pageListModel to get ERP task scheduler list: ", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Verbose, pageListModel?.ToDebugString());
            IList<ZnodeERPTaskScheduler> erpTaskSchedulerList = _erpTaskSchedulerRepository.GetPagedList(pageListModel.EntityWhereClause.WhereClause, pageListModel.OrderBy, null, pageListModel.EntityWhereClause.FilterValues, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount);
            ZnodeLogging.LogMessage("erpTaskSchedulerList count: ", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Verbose, erpTaskSchedulerList?.Count);
            ERPTaskSchedulerListModel listModel = new ERPTaskSchedulerListModel();
            listModel.ERPTaskSchedulerList = erpTaskSchedulerList?.Count > 0 ? erpTaskSchedulerList.ToModel<ERPTaskSchedulerModel>().ToList() : new List<ERPTaskSchedulerModel>();
            //Set for pagination
            listModel.BindPageListModel(pageListModel);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            return listModel;
        }

        //Create erpTaskScheduler.
        public virtual ERPTaskSchedulerModel Create(ERPTaskSchedulerModel erpTaskSchedulerModel)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            if (IsNull(erpTaskSchedulerModel))
                throw new ZnodeException(ErrorCodes.NullModel, Admin_Resources.ModelCanNotBeNull);

            if (erpTaskSchedulerModel.ERPTaskSchedulerId == 0 && IsSchedulerNameAlreadyExist(erpTaskSchedulerModel.SchedulerName) && !erpTaskSchedulerModel.IsAssignTouchPoint)
            {
                ZnodeLogging.LogMessage(Admin_Resources.ErrorSchedulerNameExists, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error);
                throw new ZnodeException(ErrorCodes.AlreadyExist, Admin_Resources.ErrorSchedulerNameExists);
            }

            int status;
            IList<View_ReturnBoolean> createResult;
            if (erpTaskSchedulerModel.IsAssignTouchPoint)
            {
                List<String> touchPointNamesList = new List<String>(erpTaskSchedulerModel.TouchPointName.Split(','));
                foreach (var item in touchPointNamesList)
                {
                    erpTaskSchedulerModel.TouchPointName = item;
                    InsertERPTaskScheduler(erpTaskSchedulerModel, out status, out createResult);
                    if (status == 1)
                        ZnodeLogging.LogMessage(Admin_Resources.SuccessERPTaskSchedulerSave, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
                    else
                        ZnodeLogging.LogMessage(Admin_Resources.ErrorERPTaskSchedulerSave, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error);
                }
                return erpTaskSchedulerModel;
            }
            else
                InsertERPTaskScheduler(erpTaskSchedulerModel, out status, out createResult);

            ZnodeLogging.LogMessage("ERPTaskSchedulerId and createResult list count: ", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Verbose, new object[] { erpTaskSchedulerModel?.ERPTaskSchedulerId, createResult?.Count });
            if (status == 1 && (erpTaskSchedulerModel.SchedulerType == ZnodeConstant.Scheduler || erpTaskSchedulerModel.SchedulerCallFor == ZnodeConstant.SearchIndex || erpTaskSchedulerModel.SchedulerCallFor == ZnodeConstant.ProductFeed))
            {
                erpTaskSchedulerModel = CreateScheduler(erpTaskSchedulerModel, createResult);

                if (erpTaskSchedulerModel.ERPTaskSchedulerId == -1)
                    throw new ZnodeException(ErrorCodes.CreationFailed, "Error while creating/updating the scheduled task");
                else
                    return erpTaskSchedulerModel;
            }

            if (erpTaskSchedulerModel.SchedulerType != ZnodeConstant.Scheduler && erpTaskSchedulerModel.SchedulerCallFor == ZnodeConstant.ERP)
            {
                //Delete ERP task scheduler from server
                ZnodeLogging.LogMessage("TaskSchedulerNames to be deleted: ", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Verbose, erpTaskSchedulerModel?.SchedulerName?.Split(',').ToList());
                bool schedulerStatus = _eRPJob.RemoveJobs(erpTaskSchedulerModel.SchedulerName.Split(',').ToList());
                ZnodeLogging.LogMessage(Admin_Resources.SuccessERPTaskSchedulerDelete, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
                return erpTaskSchedulerModel;
            }
            else
            {
                ZnodeLogging.LogMessage(Admin_Resources.ErrorERPTaskSchedulerSave, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
                return erpTaskSchedulerModel;
            }
        }


        //Get erpTaskScheduler by erpTaskScheduler id.
        public virtual ERPTaskSchedulerModel GetERPTaskScheduler(int erpTaskSchedulerId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            if (erpTaskSchedulerId <= 0)
                return null;
            IZnodeViewRepository<ERPTaskSchedulerModel> erpTaskSchedulerModel = new ZnodeViewRepository<ERPTaskSchedulerModel>();
            ZnodeLogging.LogMessage("erpTaskSchedulerId to get ERPTaskScheduler: ", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Verbose, erpTaskSchedulerId);
            erpTaskSchedulerModel.SetParameter("erpTaskSchedulerId", erpTaskSchedulerId, ParameterDirection.Input, DbType.Int32);
            var erpTaskScheduler = erpTaskSchedulerModel.ExecuteStoredProcedureList("Znode_GetERPTaskSchedulerDetail @ERPTaskSchedulerId");
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            return erpTaskScheduler?.FirstOrDefault();
        }

        //Delete erpTaskScheduler.
        public virtual bool Delete(ParameterModel erpTaskSchedulerIds)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            if (IsNull(erpTaskSchedulerIds) || string.IsNullOrEmpty(erpTaskSchedulerIds.Ids) || erpTaskSchedulerIds.Ids=="0")
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.ErrorERPTaskSchedulerIdLessThanOne);

            //Get ERP Scheduler name from Id's
            int[] erpTaskSchedulerIdsArray = erpTaskSchedulerIds.Ids.Split(',').Select(int.Parse).ToArray();
            var taskSchedulerNames = _erpTaskSchedulerRepository.Table.Where(item => erpTaskSchedulerIdsArray.Contains(item.ERPTaskSchedulerId) && item.SchedulerName!=null).Select(x => x.SchedulerName).ToList();

            foreach (int item in erpTaskSchedulerIdsArray)
            {
                if (CheckTouchPointStauts(item))
                    throw new ZnodeException(ErrorCodes.NotDeleteActiveRecord, ERP_Resources.ErrorDeleteActiveTouchPointClass);
            }
            //Delete ERP task scheduler from database
            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter(ZnodeERPTaskSchedulerEnum.ERPTaskSchedulerId.ToString(), erpTaskSchedulerIds.Ids, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Boolean);
            int status = 0;
            ZnodeLogging.LogMessage("ERP task scheduler with Ids to be deleted: ", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Verbose, erpTaskSchedulerIds?.Ids);
            objStoredProc.ExecuteStoredProcedureList("Znode_DeleteERPTaskScheduler @ERPTaskSchedulerId,  @Status OUT", 1, out status);
            if (status == 1)
            {
                //Delete ERP task scheduler from server
                ZnodeLogging.LogMessage("TaskSchedulerNames to be deleted: ", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Verbose, taskSchedulerNames);
                if (taskSchedulerNames?.Count > 0)
                {
                    bool schedulerStatus = _eRPJob.RemoveJobs(taskSchedulerNames);
                    if (schedulerStatus)
                        ZnodeLogging.LogMessage(Admin_Resources.SuccessERPTaskSchedulerDelete, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
                    else
                        ZnodeLogging.LogMessage($"Unable to remove the scheduled job. SchedulerNames: {taskSchedulerNames}", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error);
                    return schedulerStatus;
                }
                else
                    return true;
            }
            else
            {
                ZnodeLogging.LogMessage(Admin_Resources.ErrorERPTaskSchedulerDelete, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error);
                return false;
            }
        }

        // Trigger action for the task.
        public virtual string TriggerSchedulerTask(string eRPTaskSchedulerId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            string data = string.Empty;
            IZnodeViewRepository<ERPTaskSchedulerModel> erpTaskSchedulerModel = new ZnodeViewRepository<ERPTaskSchedulerModel>();
            erpTaskSchedulerModel.SetParameter("erpTaskSchedulerId", eRPTaskSchedulerId, ParameterDirection.Input, DbType.Int32);
            ZnodeLogging.LogMessage("erpTaskSchedulerId to get ERPTaskScheduler: ", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Verbose, eRPTaskSchedulerId);
            var erpTaskScheduler = erpTaskSchedulerModel.ExecuteStoredProcedureList("Znode_GetERPTaskSchedulerDetail @ERPTaskSchedulerId");

            if (!Equals(erpTaskScheduler, null))
            {
                string connectorTouchPoints = erpTaskScheduler.Select(x => x.TouchPointName).FirstOrDefault();
                ZnodeLogging.LogMessage("connectorTouchPoints: ", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Verbose, connectorTouchPoints);
                //Get current active class name
                IERPConfiguratorService eRPConfiguratorService = GetService<IERPConfiguratorService>();
                Assembly assembly = Assembly.Load("Znode.Engine.ERPConnector");
                Type className = assembly.GetTypes().FirstOrDefault(g => g.Name == eRPConfiguratorService.GetActiveERPClassName());

                //Create Instance of active class
                object instance = Activator.CreateInstance(className);

                //Get Method Information from class
                MethodInfo info = className.GetMethod(connectorTouchPoints);

                //Calling method with null parameter
                info.Invoke(instance, null);
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            return data;
        }

        //Method for get erpConfiguratorId from TouchPointName
        public virtual int GetSchedulerIdByTouchPointName(string erpTouchPointName, int erpConfiguratorId, string schedulerCallFor)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("ERPTouchPointName, erpConfiguratorId and schedulerCallFor: ", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Verbose, new object[] { erpTouchPointName, erpConfiguratorId, schedulerCallFor });
            if (schedulerCallFor == ZnodeConstant.ERP)
            {
                erpConfiguratorId = erpConfiguratorId == 0 ? _eRPConfiguratorRepository.Table.Where(x => x.IsActive).Select(g => g.ERPConfiguratorId).FirstOrDefault() : erpConfiguratorId;
                return _erpTaskSchedulerRepository.Table.Where(x => x.ERPConfiguratorId == erpConfiguratorId && x.TouchPointName == erpTouchPointName && !string.IsNullOrEmpty(x.SchedulerName)).Select(g => g.ERPTaskSchedulerId).FirstOrDefault();
            }
            else
                return _erpTaskSchedulerRepository.Table.Where(x => x.TouchPointName == erpTouchPointName && !string.IsNullOrEmpty(x.SchedulerName)).Select(g => g.ERPTaskSchedulerId).FirstOrDefault();
        }

        //Enable/Disable task scheduler from db and from windows service as well.
        public virtual bool EnableDisableTaskScheduler(int ERPTaskSchedulerId, bool isActive)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            bool isTaskSchedulerUpdated = false;

            if (ERPTaskSchedulerId == 0)
            {
                ZnodeLogging.LogMessage(Admin_Resources.ErrorTaskSchedulerExists, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error);
                throw new ZnodeException(ErrorCodes.NotFound, Admin_Resources.ErrorTaskSchedulerExists);
            }

            ZnodeLogging.LogMessage("ERPTaskSchedulerId to get ERPTaskSchedulerModel: ", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Verbose, ERPTaskSchedulerId);
            ERPTaskSchedulerModel eRPTaskSchedulerModel = GetERPTaskScheduler(ERPTaskSchedulerId);

            //Update task scheduler
            eRPTaskSchedulerModel.IsEnabled = isActive;
            isTaskSchedulerUpdated = _erpTaskSchedulerRepository.Update(eRPTaskSchedulerModel.ToEntity<ZnodeERPTaskScheduler>());
            if (isTaskSchedulerUpdated)
            {
                ZnodeLogging.LogMessage("SchedulerName and state to enable disable schedule task: ", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Verbose, new object[] { eRPTaskSchedulerModel?.SchedulerName, isActive });
                if (!isActive)
                    _eRPJob.RemoveJob(eRPTaskSchedulerModel);
            }

            if (isActive)
                ZnodeLogging.LogMessage(isTaskSchedulerUpdated ? Admin_Resources.SuccessTaskSchedulerEnable : Admin_Resources.ErrorTaskSchedulerEnable, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            else
                ZnodeLogging.LogMessage(isTaskSchedulerUpdated ? Admin_Resources.SuccessTaskSchedulerDisable : Admin_Resources.ErrorTaskSchedulerDisable, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            return isTaskSchedulerUpdated;
        }

        public ERPTaskSchedulerModel CreateScheduler(ERPTaskSchedulerModel erpTaskSchedulerModel, IList<View_ReturnBoolean> createResult)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            ZnodeLogging.LogMessage(Admin_Resources.SuccessERPTaskSchedulerSave, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            bool isCreatedRecord = false;

            if (erpTaskSchedulerModel.ERPTaskSchedulerId == 0)
                isCreatedRecord = true;

            erpTaskSchedulerModel.ERPTaskSchedulerId = createResult.FirstOrDefault().Id;
            ParameterModel erpTaskSchedulerIds = new ParameterModel();
            erpTaskSchedulerIds.Ids = Convert.ToString(erpTaskSchedulerModel.ERPTaskSchedulerId);
            bool schedulerStatus = true;
            try
            {
                string hangfireJobId = string.Empty;
                SetSchedulerParameters(erpTaskSchedulerModel, ',');


                if (erpTaskSchedulerModel.IsEnabled)
                {
                    schedulerStatus = _eRPJob.ConfigureJobs(erpTaskSchedulerModel, out hangfireJobId);
                    if (schedulerStatus && !string.IsNullOrEmpty(hangfireJobId))
                    {
                        erpTaskSchedulerModel.HangfireJobId = hangfireJobId;

                        //Update the Hangfire Job Id in the DB.
                        InsertERPTaskScheduler(erpTaskSchedulerModel, out int status, out createResult);
                    }
                }
                else
                {
                    schedulerStatus = _eRPJob.RemoveJob(erpTaskSchedulerModel);
                    if (!erpTaskSchedulerModel.IsEnabled)
                    {
                        erpTaskSchedulerModel.HangfireJobId = string.Empty;
                        erpTaskSchedulerModel.CronExpression = string.Empty;
                    }
                    InsertERPTaskScheduler(erpTaskSchedulerModel, out int status, out createResult);
                }
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(string.Format(Admin_Resources.ErrorInScheduleTask, ex.StackTrace), ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error, ex);
            }
            finally
            {
                if (!schedulerStatus && isCreatedRecord)
                {
                    bool deleteResult = Delete(erpTaskSchedulerIds);

                    if (!deleteResult)
                    {
                        erpTaskSchedulerModel.ERPTaskSchedulerId = -1;
                    }
                }
            }
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            return erpTaskSchedulerModel;
        }

        #endregion

        #region Protected Methods
        // Check scheduler Name is exist or not. 
        protected virtual bool IsSchedulerNameAlreadyExist(string schedulerName)
        => _erpTaskSchedulerRepository.Table.Any(x => x.SchedulerName == schedulerName);

        protected virtual void InsertERPTaskScheduler(ERPTaskSchedulerModel erpTaskSchedulerModel, out int status, out IList<View_ReturnBoolean> createResult)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            if (erpTaskSchedulerModel.ERPTaskSchedulerId == 0)
            {
                erpTaskSchedulerModel.ERPConfiguratorId = _eRPConfiguratorRepository.Table.Where(x => x.IsActive).Select(g => g.ERPConfiguratorId).FirstOrDefault();
                erpTaskSchedulerModel.ERPTaskSchedulerId = _erpTaskSchedulerRepository.Table.Where(x => x.ERPConfiguratorId == erpTaskSchedulerModel.ERPConfiguratorId && x.TouchPointName == erpTaskSchedulerModel.TouchPointName).Select(g => g.ERPTaskSchedulerId).FirstOrDefault();
            }

      

            var xmlData = HelperUtility.ToXML(erpTaskSchedulerModel);
            IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
            objStoredProc.SetParameter("SchedulerXML", xmlData, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("UserId", HelperMethods.GetLoginUserId(), ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Boolean);
            status = 0;
            createResult = objStoredProc.ExecuteStoredProcedureList("Znode_InsertUpdateERPScheduler @SchedulerXML,@UserId, @status OUT", 2, out status);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
        }

        //Set Scheduler Parameters
        protected virtual void SetSchedulerParameters(ERPTaskSchedulerModel erpTaskSchedulerModel, char separator = ' ')
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
            string apiDomainUrl = $"{HttpContext.Current.Request.Url.Scheme + "://"}{HttpContext.Current.Request.Url.Authority}";

            string tokenValue = string.IsNullOrEmpty(HttpContext.Current.Request.Headers[ZnodeConstant.Token]) ? "0" : HttpContext.Current.Request.Headers[ZnodeConstant.Token];

            switch (erpTaskSchedulerModel.SchedulerCallFor)
            {
                case ZnodeConstant.SearchIndex:
                    ZnodeSearchIndexMonitor searchIndexMonitor;
                    searchIndexMonitor = SearchIndexMonitorInsert(erpTaskSchedulerModel);
                    SearchHelper searchHelper = new SearchHelper();
                    var searchIndexServerStatusId = searchHelper.CreateSearchIndexServerStatus(new SearchIndexServerStatusModel()
                    {
                        SearchIndexMonitorId = searchIndexMonitor.SearchIndexMonitorId,
                        ServerName = Environment.MachineName,
                        Status = ZnodeConstant.SearchIndexStartedStatus
                    }).SearchIndexServerStatusId;

                    erpTaskSchedulerModel.ExeParameters = $"{erpTaskSchedulerModel.SchedulerCallFor}{separator}{erpTaskSchedulerModel.CatalogId}{separator}{erpTaskSchedulerModel.IndexName}{separator}{erpTaskSchedulerModel.CatalogIndexId}{separator}{searchIndexMonitor.SearchIndexMonitorId}{separator}SchedulerInUse{separator}{apiDomainUrl}{separator}{GetLoginUserId()}{separator}{searchIndexServerStatusId}{separator}PRODUCTION{separator}{HttpContext.Current.Request.Headers["Authorization"]?.Replace("Basic ", "")}{separator}{tokenValue}{separator}{ZnodeApiSettings.RequestTimeout}";
                    break;
                case ZnodeConstant.ERP:
                    erpTaskSchedulerModel.ExeParameters = $"{erpTaskSchedulerModel.SchedulerCallFor}{separator}{erpTaskSchedulerModel.ERPTaskSchedulerId}{separator}{PortalId}{separator}{apiDomainUrl}{separator}{erpTaskSchedulerModel.SchedulerName}{separator}{GetLoginUserId()}{separator}{HttpContext.Current.Request.Headers[ZnodeConstant.Authorization]?.Replace("Basic ", "")}{separator}{tokenValue}{separator}{ZnodeApiSettings.RequestTimeout}";
                    break;
                case ZnodeConstant.ProductFeed:
                    SetProductFeedParameter(erpTaskSchedulerModel, GetLoginUserId());
                    erpTaskSchedulerModel.ExeParameters = $"{erpTaskSchedulerModel.SchedulerCallFor}{separator}{erpTaskSchedulerModel.ExeParameters}";
                    break;
                case ZnodeConstant.PublishCatalog:
                    erpTaskSchedulerModel.ExeParameters = GetEXEParameterForPublishCatalog(erpTaskSchedulerModel, apiDomainUrl, separator);
                    break;
                case ZnodeConstant.RecommendationDataGeneration:
                    erpTaskSchedulerModel.ExeParameters = GetEXEParameterForRecommendationData(erpTaskSchedulerModel, apiDomainUrl, separator);
                    break;
                case ZnodeConstant.VoucherExpirationReminderEmail:
                    erpTaskSchedulerModel.ExeParameters = GetEXEParameterForVoucherExpirationReminderEmail(erpTaskSchedulerModel, apiDomainUrl, separator);
                    break;
                case ZnodeConstant.ClearUserRegistrationAttempt:
                    erpTaskSchedulerModel.ExeParameters = GetEXEParameterForClearUserRegistrationAttemptDetail(erpTaskSchedulerModel, apiDomainUrl, separator);
                    break;
                case ZnodeConstant.DeletePaymentAuthTokenHelper:
                    erpTaskSchedulerModel.ExeParameters = GetEXEParameterForDeletePaymentAuthTokens(erpTaskSchedulerModel, apiDomainUrl, separator);
                    break;
                case ZnodeConstant.PublishContentContainer:
                    erpTaskSchedulerModel.ExeParameters = GetEXEParameterForPublishContentContainer(erpTaskSchedulerModel, apiDomainUrl, separator);
                    break;
                case ZnodeConstant.StockNotification:
                    erpTaskSchedulerModel.ExeParameters = GetEXEParameterForStockNotice(erpTaskSchedulerModel, apiDomainUrl, separator);
                    break;
                case ZnodeConstant.DeleteExportHelper:
                    erpTaskSchedulerModel.ExeParameters = GetEXEParameterForDeleteExportFiles(erpTaskSchedulerModel, apiDomainUrl, separator);
                    break;
            }
            ZnodeLogging.LogMessage("ExeParameters value: ", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Verbose, erpTaskSchedulerModel?.ExeParameters);
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
        }

        //Insert into ZnodeSearch indexMonitor.
        protected virtual ZnodeSearchIndexMonitor SearchIndexMonitorInsert(ERPTaskSchedulerModel portalIndexModel)
        {
            return _searchIndexMonitorRepository.Insert(new ZnodeSearchIndexMonitor()
            {
                SourceId = 0,
                CatalogIndexId = portalIndexModel.CatalogIndexId,
                SourceType = "CreateIndex",
                SourceTransactionType = "INSERT",
                AffectedType = "CreateIndex",
                CreatedBy = portalIndexModel.CreatedBy,
                CreatedDate = portalIndexModel.CreatedDate,
                ModifiedBy = portalIndexModel.ModifiedBy,
                ModifiedDate = portalIndexModel.ModifiedDate
            });
        }

        //Get executable parameter for recommendation data generation
        protected virtual string GetEXEParameterForRecommendationData(ERPTaskSchedulerModel erpTaskSchedulerModel, string apiDomainUrl, char separator = ' ')
        {
            string[] args = erpTaskSchedulerModel.TouchPointName.Split('_');
            string tokenValue = string.IsNullOrEmpty(HttpContext.Current.Request.Headers[ZnodeConstant.Token]) ? "0" : HttpContext.Current.Request.Headers[ZnodeConstant.Token];
            return $"{erpTaskSchedulerModel.SchedulerCallFor}{separator}{args[1]}{separator}{args[2]}{separator}{apiDomainUrl}{separator}{GetLoginUserId()}{separator}{HttpContext.Current.Request.Headers[ZnodeConstant.Authorization]?.Replace("Basic ", "")}{separator}{tokenValue}{separator}{ZnodeApiSettings.RequestTimeout}";
        }

        //Get executable parameter for Voucher expiration Reminder email data
        protected virtual string GetEXEParameterForVoucherExpirationReminderEmail(ERPTaskSchedulerModel erpTaskSchedulerModel, string apiDomainUrl, char separator = ' ')
        {
            string tokenValue = string.IsNullOrEmpty(HttpContext.Current.Request.Headers[ZnodeConstant.Token]) ? "0" : HttpContext.Current.Request.Headers[ZnodeConstant.Token];
            string domainName = apiDomainUrl.Replace("http://", string.Empty).Replace("https://", string.Empty);
            var domainConfig = ZnodeConfigManager.GetDomainConfig(domainName);
            if (IsNotNull(domainConfig) && tokenValue == "0")
            {
                string authorizationTokenValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(domainConfig.DomainName + "|" + domainConfig.ApiKey));
                if(string.IsNullOrEmpty(HttpContext.Current.Request.Headers[ZnodeConstant.Authorization]))
                    HttpContext.Current.Request.Headers.Add(ZnodeConstant.Authorization, authorizationTokenValue);
                tokenValue = ZnodeTokenHelper.GenerateTokenKey(authorizationTokenValue);
            }
            return $"{erpTaskSchedulerModel.SchedulerCallFor}{separator}{apiDomainUrl}{separator}{GetLoginUserId()}{separator}{HttpContext.Current.Request.Headers[ZnodeConstant.Authorization]?.Replace("Basic ", "")}{separator}{tokenValue}{separator}{ZnodeApiSettings.RequestTimeout}";
        }

        //Get executable parameter for clear user registration attempt details.
        protected virtual string GetEXEParameterForClearUserRegistrationAttemptDetail(ERPTaskSchedulerModel erpTaskSchedulerModel, string apiDomainUrl, char separator = ' ')
        {
            try
            {
                ZnodeLogging.LogMessage("Set ExeParameter for user registration attempt process is started.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
                string tokenValue = string.IsNullOrEmpty(HttpContext.Current.Request.Headers[ZnodeConstant.Token]) ? "0" : HttpContext.Current.Request.Headers[ZnodeConstant.Token];
                string domainName = apiDomainUrl.Replace("http://", string.Empty).Replace("https://", string.Empty);
                var domainConfig = ZnodeConfigManager.GetDomainConfig(domainName);
                if (IsNotNull(domainConfig) && tokenValue == "0")
                {
                    string authorizationTokenValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(domainConfig.DomainName + "|" + domainConfig.ApiKey));
                    if (string.IsNullOrEmpty(HttpContext.Current.Request.Headers[ZnodeConstant.Authorization]))
                        HttpContext.Current.Request.Headers.Add(ZnodeConstant.Authorization, authorizationTokenValue);
                    tokenValue = ZnodeTokenHelper.GenerateTokenKey(authorizationTokenValue);
                }
                ZnodeLogging.LogMessage("Set ExeParameter for user registration attempt process is done.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);

                //Create string executable parameter
                return $"{erpTaskSchedulerModel.SchedulerCallFor}{separator}{apiDomainUrl}{separator}" +
                    $"{GetLoginUserId()}{separator}{HttpContext.Current.Request.Headers[ZnodeConstant.Authorization]?.Replace("Basic ", "")}" +
                    $"{separator}{tokenValue}{separator}{ZnodeApiSettings.RequestTimeout}";
            }
            catch(Exception ex)
            {
                ZnodeLogging.LogMessage(string.Format("Error in ExeParameter for user registration attempt", ex.StackTrace), ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error, ex);
                return string.Empty;
            }
        }

        //Get executable parameter for publish content container
        protected virtual string GetEXEParameterForPublishContentContainer(ERPTaskSchedulerModel erpTaskSchedulerModel, string apiDomainUrl, char separator = ' ')
        {
            string[] args = erpTaskSchedulerModel.TouchPointName.Split('_');
            string tokenValue = string.IsNullOrEmpty(HttpContext.Current.Request.Headers[ZnodeConstant.Token]) ? "0" : HttpContext.Current.Request.Headers[ZnodeConstant.Token];
            return $"{erpTaskSchedulerModel.SchedulerCallFor}{separator}{args[1]}{separator}{args[2]}{separator}{apiDomainUrl}{separator}{GetLoginUserId()}{separator}{HttpContext.Current.Request.Headers[ZnodeConstant.Authorization]?.Replace("Basic ", "")}{separator}{tokenValue}{separator}{ZnodeApiSettings.RequestTimeout}";
        }

        //Get executable parameter for publish catalog
        protected virtual string GetEXEParameterForPublishCatalog(ERPTaskSchedulerModel erpTaskSchedulerModel, string apiDomainUrl, char separator = ' ')
        {
            string[] args = erpTaskSchedulerModel.TouchPointName.Split('_');
            string tokenValue = string.IsNullOrEmpty(HttpContext.Current.Request.Headers[ZnodeConstant.Token]) ? "0" : HttpContext.Current.Request.Headers[ZnodeConstant.Token];
            return $"{erpTaskSchedulerModel.SchedulerCallFor}{separator}{args[1]}{separator}{args[2]}{separator}{apiDomainUrl}{separator}{GetLoginUserId()}{separator}{HttpContext.Current.Request.Headers[ZnodeConstant.Authorization]?.Replace("Basic ", "")}{separator}{tokenValue}{separator}{ZnodeApiSettings.RequestTimeout}";
        }

        #region Get exe parameter for delete payment auth tokens
        protected virtual string GetEXEParameterForDeletePaymentAuthTokens(ERPTaskSchedulerModel erpTaskSchedulerModel, string apiDomainUrl, char separator = ' ')
        {
            string tokenValue = string.IsNullOrEmpty(HttpContext.Current.Request.Headers[ZnodeConstant.Token]) ? "0" : HttpContext.Current.Request.Headers[ZnodeConstant.Token];
            string domainName = apiDomainUrl.Replace("http://", string.Empty).Replace("https://", string.Empty);
            var domainConfig = ZnodeConfigManager.GetDomainConfig(domainName);
            if (IsNotNull(domainConfig) && tokenValue == "0")
            {
                string authorizationTokenValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(domainConfig.DomainName + "|" + domainConfig.ApiKey));
                if (string.IsNullOrEmpty(HttpContext.Current.Request.Headers[ZnodeConstant.Authorization]))
                    HttpContext.Current.Request.Headers.Add(ZnodeConstant.Authorization, authorizationTokenValue);
                tokenValue = ZnodeTokenHelper.GenerateTokenKey(authorizationTokenValue);
            }
            return $"{erpTaskSchedulerModel.SchedulerCallFor}{separator}{apiDomainUrl}{separator}{GetLoginUserId()}{separator}" +
                $"{HttpContext.Current.Request.Headers[ZnodeConstant.Authorization]?.Replace("Basic ", "")}" +
                $"{separator}{tokenValue}{separator}{ZnodeApiSettings.RequestTimeout}";
        }
        #endregion
        #region  Get exe parameter for delete export files
        //Get executable parameter for Delete Export Files.
        protected virtual string GetEXEParameterForDeleteExportFiles(ERPTaskSchedulerModel erpTaskSchedulerModel, string apiDomainUrl, char separator = ' ')
        {
            string tokenValue = string.IsNullOrEmpty(HttpContext.Current.Request.Headers[ZnodeConstant.Token]) ? "0" : HttpContext.Current.Request.Headers[ZnodeConstant.Token];
            string domainName = apiDomainUrl.Replace("http://", string.Empty).Replace("https://", string.Empty);
            var domainConfig = ZnodeConfigManager.GetDomainConfig(domainName);
            if (IsNotNull(domainConfig) && tokenValue == "0")
            {
                string authorizationTokenValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(domainConfig.DomainName + "|" + domainConfig.ApiKey));
                if (string.IsNullOrEmpty(HttpContext.Current.Request.Headers[ZnodeConstant.Authorization]))
                    HttpContext.Current.Request.Headers.Add(ZnodeConstant.Authorization, authorizationTokenValue);
                tokenValue = ZnodeTokenHelper.GenerateTokenKey(authorizationTokenValue);
            }
            return $"{erpTaskSchedulerModel.SchedulerCallFor}{separator}{apiDomainUrl}{separator}{GetLoginUserId()}{separator}" +
                $"{HttpContext.Current.Request.Headers[ZnodeConstant.Authorization]?.Replace("Basic ", "")}" +
                $"{separator}{tokenValue}{separator}{ZnodeApiSettings.RequestTimeout}";
        }
        #endregion
        #region Stock Notice

        // Get executable parameters for stock notice.
        protected virtual string GetEXEParameterForStockNotice(ERPTaskSchedulerModel erpTaskSchedulerModel, string apiDomainUrl, char separator = ' ')
        {
            try
            {
                ZnodeLogging.LogMessage("Set ExeParameter for stock notice.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
                string tokenValue = string.IsNullOrEmpty(HttpContext.Current.Request.Headers[ZnodeConstant.Token]) ? "0" : HttpContext.Current.Request.Headers[ZnodeConstant.Token];
                string domainName = apiDomainUrl.Replace("http://", string.Empty).Replace("https://", string.Empty);
                var domainConfig = ZnodeConfigManager.GetDomainConfig(domainName);
                if (IsNotNull(domainConfig) && tokenValue == "0")
                {
                    string authorizationTokenValue = Convert.ToBase64String(Encoding.UTF8.GetBytes(domainConfig.DomainName + "|" + domainConfig.ApiKey));
                    if (string.IsNullOrEmpty(HttpContext.Current.Request.Headers[ZnodeConstant.Authorization]))
                        HttpContext.Current.Request.Headers.Add(ZnodeConstant.Authorization, authorizationTokenValue);
                    tokenValue = ZnodeTokenHelper.GenerateTokenKey(authorizationTokenValue);
                }
                ZnodeLogging.LogMessage("Set ExeParameter for stock notice done.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);

                // Create string executable parameter
                return $"{erpTaskSchedulerModel.SchedulerCallFor}{separator}{apiDomainUrl}{separator}" +
                    $"{GetLoginUserId()}{separator}{HttpContext.Current.Request.Headers[ZnodeConstant.Authorization]?.Replace("Basic ", "")}" +
                    $"{separator}{tokenValue}{separator}{ZnodeApiSettings.RequestTimeout}";
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage($"Error in ExeParameter for stock notice {ex.Message}", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error);
                return string.Empty;
            }
        }
        #endregion

        protected virtual void SetProductFeedParameter(ERPTaskSchedulerModel erpTaskSchedulerModel, int userId)
        {
            ZnodeLogging.LogMessage("Execution started.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);

            IProductFeedService productFeedService = GetService<IProductFeedService>();

            NameValueCollection expands = SetExpandsForProductFeed();

            string tokenValue = string.IsNullOrEmpty(HttpContext.Current.Request.Headers[ZnodeConstant.Token]) ? "0" : HttpContext.Current.Request.Headers[ZnodeConstant.Token];
            ProductFeedModel productFeedModel = productFeedService.GetProductFeed(Convert.ToInt32(erpTaskSchedulerModel.TouchPointName.Split('_')[1]), expands);
            string storeIds = Convert.ToString(productFeedModel.PortalId);

            if (IsNotNull(productFeedModel))
                erpTaskSchedulerModel.ExeParameters = $"{productFeedModel.ProductFeedId}#{productFeedModel.LocaleId}#{productFeedModel.ProductFeedSiteMapTypeCode}#{productFeedModel.ProductFeedTypeCode}#{productFeedModel.Title}#{productFeedModel.Link}#{productFeedModel.Description}#{productFeedModel.FileName}#{storeIds}#{erpTaskSchedulerModel.DomainName}#{erpTaskSchedulerModel.SchedulerName}#{userId}#{HttpContext.Current.Request.Headers[ZnodeConstant.Authorization]?.Replace("Basic ", "")}#{tokenValue}#{ZnodeApiSettings.RequestTimeout}";
            ZnodeLogging.LogMessage("Execution done.", ZnodeLogging.Components.ERP.ToString(), TraceLevel.Info);
        }

        protected virtual NameValueCollection SetExpandsForProductFeed()
        {
            NameValueCollection expands = new NameValueCollection();
            expands.Add(ZnodeProductFeedEnum.ZnodeProductFeedSiteMapType.ToString().ToLower(), ZnodeProductFeedEnum.ZnodeProductFeedSiteMapType.ToString().ToLower());
            expands.Add(ZnodeProductFeedEnum.ZnodeProductFeedType.ToString().ToLower(), ZnodeProductFeedEnum.ZnodeProductFeedType.ToString().ToLower());
            expands.Add(ZnodeProductFeedEnum.ZnodePortal.ToString().ToLower(), ZnodeProductFeedEnum.ZnodePortal.ToString().ToLower());
            return expands;
        }
        
        /// <summary>
        /// Method to check the status of Touch point
        /// </summary>
        /// <param name="erpTaskSchedulerId"></param>
        /// <returns></returns>
        protected bool CheckTouchPointStauts(int erpTaskSchedulerId)
        {
           return _erpTaskSchedulerRepository.Table.Where(item =>item.ERPTaskSchedulerId == erpTaskSchedulerId && item.IsEnabled).Any();
        }
        #endregion

    }
}
