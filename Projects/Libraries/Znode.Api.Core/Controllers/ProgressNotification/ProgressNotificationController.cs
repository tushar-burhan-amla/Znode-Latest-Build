using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Api.Controllers
{
    public class ProgressNotificationController : BaseController
    {
        #region Private Variables
        private readonly IProgressNotificationService _service;
        #endregion

        #region Constructor
        public ProgressNotificationController(IProgressNotificationService service)
        {
            _service = service;
        }
        #endregion

        #region Public Methods
        
        /// <summary>
        /// Get all progress notifications.
        /// </summary>
        /// <returns></returns>
        [ResponseType(typeof(ProgressNotificationResponse))]
        [HttpGet]
        public virtual async Task<HttpResponseMessage> GetProgressNotifications()
        {
            HttpResponseMessage response;

            try
            {                
                List<ProgressNotificationModel> data = await _service.GetProgressNotifications();
                response = HelperUtility.IsNotNull(data) ? CreateOKResponse(new ProgressNotificationResponse { ProgressNotifications = data}) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new ProgressNotificationResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(),TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ProgressNotificationResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(),TraceLevel.Error);
            }
            return response;
        }

        #endregion
    }
}
