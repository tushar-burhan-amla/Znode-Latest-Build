using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IPublishCategoryService
    {
        /// <summary>
        /// Get publish Category 
        /// </summary>
        /// <param name="publishCategoryId"></param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="expands">expandcollection</param>
        /// <returns>Publish Category Model</returns>
        PublishCategoryModel GetPublishCategory(int publishCategoryId, FilterCollection filters, NameValueCollection expands);

        /// <summary>
        /// Get publish Category 
        /// </summary>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="Page">paging parameters.</param>
        /// <returns>Publish Category List Model</returns>
        PublishCategoryListModel GetPublishCategoryList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);


        /// <summary>
        /// Get publish Category excluding assigned Ids.
        /// </summary>
        /// <param name="assignedIds">assigned Ids.</param>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="Page">paging parameters.</param>
        /// <returns>Publish Category List Model</returns>
        PublishCategoryListModel GetUnAssignedPublishCategoryList(string assignedIds, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

    }
}
