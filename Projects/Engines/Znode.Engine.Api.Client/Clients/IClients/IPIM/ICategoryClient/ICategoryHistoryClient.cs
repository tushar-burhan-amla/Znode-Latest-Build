using Znode.Engine.Api.Client.Expands;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Client
{
    public interface ICategoryHistoryClient : IBaseClient
    {
        /// <summary>
        /// Gets List of category history.
        /// </summary>
        /// <param name="expands">Expands for category history list.</param>
        /// <param name="filters">Filters for category history list.</param>
        /// <param name="sorts">Sorts for category history list.</param>
        /// <returns>List of category history.</returns>
        CategoryHistoryListModel GetCategoryHistoryList(ExpandCollection expands, FilterCollection filters, SortCollection sorts);

        /// <summary>
        /// Gets paged list of category history.
        /// </summary>
        /// <param name="expands">Expands for category history list.</param>
        /// <param name="filters">Filters for category history list.</param>
        /// <param name="sorts">Sorts for category history list.</param>
        /// <param name="pageIndex">Start page index of category history list.</param>
        /// <param name="pageSize">Page size of category history list.</param>
        /// <returns>Paged list of category history</returns>
        CategoryHistoryListModel GetCategoryHistoryList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Gets category history for the specified ID.
        /// </summary>
        /// <param name="id">ID of category history.</param>
        /// <param name="expands">Expands for category history.</param>
        /// <returns>Category history model according to ID.</returns>
        CategoryHistoryModel GetCategoryHistory(int id, ExpandCollection expands);

        /// <summary>
        /// Creates a category history.
        /// </summary>
        /// <param name="categoryHistoryModel">category History model.</param>
        /// <returns>New category history model.</returns>
        CategoryHistoryModel CreateCategoryHistoryModel(CategoryHistoryModel categoryHistoryModel);

        /// <summary>
        /// Updates a category history Model.
        /// </summary>
        /// <param name="categoryHistoryModel">Category history model with updated values.</param>
        /// <returns>Updated category history model.</returns>
        CategoryHistoryModel UpdateCategoryHistoryModel(CategoryHistoryModel categoryHistoryModel);

        /// <summary>
        /// Deletes a category history.
        /// </summary>
        /// <param name="id">ID of the category history.</param>
        /// <returns>True if category history is deleted; False if category history is not deleted.</returns>
        bool DeleteCategoryHistoryModel(int id);
    }
}
