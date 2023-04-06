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
    public class MyReportsController : BaseController
    {
        #region Private Variables
        private readonly IReportService _reportService;
        private readonly IReportCache _cache;
        #endregion

        #region Default Constructor
        public MyReportsController(IReportService reportService)
        {
            _reportService = reportService;
            _cache = new ReportCache(_reportService);
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Get the list of all Reports.
        /// </summary>
        /// <returns>HttpResponseMessage</returns>
        [ResponseType(typeof(ReportListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage List()
        {
            HttpResponseMessage response;
            try
            {
                //Get the list of reports from Cache.
                string data = _cache.GetReportList(RouteUri, RouteTemplate);
                response = string.IsNullOrEmpty(data) ? CreateNoContentResponse() : CreateOKResponse<ReportListResponse>(data);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ReportListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Reports.ToString(),TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get the data required for Dynamic Reports
        /// </summary>
        /// <param name="dynamicReportType">dataType</param>
        /// <returns>HttpResponseMessage</returns>
        [ResponseType(typeof(DynamicReportResponse))]
        public virtual HttpResponseMessage GetExportData(string dynamicReportType)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetExportData(RouteUri, RouteTemplate, dynamicReportType);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<DynamicReportResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new DynamicReportResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Reports.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new DynamicReportResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Reports.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Generate dynamic report.
        /// </summary>
        /// <param name="model">Dynamic report model</param>
        /// <returns>Retrun status as true/false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage GenerateDynamicReport(DynamicReportModel model)
        {
            HttpResponseMessage response = CreateNoContentResponse();
            try
            {
                int errorCode = 0;
                bool isCreated = _reportService.GenerateDynamicReport(model, out errorCode);
                response = isCreated && errorCode.Equals(0) ?
                    CreateOKResponse(new TrueFalseResponse { IsSuccess = isCreated }) :
                    CreateInternalServerErrorResponse(new TrueFalseResponse { IsSuccess = false, HasError = true, ErrorCode = errorCode });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Reports.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Reports.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Delete dynamic report.
        /// </summary>
        /// <param name="customReportIds">CustomReport Ids</param>
        /// <returns>Retrun status as true/false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage DeleteDynamicReport([FromBody] ParameterModel customReportIds)
        {
            HttpResponseMessage response;
            try
            {
                bool deleted = _reportService.DeleteCustomReport(customReportIds);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ProviderEngine.ToString(),TraceLevel.Warning);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.ProviderEngine.ToString(), TraceLevel.Error);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Get custom Report
        /// </summary>
        /// <param name="customReportId">CustomReport Id</param>
        /// <returns>HttpResponseMessage</returns>
        [ResponseType(typeof(DynamicReportResponse))]
        public virtual HttpResponseMessage GetCustomReport(int customReportId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetCustomReport(RouteUri, RouteTemplate, customReportId);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<DynamicReportResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new DynamicReportResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Reports.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new DynamicReportResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Reports.ToString(), TraceLevel.Error);
            }

            return response;
        }

        #endregion
    }
}
