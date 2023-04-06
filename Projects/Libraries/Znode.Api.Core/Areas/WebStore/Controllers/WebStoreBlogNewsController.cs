using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Api.Controllers
{
    public class WebStoreBlogNewsController : BaseController
    {
        #region Private Variables
        private readonly IBlogNewsCache _cache;
        private readonly IBlogNewsService _service;
        #endregion

        #region Constructor
        public WebStoreBlogNewsController(IBlogNewsService service)
        {
            _service = service;
            _cache = new BlogNewsCache(_service);
        }
        #endregion

        /// <summary>
        /// Get list of blogs/news.
        /// </summary>
        /// <returns>Returns list of blog/news.</returns>
        [ResponseType(typeof(WebStoreBlogNewsListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetBlogNewsListForWebstore()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetBlogNewsListForWebstore(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<string>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex,ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                WebStoreBlogNewsListResponse data = new WebStoreBlogNewsListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                WebStoreBlogNewsListResponse data = new WebStoreBlogNewsListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Get blog/news details by blog/newsId and localeId.
        /// </summary>
        /// <param name="blogNewsId">Blog/News Id.</param>
        /// <param name="localeId">Locale Id.</param>
        /// <param name="portalId"></param>
        /// <returns>Returns blog/news model.</returns>
        [ResponseType(typeof(WebStoreBlogNewsResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetBlogNewsForWebstore(int blogNewsId, int localeId, int portalId, string activationDate = null)
        {
            HttpResponseMessage response;

            try
            {
                //Get blog/news by id.
                string data = _cache.GetBlogNewsForWebstore(blogNewsId, localeId, portalId, RouteUri, RouteTemplate, activationDate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<string>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new WebStoreBlogNewsResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new WebStoreBlogNewsResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Save new blog/news comment.
        /// </summary>
        /// <param name="model">WebStoreBlogNewsCommentModel.</param>
        /// <returns>Return saved blog/news comments.</returns>
        [ResponseType(typeof(WebStoreBlogNewsCommentResponse))]
        [HttpPost]
        public virtual HttpResponseMessage SaveComments([FromBody] WebStoreBlogNewsCommentModel model)
        {
            HttpResponseMessage response;
            try
            {
                WebStoreBlogNewsCommentModel blogNewsComment = _service.SaveComments(model);

                if (!Equals(blogNewsComment, null))
                {
                    response = CreateCreatedResponse(new WebStoreBlogNewsCommentResponse { BlogNewsComment = blogNewsComment });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(blogNewsComment.BlogNewsId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new WebStoreBlogNewsCommentResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new WebStoreBlogNewsCommentResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get Blog/News comment list.
        /// </summary>
        /// <returns>Returns Blog/News comment List.</returns>
        [ResponseType(typeof(WebStoreBlogNewsCommentListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetUserCommentList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetUserCommentList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<string>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new WebStoreBlogNewsCommentListResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }
    }
}
