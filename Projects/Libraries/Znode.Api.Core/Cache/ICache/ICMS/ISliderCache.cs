namespace Znode.Engine.Api.Cache
{
    public interface ISliderCache
    {
        #region Slider

        /// <summary>
        /// Get the list of slider.
        /// </summary>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns>String Data.</returns>
        string GetSliders(string routeUri, string routeTemplate);


        /// <summary>
        /// Get slider on the basis of cmsSliderId.
        /// </summary>
        /// <param name="cmsSliderId">CMS Slider id.</param>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns slider.</returns>
        string GetSlider(int cmsSliderId, string routeUri, string routeTemplate);

        #endregion

        #region Banner

        /// <summary>
        /// Get banner list for selected slider.
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns list of banner for selected slider.</returns>
        string GetBannerList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get banner on the basis of cmsSliderBannerId.
        /// </summary>
        /// <param name="cmsSliderBannerId">CMS Slider Banner id.</param>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns banner.</returns>
        string GetBanner(int cmsSliderBannerId, string routeUri, string routeTemplate);

        #endregion
    }
}