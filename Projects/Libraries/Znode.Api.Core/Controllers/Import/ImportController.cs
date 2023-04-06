using System;
using System.Net.Http;
using System.Web.Http;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using Znode.Libraries.Framework.Business;
using System.Web.Http.Description;
using System.Diagnostics;

namespace Znode.Engine.Api.Controllers
{
    public class ImportController : BaseController
    {
        #region Private Variables
        private readonly IImportService _service;
        private readonly IImportCache _cache;
        #endregion

        #region Constructor
        public ImportController(IImportService service)
        {
            _service = service;
            _cache = new ImportCache(_service);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets the import type list
        /// </summary>
        /// <returns>Returns import type list.</returns>
        [ResponseType(typeof(ImportResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetImportTypeList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetImportTypeList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ImportResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ImportResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Import.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Gets the template list with respect to Import header id
        /// </summary>
        /// <param name="importHeadId">Import Header Id</param>
        /// <param name="familyId">familyId</param>
        /// <param name="promotionTypeId">Promotion Type Id</param>
        /// <returns>Returns template list.</returns>
        [ResponseType(typeof(ImportResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetAllTemplates(int importHeadId, int familyId, int promotionTypeId = 0)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetAllTemplates(importHeadId, familyId, RouteUri, RouteTemplate, promotionTypeId);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ImportResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ImportResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Import.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Gets the template data with respect to Template Id
        /// </summary>
        /// <param name="templateId">Template Id</param>
        /// <param name="importHeadId">Import head id</param>
        /// <param name="familyId">familyId</param>
        /// <param name="promotionTypeId">Promotion Type Id</param>
        /// <returns>Returns template data.</returns>
        [ResponseType(typeof(ImportResponse))]
        public virtual HttpResponseMessage GetTemplateData(int templateId, int importHeadId, int familyId, int promotionTypeId = 0)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetTemplateData(templateId, importHeadId, familyId, RouteUri, RouteTemplate, promotionTypeId);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ImportResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ImportResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Import.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// This method will fetch and process the data from uploaded file.
        /// </summary>
        /// <param name="model">Improt Model</param>
        /// <returns>Returns true if successfull else returns false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage ImportData(ImportModel model)
        {
            HttpResponseMessage response;
            response = CreateNoContentResponse();
            try
            {
                int processStarted = _service.ProcessData(model);
                response = processStarted > 0 ? CreateOKResponse(new TrueFalseResponse { IsSuccess = true }) : CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = "Import process failed.", ErrorCode = ErrorCodes.ProcessingFailed });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Import.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.ImportError });
            }
            return response;
        }

        /// <summary>
        /// Download the template
        /// </summary>
        /// <param name="importHeadId"></param>
        /// <param name="familyId">familyId</param>
        /// <param name="promotionTypeId">Promotion Type Id</param>
        /// <returns>Returns download response.</returns>
        [ResponseType(typeof(DownloadResponse))]
        [HttpPost]
        public virtual HttpResponseMessage DownLoadTemplate(int importHeadId, int familyId, int promotionTypeId = 0)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.DownLoadTemplate(importHeadId, familyId,RouteUri, RouteTemplate, promotionTypeId);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<DownloadResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new DownloadResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Import.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Get the Import logs
        /// </summary>
        /// <param name="importProcessLogId">Import Log Id</param>
        /// <returns>Gets the Import logs.</returns>
        [ResponseType(typeof(ImportLogDetailsListResponse))]
        public virtual HttpResponseMessage GetImportLogDetails(int importProcessLogId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetImportLogDetails(importProcessLogId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ImportLogDetailsListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ImportLogDetailsListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Import.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Get templates list with Status
        /// </summary>
        /// <returns>Returns templates list with status.</returns>
        [ResponseType(typeof(ImportLogsListResponse))]
        public virtual HttpResponseMessage GetImportLogs()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetImportLogs(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ImportLogsListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ImportLogsListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Import.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get the Status of the Logs by Import Process Log Id
        /// </summary>
        /// <param name="importProcessLogId">Import Process Log Id</param>
        /// <returns>Returns the status of the logs</returns>
        [ResponseType(typeof(ImportLogsListResponse))]
        public virtual HttpResponseMessage GetLogStatus(int importProcessLogId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetLogStatus(importProcessLogId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ImportLogsListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ImportLogsListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Import.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Delete the Import log data
        /// </summary>
        /// <param name="importProcessLogIds">importProcessLogId</param>
        /// <returns>HttpResponseMessage</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage Delete([FromBody] ParameterModel importProcessLogIds)
        {
            HttpResponseMessage response;
            try
            {
                bool deleted = _service.DeleteLogDetails(importProcessLogIds);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Import.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Get all families for product import
        /// </summary>
        /// <param name="isCategory">isCategory</param>
        /// <returns> Returns all families for product import.</returns>
        [ResponseType(typeof(ImportResponse))]
        public virtual HttpResponseMessage GetFamilies(bool isCategory)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetFamilies(isCategory, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ImportResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ImportResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Import.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// This method will fetch and process the data from uploaded file.
        /// </summary>
        /// <param name="model">Improt Model</param>
        /// <returns>Returns and process the data from uploaded file. </returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage UpdateMappings(ImportModel model)
        {
            HttpResponseMessage response = CreateNoContentResponse();
            try
            {
                int recordsUpdated = _service.UpdateTemplateMappings(model);

                BooleanModel boolModel = new BooleanModel();
                boolModel.IsSuccess = recordsUpdated > 0;

                TrueFalseResponse trueFalseResponse = new TrueFalseResponse();
                trueFalseResponse.booleanModel = boolModel;

                response = CreateOKResponse(trueFalseResponse);
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Import.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Import.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// check if import process is going on
        /// </summary>
        /// <returns>Returns true if successfull else returns false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpGet]
        public virtual HttpResponseMessage CheckImportStatus()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.CheckImportStatus(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<TrueFalseResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ImportLogsListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Import.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Get default template for import data.
        /// </summary>
        /// <param name="templateName"></param>
        /// <returns>Returns default template for import data.</returns>
        [ResponseType(typeof(ImportResponse))]
        public virtual HttpResponseMessage GetDefaultTemplate(string templateName)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetDefaultTemplate(templateName, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ImportResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new ImportResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode=ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Import.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ImportResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Import.ToString(), TraceLevel.Error);
            }

            return response;
        }

        ///// <summary>
        ///// Get custom import template list. It will not return the system defined import template.
        ///// </summary>
        ///// <returns>Returns templates.</returns>
        [ResponseType(typeof(ImportTemplateListResponse))]
        public virtual HttpResponseMessage GetCustomImportTemplateList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetCustomImportTemplateList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ImportTemplateListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ImportTemplateListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Import.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Deletes the custom import templates. It will not delete the system defined import templates.
        /// </summary>
        /// <param name="importTemplateIds">importTemplateId</param>
        /// <returns>HttpResponseMessage</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage DeleteImportTemplate([FromBody] ParameterModel importTemplateIds)
        {
            HttpResponseMessage response;
            try
            {
                bool deleted = _service.DeleteImportTemplate(importTemplateIds);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Import.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        [ResponseType(typeof(ExportResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetImportLogDetailsList(string fileType, int importProcessLogId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetErrorListforImport(fileType, importProcessLogId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ExportResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ImportLogDetailsListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Import.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Gets the export process log
        /// </summary>
        /// <returns>Returns import type list.</returns>
        [ResponseType(typeof(ImportResponse))]
        [HttpGet]
        public virtual HttpResponseMessage CheckExportProcess()
        {
            HttpResponseMessage response;
            try
            {
                bool data = _service.IsExportPublishInProgress();
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = data });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ImportResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Import.ToString(), TraceLevel.Error);
            }

            return response;
        }
        #endregion
    }
}