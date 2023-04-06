
namespace Znode.Engine.Api.Client.Endpoints
{
    public class BlogNewsEndpoint : BaseEndpoint
    {
        #region Blog/News
        //Create blog/news Endpoint.
        public static string CreateBlogNews() => $"{ApiRoot}/blognews/createblognews";

        //Get blog/news List Endpoint.
        public static string GetBlogNewsList() => $"{ApiRoot}/blognews/blognewslist";

        //Get blog/news on the basis of Blog/News id Endpoint.
        public static string GetBlogNews(int blogNewsId, int localeId) => $"{ApiRoot}/blognews/getblognews/{blogNewsId}/{localeId}";

        //Update blog/news Endpoint.
        public static string UpdateBlogNews() => $"{ApiRoot}/blognews/updateblognews";

        //Delete blog(s)/news Endpoint.
        public static string DeleteBlogNews() => $"{ApiRoot}/blognews/deleteblognews";

        //Activate deactivate blog(s)/news or allow/deny guest comment(s).
        public static string ActivateDeactivateBlogNews() => $"{ApiRoot}/blognews/activatedeactivateblognews";

        //Get blog news list.
        public static string GetBlogNewsListForWebstore() => $"{ApiRoot}/blognews/blognewslistforwebstore";

        //Get blog/news data.
        public static string GetBlogNewsForWebstore(int blogNewsId, int localeId,int portalId, string activationDate = null) => $"{ApiRoot}/blognews/blognewsforwebstore/{blogNewsId}/{localeId}/{portalId}/{activationDate}";
        #endregion

        #region Blog/News Comments
        //Save the comments against blog/news.
        public static string SaveComments() => $"{ApiRoot}/blognews/savecomments";

        //Get the list of user comments saved against blog/news.
        public static string GetUserCommentList() => $"{ApiRoot}/blognews/getusercommentlist";

        //Get blog/news comment list.
        public static string GetBlogNewsCommentList() => $"{ApiRoot}/blognews/getblognewscommentlist";

        //Update blog/news comment Endpoint.
        public static string UpdateBlogNewsComment() => $"{ApiRoot}/blognews/updateblognewscomment";

        //Delete blog/news comment(s) endpoint.
        public static string DeleteBlogNewsComment() => $"{ApiRoot}/blognews/deleteblognewscomment";

        //Approve/Disapprove blog(s)/news comment(s).
        public static string ApproveDisapproveBlogNewsComment() => $"{ApiRoot}/blognews/approvedisapproveblognewscomment";
        #endregion

        #region Publish BlogNews
        //Save Blog/News Endpoint.
        public static string PublishBlogNews() => $"{ApiRoot}/blognews/publishblognews";
        #endregion
    }
}
