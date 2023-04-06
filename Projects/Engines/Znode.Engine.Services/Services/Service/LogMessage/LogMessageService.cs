using MongoDB.Driver;
using MongoDB.Driver.Builders;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Diagnostics;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Engine.Services.Maps;
using Znode.Libraries.Observer;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.Data.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.MongoDB.Data;
using Utilities = Znode.Libraries.ECommerce.Utilities;
using System.Reflection;
using Znode.Libraries.Resources;

namespace Znode.Engine.Services
{
    public class LogMessageService : BaseService, ILogMessageService
    {
        #region Private variables
        private readonly IMongoRepository<LogMessageEntity> _logMessageMongoRepository;
        private readonly IZnodeRepository<ZnodeGlobalSetting> _globalSettingRepository;
        private readonly IZnodeRepository<ZnodeDomain> _domainRepository;
        private readonly IZnodeRepository<ZnodeImpersonationLog> _impersonationLogRepository;
        private readonly IZnodeRepository<ZnodeUser> _userRepository;
        private readonly IZnodeRepository<ZnodePortal> _znodePortal;
        #endregion

        #region Constructor
        public LogMessageService()
        {
            _logMessageMongoRepository = new MongoRepository<LogMessageEntity>(true);
            _globalSettingRepository = new ZnodeRepository<ZnodeGlobalSetting>();
            _domainRepository = new ZnodeRepository<ZnodeDomain>();
            _impersonationLogRepository = new ZnodeRepository<ZnodeImpersonationLog>();
            _userRepository = new ZnodeRepository<ZnodeUser>();
            _znodePortal = new ZnodeRepository<ZnodePortal>();
        }
        #endregion


        #region Public Method
        // List of log message
        public LogMessageListModel GetLogMessageList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ReplaceFilterKeys(ref filters);
            ReplaceSortKeys(ref sorts);

            List<LogMessageEntity> errorLogs = null;

            //Add date time value in filter collection against filter column name created date.
            filters = ServiceHelper.AddDateTimeValueInFilter(filters);

            // generate query for date filter 1st for without date and after with date and add it to query
            FilterTuple createdDateTuple = FilterRemoveOnDate(filters);

            bool applicationTypeTuplePresent = false;
            FilterTuple applicationTypeTuple = new FilterTuple(string.Empty, string.Empty, string.Empty);

            if (filters?.Count > 0 && filters.Any(x => x.FilterName == FilterKeys.ApplicationTypeCase))
            {
                //Generate the or query.
                applicationTypeTuplePresent = true;
                applicationTypeTuple = filters.Find(x => string.Equals(x.FilterName, FilterKeys.ApplicationTypeCase, StringComparison.InvariantCultureIgnoreCase));
                FilterRemoveApplicationType(filters);
            }

            List<IMongoQuery> withoutCreatedDateQuery = new List<IMongoQuery>();
            withoutCreatedDateQuery.Add(MongoQueryHelper.GenerateDynamicWhereClause(filters.ToFilterMongoCollection()));
            DateFilter(filters, createdDateTuple, withoutCreatedDateQuery);

            // bind to page list and get the list of log message 
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            //get list from mongo
            errorLogs = _logMessageMongoRepository.GetPagedList(withoutCreatedDateQuery?.Count > 0 ? Query.And(withoutCreatedDateQuery) : null, pageListModel.MongoOrderBy, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount);
            //map logmessage entity to logmessage model
            LogMessageListModel logMessage = new LogMessageListModel() { LogMessageList = errorLogs?.ToModel<LogMessageModel>()?.ToList() };

            logMessage = AddApplicationTypeColumn(logMessage);
            if (applicationTypeTuplePresent)
            {
                logMessage = AddPagingWhenApplicationTypeFilterPresent(filters, applicationTypeTuple, logMessage, sorts, page, pageListModel);
                return logMessage;
            }
            logMessage.BindPageListModel(pageListModel);
            return logMessage;
        }

