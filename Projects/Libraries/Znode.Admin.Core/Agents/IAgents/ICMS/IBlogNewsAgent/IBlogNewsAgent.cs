using System.Collections.Generic;
using System.Web.Mvc;
using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Agents
{
    public interface IBlogNewsAgent
    {
        #region Blog/News
        /// <summary>
        /// Get the select list of locales.
        /// </summary>
        /// <returns>Returns select list of locales.</returns>
        List<SelectListItem> GetLocalesList(int localeId = 0);

        /// <summary>
        /// Create new blog/news.
        /// </summary>
        /// <param name="blogNewsViewModel">Blog/NewsViewModel.</param>
        /// <returns>Returns blog/news viewModel with data.</returns>
        BlogNewsViewModel CreateBlogNews(BlogNewsViewModel blogNewsViewModel);

        /// <summary>
        /// Get blog/news information.
        /// </summary>
        /// <param name="blogNewsid">Uses blog news id to get data.</param>
        /// <param name="localeId">Uses locale id to get data.</param>
        /// <returns>Returns blog/news model with data.</returns>
        BlogNewsViewModel GetBlogNews(int blogNewsid, int localeId);

        /// <summary>
        /// Update existing blog/news.
        /// </summary>
        /// <param name="blogNewsViewModel">Uses blog/NewsViewModel with data.</param>
        /// <returns>Return updated blog/news model.</returns>
        BlogNewsViewModel UpdateBlogNews(BlogNewsViewModel blogNewsViewModel);

        /// <summary>
        /// Get list of all blogs/news.
        /// </summary>
        /// <param name="filters">Filter collection for blog/news.</param>
        /// <param name="sortCollection">Sort collection for blog/news.</param>
        /// <param name="expands">Expands collection for blog/news.</param>
        /// <param name="pageIndex">Page index for blog/news.</param>
        /// <param name="pageSize">Page size for blog/news.</param>
        /// <returns>Returns list of blog/news.</returns>
        BlogNewsListViewModel GetBlogNewsList(FilterCollection filters = null, SortCollection sortCollection = null, ExpandCollection expands = null, int? pageIndex = null, int? pageSize = null);

        /// <summary>
        /// Delete blog(s)/news.
        /// </summary>
        /// <param name="blogNewsIds">Uses blog/news ids.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool DeleteBlogNews(string blogNewsIds);

        /// <summary>
        /// Activate/Deactivate blog(s)/news or allow/deny guest comments.
        /// </summary>
        /// <param name="blogNewsIds">Activate/Deactivate blog(s)/news on the basis of blog/news id.</param>
        /// <param name="isTrueOrFalse">Activate/Deactivate blog/news or allow/deny guest comments on the basis of true or false.</param>
        /// <param name="activity">Check whether to activate/deactivate blog/news or allow guest comments.</param>
        /// <returns>Returns true if updated successfully else return false.</returns>
        bool ActivateDeactivateBlogNews(string blogNewsIds, bool isTrueOrFalse, string activity);

        /// <summary>
        /// Get Content page list for blog/news. 
        /// </summary>
        /// <param name="filters">Filters for content page list.</param>
        /// <param name="sorts">Sorts for content page.</param>
        /// <param name="pageIndex">Page index.</param>
        /// <param name="pageSize">Size of page.</param>
        /// <param name="PortalId">PortalId</param>
        /// <param name="localeId">Get content page list information on the basis of localeId.</param>
        /// <returns>Returns list of Content pages for blog/news.</returns>
        ContentPageListViewModel GetContentPageList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize, int? PortalId, int? localeId);
        #endregion

        #region Blog/News Comment
        /// <summary>
        /// Get blog/news comment list.
        /// </summary>
        /// <param name="blogNewsTitle">Blog News title for filters.</param>
        /// <param name="filters">Filter collection for blog/news comments.</param>
        /// <param name="sortCollection">Sort collection for blog/news comments.</param>
        /// <param name="expands">Expands collection for blog/news comments.</param>
        /// <param name="pageIndex">Page index.</param>
        /// <param name="pageSize">Page size.</param>
        /// <returns>Returns blog/news comment list.</returns>
        BlogNewsCommentListViewModel GetBlogNewsCommentList(int blogNewsId, FilterCollection filters = null, SortCollection sortCollection = null, ExpandCollection expands = null, int? pageIndex = null, int? pageSize = null);

        /// <summary>
        /// Update blog/news comment data.
        /// </summary>
        /// <param name="data">Blog/News comment details as json format.</param>
        /// <returns>Returns blog/news comment view model with information.</returns>
        BlogNewsCommentViewModel UpdateBlogNewsComment(string data);

        /// <summary>
        /// Delete blog/news comments.
        /// </summary>
        /// <param name="blogNewsCommentIds">Uses blog/news comment ids.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool DeleteBlogNewsComment(string blogNewsCommentIds);

        /// <summary>
        /// Approves/Disapprove blog/news comment(s).
        /// </summary>
        /// <param name="blogNewsCommentIds">Uses blo/news comment ids.</param>
        /// <param name="isApproved">Status to Approve or disapprove.</param>
        /// <returns>Returns true if updated successfully else return false.</returns>
        bool ApproveDisapproveBlogNewsComment(string blogNewsCommentIds, bool isApproved);
        #endregion

        #region
        /// <summary>
        /// Publish the Blog/News.
        /// </summary>
        /// <param name="blogNewsId">blogNewsId</param>
        /// <param name="localeId">localeId</param>
        /// <param name="errorMessage">errorMessage</param>
        /// <param name="targetPublishState">targetPublishState</param>
        /// <param name="takeFromDraftFirst">takeFromDraftFirst</param>
        /// <returns>Returns the model with result true if published successfully else return model with false and error message.</returns>
        bool PublishBlogNews(string blogNewsId, out string errorMessage, int localeId = 0, string targetPublishState = null, bool takeFromDraftFirst = false);

        #endregion
    }
}
