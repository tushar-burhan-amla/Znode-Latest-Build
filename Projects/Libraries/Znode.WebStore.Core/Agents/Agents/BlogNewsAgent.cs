using System;
using System.Collections.Generic;
using System.Linq;
using Znode.Engine.Api.Client;
using Znode.Engine.Api.Models;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.WebStore.Agents
{
    public class BlogNewsAgent : BaseAgent, IBlogNewsAgent
    {
        #region Private Variables
        private readonly IBlogNewsClient _blogNewsClient;
        #endregion

        #region Constructor
        public BlogNewsAgent(IBlogNewsClient blogNewsClient)
        {
            _blogNewsClient = GetClient<IBlogNewsClient>(blogNewsClient);
        }
        #endregion

        #region Public Methods
        //Get list of blog news.
        public virtual BlogNewsListViewModel GetBlogNewsList(string blogNewsType)
        {
            FilterCollection filters = GetFiltersForBlogNews(blogNewsType);
            _blogNewsClient.SetPublishStateExplicitly(PortalAgent.CurrentPortal.PublishState);
            _blogNewsClient.SetLocaleExplicitly(PortalAgent.CurrentPortal.LocaleId);
            _blogNewsClient.SetDomainHeaderExplicitly(GetCurrentWebstoreDomain());
            WebStoreBlogNewsListModel blogNewsListModel = _blogNewsClient.GetBlogNewsListForWebstore(filters);
            return new BlogNewsListViewModel { BlogNewsList = blogNewsListModel?.BlogNewsList?.ToViewModel<BlogNewsViewModel>().ToList() };
        }

        //Get the published blog news data on the basis of its id and locale id. 
        public virtual BlogNewsViewModel GetBlogNewsData(int blogNewsId)
        {
            int localeId = PortalAgent.LocaleId;
            int portallId = PortalAgent.CurrentPortal.PortalId;
            WebStoreBlogNewsModel blogNews = _blogNewsClient.GetBlogNewsForWebstore(blogNewsId, localeId, portallId, null, DateTime.UtcNow.Date.ToString("dd-MM-yyyy"));
            return HelperUtility.IsNotNull(blogNews) ? blogNews.ToViewModel<BlogNewsViewModel>() : new BlogNewsViewModel();
        }

        //Save comments against a blog/news.
        public virtual BlogNewsCommentViewModel SaveComments(BlogNewsCommentViewModel model)
        {
            WebStoreBlogNewsCommentModel commentModel = _blogNewsClient.SaveComments(model?.ToModel<WebStoreBlogNewsCommentModel>());
            return HelperUtility.IsNotNull(commentModel) ? commentModel.ToViewModel<BlogNewsCommentViewModel>() : new BlogNewsCommentViewModel();
        }

        //Get list of user comments against a blog/news.
        public virtual List<BlogNewsCommentViewModel> GetUserCommentList(int blogNewsId)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(ZnodeBlogNewsCommentEnum.BlogNewsId.ToString(), FilterOperators.Equals, blogNewsId.ToString());
            List<WebStoreBlogNewsCommentModel> blogNewsComments = _blogNewsClient.GetUserCommentList(filters, null, null, null, null);

            List<BlogNewsCommentViewModel> blogNewsCommentListViewModel = blogNewsComments?.ToViewModel<BlogNewsCommentViewModel>().ToList();
            return blogNewsCommentListViewModel?.Count > 0 ? blogNewsCommentListViewModel : new List<BlogNewsCommentViewModel>();
        }
        #endregion

        #region Private Methods
        //Get filters for blog news.
        private static FilterCollection GetFiltersForBlogNews(string blogNewsType)
        {
            FilterCollection filters = new FilterCollection();
            filters.Add(new FilterTuple(ZnodeBlogNewEnum.BlogNewsType.ToString(), FilterOperators.Equals, blogNewsType.ToString()));
            filters.Add(new FilterTuple(ZnodeBlogNewEnum.ActivationDate.ToString(), FilterOperators.Equals, HelperUtility.GetDateTime().ToShortDateString()));
            filters.Add(new FilterTuple(ZnodeLocaleEnum.LocaleId.ToString(), FilterOperators.Equals, PortalAgent.LocaleId.ToString()));
            filters.Add(ZnodePortalEnum.PortalId.ToString(), FilterOperators.Equals, PortalAgent.CurrentPortal.PortalId.ToString());
            return filters;
        }
        #endregion
    }
}
