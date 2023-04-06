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
    public class ManageMessageController : BaseController
    {
        #region Private Variables
        private readonly IManageMessageCache _cache;
        private readonly IManageMessageService _service;
        #endregion

        #region Constructor
        public ManageMessageController(IManageMessageService service)
        {
            _service = service;
            _cache = new ManageMessageCache(_service);
        }
        #endregion

        #region Public Methods
        #region Manage Message
        /// <summary>
        /// Create ManageMessage.
        /// </summary>
        /// <param name="ManageMessageModel">ManageMessageModel.</param>
        /// <returns>Returns created ManageMessage.</returns>
        [ResponseType(typeof(ManageMessageResponse))]
        [HttpPost]
        public virtual HttpResponseMessage CreateManageMessage([FromBody] ManageMessageModel ManageMessageModel)
        {
            HttpResponseMessage response;
            try
            {
                ManageMessageModel = _service.CreateManageMessage(ManageMessageModel);

                if (HelperUtility.IsNotNull(ManageMessageModel))
                {
                    response = CreateCreatedResponse(new ManageMessageResponse { ManageMessage = ManageMessageModel });
                    
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(ManageMessageModel.CMSMessageId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new ManageMessageResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ManageMessageResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get ManageMessage List.
        /// </summary>
        /// <returns>Returns ManageMessage List.</returns>
        [ResponseType(typeof(ManageMessageListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetManageMessages()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetManageMessages(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ManageMessageListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ManageMessageListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get ManageMessage details.
        /// </summary>
        /// <returns>Returns ManageMessage model.</returns>
        [ResponseType(typeof(ManageMessageResponse))]
        [HttpPut]
        public virtual HttpResponseMessage GetManageMessage([FromBody] ManageMessageMapperModel manageMessageModel)
        {
            HttpResponseMessage response;

            try
            {
                //Get message by id.
                string data = _cache.GetManageMessage(manageMessageModel, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<ManageMessageResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ManageMessageResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Update ManageMessage.
        /// </summary>
        /// <param name="ManageMessageModel">ManageMessageModel.</param>
        /// <returns>Returns updated model.</returns>
        [ResponseType(typeof(ManageMessageResponse))]
        [HttpPut]
        public virtual HttpResponseMessage UpdateManageMessage([FromBody] ManageMessageModel ManageMessageModel)
        {
            HttpResponseMessage response;
            try
            {
                //Update ManageMessage.
                bool status = _service.UpdateManageMessage(ManageMessageModel);
                response = status ? CreateCreatedResponse(new ManageMessageResponse { ManageMessage = ManageMessageModel, ErrorCode = 0 }) : CreateInternalServerErrorResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(ManageMessageModel.CMSMessageId)));
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new ManageMessageResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ManageMessageResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.ExceptionalError });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Delete ManageMessage.
        /// </summary>
        /// <param name="cmsManageMessageId">cmsManageMessageId to delete ManageMessage.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage DeleteManageMessage([FromBody] ParameterModel cmsManageMessageId)
        {
            HttpResponseMessage response;
            try
            {
                //Delete ManageMessage.
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.DeleteManageMessage(cmsManageMessageId) });
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new ManageMessageResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.AssociationDeleteError });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }


        /// <summary>
        /// Publish Message 
        /// </summary>
        /// <param name="contentPageParameterModel"></param>
        /// <returns>returns true on publish else false </returns>
        [ResponseType(typeof(PublishedResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage PublishManageMessageWithPreview(ContentPageParameterModel parameterModel)
        {
            HttpResponseMessage response;
            try
            {
                PublishedModel published = _service.PublishManageMessage(parameterModel.Ids, parameterModel.portalId, parameterModel.localeId, parameterModel.TargetPublishState, parameterModel.TakeFromDraftFirst);
                response = !Equals(published, null) ? CreateOKResponse(new PublishedResponse { PublishedModel = published }) : CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
                response = CreateInternalServerErrorResponse(new PublishedResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
                response = CreateInternalServerErrorResponse(new PublishedResponse { HasError = true, ErrorMessage = ex.Message });
            }
            return response;
        }
        #endregion
        #endregion
    }
}
