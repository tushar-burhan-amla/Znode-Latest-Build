namespace Znode.Engine.Api.Cache
{
    public interface IPortalProfileCache
    {
        /// <summary>
        /// Get Portal profiles.
        /// </summary>
        /// <param name="routeUri">string routeUri</param>
        /// <param name="routeTemplate">string routeTemplate</param>
        /// <returns>string data.</returns>
        string GetPortalProfiles(string routeUri, string routeTemplate);

        /// <summary>
        /// Get portal profile
        /// </summary>
        /// <param name="portalProfileId">Portal profile id.</param>
        /// <param name="routeUri">string routeUri</param>
        /// <param name="routeTemplate">string routeTemplate</param>
        /// <returns>string data.</returns>
        string GetPortalProfile(int portalProfileId, string routeUri, string routeTemplate);

    }
}
