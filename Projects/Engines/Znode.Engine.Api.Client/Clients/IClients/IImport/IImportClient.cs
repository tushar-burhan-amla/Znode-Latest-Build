using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IImportClient : IBaseClient
    {
        /// <summary>
        /// Get all Import types.
        /// </summary>
        /// <returns>ImportModel</returns>
        ImportModel GetImportTypeList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get All Templates with respect to Import Head Id
        /// </summary>
        /// <param name="importHeadId">Import Head Id</param>
        /// <param name="promotionTypeId">Promotion Type Id</param>
        /// <returns>ImportModel</returns>
        ImportModel GetAllTemplates(int importHeadId, int familyId, int promotionTypeId = 0);

        /// <summary>
        /// Get all templates data with respect to template id
        /// </summary>
        /// <param name="templateId">Template Id</param>
        /// <param name="promotionTypeId">Promotion Type Id</param>
        /// <returns>ImportModel</returns>
        ImportModel GetTemplateData(int templateId, int importHeadId, int familyId, int  promotionTypeId = 0);

        /// <summary>
        /// Post and process the import data
        /// </summary>
        /// <param name="model">Import Model</param>
        /// <returns>bool</returns>
        bool ImportData(ImportModel model);

        /// <summary>
        /// Download the template in CSV format
        /// </summary>
        /// <param name="importHeadId">Import Head Id</param>
        /// <param name="downloadImportPromotionTypeId">Download Import Promotion Type Id</param>
        /// <returns>DownloadModel</returns>
        DownloadModel DownloadTemplate(int importHeadId, int downloadImportFamilyId, int downloadImportPromotionTypeId = 0);

        /// <summary>
        /// Gets the Import Logs to check import status
        /// </summary>
        /// <param name="expands">Expand Collection</param>
        /// <param name="filters">Filter Collection</param>
        /// <param name="sorts">Sort Collection</param>
        /// <param name="pageIndex">Page Index</param>
        /// <param name="pageSize">Page Size</param>
        /// <returns>Import Logs List Model</returns>
        ImportLogsListModel GetImportLogs(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int pageIndex, int pageSize);

        /// <summary>
        /// Gets the Import Logs details on the basis of importLogId
        /// </summary>
        /// <param name="importLogId">Import Log Id</param>
        /// <param name="expands">Expand Collection</param>
        /// <param name="filters">Filter Collection</param>
        /// <param name="sorts">Sort Collection</param>
        /// <param name="pageIndex">Page Index</param>
        /// <param name="pageSize">Page Size</param>
        /// <returns>Import Logs List Model</returns>
        ImportLogDetailsListModel GetImportLogDetails(int importProcessLogId, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int pageIndex, int pageSize);

        /// <summary>
        /// Get the import logs current status
        /// </summary>
        /// <param name="importLogId">Import Log Id</param>
        /// <param name="expands">Expand Collection</param>
        /// <param name="filters">Filter Collection</param>
        /// <param name="sorts">Sort Collection</param>
        /// <param name="pageIndex">Page Index</param>
        /// <param name="pageSize">Page Size</param>
        /// <returns>Import Logs List Model</returns>
        ImportLogsListModel GetImportLogStatus(int importProcessLogId, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int pageIndex, int pageSize);

        /// <summary>
        /// Delete the logs from ZnodeImportLog and ZnodeImportProcessLog table
        /// </summary>
        /// <param name="importProcessLogId">ParameterModel</param>
        /// <returns>bool</returns>
        bool DeleteLogs(ParameterModel importProcessLogId);

        /// <summary>
        /// Get all families for product import
        /// </summary>
        /// <param name="isCategory">isCategory</param>
        /// <returns>ImportProductFamilyListModel</returns>
        ImportProductFamilyListModel GetAllFamilies(bool isCategory);

        /// <summary>
        /// Update Import Mappings
        /// </summary>
        /// <param name="filters">Filter Collection</param>
        /// <param name="model">ImportModel</param>
        /// <returns>bool</returns>
        bool UpdateMappings(ImportModel model, FilterCollection filters);

        /// <summary>
        /// Check import process is going on or not
        /// </summary>
        /// <returns>bool</returns>
        bool CheckImportProcess();

        /// <summary>
        /// Get default template for import data.
        /// </summary>
        /// <param name="templateName"></param>
        /// <returns>ImportModel</returns>
        ImportModel GetDefaultTemplate(string templateName);

        ///// <summary>
        ///// Get custom import template list. It will not return the system defined import template.
        ///// </summary>
        ///// <param name="expands">Expand Collection</param>
        ///// <param name="filters">Filter Collection</param>
        ///// <param name="sorts">Sort Collection</param>
        ///// <param name="pageIndex">Page Index</param>
        ///// <param name="pageSize">Page Size</param>
        ///// <returns>ImportManageTemplateListModel</returns>
        ImportManageTemplateListModel GetCustomImportTemplateList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int pageIndex, int pageSize);

        /// <summary>
        /// Deletes the custom import templates. It will not delete the system defined import templates.
        /// </summary>
        /// <param name="importTemplateId">ParameterModel</param>
        /// <returns>bool</returns>
        bool DeleteImportTemplate(ParameterModel importTemplateId);


        /// <summary>
        /// Gets the Import Logs details on the basis of importProcessLogId & filetype
        /// </summary>
        /// <param name="filetype">Import Log Id</param>
        /// <param name="importProcessLogId">Import Log Id</param>
        /// <param name="expands">Expand Collection</param>
        /// <param name="filters">Filter Collection</param>
        /// <param name="sorts">Sort Collection</param>
        /// <param name="pageIndex">Page Index</param>
        /// <param name="pageSize">Page Size</param>
        /// <returns>Export Model</returns>
        ExportModel GetImportLogDetailsList(string filetype,int importProcessLogId, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int pageIndex, int pageSize);
        
        /// <summary>
        /// Check Export process is going on or not
        /// </summary>
        /// <returns>bool</returns>
        bool CheckExportProcess();
    }
}
