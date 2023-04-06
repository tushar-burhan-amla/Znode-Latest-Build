
namespace Znode.Engine.Api.Cache
{
    public interface IBlogNewsCache
    {
        #region Blog/News
        /// <summary>
        /// Get blog/news list. 
        /// </summary>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns>String Data.</returns>
        string GetBlogNewsList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get blog/news data on the basis of blog/news id and locale id.
        /// </summary>
        /// <param name="blogNewsId">Blog/News id.</param>
        /// <param name="localeId">Locale id.</param>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns>String Data.</returns>
        string GetBlogNews(int blogNewsId, int localeId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get the list of blogs/news to display on webstore.
        /// </summary>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns>Returns list of blogs/news.</returns>
        string GetBlogNewsListForWebstore(string routeUri, string routeTemplate);

        /// <summary>
        /// Get the published blog news data on the basis of its id and locale id. 
        /// </summary>
        /// <param name="blogNewsId">Blog/News id.</param>
        /// <param name="localeId">Current locale id.</param>
        /// <param name="portalId"></param>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <param name="activationdate">activationdate nullable date.</param>
        /// <returns>Returns published data for a blog/news.</returns>
        string GetBlogNewsForWebstore(int blogNewsId, int localeId,int portalId, string routeUri, string routeTemplate, string activationdate = null);
        #endregion

        #region Blog/News Comments
        /// <summary>
        /// Get the list of user comments saved against blog/news.
        /// </summary>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns>Returns list of user comments saved against blog/news.</returns>
        string GetUserCommentList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get blog/news comments list.
        /// </summary>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns>Return list of blog/news comments.</returns>
        string GetBlogNewsCommentList(string routeUri, string routeTemplate);
        #endregion
    }
}
