using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface ISliderService
    {
        #region Slider

        /// <summary>
        /// Get the list of sliders.
        /// </summary>
        /// <param name="filters">List of filter tuples</param>
        /// <param name="sorts">Collection of sorting parameters</param>
        /// <param name="page">Collection of paging parameters</param>
        /// <returns>List of sliders</returns>
        SliderListModel GetSliders(FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Create new slider
        /// </summary>
        /// <param name="sliderModel">Slider model</param>
        /// <returns>Slider Model</returns>
        SliderModel CreateSlider(SliderModel sliderModel);

        /// <summary>
        /// Get slider on the basis of cmsSliderid.
        /// </summary>
        /// <param name="cmsSliderId">CMS Slider Id.</param>
        /// <returns>Returns slider model.</returns>
        SliderModel GetSlider(int cmsSliderId);

        /// <summary>
        /// Update slider data.
        /// </summary>
        /// <param name="sliderModel">Slider model to update.</param>
        /// <returns>Returns true if model updated successfully else return false.</returns>
        bool UpdateSlider(SliderModel sliderModel);

        /// <summary>
        /// Delete slider.
        /// </summary>
        /// <param name="cmsPortalSliderIds">CMS Portal Slider Id.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool DeleteSlider(ParameterModel cmsPortalSliderIds);

        /// <summary>
        /// Publish slider.
        /// </summary>
        /// <param name="cmsPortalSliderId"></param>
        /// <param name="portalId"></param>
        /// <param name="localeId"></param>
        /// <param name="targetPublishState"></param>
        /// <param name="takeFromDraftFirst"></param>
        /// <returns></returns>
        PublishedModel PublishSlider(string cmsPortalSliderId, int portalId, int localeId, string targetPublishState = null, bool takeFromDraftFirst = false);

        #endregion

        #region Banner

        /// <summary>
        ///  Get banner list for selected slider.
        /// </summary>
        /// <param name="expands">Expands for banner.</param>
        /// <param name="filters">Filters for banner.</param>
        /// <param name="sorts">Sorts for for banner.</param>
        /// <param name="page">Page size.</param>
        /// <returns>Returns list of banner for selected slider.</returns>
        BannerListModel GetBannerList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Create banner.
        /// </summary>
        /// <param name="bannerModel">Banner Model.</param>
        /// <returns>Returns created banner Model.</returns>
        BannerModel CreateBanner(BannerModel bannerModel);

        /// <summary>
        /// Get banner on the basis of cmsSliderBannerId.
        /// </summary>
        /// <param name="cmsSliderBannerId">CMS Slider Banner Id.</param>
        /// <param name="filters">Filter to pass.</param>
        /// <returns>Returns banner model.</returns>
        BannerModel GetBanner(int cmsSliderBannerId, FilterCollection filters);

        /// <summary>
        /// Update banner data.
        /// </summary>
        /// <param name="bannerModel">Banner model to update.</param>
        /// <returns>Returns true if model updated successfully else return false.</returns>
        bool UpdateBanner(BannerModel bannerModel);

        /// <summary>
        /// Delete banner.
        /// </summary>
        /// <param name="cmsSliderBannerIds">CMS Sldier Banner Id.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool DeleteBanner(ParameterModel cmsSliderBannerIds);
        #endregion
    }
}
