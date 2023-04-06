using System.Collections.Generic;
using Znode.Engine.WebStore.ViewModels;

namespace Znode.Engine.WebStore.Agents
{
    public interface IBlogNewsAgent
    {
        /// <summary>
        /// Get list of blog news.
        /// </summary>
        /// <param name="blogNewsType">Type:- Blog or News.</param>
        /// <returns>Returns list of blog news.</returns>
        BlogNewsListViewModel GetBlogNewsList(string blogNewsType);
        
        /// <summary>
        /// Get the published blog news data on the basis of its id and locale id. 
        /// </summary>
        /// <param name="blogNewsId">BlogNews id.</param>
        /// <returns>Returns published data for a blog/news.</returns>
        BlogNewsViewModel GetBlogNewsData(int blogNewsId);

        /// <summary>
        /// Save the comments against blog/news.
        /// </summary>
        /// <param name="model">BlogNewsCommentViewModel model.</param>
        /// <returns>Returns the comment model.</returns>
        BlogNewsCommentViewModel SaveComments(BlogNewsCommentViewModel model);

        /// <summary>
        ///  Get the list of user comments saved against blog/news.
        /// </summary>
        /// <param name="blogNewsId">Id of blog/news.</param>
        /// <returns>Returns list of user comments saved against blog/news.</returns>
        List<BlogNewsCommentViewModel> GetUserCommentList(int blogNewsId);        
    }
}
