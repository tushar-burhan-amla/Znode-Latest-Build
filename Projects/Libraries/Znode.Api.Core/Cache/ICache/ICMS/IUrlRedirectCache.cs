namespace Znode.Engine.Api.Cache
{
    public interface IUrlRedirectCache
    {
        /// <summary>
        /// Get Url Redirect list.
        /// </summary>
        /// <param name="routeUri">Route URI</param>
        /// <param name="routeTemplate">Route Template</param>
        /// <returns>Returns Url Redirect list.</returns>
        string GetUrlRedirectlist(string routeUri, string routeTemplate);

        /// <summary>
        /// Get Url Redirect.
        /// </summary>
        /// <param name="routeUri">Route URI</param>
        /// <param name="routeTemplate">Route Template</param>
        /// <returns>Returns Url Redirect.</returns>
        string GetUrlRedirect(string routeUri, string routeTemplate);
    }
}
