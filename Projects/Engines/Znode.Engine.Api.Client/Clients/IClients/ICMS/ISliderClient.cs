using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface ISliderClient : IBaseClient
    {
        #region Slider

        /// Get the list of slider.
        /// </summary>
        /// <param name="filters">Filter Collection for slider list.</param>
        /// <param name="sorts">Sort collection of slider list.</param>
        /// <param name="pageIndex">Index of requesting page</param>
        /// <param name="pageSize">Record per page</param>
        /// <returns>List of slider</returns>
        SliderListModel GetSliders(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Create new slider.
        /// </summary>
        /// <param name="model">Slider Model</param>
        /// <returns>Slider Model</returns>
        SliderModel CreateSlider(SliderModel model);

        /// <summary>
        /// Get slider on the basis of cmsSliderId.
        /// </summary>
        /// <param name="cmsSliderId">cmsSliderId to get slider details.</param>
        /// <returns>Returns SliderModel.</returns>
        SliderModel GetSlider(int cmsSliderId);

        /// <summary>
        /// Update slider data.
        /// </summary>
        /// <param name="sliderModel">Slider model to update.</param>
        /// <returns>Returns updated slider model.</returns>
        SliderModel UpdateSlider(SliderModel sliderModel);

        /// <summary>
        /// Delete slider.
        /// </summary>
        /// <param name="cmsPortalSliderId">CMS Portal Slider Id.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool DeleteSlider(ParameterModel cmsPortalSliderId);

        /// <summary>
        /// Publish Slider.
        /// </summary>
        /// <param name="parameterModel">Parameter model containing cmsSliderId id for publishing.</param>
        /// <returns>Returns published model.</returns>
        PublishedModel PublishSlider(ParameterModel parameterModel);

        #endregion

        #region Banner

        /// <summary>
        /// Get banner list for selected slider.
        /// </summary>
        /// <param name="expands">Expands for banner.</param>
        /// <param name="filters">Filters for banner.</param>
        /// <param name="sorts">Sorts for for banner.</param>
        /// <param name="pageIndex">Index of page.</param>
        /// <param name="pageSize">Size of page.</param>
        /// <returns>Returns list of banner for selected slider.</returns>
        BannerListModel GetBannerList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Create banner.
        /// </summary>
        /// <param name="bannerModel">Banner Model.</param>
        /// <returns>Returns created banner Model.</returns>
        BannerModel CreateBanner(BannerModel bannerModel);

        /// <summary>
        /// Get banner on the basis of cmsSliderBannerId.
        /// </summary>
        /// <param name="cmsSliderBannerId">cmsSliderBannerId to get banner details.</param>
        /// <param name="filters">Filter to pass.</param>
        /// <returns>Returns BannerModel.</returns>
        BannerModel GetBanner(int cmsSliderBannerId, FilterCollection filters);

        /// <summary>
        /// Update banner data.
        /// </summary>
        /// <param name="bannerModel">Banner model to update.</param>
        /// <returns>Returns updated banner model.</returns>
        BannerModel UpdateBanner(BannerModel bannerModel);

        /// <summary>
        /// Delete banner.
        /// </summary>
        /// <param name="cmsSliderBannerId">CMSSliderBannerId.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool DeleteBanner(ParameterModel cmsSliderBannerId);

        #endregion
    }
}
