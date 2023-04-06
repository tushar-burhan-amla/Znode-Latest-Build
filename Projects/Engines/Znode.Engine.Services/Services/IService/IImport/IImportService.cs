using System.Collections.Specialized;
using System.Data;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IImportService
    {
        /// <summary>
        /// Process the data from the file.  
        /// This method will fetch the data from file and insert it into DB and then
        /// inserted data will be processed.
        /// </summary>
        /// <param name="importModel">ImportModel</param>
        /// <returns>ImportModel</returns>
        int ProcessData(ImportModel importModel);

        /// <summary>
        /// get all templates with respect to import head id
        /// </summary>
        /// <param name="importHeadId">Import Head Id</param>
        /// <param name="promotionTypeId">Promotion Type Id</param>
        /// <returns>ImportModel</returns>
        ImportModel GetAllTemplates(int importHeadId, int familyId, int promotionTypeId = 0);

        /// <summary>
        /// get complete list of import types available
        /// </summary>
        /// <returns>ImportModel</returns>
        ImportModel GetImportTypeList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// get all data of templates
        /// </summary>
        /// <param name="templateId"></param>
        /// <param name="promotionTypeId">Promotion Type Id</param>
        /// <returns>ImportModel</returns>
        ImportModel GetTemplateData(int templateId, int importHeadId, int familyId, int promotionTypeId = 0);

        /// <summary>
        /// Downloads the model
        /// </summary>
        /// <param name="importHeadId">Import Head Id</param>
        /// <param name="promotionTypeId">Promotion Type Id</param>
        /// <returns></returns>
        DownloadModel DownLoadTemplate(int importHeadId, int familyId, int promotionTypeId = 0);

        /// <summary>
        /// Get the import log details on the basis of Import Log Id
        /// </summary>
        /// <param name="importLogId">Import Log Id</param>
        /// <param name="expands">Name Value Collection</param>
        /// <param name="filters">Filter Collection</param>
        /// <param name="sorts">Name Value Collection</param>
        /// <param name="page">Name Value Collection</param>
        /// <returns>ImportLogDetailsListModel</returns>
        ImportLogDetailsListModel GetImportLogDetails(int importProcessLogId, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get the Import Logs
        /// </summary>
        /// <param name="expands"></param>
        /// <param name="filters"></param>
        /// <param name="sorts"></param>
        /// <param name="page"></param>
        /// <returns></returns>
        ImportLogsListModel GetImportLogs(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get the import log status
        /// </summary>
        /// <param name="importProcessLogId"></param>
        /// <returns>ImportLogsListModel</returns>
        ImportLogsListModel GetLogStatus(int importProcessLogId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="importProcessLogId"></param>
        /// <returns></returns>
        ImportLogsModel GetCurrentLogStatus(int importProcessLogId);

        /// <summary>
        /// Delete the import logs
        /// </summary>
        /// <param name="importProcessLogId">ParameterModel</param>
        /// <returns>bool</returns>
        bool DeleteLogDetails(ParameterModel importProcessLogIds);

        /// <summary>
        /// Get all families for Product and Category Import
        /// </summary>
        /// <param name="isCategory">isCategory</param>
        /// <returns>ImportModel</returns>
        ImportModel GetFamilies(bool isCategory);

        /// <summary>
        /// Updates the saved mapping against the template
        /// </summary>
        /// <param name="model">ImportModel</param>
        /// <returns>Int</returns>
        int UpdateTemplateMappings(ImportModel model);

        /// <summary>
        /// Check import status
        /// </summary>
        /// <returns>bool</returns>
        bool CheckImportStatus();

        /// <summary>
        /// Get default template for import the data.
        /// </summary>
        /// <param name="templateName"></param>
        /// <returns>ImportModel</returns>
        ImportModel GetDefaultTemplate(string templateName);

        ///// <summary>
        ///// Get custom import template list. It will not return the system defined import template.
        ///// </summary>
        ///// <param name="expands"></param>
        ///// <param name="filters"></param>
        ///// <param name="sorts"></param>
        ///// <param name="page"></param>
        ///// <returns></returns>
        ImportManageTemplateListModel GetCustomImportTemplateList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Deletes the custom import templates. It will not delete the system defined import templates.
        /// </summary>
        /// <param name="importTemplateId">ParameterModel</param>
        /// <returns>bool</returns>
        bool DeleteImportTemplate(ParameterModel importTemplateIds);


        /// <summary>
        /// Get All Forms List For Export.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with form submission list.</param>
        /// <param name="filters">Filters to be applied on form submission list.</param>
        /// <param name="sorts">Sorting to be applied on form submission list.</param>
        /// <param name="page">Page index.</param>
        /// <param name="exportFileTypeId">export File Type Id</param>
        /// <returns>returns list of form submission</returns>
        ExportModel GetAllFormsListForExport(string fileType, int ImportProcessLogId,NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Check export status
        /// </summary>
        /// <returns>bool</returns>
        bool IsExportPublishInProgress();
    }
}
