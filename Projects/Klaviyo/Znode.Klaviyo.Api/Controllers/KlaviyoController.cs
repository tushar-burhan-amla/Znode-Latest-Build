using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;

using Znode.Engine.Api.Cache;
using Znode.Engine.Api.klaviyo.Cache;
using Znode.Engine.klaviyo.Models;
using Znode.Engine.klaviyo.Models.Responses;
using Znode.Engine.Klaviyo.Services;
using Znode.Libraries.Abstract.Controllers;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Klaviyo.Api
{
    public class KlaviyoController : BaseController
    {
        #region Private Variables
        private readonly IKlaviyoCache _cache;
        private readonly IKlaviyoService _service;
        #endregion

        #region Constructor
        public KlaviyoController()
        {
            _service = GetService<IKlaviyoService>();
            _cache = new KlaviyoCache(new KlaviyoService());
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Get Klaviyo details for specified portal ID
        /// </summary>
        /// <param name="portalId">Int PortalId to get klaviyo details</param>
        /// <returns>Klaviyo details as response</returns>
        [ResponseType(typeof(KlaviyoResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetKlaviyo(int portalId)
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetKlaviyo(portalId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<KlaviyoResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                KlaviyoResponse klaviyoResponse = new KlaviyoResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(klaviyoResponse);
            }
            return response;
        }

        [ResponseType(typeof(KlaviyoResponse))]
        [HttpPost]
        //[ValidateModel]
        public virtual HttpResponseMessage UpdateKlaviyoSetting([FromBody] KlaviyoModel klaviyoModel)
        {
            HttpResponseMessage response;
            try
            {
                bool isUpdated = _service.UpdateKlaviyo(klaviyoModel);
                response = isUpdated ? CreateOKResponse(new KlaviyoResponse { Klaviyo = klaviyoModel }) : CreateInternalServerErrorResponse();
            }
            catch (Exception ex)
            {
                KlaviyoResponse klaviyoResponse = new KlaviyoResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(klaviyoResponse);
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get Track Klaviyo details for specified portal ID
        /// </summary>
        /// <param name="KlaviyoProductDetailModel">KlaviyoProductDetailModel to get klaviyo details</param>
        /// <returns>Klaviyo details as response</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage Track(KlaviyoProductDetailModel klaviyoProductDetailModel)
        {
            HttpResponseMessage response;
            try
            {
                bool trackSuccess = _service.KlaviyoTrack(klaviyoProductDetailModel);

                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = trackSuccess });

            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                TrueFalseResponse klaviyoResponse = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(klaviyoResponse);
            }
            return response;
        }

        /// <summary>
        /// Get Identify Klaviyo details for specified portal ID
        /// </summary>
        /// <param name="UserModel">UserModel to get klaviyo details</param>
        /// <returns>Klaviyo details as response</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost]
        public virtual HttpResponseMessage Identify([FromBody] IdentifyModel userModel)
        {
            HttpResponseMessage response;
            try
            {
                bool identifySuccess = _service.KlaviyoIdentify(userModel);

                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = identifySuccess });
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                TrueFalseResponse klaviyoResponse = new TrueFalseResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(klaviyoResponse);
            }
            return response;
        }

        /// <summary>
        /// Get Email Provider details for specified portal ID
        /// </summary>
        /// <param name="portalId">Int PortalId to get Email Provider details</param>
        /// <returns>Email Provider details as response</returns>
        [ResponseType(typeof(KlaviyoResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetEmailProviderList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetEmailProviderList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<KlaviyoResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.Portal.ToString(), TraceLevel.Error);
                KlaviyoResponse klaviyoResponse = new KlaviyoResponse { HasError = true, ErrorMessage = ex.Message };
                response = CreateInternalServerErrorResponse(klaviyoResponse);
            }
            return response;
        }

        #endregion
    }
}

