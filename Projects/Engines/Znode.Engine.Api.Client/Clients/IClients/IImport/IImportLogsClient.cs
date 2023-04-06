using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using ZNode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IImportLogsClient
    {
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
        /// <param name="importProcessLogId">ImportProcessLogId</param>
        /// <returns>bool</returns>
        bool DeleteLogs(int importProcessLogId);
    }
}
