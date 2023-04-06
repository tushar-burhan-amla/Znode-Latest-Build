using System.Collections.Generic;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IBlogNewsClient : IBaseClient
    {
        #region Blog/News
        /// <summary>
        /// Create new blog/news.
        /// </summary>
        /// <param name="model">BlogNewsModel.</param>
        /// <returns>Return model blogNewsModel with information.</returns>
        BlogNewsModel CreateBlogNews(BlogNewsModel model);

        /// <summary>
        /// Get blog/news list. 
        /// </summary>
        /// <param name="filters">Filters for blog/news.</param>
        /// <param name="sorts">Sorts for blog/news.</param>
        /// <param name="pageIndex">Index of page.</param>
        /// <param name="pageSize">Size of page.</param>
        /// <returns>Returns BlogNewsListModel with data.</returns>
        BlogNewsListModel GetBlogNewsList(FilterCollection filters, ExpandCollection expands, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get blog/news data by blog/newsId.
        /// </summary>
        /// <param name="blogNewsId">Uses blog/news id.</param>
        /// <param name="localeId">Uses locale id.</param>
        /// <param name="expands">Expands for blog/news.</param>
        /// <returns>Returns blog/news model with data.</returns>
        BlogNewsModel GetBlogNews(int blogNewsId, int localeId, ExpandCollection expands);

        /// <summary>
        /// Update blog/news.
        /// </summary>
        /// <param name="BlogNewsModel">BlogNewsModel.</param>
        /// <returns>Returns updated BlogNewsModel.</returns>
        BlogNewsModel UpdateBlogNews(BlogNewsModel blogNewsModel);

        /// <summary>
        /// Delete Blog(s)/News.
        /// </summary>
        /// <param name="BlogNewsId">Parameter model containing Blog/News ids to be deleted.</param>
        /// <returns>Returns true if blog(s)/news deleted sucessfully else return false.</returns>
        bool DeleteBlogNews(ParameterModel BlogNewsId);

        /// <summary>
        /// Activate/Deactivate muliple blog(s)/news or allow/disallow muliple guest comments.
        /// </summary>
        /// <param name="model">Uses model with data.</param>
        /// <returns>Returns true if blog(s)/news updated sucessfully else return false.</returns>
        bool ActivateDeactivateBlogNews(BlogNewsParameterModel model);

        /// <summary>
        /// Get blog news list to display on webstore.
        /// </summary>
        /// <param name="filters">Filter collection data.</param>
        /// <returns>Returns list of blog/news.</returns>
        WebStoreBlogNewsListModel GetBlogNewsListForWebstore(FilterCollection filters);

        /// <summary>
        /// Get the published blog news data on the basis of its id and locale id. 
        /// </summary>
        /// <param name="blogNewsId">Blog/News id.</param>
        /// <param name="localeId">Current locale id.</param>
        /// <param name="expands">Expands if any.</param>
        /// <param name="activationDate">activationDate nullable date.</param>
        /// <returns>Returns published data for a blog/news.</returns>
        WebStoreBlogNewsModel GetBlogNewsForWebstore(int blogNewsId, int localeId, int portalId, ExpandCollection expands, string activationDate = null);
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
        /// <param name="pageIndex">Index of page.</param>
        /// <param name="pageSize">Size of page.</param>
        /// <returns>Returns list of user comments saved against blog/news.</returns>
        List<WebStoreBlogNewsCommentModel> GetUserCommentList(FilterCollection filters, ExpandCollection expands, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get blog/news comment list. 
        /// </summary>
        /// <param name="filters">Filters for blog/news comment list.</param>
        /// <param name="sorts">Sorts for blog/news comment list.</param>
        /// <param name="pageIndex">Index of page.</param>
        /// <param name="pageSize">Size of page.</param>
        /// <returns>Returns BlogNewsCommentListModel with data.</returns>
        BlogNewsCommentListModel GetBlogNewsCommentList(FilterCollection filters, ExpandCollection expands, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Update blog/news comment.
        /// </summary>
        /// <param name="blogNewsCommentModel">Uses blog/news comment model.</param>
        /// <returns>Returns updated blog/news comment model.</returns>
        BlogNewsCommentModel UpdateBlogNewsComment(BlogNewsCommentModel blogNewsCommentModel);

        /// <summary>
        /// Delete blog/news comment(s).
        /// </summary>
        /// <param name="BlogNewsCommentId">Parameter model containing Blog/News comment ids to be deleted.</param>
        /// <returns>Returns true if blog/news comments deleted sucessfully else return false.</returns>
        bool DeleteBlogNewsComment(ParameterModel BlogNewsCommentId);

        /// <summary>
        /// Approve or disapprove multiple blog/news comment(s).
        /// </summary>
        /// <param name="model">Uses model with data.</param>
        /// <returns>Returns true if blog/news comments updated sucessfully else return false.</returns>
        bool ApproveDisapproveBlogNewsComment(BlogNewsParameterModel model);
        #endregion

        #region Blog/News Publish
        /// <summary>
        /// Publish the Blog/News
        /// </summary>
        /// <param name="parameterModel"></param>
        /// <returns>Returns the model with result true if published successfully else return model with false and error message.</returns>
        PublishedModel PublishBlogNews(BlogNewsParameterModel parameterModel);
        #endregion
    }
}