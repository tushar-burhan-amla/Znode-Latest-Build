using System;
using System.Collections.Generic;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Cache
{
    public class BlogNewsCache : BaseCache, IBlogNewsCache
    {
        #region Private Variable
        private readonly IBlogNewsService _service;
        #endregion

        #region Constructor
        public BlogNewsCache(IBlogNewsService service)
        {
            _service = service;
        }
        #endregion

        #region Public Methods
        #region Blog/News
        //Get blog/news list.
        public virtual string GetBlogNewsList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Blog/News list
                BlogNewsListModel list = _service.GetBlogNewsList(Filters, Expands, Sorts, Page);
                if (list?.BlogNewsList?.Count > 0)
                {
                    BlogNewsListResponse response = new BlogNewsListResponse { BlogNewsList = list.BlogNewsList };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                    return data;
                }
            }
            return data;
        }

        //Get blog/news by blog/news id and locale id.
        public virtual string GetBlogNews(int blogNewsId, int localeId, string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                BlogNewsModel blogNews = _service.GetBlogNews(blogNewsId, localeId, Expands);
                if (HelperUtility.IsNotNull(blogNews))
                    data = InsertIntoCache(routeUri, routeTemplate, new BlogNewsResponse { BlogNews = blogNews });
            }
            return data;
        }

        //Get blogs/news list for portal.
        public virtual string GetBlogNewsListForWebstore(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                WebStoreBlogNewsListModel list = _service.GetBlogNewsListForWebstore(Filters);
                if (list?.BlogNewsList?.Count > 0)
                {
                    WebStoreBlogNewsListResponse response = new WebStoreBlogNewsListResponse { BlogNewsList = list.BlogNewsList };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                    return data;
                }
            }
            return data;
        }

        //Get blog/news by blog/news id and locale id.
        public virtual string GetBlogNewsForWebstore(int blogNewsId, int localeId, int portalId, string routeUri, string routeTemplate, string activationdate = null)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                WebStoreBlogNewsModel blogNews = _service.GetBlogNewsForWebstore(blogNewsId, localeId, portalId, Expands, activationdate);
                if (HelperUtility.IsNotNull(blogNews))
                    data = InsertIntoCache(routeUri, routeTemplate, new WebStoreBlogNewsResponse { BlogNews = blogNews });
            }
            return data;
        }
        #endregion

        #region Blog/News Comments
        //Get blog/news comment list.
        public virtual string GetUserCommentList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Blog/News list
                List<WebStoreBlogNewsCommentModel> list = _service.GetUserCommentList(Filters, Expands, Sorts, Page);
                if (list?.Count > 0)
                {
                    WebStoreBlogNewsCommentListResponse response = new WebStoreBlogNewsCommentListResponse { BlogNewsCommentList = list };
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                    return data;
                }
            }
            return data;
        }

        //Get blog/news comment list.
        public virtual string GetBlogNewsCommentList(string routeUri, string routeTemplate)
        {
            //Get data from cache.
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Blog/News list
                BlogNewsCommentListModel list = _service.GetBlogNewsCommentList(Filters, Expands, Sorts, Page);
                if (list?.BlogNewsCommentList?.Count > 0)
                {
                    BlogNewsCommentListResponse response = new BlogNewsCommentListResponse { BlogNewsCommentList = list.BlogNewsCommentList };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                    return data;
                }
            }
            return data;
        }
        #endregion
        #endregion
    }
}
