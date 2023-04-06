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
    public class FormSubmissionController : BaseController
    {
        #region Private Variables
        private readonly IFormSubmissionCache _cache;
        #endregion

        #region Constructor
        public FormSubmissionController(IFormSubmissionService service)
        {
            _cache = new FormSubmissionCache(service);
        }
        #endregion

        #region Public Method
        /// <summary>
        /// Get list of form submission.
        /// </summary>
        /// <returns>Returns list of form submission.</returns>
        [HttpGet]
        [ResponseType(typeof(FormSubmissionListResponse))]
        public virtual HttpResponseMessage List()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetFormSubmissionList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<FormSubmissionListResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new FormSubmissionListResponse { HasError = true, ErrorMessage = ex.Message });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new FormSubmissionListResponse { HasError = true, ErrorMessage = ex.Message });
            }

            return response;
        }

        /// <summary>
        ///  Get Form submit Details.
        /// </summary>
        /// <param name="formSubmitId">Int form submit Id</param>
        /// <returns>Returns Form Builder Attribute Group Response.</returns>
        [ResponseType(typeof(FormBuilderAttributeGroupResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetFormSubmitDetails(int formSubmitId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetFormSubmitDetails(formSubmitId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<FormBuilderAttributeGroupResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new FormBuilderAttributeGroupResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new FormBuilderAttributeGroupResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        ///  Get Export Response Message of Form Submission.
        /// </summary>
        /// <returns>Return Export Response Message of Form Submission.</returns>
        [ResponseType(typeof(ExportResponse))]
        [HttpGet]
        public virtual HttpResponseMessage Formsubmissionexportlist(string exportType)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetFormSubmissionListforExport(RouteUri, RouteTemplate, exportType);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ExportResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                ProductListResponse data = new ProductListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }


        #endregion

    }
}
