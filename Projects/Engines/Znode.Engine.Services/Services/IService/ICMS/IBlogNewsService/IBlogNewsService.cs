using System.Collections.Generic;
using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IBlogNewsService
    {
        #region Blog/News
        /// <summary>
        /// Create new blog/news.
        /// </summary>
        /// <param name="model">Blog/NewsModel.</param>
        /// <returns>Returns blog/NewsModel with information.</returns>
        BlogNewsModel CreateBlogNews(BlogNewsModel model);

        /// <summary>
        /// Get blog/news list. 
        /// </summary>
        /// <param name="filters">Filters for static page.</param>
        /// <param name="sorts">Sorts for static page.</param>
        /// <param name="page">Page.</param>
        /// <returns>Returns StaticPageListModel.</returns>
        BlogNewsListModel GetBlogNewsList(FilterCollection filters, NameValueCollection expands, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get blog/news data by blog/news id.
        /// </summary>
        /// <param name="blogNewsId">Blog/NewsId.</param>
        ///<param name="localeId">Locale id.</param>
        /// <param name="expands">Expands for blog/news.</param>
        /// <returns>Returns blog/news model with data.</returns>
        BlogNewsModel GetBlogNews(int blogNewsId, int localeId, NameValueCollection expands);

        /// <summary>
        /// Update blog/news.
        /// </summary>
        /// <param name="blogNewsModel">Uses blogNewsModel data.</param>
        /// <returns>Returns true if data updated sucessfully else return false.</returns>
        bool UpdateBlogNews(BlogNewsModel blogNewsModel);

        /// <summary>
        /// Delete blog/news.
        /// </summary>
        /// <param name="blogNewsIds">Parameter model containing blog/news ids to be deleted.</param>
        /// <returns>Returns true if blog/news deleted sucessfully else return false.</returns>
        bool DeleteBlogNews(ParameterModel blogNewsIds);

        /// <summary>
        /// Activate/deactivate blog/news or allow/disallow guest comments.
        /// </summary>
        /// <param name="model">Uses model for multiple update.</param>
        /// <returns>Returns true if data updated successfully else return false.</returns>
        bool ActivateDeactivateBlogNews(BlogNewsParameterModel model);

        /// <summary>
        /// Get list of blogs/news to display on webstore.
        /// </summary>
        /// <param name="filters">Filter Collection data.</param>
        /// <returns>Returns list of blogs/news.</returns>
        WebStoreBlogNewsListModel GetBlogNewsListForWebstore(FilterCollection filters);

        /// <summary>
        /// Get the published blog news data on the basis of its id and locale id. 
        /// </summary>
        /// <param name="blogNewsId">Blog/News id.</param>
        /// <param name="localeId">Current locale id.</param>
        /// <param name="expands">Expands if any.</param>
        /// <param name="activationdate">activationdate nullable date.</param>
        /// <returns>Returns published data for a blog/news.</returns>
        WebStoreBlogNewsModel GetBlogNewsForWebstore(int blogNewsId, int localeId, int portalId, NameValueCollection expands, string activationDate = null);
        #endregion

        #region Blog/News Comments
        /// <summary>
        /// Save the comments against blog/news.
        /// </summary>
        /// <param name="model">WebStoreBlogNewsCommentModel model.</param>
        /// <returns>Returns the comment model.</returns>
        WebStoreBlogNewsCommentModel SaveComments(WebStoreBlogNewsCommentModel model);

        /// <summary>
        /// Get the list of user comments saved against blog/news.
        /// </summary>
        /// <param name="filters">Filter collection.</param>
        /// <param name="expands">Expand collection.</param>
        /// <param name="sorts">Sort collection.</param>
        /// <param name="page">NameValueCollection.</param>
        /// <returns>Returns list of user comments saved against blog/news.</returns>
        List<WebStoreBlogNewsCommentModel> GetUserCommentList(FilterCollection filters, NameValueCollection expands, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get blog/news comment list.
        /// </summary>
        /// <param name="filters">Filter collection.</param>
        /// <param name="expands">Expand collection.</param>
        /// <param name="sorts">Sort collection.</param>
        /// <param name="page">NameValueCollection.</param>
        /// <returns>Returns list of blog/news comments.</returns>
        BlogNewsCommentListModel GetBlogNewsCommentList(FilterCollection filters, NameValueCollection expands, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Update blog/news comment.
        /// </summary>
        /// <param name="blogNewsCommentModel">Uses blogNewsCommentModel data.</param>
        /// <returns>Returns true if data updated sucessfully else return false.</returns>
        bool UpdateBlogNewsComment(BlogNewsCommentModel blogNewsCommentModel);

        /// <summary>
        /// Delete blog/news comment(s).
        /// </summary>
        /// <param name="blogNewsCommentIds">Parameter model containing blog/news comment ids to be deleted.</param>
        /// <returns>Returns true if blog/news comment(s) deleted sucessfully else return false.</returns>
        bool DeleteBlogNewsComment(ParameterModel blogNewsCommentIds);

        /// <summary>
        /// Approve or disapprove blog/news comments.
        /// </summary>
        /// <param name="model">Uses model.</param>
        /// <returns>Returns true if data updated sucessfully else return false.</returns>
        bool ApproveDisapproveBlogNewsComment(BlogNewsParameterModel model);
        #endregion

        /// <summary>
        /// Publish the blog/news.
        /// </summary>
        /// <param name="blogNewsId">bognews Id.</param>
        /// <param name="portalId">portal Id.</param>
        /// <param name="IsCMSPreviewEnable">Is CMS preview enable</param>
        /// <param name="localeId">local id of page</param>
        /// <param name="targetPublishState">Publish state.</param>
        /// <param name="takeFromDraftFirst">Take from draft.</param>
        /// <returns>Returns the published model</returns>
        PublishedModel PublishBlogNews(int blogNewsId, int portalId,bool IsCMSPreviewEnable, int localeId = 0, string targetPublishState = null, bool takeFromDraftFirst = false);

    }
}
