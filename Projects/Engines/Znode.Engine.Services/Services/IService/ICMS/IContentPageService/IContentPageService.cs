using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IContentPageService
    {
        #region Content Page
        /// <summary>
        /// Create content page.
        /// </summary>
        /// <param name="model">StaticPageModel.</param>
        /// <returns>Returns created static page model.</returns>
        ContentPageModel CreateContentPage(ContentPageModel model);

        /// <summary>
        /// Get content page list. 
        /// </summary>
        /// <param name="filters">Filters for static page.</param>
        /// <param name="sorts">Sorts for static page.</param>
        /// <param name="page">Page.</param>
        /// <returns>Returns StaticPageListModel.</returns>
        ContentPageListModel GetContentPageList(FilterCollection filters, NameValueCollection expands, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get static page data by static page id.
        /// </summary>
        /// <param name="filters"></param>
        /// <returns>Returns content page data</returns>
        ContentPageModel GetContentPage(FilterCollection filters);

        /// <summary>
        /// Update Static page.
        /// </summary>
        /// <param name="staticPageModel">StaticPageModel.</param>
        /// <returns>Returns true if static page updated successfully else return false.</returns>
        bool UpdateContentPage(ContentPageModel contentPageModel);

        /// <summary>
        /// Delete Static Page.
        /// </summary>
        /// <param name="staticPageIds">Parameter model containing static page ids to be deleted.</param>
        /// <returns>Returns true if static page deleted successfully else return false.</returns>
        bool DeleteContentPage(ParameterModel contentPageIds);    

        /// <summary>
        /// Publish the content page.
        /// </summary>
        /// <param name="contentPageId"></param>
        /// <param name="portalId"></param>
        /// <param name="targetPublishState"></param>
        /// <param name="takeFromDraftFirst"></param>
        /// <returns></returns>
        PublishedModel PublishContentPage(int contentPageId, int portalId, int localeId = 0, string targetPublishState = null, bool takeFromDraftFirst = false);

        /// <summary>
        /// Get list of content pages by portal id.
        /// </summary>
        /// <param name="filters">Filter for content pages.</param>
        /// <returns>List of content pages.</returns>
        WebStoreContentPageListModel GetContentPagesList(FilterCollection filters);
       
        #endregion

        #region Content Page Tree
        /// <summary>
        /// Gets content page tree.
        /// </summary>
        /// <returns>Returns content page tree.</returns>
        ContentPageTreeModel GetTreeNode();

        /// <summary>
        /// Rename the existing folder.
        /// </summary>
        /// <param name="model">Folder to rename.</param>
        /// <returns>Returns true if renamed successfully else false.</returns>
        bool RenameFolder(ContentPageFolderModel model);

        /// <summary>
        /// Add new folder.
        /// </summary>
        /// <param name="model">Folder to add.</param>
        /// <returns>Returns true if added successfully else false.</returns>
        bool AddFolder(ContentPageFolderModel model);

        /// <summary>
        /// Delete the existing folder.
        /// </summary>
        /// <param name="model">Ids to delete.</param>
        /// <returns>Returns true if deleted successfully else false.</returns>
        bool DeleteFolder(ParameterModel model);

        /// <summary>
        /// Move one folder to other folder within content page.
        /// </summary>
        /// <param name="model">ContentPageFolderModel</param>
        /// <returns>Returns true if moved successfully else false.</returns>
        bool MoveContentPagesFolder(ContentPageFolderModel model);

        /// <summary>
        /// Move content page from one folder to other folder.
        /// </summary>
        /// <param name="model">AddPageToFolderModel model.</param>
        /// <returns>Returns true if pages moved successfully to other folder else return false.</returns>
        bool MovePageToFolder(AddPageToFolderModel model);
        #endregion
    }
}
