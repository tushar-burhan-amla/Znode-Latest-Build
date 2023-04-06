using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IContentPageClient : IBaseClient
    {
        #region Content Page
        /// <summary>
        /// Create Content page.
        /// </summary>
        /// <param name="model">ContentPageModel.</param>
        /// <returns>Returns created Content page model.</returns>
        ContentPageModel CreateContentPage(ContentPageModel model);

        /// <summary>
        /// Get Content page list. 
        /// </summary>
        /// <param name="filters">Filters for Content page.</param>
        /// <param name="sorts">Sorts for Content page.</param>
        /// <param name="pageIndex">Index of page.</param>
        /// <param name="pageSize">Size of page.</param>
        /// <returns>Returns ContentPageListModel.</returns>
        ContentPageListModel GetContentPageList(FilterCollection filters, ExpandCollection expands, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get content page list for portal.
        /// </summary>
        /// <param name="filters">Filters for content page list.</param>
        /// <returns>Model wth content page list.</returns>
        WebStoreContentPageListModel GetContentPagesList(FilterCollection filters);

        /// <summary>
        /// Get Content page data by Content page id and LocaleId.
        /// </summary>        
        /// <param name="filters">Content page id and LocaleId</param>
        /// <returns>Returns ContentPageModel.</returns>
        ContentPageModel GetContentPage(FilterCollection filters);

        /// <summary>
        /// Update Content page.
        /// </summary>
        /// <param name="ContentPageModel">ContentPageModel.</param>
        /// <returns>Returns updated Content page.</returns>
        ContentPageModel UpdateContentPage(ContentPageModel ContentPageModel);

        /// <summary>
        /// Delete Content Page.
        /// </summary>
        /// <param name="ContentPageId">Parameter model containing Content page ids to be deleted.</param>
        /// <returns>Returns true if content page deleted successfully else return false.</returns>
        bool DeleteContentPage(ParameterModel ContentPageId);

        /// <summary>
        /// Publish the Content Page
        /// </summary>
        /// <param name="parameterModel"></param>
        /// <returns>Returns the model with result true if published successfully else return model with false and error message.</returns>
        PublishedModel PublishContentPage(ContentPageParameterModel parameterModel);

        #region Content Page Template
        /// <summary>
        /// Get Content page template list. 
        /// </summary>
        /// <param name="filters">Filters for Content page template.</param>
        /// <param name="sorts">Sorts for Content page template.</param>
        /// <param name="pageIndex">Index of page template.</param>
        /// <param name="pageSize">Size of page template.</param>
        /// <returns>Returns CMSContentPageTemplateListModel.</returns>
        CMSContentPageTemplateListModel GetContentPageTemplateList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);
        #endregion
        #endregion



        #region Content Page Tree
        /// <summary>
        /// Get the content page tree.
        /// </summary>
        /// <returns>Returns content page tree.</returns>
        ContentPageTreeModel GetTree();

        /// <summary>
        /// Rename the existing folder.
        /// </summary>
        /// <param name="folderModel">Folder to rename.</param>
        /// <returns>Returns true if renamed successfully else false.</returns>
        bool RenameFolder(ContentPageFolderModel folderModel);

        /// <summary>
        /// Add new folder.
        /// </summary>
        /// <param name="folderModel">Folder to add.</param>
        /// <returns>Returns Content page model.</returns>
        ContentPageFolderModel AddFolder(ContentPageFolderModel folderModel);

        /// <summary>
        /// Delete the existing folder.
        /// </summary>
        /// <param name="cmsContentPageGroupId">Ids to delete.</param>
        /// <returns>Returns true if deleted successfully else false.</returns>
        bool DeleteFolder(ParameterModel cmsContentPageGroupId);

        /// <summary>
        /// Move one folder to other folder within content page.
        /// </summary>
        /// <param name="model">ContentPageFolderModel</param>
        /// <returns>Returns true if moved successfully else false.</returns>
        bool MoveContentPagesFolder(ContentPageFolderModel folderModel);

        /// <summary>
        /// Move content pages from one folder to another.
        /// </summary>
        /// <param name="model">AddPageToFolderModel model.</param>
        /// <returns>Returns true if pages moved successfully to other folder else return false.</returns>
        bool MovePageToFolder(AddPageToFolderModel model);
        #endregion
    }
}
