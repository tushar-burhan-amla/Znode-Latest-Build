using Znode.Engine.Api.Client.Expands;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Client
{
    public interface ICatalogHistoryClient : IBaseClient
    {
        /// <summary>
        /// Gets List of catalog history.
        /// </summary>
        /// <param name="expands">Expands for catalog history list.</param>
        /// <param name="filters">Filters for catalog history list.</param>
        /// <param name="sorts">Sorts for catalog history list.</param>
        /// <returns>List of catalog history.</returns>
        CatalogHistoryListModel GetCatalogHistoryList(ExpandCollection expands, FilterCollection filters, SortCollection sorts);

        /// <summary>
        /// Gets paged list of catalog history.
        /// </summary>
        /// <param name="expands">Expands for catalog history list.</param>
        /// <param name="filters">Filters for catalog history list.</param>
        /// <param name="sorts">Sorts for catalog history list.</param>
        /// <param name="pageIndex">Start page index of catalog history list.</param>
        /// <param name="pageSize">Page size of catalog history list.</param>
        /// <returns>Paged list of catalog history</returns>
        CatalogHistoryListModel GetCatalogHistoryList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Gets catalog history for the specified ID.
        /// </summary>
        /// <param name="id">ID of catalog history.</param>
        /// <param name="expands">Expands for catalog history.</param>
        /// <returns>Catalog history model according to ID.</returns>
        CatalogHistoryModel GetCatalogHistory(int id, ExpandCollection expands);

        /// <summary>
        /// Creates a catalog history.
        /// </summary>
        /// <param name="catalogHistoryModel">Catalog History model.</param>
        /// <returns>New catalog history model.</returns>
        CatalogHistoryModel CreateCatalogHistoryModel(CatalogHistoryModel catalogHistoryModel);

        /// <summary>
        /// Updates a catalog history Model.
        /// </summary>
        /// <param name="catalogHistoryModel">Catalog history model with updated values.</param>
        /// <returns>Updated catalog history model.</returns>
        CatalogHistoryModel UpdateCatalogHistoryModel(CatalogHistoryModel catalogHistoryModel);

        /// <summary>
        /// Deletes a catalog history.
        /// </summary>
        /// <param name="id">ID of the catalog history.</param>
        /// <returns>True if catalog history is deleted; False if catalog history is not deleted.</returns>
        bool DeleteCatalogHistoryModel(int id);
    }
}
