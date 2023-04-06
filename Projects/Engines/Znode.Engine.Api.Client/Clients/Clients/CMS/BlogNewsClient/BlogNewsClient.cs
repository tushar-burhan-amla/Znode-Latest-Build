using Newtonsoft.Json;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public class BlogNewsClient : BaseClient, IBlogNewsClient
    {
        #region Public Methods
        #region Blog/News
        //This method will create a new blog/news.
        public virtual BlogNewsModel CreateBlogNews(BlogNewsModel model)
        {
            string endpoint = BlogNewsEndpoint.CreateBlogNews();

            ApiStatus status = new ApiStatus();
            BlogNewsResponse response = PostResourceToEndpoint<BlogNewsResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.BlogNews;
        }

        //Get Blog/News list.
        public virtual BlogNewsListModel GetBlogNewsList(FilterCollection filters, ExpandCollection expands, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = BlogNewsEndpoint.GetBlogNewsList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            BlogNewsListResponse response = GetResourceFromEndpoint<BlogNewsListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            BlogNewsListModel list = new BlogNewsListModel { BlogNewsList = response?.BlogNewsList };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Get blog/news on the basis of blog/news id.
        public virtual BlogNewsModel GetBlogNews(int blogNewsId, int localeId, ExpandCollection expands)
        {
            string endpoint = BlogNewsEndpoint.GetBlogNews(blogNewsId, localeId);
            endpoint += BuildEndpointQueryString(expands);

            ApiStatus status = new ApiStatus();
            BlogNewsResponse response = GetResourceFromEndpoint<BlogNewsResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.BlogNews;
        }

        //Update blog/news.
        public virtual BlogNewsModel UpdateBlogNews(BlogNewsModel blogNewsModel)
        {
            string endpoint = BlogNewsEndpoint.UpdateBlogNews();

            ApiStatus status = new ApiStatus();
            BlogNewsResponse response = PutResourceToEndpoint<BlogNewsResponse>(endpoint, JsonConvert.SerializeObject(blogNewsModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.BlogNews;
        }

        //Delete blog(s)/news.
        public virtual bool DeleteBlogNews(ParameterModel BlogNewsId)
        {
            string endpoint = BlogNewsEndpoint.DeleteBlogNews();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(BlogNewsId), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        //Activate/Deactivate blog(s)/news or allow/disallow guest comments.
        public virtual bool ActivateDeactivateBlogNews(BlogNewsParameterModel model)
        {
            string endpoint = BlogNewsEndpoint.ActivateDeactivateBlogNews();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(model), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }

        //Get blogs/news list.
        public virtual WebStoreBlogNewsListModel GetBlogNewsListForWebstore(FilterCollection filters)
        {
            string endpoint = BlogNewsEndpoint.GetBlogNewsListForWebstore();
            endpoint += BuildEndpointQueryString(null, filters, null, null, null);

            ApiStatus status = new ApiStatus();
            WebStoreBlogNewsListResponse response = GetResourceFromEndpoint<WebStoreBlogNewsListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            WebStoreBlogNewsListModel list = new WebStoreBlogNewsListModel { BlogNewsList = response?.BlogNewsList };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Get blog/news on the basis of blog/news id and locale for displaying on webstore.
        public virtual WebStoreBlogNewsModel GetBlogNewsForWebstore(int blogNewsId, int localeId, int portalId, ExpandCollection expands, string activationDate = null)
        {
            string endpoint = BlogNewsEndpoint.GetBlogNewsForWebstore(blogNewsId, localeId, portalId, activationDate);
            endpoint += BuildEndpointQueryString(expands);

            ApiStatus status = new ApiStatus();
            WebStoreBlogNewsResponse response = GetResourceFromEndpoint<WebStoreBlogNewsResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.BlogNews;
        }
        #endregion

        #region Blog/News Comments
        //Save the comments against blog/news.
        public virtual WebStoreBlogNewsCommentModel SaveComments(WebStoreBlogNewsCommentModel model)
        {
            string endpoint = BlogNewsEndpoint.SaveComments();

            ApiStatus status = new ApiStatus();
            WebStoreBlogNewsCommentResponse response = PostResourceToEndpoint<WebStoreBlogNewsCommentResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.BlogNewsComment;
        }

        //Get the list of user comments saved against blog/news.
        public virtual List<WebStoreBlogNewsCommentModel> GetUserCommentList(FilterCollection filters, ExpandCollection expands, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = BlogNewsEndpoint.GetUserCommentList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            WebStoreBlogNewsCommentListResponse response = GetResourceFromEndpoint<WebStoreBlogNewsCommentListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            WebStoreBlogNewsCommentListModel list = new WebStoreBlogNewsCommentListModel { BlogNewsCommentList = response?.BlogNewsCommentList };
            list.MapPagingDataFromResponse(response);

            return list.BlogNewsCommentList;
        }

        //Get blog/news comment list.
        public virtual BlogNewsCommentListModel GetBlogNewsCommentList(FilterCollection filters, ExpandCollection expands, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = BlogNewsEndpoint.GetBlogNewsCommentList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            BlogNewsCommentListResponse response = GetResourceFromEndpoint<BlogNewsCommentListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            BlogNewsCommentListModel list = new BlogNewsCommentListModel { BlogNewsCommentList = response?.BlogNewsCommentList };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Update blog/news comment.
        public virtual BlogNewsCommentModel UpdateBlogNewsComment(BlogNewsCommentModel blogNewsCommentModel)
        {
            string endpoint = BlogNewsEndpoint.UpdateBlogNewsComment();

            ApiStatus status = new ApiStatus();
            BlogNewsCommentResponse response = PutResourceToEndpoint<BlogNewsCommentResponse>(endpoint, JsonConvert.SerializeObject(blogNewsCommentModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.BlogNewsComment;
        }

        //Delete blog/news comment(s).
        public virtual bool DeleteBlogNewsComment(ParameterModel BlogNewsCommentId)
        {
            string endpoint = BlogNewsEndpoint.DeleteBlogNewsComment();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(BlogNewsCommentId), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        //Approve or disapprove blog/news comment(s).
        public virtual bool ApproveDisapproveBlogNewsComment(BlogNewsParameterModel model)
        {
            string endpoint = BlogNewsEndpoint.ApproveDisapproveBlogNewsComment();

            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(model), status);
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response.IsSuccess;
        }
        #endregion

        #region Publish BlogNews
        //Publish BlogNews.
        public virtual PublishedModel PublishBlogNews(BlogNewsParameterModel parameterModel)
        {
            string endPoint = BlogNewsEndpoint.PublishBlogNews();
            ApiStatus status = new ApiStatus();
            PublishedResponse response = PostResourceToEndpoint<PublishedResponse>(endPoint, JsonConvert.SerializeObject(parameterModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response?.PublishedModel;
        }
        #endregion

        #endregion
    }
}
