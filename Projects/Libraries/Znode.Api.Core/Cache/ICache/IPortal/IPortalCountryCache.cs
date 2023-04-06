namespace Znode.Engine.Api.Cache
{
    public interface IPortalCountryCache
    {
        #region Country Association
        /// <summary>
        /// Get list of unassociate countries. 
        /// </summary>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns></returns>
        string GetUnAssociatedCountryList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get list of associated countries based on portal.
        /// </summary>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns></returns>
        string GetAssociatedCountryList(string routeUri, string routeTemplate);
        #endregion
    }
}
