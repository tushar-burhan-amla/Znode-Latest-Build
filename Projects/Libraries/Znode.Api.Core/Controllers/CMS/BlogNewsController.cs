using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Helper;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Api.Controllers
{
    public class BlogNewsController : BaseController
    {
        #region Private Variables
        private readonly IBlogNewsCache _cache;
        private readonly IBlogNewsService _service;
        #endregion

        #region Constructor
        public BlogNewsController(IBlogNewsService service)
        {
            _service = service;
            _cache = new BlogNewsCache(_service);
        }
        #endregion

        #region Public Methods
        #region Blog/News
        /// <summary>
        /// Create new blog/news.
        /// </summary>
        /// <param name="model">BlogNewsModel.</param>
        /// <returns>Return created blog/news.</returns>
        [ResponseType(typeof(BlogNewsResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage CreateBlogNews([FromBody] BlogNewsModel model)
        {
            HttpResponseMessage response;
            try
            {
                BlogNewsModel BlogNews = _service.CreateBlogNews(model);

                if (!Equals(BlogNews, null))
                {
                    response = CreateCreatedResponse(new BlogNewsResponse { BlogNews = BlogNews });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(BlogNews.BlogNewsId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new BlogNewsResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex,ZnodeLogging.Components.CMS.ToString(),TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new BlogNewsResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(),TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get Blog/News List.
        /// </summary>
        /// <returns>Returns Blog/News List.</returns>
        [ResponseType(typeof(BlogNewsListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetBlogNewsList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetBlogNewsList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<BlogNewsListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new BlogNewsListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(),TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get blog/news details by blog/newsId and localeId.
        /// </summary>
        /// <param name="blogNewsId">Blog/News Id.</param>
        /// <param name="localeId">Locale Id.</param>
        /// <returns>Returns blog/news model.</returns>
        [ResponseType(typeof(BlogNewsResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetBlogNews(int blogNewsId, int localeId)
        {
            HttpResponseMessage response;

            try
            {
                //Get blog/news by id.
                string data = _cache.GetBlogNews(blogNewsId, localeId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<BlogNewsResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new BlogNewsResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(),TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new BlogNewsResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(),TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Update blog/news.
        /// </summary>
        /// <param name="model">model to update.</param>
        /// <returns>Returns updated model.</returns>
        [ResponseType(typeof(BlogNewsResponse))]
        [HttpPut, ValidateModel]
        public virtual HttpResponseMessage UpdateBlogNews([FromBody] BlogNewsModel model)
        {
            HttpResponseMessage response;
            try
            {
                bool isUpdated = _service.UpdateBlogNews(model);
                response = isUpdated ? CreateOKResponse(new BlogNewsResponse { BlogNews = model }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new BlogNewsResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new BlogNewsResponse { HasError = true, ErrorMessage = ex.Message ,ErrorCode=ErrorCodes.AssociationDeleteError});
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Delete blog(s)/news.
        /// </summary>
        /// <param name="BlogNewsId">Blog/News Id.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage DeleteBlogNews([FromBody] ParameterModel BlogNewsId)
        {
            HttpResponseMessage response;

            try
            {
                //Delete blog/news.
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.DeleteBlogNews(BlogNewsId) });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(),TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(),TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Activate/deactivate blog(s)/news.
        /// </summary>
        /// <param name="model">Uses Activate Deactivate blog/news model.</param>
        /// <returns>Returns true if updated successfully else false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        public virtual HttpResponseMessage ActivateDeactivateBlogNews(BlogNewsParameterModel model)
        {
            HttpResponseMessage response;

            try
            {
                bool status = _service.ActivateDeactivateBlogNews(model);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = status });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode=ErrorCodes.NotFound });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(),TraceLevel.Error);
            }
            return response;
        }
        #endregion

        #region Blog/News Comments
        /// <summary>
        /// Get Blog/News comment list.
        /// </summary>
        /// <returns>Returns Blog/News comment List.</returns>
        [ResponseType(typeof(BlogNewsCommentListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetBlogNewsCommentList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetBlogNewsCommentList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<BlogNewsCommentListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new BlogNewsCommentListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex,ZnodeLogging.Components.CMS.ToString(),TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Update blog/news comment.
        /// </summary>
        /// <param name="model">Blog/News comment model.</param>
        /// <returns>Returns updated model.</returns>
        [ResponseType(typeof(BlogNewsCommentResponse))]
        [HttpPut]
        public virtual HttpResponseMessage UpdateBlogNewsComment([FromBody] BlogNewsCommentModel model)
        {
            HttpResponseMessage response;
            try
            {
                bool isUpdated = _service.UpdateBlogNewsComment(model);
                response = isUpdated ? CreateOKResponse(new BlogNewsCommentResponse { BlogNewsComment = model }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new BlogNewsCommentResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(),TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new BlogNewsCommentResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Delete blog/news comment(s).
        /// </summary>
        /// <param name="BlogNewsCommentId">Blog/News comment id.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage DeleteBlogNewsComment([FromBody] ParameterModel BlogNewsCommentId)
        {
            HttpResponseMessage response;

            try
            {
                //Delete blog/news comments.
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.DeleteBlogNewsComment(BlogNewsCommentId) });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(),TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(),TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Approve/Disapprove blog/news comment(s).
        /// </summary>
        /// <param name="model">Uses model with data.</param>
        /// <returns>Returns true if updated successfully else false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        public virtual HttpResponseMessage ApproveDisapproveBlogNewsComment(BlogNewsParameterModel model)
        {
            HttpResponseMessage response;

            try
            {
                bool status = _service.ApproveDisapproveBlogNewsComment(model);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = status });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.NotFound });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }
        #endregion

        #region Publish BlogNews
        /// <summary>
        /// Publish the BlogNews
        /// </summary>
        /// <param name="parameterModel"></param>
        /// <returns>Returns the httpresponse model with result true if published successfully else return model with false and error message.</returns>
        [ResponseType(typeof(PublishedResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage PublishBlogNews(BlogNewsParameterModel parameterModel)
        {
            HttpResponseMessage response;
            try
            {
                PublishedModel publishedModel = _service.PublishBlogNews(Convert.ToInt32(parameterModel.BlogNewsId), parameterModel.PortalId, parameterModel.IsCMSPreviewEnable, parameterModel.LocaleId, parameterModel.TargetPublishState, parameterModel.TakeFromDraftFirst);
                response = !Equals(publishedModel, null) ? CreateOKResponse(new PublishedResponse { PublishedModel = publishedModel }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new PublishedResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new PublishedResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }
        #endregion
        #endregion
    }
}
