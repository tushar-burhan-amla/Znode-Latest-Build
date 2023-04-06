using System.Collections.Generic;
using System.Web;
using System.Web.Mvc;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Admin.Models;

namespace Znode.Engine.Admin.Agents
{
    //Interface for Import Agent
    public interface IImportAgent
    {
        /// <summary>
        /// This method will fetch the list of all import types.
        /// </summary>
        /// <returns></returns>
        ImportViewModel BindViewModel();

        /// <summary>
        /// This method process the data from the file for import.
        /// </summary>
        /// <param name="importModel"></param>
        /// <returns>bool</returns>
        bool ImportData(ImportViewModel importModel);

        /// <summary>
        /// This method will get the saved Template list
        /// </summary>
        /// <param name="importHeadId"></param>
        /// <param name="promotionTypeId">Promotion Type Id</param>
        /// <returns></returns>
        List<SelectListItem> GetImportTemplateList(int importHeadId, int familyId, int promotionTypeId = 0);

        /// <summary>
        /// This method will get the mapped template list
        /// </summary>
        /// <param name="templateId"></param>
        /// <param name="promotionTypeId">Promotion Type Id</param>
        /// <returns></returns>
        List<List<SelectListItem>> GetImportTemplateMappingList(int templateId, int importHeadId, int familyId, int promotionTypeId = 0);

        /// <summary>
        /// This method is to get the CSV file headers
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        string GetCsvHeaders(HttpPostedFileBase fileName, out string changedFileName);

        /// <summary>
        /// Download the template
        /// </summary>
        /// <param name="importHeadId">Import Head Id</param>
        /// <param name="downloadImportPromotionTypeId">Download Import Promotion Type Id</param>
        /// <param name="response">HttpResponseBase</param>
        ImportViewModel DownloadTemplate(int importHeadId, string importName, int downloadImportFamilyId, int downloadImportPromotionTypeId, HttpResponseBase response);

        /// <summary>
        /// Get the import logs
        /// </summary>
        /// <param name="expands">ExpandCollection</param>
        /// <param name="filters">FilterCollection</param>
        /// <param name="sorts">SortCollection</param>
        /// <param name="pageIndex">PageIndex</param>
        /// <param name="pageSize">PageSize</param>
        /// <returns>ImportProcessLogsListViewModel</returns>
        ImportProcessLogsListViewModel GetImportLogs(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int pageIndex, int pageSize);

        /// <summary>
        /// Get the Import Logs status
        /// </summary>
        /// <param name="importProcessLogId">ImportProcessLogId</param>
        /// <param name="expands">ExpandCollection</param>
        /// <param name="filters">FilterCollection</param>
        /// <param name="sorts">SortCollection</param>
        /// <param name="pageIndex">PageIndex</param>
        /// <param name="pageSize">PageSize</param>
        /// <returns>ImportProcessLogsListViewModel</returns>
        ImportProcessLogsListViewModel GetImportLogStatus(int importProcessLogId, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int pageIndex, int pageSize);

        /// <summary>
        /// Get import logs details
        /// </summary>
        /// <param name="importProcessLogId">ImportProcessLogId</param>
        ///  <param name="expands">ExpandCollection</param>
        /// <param name="filters">FilterCollection</param>
        /// <param name="sorts">SortCollection</param>
        /// <param name="pageIndex">PageIndex</param>
        /// <param name="pageSize">PageSize</param>
        /// <returns>ImportLogsListViewModel</returns>
        ImportLogsListViewModel GetImportLogDetails(int importProcessLogId, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int pageIndex, int pageSize);

        /// <summary>
        /// Get import logs details List
        /// </summary>
        /// <param name="importProcessLogId">ImportProcessLogId</param>
        ///  <param name="expands">ExpandCollection</param>
        /// <param name="filters">FilterCollection</param>
        /// <param name="sorts">SortCollection</param>
        /// <param name="pageIndex">PageIndex</param>
        /// <param name="pageSize">PageSize</param>
        /// <returns>byte[]</returns>
        ImportLogsDownloadListViewModel DownloadImportLogDetails(int importProcessLogId, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int pageIndex, int pageSize);

        /// <summary>
        /// Delete the Import logs
        /// </summary>
        /// <param name="importProcessLogIds">importProcessLogIds</param>
        /// <returns>bool</returns>
        bool DeleteLog(string importProcessLogIds);

        /// <summary>
        /// Get all families for product import
        /// </summary>
        /// <param name="isCategory">isCategory</param>
        /// <returns>List<SelectListItem></returns>
        List<SelectListItem> GetAllFamilies(bool isCategory);

        /// <summary>
        /// Update Mappings
        /// </summary>
        /// <param name="model">ImportViewModel</param>
        /// <returns>bool</returns>
        bool UpdateMappings(ImportViewModel model);

        /// <summary>
        /// Get the import type list
        /// </summary>
        /// <param name="isDynamicReport">isDynamicReport</param>
        /// <returns>List<SelectListItem></returns>
        List<SelectListItem> GetImportTypeList(bool isDynamicReport = false);

        /// <summary>
        /// Get pricing list.
        /// </summary>
        /// <returns>SelectListItem list</returns>
        List<SelectListItem> GetPricingList();

        /// <summary>
        /// Get country list.
        /// </summary>
        /// <returns>SelectListItem country list.</returns>
        List<SelectListItem> GetCountryList();

        /// <summary>
        /// Get Portal list.
        /// </summary>
        /// <returns>SelectListItem Portal list.</returns>
        List<SelectListItem> GetPortalList();

        /// <summary>
        /// Check if the import process is going on or not
        /// </summary>
        /// <returns>bool</returns>
        bool CheckImportProcess();

        /// <summary>
        /// Get catalog list.
        /// </summary>
        /// <returns>SelectListItem catalog list.</returns>
        List<SelectListItem> GetCatalogList();

        /// <summary>
        /// Get promotions type list.
        /// </summary>
        /// <returns>SelectListItem promotion type list.</returns>
        List<SelectListItem> GetPromotionTypeList();

        /// <summary>
        /// Get custom import template list. It will not return the system defined import template.
        /// </summary>
        /// <param name="expands">ExpandCollection</param>
        /// <param name="filters">FilterCollection</param>
        /// <param name="sorts">SortCollection</param>
        /// <param name="pageIndex">PageIndex</param>
        /// <param name="pageSize">PageSize</param>
        /// <returns>ImportTemplateMappingResponseListViewModel</returns>
        ImportManageTemplateMappingListViewModel GetCustomImportTemplateList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int pageIndex, int pageSize);

        /// <summary>
        /// Deletes the custom import templates. It will not delete the system defined import templates.
        /// </summary>
        /// <param name="importTemplateId">importTemplateIds</param>
        /// <returns>bool</returns>
        bool DeleteImportTemplate(string importTemplateId);

        /// <summary>
        /// get import details for csv,excel and pdf download.
        /// </summary>
        /// <param name="fileType">fileType</param>
        /// <param name="importProcessLogId">importProcessLogId</param>
        /// <returns>ExportResponseMessageModel</returns>

        // get import details for csv,excel and pdf download.
        ExportResponseMessageModel GetImportErrorLogList(string fileType,int importProcessLogId, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int pageIndex, int pageSize);

        /// <summary>
        /// Check if the export process is going on or not
        /// </summary>
        /// <returns>bool</returns>
        bool CheckExportProcess();


    }
}
