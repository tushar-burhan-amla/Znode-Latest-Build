using System;
using System.Web.Mvc;
using Znode.Engine.WebStore.Agents;
using Znode.Engine.WebStore.Controllers;
using Znode.Engine.WebStore.ViewModels;

namespace Znode.WebStore.Core.Controllers
{
    public class BlogNewsController : BaseController
    {
        #region Private Readonly members
        private readonly IBlogNewsAgent _blogNewsAgent;
        #endregion

        #region Public Constructor
        public BlogNewsController(IBlogNewsAgent blogNewsAgent)
        {
            _blogNewsAgent = blogNewsAgent;
        }
        #endregion

        //Get list of blog news.
        [Route("Blog")]
        [Route("News")]
        public virtual ActionResult BlogNewsList()
        {
            string actionName = ((System.Web.Routing.Route)ControllerContext.RouteData.Route).Url;
            BlogNewsListViewModel blogNewsList = _blogNewsAgent.GetBlogNewsList(actionName);
            blogNewsList.BlogNewsType = actionName;
            return View("BlogNews", blogNewsList);
        }

        //Get published blog news data.
        [HttpGet]
        public virtual ActionResult Index(int blogNewsId = 0)
        {
            var model = _blogNewsAgent.GetBlogNewsData(blogNewsId);
            if (model?.BlogNewsId > 0)
            {
                //Set Properties for SEO data.
                ViewBag.Title = model?.SEOTitle;
                ViewBag.Description = model?.SEODescription;
                ViewBag.Keywords = model?.SEOKeywords;
                return View("BlogNewsDetails", model);
            }
            else
            {
                return Redirect("/404");
            }
        }

        //Save the comments against a blog/news.
        [HttpPost]
        public virtual ActionResult SaveComments(int BlogNewsId, string BlogNewsComment)
            => Json(_blogNewsAgent.SaveComments(new BlogNewsCommentViewModel { BlogNewsId = BlogNewsId, BlogNewsComment = BlogNewsComment, LocaleId = PortalAgent.LocaleId }), JsonRequestBehavior.AllowGet);

        //Get the list of user comments against a blog/news.
        [Route("BlogNews/GetUserCommentList")]
        public virtual ActionResult GetUserCommentList(string blogNewsId)
        => View("_ComentDisplaySection", _blogNewsAgent.GetUserCommentList(Convert.ToInt32(blogNewsId)));
    }
}
