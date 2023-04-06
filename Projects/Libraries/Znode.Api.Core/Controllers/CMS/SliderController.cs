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
    public class SliderController : BaseController
    {
        #region Private Variables
        private readonly ISliderCache _cache;
        private readonly ISliderService _service;
        #endregion

        #region Constructor
        public SliderController(ISliderService service)
        {
            _service = service;
            _cache = new SliderCache(_service);
        }
        #endregion

        #region Public Methods

        #region Slider
        /// <summary>
        /// Get List of slider.
        /// </summary>
        /// <returns>Returns slider list.</returns>
        [ResponseType(typeof(SliderListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage SliderList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetSliders(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<SliderListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new SliderListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Create slider.
        /// </summary>
        /// <param name="model">model</param>
        /// <returns>Returns created model.</returns>
        [ResponseType(typeof(SliderResponse))]
        [HttpPost]
        public virtual HttpResponseMessage CreateSlider([FromBody] SliderModel model)
        {
            HttpResponseMessage response;
            try
            {
                SliderModel slider = _service.CreateSlider(model);

                if (HelperUtility.IsNotNull(slider))
                {
                    response = CreateCreatedResponse(new SliderResponse { Slider = slider });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(slider.CMSSliderId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new SliderResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(),TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new SliderResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(),TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get slider by cmsSliderId.
        /// </summary>
        /// <param name="cmsSliderId">CMS Slider  Id to get slider details.</param>
        /// <returns>Returns slider details.</returns>
        [ResponseType(typeof(SliderResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetSlider(int cmsSliderId)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetSlider(cmsSliderId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<SliderResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new SliderResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(),TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Update slider.
        /// </summary>
        /// <param name="sliderModel">model to update.</param>
        /// <returns>Returns updated model.</returns>
        [ResponseType(typeof(SliderResponse))]
        [HttpPut]
        public virtual HttpResponseMessage UpdateSlider([FromBody] SliderModel sliderModel)
        {
            HttpResponseMessage response;
            try
            {
                //Update slider.
                response = _service.UpdateSlider(sliderModel) ? CreateCreatedResponse(new SliderResponse { Slider = sliderModel, ErrorCode = 0 }) : CreateInternalServerErrorResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(sliderModel.CMSSliderId)));
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new BannerResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new SliderResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Delete slider.
        /// </summary>
        /// <param name="cmsPortalSliderId">CMS Portal Slider Id.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage DeleteSlider([FromBody] ParameterModel cmsPortalSliderId)
        {
            HttpResponseMessage response;

            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.DeleteSlider(cmsPortalSliderId) });
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new SliderResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new SliderResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ErrorCodes.AssociationDeleteError });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }


        /// <summary>
        /// Method to publish slider.
        /// </summary>
        /// <param name="cmsPortalSliderModel"></param>
        /// <returns>Returns status as per published operation.</returns>
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage PublishSliderWithPreview(ParameterModel cmsPortalSliderModel)
        {
            HttpResponseMessage response;
            try
            {
                PublishedModel published = _service.PublishSlider(cmsPortalSliderModel.Ids, cmsPortalSliderModel.PortalId, cmsPortalSliderModel.LocaleId, cmsPortalSliderModel.TargetPublishState, cmsPortalSliderModel.TakeFromDraftFirst);
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

        #region Banner
        /// <summary>
        /// Get List of banner for selected slider.
        /// </summary>
        /// <returns>List of banner list for selected slider.</returns>
        [ResponseType(typeof(BannerListResponse))]
        [HttpGet]
        public virtual HttpResponseMessage BannerList()
        {
            HttpResponseMessage response;
            try
            {
                string data = _cache.GetBannerList(RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<BannerListResponse>(data) : CreateNoContentResponse();
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new BannerListResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Create banner.
        /// </summary>
        /// <param name="bannerModel">Banner model.</param>
        /// <returns>Returns created model.</returns>
        [ResponseType(typeof(BannerResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage CreateBanner([FromBody] BannerModel bannerModel)
        {
            HttpResponseMessage response;

            try
            {
                BannerModel banner = _service.CreateBanner(bannerModel);

                if (HelperUtility.IsNotNull(banner))
                {
                    response = CreateCreatedResponse(new BannerResponse { Banner = banner });
                    response.Headers.Add("Location", GetUriLocation(Convert.ToString(banner.CMSSliderBannerId)));
                }
                else
                    response = CreateInternalServerErrorResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new BannerResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new BannerResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Get banner by cmsSliderBannerId.
        /// </summary>
        /// <param name="cmsSliderBannerId">CMS Slider Banner Id to get banner details.</param>
        /// <returns>Returns banner details.</returns>
        [ResponseType(typeof(BannerResponse))]
        [HttpGet]
        public virtual HttpResponseMessage GetBanner(int cmsSliderBannerId)
        {
            HttpResponseMessage response;

            try
            {
                string data = _cache.GetBanner(cmsSliderBannerId, RouteUri, RouteTemplate);
                response = !string.IsNullOrEmpty(data) ? CreateOKResponse<BannerResponse>(data) : CreateNoContentResponse();
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new BannerResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new BannerResponse { HasError = true, ErrorMessage = ex.Message});
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }

            return response;
        }

        /// <summary>
        /// Update banner.
        /// </summary>
        /// <param name="bannerModel">model to update.</param>
        /// <returns>Returns updated model.</returns>
        [ResponseType(typeof(BannerResponse))]
        [HttpPut]
        public virtual HttpResponseMessage UpdateBanner([FromBody] BannerModel bannerModel)
        {
            HttpResponseMessage response;
            try
            {
                //Update banner.
                response = _service.UpdateBanner(bannerModel) ? CreateCreatedResponse(new BannerResponse { Banner = bannerModel, ErrorCode = 0 }) : CreateInternalServerErrorResponse();
                response.Headers.Add("Location", GetUriLocation(Convert.ToString(bannerModel.CMSSliderBannerId)));
            }
            catch (ZnodeException ex)
            {
                response = CreateInternalServerErrorResponse(new BannerResponse { HasError = true, ErrorMessage = ex.Message, ErrorCode = ex.ErrorCode });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Warning);
            }
            catch (Exception ex)
            {
                response = CreateInternalServerErrorResponse(new BannerResponse { HasError = true, ErrorMessage = ex.Message });
                ZnodeLogging.LogMessage(ex, ZnodeLogging.Components.CMS.ToString(), TraceLevel.Error);
            }
            return response;
        }

        /// <summary>
        /// Delete banner.
        /// </summary>
        /// <param name="cmsSliderBannerId">CMS Slider Banner Id.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        [ResponseType(typeof(TrueFalseResponse))]
        [HttpPost, ValidateModel]
        public virtual HttpResponseMessage DeleteBanner([FromBody] ParameterModel cmsSliderBannerId)
        {
            HttpResponseMessage response;

            try
            {
                response = CreateOKResponse(new TrueFalseResponse { IsSuccess = _service.DeleteBanner(cmsSliderBannerId) });
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
        #endregion
    }
}