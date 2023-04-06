using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Helper;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Api.Controllers
{
    public class LogMessageController : BaseController
    {

        #region Private Variables
        private readonly ILogMessageCache _cache;
        private readonly ILogMessageService _service;
        #endregion

        #region Constructor
        public LogMessageController(ILogMessageService service)
        {
            _service = service;
            _cache = new LogMessageCache(_service);
        }
        #endregion

        #region Public Method
        /// <summary>
        /// Gets list of Publish Catalogs.
        /// </summary>
        /// <returns>Return publish catalog list.</returns>
        [ResponseType(typeof(LogMessageListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage List()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetLogMessageList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<LogMessageListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new LogMessageListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Gets list of Integration Logs.
        /// </summary>
        /// <returns>Return integration log messages list.</returns>
        [ResponseType(typeof(LogMessageListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage IntegrationLogList()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetIntegrationLogMessageList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<LogMessageListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new LogMessageListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Gets list of Event Logs.
        /// </summary>
        /// <returns>Return event log messages list.</returns>
        [ResponseType(typeof(LogMessageListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage EventLogList()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetEventLogMessageList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<LogMessageListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new LogMessageListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Gets list of database Logs.
        /// </summary>
        /// <returns>Return database log messages list.</returns>
        [ResponseType(typeof(LogMessageListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage DatabaseLogList()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetDatabaseLogMessageList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<LogMessageListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new LogMessageListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Get  log message list by  log message id.
        /// </summary>
        /// <param name="logMessageId"> Log message id to get  Log message details.</param>
        /// <returns>Return log message.</returns>
        [ResponseType(typeof(LogMessageResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetLogMessage(string logMessageId)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetLogMessage(logMessageId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<LogMessageResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new LogMessageResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new LogMessageResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Get  log message by  log message id from SQL.
        /// </summary>
        /// <param name="logMessageId"> Log message id to get  Log message details.</param>
        /// <returns>Return log message.</returns>
        [ResponseType(typeof(LogMessageResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetDatabaseLogMessage(string logMessageId)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetDatabaseLogMessage(logMessageId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<LogMessageResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new LogMessageResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Get log configuration.
        /// </summary>
        /// <returns>Return log message configuration.</returns>
        [ResponseType(typeof(LogMessageConfigurationResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetLogConfiguration()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetLogConfiguration(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<LogMessageConfigurationResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new LogMessageConfigurationResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        ///Update Log configuration
        /// </summary>
        /// <param name="logMessageConfigurationModel">model</param>
        /// <returns>Returns created model.</returns>
        [ResponseType(typeof(LogMessageConfigurationResponse))]
        [HttpPost]
        public virtual HttpResponseMessage UpdateLogConfiguration([FromBody] LogMessageConfigurationModel logMessageConfigurationModel)
        {
            HttpResponseMessage response;
            try
            {
                   response   = _service.UpdateLogConfiguration(logMessageConfigurationModel) ? CreateCreatedResponse(new LogMessageConfigurationResponse { LogMessageConfiguration = logMessageConfigurationModel, ErrorCode = 0 }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new LogMessageConfigurationResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new LogMessageConfigurationResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Purge Logs.
        /// </summary>
        /// <param name="logCategoryIds">Log category Id.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage PurgeLogs([FromBody] ParameterModel logCategoryIds)
        {
            HttpResponseMessage response;

            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.PurgeLogs(logCategoryIds) });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Error);
            }

            return response;
        }
        #endregion

        #region Impersonation
        /// <summary>
        /// Gets list of Event Logs.
        /// </summary>
        /// <returns>Return event log messages list.</returns>
        [ResponseType(typeof(ImpersonationActivityListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage ImpersonationLogList()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetImpersonationLogList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ImpersonationActivityListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ImpersonationActivityListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
            }

            return response;
        }
        #endregion
    }
}
