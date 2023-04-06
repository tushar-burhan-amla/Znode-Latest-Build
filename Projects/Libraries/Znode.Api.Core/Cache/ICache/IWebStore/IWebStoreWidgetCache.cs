using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Cache
{
    public interface IWebStoreWidgetCache
    {
        /// <summary>
        /// Get slider data.
        /// </summary>
        /// <param name="parameter">Web Store Widget Parameter Model.</param>
        /// <param name="routeUri">Route Uri.</param>
        /// <param name="routeTemplate">Route Template.</param>
        /// <returns>Returns slider data.</returns>
        string GetSlider(WebStoreWidgetParameterModel parameter, string routeUri, string routeTemplate);

        /// <summary>
        ///  Get product list widget data.
        /// </summary>
        /// <param name="parameter">Web Store Widget Parameter Model.</param>
        /// <param name="routeUri">Route Uri.</param>
        /// <param name="routeTemplate">Route Template.</param>
        /// <returns>Returns product list widget data.</returns>
        string GetProducts(WebStoreWidgetParameterModel parameter, string routeUri, string routeTemplate);

        /// <summary>
        /// Get link widget data.
        /// </summary>
        /// <param name="parameter">Web Store Widget Parameter Model.</param>
        /// <param name="routeUri">Route Uri.</param>
        /// <param name="routeTemplate">Route Template.</param>
        /// <returns>Returns link widget data.</returns>
        string GetLinkWidget(WebStoreWidgetParameterModel parameter, string routeUri, string routeTemplate);

        /// <summary>
        /// Get category list data.
        /// </summary>
        /// <param name="parameter">Web Store Widget Parameter Model.</param>
        /// <param name="routeUri">Route Uri.</param>
        /// <param name="routeTemplate">Route Template.</param>
        /// <returns>Returns category list.</returns>
        string GetCategories(WebStoreWidgetParameterModel parameter, string routeUri, string routeTemplate);

        /// <summary>
        /// Get link product list.
        /// </summary>
        /// <param name="parameter">WebStoreWidgetParameterModel parameter.</param>
        /// <param name="routeUri">Route Uri.</param>
        /// <param name="routeTemplate">Route Template.</param>
        /// <returns>Returns link product list.</returns>
        string GetLinkProducts(WebStoreWidgetParameterModel parameter, string routeUri, string routeTemplate);

        /// <summary>
        /// Get tag manager list.
        /// </summary>
        /// <param name="parameter">WebStoreWidgetParameterModel parameter.</param>
        /// <param name="routeUri">Route Uri.</param>
        /// <param name="routeTemplate">Route Template.</param>
        /// <returns>Returns tag manager.</returns>
        string GetTagManager(WebStoreWidgetParameterModel parameter, string routeUri, string routeTemplate);

        /// <summary>
        /// Get Media Widget Details.
        /// </summary>
        /// <param name="parameter">WebStoreWidgetParameterModel parameter.</param>
        /// <param name="routeUri">Route Uri.</param>
        /// <param name="routeTemplate">Route Template.</param>
        /// <returns>Returns Widget Details.</returns>
        string GetMediaWidgetDetails(WebStoreWidgetParameterModel parameter, string routeUri, string routeTemplate);

        /// <summary>
        /// Get brand list data.
        /// </summary>
        /// <param name="parameter">Web Store Widget Parameter Model.</param>
        /// <param name="routeUri">Route Uri.</param>
        /// <param name="routeTemplate">Route Template.</param>
        /// <returns>Returns brand list.</returns>
        string GetBrands(WebStoreWidgetParameterModel parameter, string routeUri, string routeTemplate);

        /// <summary>
        /// Get Form Configuration By CMSMappingId.
        /// </summary>
        /// <param name="parameter">WebStore Widget Form Parameters</param>
        /// <param name="routeUri">Route Uri</param>
        /// <param name="routeTemplate">Route Template</param>
        /// <returns>Returns Form Configuration Model.</returns>
        string GetFormConfigurationByCMSMappingId(WebStoreWidgetFormParameters parameter, string routeUri, string routeTemplate);

        /// <summary>
        /// Get search widget products.
        /// </summary>
        /// <param name="parameter">WebStore Widget Form Parameters</param>
        /// <param name="routeUri">Route Uri</param>
        /// <param name="routeTemplate">Route Template</param>
        /// <returns>Returns search widget products</returns>
        string GetSearchWidgetData(WebStoreSearchWidgetParameterModel parameter, string routeUri, string routeTemplate);

        /// <summary>
        /// Get container data.
        /// </summary>
        /// <param name="parameter">Web Store Widget Parameter Model.</param>
        /// <param name="routeUri">Route Uri.</param>
        /// <param name="routeTemplate">Route Template.</param>
        /// <returns>Returns ContainerKey.</returns>
        string GetContainer(WebStoreWidgetParameterModel parameter, string routeUri, string routeTemplate);
    }
}
