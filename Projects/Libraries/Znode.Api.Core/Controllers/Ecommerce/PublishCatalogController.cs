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
    public class PublishCatalogController : BaseController
    {
        #region Private Variables

        private readonly IPublishCatalogCache _cache;

        #endregion

        #region Constructor
        public PublishCatalogController(IPublishCatalogService service)
        {
            _cache = new PublishCatalogCache(service);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets list of Publish Catalogs.
        /// </summary>
        /// <returns>Return publish catalog list.</returns>
        [ResponseType(typeof(PublishCatalogListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage List()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetPublishCatalogList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PublishCatalogListResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new PublishCatalogListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PublishCatalogListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Error);
            }

            return response;
        }


        /// <summary>
        /// Get publish catalog excluding assigned ids.
        /// </summary>
        /// <param name="assignedIds">Already assigned Ids</param>
        /// <returns>Return publish catalog list.</returns>
        [ResponseType(typeof(PublishCatalogListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage UnassignedList(string assignedIds)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetUnAssignedPublishCatelogList(assignedIds, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PublishCatalogListResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new PublishCatalogListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PublishCatalogListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Get  Publish Catalog by  Publish Catalog id.
        /// </summary>
        /// <param name="publishCatalogId"> Publish Catalog id to get  Publish Catalog details.</param>
        /// <param name="localeId"> localeId to get locale Specific Publish Catalog details.</param> 
        /// <returns>Return catalog.</returns>
        [ResponseType(typeof(PublishCatalogResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetPublishCatalog(int publishCatalogId, int? localeId = null)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetPublishCatalog(publishCatalogId, localeId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PublishCatalogResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new PublishCatalogResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PublishCatalogResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Marketing.ToString(), TraceLevel.Error);
            }

            return response;
        }
    }
    #endregion
}