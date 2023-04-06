using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface ICatalogHistoryService
    {
        /// <summary>
        /// Gets the list of catalog history.
        /// </summary>
        /// <param name="expands">Expands for catalog history list.</param>
        /// <param name="filters">Filters for catalog history list.</param>
        /// <param name="sorts">Sorting for catalog history list.</param>
        /// <param name="page">Paging for catalog history list.</param>
        /// <returns>List of catalog history.</returns>
        CatalogHistoryListModel GetCatalogHistoryList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Gets Catalog history by specified ID.
        /// </summary>
        /// <param name="id">ID of catalog history.</param>
        /// <param name="expands">Expands for catalog history.</param>
        /// <returns>Catalog history for provided ID.</returns>
        CatalogHistoryModel GetCatalogHistoryById(int id, NameValueCollection expands);

        /// <summary>
        /// Updates a catalog history.
        /// </summary>
        /// <param name="catalogHistoryModel">Catalog History Model with updated values.</param>
        /// <returns>Updated catalog history model</returns>
        bool UpdateCatalogHistory(CatalogHistoryModel catalogHistoryModel);

        /// <summary>
        /// Creates a catalog history.
        /// </summary>
        /// <param name="catalogHistoryModel">Catalog history model to be created.</param>
        /// <returns>Newly created catalg history model.</returns>
        CatalogHistoryModel CreateCatalogHistory(CatalogHistoryModel catalogHistoryModel);

        /// <summary>
        /// Deletes a catalog history.
        /// </summary>
        /// <param name="id">ID of the catalog history to be deleted.</param>
        /// <returns>True if catalog history is deleted;False if catalog history is not deleted.</returns>
        bool DeleteCatalogHistory(int id);
    }
}
