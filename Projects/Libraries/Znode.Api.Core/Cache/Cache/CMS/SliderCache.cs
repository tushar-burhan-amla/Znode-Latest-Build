using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.Extensions;
using Znode.Engine.Api.Models.Responses;
using Znode.Engine.Services;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Cache
{
    public class SliderCache : BaseCache, ISliderCache
    {
        #region Private Variable
        private readonly ISliderService _service;
        #endregion

        #region Constructor
        public SliderCache(ISliderService sliderService)
        {
            _service = sliderService;
        }
        #endregion

        #region Public Methods

        #region Slider
        //Get the list of slider.
        public virtual string GetSliders(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                SliderListModel list = _service.GetSliders(Filters, Sorts, Page);
                if (list?.Sliders?.Count > 0)
                {
                    SliderListResponse response = new SliderListResponse { Sliders = list.Sliders };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                    return data;
                }
            }
            return data;
        }

        //Get slider by cmsSliderId.
        public virtual string GetSlider(int cmsSliderId, string routeUri, string routeTemplate)
        {
            //Get data from Cache
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Create Response
                SliderModel sliderModel = _service.GetSlider(cmsSliderId);
                if (HelperUtility.IsNotNull(sliderModel))
                    data = InsertIntoCache(routeUri, routeTemplate, new SliderResponse { Slider = sliderModel });
            }
            return data;
        }
        #endregion

        #region Banner
        //Get banner list as per selected slider
        public virtual string GetBannerList(string routeUri, string routeTemplate)
        {
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                BannerListModel list = _service.GetBannerList(Expands, Filters, Sorts, Page);
                if (list?.Banners?.Count > 0)
                {
                    BannerListResponse response = new BannerListResponse { Banners = list.Banners };
                    response.MapPagingDataFromModel(list);
                    data = InsertIntoCache(routeUri, routeTemplate, response);
                }
            }
            return data;
        }

        //Get banner by cmsSliderBannerId.
        public virtual string GetBanner(int cmsSliderBannerId, string routeUri, string routeTemplate)
        {
            //Get data from Cache
            string data = GetFromCache(routeUri);
            if (string.IsNullOrEmpty(data))
            {
                //Create Response
                BannerModel bannerModel = _service.GetBanner(cmsSliderBannerId, Filters);
                if (HelperUtility.IsNotNull(bannerModel))
                    data = InsertIntoCache(routeUri, routeTemplate, new BannerResponse { Banner = bannerModel });
            }
            return data;
        }
        #endregion
        #endregion
    }
}