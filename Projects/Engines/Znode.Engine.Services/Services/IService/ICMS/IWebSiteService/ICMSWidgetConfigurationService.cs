using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface ICMSWidgetConfigurationService
    {
        #region Text Widget Configuration
        /// <summary>
        /// Get the list of Text Widget Configuration.
        /// </summary>
        /// <param name="expands">List of expands tuples</param>
        /// <param name="filters">List of filter tuples</param>
        /// <param name="sorts">Collection of sorting parameters</param>
        /// <param name="page">Collection of paging parameters</param>
        /// <returns>List of Text Widget Configuration</returns>
        CMSTextWidgetConfigurationListModel GetTextWidgetConfigurationList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get Text Widget Configuration on the basis of Widget Configuration id.
        /// </summary>
        /// <param name="textWidgetConfigurationId">Widget Configuration Id.</param>
        /// <returns>Returns Text Widget Configuration Model.</returns>
        CMSTextWidgetConfigurationModel GetTextWidgetConfiguration(int textWidgetConfigurationId);

        /// <summary>
        /// Create Text Widget Configuration.
        /// </summary>
        /// <param name="model">Text Widget Configuration Model.</param>
        /// <returns>Returns created Text Widget Configuration Model.</returns>
        CMSTextWidgetConfigurationModel CreateTextWidgetConfiguration(CMSTextWidgetConfigurationModel model);

        /// <summary>
        /// Update Text Widget Configuration data.
        /// </summary>
        /// <param name="model">Text Widget Configuration model to update.</param>
        /// <returns>Returns true if model updated successfully else return false.</returns>
        bool UpdateTextWidgetConfiguration(CMSTextWidgetConfigurationModel model);
        #endregion

        /// <summary>
        /// Save and update media configurations
        /// </summary>
        /// <param name="model"></param>
        /// <returns>CMSMediaWidgetConfigurationModel model</returns>
        CMSMediaWidgetConfigurationModel SaveAndUpdateMediawidgetConfiguration(CMSMediaWidgetConfigurationModel model);

        /// <summary>
        /// Remove media configurations
        /// </summary>
        /// <param name="CmsContainerWidgetConfigurationModel">removeWidgetConfigurationModel</param>
        /// <returns>true/false</returns>
        bool RemoveWidgetDataFromContentPageConfiguration(CmsContainerWidgetConfigurationModel removeWidgetConfigurationModel);

        #region Form Widget Configration

        /// <summary>
        /// Get Form widget Configuration list.
        /// </summary>
        /// <param name="expands">expands</param>
        /// <param name="filters">filters</param>
        /// <param name="sorts">sorts</param>
        /// <param name="page">page</param>
        /// <returns>return CMSFormWidgetConfigurationListModel</returns>
        CMSFormWidgetConfigurationListModel GetFormWidgetConfigurationList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Create Form Widget Configuration.
        /// </summary>
        /// <param name="model">Form Widget Configuration Model.</param>
        /// <returns>CMSFormWidgetConfigrationModel</returns>
        CMSFormWidgetConfigrationModel CreateFormWidgetConfiguration(CMSFormWidgetConfigrationModel model);

        /// <summary>
        /// Update Form Widget Configuration data.
        /// </summary>
        /// <param name="model">Form Widget Configuration model to update.</param>
        /// <returns>Returns true if model updated successfully else return false.</returns>
        bool UpdateFormWidgetConfiguration(CMSFormWidgetConfigrationModel model);

        #endregion

        #region CMSWidgetProduct
        /// <summary>
        /// Get associated Publish Product list.
        /// </summary>
        /// <param name="expands">Expands for associated product.</param>
        /// <param name="filters">Filters for associated product.</param>
        /// <param name="sorts">Sorts for for associated product.</param>
        /// <param name="page">Page size.</param>
        /// <returns>Returns list of associated products .</returns>
        CMSWidgetProductListModel GetAssociatedProductList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Get un associated Publish Product list.
        /// </summary>
        /// <param name="expands">Expands for associated product.</param>
        /// <param name="filters">Filters for associated product.</param>
        /// <param name="sorts">Sorts for for associated product.</param>
        /// <param name="page">Page size.</param>
        /// <returns>Returns list of unassociated products .</returns>
        ProductDetailsListModel GetUnAssociatedProductList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Associate Publish Product to the Widget.
        /// </summary>
        /// <param name="cmsWidgetProductListModel">cmsWidgetProductListModel</param>
        /// <returns>Returns true if product associated successfully else return false.</returns>
        bool AssociateProduct(CMSWidgetProductListModel cmsWidgetProductListModel);

        /// <summary>
        /// Un associate associated Publish Products to the Widget.
        /// </summary>
        /// <param name="cmsWidgetProductId">ParameterModel contains cmsWidgetProductId to un associate products .</param>
        /// <returns>Returns true if product un associated successfully else return false.</returns>
        bool UnassociateProduct(ParameterModel cmsWidgetProductId);


        /// <summary>
        /// Update CMSWidget Product.
        /// </summary>
        /// <param name="ProductDetailsModel"></param>
        /// <returns>true or false response</returns>
        bool UpdateCMSAssociateProduct(ProductDetailsModel productDetailsModel);



        #endregion

        #region CMS Widget Slider Banner
        /// <summary>
        /// Get the CMS Widget Slider Banner Details.
        /// </summary>
        /// <param name="filters">List of filter tuples</param>
        /// <returns>CMSWidgetSliderBanner details.</returns>
        CMSWidgetConfigurationModel GetCMSWidgetSliderBanner(FilterCollection filters);

        /// <summary>
        /// Save New CMS Widget Slider Banner Details.
        /// </summary>
        /// <param name="model">CMSWidgetConfigurationModel</param>
        /// <returns>Returns true or false.</returns>
        bool SaveCMSWidgetSliderBanner(CMSWidgetConfigurationModel model);
        #endregion

        #region Link Widget Configuration
        /// <summary>
        /// Create Link Widget Configuration.
        /// </summary>
        /// <param name="model">LinkWidgetConfigurationModel</param>
        /// <returns>Return LinkWidgetConfigurationModel.</returns>
        LinkWidgetConfigurationModel CreateUpdateLinkWidgetConfiguration(LinkWidgetConfigurationModel model);

        /// <summary>
        /// Get Link Widget Configuration List
        /// </summary>
        /// <param name="expands">expands for Link Widget Configuration.</param>
        /// <param name="filters">Filter Collection for Link Widget Configuration.</param>
        /// <param name="sortCollection">sorts for Link Widget Configuration.</param>
        /// <param name="pageIndex">Index of requesting page</param>
        /// <param name="recordPerPage">Record per page</param>
        /// <returns>list of Link Widget Configuration.</returns>
        LinkWidgetConfigurationListModel LinkWidgetConfigurationList(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Delete Link Widget Configuration.
        /// </summary>
        /// <param name="parameterModel">model with ids</param>
        /// <param name="localeId">localeId</param>
        /// <returns>true/false</returns>
        bool DeleteLinkWidgetConfiguration(ParameterModel parameterModel, int localeId);
        #endregion

        #region Category Association
        /// <summary>
        /// Get list of un associate Publish Categories.
        /// </summary>
        /// <param name="expands">expand for category.</param>
        /// <param name="filters">filter for category.</param>
        /// <param name="sorts">sorts for category.</param>
        /// <param name="pageIndex">pageindex</param>
        /// <param name="pageSize">pagesize</param>
        /// <returns>CategoryListModel</returns>
        CategoryListModel GetUnAssociatedCategories(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Remove associated Publish Categories.
        /// </summary>
        /// <param name="cmsWidgetCategoryId">Ids to delete associated categories.</param>
        /// <returns></returns>
        bool DeleteCategories(ParameterModel cmsWidgetCategoryId);

        /// <summary>
        /// Get associated Publish Category list.
        /// </summary>
        /// <param name="expands">expand</param>
        /// <param name="filters">filter for category</param>
        /// <param name="sorts">sorts</param>
        /// <param name="pageIndex">pageindex</param>
        /// <param name="pageSize">pagesize</param>
        /// <returns></returns>
        CategoryListModel GetAssociatedCategories(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Associate Publish Categories.
        /// </summary>
        /// <param name="model">model with cmsWidgetsIds and multiple publish category ids.</param>
        /// <returns>true or false response</returns>
        bool AssociateCategories(ParameterModelForWidgetCategory model);

        /// <summary>
        /// Update CMSWidget Category.
        /// </summary>
        /// <param name="categoryModel"></param>
        /// <returns>true or false response</returns>
        bool UpdateCMSWidgetCategory(CategoryModel categoryModel);
        #endregion

        #region Brand Association
        /// <summary>
        /// Get list of unassociated brands.
        /// </summary>
        /// <param name="expands">expand for brands.</param>
        /// <param name="filters">filter for brands.</param>
        /// <param name="sorts">sorts for brands.</param>
        /// <param name="pageIndex">pageindex</param>
        /// <param name="pageSize">pagesize</param>
        /// <returns>BrandListModel</returns>
        BrandListModel GetUnAssociatedBrands(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Remove associated brands.
        /// </summary>
        /// <param name="cmsWidgetBrandId">Ids to delete associated brands.</param>
        /// <returns>Returns true if brands removed successfully else returns false.</returns>
        bool DeleteBrands(ParameterModel cmsWidgetBrandId);

        /// <summary>
        /// Get associated brand list.
        /// </summary>
        /// <param name="expands">expand for brands.</param>
        /// <param name="filters">filter for brands.</param>
        /// <param name="sorts">sorts for brands.</param>
        /// <param name="pageIndex">pageindex</param>
        /// <param name="pageSize">pagesize</param>
        /// <returns>Returns associated brands.</returns>
        BrandListModel GetAssociatedBrands(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection page);

        /// <summary>
        /// Associate brands.
        /// </summary>
        /// <param name="model">Model with cmsWidgetsIds and multiple brand ids.</param>
        /// <returns>Returns true if brands associated successfully else returns false.</returns>
        bool AssociateBrands(ParameterModelForWidgetBrand model);


        /// <summary>
        /// Update CMSWidget Brand.
        /// </summary>
        /// <param name="brandModel"></param>
        /// <returns>true or false response</returns>
        bool UpdateCMSWidgetBrand(BrandModel brandModel);


        #endregion

        #region Form Widget Email Configuration
        /// <summary>
        /// Use to create Form Widget Email Configuration
        /// </summary>
        /// <param name="model"></param>
        /// <returns></returns>
        FormWidgetEmailConfigurationModel CreateFormWidgetEmailConfiguration(FormWidgetEmailConfigurationModel model);

        /// <summary>
        /// Update Form Widget Email Configuration data.
        /// </summary>
        /// <param name="model">Form Widget Email Configuration model to update.</param>
        /// <returns>Returns true if model updated successfully else return false.</returns>
        bool UpdateFormWidgetEmailConfiguration(FormWidgetEmailConfigurationModel model);

        /// <summary>
        /// Get Form Widget Email Configuration
        /// </summary>
        /// <param name="cMSContentPagesId"></param>
        /// <param name="expands"></param>
        /// <returns></returns>
        FormWidgetEmailConfigurationModel GetFormWidgetEmailConfiguration(int cMSContentPagesId, NameValueCollection expands);



        #region Search Widget Configuration
        /// <summary>
        /// Get the search widget configuration.
        /// </summary>
        /// <param name="filters">filter sto get widget configuration</param>
        /// <param name="expands">Expands to get data from related table.</param>
        /// <returns>Search widget configuration</returns>
        CMSSearchWidgetConfigurationModel GetSearchWidgetConfiguration(FilterCollection filters, NameValueCollection expands);

        /// <summary>
        /// Create the new search widget configuration
        /// </summary>
        /// <param name="cMSSearchWidgetConfigurationModel">model for create search widget.</param>
        /// <returns>model with the created search widget configuration.</returns>
        CMSSearchWidgetConfigurationModel CreateSearchWidgetConfiguration(CMSSearchWidgetConfigurationModel model);

        /// <summary>
        /// Update search widget configuration.
        /// </summary>
        /// <param name="model">model for update search widget.</param>
        /// <returns>true or false depending upon the status</returns>
        bool UpdateSearchWidgetConfiguration(CMSSearchWidgetConfigurationModel model);
        #endregion
        #endregion

        /// <summary>
        /// Save the CMS Widget Content Container Details.
        /// </summary>
        /// <param name="model">CMSContainerWidgetConfigurationModel</param>
        /// <returns>Returns true or false.</returns>
        bool SaveCmsContainerDetails(CmsContainerWidgetConfigurationModel model);
    }
}
