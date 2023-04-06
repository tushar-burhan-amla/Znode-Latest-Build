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
    public class TemplateController : BaseController
    {
        #region Private Variables
        private readonly ITemplateCache _cache;
        private readonly ITemplateService _service;
        #endregion

        #region Constructor
        public TemplateController(ITemplateService service)
        {
            _service = service;
            _cache = new TemplateCache(_service);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Get the templates list.
        /// </summary>
        /// <returns>Returns templates list.</returns>
        [ResponseType(typeof(TemplateListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage List()
        {
            HttpResponseMessage response;

            try
            {
                //Get list of templates.
                string data = _cache.GetTemplates(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<TemplateListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TemplateListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Gets the existing template.
        /// </summary>
        /// <param name="cmsTemplateId">Template id.</param>
        /// <returns>Returns template list.</returns>
        [ResponseType(typeof(TemplateListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage Get(int cmsTemplateId)
        {
            HttpResponseMessage response;

            try
            {
                //Get template by id.
                string data = _cache.GetTemplate(cmsTemplateId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<TemplateListResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TemplateListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TemplateListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Create the template.
        /// </summary>
        /// <param name="model">Template model to create.</param>
        /// <returns>Returns created model.</returns>
        [ResponseType(typeof(TemplateListResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage Create([FromBody] TemplateModel model)
        {
            HttpResponseMessage response;
            try
            {
                TemplateModel data = _service.CreateTemplate(model);
                if (HelperUtility.IsNotNull(data))
                {
                    response = CreateCreatedResponse(new TemplateListResponse { Template = data });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(data.CMSTemplateId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TemplateListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TemplateListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Update Template.
        /// </summary>
        /// <param name="model">model to update.</param>
        /// <returns>Returns updated model.</returns>
        [ResponseType(typeof(TemplateListResponse))]
        [HttpPut, ValidateModel]
        public virtual HttpResponseMessage Update([FromBody] TemplateModel model)
        {
            HttpResponseMessage response;
            try
            {
                //Update template.
                response = _service.UpdateTemplate(model) ? CreateOKResponse(new TemplateListResponse { Template = model, ErrorCode = 0 }) : CreateInternalServerErrorResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(model.CMSTemplateId)));
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new TemplateListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new TemplateListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Delete CMS template.
        /// </summary>
        /// <param name="cmsTemplateId">CMS template Id.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage Delete([FromBody] ParameterModel cmsTemplateId)
        {
            HttpResponseMessage response;

            try
            {
                //Delete templates.
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.DeleteTemplate(cmsTemplateId) });
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
        #endregion
    }
}