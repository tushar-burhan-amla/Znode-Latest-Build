using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Admin.Models;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Admin.Agents
{
   public interface IMediaManagerAgent
    {
        /// <summary>
        /// This method is used to get all medias.
        /// </summary>
        /// <param name="model">FilterCollectionDataModel.</param>
        /// <param name="folderId">Folder Id.</param>
        /// <returns>Returns MediaManagerListViewModel.</returns>
        MediaManagerListViewModel GetMedias(FilterCollectionDataModel model, int? folderId = null);

        /// <summary>
        /// This used to get all media files
        /// </summary>
        /// <param name="filters">FilterCollection.</param>
        /// <param name="sortCollection">SortCollection.</param>
        /// <param name="pageIndex">PageIndex.</param>
        /// <param name="recordPerPage">RecordPerPage.</param>
        /// <returns>Returns MediaManagerListModel.</returns>
        MediaManagerListViewModel GetMedias(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null, int? folderId = null);

        /// <summary>
        /// This method is used for getting tree folders.
        /// </summary>
        /// <returns>retruns folder structure</returns>
        string GetTree();

        /// <summary>
        /// This method is used to get a media metadata according to media Id.
        /// </summary>
        /// <param name="mediaId">ID of media to get metadata</param>
        /// <returns>Returns Media Manager Model</returns>
        MediaManagerViewModel GetMedia(int mediaId);

        /// <summary>
        /// This method is used for add new folder to database.
        /// </summary>
        /// <param name="parentId">Add child folder to parent.</param>
        /// <param name="folderName">Name of folder</param>
        /// <param name="message">Set message if error occured while renaming folder.</param>
        /// <returns>Returns true/false if succeeded.</returns>
        MediaManagerFolderViewModel AddFolder(int parentId, string folderName, out string message);

        /// <summary>
        /// This method is used to rename folder name to database
        /// </summary>
        /// <param name="folderId">Folder tobe renamed.</param>
        /// <param name="folderName">New folder name.</param>
        /// <param name="message">Set message if error occured while renaming folder.</param>
        /// <returns>returns true/false if succeeded.</returns>
        bool RenameFolder(int folderId, string folderName, out string message);


        /// <summary>
        /// Gets the attributes for the specified media.
        /// </summary>
        /// <param name="mediaId">ID of the media.</param>
        /// <returns>Returns List of attributes for the media.</returns>
        MediaAttributeValuesListViewModel GetMediaAttributeValues(int mediaId);

        /// <summary>
        /// This Method is used to add folder to anther folder.
        /// </summary>
        /// <param name="folderId">Folder to be moved.</param>
        /// <param name="addtoFolderId">Fllder where to move.</param>
        /// <param name="message">Set message if error occured while renaming folder.</param>
        /// <returns>Returns true if succeeded.</returns>
        bool MoveFolder(int folderId, int addtoFolderId, out string message);

        /// <summary>
        /// This method is used to copy media to specified folder
        /// </summary>
        /// <param name="folderId">FolderId.</param>
        /// <param name="MediaIds">Media Ids to be copy.</param>
        /// <param name="message">Set message if error occurred while renaming folder.</param>
        /// <returns>Returns true if media moved to folder successfully.</returns>
        bool MoveMediaToFolder(int folderId, string mediaIds, out string message);

        /// <summary>
        /// This method is used to delete media with specific ids.
        /// </summary>
        /// <param name="mediaIds">Media Ids to be delete</param>
        /// <param name="folderId">Folder id.</param>
        /// <param name="message">Set message if error occured while deleting media.</param>
        /// <returns>Returns status if medias are deleted or not.</returns>
        bool DeleteMedia(string mediaIds, out string message);

        /// <summary>
        /// This method is used to delete folder with specific id.
        /// </summary>
        /// <param name="folderId">Folder Id to be delete.</param>
        /// <param name="message">Set message if error occured while deleting folder.</param>
        /// <returns>Returns status if folder is deleted or not.</returns>
        bool DeleteFolder(int folderId, out string message);

        /// <summary>
        /// Updates list of media attributeValues.
        /// </summary>
        /// <param name="viewModelList">Bind data model from the view.</param>
        /// <param name="errorMessage">Error message.</param>
        /// <returns>Returns boolean value on updating media attribute values.</returns>
        bool UpdateMediaAttributeValueList(BindDataModel viewModelList, MediaManagerViewModel MetaInfo, out string errorMessage);

        /// <summary>
        /// Checks for file name present in Znode media table.
        /// </summary>
        /// <param name="fileName">File name to check.</param>
        /// <param name="folderId">folder id of media to check.</param>
        /// <returns>Returns path of the file if file exists.</returns>
        string IsFileNamePresent(string fileName, int folderId);

        /// <summary>
        /// Share Media/Folder with users.
        /// </summary>
        /// <param name="folderId">ID of folder which is going to share.</param>
        /// <param name="accountIds">IDs of account with which folder is going to share.</param>
        /// <param name="message">error message if error occured during media/folder share.</param>
        /// <returns>Returns true/false status if shared or not</returns>
        bool ShareMediaFolder(int folderId, string accountIds, out string message);

        /// <summary>
        /// This method set the media manager view model properties
        /// </summary>
        void SetMediaManagerViewProperties(MediaManagerViewModel viewModel);

        /// <summary>
        /// Gets the list of view modes.
        /// </summary>
        /// <param name="selectedViewMode">selected view mode.</param>
        /// <returns>List of View modes</returns>
        List<SelectListItem> GetViewModes(string selectedViewMode);

        /// <summary>
        /// Method to create a data set that contains column name exactly as in grid.
        /// </summary>
        /// <param name="dataModel">List<dynamic> dataModel</param>
        /// <returns>Returns required data set.</returns>
        List<dynamic> CreateDataSource(List<dynamic> dataModel);

        /// <summary>
        /// Get allowed extensions.
        /// </summary>
        /// <returns>Returns list of family with validation.</returns>
        FamilyExtensionListModel GetAllowedExtensions();

        /// <summary>
        /// Get media manager view model with properties set to its value.
        /// </summary>
        /// <param name="model">Filter collection to set values.</param>
        /// <param name="popupViewModel">Popup view model to set the data.</param>
        /// <returns>Returns the media manager view model.</returns>

        /// <summary>
        ///  Get media manager view model with properties set to its value.
        /// </summary>
        /// <param name="model">Filter collection to set values.</param>
        /// <param name="popupViewModel">Popup view model to set the data.</param>
        /// <param name="displayMode">display mode for media grid</param>
        /// <param name="folderId">folder id</param>
        /// <returns>Returns the media manager view model.</returns>
        MediaManagerViewModel GetMediaManagerViewModel(FilterCollectionDataModel model, PopupViewModel popupViewModel, string displayMode, int folderId);

        /// <summary>
        /// Set media manager list with required properties.
        /// </summary>
        /// <param name="model">Filter Collection Data Model.</param>
        /// <param name="popupViewModel">Popup View Model.</param>
        /// <param name="viewModel">Media Manager View Model.</param>
        /// <param name="mediaManager">Media Manager List View Model.</param>
        /// <returns>Returns media manager view model.</returns>
        MediaManagerViewModel MediaManagerList(FilterCollectionDataModel model, PopupViewModel popupViewModel, MediaManagerViewModel viewModel, MediaManagerListViewModel mediaManager);

        /// <summary>
        /// Get application header
        /// </summary>
        /// <returns></returns>
        AjaxHeadersModel GetAppHeader();

        /// <summary>
        /// Generate specific image on edit.
        /// </summary>
        /// <param name="mediaPath">MediaPath of media</param>
        /// <returns>Returns boolean value on generate specific image.</returns>
        bool GenerateImageOnEdit(string mediaPath);
    }
}
