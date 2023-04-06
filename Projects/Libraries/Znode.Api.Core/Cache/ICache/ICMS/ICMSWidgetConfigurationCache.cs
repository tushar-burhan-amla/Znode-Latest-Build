namespace Znode.Engine.Api.Cache
{
    public interface ICMSWidgetConfigurationCache
    {
         
        /// <summary>
        /// Get the list of CMS Text Widget Configuration.
        /// </summary>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns>String Data.</returns>
        string GetTextWidgetConfigurationList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get Widget Configuration on the basis of Widget Configuration id.
        /// </summary>
        /// <param name="textWidgetConfigurationId">Text Widget Configuration id.</param>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns Text Widget Configuration.</returns>
        string GetTextWidgetConfiguration(int textWidgetConfigurationId, string routeUri, string routeTemplate);

        /// <summary>
        /// Get the list of CMS Text Widget Configuration.
        /// </summary>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns>String Data.</returns>
        string GetFormWidgetConfigurationList(string routeUri, string routeTemplate);


        #region CMSWidgetProduct 
        /// <summary>
        ///Get associated product list.
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns list of associated products.</returns>
        string GetAssociatedProductList(string routeUri, string routeTemplate);

        /// <summary>
        /// Get unassociated product list .
        /// </summary>
        /// <param name="routeUri">route uri.</param>
        /// <param name="routeTemplate">route template.</param>
        /// <returns>Returns list of unassociated products.</returns>
        string GetUnAssociatedProductList(string routeUri, string routeTemplate);
        #endregion

        #region CMS Widget Slider Banner
        /// <summary>
        /// Get the CMS Widget Slider Banner Details.
        /// </summary>
        /// <param name="routeUri">route URL</param>
        /// <param name="routeTemplate">route Template</param>
        /// <returns>Data in string format</returns>
        string GetCMSWidgetSliderBanner(string routeUri, string routeTemplate);
        #endregion

        #region Link Widget Configuration
        /// <summary>
        /// Get link widget configuration list.
        /// </summary>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns>String Data.</returns>
        string GetLinkWidgetConfigurationList(string routeUri, string routeTemplate);
        #endregion

        #region Category Association
        /// <summary>
        /// Get list of unassociate categories. 
        /// </summary>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns></returns>
        string GetUnAssociatedCategories(string routeUri, string routeTemplate);

        /// <summary>
        /// Get list of associated categories based on cms widgets.
        /// </summary>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns></returns>
        string GetAssociatedCategories(string routeUri, string routeTemplate);
        #endregion

        #region Brand Association
        /// <summary>
        /// Get list of unassociate brands. 
        /// </summary>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns></returns>
        string GetUnAssociatedBrands(string routeUri, string routeTemplate);

        /// <summary>
        /// Get list of associated brands based on cms widgets.
        /// </summary>
        /// <param name="routeUri">Route URI.</param>
        /// <param name="routeTemplate">Route template.</param>
        /// <returns></returns>
        string GetAssociatedBrands(string routeUri, string routeTemplate);
        #endregion

       /// <summary>
       /// 
       /// </summary>
       /// <param name="cMSContentPagesId"></param>
       /// <param name="routeUri"></param>
       /// <param name="routeTemplate"></param>
       /// <returns></returns>
        string GetFormWidgetEmailConfiguration(int cMSContentPagesId, string routeUri, string routeTemplate);
        string GetSearchWidgetConfiguration(string routeUri, string routeTemplate);
    }
}
