using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Cache
{
    public interface IMediaManagerCache
    {
        #region Public method

        /// <summary>
        /// This method is used to get a media metadata according to media Id.
        /// </summary>
        /// <param name="mediaId">ID of media to get metadata.</param>
        /// <param name="routeUri">Url of api route.</param>
        /// <param name="routeTemplate">Template of route.</param>
        /// <returns>Returns Media Manager Model.</returns>
        string GetMediaID(int mediaId, string routeUri, string routeTemplate);

        /// <summary>
        /// This method is used to get all media or filter medias by folder id if not present present return all medias.
        /// </summary>
        /// <param name="routeUri">Url of api route.</param>
        /// <param name="routeTemplate">Template of route.</param>
        /// <returns>Returns json string with all medias data string.</returns>
        string GetMedias(string routeUri, string routeTemplate);

        /// <summary>
        ///  This method is used to get all media or filter medias by folder id if not present present return all medias.
        /// </summary>
        /// <param name="paramFilter">FilterCollection.</param>
        /// <param name="routeUri">Url of api route.</param>
        /// <param name="routeTemplate">Template of route.</param>
        /// <returns>Returns json string with all medias data string.</returns>
        string GetMedias(FilterCollection paramFilter, string routeUri, string routeTemplate);

        /// <summary>
        /// Gets the attributes for the specified media.
        /// </summary>
        /// <param name="routeUri">Url of api route.</param>
        /// <param name="routeTemplate">Template of route.</param>
        /// <param name="mediaId">ID of the media.</param>
        /// <returns>Returns List of attributes for the media.</returns>
        string GetMediaAttributeValues(string routeUri, string routeTemplate, int mediaId);

        /// <summary>
        /// This method is used to get a media attribute family id by extension.
        /// </summary>
        /// <param name="routeUri">Url of api route.</param>
        /// <param name="routeTemplate">Template of route.</param>
        /// <param name="extension">extension of file to get family Id.</param>
        /// <returns>Returns attribute Id.</returns>
        string GetAttributeFamilyIdByName(string routeUri, string routeTemplate, string extension);

        /// <summary>
        /// Get Tree Nodes.
        /// </summary>
        /// <param name="routeUri">Url of api route.</param>
        /// <param name="routeTemplate">Template of route.</param>
        /// <returns>Returns tree.</returns>
        string GetTree(string routeUri, string routeTemplate);

        /// <summary>
        /// Get allowed extensions.
        /// </summary>
        /// <param name="routeUri">Url of api route.</param>
        /// <param name="routeTemplate">Template of route.</param>
        /// <returns>Returns allowed extensions.</returns>
        string GetAllowedExtensions(string routeUri, string routeTemplate);

        /// <summary>
        /// Get Media Details by MediaID
        /// </summary>
        /// <param name="mediaId">MediaId</param>
        /// <param name="routeUri">Url of api route</param>
        /// <param name="routeTemplate">Template of route</param>
        /// <returns>Media Detail By Media Id</returns>
        string GetMediaDetailsById(int mediaId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get Media Details by GUID
        /// </summary>
        /// <param name="mediaGuid">guid </param>
        /// <param name="routeUri">Url of api route</param>
        /// <param name="routeTemplate">Template of route</param>
        /// <returns>Media Detail By Media Guid</returns>
        string GetMediaDetailsByGuid(string mediaGuid, string routeUri, string routeTemplate);
        #endregion
    }
}
