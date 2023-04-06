namespace Znode.Engine.Api.Cache
{
    public interface ICMSWidgetsCache
    {
        /// <summary>
        /// Get CMS Widgets list.
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns list of CMS Widgets.</returns>
        string List(string routeUri, string routeTemplate);
    }
}
