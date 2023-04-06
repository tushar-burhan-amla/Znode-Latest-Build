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
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Api.Controllers
{

    public class ContainerTemplateController : BaseController
    {

        #region Private Variables
        private readonly IContainerTemplateCache _cache;
        private readonly IContainerTemplateService _service;
        #endregion

        #region Constructor
        public ContainerTemplateController(IContainerTemplateService service)
        {
            _service = service;
            _cache = new ContainerTemplateCache(_service);
        }
        #endregion

        /// <summary>
        /// Get the List of Container Template
        /// </summary>
        /// <returns>ContainerTemplateListResponse model</returns>
        [ResponseType(typeof(ContainerTemplateListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage List()
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.List(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ContainerTemplateListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ContainerTemplateListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Create Container Template
        /// </summary>
        /// <param name="model">ContainerTemplateCreateModel model</param>
        /// <returns>ContainerTemplateModel model</returns>
        [ResponseType(typeof(ContainerTemplateListResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage Create([FromBody] ContainerTemplateCreateModel model)
        {
            HttpResponseMessage response;
            try
            {
                ContainerTemplateModel containerModel = _service.CreateContainerTemplate(model);
                response = !Equals(containerModel, null) ? CreateCreatedResponse(new ContainerTemplateListResponse { ContainerTemplate = containerModel }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                ContainerTemplateListResponse data = new ContainerTemplateListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                ContainerTemplateListResponse data = new ContainerTemplateListResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Get Container Template
        /// </summary>
        /// <param name="templateCode">tempateCode</param>
        /// <returns>ContainerTemplateModel model</returns>
        [ResponseType(typeof(ContainerTemplateListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage Get(string templateCode)
        {
            HttpResponseMessage response;

            try
            {
                //Get template by id.
                string data = _cache.GetContainerTemplate(templateCode, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<TemplateListResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new ContainerTemplateListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ContainerTemplateListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Update Container Template
        /// </summary>
        /// <param name="model">ContainerTemplateUpdateModel model</param>
        /// <returns>ContainerTemplateModel model</returns>
        [ResponseType(typeof(ContainerTemplateListResponse))]
        [HttpPut, ValidateModel]
        public virtual HttpResponseMessage Update([FromBody] ContainerTemplateUpdateModel model)
        {
            HttpResponseMessage response;
            try
            {
                ContainerTemplateModel containerTemplate = _service.UpdateContainerTemplate(model);
                response = !Equals(containerTemplate, null) ? CreateCreatedResponse(new ContainerTemplateListResponse { ContainerTemplate = containerTemplate, ErrorCode = 0 }) : CreateInternalServerErrorResponse();

            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new ContainerTemplateListResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ContainerTemplateListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Delete Container Template
        /// </summary>
        /// <param name="ContainerTemplateIds">Container Template Ids</param>
        /// <returns>status</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage Delete(ParameterModel ContainerTemplateIds)
        {
            HttpResponseMessage response;
            try
            {
                bool deleted = _service.DeleteContainerTemplateById(ContainerTemplateIds);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Delete Container Template
        /// </summary>
        /// <param name="templateCode">templateCode</param>
        /// <returns>status</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage DeleteTemplateByCode(string templateCode)
        {
            HttpResponseMessage response;
            try
            {
                bool deleted = _service.DeleteContainerTemplateByCode(templateCode);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = deleted });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Validate if the Container Template exists
        /// </summary>
        /// <param name="templateCode">Container Template code</param>
        /// <returns>status</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpGet]
        public virtual HttpResponseMessage IsContainerTemplateExist(string templateCode)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.IsContainerTemplateExists(templateCode) });
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                TrueFalseResponse data = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
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
