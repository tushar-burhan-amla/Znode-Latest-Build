namespace Znode.Engine.Api.Cache
{
    public interface IContentPageCache
    {
        #region Content Page
        /// <summary>
        /// Get Content page list. 
        /// </summary>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns>String Data.</returns>
        string GetContentPageList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get Content page data by Content page id.
        /// </summary>       
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns>String Data.</returns>
        string GetContentPage(string routeUri, string routeTemplate);

        #region Tree
        /// <summary>
        /// Gets the content page tree.
        /// </summary>
        /// <param name="routeUri">Route uri.</param>
        /// <param name="routeTemplate">Route Template.</param>
        /// <returns>Returns tree.</returns>
        string GetTree(string routeUri, string routeTemplate);

        /// <summary>
        /// Get Content page list for portal. 
        /// </summary>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns>String Data.</returns>
        string GetContentPagesList(string routeUri, string routeTemplate);
        #endregion
        #endregion
    }
}
