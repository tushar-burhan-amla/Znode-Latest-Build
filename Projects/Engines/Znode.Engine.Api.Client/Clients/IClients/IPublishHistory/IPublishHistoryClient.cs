using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IPublishHistoryClient : IBaseClient
    {
        /// <summary>
        /// Get publish history message 
        /// </summary>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="pageIndex">Current index of page.</param>
        /// <param name="pageSize">record per page.</param>
        /// <returns>publish history List Model</returns>
        PublishHistoryListModel List(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Delete Product Logs by versionId.
        /// </summary>
        /// <param name="versionId">version Id</param>
        /// <returns>true or false depending on result.</returns>
        bool DeleteProductLogs(int versionId);
    }
}
