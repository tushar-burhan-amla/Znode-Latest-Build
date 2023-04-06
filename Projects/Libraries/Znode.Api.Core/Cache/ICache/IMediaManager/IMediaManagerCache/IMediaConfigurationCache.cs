namespace Znode.Engine.Api.Cache
{
    public interface IMediaConfigurationCache
    {
        /// <summary>
        /// Gets list of media server.
        /// </summary>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns>Returns list of media server.</returns>
        string GetMediaServers(string routeUri, string routeTemplate);

        /// <summary>
        /// Get media configuration.
        /// </summary>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns>Returns media configuration.</returns>
        string GetMediaConfiguration(string routeUri, string routeTemplate);

        /// <summary>
        /// Get default media configuration.
        /// </summary>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns>Returns default media configuration.</returns>
        string GetDefaultMediaConfiguration(string routeUri, string routeTemplate);

        /// <summary>
        /// Get media count.
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Total media count.</returns>
        string GetMediaCount(string routeUri, string routeTemplate);

        /// <summary>
        /// Get media list for generate images.
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Media list.</returns>
        string GetMediaListData(string routeUri, string routeTemplate);
    }
}
