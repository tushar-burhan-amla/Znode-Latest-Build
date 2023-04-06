using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Controllers;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Api.Areas.WebStore.Controllers
{
    public class WebStoreMessageController : BaseController
    {
        #region Private Variables
        private readonly IWebStoreMessageCache _cache;
        #endregion

        #region Constructor
        public WebStoreMessageController(IManageMessageService service)
        {
            _cache = new WebStoreMessageCache(service);
        }
        #endregion

        /// <summary>
        /// Get message by message key and portal id.
        /// </summary>
        /// <returns>Returns manage message model.</returns>
        [ResponseType(typeof(WebStoreMessageResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetMessage()
        {
            HttpResponseMessage response;

            try
            {
                //Get messsage by Message Key, Area and Portal Id.
                string data = _cache.GetMessage(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<string>(data) : CreateNoContentResponse();
            }

            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Webstore.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new WebStoreMessageResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Get list of messages by locale id and portal id.
        /// </summary>
        /// <param name="localeId">Current Locale Id.</param>
        /// <returns>Returns manage message model.</returns>
        [ResponseType(typeof(WebStoreMessageListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage List(int localeId)
        {
            HttpResponseMessage response;
            try
            {
                //Get messsages by Area and Portal Id.
                string data = _cache.GetMessages(RouteUri, RouteTemplate, localeId);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<string>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Warning);
                WebStoreMessageListResponse data = new WebStoreMessageListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                WebStoreMessageListResponse data = new WebStoreMessageListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }
    }
}