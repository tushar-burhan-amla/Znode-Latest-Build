using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IPublishCatalogClient : IBaseClient
    {
        /// <summary>
        /// Get publish catalogs 
        /// </summary>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <returns>Publish Catalog List Model</returns>
        PublishCatalogListModel GetPublishCatalogList(ExpandCollection expands, FilterCollection filters, SortCollection sorts);

        /// <summary>
        /// Get publish catalogs
        /// </summary>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="pageIndex">Current index of page.</param>
        /// <param name="pageSize">record per page.</param>
        /// <returns>Publish Catalog List Model</returns>
        PublishCatalogListModel GetPublishCatalogList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        ///  Get publish catalog 
        /// </summary>
        /// <param name="publishCatalogId">Publish Catalog Id</param>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="localeId"></param>
        /// <returns>Publish Catalog Model</returns>
        PublishCatalogModel GetPublishCatalog(int publishCatalogId, ExpandCollection expands, int? localeId = null);

        /// <summary>
        /// Get publish catalogs excluding assigned ids.
        /// </summary>
        /// <param name="assignedIds">already assigned ids.</param>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="pageIndex">Current index of page.</param>
        /// <param name="pageSize">record per page.</param>
        /// <returns>Publish Category List Model</returns>
        PublishCatalogListModel GetUnAssignedPublishCatelogList(string assignedIds, ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

    }
}
