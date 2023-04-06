using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Znode.Engine.Admin.Extensions;
using Znode.Engine.Admin.Helpers;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.Agents
{
    public class LogMessageAgent : BaseAgent, ILogMessageAgent
    {
        #region Private Variables
        private readonly ILogMessageClient _logmessageClient;
        private static readonly IDefaultGlobalConfigClient _defaultGlobalConfigClient = new DefaultGlobalConfigClient();
        #endregion

        #region Constructor
        public LogMessageAgent(ILogMessageClient logmessageClient)
        {
            _logmessageClient = GetClient<ILogMessageClient>(logmessageClient);
        }
        #endregion

        #region Public Method
        public LogMessageViewModel GetLogMessage(string logMessageId)
        {
            LogMessageModel logMessageModel = _logmessageClient.GetLogMessage(logMessageId);

            if (logMessageModel != null)
            {
                LogMessageViewModel logMessageViewModel = logMessageModel.ToViewModel<LogMessageViewModel>();
                logMessageViewModel.CreatedDate = logMessageModel.CreatedDate.ToString("dd/MM/yyyy HH:mm:ss.fff tt");
                return logMessageViewModel;
            }
            return new LogMessageViewModel();
        }

        public LogMessageViewModel GetDatabaseLogMessage(string logMessageId)
        {
            LogMessageModel logMessageModel = _logmessageClient.GetDatabaseLogMessage(logMessageId);
            LogMessageViewModel logMessageViewModel = logMessageModel.ToViewModel<LogMessageViewModel>();
            logMessageViewModel.CreatedDate = logMessageModel.CreatedDate.ToString("dd/MM/yyyy HH:mm:ss.fff tt");
            return logMessageViewModel;
        }


        public LogConfigurationViewModel GetLogConfiguration()
            => _logmessageClient.GetLogConfiguration().ToViewModel<LogConfigurationViewModel>();

        //Update Log Configuration.
        public virtual LogConfigurationViewModel UpdateLogConfiguration(LogConfigurationViewModel logConfigurationViewModel)
        {
            try
            {
                logConfigurationViewModel = _logmessageClient.UpdateLogConfiguration(logConfigurationViewModel?.ToModel<LogMessageConfigurationModel>())?.ToViewModel<LogConfigurationViewModel>();
                if (logConfigurationViewModel != null)
                {
                    HttpContext.Current.Cache.Remove(CachedKeys.globalErrorLoggingSetting);
                    HttpContext.Current.Cache.Remove(CachedKeys.DefaultLoggingConfigCache);
                    return logConfigurationViewModel;
                }
                else
                {
                    throw new Exception("Update Logging Configuration Failed");
                }
            }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, string.Empty, TraceLevel.Error);
                return (LogConfigurationViewModel)GetViewModelWithErrorMessage(logConfigurationViewModel, Admin_Resources.ErrorFailedToCreate);
            }
        }
        //Set Default Logging Setting in cache
        public virtual void SetGlobalLoggingSetting()
        {
            try
            {
                if (HttpRuntime.Cache[CachedKeys.DefaultLoggingConfigCache] == null)
                {
                    Dictionary<string, string> loggingConfigurationSettings = ZnodeDependencyResolver.GetService<IDefaultGlobalConfigClient>()
                                        ?.GetLoggingGlobalConfigList();
                HttpRuntime.Cache.Insert(CachedKeys.DefaultLoggingConfigCache, loggingConfigurationSettings);
            }
            }
            catch (Exception) { }
        }

        public LogMessageListViewModel GetLogMessageList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            ZnodeLogging.LogMessage("Input parameters:", ZnodeLogging.Components.OMS.ToString(), TraceLevel.Verbose, new { Filters = filters, Sorts = sorts });

            if ((HelperUtility.IsNull(sorts) || sorts.Count == 0))
            {
                sorts = new SortCollection();
                sorts.Add(SortKeys.CreatedDate, DynamicGridConstants.DESCKey);
            }

            FilterCollection _filters = new FilterCollection();
            _filters.AddRange(filters);

            DateRangePickerHelper.FormatFilterForDateTimeRange(_filters, DateTimeRange.Last_Hour.ToString(), DateTimeRange.All_Logs.ToString());
            LogMessageListModel logmessageList = _logmessageClient.GetLogMessageList(expands, _filters, sorts, pageIndex, pageSize);
            LogMessageListViewModel listViewModel = new LogMessageListViewModel { LogMessageList = logmessageList?.LogMessageList?.ToViewModel<LogMessageViewModel>().ToList() };
            SetListPagingData(listViewModel, logmessageList);
            return logmessageList?.LogMessageList?.Count > 0 ? listViewModel : new LogMessageListViewModel() { LogMessageList = new List<LogMessageViewModel>() };
        }

        public LogMessageListViewModel GetIntegrationLogMessageList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            if ((HelperUtility.IsNull(sorts) || sorts.Count == 0))
            {
                sorts = new SortCollection();
                sorts.Add(SortKeys.CreatedDate, DynamicGridConstants.DESCKey);
            }
            FilterCollection _filters = new FilterCollection();
            _filters.AddRange(filters);

            DateRangePickerHelper.FormatFilterForDateTimeRange(_filters, DateTimeRange.Last_Hour.ToString(), DateTimeRange.All_Logs.ToString());
            LogMessageListModel logmessageList = _logmessageClient.GetIntegrationLogMessageList(expands, _filters, sorts, pageIndex, pageSize);
            LogMessageListViewModel listViewModel = new LogMessageListViewModel { LogMessageList = logmessageList?.LogMessageList?.ToViewModel<LogMessageViewModel>().ToList() };
            SetListPagingData(listViewModel, logmessageList);
            return logmessageList?.LogMessageList?.Count > 0 ? listViewModel : new LogMessageListViewModel() { LogMessageList = new List<LogMessageViewModel>() };
        }

        //Get event log messages
        public LogMessageListViewModel GetEventLogMessageList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            if ((HelperUtility.IsNull(sorts) || sorts.Count == 0))
            {
                sorts = new SortCollection();
                sorts.Add(SortKeys.CreatedDate, DynamicGridConstants.DESCKey);
            }

            FilterCollection _filters = new FilterCollection();
            _filters.AddRange(filters);

            DateRangePickerHelper.FormatFilterForDateTimeRange(_filters, DateTimeRange.Last_Hour.ToString(), DateTimeRange.All_Logs.ToString());
            LogMessageListModel logmessageList = _logmessageClient.GetEventLogMessageList(expands, _filters, sorts, pageIndex, pageSize);
            LogMessageListViewModel listViewModel = new LogMessageListViewModel { LogMessageList = logmessageList?.LogMessageList?.ToViewModel<LogMessageViewModel>().ToList() };
            SetListPagingData(listViewModel, logmessageList);
            return logmessageList?.LogMessageList?.Count > 0 ? listViewModel : new LogMessageListViewModel() { LogMessageList = new List<LogMessageViewModel>() };
        }

        //Get database log messages
        public LogMessageListViewModel GetDatabaseLogMessageList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            if ((HelperUtility.IsNull(sorts) || sorts.Count == 0))
            {
                sorts = new SortCollection();
                sorts.Add(SortKeys.CreatedDate, DynamicGridConstants.DESCKey);
            }
            FilterCollection _filters = new FilterCollection();
            _filters.AddRange(filters);

            DateRangePickerHelper.FormatFilterForDateTimeRange(_filters, DateTimeRange.Last_Hour.ToString(), DateTimeRange.All_Logs.ToString());
            LogMessageListModel logmessageList = _logmessageClient.GetDatabaseLogMessageList(expands, _filters, sorts, pageIndex, pageSize);
            LogMessageListViewModel listViewModel = new LogMessageListViewModel { LogMessageList = logmessageList?.LogMessageList?.ToViewModel<LogMessageViewModel>().ToList() };
            SetListPagingData(listViewModel, logmessageList);
            return logmessageList?.LogMessageList?.Count > 0 ? listViewModel : new LogMessageListViewModel() { LogMessageList = new List<LogMessageViewModel>() };
        }
        /// <summary>
        /// to purge logs from mongoDB
        /// </summary>
        /// <param name="logCategoryIds"></param>
        /// <returns></returns>
        /// This method is obsolete instead of this method please use overloaded method.
        [Obsolete]
        public virtual bool PurgeLogs(string logCategoryIds)
        {
            try
            {
                return _logmessageClient.PurgeLogs(new ParameterModel { Ids = logCategoryIds });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return false;
            }
        }

        /// <summary>
        /// Method use to purge logs.
        /// </summary>
        /// <param name="logCategoryIds"></param>
        /// <param name="message">If logs successfully deleted then this method will return message parameter with required message else message parameter will be empty</param>
        /// <returns></returns>
        public virtual bool PurgeLogs(string logCategoryIds, out string message)
        {
            message = string.Empty;
            try
            {
                bool status = _logmessageClient.PurgeLogs(new ParameterModel { Ids = logCategoryIds });
                message = status ? Admin_Resources.DeleteMultipleLogsMessage : Admin_Resources.DeleteMessageIfNoRecordFound;
                return status;
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                return false;
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                return false;
            }
        }

        #region Impersonation
        public ImpersonationLogListViewModel GetImpersonationLogList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            if ((HelperUtility.IsNull(sorts) || sorts.Count == 0))
            {
                sorts = new SortCollection();
                sorts.Add(SortKeys.CreatedDate, DynamicGridConstants.DESCKey);
            }
            FilterCollection _filters = new FilterCollection();
            _filters.AddRange(filters);

            //FormatFilterForDateTimeRange(_filters);
            ImpersonationActivityListModel impersonationLogList = _logmessageClient.GetImpersonationLogList(expands, _filters, sorts, pageIndex, pageSize);
            ImpersonationLogListViewModel listViewModel = new ImpersonationLogListViewModel { ImpersonationActivityList = impersonationLogList?.LogActivityList?.ToViewModel<ImpersonationLogViewModel>().ToList() };
            SetListPagingData(listViewModel, impersonationLogList);

            SetListPagingData(listViewModel, impersonationLogList);
            return listViewModel;
        }


        #endregion
        #endregion     
    }
}
