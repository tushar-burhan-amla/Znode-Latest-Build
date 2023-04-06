using System.Threading.Tasks;
using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IWebStoreWidgetClient : IBaseClient
    {
        /// <summary>
        /// Async call to get slider details.
        /// </summary>
        /// <param name="parameter">Web Store Widget Parameter Model.</param>
        /// <returns>Returns CMSWidgetConfigurationModel asynchronously.</returns>
        Task<CMSWidgetConfigurationModel> GetSliderAsync(WebStoreWidgetParameterModel parameter);

        /// <summary>
        /// Get slider details.
        /// </summary>
        /// <param name="parameter">Web Store Widget Parameter Model.</param>
        /// <param name="key">Unique key for a widget.</param>
        /// <returns>Returns CMSWidgetConfigurationModel containing slider details.</returns>
        CMSWidgetConfigurationModel GetSlider(WebStoreWidgetParameterModel parameter, string key);

        /// <summary>
        /// Get product list widget details.
        /// </summary>
        /// <param name="parameter">Web Store Widget Parameter Model.</param>
        /// <param name="key">Unique key for a widget.</param>
        /// <param name="expands">Expand Collection.</param>
        /// <returns>Returns CMSWidgetProductListModel containing product list details.</returns>
        WebStoreWidgetProductListModel GetProducts(WebStoreWidgetParameterModel parameter, string key, ExpandCollection expands);

        /// <summary>
        /// Get the details of the media widget
        /// </summary>
        /// <param name="parameter">Web Store Widget Parameter Model.</param>
        /// <param name="key">Unique key for a widget.</param>
        /// <param name="expands">CMSMediaWidgetConfigurationModel model</param>
        /// <returns>CMSMediaWidgetConfigurationModel model</returns>
        CMSMediaWidgetConfigurationModel GetMediaWidgetDetails(WebStoreWidgetParameterModel parameter, string key, ExpandCollection expands);
        
        /// <summary>
        /// Get link widget data.
        /// </summary>
        /// <param name="parameter">Web Store Widget Parameter Model.</param>
        /// <param name="key">Unique key for a widget.</param>
        /// <returns>Returns LinkWidgetConfigurationListModel containing link widget data.</returns>
        LinkWidgetConfigurationListModel GetLinkWidget(WebStoreWidgetParameterModel parameter, string key);

        /// <summary>
        /// Get categories data.
        /// </summary>
        /// <param name="parameter">Web Store Widget Parameter Model.</param>
        /// <param name="key">Unique key for a widget.</param>
        /// <returns>Returns WebStoreWidgetCategoryListModel containing category list.</returns>
        WebStoreWidgetCategoryListModel GetCategories(WebStoreWidgetParameterModel parameter, string key);

        /// <summary>
        /// Get link product list.
        /// </summary>
        /// <param name="parameter">WebStoreWidgetParameterModel model.</param>
        /// <param name="key">Unique key for a widget.</param>
        /// <param name="expands">Expand Collection.</param>
        /// <returns>Returns WebStoreLinkProductListModel.</returns>
        WebStoreLinkProductListModel GetLinkProductList(WebStoreWidgetParameterModel parameter, string key, ExpandCollection expands);

        /// <summary>
        /// Get tag manager data.
        /// </summary>
        /// <param name="parameter">Web Store Widget Parameter Model.</param>
        /// <param name="key">Unique key for a widget.</param>
        /// <returns>Returns WebStoreWidgetCategoryListModel containing tag manager data.</returns>
        CMSTextWidgetConfigurationModel GetTagManager(WebStoreWidgetParameterModel parameter, string key);

        /// <summary>
        /// Get brands data.
        /// </summary>
        /// <param name="parameter">Web Store Widget Parameter Model.</param>
        /// <param name="key">Unique key for a widget.</param>
        /// <returns>Returns WebStoreWidgetBrandListModel containing brand list.</returns>
        WebStoreWidgetBrandListModel GetBrands(WebStoreWidgetParameterModel parameter, string key);

        /// <summary>
        /// Get Form Attribute Configuration.
        /// </summary>
        /// <param name="parameter">Web Store Widget Parameter Model.</param>
        /// <returns>return WebStoreWidgetFormParameters</returns>
        WebStoreWidgetFormParameters GetFormConfiguration(WebStoreWidgetParameterModel parameter);

        /// <summary>
        ///Get search widget products ad facets.
        /// </summary>
        /// <param name="webStoreWidgetParameterModel">model with widget parameter</param>
        /// <param name="expands">expands to get other data.</param>
        /// <param name="filters">filter for product list.</param>
        /// <param name="sortCollection">Sort collection for sorting</param>
        /// <returns>Widget data.</returns>
        WebStoreWidgetSearchModel GetSearchWidgetData(WebStoreSearchWidgetParameterModel webStoreWidgetParameterModel, ExpandCollection expands, FilterCollection filters, SortCollection sortCollection);

        /// <summary>
        /// Get total available balance of ECertificate for given username
        /// </summary>
        /// <param name="parameter">model with widget parameter</param>
        /// <param name="userName">Logged in username</param>
        /// <returns>Total available balance of ECertificate</returns>
        ECertTotalBalanceModel GetECertTotalBalance(WebStoreWidgetParameterModel parameter, string userName, string email);

        /// <summary>
        /// Get Container details.
        /// </summary>
        /// <param name="parameter">Web Store Widget Parameter Model.</param>
        /// <returns>Returns ContainerKey</returns>
        string GetContainer(WebStoreWidgetParameterModel parameter);
    }
}
