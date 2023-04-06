using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Agents
{
    public interface IPublishHistoryAgent
    {
        /// <summary>
        /// Get Publish History List.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with Publish History list.</param>
        /// <param name="filters">Filters to be applied on Publish History list.</param>
        /// <param name="sorts">Sorting to be applied on Publish History list.</param>
        /// <param name="pageIndex">Start page index of Publish History list.</param>
        /// <param name="pageSize">Records per page in Publish History list.</param>
        /// <returns></returns>
        PublishHistoryListViewModel GetPublishHistoryList(string publishState, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Delete Product Log.
        /// </summary>
        /// <param name="versionId">versionId</param>
        /// <returns>true or false.</returns>
        bool DeleteProductLog(int versionId);
    }
}
