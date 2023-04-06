using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Api.Cache
{
    class LogMessageCache : BaseCache, ILogMessageCache
    {
        #region Private Variable
        private readonly ILogMessageService _service;
        #endregion

        #region Constructor
        public LogMessageCache(ILogMessageService logMessageService)
        {
            _service = logMessageService;
        }
        #endregion

        #region Public Method
        public string GetLogMessage(string logmessageId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Create Response
                LogMessageModel logMessage = _service.GetLogMessage(logmessageId, Expands);
                if (IsNotNull(logMessage))
                {
                    LogMessageResponse response = new LogMessageResponse { LogMessage = logMessage };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get database log message details
        public string GetDatabaseLogMessage(string logmessageId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Create Response
                LogMessageModel logMessage = _service.GetDatabaseLogMessage(logmessageId, Expands, Filters, Sorts, Page);
                if (IsNotNull(logMessage))
                {
                    LogMessageResponse response = new LogMessageResponse { LogMessage = logMessage };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public string GetLogConfiguration(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Create Response
                LogMessageConfigurationModel logMessageConfigurationModel = _service.GetLogConfiguration();
                if (IsNotNull(logMessageConfigurationModel))
                {
                    LogMessageConfigurationResponse response = new LogMessageConfigurationResponse { LogMessageConfiguration = logMessageConfigurationModel };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public string GetLogMessageList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (IsNull(data))
            {
                //Get published catelog list .
                LogMessageListModel list = _service.GetLogMessageList(Expands, Filters, Sorts, Page);

                if (list?.LogMessageList?.Count > 0)
                {
                    //Create response.
                    LogMessageListResponse response = new LogMessageListResponse { LogMessageList = list.LogMessageList };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        public string GetIntegrationLogMessageList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (IsNull(data))
            {
                LogMessageListModel list = _service.GetIntegrationLogMessageList(Expands, Filters, Sorts, Page);

                if (list?.LogMessageList?.Count > 0)
                {
                    //Create response.
                    LogMessageListResponse response = new LogMessageListResponse { LogMessageList = list.LogMessageList };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get event log messages
        public string GetEventLogMessageList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (IsNull(data))
            {
                LogMessageListModel list = _service.GetEventLogMessageList(Expands, Filters, Sorts, Page);

                if (list?.LogMessageList?.Count > 0)
                {
                    //Create response.
                    LogMessageListResponse response = new LogMessageListResponse { LogMessageList = list.LogMessageList };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get database log message list
        public string GetDatabaseLogMessageList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (IsNull(data))
            {
                LogMessageListModel list = _service.GetDatabaseLogMessageList(Expands, Filters, Sorts, Page);

                if (list?.LogMessageList?.Count > 0)
                {
                    //Create response.
                    LogMessageListResponse response = new LogMessageListResponse { LogMessageList = list.LogMessageList };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion

        #region Impersonation
        //Get impersonation activity log
        public string GetImpersonationLogList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (IsNull(data))
            {
                ImpersonationActivityListModel list = _service.GetImpersonationLogList(Expands, Filters, Sorts, Page);

                if (list?.LogActivityList?.Count > 0)
                {
                    //Create response.
                    ImpersonationActivityListResponse response = new ImpersonationActivityListResponse { LogActivityList = list.LogActivityList };

                    //apply pagination parameters.
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }
        #endregion
    }
}
