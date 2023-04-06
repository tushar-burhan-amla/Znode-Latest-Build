using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface ICategoryHistoryService
    {
        /// <summary>
        /// Gets the list of category history.
        /// </summary>
        /// <param name="expands">Expands for category history list.</param>
        /// <param name="filters">Filters for category history list.</param>
        /// <param name="sorts">Sorting for category history list.</param>
        /// <param name="page">Paging for category history list.</param>
        /// <returns>List of category history.</returns>
        CategoryHistoryListModel GetCategoryHistoryList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Gets category history by specified ID.
        /// </summary>
        /// <param name="id">ID of category history.</param>
        /// <param name="expands">Expands for category history.</param>
        /// <returns>Category history for provided ID.</returns>
        CategoryHistoryModel GetCategoryHistoryById(int id, NameValueCollection expands);

        /// <summary>
        /// Updates a category history.
        /// </summary>
        /// <param name="categoryHistoryModel">category History Model with updated values.</param>
        /// <returns>Updated category history model</returns>
        bool UpdateCategoryHistory(CategoryHistoryModel categoryHistoryModel);

        /// <summary>
        /// Creates a category history.
        /// </summary>
        /// <param name="categoryHistoryModel">category history model to be created.</param>
        /// <returns>Newly created category history model.</returns>
        CategoryHistoryModel CreateCategoryHistory(CategoryHistoryModel categoryHistoryModel);

        /// <summary>
        /// Deletes a category history.
        /// </summary>
        /// <param name="id">ID of the category history to be deleted.</param>
        /// <returns>True if category history is deleted;False if category history is not deleted.</returns>
        bool DeleteCategoryHistory(int id);
    }
}
