using System.Collections.Generic;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IMediaManagerClient : IBaseClient
    {
        /// <summary>
        /// This helps us to metadata files to database
        /// </summary>
        /// <param name="mediaManagerModel">Media Manager model to save.</param>
        /// <returns>Returns MediaManagerModel</returns>
        MediaManagerModel SaveMediaFile(MediaManagerModel mediaManagerModel);

        /// <summary>
        /// This method is used for updating media by media name.
        /// </summary>
        /// <param name="mediaMangerModel">Model to update in database.</param>
        /// <returns>Returns updated model.</returns>
        MediaManagerModel UpdateMedia(MediaManagerModel mediaMangerModel);

        /// <summary>
        /// This method is used for getting all media metadata
        /// </summary>
        /// <param name="filters">Filter data with help of filters.</param>
        /// <param name="sortCollection">Sort data with help of filters sortCollection.</param>
        /// <param name="pageIndex">Page number to see on page pageIndex.</param>
        /// <param name="recordPerPage">RecordPerPage.</param>
        /// <returns>Returns MediaManagerListModel.</returns>
        MediaManagerListModel GetMedias(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// This Method is used for get folder structure.
        /// </summary>
        /// <returns>Returns MediaManagerTreeModel.</returns>
        MediaManagerTreeModel GetTree();

        /// <summary>
        /// Add new folder to database in with under provided parent id.
        /// </summary>
        /// <param name="folderModel">MediaManagerFolderModel model.</param>
        /// <returns>Return MediaManagerFolderModel.</returns>
        MediaManagerFolderModel AddFolder(MediaManagerFolderModel folderModel);

        /// <summary>
        /// This method is used to get a media metadata according to media Id.
        /// </summary>
        /// <param name="mediaId">ID of media to get metadata.</param>
        /// <returns>Returns Media Manager Model.</returns>
        MediaManagerModel GetMedia(int mediaId);

        /// <summary>
        /// This method is used to rename folder name.
        /// </summary>
        /// <param name="folderModel">MediaManagerFolderModel model.</param>
        /// <returns>Returns true if folder is renamed else returns false.</returns>
        bool RenameFolder(MediaManagerFolderModel folderModel);

        /// <summary>
        /// This Method is used to add folder to anther folder.
        /// </summary>
        /// <param name="mediaModel">Media Manager Moved to Folder.</param>
        /// <returns>Returns true if folder is moved else returns false.</returns>
        bool MoveFolder(MediaManagerMoveFolderModel mediaModel);

        /// <summary>
        /// This method is used to copy media to specified folder.
        /// </summary>
        /// <param name="mediaModel">Media added to folder.</param>
        /// <returns>Returns true if folder is coppied else returns false.</returns>
        bool MoveMediaToFolder(AddMediaToFolderModel mediaModel);

        /// <summary>
        /// Gets the attributes for the specified media.
        /// </summary>
        /// <param name="mediaId">ID of the media.</param>
        /// <param name="expands">collection of expands.</param>
        /// <returns>Returns List of attributes for the media.</returns>
        MediaAttributeValuesListModel GetMediaAttributeValues(int mediaId, ExpandCollection expands);

        /// <summary>
        /// This method is used to delete medias.
        /// </summary>
        /// <param name="deleteMediaModel">Media to be deleted.</param>
        /// <returns>Returns true if folder is deleted else returns false.</returns>
        string DeleteMedia(DeleteMediaModel deleteMediaModel);

        /// <summary>
        /// Updates media attribute value.
        /// </summary>
        ///<param name="model">List of Media Attribute values model.</param>
        /// <returns>Returns boolean value according to the status of update.</returns>
        bool UpdateMediaAttributeValue(MediaAttributeValuesListModel model);

        /// <summary>
        /// This method is used to get attribute family id by file extension.
        /// </summary>
        /// <param name="extension">String extension.</param>
        /// <returns>Returns MediaAttributeFamily.</returns>
        MediaAttributeFamily GetAttributeFamilyIdByName(string extension);

        /// <summary>
        /// Creates a media attribute value.
        /// </summary>
        /// <param name="model">Media attribute value.</param>
        /// <returns>Returns newly created media attribute value.</returns>
        MediaAttributeValuesModel CreateMediaAttributeValue(MediaAttributeValuesModel model);

        /// <summary>
        /// Share Media/Folder with users.
        /// </summary>
        /// <param name="ShareMediaFoldermodel">List model of ShareMediaFolderModel.</param>
        /// <returns>Returns true/false status if shared or not.</returns>
        bool ShareMediaFolder(List<ShareMediaFolderModel> ShareMediaFolderListModel);

        /// <summary>
        /// Get allowed extensions.
        /// </summary>
        /// <returns>Returns list of family with validation.</returns>
        FamilyExtensionListModel GetAllowedExtensions();

        /// <summary>
        /// Generate specific image on edit.
        /// </summary>
        /// <param name="mediaPath">MediaPath of media</param>
        /// <returns>Returns boolean value on generate specific image.</returns>
        bool GenerateImageOnEdit(string mediaPath);

        /// <summary>
        ///  Get MediaDetail Model is a light weighted model which returns basic properties
        /// </summary>
        /// <param name="mediaId">Media DetailModel </param>
        /// <returns>Returns Media Details</returns>
        MediaDetailModel GetMediaDetailsById(int mediaId);

        /// <summary>
        /// Get MediaDetail Model is a light weighted model which returns basic properties
        /// </summary>
        /// <param name="mediaGuid">Media detail Model</param>
        /// <returns>Returns Media Details </returns>
        MediaDetailModel GetMediaDetailsByGuid(string mediaGuid);
    }
}
