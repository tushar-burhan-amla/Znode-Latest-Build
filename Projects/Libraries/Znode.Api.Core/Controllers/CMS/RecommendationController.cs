using Newtonsoft.Json;
using System;
using System.Diagnostics;
using System.Net.Http;
using System.Web.Http;
using System.Web.Http.Description;
using Znode.Engine.Api.Cache;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Engine.Services;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Api.Controllers
{
    public class RecommendationController : BaseController
    {
        #region Private Variables
        private readonly IRecommendationCache _recommendationCache;
        private readonly IRecommendationService _recommendationService;
        #endregion

        #region Constructor
        public RecommendationController(IRecommendationService recommendationService)
        {
            _recommendationService = recommendationService;
            _recommendationCache = new RecommendationCache(_recommendationService);
        }
        #endregion

        #region Public Methods
        [ResponseType(typeof(RecommendationSettingResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetRecommendationSetting(int portalId, string touchPointName)
        {
            HttpResponseMessage response;

            try
            {
                string data = _recommendationCache.GetRecommendationSetting(portalId, touchPointName, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<RecommendationSettingResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new RecommendationSettingResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new RecommendationSettingResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        [ResponseType(typeof(RecommendationSettingResponse))]
        [HttpPut]
        public virtual HttpResponseMessage SaveRecommendationSetting([FromBody] RecommendationSettingModel model)
        {
            HttpResponseMessage response;
            try
            {
                response = CreateOKResponse(new RecommendationSettingResponse { RecommendationSetting = _recommendationService.SaveRecommendationSetting(model) });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new RecommendationSettingResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new RecommendationSettingResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }
        #endregion

        #region Recommendation Engine
        /// <summary>
        /// Gets list of recommended products.
        /// </summary>
        /// <returns> Recommended products.</returns>
        [ResponseType(typeof(RecommendationResponse))]
        [HttpPost]
        public virtual HttpResponseMessage GetRecommendation([FromBody]RecommendationRequestModel recommendationRequestModel)
        {
            HttpResponseMessage response;

            try
            {
                RecommendationResponse res = new RecommendationResponse
                {
                    Recommendation = _recommendationService.GetRecommendation(recommendationRequestModel)
                };
                response = CreateOKResponse<RecommendationResponse>(JsonConvert.SerializeObject(res));
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new PublishProductListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Gets list of recommended products.
        /// </summary>
        /// <returns> Recommended products.</returns>
        [ResponseType(typeof(RecommendationDataGenerationResponse))]
        [HttpPost]
        public virtual HttpResponseMessage GenerateRecommendationData([FromBody]RecommendationDataGenerationRequestModel recommendationDataGenerationRequest)
        {
            HttpResponseMessage response;

            try
            {
                response = CreateOKResponse(new RecommendationDataGenerationResponse
                {
                    recommendationGeneratedData = _recommendationService.GenerateRecommendationData(recommendationDataGenerationRequest.PortalId, recommendationDataGenerationRequest.IsBuildPartial)
                });   
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new RecommendationDataGenerationResponse { ErrorCode = ex.ErrorCode, HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new RecommendationDataGenerationResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }

            return response;
        }
        #endregion
    }
}
