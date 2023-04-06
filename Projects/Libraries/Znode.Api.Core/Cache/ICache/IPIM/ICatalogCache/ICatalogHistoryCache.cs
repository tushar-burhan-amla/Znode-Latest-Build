namespace Znode.Engine.Api.Cache
{
    public interface ICatalogHistoryCache
    {
        /// <summary>
        /// Gets catalog history list.
        /// </summary>
        /// <param name="routeUri">Route Uri.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns>Catalog history list.</returns>
        string GetCatalogHistoryList(string routeUri, string routeTemplate);

        /// <summary>
        /// Gets catalog history by specified ID.
        /// </summary>
        /// <param name="id">ID of catalog history.</param>
        /// <param name="routeUri">Route Uri</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns>Catalog history of the specified ID.</returns>
        string GetCatalogHistory(int id, string routeUri, string routeTemplate);
    }
}
