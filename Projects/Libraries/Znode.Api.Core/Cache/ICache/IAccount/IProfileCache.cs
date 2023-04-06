namespace Znode.Engine.Api.Cache
{
    public interface IProfileCache
    {
        #region Profile 
        /// <summary>
        /// Get profile list from database.
        /// </summary>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <returns>Returns profile string</returns>
        string GetProfiles(string routeUri, string routeTemplate);

        /// <summary>
        /// Get profile details.
        /// </summary>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <param name="profileId"></param>
        /// <returns>Returns profile data</returns>
        string GetProfile(int profileId, string routeUri, string routeTemplate);
        #endregion

        /// <summary>
        /// Get profile catalog list from database.
        /// </summary>
        /// <param name="routeUri"></param>
        /// <param name="routeTemplate"></param>
        /// <returns>Returns profile catalog string</returns>
        string GetProfileCatalogs(string routeUri, string routeTemplate);
    }
}