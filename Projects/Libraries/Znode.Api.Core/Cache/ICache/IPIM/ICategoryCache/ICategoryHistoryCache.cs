namespace Znode.Engine.Api.Cache
{
    public interface ICategoryHistoryCache
    {
        /// <summary>
        /// Gets the list of category history.
        /// </summary>
        /// <param name="routeUri">Route Uri.</param>
        /// <param name="routeTemplate">Rout template.</param>
        /// <returns>Gets the list of category history.</returns>
        string GetCategoryHistoryList(string routeUri, string routeTemplate);

        /// <summary>
        /// Gets the category history by specified ID.
        /// </summary>
        /// <param name="id">ID of the category history.</param>
        /// <param name="routeUri">Route Uri.</param>
        /// <param name="routeTemplate">Route Template.</param>
        /// <returns>Gets the category history of the specified ID.</returns>
        string GetCategoryHistory(int id, string routeUri, string routeTemplate);
    }
}