        // List of integration log message
        public LogMessageListModel GetIntegrationLogMessageList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ReplaceFilterKeys(ref filters);
            ReplaceSortKeys(ref sorts);

            List<LogMessageEntity> errorLogs = null;

            //Add date time value in filter collection against filter column name created date.
            filters = ServiceHelper.AddDateTimeValueInFilter(filters);

            // generate query for date filter 1st for without date and after with date and add it to query
            FilterTuple createdDateTuple = FilterRemoveOnDate(filters);

            bool applicationTypeTuplePresent = false;
            FilterTuple applicationTypeTuple = new FilterTuple(string.Empty, string.Empty, string.Empty);

            if (filters?.Count > 0 && filters.Any(x => x.FilterName == FilterKeys.ApplicationTypeCase))
            {
                //Generate the or query.
                applicationTypeTuplePresent = true;
                applicationTypeTuple = filters.Find(x => string.Equals(x.FilterName, FilterKeys.ApplicationTypeCase, StringComparison.InvariantCultureIgnoreCase));
                FilterRemoveApplicationType(filters);
            }

            List<IMongoQuery> withoutCreatedDateQuery = new List<IMongoQuery>();
            withoutCreatedDateQuery.Add(MongoQueryHelper.GenerateDynamicWhereClause(filters.ToFilterMongoCollection()));
            DateFilter(filters, createdDateTuple, withoutCreatedDateQuery);
            withoutCreatedDateQuery.Add(Query.Or(Query<LogMessageEntity>.EQ(pr => pr.Component, ZnodeLogging.Components.Shipping.ToString()), Query<LogMessageEntity>.EQ(pr => pr.Component, ZnodeLogging.Components.ERP.ToString()), Query<LogMessageEntity>.EQ(pr => pr.Component, ZnodeLogging.Components.Payment.ToString())));
            // bind to page list and get the list of log message 
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            //get list from mongo
            errorLogs = _logMessageMongoRepository.GetPagedList(withoutCreatedDateQuery?.Count > 0 ? Query.And(withoutCreatedDateQuery) : null, pageListModel.MongoOrderBy, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount);
            //map logmessage entity to logMessage model
            LogMessageListModel logMessage = new LogMessageListModel() { LogMessageList = errorLogs?.ToModel<LogMessageModel>()?.ToList() };

