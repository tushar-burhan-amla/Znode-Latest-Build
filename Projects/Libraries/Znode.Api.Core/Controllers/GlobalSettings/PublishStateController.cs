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

namespace Znode.Engine.Api.Controllers
{
    public class PublishStateController : BaseController
    {
        #region Private Variables
        private readonly IPublishStateService _service;
        private readonly IPublishStateCache _cache;
        #endregion

        #region Default Constructor
        public PublishStateController(IPublishStateService service)
        {
            _service = service;
            _cache = new PublishStateCache(_service);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Gets list of all available publish state mappings.
        /// </summary>
        /// <returns>List of all available publish state to application type mappings.</returns>
        [ResponseType(typeof(PublishStateMappingListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage List()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetPublishStateMappingList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<PublishStateMappingListResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new PublishStateMappingListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(),TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PublishStateMappingListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Enable/Disable publish state to application type mapping.
        /// </summary>
        /// <param name="mappingId">PublishStateMappingId</param>
        /// <param name="isEnabled">Supply true to enable.</param>
        /// <returns></returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPut]
        public HttpResponseMessage EnableDisableMapping([FromBody] int mappingId, bool isEnabled)
        {
            HttpResponseMessage response;
            try
            {
                bool resp = _service.EnableDisableMapping(mappingId, isEnabled);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = resp });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.GlobalSettings.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }
        #endregion
    }
}
