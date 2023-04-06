using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IPublishCategoryClient : IBaseClient
    {
        /// <summary>
        /// Get publish catalogs 
        /// </summary>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <returns>Publish Category List Model</returns>
        PublishCategoryListModel GetPublishCategoryList(ExpandCollection expands, FilterCollection filters, SortCollection sorts);

        /// <summary>
        /// Get publish catalogs 
        /// </summary>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="pageIndex">Current index of page.</param>
        /// <param name="pageSize">record per page.</param>
        /// <returns>Publish Category List Model</returns>
        PublishCategoryListModel GetPublishCategoryList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        ///  Get publish catalog 
        /// </summary>
        /// <param name="publishCategoryId">Publish Category Id</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="expands">Expand Collection.</param>
        /// <returns>Publish Category Model</returns>
        PublishCategoryModel GetPublishCategory(int publishCategoryId, FilterCollection filters, ExpandCollection expands);

        /// <summary>
        /// Get publish category excluding assigned ids.
        /// </summary>
        /// <param name="assignedIds">already assigned ids.</param>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="pageIndex">Current index of page.</param>
        /// <param name="pageSize">record per page.</param>
        /// <returns>Publish Category List Model</returns>
        PublishCategoryListModel GetUnAssignedPublishCategoryList(string assignedIds, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);


        /// <summary>
        /// Get category list for SEO
        /// </summary>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="pageIndex">Current index of page.</param>
        /// <param name="pageSize">record per page.</param>
        /// <returns>Category List Model</returns>
        PublishCategoryListModel GetCategoryListForSEO(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);
    }
}
