using System.Web;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.WebStore.Agents
{
    public interface IImportAgent
    {
        /// <summary>
        /// Download the shipping address template.
        /// </summary>        
        /// <param name="response">HttpResponseBase</param>
        /// <returns>ImportViewModel</returns>
        ImportViewModel DownloadShippingAddressTemplate(HttpResponseBase response);

        /// <summary>
        /// Import shipping address.
        /// </summary>
        /// <param name="postedFile"></param>
        /// <returns>ImportShippingAddressViewModel</returns>
        ImportViewModel ImportShippingAddress(HttpPostedFileBase postedFile);

        /// <summary>
        /// Download the user template.
        /// </summary>
        /// <param name="response"></param>
        /// <returns>ImportViewModel</returns>
        ImportViewModel DownloadUserTemplate(HttpResponseBase response);

        /// <summary>
        /// Import user.
        /// </summary>
        /// <param name="postedFile"></param>
        /// <returns>ImportViewModel</returns>
        ImportViewModel ImportUsers(HttpPostedFileBase postedFile);

        /// <summary>
        /// Get the import logs.
        /// </summary>
        /// <param name="expands">ExpandCollection</param>
        /// <param name="filters">FilterCollection</param>
        /// <param name="sorts">SortCollection</param>
        /// <param name="pageIndex">PageIndex</param>
        /// <param name="pageSize">PageSize</param>
        /// <returns>ImportProcessLogsListViewModel</returns>
        ImportProcessLogsListViewModel ImportShippingLogs(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int pageIndex, int pageSize);

        /// <summary>
        /// Get import logs details.
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
        /// Delete the import logs.
        /// </summary>
        /// <param name="importProcessLogIds">ImportProcessLogIds</param>
        /// <returns>Returns true if deleted successfully.</returns>
        bool DeleteLog(string importProcessLogIds);

        /// <summary>
        /// Check if the user is an Admin user
        /// </summary>
        /// <returns>Returns true is user is having role Administrator</returns>
        bool IsAdminUser();

        /// <summary>
        /// Get the import log data
        /// </summary>
        ///  <param name="expands">ExpandCollection</param>
        /// <param name="filters">FilterCollection</param>
        /// <param name="sorts">SortCollection</param>
        /// <param name="pageIndex">PageIndex</param>
        /// <param name="pageSize">PageSize</param>
        /// <returns>Returns user import process logs list.</returns>
        ImportProcessLogsListViewModel ImportUserLogs(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int pageIndex, int pageSize);
    }
}
