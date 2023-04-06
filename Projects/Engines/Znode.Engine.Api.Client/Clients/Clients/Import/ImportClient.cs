using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Linq;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public class ImportClient : BaseClient, IImportClient
    {
        //Get all templates with import head
        public virtual ImportModel GetAllTemplates(int importHeadId, int familyId, int promotionTypeId = 0)
        {
            string endpoint = ImportEndpoint.GetAllTemplates(importHeadId, familyId, promotionTypeId);
            endpoint += BuildEndpointQueryString(null, null, null, null, null);

            ApiStatus status = new ApiStatus();
            ImportResponse response = GetResourceFromEndpoint<ImportResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.Import;
        }

        //Get all import types
        public virtual ImportModel GetImportTypeList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = ImportEndpoint.GetImportTypeList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            ImportResponse response = GetResourceFromEndpoint<ImportResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.Import;
        }

        //Get template data with respect template id
        public virtual ImportModel GetTemplateData(int templateId, int importHeadId, int familyId, int promotionTypeId = 0)
        {
            string endpoint = ImportEndpoint.GetTemplateData(templateId, importHeadId, familyId, promotionTypeId);
            endpoint += BuildEndpointQueryString(null, null, null, null, null);

            ApiStatus status = new ApiStatus();
            ImportResponse response = GetResourceFromEndpoint<ImportResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.Import;
        }

        //post and process import data
        public virtual bool ImportData(ImportModel model)
        {
            string endpoint = ImportEndpoint.ImportData();
            endpoint += BuildEndpointQueryString(null, null, null, null, null);

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response.IsSuccess;
        }

        //Download the template
        public virtual DownloadModel DownloadTemplate(int importHeadId, int downloadImportFamilyId, int downloadImportPromotionTypeId = 0)
        {
            string endpoint = ImportEndpoint.DownloadTemplate(importHeadId, downloadImportFamilyId, downloadImportPromotionTypeId);
            endpoint += BuildEndpointQueryString(null, null, null, null, null);

            ApiStatus status = new ApiStatus();
            DownloadResponse response = PostResourceToEndpoint<DownloadResponse>(endpoint, JsonConvert.SerializeObject(importHeadId), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.downloadModel;
        }

        // Gets the Import Logs to check import status
        public virtual ImportLogsListModel GetImportLogs(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int pageIndex, int pageSize)
        {
            string endpoint = ImportEndpoint.GetImportLogs();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            ImportLogsListResponse response = GetResourceFromEndpoint<ImportLogsListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            ImportLogsListModel list = new ImportLogsListModel { ImportLogs = response?.LogsList?.ToList() };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        // Gets the Import Logs details on the basis of importLogId
        public virtual ImportLogDetailsListModel GetImportLogDetails(int importProcessLogId, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int pageIndex, int pageSize)
        {
            string endpoint = ImportEndpoint.GetImportLogDetails(importProcessLogId);
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            ImportLogDetailsListResponse response = GetResourceFromEndpoint<ImportLogDetailsListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            ImportLogDetailsListModel list = new ImportLogDetailsListModel { ImportLogDetails = response?.LogDetailsList?.ToList(), ImportLogs = response?.ImportLogs };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Get the import log current status.
        public virtual ImportLogsListModel GetImportLogStatus(int importProcessLogId, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int pageIndex, int pageSize)
        {
            string endpoint = ImportEndpoint.GetImportLogStatus(importProcessLogId);

            ApiStatus status = new ApiStatus();
            ImportLogsListResponse response = GetResourceFromEndpoint<ImportLogsListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            ImportLogsListModel list = new ImportLogsListModel { ImportLogs = response?.LogsList?.ToList() };

            return list;
        }

        // Delete the logs from ZnodeImportLog and ZnodeImportProcessLog table
        public virtual bool DeleteLogs(ParameterModel importProcessLogId)
        {
            string endpoint = ImportEndpoint.DeleteLog();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(importProcessLogId), status);

            //check the status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        //Get all families for product import
        public virtual ImportProductFamilyListModel GetAllFamilies(bool isCategory)
        {
            string endpoint = ImportEndpoint.GetAllFamilies(isCategory);

            ApiStatus status = new ApiStatus();
            ImportResponse response = GetResourceFromEndpoint<ImportResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            ImportProductFamilyListModel list = new ImportProductFamilyListModel { FamilyList = response?.Import?.FamilyList?.FamilyList?.ToList() };

            return list;
        }

        //Update Mappings
        public virtual bool UpdateMappings(ImportModel model, FilterCollection filters)
        {
            string endpoint = ImportEndpoint.UpdateMappings();
            endpoint += BuildEndpointQueryString(null, filters, null, null, null);

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response.booleanModel.IsSuccess;
        }

        //check import status
        public virtual bool CheckImportProcess()
        {
            string endpoint = ImportEndpoint.CheckImportProcess();
            endpoint += BuildEndpointQueryString(null, null, null, null, null);

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = GetResourceFromEndpoint<TrueFalseResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response.booleanModel.IsSuccess;
        }

        // Get default template for data import.
        public virtual ImportModel GetDefaultTemplate(string templateName)
        {
            string endpoint = ImportEndpoint.GetDefaultTemplate(templateName);

            ApiStatus status = new ApiStatus();
            ImportResponse response = GetResourceFromEndpoint<ImportResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.Import;
        }

        //Get custom import template list. It will not return the system defined import template.
        public virtual ImportManageTemplateListModel GetCustomImportTemplateList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int pageIndex, int pageSize)
        {
            string endpoint = ImportEndpoint.GetCustomImportTemplateList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            ImportTemplateListResponse response = GetResourceFromEndpoint<ImportTemplateListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            ImportManageTemplateListModel list = new ImportManageTemplateListModel { ImportManageTemplates = response?.ImportTemplateList?.ToList() };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        // Deletes the custom import templates. It will not delete the system defined import templates.
        public virtual bool DeleteImportTemplate(ParameterModel importTemplateId)
        {
            string endpoint = ImportEndpoint.DeleteImportTemplate();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(importTemplateId), status);

            //check the status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        // get import details for csv,excel and pdf
        public virtual ExportModel GetImportLogDetailsList(string fileType, int ImportProcessLogId, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int pageIndex, int pageSize)
        {
            string endpoint = ImportEndpoint.GetImportLogDetailsList(fileType, ImportProcessLogId);
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            ExportResponse response = GetResourceFromEndpoint<ExportResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return new ExportModel { Message = response.ExportMessageModel.Message, HasError = response.ExportMessageModel.HasError };
        }

        //post and process import data
        public virtual bool CheckExportProcess()
        {
            string endpoint = ImportEndpoint.CheckExportProcess();
            endpoint += BuildEndpointQueryString(null, null, null, null, null);

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = GetResourceFromEndpoint<TrueFalseResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response.IsSuccess;
        }
    }
   }
