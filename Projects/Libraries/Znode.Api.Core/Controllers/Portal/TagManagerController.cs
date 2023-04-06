using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Controllers;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using Znode.Libraries.Framework.Business;

namespace Znode.Api.Core.Controllers
{
    public class TagManagerController : BaseController
    {
        #region Private Variables
        private readonly ITagManagerService _tagManagerService;
        private readonly ITagManagerCache _tagManagerCache;
        #endregion

        #region Public Constructor
        public TagManagerController(ITagManagerService service)
        {
            _tagManagerService = service;
            _tagManagerCache = new TagManagerCache(_tagManagerService);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Get tag manager data by portal id.
        /// </summary>
        /// <param name="portalId">PortalId to get tag manager data.</param>
        /// <returns>Returns tag manager information.</returns>
        [ResponseType(typeof(TagManagerResponse))]
        [HttpGet]
        public virtual HttpResponseMessage Get(int portalId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _tagManagerCache.GetTagManager(portalId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<TagManagerResponse>(data) : CreateNotFoundResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                TagManagerResponse data = new TagManagerResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Save tag manager data.
        /// </summary>
        /// <param name="tagManagerModel">Model with tag manager data.</param>
        /// <returns>Returns tag manager information.</returns>
        [ResponseType(typeof(TagManagerResponse))]
        [HttpPut]
        public virtual HttpResponseMessage Save([FromBody] TagManagerModel tagManagerModel)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _tagManagerService.SaveTagManager(tagManagerModel) });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Setup.ToString(), TraceLevel.Error);
            }
            return response;
        }
        #endregion
    }
}
