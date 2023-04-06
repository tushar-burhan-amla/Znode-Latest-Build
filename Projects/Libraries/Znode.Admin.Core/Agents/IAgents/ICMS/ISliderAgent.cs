using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Sorts;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Agents
{
    public interface ISliderAgent
    {
        #region Slider
        /// <summary>
        /// Get list of sliders from portal id
        /// </summary>
        /// <param name="filters">filter list across sliders.</param>
        /// <param name="sortCollection">sort collection for sliders.</param>
        /// <param name="pageIndex">pageIndex for sliders record. </param>
        /// <param name="recordPerPage">paging sliders record per page.</param>
        /// <returns>Slider list</returns>
        SliderListViewModel GetSliders(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Create new slider.
        /// </summary>
        /// <param name="sliderViewModel">SliderViewModel</param>
        /// <returns>Returns sliderViewModel.</returns>
        SliderViewModel CreateSlider(SliderViewModel sliderViewModel);


        /// <summary>
        /// Get slider by CMS Slider Id.
        /// </summary>
        /// <param name="cmsSliderId">CMS Slider Id</param>
        /// <returns>Returns SliderViewModel.</returns>
        SliderViewModel GetSlider(int cmsSliderId);

        /// <summary>
        /// Update slider.
        /// </summary>
        /// <param name="cmsSliderId">cmsSliderId</param>
        /// <param name="data">model as json format</param>
        /// <returns>Returns updated SliderViewModel.</returns>
        SliderViewModel UpdateSlider(int cmsSliderId, string data);

        /// <summary>
        /// Delete slider.
        /// </summary>
        /// <param name="cmsPortalSliderId">cms Portal SliderId.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool DeleteSlider(string cmsPortalSliderId, out string errorMessage);

        /// <summary>
        /// Publish Slider.
        /// </summary>
        /// <param name="cmsPortalSliderId">Slider Id to publish.</param>
        /// <param name="errorMessage">error Message</param>
        /// <returns>Returns true if published successfully else return false.</returns>
        bool PublishSlider(string cmsPortalSliderId, out string errorMessage);

        /// <summary>
        /// Publish Slider.
        /// </summary>
        /// <param name="cmsPortalSliderId"></param>
        /// <param name="localeId"></param>
        /// <param name="portalId"></param>
        /// <param name="errorMessage"></param>
        /// <param name="targetPublishSlider"></param>
        /// <param name="takeFromDraftFirst"></param>
        /// <returns></returns>
        bool PublishSlider(string cmsPortalSliderId, int portalId, int localeId, out string errorMessage, string targetPublishSlider = null, bool takeFromDraftFirst = false);

        /// <summary>
        /// Check Whether the Slider Name is already exists.
        /// </summary>
        /// <param name="sliderName">is a sliderName</param>
        /// <param name="cmsSliderId">id for the slider</param>
        /// <returns>return the status in true or false</returns>
        bool CheckSliderNameExist(string sliderName, int cmsSliderId);
        #endregion

        #region Banners

        /// <summary>
        /// Get banner list for selected slider.
        /// </summary>
        /// <param name="filters">Filters for banner.</param>
        /// <param name="sorts">Sorts for for banner.</param>
        /// <param name="pageIndex">Index of page.</param>
        /// <param name="pageSize">Size of page.</param>
        /// <returns>Returns list of banner for selected slider.</returns>
        BannerListViewModel GetBannerList(FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Create banner.
        /// </summary>
        /// <param name="bannerViewModel">Banner View Model.</param>
        /// <returns>Returns created model.</returns>
        BannerViewModel CreateBanner(BannerViewModel bannerViewModel);

        /// <summary>
        /// Get banner list by CMS Slider Banner Id.
        /// </summary>
        /// <param name="cmsSliderBannerId">CMS Slider Banner Id</param>
        /// <param name="localeId">Locale id.</param>
        /// <returns>Returns BannerViewModel.</returns>
        BannerViewModel GetBanner(int cmsSliderBannerId, int localeId);

        /// <summary>
        /// Update banner.
        /// </summary>
        /// <param name="bannerViewModel">Banner view model to update.</param>
        /// <returns>Returns updated banner model.</returns>
        BannerViewModel UpdateBanner(BannerViewModel bannerViewModel);

        /// <summary>
        /// Delete banner.
        /// </summary>
        /// <param name="cmsSliderBannerId">CMS Slider Banner Id.</param>
        /// <returns>Returns true if deleted successfully else return false.</returns>
        bool DeleteBanner(string cmsSliderBannerId);

        /// <summary>
        /// Check Whether the Banner Name is already exists.
        /// </summary>
        /// <param name="bannerName">is a bannerName</param>
        /// <param name="cmsSliderBannerId">id for the banner</param>
        /// <param name="cmsSliderId">id for the slider</param>
        /// <returns>return the status in true or false</returns>
        bool CheckBannerNameExist(string sliderName, int cmsSliderBannerId, int cmsSliderId);

        /// <summary>
        /// //Inline edit and update the banner sequence for respective banner
        /// </summary>
        /// <param name="cmsSliderBannerId">Id of banner whose sequence is to be updated.</param>
        /// /// <param name="data">data containing banner sequence in JSON format</param>
        /// <returns>Returns updated banner model with updated sequence.</returns>
        BannerViewModel UpdateBannerSequence(int cmsSliderBannerId, string data);
        #endregion
    }
}