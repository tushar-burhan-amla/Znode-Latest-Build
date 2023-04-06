using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IPublishCatalogService
    {
        /// <summary>
        ///  Get publish catalog 
        /// </summary>
        /// <param name="publishCatalogId">Publish Catalog Id</param>
        /// <param name="localeId"></param>
        /// <param name="expands">Expand Collection.</param>
        /// <returns>Publish Catalog Model</returns>
        PublishCatalogModel GetPublisCatalog(int publishCatalogId, int? localeId, NameValueCollection expands);

        /// <summary>
        /// Get publish catalogs 
        /// </summary>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="Page">paging parameters.</param>
        /// <returns>Publish Catalog List Model</returns>
        PublishCatalogListModel GetPublisCatalogList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get list of publish category excluding assigned ids.
        /// </summary>
        /// <param name="assignedIds">AssignedIds</param>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="Page">paging parameters.</param>
        /// <returns>Publish Catalog List Model</returns>
        PublishCatalogListModel GetUnAssignedPublishCatelogList(string assignedIds, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);
    }
}
