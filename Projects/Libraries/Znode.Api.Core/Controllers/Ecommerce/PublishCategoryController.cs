using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Api.Controllers.Ecommerce
{
    public class PublishCategoryController : BaseController
    {
        #region Private Variables

        private readonly IPublishCategoryCache _cache;
        #endregion

        #region Constructor
        public PublishCategoryController(IPublishCategoryService service)
        {
            _cache = new PublishCategoryCache(service);
        }
        #endregion

        #region public virtual Methods
        /// <summary>
        /// Gets list of Publish Categories.
        /// </summary>
        /// <returns>Returns list.</returns>
        [ResponseType(typeof(PublishCategoryListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage List()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetPublishCategoryList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PublishCategoryListResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new PublishCategoryListResponse { HasError = true, ErrorMessage = ex.Message,ErrorCode=ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PublishCategoryListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Get publish category excluding assigned ids.
        /// </summary>
        /// <param name="assignedIds">Already assigned Ids</param>
        /// <returns>Returns list.</returns>
        [ResponseType(typeof(PublishCategoryListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage UnassignedList(string assignedIds)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetUnAssignedPublishCategoryList(assignedIds, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PublishCategoryListResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new PublishCategoryListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PublishCategoryListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Get  Publish Category by  Publish Category id.
        /// </summary>
        /// <param name="publishCategoryId"> Publish Category id to get  Publish Category details.</param>
        /// <returns>Returns publish category.</returns>
        [ResponseType(typeof(PublishCategoryResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetPublishCategory(int publishCategoryId)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetPublishCategory(publishCategoryId,RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PublishCategoryResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new PublishCategoryResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PublishCategoryResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Error);
            }



            return response;
        }
    }
    #endregion
}