using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Api.Controllers
{
    public class TouchPointConfigurationController : BaseController
    {
        #region Private Variables

        private readonly ITouchPointConfigurationCache _cache;
        private readonly ITouchPointConfigurationService _service;
        #endregion

        #region Constructor
        public TouchPointConfigurationController(ITouchPointConfigurationService service)
        {
            _service = service;
            _cache = new TouchPointConfigurationCache(_service);

        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets list of TouchPointConfiguration.
        /// </summary>
        /// <returns>Returns list of TouchPointConfiguration.</returns>
        [ResponseType(typeof(TouchPointConfigurationListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage List()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetTouchPointConfigurationList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<TouchPointConfigurationListResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TouchPointConfigurationListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TouchPointConfigurationListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// trigger touch point task scheduler
        /// </summary>
        /// <param name="connectorTouchPoints"></param>
        /// <returns>Return trigger touch point task scheduler if true else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpGet]
        public virtual HttpResponseMessage TriggerTaskScheduler(string connectorTouchPoints)
        {
            HttpResponseMessage response;
            try
            {
                bool status = _service.TriggerTaskScheduler(connectorTouchPoints);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = status });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                if (ex.InnerException != null)
                {
                    ZnodeException obj = (ZnodeException)ex.InnerException;
                    if (obj != null)
                    {
                        if (obj.ErrorCode == ErrorCodes.FileNotFound)
                        {
                            response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = Znode.Libraries.Resources.ERP_Resources.ErrorFileNotAvailable, ErrorCode = ErrorCodes.FileNotFound });
                            ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error);
                            return response;
                        }
                    }
                }
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.NotFound });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Gets list of Scheduler Log List.
        /// </summary>
        /// <returns>Returns list of Scheduler Log.</returns>
        [ResponseType(typeof(TouchPointConfigurationListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetSchedulerLogList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.SchedulerLogList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<TouchPointConfigurationListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TouchPointConfigurationListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Send Email to Site Admin
        /// </summary>
        /// <param name="erpSchedulerLogActivityModel">erpSchedulerLogActivityModel.</param>
        /// <returns>Returns true if send email successfully else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage SendSchedulerActivityLog([FromBody] ERPSchedulerLogActivityModel erpSchedulerLogActivityModel)
        {
            HttpResponseMessage response;
            try
            {
                bool emailSendStatus = _service.SendSchedulerActivityLog(erpSchedulerLogActivityModel);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = emailSendStatus });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message});
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error);
            }
            return response;
        }
        #endregion
    }
}
