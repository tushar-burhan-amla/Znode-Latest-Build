using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Helper;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Api.Controllers
{
    public class SMSController : BaseController
    {
        #region Private Variables
        private readonly ISMSCache _cache;
        private readonly ISMSService _service;
        #endregion

        #region Constructor
        public SMSController(ISMSService service)
        {
            _service = service;
            _cache = new SMSCache(_service);
        }
        #endregion

        #region Public Methods

        /// <summary>
        /// Get SMTP details for specified portal ID
        /// </summary>
        /// <param name="portalId">Int PortalId to get smtp details</param>
        /// <param name="isSMSSettingEnabled"></param>
        /// <returns>Smtp details as response</returns>
        [ResponseType(typeof(SMSResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetSMSDetails(int portalId, bool isSMSSettingEnabled = false)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetSMSDetails(portalId, RouteUri, RouteTemplate,isSMSSettingEnabled);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<SMSResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                SMSResponse smtpResponse = new SMSResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(smtpResponse);
            }
            return response;
        }

        /// <summary>
        /// Update SMS details.
        /// </summary>
        /// <param name = "smsModel" > SMSModel to update existing sms</param>
        /// <returns>SMS Response</returns>
        [ResponseType(typeof(SMSResponse))]
        [HttpPut]
        [ValidateModel]
        public virtual HttpResponseMessage InsertUpdateSMSSetting([FromBody] SMSModel smsModel)
        {
            HttpResponseMessage response;
            try
            {
                bool isUpdated = _service.InsertUpdateSMSSetting(smsModel);
                response = isUpdated ? CreateOKResponse(new SMSResponse { Sms = smsModel }) : CreateInternalServerErrorResponse();
            }
            catch (Exception ex)
            {
                SMSResponse smsResponse = new SMSResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(smsResponse);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get SMTP details for specified portal ID
        /// </summary>
        /// <param name="portalId">Int PortalId to get smtp details</param>
        /// <returns>Smtp details as response</returns>
        [ResponseType(typeof(SMSResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetSmsProviderList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetSmsProviderList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<SMSResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                SMSResponse smtpResponse = new SMSResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(smtpResponse);
            }
            return response;
        }
        #endregion
    }
}
