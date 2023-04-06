using System.Collections.Generic;
using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IMediaManagerServices
    {
        /// <summary>
        /// Get Medias list.
        /// </summary>
        /// <param name="expands"></param>
        /// <param name="filters">Filter value.</param>
        /// <param name="sorts">Sort.</param>
        /// <param name="page">Page.</param>
        /// <returns>Returns MediaManagerListModel.</returns>
        MediaManagerListModel GetMedias(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// This Method is used for saving media metadata to database.
        /// </summary>
        /// <param name="mediaManagerModel">Media Manager Model.</param>
        /// <returns>Returns MediaManagerModel.</returns>
        MediaManagerModel SaveMedia(MediaManagerModel mediaManagerModel);

        /// <summary>
        /// This method is for getting folder structure.
        /// </summary>      
        /// <returns>Retruns MediaManangerTreeModel.</returns>
        MediaManagerTreeModel GetTreeNode();

        /// <summary>
        /// This method add folder of media to database.
        /// </summary>
        /// <param name="model">Add MediaManagerFolderModel.</param>
        /// <returns>Return true/false when added successfully.</returns>
        bool AddMedia(MediaManagerFolderModel model);

        /// <summary>
        /// This method is used to rename folder Name.
        /// </summary>
        /// <param name="model">MediaManagerFolderModel model.</param>
        /// <returns>Returns true if media renamed successfully.</returns>
        bool RenameMedia(MediaManagerFolderModel model);

        /// <summary>
        /// This method is used to get a media metadata according to media Id.
        /// </summary>
        /// <param name="mediaId">ID of media to get metadata.</param>
        /// <param name="expands">collection of expands.</param>
        /// <returns>Returns Media Manager Model.</returns>
        MediaManagerModel GetMediaByID(int mediaId, NameValueCollection expands);

        /// <summary>
        /// Gets the attributes for the specified media.
        /// </summary>
        /// <param name="mediaId">ID of the media.</param>
        /// <param name="expands">Collection of expands.</param>
        /// <returns>Returns List of attributes for the media.</returns>
        MediaAttributeValuesListModel GetMediaAttributeValues(int mediaId, NameValueCollection expands);


        /// <summary>
        /// This Method is used to move folder to another folder.
        /// </summary>
        /// <param name="model">MediaManagerMoveFolderModel model.</param>
        /// <returns>Return true/false when added successfully.</returns>
        bool MoveFolder(MediaManagerMoveFolderModel model);

        /// <summary>
        /// This method is used to move media to specific folder.
        /// </summary>
        /// <param name="model">AddMediaToFolderModel model.</param>
        /// <returns>Return true/false when added successfully.</returns>
        bool MoveMediaToFolder(AddMediaToFolderModel model);

        /// <summary>
        /// This method is used to delete media from database.
        /// </summary>
        /// <param name="deleteMediaModel">Media to be deleted</param>
        /// <returns>String of deleted media path comma seprated.</returns>
        string DeleteMedia(DeleteMediaModel deleteMediaModel);

        /// <summary>
        /// Updates media attribute value.
        /// </summary>
        ///<param name="model">List of Media attribute values model.</param>
        /// <returns>Boolean value according to the status of update.</returns>
        bool UpdateMediaAttributeValue(MediaAttributeValuesListModel model);

        /// <summary>
        /// This method is used to get attribute family id by extension.
        /// </summary>
        /// <param name="extension">File extension.</param>
        /// <returns>Returns Attribute family.</returns>
        MediaAttributeFamily GetAttributeFamilyIdByName(string extension);

        /// <summary>
        /// Creates a media attribute value.
        /// </summary>
        /// <param name="model">Media attribute values model to be created.</param>
        /// <returns>Created media attribute model.</returns>
        MediaAttributeValuesModel CreateMediaAttribute(MediaAttributeValuesModel model);

        /// <summary>
        /// Update the media manager model.
        /// </summary>
        /// <param name="mediaManagerModel">MediaManagerModel.</param>
        /// <returns>Returns the updated model.</returns>
        MediaManagerModel Update(MediaManagerModel mediaManagerModel);

        /// <summary>
        /// Share Media/Folder with different users.
        /// </summary>
        /// <param name="shareMediaFolderModel">Share media folder list model.</param>
        /// <returns>Return true/false status for share or not.</returns>
        bool ShareFolder(List<ShareMediaFolderModel> shareMediaFolderModel);

        /// <summary>
        /// Get allowed extensions.
        /// </summary>
        /// <returns>Returns list of family with validation.</returns>
        FamilyExtensionListModel GetAllowedExtensions();

        /// <summary>
        /// Get if file already present or not.
        /// </summary>
        /// <param name="fileName">File name.</param>
        /// <param name="folderId">Folder id.</param>
        /// <returns>Returns media id.</returns>
        int FileAlreadyExist(string fileName, int folderId);

        /// <summary>
        /// Get list of store associated to mediapath
        /// </summary>
        /// <param name="mediaPaths">File path.</param>
        /// <returns>Returns associated store list.</returns>
        List<int> GetAssociatedStoreOfMedia(string mediaPaths);

        /// <summary>
        /// Generate specific image on edit.
        /// </summary>
        /// <param name="mediaPath">MediaPath of media</param>
        /// <returns>Returns boolean value on generate specific image.</returns>
        bool GenerateImageOnEdit(string mediaPath);

        /// <summary>
        ///  Get MediaDetail Model is a light weighted model which returns basic properties
        /// </summary>
        /// <param name="mediaId">MediaId</param>
        /// <returns>Returns MediaDetailModel </returns>
        MediaDetailModel GetMediaDetailsById(int mediaId);

        /// <summary>
        /// Get MediaDetail Model is a light weighted model which returns basic properties
        /// </summary>
        /// <param name="mediaGuid">MediaGuid</param>
        /// <returns>Returns MediaDetailModel </returns>
        MediaDetailModel GetMediaDetailsByGuid(string mediaGuid);
    }
}