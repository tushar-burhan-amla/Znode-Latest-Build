using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Services;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;

namespace Znode.Engine.Api.Cache
{
    public class ImportCache : BaseCache, IImportCache
    {
        #region Private Variable
        private readonly IImportService _service;
        #endregion

        #region Constructor
        public ImportCache(IImportService importService)
        {
            _service = importService;
        }
        #endregion

        #region Public Methods
        //get all templates with respect to Import Type Id
        public virtual string GetAllTemplates(int importTypeId, int familyId, string routeUri, string routeTemplate, int promotionTypeId = 0)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (IsNull(data))
            {
                //Get Inventory list.
                ImportModel importData = _service.GetAllTemplates(importTypeId, familyId, promotionTypeId);

                //Create response.
                ImportResponse response = new ImportResponse { Import = importData };

                //Insert data in cache
                data = InsertIntoCache(routeUri, routeTemplate, response);
            }
            return data;
        }

        //gets te Import type list
        public virtual string GetImportTypeList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (IsNull(data))
            {
                //Get Inventory list.
                ImportModel importData = _service.GetImportTypeList(Expands, Filters, Sorts, Page);

                //Create response.
                ImportResponse response = new ImportResponse { Import = importData };

                //Insert data in cache
                data = InsertIntoCache(routeUri, routeTemplate, response);
            }
            return data;
        }

        //gets the list of template data with respect to template id
        public virtual string GetTemplateData(int templateId, int importHeadId, int familyId, string routeUri, string routeTemplate, int promotionTypeId = 0)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (IsNull(data))
            {
                //Get Inventory list.
                ImportModel importData = _service.GetTemplateData(templateId, importHeadId, familyId, promotionTypeId);

                //Create response.
                ImportResponse response = new ImportResponse { Import = importData };

                //Insert data in cache
                data = InsertIntoCache(routeUri, routeTemplate, response);
            }
            return data;
        }

        // Downloads the columns for import depending upon ImportHead Id
        public virtual string DownLoadTemplate(int importHeadId, int familyId, string routeUri, string routeTemplate, int promotionTypeId = 0)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (IsNull(data))
            {
                //Get Inventory list.
                DownloadModel importData = _service.DownLoadTemplate(importHeadId, familyId, promotionTypeId);

                //Create response.
                DownloadResponse response = new DownloadResponse { downloadModel = importData };

                //Insert data in cache
                data = InsertIntoCache(routeUri, routeTemplate, response);
            }
            return data;
        }

        //Get the import logs
        public virtual string GetImportLogDetails(int importProcessLogId, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (IsNull(data))
            {
                //Get Inventory list.
                ImportLogDetailsListModel importLogsData = _service.GetImportLogDetails(importProcessLogId, Expands, Filters, Sorts, Page);

                //Create response.
                ImportLogDetailsListResponse response = new ImportLogDetailsListResponse { LogDetailsList = importLogsData?.ImportLogDetails?.ToList(), ImportLogs = importLogsData?.ImportLogs };

                //apply pagination parameters.
                response.MapPagingDataFromModel(importLogsData);

                //Insert data in cache
                data = InsertIntoCache(routeUri, routeTemplate, response);
            }
            return data;
        }

        //Get the import logs
        public virtual string GetImportLogs(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (IsNull(data))
            {
                //Get Inventory list.
                ImportLogsListModel importLogsData = _service.GetImportLogs(Expands, Filters, Sorts, Page);

                //Create response.
                ImportLogsListResponse response = new ImportLogsListResponse { LogsList = importLogsData?.ImportLogs?.ToList() };

                //apply pagination parameters.
                response.MapPagingDataFromModel(importLogsData);

                //Insert data in cache
                data = InsertIntoCache(routeUri, routeTemplate, response);
            }
            return data;
        }

        //Get the import log status
        public virtual string GetLogStatus(int importProcessLogId, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (IsNull(data))
            {
                //Get Inventory list.
                ImportLogsListModel importLogsData = _service.GetLogStatus(importProcessLogId);

                //Create response.
                ImportLogsListResponse response = new ImportLogsListResponse { LogsList = importLogsData?.ImportLogs?.ToList() };

                //Insert data in cache
                data = InsertIntoCache(routeUri, routeTemplate, response);
            }
            return data;
        }

        //Get all families
        public virtual string GetFamilies(bool isCategory, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (IsNull(data))
            {
                //Get Inventory list.
                ImportModel importModel = _service.GetFamilies(isCategory);

                //Create response.
                ImportResponse response = new ImportResponse { Import = importModel };

                //Insert data in cache
                data = InsertIntoCache(routeUri, routeTemplate, response);
            }
            return data;
        }

        //check import status
        public virtual string CheckImportStatus(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (IsNull(data))
            {
                //check status
                bool status = _service.CheckImportStatus();
                BooleanModel model = new BooleanModel { IsSuccess = status };

                //Create response.
                TrueFalseResponse response = new TrueFalseResponse { booleanModel = model };

                //Insert data in cache
                data = InsertIntoCache(routeUri, routeTemplate, response);
            }
            return data;
        }

        public virtual string GetDefaultTemplate(string templateName, string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (IsNull(data))
            {
                //Get template.
                ImportModel importModel = _service.GetDefaultTemplate(templateName);

                //Create response.
                ImportResponse response = new ImportResponse { Import = importModel };

                //Insert data in cache
                data = InsertIntoCache(routeUri, routeTemplate, response);
            }
            return data;
        }

        //Get custom import template list. It will not return the system defined import template.
        public virtual string GetCustomImportTemplateList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (IsNull(data))
            {
                //Get template list.
                ImportManageTemplateListModel importTemplateData = _service.GetCustomImportTemplateList(Expands, Filters, Sorts, Page);

                //Create response.
                ImportTemplateListResponse response = new ImportTemplateListResponse { ImportTemplateList = importTemplateData?.ImportManageTemplates?.ToList() };

                //apply pagination parameters.
                response.MapPagingDataFromModel(importTemplateData);

                //Insert data in cache
                data = InsertIntoCache(routeUri, routeTemplate, response);
            }
            return data;
        }

        //Get Response message for Form Submission export.
        public string GetErrorListforImport(string fileType, int importProcessLogId, string routeUri, string routeTemplate)
        {
            ExportModel exportResponse = _service.GetAllFormsListForExport(fileType,importProcessLogId, Expands, Filters, Sorts, Page);
            //Generate Response
            ExportResponse response = new ExportResponse { ExportMessageModel = exportResponse };

            return ApiHelper.ToJson(response);
        }

        #endregion
    }
}