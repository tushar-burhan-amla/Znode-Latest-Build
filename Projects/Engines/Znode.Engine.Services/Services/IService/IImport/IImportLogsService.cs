using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using ZNode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IImportLogsService
    {
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
        /// Delete the import logs
        /// </summary>
        /// <param name="importProcessLogId"></param>
        /// <returns>bool</returns>
        bool DeleteLogDetails(int importProcessLogId);
    }
}
