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
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Api.Controllers
{
    public class ERPTaskSchedulerController : BaseController
    {
        #region Private Variables
        private readonly IERPTaskSchedulerCache _cache;
        private readonly IERPTaskSchedulerService _service;
        #endregion

        #region Constructor
        public ERPTaskSchedulerController(IERPTaskSchedulerService service)
        {
            _service = service;
            _cache = new ERPTaskSchedulerCache(_service);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets list of ERPTaskScheduler.
        /// </summary>
        /// <returns>Returns list of ERPTaskScheduler.</returns>
        [ResponseType(typeof(ERPTaskSchedulerListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage List()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetERPTaskSchedulerList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ERPTaskSchedulerListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ERPTaskSchedulerListResponse data = new ERPTaskSchedulerListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Create ERPTaskScheduler.
        /// </summary>
        /// <param name="erpTaskSchedulerModel">erpTaskSchedulerModel model.</param>
        /// <returns>Returns created model.</returns>
        [ResponseType(typeof(ERPTaskSchedulerResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage Create([FromBody] ERPTaskSchedulerModel erpTaskSchedulerModel)
        {
            HttpResponseMessage response;
            try
            {
                ERPTaskSchedulerModel erpTaskScheduler = _service.Create(erpTaskSchedulerModel);
                if (HelperUtility.IsNotNull(erpTaskScheduler))
                {
                    response = CreateCreatedResponse(new ERPTaskSchedulerResponse { ERPTaskScheduler = erpTaskScheduler });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(erpTaskScheduler.ERPTaskSchedulerId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Warning);
                ERPTaskSchedulerResponse data = new ERPTaskSchedulerResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error);
                ERPTaskSchedulerResponse data = new ERPTaskSchedulerResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.ExceptionalError };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Get erpTaskScheduler by erpTaskScheduler id.
        /// </summary>
        /// <param name="erpTaskSchedulerId">ERPTaskScheduler id to get erpTaskScheduler details.</param>
        /// <returns>Returns erpTaskScheduler model.</returns>
        [ResponseType(typeof(ERPTaskSchedulerResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetERPTaskScheduler(int erpTaskSchedulerId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetERPTaskScheduler(erpTaskSchedulerId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ERPTaskSchedulerResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error);
                ERPTaskSchedulerResponse data = new ERPTaskSchedulerResponse { HasError = true, ErrorMessage = ex.Message};
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Delete erpTaskScheduler.
        /// </summary>
        /// <param name="erpTaskSchedulerId">ERPTaskScheduler Id.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage Delete([FromBody] ParameterModel erpTaskSchedulerId)
        {
            HttpResponseMessage response;
            try
            {
                bool deleted = _service.Delete(erpTaskSchedulerId);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Warning);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.ExceptionalError };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Get the ERPTaskSchedulerId From Touch point name
        /// </summary>
        /// <returns>Return ERP task scheduler id.</returns>
        [ResponseType(typeof(ERPConfiguratorResponse))]
        [HttpPost]
        public virtual HttpResponseMessage GetSchedulerIdByTouchPointName([FromBody] ParameterKeyModel parameterKeyModel)
        {
            HttpResponseMessage response;
            try
            {
                int data = _service.GetSchedulerIdByTouchPointName(parameterKeyModel.ParameterKey, 0, parameterKeyModel.SchedulerCallFor);
                response = CreateOKResponse(new ERPConfiguratorResponse { ERPTaskSchedulerId = data });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ERPConfiguratorResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error);
            }
            return response;
        }
        #endregion

        #region Task scheduler 
        /// <summary>
        /// This method called from the task scheduler service on time.
        /// </summary>
        /// <param name="eRPTaskSchedulerId"></param>
        /// <returns>Return task scheduler service.</returns>
        [ResponseType(typeof(ERPTaskSchedulerResponse))]
        [HttpGet]
        public virtual HttpResponseMessage TriggerSchedulerTask(string eRPTaskSchedulerId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _service.TriggerSchedulerTask(eRPTaskSchedulerId);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ERPTaskSchedulerResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ERPTaskSchedulerResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ERP.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Enable disable ERP task Scheduler from task service.
        /// </summary>
        /// <param name="ERPTaskSchedulerId">ERPTaskSchedulerId</param>
        /// <param name="isActive">isActive</param>
        /// <returns>Return True/False.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage EnableDisableTaskScheduler(int ERPTaskSchedulerId, bool isActive)
        {
            HttpResponseMessage response;
            try
            {
                bool resp = _service.EnableDisableTaskScheduler(ERPTaskSchedulerId, isActive);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = resp });
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
