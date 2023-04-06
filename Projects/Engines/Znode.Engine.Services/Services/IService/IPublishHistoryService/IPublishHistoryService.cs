using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IPublishHistoryService
    {
        /// <summary>
        /// Get Publish History List.
        /// </summary>
        /// <param name="expands">Expands to be retrieved along with warehouse list.</param>
        /// <param name="filters">Filters to be applied on warehouse list.</param>
        /// <param name="sorts">Sorting to be applied on warehouse list.</param>
        /// <param name="page">Page index.</param>
        /// <returns>Returns list of Publish History.</returns>
        PublishHistoryListModel GetPublishHistoryList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Delete Product Logs by versionId.
        /// </summary>
        /// <param name="versionId">versionId</param>
        void DeleteProductLogs(int versionId);
    }
}
