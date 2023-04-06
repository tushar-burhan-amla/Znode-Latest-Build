using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Helper;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Api.Controllers
{
    public class EmailTemplatesController : BaseController
    {
        private readonly IEmailTemplateService _service;
        private readonly IEmailTemplateCache _cache;

        public EmailTemplatesController(IEmailTemplateService service)
        {
            _service = service;
            _cache = new EmailTemplateCache(_service);
        }

        /// <summary>
        /// Gets a list of EmailTemplates.
        /// </summary>
        /// <returns>Returns email template list. </returns>        
        [ResponseType(typeof(EmailTemplateListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage List()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetEmailTemplates(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<EmailTemplateListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new EmailTemplateListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Creates a new email template Page.
        /// </summary>
        /// <param name="emailTemplateModel">The model of the Email template Page.</param>
        /// <returns>Return email template.</returns>
        [ResponseType(typeof(EmailTemplateResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage Create([FromBody] EmailTemplateModel emailTemplateModel)
        {
            HttpResponseMessage response;

            try
            {
                EmailTemplateModel templatePage = _service.CreateTemplatePage(emailTemplateModel);
                if (HelperUtility.IsNotNull(templatePage))
                {                  
                    response = CreateCreatedResponse(new EmailTemplateResponse { EmailTemplate = templatePage });                    
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(templatePage.EmailTemplateId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new EmailTemplateResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new EmailTemplateResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Gets an email template page.
        /// </summary>
        /// <param name="emailTemplateId">ID of the email Template.</param>
        /// <returns> Return Email Template.</returns>
        [ResponseType(typeof(EmailTemplateResponse))]
        [HttpGet]
        public virtual HttpResponseMessage Get(int emailTemplateId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetTemplatePage(emailTemplateId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<EmailTemplateResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new EmailTemplateResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new EmailTemplateResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Updates an existing email template page.
        /// </summary>        
        /// <param name="emailTemplateModel">EmailTemplateModel</param>
        /// <returns>Return Email Template.</returns>
        [ResponseType(typeof(EmailTemplateResponse))]
        [HttpPut, ValidateModel]
        public virtual HttpResponseMessage Update([FromBody] EmailTemplateModel emailTemplateModel)
        {
            HttpResponseMessage response;
            try
            {
                //Update email template.
                bool isUpdated = _service.UpdateTemplatePage(emailTemplateModel);
                response = isUpdated ? CreateOKResponse(new EmailTemplateResponse { EmailTemplate = emailTemplateModel }) : CreateInternalServerErrorResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(emailTemplateModel.EmailTemplateId)));
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new EmailTemplateResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });              
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new EmailTemplateResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Delete Email Template.
        /// </summary>
        /// <param name="emailTemplateId">Email Template Id.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage Delete([FromBody] ParameterModel emailTemplateId)
        {
            HttpResponseMessage response;
            try
            {
                bool deleted = _service.DeleteTemplatePage(emailTemplateId);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);              
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Gets an email template tokens.
        /// </summary>
        /// <returns>Return Email Template.</returns>
        [ResponseType(typeof(EmailTemplateResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetEmailTemplateTokens()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetEmailTemplateTokens(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<EmailTemplateResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new EmailTemplateResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Gets a list of EmailTemplates Areas.
        /// </summary>
        /// <returns>Return email template area list. </returns>        
        [ResponseType(typeof(EmailTemplateAreaListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage EmailTemplateAreaList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetEmailTemplateAreaList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<EmailTemplateAreaListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new EmailTemplateAreaListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(),TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Gets a list of EmailTemplates Areas Mapper Details.
        /// </summary>
        /// <returns>Return email template area mapper list.</returns>        
        [ResponseType(typeof(EmailTemplateAreaMapperListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage EmailTemplateAreaMapperList(int portalId)
        {
            HttpResponseMessage response;
            try
            {
                EmailTemplateAreaMapperListModel data = _service.GetEmailTemplateAreaMapperList(portalId);
                response = HelperUtility.IsNotNull(data) ? CreateOKResponse(new EmailTemplateAreaMapperListResponse { EmailTemplateAreaMapper = data.EmailTemplatesAreaMapperList }) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new EmailTemplateAreaListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Delete Email Template Area configuration by id.
        /// </summary>
        /// <param name="areaMappingId">id to delete email Template Area configuration.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage DeleteEmailTemplateAreaConfiguration(ParameterModel areaMappingId)
        {
            HttpResponseMessage response;
            try
            {
                bool deleted = _service.DeleteEmailTemplateAreaConfiguration(areaMappingId);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Create or Update the email template area configuration.
        /// </summary>
        /// <param name="model">model with email template area configuration data</param>
        /// <returns>Return saved email template if true else false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage SaveEmailTemplateAreaConfiguration([FromBody] EmailTemplateAreaMapperModel model)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateCreatedResponse(new TrueFalseResponse { IsSuccess = _service.SaveEmailTemplateAreaConfiguration(model), ErrorCode = 0 });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

    }
}