            logMessage = AddApplicationTypeColumn(logMessage);
            if (applicationTypeTuplePresent)
            {
                logMessage = AddPagingWhenApplicationTypeFilterPresent(filters, applicationTypeTuple, logMessage, sorts, page, pageListModel);
                return logMessage;
            }
            logMessage.BindPageListModel(pageListModel);
            return logMessage;
        }

        // List of event log message
        public LogMessageListModel GetEventLogMessageList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            ReplaceFilterKeys(ref filters);
            ReplaceSortKeys(ref sorts);

            List<LogMessageEntity> errorLogs = null;

            //Add date time value in filter collection against filter column name created date.
            filters = ServiceHelper.AddDateTimeValueInFilter(filters);

            // generate query for date filter 1st for without date and after with date and add it to query
            FilterTuple createdDateTuple = FilterRemoveOnDate(filters);

            bool applicationTypeTuplePresent = false;
            FilterTuple applicationTypeTuple = new FilterTuple(string.Empty, string.Empty, string.Empty);

            if (filters?.Count > 0 && filters.Any(x => x.FilterName == FilterKeys.ApplicationTypeCase))
            {
                //Generate the or query.
                applicationTypeTuplePresent = true;
                applicationTypeTuple = filters.Find(x => string.Equals(x.FilterName, FilterKeys.ApplicationTypeCase, StringComparison.InvariantCultureIgnoreCase));
                FilterRemoveApplicationType(filters);
            }

            List<IMongoQuery> withoutCreatedDateQuery = new List<IMongoQuery>();
            withoutCreatedDateQuery.Add(MongoQueryHelper.GenerateDynamicWhereClause(filters.ToFilterMongoCollection()));
            withoutCreatedDateQuery.Add(Query.Or(Query<LogMessageEntity>.EQ(pr => pr.Component, ZnodeLogging.Components.Search.ToString()), Query<LogMessageEntity>.EQ(pr => pr.Component, ZnodeLogging.Components.Import.ToString()), Query<LogMessageEntity>.EQ(pr => pr.Component, ZnodeLogging.Components.ImageScheduler.ToString())));

            DateFilter(filters, createdDateTuple, withoutCreatedDateQuery);

            // bind to page list and get the list of log message 
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            //get list from mongo
            errorLogs = _logMessageMongoRepository.GetPagedList(withoutCreatedDateQuery?.Count > 0 ? Query.And(withoutCreatedDateQuery) : null, pageListModel.MongoOrderBy, pageListModel.PagingStart, pageListModel.PagingLength, out pageListModel.TotalRowCount);
            //map logmessage entity to logMessage model
            LogMessageListModel logMessage = new LogMessageListModel() { LogMessageList = errorLogs?.ToModel<LogMessageModel>()?.ToList() };

            logMessage = AddApplicationTypeColumn(logMessage);
            if (applicationTypeTuplePresent)
            {
                logMessage = AddPagingWhenApplicationTypeFilterPresent(filters, applicationTypeTuple, logMessage, sorts, page, pageListModel);
                return logMessage;
            }
            logMessage.BindPageListModel(pageListModel);
            return logMessage;
        }

        // List of database log message
        public virtual LogMessageListModel GetDatabaseLogMessageList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            //Add date time value in filter collection against filter column name created date.
            filters = ServiceHelper.AddDateTimeValueInFilter(filters);

            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            string logMessageId = string.Empty;
            IZnodeViewRepository<LogMessageModel> objStoredProc = new ZnodeViewRepository<LogMessageModel>();

            //SP parameters
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@LogMessageId", logMessageId, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);

            IList<LogMessageModel> logmessagelist = objStoredProc.ExecuteStoredProcedureList("Znode_GetProcedureErrorLog  @WhereClause,@Rows,@PageNo,@Order_By,@LogMessageId,@RowCount OUT", 5, out pageListModel.TotalRowCount);

            LogMessageListModel listModel = new LogMessageListModel { LogMessageList = logmessagelist?.ToList() };
            listModel.BindPageListModel(pageListModel);

            return listModel;
        }

        // List of database log message details
        public LogMessageModel GetDatabaseLogMessage(string logMessageId, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            IZnodeViewRepository<LogMessageModel> objStoredProc = new ZnodeViewRepository<LogMessageModel>();

            //SP parameters
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@LogMessageId", logMessageId, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);

            return objStoredProc.ExecuteStoredProcedureList("Znode_GetProcedureErrorLog  @WhereClause,@Rows,@PageNo,@Order_By,@LogMessageId,@RowCount OUT", 5, out pageListModel.TotalRowCount).FirstOrDefault();
        }

        // List of log message by id
        public LogMessageModel GetLogMessage(string logmessageId, NameValueCollection expands)
            => _logMessageMongoRepository.Table.MongoCollection.FindOne(
                (Query.And(Query<LogMessageEntity>.EQ(pr => pr.LogMessageId, logmessageId))))?.ToModel<LogMessageModel>();

        // Log configuration
        public virtual LogMessageConfigurationModel GetLogConfiguration()
        {

            LogMessageConfigurationModel logMessageConfigurationModel = new LogMessageConfigurationModel();

            List<string> loggingConfiguration = Enum.GetNames(typeof(LoggingSettingEnum)).Cast<string>().ToList();
            List<ZnodeGlobalSetting> globalSettingList = _globalSettingRepository.Table.Where(x => loggingConfiguration.Contains(x.FeatureName)).ToList();
            foreach (var item in globalSettingList)
            {
                SetlogMessageConfigurationModel(logMessageConfigurationModel, item);
            }
            return logMessageConfigurationModel;
        }

        //Set log configuration model
        public void SetlogMessageConfigurationModel(LogMessageConfigurationModel logMessageConfigurationModel, ZnodeGlobalSetting item)
        {
            PropertyInfo propInfo = logMessageConfigurationModel.GetType().GetProperty(item.FeatureName);
            if (!Equals(propInfo, null))
            {
                propInfo.SetValue(logMessageConfigurationModel, Convert.ToBoolean(item.FeatureValues));
            }
        }



        //Method To Update Existing General Setting

        public virtual bool UpdateLogConfiguration(LogMessageConfigurationModel model)
        {
            List<string> loggingConfigurationList = Enum.GetNames(typeof(LoggingSettingEnum)).Cast<string>().ToList();
            bool updated = loggingConfigurationList.All(m => UpdateAllLoggingLevel(model, m));
            if (updated)
            {
                IDefaultGlobalConfigService _service = ZnodeDependencyResolver.GetService<IDefaultGlobalConfigService>();
                Dictionary<string, string> globalSettingData = _service.GetLoggingGlobalConfigList();
                var clearCache = new ZnodeEventNotifier<Dictionary<string, string>>(globalSettingData);

                ZnodeCacheDependencyManager.Insert(CachedKeys.DefaultLoggingConfigCache, globalSettingData, "ZnodeGlobalSetting");
            }
            return updated;
        }


        public virtual bool PurgeLogs(ParameterModel logCategoryIds)
        {
            if (HelperUtility.IsNull(logCategoryIds) || string.IsNullOrEmpty(logCategoryIds.Ids))
                throw new ZnodeException(ErrorCodes.IdLessThanOne, Admin_Resources.LogCategoryIdNotNull);
            bool IsDeleted = false;
            int categoryId = 0;
            Int32.TryParse(logCategoryIds.Ids, out categoryId);
            switch (categoryId)
            {
                case 0:
                    IsDeleted = _logMessageMongoRepository.DeleteByQuery(Query<LogMessageEntity>.NE(p => p.LogMessageId, null));
                    break;
                case 1:
                    IsDeleted = _logMessageMongoRepository.DeleteByQuery(Query.Or(Query<LogMessageEntity>.EQ(pr => pr.Component, ZnodeLogging.Components.Search.ToString()), Query<LogMessageEntity>.EQ(pr => pr.Component, ZnodeLogging.Components.Import.ToString()), Query<LogMessageEntity>.EQ(pr => pr.Component, ZnodeLogging.Components.ImageScheduler.ToString())));
                    ZnodeLogging.LogMessage(IsDeleted ? Admin_Resources.SuccessEventLogsDelete : Admin_Resources.ErrorEventLogsDelete, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
                    break;
                case 2:
                    IsDeleted = _logMessageMongoRepository.DeleteByQuery(Query.Or(Query<LogMessageEntity>.EQ(pr => pr.Component, ZnodeLogging.Components.Shipping.ToString()), Query<LogMessageEntity>.EQ(pr => pr.Component, ZnodeLogging.Components.ERP.ToString()), Query<LogMessageEntity>.EQ(pr => pr.Component, ZnodeLogging.Components.Payment.ToString())));
                    ZnodeLogging.LogMessage(IsDeleted ? Admin_Resources.SuccessIntegrationLogsDelete : Admin_Resources.ErrorIntegrationLogsDelete, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
                    break;
                case 3:
                    int status = 0;
                    IZnodeViewRepository<View_ReturnBoolean> objStoredProc = new ZnodeViewRepository<View_ReturnBoolean>();
                    objStoredProc.SetParameter("ProcedureName", string.Empty, ParameterDirection.Input, DbType.String);
                    objStoredProc.SetParameter("Status", null, ParameterDirection.Output, DbType.Int32);
                    IList<View_ReturnBoolean> deleteResult = objStoredProc.ExecuteStoredProcedureList("Znode_DeleteProcedureErrorLog @ProcedureName, @Status OUT", 1, out status);
                    IsDeleted = deleteResult.FirstOrDefault().Status.Value;
                    ZnodeLogging.LogMessage(IsDeleted ? Admin_Resources.SuccessDataBaseLogsDelete : Admin_Resources.ErrorDataBaseLogsDelete, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Info);
                    break;
                default:
                    break;

            }
            return IsDeleted;
        }

        #endregion

        #region Impersonation

        // List of database log message
        public virtual ImpersonationActivityListModel GetImpersonationLogList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page)
        {
            //Add date time value in filter collection against filter column name created date.
            PageListModel pageListModel = new PageListModel(filters, sorts, page);
            EntityWhereClauseModel whereClause = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filters.ToFilterDataCollection());
            IZnodeViewRepository<ImpersonationActivityLogModel> objStoredProc = new ZnodeViewRepository<ImpersonationActivityLogModel>();

            //SP parameters
            objStoredProc.SetParameter("@WhereClause", pageListModel.SPWhereClause, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@Rows", pageListModel.PagingLength, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@PageNo", pageListModel.PagingStart, ParameterDirection.Input, DbType.Int32);
            objStoredProc.SetParameter("@Order_By", pageListModel.OrderBy, ParameterDirection.Input, DbType.String);
            objStoredProc.SetParameter("@RowCount", pageListModel.TotalRowCount, ParameterDirection.Output, DbType.Int32);
            IList<ImpersonationActivityLogModel> list = objStoredProc.ExecuteStoredProcedureList("Znode_GetImpersonationLog  @WhereClause,@Rows,@PageNo,@Order_By,@RowCount OUT", 4, out pageListModel.TotalRowCount);

            ImpersonationActivityListModel listModel = new ImpersonationActivityListModel { LogActivityList = list?.ToList() };
            listModel.BindPageListModel(pageListModel);

            /*if (!string.IsNullOrEmpty(whereClause.WhereClause))
            {
                model = (from impersonation in _impersonationLogRepository.Table
                                                             join user in _userRepository.Table on impersonation.CSRId equals user.UserId
                                                             join webstoreUser in _userRepository.Table on impersonation.WebstoreuserId equals webstoreUser.UserId
                                                              join portal in _znodePortal.Table on impersonation.PortalId equals portal.PortalId
                                                             select new ImpersonationActivityLogModel
                                                             {
                                                                 PortalId = impersonation.PortalId,
                                                                 StoreName = portal.StoreName,
                                                                 CSRName = user.FirstName + " " + user.LastName,
                                                                 ShopperName = webstoreUser.FirstName + " " + webstoreUser.LastName,
                                                                 CSRId = impersonation.CSRId ?? 0,
                                                                 ShopperId = impersonation.WebstoreuserId ?? 0,
                                                                 ActivityId = impersonation.ActivityId ?? 0,
                                                                 ActivityName = impersonation.ActivityType,
                                                                 OperationType = impersonation.OperationType,
                                                                 CreatedDate = impersonation.CreatedDate,
                                                             }).Where(whereClause.WhereClause, whereClause.FilterValues).ToList();
            }
            else
            {
                model = (from impersonation in _impersonationLogRepository.Table
                         join user in _userRepository.Table on impersonation.CSRId equals user.UserId
                         join webstoreUser in _userRepository.Table on impersonation.WebstoreuserId equals webstoreUser.UserId
                         join portal in _znodePortal.Table on impersonation.PortalId equals portal.PortalId
                         select new ImpersonationActivityLogModel
                         {
                             PortalId = impersonation.PortalId,
                             StoreName = portal.StoreName,
                             CSRName = user.FirstName + " " + user.LastName,
                             ShopperName = webstoreUser.FirstName + " " + webstoreUser.LastName,
                             CSRId = impersonation.CSRId ?? 0,
                             ShopperId = impersonation.WebstoreuserId ?? 0,
                             ActivityId = impersonation.ActivityId ?? 0,
                             ActivityName = impersonation.ActivityType,
                             OperationType = impersonation.OperationType,
                             CreatedDate = impersonation.CreatedDate,
                         }).ToList();
            }
            pageListModel.TotalRowCount= model.Count;
            impersonationActivityListModel.BindPageListModel(pageListModel);
            impersonationActivityListModel.LogActivityList = model;//ImpersonationMap.ToImpersonationListModel(model1,null).LogActivityList;
            */
            return listModel;
        }


        #endregion
        #region Private Method





        // replace entity filter by mongo filter
        private void ReplaceFilterKeys(ref FilterCollection filters)
        {
            foreach (FilterTuple tuple in filters)
            {
                if (string.Equals(tuple.Item1, Utilities.FilterKeys.CreatedDate, StringComparison.InvariantCultureIgnoreCase)) { ReplaceFilterKeyName(ref filters, Utilities.FilterKeys.CreatedDate, Utilities.FilterKeys.CreatedDate); }
                if (string.Equals(tuple.Item1, Utilities.FilterKeys.Component, StringComparison.InvariantCultureIgnoreCase)) { ReplaceFilterKeyName(ref filters, Utilities.FilterKeys.Component, Utilities.FilterKeys.Component); }
                if (string.Equals(tuple.Item1, Utilities.FilterKeys.TraceLevel, StringComparison.InvariantCultureIgnoreCase)) { ReplaceFilterKeyName(ref filters, Utilities.FilterKeys.TraceLevel, Utilities.FilterKeys.TraceLevel); }
                if (string.Equals(tuple.Item1, Utilities.FilterKeys.LogMessage, StringComparison.InvariantCultureIgnoreCase)) { ReplaceFilterKeyName(ref filters, Utilities.FilterKeys.LogMessage, Utilities.FilterKeys.LogMessage); }
                if (string.Equals(tuple.Item1, Utilities.FilterKeys.LogMessageId, StringComparison.InvariantCultureIgnoreCase)) { ReplaceFilterKeyName(ref filters, Utilities.FilterKeys.LogMessageId, Utilities.FilterKeys.LogMessageId); }
                if (string.Equals(tuple.Item1, Utilities.FilterKeys.DomainNameCase, StringComparison.InvariantCultureIgnoreCase)) { ReplaceFilterKeyName(ref filters, Utilities.FilterKeys.DomainNameCase, Utilities.FilterKeys.DomainNameCase); }
                if (string.Equals(tuple.Item1, Utilities.FilterKeys.ApplicationTypeCase, StringComparison.InvariantCultureIgnoreCase)) { ReplaceFilterKeyName(ref filters, Utilities.FilterKeys.ApplicationTypeCase, Utilities.FilterKeys.ApplicationTypeCase); }
                if (string.Equals(tuple.Item1, Utilities.FilterKeys.SearchString, StringComparison.InvariantCultureIgnoreCase)) { ReplaceFilterKeyName(ref filters, Utilities.FilterKeys.SearchString, Utilities.FilterKeys.SearchString); }
                if (string.Equals(tuple.Item1, Utilities.FilterKeys.StackTraceMessage, StringComparison.InvariantCultureIgnoreCase)) { ReplaceFilterKeyName(ref filters, Utilities.FilterKeys.StackTraceMessage, Utilities.FilterKeys.StackTraceMessage); }
            }
        }

        // replace entity sort by mongo sort
        private void ReplaceSortKeys(ref NameValueCollection sorts)
        {
            foreach (string key in sorts.Keys)
            {
                if (string.Equals(key, Utilities.FilterKeys.Component, StringComparison.InvariantCultureIgnoreCase)) { ReplaceSortKeyName(ref sorts, Utilities.FilterKeys.Component.ToLower(), Utilities.FilterKeys.Component); }
                if (string.Equals(key, Utilities.FilterKeys.LogMessageId, StringComparison.InvariantCultureIgnoreCase)) { ReplaceSortKeyName(ref sorts, Utilities.FilterKeys.LogMessageId.ToLower(), Utilities.FilterKeys.LogMessageId); }
                if (string.Equals(key, Utilities.FilterKeys.CreatedDate, StringComparison.InvariantCultureIgnoreCase)) { ReplaceSortKeyName(ref sorts, Utilities.FilterKeys.CreatedDate.ToLower(), Utilities.FilterKeys.CreatedDate); }
                if (string.Equals(key, Utilities.FilterKeys.LogMessage, StringComparison.InvariantCultureIgnoreCase)) { ReplaceSortKeyName(ref sorts, Utilities.FilterKeys.LogMessage.ToLower(), Utilities.FilterKeys.LogMessage); }
                if (string.Equals(key, Utilities.FilterKeys.TraceLevel, StringComparison.InvariantCultureIgnoreCase)) { ReplaceSortKeyName(ref sorts, Utilities.FilterKeys.TraceLevel.ToLower(), Utilities.FilterKeys.TraceLevel); }
                if (string.Equals(key, Utilities.FilterKeys.DomainName, StringComparison.InvariantCultureIgnoreCase)) { ReplaceSortKeyName(ref sorts, Utilities.FilterKeys.DomainName.ToLower(), Utilities.FilterKeys.DomainName); }
            }
        }

        // Replace single quote with space &  Trim space  of date
        protected virtual string FormatDate(string date)
            => date = !string.IsNullOrEmpty(date) ? date.Replace("'", "").Trim() : string.Empty;

        // Split the date and add to query
        protected virtual void DateFilter(FilterCollection filters, FilterTuple createdDateTuple, List<IMongoQuery> withoutCreatedDateQuery)
        {
            if (HelperUtility.IsNotNull(createdDateTuple))
            {
                string filterKey = createdDateTuple.Item1;
                string filterOperator = createdDateTuple.Item2;
                string filterValue = createdDateTuple.Item3;
                if (string.Equals(filterKey, FilterKeys.CreatedDate, StringComparison.OrdinalIgnoreCase))
                {
                    if (filterValue.Contains(ZnodeConstant.And))
                    {
                        string[] andDateSplit = filterValue.Split(new string[] { ZnodeConstant.And }, StringSplitOptions.None);
                        withoutCreatedDateQuery.Add(Query<LogMessageEntity>.GTE(l => l.CreatedDate, Convert.ToDateTime(FormatDate(andDateSplit[0]))));
                        withoutCreatedDateQuery.Add(Query<LogMessageEntity>.LT(l => l.CreatedDate, Convert.ToDateTime(FormatDate(andDateSplit[1]))));
                    }
                    else
                    {
                        filters.Add(filterKey, filterOperator, Convert.ToDateTime(FormatDate(filterValue)).ToString());
                        IMongoQuery data = MongoQueryHelper.GenerateDynamicWhereClause(filters.ToFilterMongoCollection());
                        withoutCreatedDateQuery.Add(data);
                    }
                }
            }
        }
        protected virtual FilterTuple FilterRemoveOnDate(FilterCollection filters)
        {
            FilterTuple createdDateTuple = filters.Find(x => string.Equals(x.FilterName, Utilities.FilterKeys.CreatedDate, StringComparison.InvariantCultureIgnoreCase));
            filters.RemoveAll(x => string.Equals(x.FilterName, Utilities.FilterKeys.CreatedDate, StringComparison.InvariantCultureIgnoreCase));
            return createdDateTuple;
        }

        private void FilterRemoveApplicationType(FilterCollection filters)
        {
            filters.RemoveAll(x => string.Equals(x.FilterName, FilterKeys.ApplicationTypeCase, StringComparison.InvariantCultureIgnoreCase));
        }

        private LogMessageListModel ApplyFilterOnApplicationType(FilterTuple applicationTypeTuple, LogMessageListModel logMessage)
        {
            if (applicationTypeTuple.FilterOperator == MongoFilterOperators.Contains)
                logMessage.LogMessageList = logMessage.LogMessageList?.Where(x => x.ApplicationType.ToLower().Contains(applicationTypeTuple.FilterValue.ToLower())).ToList();
            if (applicationTypeTuple.FilterOperator == MongoFilterOperators.StartsWith)
                logMessage.LogMessageList = logMessage.LogMessageList?.Where(x => x.ApplicationType.ToLower().StartsWith(applicationTypeTuple.FilterValue.ToLower())).ToList();
            if (applicationTypeTuple.FilterOperator == MongoFilterOperators.EndsWith)
                logMessage.LogMessageList = logMessage.LogMessageList?.Where(x => x.ApplicationType.ToLower().EndsWith(applicationTypeTuple.FilterValue.ToLower())).ToList();
            if (applicationTypeTuple.FilterOperator == MongoFilterOperators.Is)
                logMessage.LogMessageList = logMessage.LogMessageList?.Where(x => x.ApplicationType.ToLower() == applicationTypeTuple.FilterValue.ToLower()).ToList();

            return logMessage;
        }

        private LogMessageListModel AddApplicationTypeColumn(LogMessageListModel logMessage)
        {
            foreach (LogMessageModel logMessageItem in logMessage.LogMessageList)
            {
                if (logMessageItem.DomainName != null)
                    logMessageItem.ApplicationType = _domainRepository.Table.FirstOrDefault(x => x.DomainName.Contains(logMessageItem.DomainName))?.ApplicationType?.ToString();
                else
                    logMessageItem.ApplicationType = null;
            }
            return logMessage;
        }

        private LogMessageListModel AddPagingWhenApplicationTypeFilterPresent(FilterCollection filters, FilterTuple applicationTypeTuple, LogMessageListModel logMessage, NameValueCollection sorts, NameValueCollection page, PageListModel pageListModel)
        {
            filters.Add(applicationTypeTuple);
            logMessage = ApplyFilterOnApplicationType(applicationTypeTuple, logMessage);
            long longTotalCount = logMessage.LogMessageList.Count;
            PageListModel pageListModelApplicationType = new PageListModel(filters, sorts, page);
            pageListModelApplicationType.PagingStart = pageListModel.PagingStart;
            pageListModelApplicationType.PagingLength = pageListModel.PagingLength;
            Int32.TryParse(Convert.ToString(longTotalCount), out pageListModelApplicationType.TotalRowCount);
            logMessage.LogMessageList = logMessage.LogMessageList.Skip(pageListModelApplicationType.PagingStart - 1 * pageListModelApplicationType.PagingLength).Take(pageListModelApplicationType.PagingLength).ToList();
            logMessage.BindPageListModel(pageListModelApplicationType);
            return logMessage;
        }


        private bool UpdateAllLoggingLevel(LogMessageConfigurationModel model, string loggingLevelsType)
        {
            bool? loggingLevelsValue = false;
            loggingLevelsValue = (bool?)model.GetType().GetProperty(loggingLevelsType)?.GetValue(model, null);
            if (loggingLevelsValue == null)
                throw new ZnodeException(ErrorCodes.InvalidData, Admin_Resources.LoggingLevelPassNull);
            try
            {
                FilterTuple filter = new FilterTuple(ZnodeGlobalSettingEnum.FeatureName.ToString(), ProcedureFilterOperators.Is, loggingLevelsType);
                FilterCollection filtersList = new FilterCollection();
                filtersList.Add(filter);

                //gets the where clause.              
                EntityWhereClauseModel whereClauseModel = DynamicClauseHelper.GenerateDynamicWhereClauseWithFilter(filtersList.ToFilterDataCollection());

                ZnodeGlobalSetting globalSetting = _globalSettingRepository.GetEntity(whereClauseModel.WhereClause, whereClauseModel.FilterValues);

                if (!Equals(globalSetting?.FeatureValues, loggingLevelsValue.ToString()))
                {
                    globalSetting.FeatureValues = loggingLevelsValue.ToString();
                    return (_globalSettingRepository.Update(globalSetting)) ? true : false;
                }
                return true;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(Admin_Resources.ErrorDataBaseLoggingConfigUpdate, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Error, ex);
                return false;
            }
        }

        #endregion
    }
}
