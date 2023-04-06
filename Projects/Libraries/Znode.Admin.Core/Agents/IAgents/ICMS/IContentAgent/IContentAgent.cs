using System;
using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Sorts;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Agents
{
    public interface IContentAgent
    {
        #region Content Page

        /// <summary>
        /// Get Content page list. 
        /// </summary>
        /// <param name="filters">Filters for Content page.</param>
        /// <param name="sorts">Sorts for Content page.</param>
        /// <param name="pageIndex">Page index.</param>
        /// <param name="pageSize">Size of page.</param>
        /// <returns>Returns list of Content pages.</returns>
        ContentPageListViewModel GetContentPageList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Create Content page.
        /// </summary>
        /// <param name="contentPageViewModel">ContentPageViewModel.</param>
        /// <returns>Returns ContentPageViewModel.</returns>
        ContentPageViewModel CreateContentPage(ContentPageViewModel contentPageViewModel);

        /// <summary>
        /// Get Content page data by Content page id.
        /// </summary>
        /// <param name="contentPageId">Content page id.</param>
        /// <param name="localeId">Locale Id.</param>
        /// <returns>Returns ContentPageModel.</returns>
        ContentPageViewModel GetContentPage(int contentPageId, int localeId);

        /// <summary>
        /// Update Content page for both WebSite Content Page and Content Content Page.
        /// </summary>
        /// <param name="contentPageViewModel">ContentPageViewModel.</param>
        /// <returns>Returns updated ContentPageViewModel.</returns>
        ContentPageViewModel UpdateContentPage(ContentPageViewModel contentPageViewModel);

        /// <summary>
        /// Delete Content Page.
        /// </summary>
        /// <param name="contentPageId">Content page ids to be deleted.</param>
        /// <param name="errorMessage">Error message.</param>
        /// <returns>Returns true if Content page deleted successfully else return false.</returns>
        bool DeleteContentPage(string contentPageId, out string errorMessage);

        /// <summary>
        /// Publish the Content Page.
        /// </summary>
        /// <param name="contentPageId"></param>
        /// <param name="errorMessage"></param>
        /// <returns>Returns the model with result true if published successfully else return model with false and error message.</returns>
        [Obsolete("To be discontinued in upcoming versions.")]
        bool PublishContentPage(string contentPageId, int localeId, out string errorMessage);

        /// <summary>
        /// Publish the Content Page.
        /// </summary>
        /// <param name="contentPageId"></param>
        /// <param name="localeId"></param>
        /// <param name="errorMessage"></param>
        /// <param name="targetPublishState"></param>
        /// <param name="takeFromDraftFirst"></param>
        /// <returns>Returns the model with result true if published successfully else return model with false and error message.</returns>
        bool PublishContentPage(string contentPageId, out string errorMessage, int localeId = 0, string targetPublishState = null, bool takeFromDraftFirst = false);
        
        /// <summary>
        /// Get the list of profile on the basis of portalId.
        /// </summary>
        /// <param name="portalId">Id of portal</param>
        /// <returns>list of profile in select list item format.</returns>
        List<SelectListItem> GetProfileList(int portalId);

        /// <summary>
        /// Get the information required to create new Content Page.
        /// </summary>
        /// <param name="contentPageViewModel">ContentPageViewModel.</param>
        /// <returns>ContentPageViewModel</returns>
        [Obsolete]
        ContentPageViewModel GetContentPageInformation(ContentPageViewModel contentPageViewModel);

        /// <summary>
        /// Get the information required to create new Content Page with image template.
        /// </summary>
        /// <param name="contentPageViewModel">ContentPageViewModel.</param>
        /// <returns>ContentPageViewModel</returns>
        ContentPageViewModel GetContentPageDetail(ContentPageViewModel contentPageViewModel);


        /// <summary>
        /// Get the list of all widgets contents in Template.
        /// </summary>
        /// <param name="cmsMappingId">Id of content page.</param>
        /// <param name="typeOfMapping">Type of mapping.</param>
        /// <param name="templatePath">Path of template to read widgets.</param>
        /// <param name="displayName">Display name of widgets.</param>
        /// <param name="fileName">File name of widgets.</param>
        /// <returns>CMSWidgetsListViewModel</returns>
        CMSWidgetsListViewModel GetTemplateWidgets(int cmsMappingId, string typeOfMapping, string templatePath, string displayName, string fileName);

        /// <summary>
        /// Check Whether the seo name is already exists.
        /// </summary>
        /// <param name="seoName">seo name</param>
        /// <param name="cmsContentPagesId">id for the Content Page</param>
        /// <param name="PortalId">PortalId</param>
        /// <returns>return the status in true or false</returns>
        bool CheckSeoNameExist(string seoName, int cmsContentPagesId, int PortalId);

        /// <summary>
        /// Check Whether the Content Page Name already exists.
        /// </summary>
        /// <param name="contentPageName">contentPageName</param>
        /// <returns>return the status in true or false</returns>
        [Obsolete("This method is deprecated and will be discontinued in upcoming versions." +
        " This method mark as obsolute due to portalId check was not present while checking page name exist." +
        " Please use overload of this method having contentPageName & portalId as a parameters")]
        bool CheckContentPageNameExist(string contentPageName);

        /// <summary>
        /// Check Whether the Content Page Name already exists for the particular portal.
        /// </summary>
        /// <param name="contentPageName">contentPageName</param>
        /// <param name="portalId">This parameter should contains portal id</param>
        /// <returns>return the status in true or false</returns>
        bool CheckContentPageNameExist(string contentPageName, int portalId);

        /// <summary>
        /// Get preview url depending on the condition
        ///  parameter isStorePublish is assign default value as this method called from  EditContentPage where isStorePublish parameter is not available
        /// </summary>
        /// <param name="portalId">portalId</param>
        /// <param name="isPreviewGloballyEnabled">If preview setting set in global setting</param>
        /// <param name="isStorePublish">isStorePublish</param>
        /// <returns>return the previewURL</returns>
        string GetPreviewURL(int portalId, bool isPreviewGloballyEnabled, bool isStorePublish = true);

        /// <summary>
        /// Publish the Saved TextWidgetConfiguration to Preview.
        /// </summary>
        /// <param name="IsGlobalPreviewEnabled"></param>
        /// <param name="CMSMappingId"></param>
        /// <param name="message"></param>
        /// <param name="localeId"></param>
        /// <returns>Returns the model with result true if published successfully else return model with false and error message.</returns>
        bool PublishTextWidgetConfigurationToPreview(bool IsGlobalPreviewEnabled, string CMSMappingId, int localeId, out string message);

        #endregion

        #region Content Page Tree
        /// <summary>
        /// Get the content page tree.
        /// </summary>
        /// <returns>Returns content page tree.</returns>
        string GetTree();

        /// <summary>
        /// Add new folder.
        /// </summary>
        /// <param name="parentId">Parent folder id.</param>
        /// <param name="folderName">Folder name.</param>
        /// <returns>Returns Content page group id.</returns>
        int AddFolder(int parentId, string folderName);

        /// <summary>
        /// Rename the existing folder.
        /// </summary>
        /// <param name="folderId">Folder id to rename.</param>
        /// <param name="folderName">New folder name.</param>
        /// <returns>Returns true if renamed successfully else false.</returns>
        bool RenameFolder(int folderId, string folderName);

        /// <summary>
        /// Delete the existing folder.
        /// </summary>
        /// <param name="folderId">Folder id to delete.</param>
        /// <returns>Returns true if deleted successfully else false.</returns>
        bool DeleteFolder(string folderId);

        /// <summary>
        /// Gets Content Page List View Model.
        /// </summary>
        /// <param name="model">Filters collection data model.</param>
        /// <param name="folderId">Folder id to set in tree.</param>
        /// <param name="isRootFolder">Is root folder.</param>
        /// <returns>Returns Content page list view model.</returns>
        ContentPageListViewModel GetContentPageListViewModel(FilterCollectionDataModel model, int folderId, bool isRootFolder);

        /// <summary>
        /// Get the select list of locales.
        /// </summary>
        /// <returns>Returns select list of locales.</returns>
        List<SelectListItem> GetLocalesList(int localeId = 0);

        /// <summary>
        /// Move content page folder to another folder.
        /// </summary>
        /// <param name="folderId">folder Id to move</param>
        /// <param name="addtoFolderId">folder Id to which move.</param>
        /// <param name="cmsContentPagesId">cmsContentPagesId.</param>
        /// <returns>Returns true if Move Content Pages Folder else false.</returns>
        bool MoveContentPagesFolder(int folderId, int addtoFolderId);

        /// <summary>
        /// Move content pages from one folder to another.
        /// </summary>
        /// <param name="folderId">Folder Id.</param>
        /// <param name="pageIds">Content page ids.</param>
        /// <param name="message">Error message.</param>
        /// <returns>Returns true if pages moved successfully to other folder else return false.</returns>
        bool MovePageToFolder(int folderId, string pageIds, out string message);

        #endregion
    }
}
