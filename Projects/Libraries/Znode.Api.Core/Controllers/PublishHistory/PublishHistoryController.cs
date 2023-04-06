using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using Znode.Engine.Exceptions;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Api.Controllers
{
    public class PublishHistoryController : BaseController
    {
        #region Private Variables
        private readonly IPublishHistoryCache _publishHistoryCache;
        private readonly IPublishHistoryService _publishHistoryService;
        #endregion

        #region Constructor
        public PublishHistoryController(IPublishHistoryService publishHistoryService)
        {
            _publishHistoryService = publishHistoryService;
            _publishHistoryCache = new PublishHistoryCache(_publishHistoryService);
        }
        #endregion

        #region Public

        /// <summary>
        /// Ge the list of Publish History.
        /// </summary>
        /// <returns>HttpResponseMessage</returns>
        [ResponseType(typeof(PublishHistoryListResponse))]
        [HttpGet]
        public HttpResponseMessage List()
        {
            HttpResponseMessage response;
            try
            {
                string data = _publishHistoryCache.GetPublishHistoryList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PublishHistoryListResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new PublishHistoryListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PublishHistoryListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Deleting product logs 
        /// </summary>
        /// <param name="versionId">versionId</param>
        /// <returns>bool</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public HttpResponseMessage Delete(int versionId)
        {
            HttpResponseMessage response;
            try
            {
                _publishHistoryService.DeleteProductLogs(versionId);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = true });
            }
            catch (Exception ex)
            {
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message };
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }
        #endregion
    }
}
