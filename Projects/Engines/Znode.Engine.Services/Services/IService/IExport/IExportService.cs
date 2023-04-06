using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;

using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IExportService
    {
        /// <summary>
        /// Get the export Logs.
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="page">paging parameters.</param>
        /// <returns>ExportLogsListModel</returns>
        ExportLogsListModel GetExportLogs(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get Export File Names List.
        /// </summary>
        /// <returns>return Export File Names List.</returns>
        List<string> GetExportFileNamesList();

        /// <summary>
        /// Delete Expired Export Files.
        /// </summary>
        /// <returns>return true or false based on file deletion.</returns>
        bool DeleteExpiredExportFiles();

        /// <summary>
        /// Get Export File Path.
        /// </summary>
        /// <returns>return Get Export File Path.</returns>
        string GetExportFilePath(string tableName);

        /// <summary>
        /// Create File Path.
        /// </summary>
        /// <param name="path">path</param>
        /// <param name="folderName">folderName</param>
        /// <returns>return path.</returns>
        string CreateFilePath(string path, string folderName);

        /// <summary>
        /// Save File.
        /// </summary>
        /// <param name="dtDataTable">dtDataTable</param>
        /// <param name="exportTypeId">exportTypeId</param>
        /// <param name="type">type</param>
        /// <param name="tableName">tableName</param>
        /// <param name="loopCount">loopCount</param>
        void SaveFile(DataTable dtDataTable, string exportType, string type, string tableName, int loopCount, int? chunkSizeForRemainingData);

        /// <summary>
        /// Get Zip File.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="folderName"></param>
        void GetZipFile(string filePath, string folderName);

        /// <summary>
        /// Delete Temporary Created Folder.
        /// </summary>
        /// <param name="strFilePath"></param>
        /// <param name="folderName"></param>
        void DeleteTemporaryCreatedFolder(string strFilePath, string folderName);

        /// <summary>
        /// Delete Export Files.
        /// </summary>
        /// <param name="exportProcessLogIds">export Process Log Ids</param>
        /// <returns></returns>
        bool DeleteExportFiles(ParameterModel exportProcessLogIds);
    }
}
