using System;
using System.Collections.Generic;
using System.Threading.Tasks;

using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Engine.WebStore.ViewModels;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.WebStore.Agents
{
    public interface IWidgetDataAgent
    {
        /// <summary>
        /// Get slider data asynchronously.
        /// </summary>
        /// <param name="parameter">WidgetParameter parameter.</param>
        /// <returns>Returns slider data.</returns>
        Task<WidgetSliderBannerViewModel> GetSliderAsync(WidgetParameter parameter);

        /// <summary>
        /// Get Slider Banner Widget details.
        /// </summary>
        /// <param name="parameter">WidgetParameter.</param>
        /// <returns>Returns WidgetSliderBannerViewModel containing Slider Banner Widget details.</returns>
        WidgetSliderBannerViewModel GetSlider(WidgetParameter parameter);

        /// <summary>
        /// Get Product List Widget details.
        /// </summary>
        /// <param name="parameter">WidgetParameter.</param>
        /// <returns>Returns WidgetProductListViewModel containing product list details.</returns>
        WidgetProductListViewModel GetProducts(WidgetParameter parameter);

        /// <summary>
        /// Get Link Widget data.
        /// </summary>
        /// <param name="parameter">WidgetParameter.</param>
        /// <returns>Returns WidgetTitleListViewModel containing link widget details.</returns>
        WidgetTitleListViewModel GetLinkData(WidgetParameter parameter);

        /// <summary>
        /// Get Category List Widget data.
        /// </summary>
        /// <param name="parameter">WidgetParameter.</param>
        /// <returns>Returns WidgetCategoryListViewModel containing category list details.</returns>
        WidgetCategoryListViewModel GetCategories(WidgetParameter parameter);

        /// <summary>
        /// Get content page list.
        /// </summary>
        /// <param name="parameter">WidgetParameter.</param>
        /// <returns>List of content pages.</returns>
        ContentPageListViewModel GetContentPages(WidgetParameter parameter);

        /// <summary>
        /// Get content page data.
        /// </summary>
        /// <param name="widgetparameter">WidgetParameter.</param>
        /// <returns>Content page data.</returns>
        WidgetTextViewModel GetContent(WidgetParameter widgetparameter);


        /// <summary>
        /// Get media widget details
        /// </summary>
        /// <param name="widgetparameter"></param>
        /// <returns>WidgetMediaViewModel model</returns>
        WidgetMediaViewModel GetMediaWidgetDetails(WidgetParameter widgetparameter);

        /// <summary>
        /// Get product facets list.
        /// </summary>
        /// <param name="widgetparameter">WidgetParameter.</param>
        /// <param name="pageNum">Page start.</param>
        /// <param name="pageSize">No of records per page.</param>
        /// <param name="sortValue">Default sort value.</param>
        /// <returns>Gets the facet list.</returns>
        SearchResultViewModel GetFacetList(WidgetParameter widgetparameter, int pageNum = 1, int pageSize = -1, int sortValue = 0);

        /// <summary>
        /// Get Sub categories of selected category
        /// </summary>
        /// <param name="widgetparameter">WidgetParameter</param>
        /// <returns>Sub category list model</returns>
        List<CategoryViewModel> GetSubCategories(WidgetParameter widgetparameter);

        /// <summary>
        /// Get category product of selected category 
        /// </summary>
        /// <param name="widgetparameter">widgetparameter</param>
        /// <param name="sortValue">Default Sort value.</param>
        /// <returns>Category product list</returns>
        ProductListViewModel GetCategoryProducts(WidgetParameter widgetparameter, int pageNum = 1, int pageSize = 16, int sortValue = 0, bool isAssociatedCategoriesRequired = false);

        /// <summary>
        /// Get product quick view data.
        /// </summary>
        /// <param name="widgetparameter">WidgetParameter containing the product id.</param>
        /// <returns>Returns ProductViewModel containing product quick view details.</returns>
        ProductViewModel GetProductQuickView(WidgetParameter widgetparameter);

        /// <summary>
        /// Get product of search widget.
        /// </summary>
        /// <param name="widgetparameter">WidgetParameter.</param>
        /// <param name="pageNum">Page start.</param>
        /// <param name="pageSize">No of records per page.</param>
        /// <param name="sortValue">Default sort value.</param>
        /// <returns>Gets the facet list.</returns>
        WidgetSearchDataViewModel GetSearchWidgetData(WidgetParameter widgetparameter, int pageNum = 1, int pageSize = 16, int sortValue = 0);

        /// <summary>
        /// Get link product list.
        /// </summary>
        /// <param name="parameter">WidgetParameter parameter.</param>
        /// <returns>Returns list of link products.</returns>
        List<LinkProductViewModel> GetLinkProductList(WidgetParameter parameter);

        /// <summary>
        /// Get total count of cart items.
        /// </summary>
        /// <returns>Count of cart items.</returns>
        decimal GetCartCount();

        /// <summary>
        /// Get tag manager data.
        /// </summary>
        /// <param name="widgetparameter">WidgetParameter containing the product id.</param>
        /// <returns>Returns ProductViewModel containing product quick view details.</returns>
        WidgetTextViewModel GetTagManager(WidgetParameter widgetparameter);

        /// <summary>
        /// Get brand list widget data.
        /// </summary>
        /// <param name="parameter">WidgetParameter.</param>
        /// <returns>Returns WidgetBrandListViewModel containing brand list details.</returns>
        WidgetBrandListViewModel GetBrands(WidgetParameter parameter);

        /// <summary>
        /// Get Widget Form Builder Attribute ViewModel.
        /// </summary>
        /// <param name="parameter">parameter.</param>
        /// <returns>Returns WidgetFormConfigurationViewModel.</returns>
        WidgetFormConfigurationViewModel GetFormConfiguration(WidgetParameter parameter);

        /// <summary>
        /// Get total available balance
        /// </summary>
        /// <param name="parameter"></param>
        /// <returns></returns>
        ECertTotalBalanceViewModel GetECertTotalBalance(WidgetParameter parameter, decimal availableBalance = 0);

        /// <summary>
        /// Get request model.
        /// </summary>
        /// <param name="widgetparameter">widgetparameter</param>
        /// <param name="pageNum">pageNum</param>
        /// <param name="pageSize">pageSize</param>
        /// <returns>SearchRequestViewModel</returns>
        SearchRequestViewModel GetSearchRequestModel(WidgetParameter widgetparameter, int pageNum = 0, int pageSize = 16);

        /// <summary>
        /// Set Search Request Model
        /// </summary>
        /// <param name="widgetparameter">widgetparameter</param>
        /// <param name="pageNum">pageNum</param>
        /// <param name="pageSize">pageSize</param>
        /// <param name="searchTerm">searchTerm</param>
        /// <returns>SearchRequestViewModel</returns>
        SearchRequestViewModel SetSearchRequestModel(WidgetParameter widgetparameter, int pageNum, int pageSize, object searchTerm);

        /// <summary>
        /// Get Sort Collection.
        /// </summary>
        /// <param name="sort">sort</param>
        /// <returns>SortCollection</returns>
        SortCollection GetSortForSearch(int sort);

        /// <summary>
        /// Get Search Result
        /// </summary>
        /// <param name="searchRequestModel">searchRequestModel</param>
        /// <param name="filters">filters</param>
        /// <param name="isFacetCall">isFacetCall</param>
        /// <returns>KeywordSearchModel</returns>
        KeywordSearchModel GetSearchResult(SearchRequestViewModel searchRequestModel, FilterCollection filters, bool isFacetCall = false);


        /// <summary>
        /// Get Product Expand For Search
        /// </summary>
        /// <param name="isWithCategoryFacet">isWithCategoryFacet</param>
        /// <param name="isFacetCall">isFacetCall</param>
        /// <returns>ExpandCollection</returns>
        ExpandCollection GetProductExpandForSearch(bool isWithCategoryFacet, bool isFacetCall = false);

        /// <summary>
        /// Get Product Expands
        /// </summary>
        /// <returns>ExpandCollection</returns>
        ExpandCollection GetProductExpands();

        /// <summary>
        ///  Set Filter Parameter
        /// </summary>
        /// <param name="cmsMappingId">cmsMappingId</param>
        /// <param name="typeOfMapping">typeOfMapping</param>
        /// <param name="properties">properties</param>
        /// <param name="Facets">Facets</param>
        /// <returns>filter values for already added filter</returns>
        List<Tuple<string, List<KeyValuePair<string, string>>>> SetFilterParameter(int cmsMappingId, string typeOfMapping, Dictionary<string, object> properties, List<FacetViewModel> Facets);

        /// <summary>
        /// Set Filter Parameter
        /// </summary>
        /// <param name="parameter">WidgetParameter</param>
        /// <param name="viewModel">SearchResultViewModel</param>
        /// <param name="brandName">brandName</param>
        void SetFilterParameter(WidgetParameter parameter, SearchResultViewModel viewModel, string brandName = "");


        /// <summary>
        /// Get page default size
        /// </summary>
        /// <returns></returns>
        string GetPageDefaultSize();

        /// <summary>
        /// Get Default Page Size set at the store.
        /// </summary>
        /// <returns>Default Page Size</returns>
        int GetDefaultPageSize();

        /// <summary>
        /// Get Default sort value set at the store.
        /// </summary>
        /// <returns></returns>
        string GetPortalDefaultSortValue();

        #region CMS page search request.

        /// <summary>
        /// Get CMS Pages for the search keyword.
        /// </summary>
        /// <param name="widgetparameter">widgetparameter</param>
        /// <returns>Return CMS Pages from elastic search</returns>
        CMSPageListViewModel GetSearchCMSPages(WidgetParameter widgetparameter);

        /// <summary>
        /// Get CMS pages based on keyword search
        /// </summary>
        /// <param name="searchRequestModel">Request model</param>
        /// <param name="filters">filters</param>
        /// <returns>Return result from search</returns>
        CMSKeywordSearchModel FullTextContentPageSearch(CMSPageSearchRequestViewModel searchRequestModel,FilterCollection filters);

        #endregion

        /// <summary>
        /// Get Container Widget details.
        /// </summary>
        /// <param name="parameter">WidgetParameter.</param>
        /// <returns>Returns ContainerKey containing Container Widget details.</returns>
        string GetContainer(WidgetParameter parameter);

    }
}
