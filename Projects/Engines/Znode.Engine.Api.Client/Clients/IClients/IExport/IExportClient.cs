using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IExportClient : IBaseClient
    {
        /// <summary>
        /// Gets the Export Logs to check export status.
        /// </summary>
        /// <param name="expands">Expand Collection</param>
        /// <param name="filters">Filter Collection</param>
        /// <param name="sorts">Sort Collection</param>
        /// <param name="pageIndex">Page Index</param>
        /// <param name="pageSize">Page Size</param>
        /// <returns>Export Logs List Model</returns>
        ExportLogsListModel GetExportLogs(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int pageIndex, int pageSize);

        /// <summary>
        /// Delete Export Files.
        /// </summary>
        /// <returns>returns true false value for export file delete.</returns>
        bool DeleteExportFiles();

        /// <summary>
        /// Get Export File Path.
        /// </summary>
        /// <returns>return export file path.</returns>
        string GetExportFilePath(string tableName);

        /// <summary>
        /// Delete the logs from ZnodeExportLog and ZnodeExportProcessLog table
        /// </summary>
        /// <param name="exportProcessLogId">ParameterModel</param>
        /// <returns>return Export Delete logs List</returns>
        bool DeleteLogs(ParameterModel exportProcessLogIds);
    }

}
