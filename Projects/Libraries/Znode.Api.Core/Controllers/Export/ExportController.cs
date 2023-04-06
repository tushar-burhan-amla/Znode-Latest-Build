using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http.Description;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Controllers;
using Znode.Engine.Api.Models.Responses;
using Znode.Libraries.Framework.Business;
using Znode.Engine.Services;
using System.Web.Http;
using Znode.Engine.Api.Models;

namespace Znode.Api.Core.Controllers
{
    public   class ExportController : BaseController
    {
        private readonly IExportCache _cache;
        private IExportService _service;
        #region Constructor

        public ExportController(IExportService service)
        {
            _service = service;
            _cache = new ExportCache(_service);
        }
        /// <summary>
        /// Get templates list with Status
        /// </summary>
        /// <returns>Returns templates list with status.</returns>
        [ResponseType(typeof(ExportLogsListResponse))]
        public virtual HttpResponseMessage GetExportLogs()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetExportLogs(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ExportLogsListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ImportLogsListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Import.ToString(), TraceLevel.Error);
            }
            return response;
        }
        /// <summary>
        /// Delete the export log data
        /// </summary>
        /// <param name="exportProcessLogIds">exportProcessLogId</param>
        /// <returns>Returns Response for Export  delete Log</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage Delete([FromBody] ParameterModel exportProcessLogIds)
        {
            HttpResponseMessage response;
            try
            {
                bool deleted = _service.DeleteExportFiles(exportProcessLogIds);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted });
            }
            catch (Exception ex)
            {
                 ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Import.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }



        //Delete Expired Export Files and get response.
        [HttpDelete]
        [ResponseType(typeof(TrueFalseResponse))]
        public virtual HttpResponseMessage DeleteExpiredExportFiles()
        {
            HttpResponseMessage response;
            try
            {
                bool status = _service.DeleteExpiredExportFiles();
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = status });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        //Get Export File Path.
        public virtual HttpResponseMessage GetExportFilePath(string tableName)
        {
            HttpResponseMessage response;
            try
            {
                string filePath = _service.GetExportFilePath(tableName);
                response = CreateOKResponse(new StringResponse { Response = filePath });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ImportLogsListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Import.ToString(), TraceLevel.Error);
            }
            return response;
        }



    }
}
#endregion