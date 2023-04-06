using Newtonsoft.Json;
using System.Collections.ObjectModel;
using System.Net;
using Znode.Engine.Api.Client.Endpoints;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Exceptions;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public class SliderClient : BaseClient, ISliderClient
    {
        #region Slider
        //Get the list of slider.
        public virtual SliderListModel GetSliders(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = SliderEndpoint.SliderList();
            endpoint += BuildEndpointQueryString(null, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            SliderListResponse response = GetResourceFromEndpoint<SliderListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            SliderListModel list = new SliderListModel { Sliders = response?.Sliders };
            list.MapPagingDataFromResponse(response);

            return list;
        }

        //Create slider.
        public virtual SliderModel CreateSlider(SliderModel model)
        {
            string endpoint = SliderEndpoint.CreateSlider();

            ApiStatus status = new ApiStatus();
            SliderResponse response = PostResourceToEndpoint<SliderResponse>(endpoint, JsonConvert.SerializeObject(model), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.Slider;
        }

        //Get slider by cmsSliderId
        public virtual SliderModel GetSlider(int cmsSliderId)
        {
            string endpoint = SliderEndpoint.GetSlider(cmsSliderId);

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            SliderResponse response = GetResourceFromEndpoint<SliderResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.Slider;
        }

        //Update slider.
        public virtual SliderModel UpdateSlider(SliderModel sliderModel)
        {
            string endpoint = SliderEndpoint.UpdateSlider();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            SliderResponse response = PutResourceToEndpoint<SliderResponse>(endpoint, JsonConvert.SerializeObject(sliderModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.Slider;
        }

        //Delete slider.
        public virtual bool DeleteSlider(ParameterModel cmsPortalSliderId)
        {
            string endpoint = SliderEndpoint.DeleteSlider();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(cmsPortalSliderId), status);

            //check the status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }
        
        //Publish slider.
        public virtual PublishedModel PublishSlider(ParameterModel parameterModel)
        {
            string endpoint = SliderEndpoint.PublishSliderWithPreview();

            ApiStatus status = new ApiStatus();
            PublishedResponse response = PostResourceToEndpoint<PublishedResponse>(endpoint, JsonConvert.SerializeObject(parameterModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);

            return response?.PublishedModel;
        }

        #endregion

        #region Banner
        //Get banner list for selected  slider.
        public virtual BannerListModel GetBannerList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize)
        {
            string endpoint = SliderEndpoint.GetBannerList();
            endpoint += BuildEndpointQueryString(expands, filters, sorts, pageIndex, pageSize);

            ApiStatus status = new ApiStatus();
            BannerListResponse response = GetResourceFromEndpoint<BannerListResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            BannerListModel listModel = new BannerListModel { Banners = response?.Banners };
            listModel.MapPagingDataFromResponse(response);

            return listModel;
        }

        //Create Slider.
        public virtual BannerModel CreateBanner(BannerModel bannerModel)
        {
            //Get Endpoint.
            string endpoint = SliderEndpoint.CreateBanner();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            BannerResponse response = PostResourceToEndpoint<BannerResponse>(endpoint, JsonConvert.SerializeObject(bannerModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.Banner;
        }

        //Get banner by cmsSliderBannerId
        public virtual BannerModel GetBanner(int cmsSliderBannerId, FilterCollection filters)
        {
            string endpoint = SliderEndpoint.GetBanner(cmsSliderBannerId);
            endpoint += BuildEndpointQueryString(null, filters, null, null, null);

            ApiStatus status = new ApiStatus();
            BannerResponse response = GetResourceFromEndpoint<BannerResponse>(endpoint, status);

            Collection<HttpStatusCode> expectedStatusCodes = new Collection<HttpStatusCode> { HttpStatusCode.OK, HttpStatusCode.NoContent };
            CheckStatusAndThrow<ZnodeException>(status, expectedStatusCodes);

            return response?.Banner;
        }

        //Update banner.
        public virtual BannerModel UpdateBanner(BannerModel bannerModel)
        {
            string endpoint = SliderEndpoint.UpdateBanner();

            //Get Serialize object as a response.
            ApiStatus status = new ApiStatus();
            BannerResponse response = PutResourceToEndpoint<BannerResponse>(endpoint, JsonConvert.SerializeObject(bannerModel), status);

            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.Created);

            return response?.Banner;
        }

        //Delete banner.
        public virtual bool DeleteBanner(ParameterModel cmsSliderBannerId)
        {
            string endpoint = SliderEndpoint.DeleteBanner();
            ApiStatus status = new ApiStatus();
            TrueFalseResponse response = PostResourceToEndpoint<TrueFalseResponse>(endpoint, JsonConvert.SerializeObject(cmsSliderBannerId), status);

            //check the status of response.
            CheckStatusAndThrow<ZnodeException>(status, HttpStatusCode.OK);
            return response.IsSuccess;
        }

        #endregion
    }
}
