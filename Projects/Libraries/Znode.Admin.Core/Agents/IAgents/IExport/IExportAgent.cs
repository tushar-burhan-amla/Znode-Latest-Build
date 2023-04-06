using System.Collections.Generic;

using Znode.Engine.Admin.Models;

using Znode.Admin.Core.ViewModels;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Agents
{
    public interface IExportAgent
    {
        /// <summary>
        /// Get List of Area type to be exported .
        /// </summary>
        /// <param name="type">Area type</param>
        /// <param name="filters">Filter Collection</param>
        /// <param name="sorts">Sorting Collection</param>
        /// <param name="pageIndex">pageIndex</param>
        /// <param name="pageSize">PageSize to get list</param>
        /// <param name="folderId">Folder Id for media</param>
        /// <param name="localId">selected Local of entity</param>
        /// <param name="pimCatalogId">pimCatalogId for catalog filter</param>
        /// <param name="catalogName">catalogName for catalog filter</param>
        /// <returns>Return List of data of Area Type</returns>
        List<dynamic> GetExportList(string type, FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? pageSize = null, int folderId = 0, int localId = 0, int pimCatalogId = 0, string catalogName = null, int paramId = 0);

        /// <summary>
        /// Method to create a data set that contains column name exactly as in grid.
        /// </summary>
        /// <param name="dataModel">dataModel</param>
        /// <returns>Returns required data set.</returns>
        List<dynamic> CreateDataSource(List<dynamic> dataModel);

        /// <summary>
        /// Get Response message for Export Form Submission.
        /// </summary>
        /// <param name="exportFileTypeId">export File TypeId</param>
        /// <param name="filters">Filter Collection</param>
        /// <param name="sorts">Sorting Collection</param>
        /// <param name="pageIndex">pageIndex</param>
        /// <param name="pageSize">PageSize to get list</param>
        /// <param name="folderId">Folder Id for media</param>
        /// <returns>returns Export FormSubmission List.</returns>
        ExportResponseMessageModel GetExportFormSubmissionList(string exportType, FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? pageSize = null, int folderId = 0);

     


        /// <summary>
        /// Get the export logs
        /// </summary>
        /// <param name="expands">ExpandCollection</param>
        /// <param name="filters">FilterCollection</param>
        /// <param name="sorts">SortCollection</param>
        /// <param name="pageIndex">PageIndex</param>
        /// <param name="pageSize">PageSize</param>
        /// <returns>exportProcessLogsListViewModel</returns>
        ExportProcessLogsListViewModel GetExportLogs(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int pageIndex, int pageSize);

        /// <summary>
        /// Download Export File.
        /// </summary>
        /// <param name="tableName">Table Name</param>
        /// <returns>return download export file name.</returns>
        string DownloadExportFile(string tableName);

        /// <summary>
        /// Get Zip File.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns>return zip file.</returns>
        byte[] GetZipFile(string filePath);
        /// <summary>
        /// Delete the Export logs
        /// </summary>
        /// <param name="exportProcessLogIds">exportProcessLogIds</param>
        /// <returns>Returns Response  for Export Delete Log </returns>
        bool DeleteLog(string exportProcessLogIds);

        /// <summary>
        /// Get List of Area type to be exported .
        /// </summary>
        /// <param name="type">Area type</param>
        /// <param name="filters">Filter Collection</param>
        /// <param name="sorts">Sorting Collection</param>
        /// <param name="pageIndex">pageIndex</param>
        /// <param name="pageSize">PageSize to get list</param>
        /// <param name="folderId">Folder Id for media</param>
        /// <param name="localId">selected Local of entity</param>
        /// <param name="pimCatalogId">pimCatalogId for catalog filter</param>
        /// <param name="catalogName">catalogName for catalog filter</param>
        /// <returns>Return List of data of Area Type</returns>
        ExportResponseMessageModel GetExportLogList(string fileType,string type, FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? pageSize = null, int folderId = 0, int localId = 0, int pimCatalogId = 0, string catalogName = null, int paramId = 0);
      // </summary>
    }
}
