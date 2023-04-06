using System.Collections.Generic;
using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IWebStoreWidgetService
    {
        /// <summary>
        /// Get slider widget information.
        /// </summary>
        /// <param name="parameter">Web Store Widget Parameter Model.</param>
        /// <returns>Returns slider information in CMSWidgetConfigurationModel.</returns>
        CMSWidgetConfigurationModel GetSlider(WebStoreWidgetParameterModel parameter);

        /// <summary>
        /// Get product list widget data.
        /// </summary>
        /// <param name="parameter">WebStoreWidgetParameterModel.</param>
        /// <param name="expands">NameValueCollection.</param>
        /// <returns>Returns product list widget data.</returns>
        WebStoreWidgetProductListModel GetProducts(WebStoreWidgetParameterModel parameter, NameValueCollection expands);

        /// <summary>
        /// Get link widget data.
        /// </summary>
        /// <param name="parameter">Web Store Widget Parameter Model.</param>
        /// <returns>Returns list of link widget data.</returns>
        LinkWidgetConfigurationListModel GetLinkWidget(WebStoreWidgetParameterModel parameter);

        /// <summary>
        ///  Get category list data.
        /// </summary>
        /// <param name="parameter">Web Store Widget Parameter Model.</param>
        /// <returns>Returns category list.</returns>
        WebStoreWidgetCategoryListModel GetCategories(WebStoreWidgetParameterModel parameter);

        /// <summary>
        /// Get link product list.
        /// </summary>
        /// <param name="parameter">WebStoreWidgetParameterModel parameter./</param>
        /// <param name="expands">NameValueCollection.</param>
        /// <returns>Returns WebStoreLinkProductListModel.</returns>
        WebStoreLinkProductListModel GetLinkProductList(WebStoreWidgetParameterModel parameter, NameValueCollection expands);

        /// <summary>
        ///  Get tag manager data.
        /// </summary>
        /// <param name="parameter">Web Store Widget Parameter Model.</param>
        /// <returns>Returns tag manager.</returns>
        CMSTextWidgetConfigurationModel GetTagManager(WebStoreWidgetParameterModel parameter);

        /// <summary>
        /// Get Media Widget Details
        /// </summary>
        /// <param name="parameter">Web Store Widget Parameter Model.</param>
        /// <returns>CMSMediaWidgetConfigurationModel model</returns>
        CMSMediaWidgetConfigurationModel GetMediaWidgetDetails(WebStoreWidgetParameterModel parameter);

        /// <summary>
        ///  Get brand list data.
        /// </summary>
        /// <param name="parameter">Web Store Widget Parameter Model.</param>
        /// <returns>Returns brand list.</returns>
        WebStoreWidgetBrandListModel GetBrands(WebStoreWidgetParameterModel parameter);

        /// <summary>
        /// Get WebStoreWidgetFormParameters model.
        /// </summary>
        /// <param name="mappingId">mappingId</param>
        /// <param name="localeId">localeId</param>
        /// <returns>Return WebStoreWidgetFormParameters model </returns>
        WebStoreWidgetFormParameters GetFormConfigurationByCMSMappingId(int mappingId, int localeId);

        /// <summary>
        ///Get search widget products ad facets.
        /// </summary>
        /// <param name="parameter">model with widget parameter</param>
        /// <param name="expands">expands to get other data.</param>
        /// <param name="filters">filter for product list.</param>
        /// <param name="sorts">Sort collection for sorting</param>
        /// <returns>Widget data.</returns>
        WebStoreWidgetSearchModel GetSearchWidgetData(WebStoreSearchWidgetParameterModel parameter, NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Map link product data.
        /// </summary>
        /// <param name="parameter"></param>
        /// <param name="attributeLinkProduct"></param>
        /// <param name="linkProductList"></param>
        /// <param name="expands"></param>
        void MapLinkProducts(WebStoreWidgetParameterModel parameter, IEnumerable<PublishAttributeModel> attributeLinkProduct, List<WebStoreLinkProductModel> linkProductList, NameValueCollection expands);

        /// <summary>
        /// Get container widget information.
        /// </summary>
        /// <param name="parameter">Web Store Widget Parameter Model.</param>
        /// <returns>Returns ContainerKey.</returns>
        string GetContainer(WebStoreWidgetParameterModel parameter);
    }
}
