using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Net.Http;
using System.Web;
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
     public  class ContentContainerController : BaseController
    {

        #region Private Variables
        private readonly IContentContainerCache _cache;
        private readonly IContentContainerService _service;
        private readonly IPublishContainerDataService _publishContainerDataService;
        #endregion

        #region Constructor
        public ContentContainerController(IContentContainerService service, IPublishContainerDataService publishContainerDataService)
        {
            _service = service;
            _publishContainerDataService = publishContainerDataService;
            _cache = new ContentContainerCache(_service);
        }
        #endregion


        /// <summary>
        /// List of Content Containers
        /// </summary>
        /// <returns>ContentContainerListResponseModel</returns>
        [ResponseType(typeof(ContentContainerListResponseModel))]
        [HttpGet]
        public virtual HttpResponseMessage List()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.List(RouteUri, RouteTemplate);

                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ContentContainerListResponseModel>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                ContentContainerListResponseModel data = new ContentContainerListResponseModel { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Create List of Content Containers
        /// </summary>
        /// <param name="model">ContentContainerCreateModel model</param>
        /// <returns>ContentContainerResponseModel model</returns>
        [ResponseType(typeof(ContentContainerResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage Create([FromBody] ContentContainerCreateModel model)
        {
            HttpResponseMessage response;
            try
            {
                ContentContainerResponseModel containerModel = _service.Create(model);
                response = !Equals(containerModel, null) ? CreateCreatedResponse(new ContentContainerResponse { ContentContainerModel = containerModel }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                ContentContainerResponse data = new ContentContainerResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode };
                response = CreateInternalServerErrorResponse(data);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                ContentContainerResponse data = new ContentContainerResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }
        /// <summary>
        /// Update Content Containers
        /// </summary>
        /// <param name="model">ContentContainerUpdateModel model</param>
        /// <returns>ContentContainerResponseModel model</returns>
        [HttpPut]
        [ResponseType(typeof(ContentContainerResponse))]
        public virtual HttpResponseMessage Update([FromBody] ContentContainerUpdateModel model)
        {
            HttpResponseMessage response;
            try
            {
                ContentContainerResponseModel containerModel = _service.Update(model);
                response = !Equals(containerModel, null) ? CreateCreatedResponse(new ContentContainerResponse { ContentContainerModel = containerModel, ErrorCode = 0 }) : CreateInternalServerErrorResponse();
            }

            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new ContentContainerResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new ContentContainerResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Get Content Container
        /// </summary>
        /// <param name="containerKey">containerKey</param>
        /// <returns>ContentContainerResponseModel model</returns>
        [HttpGet]
        [ResponseType(typeof(ContentContainerResponse))]
        public virtual HttpResponseMessage GetContentContainer(string containerKey)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetContentContainer(containerKey, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ContentContainerResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new ContentContainerResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new ContentContainerResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }


        /// <summary>
        /// Get Associated Variants
        /// </summary>
        /// <param name="containerKey">containerKey</param>
        /// <returns>List Of Variants</returns>
        [HttpGet]
        [ResponseType(typeof(ContentContainerListResponseModel))]
        public virtual HttpResponseMessage GetAssociatedVariants(string containerKey)
        {
            HttpResponseMessage response;

            try
            {
                string variants = _cache.GetAssociatedVariants(containerKey, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(variants) ? CreateOKResponse<ContentContainerListResponseModel>(variants) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new ContentContainerListResponseModel { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new ContentContainerListResponseModel { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Associate Variants
        /// </summary>
        /// <param name="model">AssociatedVariantModel model</param>
        /// <returns>List of AssociatedVariantModel</returns>
        [ResponseType(typeof(ContentContainerListResponseModel))]
        [HttpPost]
        public virtual HttpResponseMessage AssociateVariant(AssociatedVariantModel model)
        {
            HttpResponseMessage response;
            try
            {
                List<AssociatedVariantModel> associatedVariants  = _service.AssociateVariant(model);
                response = !Equals(associatedVariants, null) ? CreateCreatedResponse(new ContentContainerListResponseModel { AssociatedVariants = associatedVariants, ErrorCode = 0 }) : CreateInternalServerErrorResponse();

            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new ContentContainerListResponseModel { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new ContentContainerListResponseModel { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Delete Content Containers
        /// </summary>
        /// <param name="ContentContainerIds">ContentContainerIds</param>
        /// <returns>Status</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage DeleteContentContainer(ParameterModel ContentContainerIds)
        {
            HttpResponseMessage response;
            try
            {
                bool IsDeleted = _service.DeleteContentContainer(ContentContainerIds);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = IsDeleted });
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
        /// Delete Content Container By Container Key
        /// </summary>
        /// <param name="containerKey">containerKey</param>
        /// <returns>Response</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage DeleteContentContainerByContainerKey(string containerKey)
        {
            HttpResponseMessage response;
            try
            {
                bool IsDeleted = _service.DeleteContentContainerByContainerKey(containerKey);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = IsDeleted });
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
        /// Delete Associated Variants
        /// </summary>
        /// <param name="variantId">variantId</param>
        /// <returns>Status</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage DeleteAssociatedVariant(ParameterModel variantIds)
        {
            HttpResponseMessage response;
            try
            {
                bool IsDeleted = _service.DeleteAssociatedVariant(variantIds);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = IsDeleted });
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
        /// Verify if the Container Key Exist
        /// </summary>
        /// <param name="containerKey">Container Key</param>
        /// <returns>status</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpGet]
        public virtual HttpResponseMessage IsContainerKeyExists(string containerKey)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.IsContainerKeyExists(containerKey) });
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

        /// <summary>
        /// Associate Container Template
        /// </summary>
        /// <param name="variantId">variantId</param>
        /// <param name="containerTemplateId">containerTemplateId</param>
        /// <returns></returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPut]
        public virtual HttpResponseMessage AssociateContainerTemplate(int variantId, int containerTemplateId)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.AssociateContainerTemplate(variantId, containerTemplateId) });
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

        /// <summary>
        /// Get the List of Associated Variants
        /// </summary>
        /// <returns>ContentContainerListResponseModel</returns>
        [ResponseType(typeof(ContentContainerListResponseModel))]
        [HttpGet]
        public virtual HttpResponseMessage GetAssociatedVariantList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetAssociatedVariantList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ContentContainerListResponseModel>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                ContentContainerListResponseModel data = new ContentContainerListResponseModel { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Associate container variant data
        /// </summary>
        /// <param name="localeId">localeId</param>
        /// <param name="variantId">variantId</param>
        /// <param name="templateId">Container TemplateId</param>
        /// <param name="isActive">IsActive</param>
        /// <returns></returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPut]
        public virtual HttpResponseMessage SaveVariantData(int localeId, int? templateId, int variantId, bool isActive)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.SaveVariantData(localeId, templateId, variantId, isActive) });
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

        /// <summary>
        /// Get Content Container Attribute Data
        /// </summary>
        /// <param name="containerKey">containerKey</param>
        /// <param name="localeId">localeId</param>
        /// <param name="portalId">portalId</param>
        /// <param name="profileId">profileId</param>
        /// <returns>ContentContainerDataResponseModel model</returns>
        [HttpGet]
        [ResponseType(typeof(ContentContainerDataResponse))]
        public virtual HttpResponseMessage GetContentContainerData(string containerKey, int localeId, int portalId = 0, int profileId = 0)
        {

            HttpResponseMessage response;
            try
            {
                //Get attributes data.
                string data = _cache.GetContentContainerData(RouteUri, RouteTemplate, HttpUtility.UrlDecode(containerKey), localeId, portalId, profileId);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ContentContainerDataResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Admin.ToString(), TraceLevel.Error);
                ContentContainerDataResponse data = new ContentContainerDataResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(data);
            }
            return response;
        }

        /// <summary>
        /// Get Content Container Variant locale data
        /// </summary>
        /// <param name="variantId">variantId</param>
        /// <returns>ContentContainerResponseModel model</returns>
        [HttpGet]
        [ResponseType(typeof(ContentContainerResponse))]
        public virtual HttpResponseMessage GetVariantLocaleData(int variantId)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetVariantLocaleData(variantId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ContentContainerResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new ContentContainerResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new ContentContainerResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }

        /// <summary>
        /// Activate/Deactivate Variants
        /// </summary>
        /// <param name="variantIds">variantIds</param>
        /// <param name="isActivate">isActivate</param>
        /// <returns>Status</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage ActivateDeactivateVariant(ParameterModel variantIds, bool isActivate)
        {
            HttpResponseMessage response;
            try
            {
                bool IsUpdated = _service.ActivateDeactivateVariant(variantIds, isActivate);
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = IsUpdated });
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
        /// Publish Content Container
        /// </summary>
        /// <param name="containerKey">containerKey</param>
        /// <param name="targetPublishState">targetPublishState</param>
        /// <returns>Status</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage PublishContentContainer(string containerKey, string targetPublishState)
        {
            HttpResponseMessage response;
            try
            {
                PublishedModel publishedModel = _publishContainerDataService.PublishContentContainer(containerKey, targetPublishState);
                response = !Equals(publishedModel, null) ? CreateOKResponse(new PublishedResponse { PublishedModel = publishedModel }) : CreateInternalServerErrorResponse();
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
        /// Publish Content Container Variant
        /// </summary>
        /// <param name="containerKey">containerKey</param>
        /// <param name="containerProfileVariantId">containerProfileVariantId</param>
        /// <param name="targetPublishState">targetPublishState</param>
        /// <returns>Status</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage PublishContainerVariant(string containerKey, int containerProfileVariantId, string targetPublishState)
        {
            HttpResponseMessage response;
            try
            {
                PublishedModel publishedModel = _publishContainerDataService.PublishContainerVariant(containerKey, containerProfileVariantId, targetPublishState);
                response = !Equals(publishedModel, null) ? CreateOKResponse(new PublishedResponse { PublishedModel = publishedModel }) : CreateInternalServerErrorResponse();
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
    }

}

