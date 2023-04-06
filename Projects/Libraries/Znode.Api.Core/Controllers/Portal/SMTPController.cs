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
    public class SMTPController : BaseController
    {
        #region Private Variables
        private readonly ISMTPCache _cache;
        private readonly ISMTPService _service;
        #endregion

        #region Constructor
        public SMTPController(ISMTPService service)
        {
            _service = service;
            _cache = new SMTPCache(_service);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Get SMTP details for specified portal ID
        /// </summary>
        /// <param name="portalId">Int PortalId to get smtp details</param>
        /// <returns>Smtp details as response</returns>
        [ResponseType(typeof(SMTPResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetSMTP(int portalId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetSMTP(portalId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<SMTPResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                SMTPResponse smtpResponse = new SMTPResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(smtpResponse);
            }
            return response;
        }

        /// <summary>
        /// Update SMTP details.
        /// </summary>
        /// <param name = "smtpModel" > SMTPModel to update existing smtp</param>
        /// <returns>SMTP Response</returns>
        [ResponseType(typeof(SMTPResponse))]
        [HttpPut]
        [ValidateModel]
        public virtual HttpResponseMessage Update([FromBody] SMTPModel smtpModel)
        {
            HttpResponseMessage response;
            try
            {
                bool isUpdated = _service.UpdateSMTP(smtpModel);
                response = isUpdated ? CreateOKResponse(new SMTPResponse { Smtp = smtpModel }) : CreateInternalServerErrorResponse();
            }
            catch (Exception ex)
            {
                SMTPResponse smtpResponse = new SMTPResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(smtpResponse);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
            return response;
        }


        /// <summary>
        /// Send test Email
        /// </summary>
        /// <param name = "smtpModel" > SMTPModel to send email</param>
        /// <returns>Email model Response</returns>
        [ResponseType(typeof(EmailResponse))]
        [HttpPost]
        [ValidateModel]
        public virtual HttpResponseMessage SendEmail([FromBody] EmailModel emailModel)
        {
            HttpResponseMessage response;
            try
            {
                bool isSendEmail = _service.SendEmail(emailModel);
                response = isSendEmail ? CreateOKResponse(new EmailResponse { EmailModel = emailModel }) : CreateInternalServerErrorResponse();
            }
            catch (Exception ex)
            {
                EmailResponse emailResponse = new EmailResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(emailResponse);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
            return response;
        }
    }
    #endregion
}

