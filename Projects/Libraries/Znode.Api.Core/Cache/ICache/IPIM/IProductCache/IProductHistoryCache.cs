namespace Znode.Engine.Api.Cache
{
    // Product History Cache Interface.
    public interface IProductHistoryCache
    {
        /// <summary>
        /// Get Product History list.
        /// </summary>
        /// <param name="routeUri">route uri</param>
        /// <param name="routeTemplate">route template</param>
        /// <returns>Returns list of Product History.</returns>
        string GetProductHistoryList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get Product History by Product History Id.
        /// </summary>
        /// <param name="id">Product History Id.</param>
        /// <param name="routeUri">route uri</param>
        /// <param name="routeTemplate">route template</param>
        /// <returns>Returns Product History on the basis of Product History Id.</returns>
        string GetProductHistory(int id, string routeUri, string routeTemplate);
    }
}

