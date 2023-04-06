using Znode.Engine.Api.Client.Expands;
using Znode.Engine.Api.Client.Sorts;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface ICMSWidgetConfigurationClient :IBaseClient
    {
       
        /// Get the list of Text Widget Configuration.
        /// </summary>
        /// <param name="expands">Expand Collection for Widget Configuration list.</param>
        /// <param name="filters">Filter Collection for Widget Configuration list.</param>
        /// <param name="sorts">Sort collection of Widget Configuration list.</param>
        /// <returns>List of Text Widget Configuration</returns>
        CMSTextWidgetConfigurationListModel GetTextWidgetConfigurationList(ExpandCollection expands, FilterCollection filters, SortCollection sorts);

        /// <summary>
        /// Get Text Widget Configuration on the basis of Widget Configuration Id.
        /// </summary>
        /// <param name="textWidgetConfigurationId">Widget Configuration Id to get Widget Configuration details.</param>
        /// <returns>Returns Text Widget Configuration Model.</returns>
        CMSTextWidgetConfigurationModel GetTextWidgetConfiguration(int textWidgetConfigurationId);

        
        /// <summary>
        /// Create Widget Configuration.
        /// </summary>
        /// <param name="model"> Text Widget Configuration Model.</param>
        /// <returns>Returns created Text Widget Configuration Model.</returns>
        CMSTextWidgetConfigurationModel CreateTextWidgetConfiguration(CMSTextWidgetConfigurationModel model);

        /// <summary>
        /// Update Configuration data.
        /// </summary>
        /// <param name="model">Text Widget Configuration to update.</param>
        /// <returns>Returns updated Text Widget Configuration model.</returns>
        CMSTextWidgetConfigurationModel UpdateTextWidgetConfiguration(CMSTextWidgetConfigurationModel model);

        #region Media Widget Configration 
        /// <summary>
        /// Save and Update Media Configuration Data
        /// </summary>
        /// <param name="model">Media Widget Configuration Model.</param>
        /// <returns>CMSMediaWidgetConfigurationModel model</returns>
        CMSMediaWidgetConfigurationModel SaveAndUpdateMediaWidgetConfiguration(CMSMediaWidgetConfigurationModel model);
        #endregion

        #region Remove Widget Configuration Data
        /// <summary>
        /// Remove Widget Configuration Data
        /// </summary>
        /// <param name="CmsContainerWidgetConfigurationModel">removeWidgetConfigurationModel</param>
        /// <returns>true/false</returns>
        bool RemoveWidgetDataFromContentPageConfiguration(CmsContainerWidgetConfigurationModel removeWidgetConfigurationModel);
        #endregion

        #region Form Widget Configration  

        /// <summary>
        /// Get the list of Form Widget Configuration.
        /// </summary>
        /// <param name="expands">Expand Collection for Widget Configuration list.</param>
        /// <param name="filters">Filter Collection for Widget Configuration list.</param>
        /// <param name="sorts">Sort collection of Widget Configuration list.</param>
        /// <returns>List of Form Widget Configuration</returns>
        CMSFormWidgetConfigurationListModel GetFormWidgetConfigurationList(ExpandCollection expands, FilterCollection filters, SortCollection sorts);
        

        /// <summary>
        /// Create Form Widget Configuration.
        /// </summary>
        /// <param name="model"> Form Widget Configuration Model.</param>
        /// <returns>Returns created Text Widget Configuration Model.</returns>
        CMSFormWidgetConfigrationModel CreateFormWidgetConfiguration(CMSFormWidgetConfigrationModel model);

        /// <summary>
        /// Update Form Configuration data.
        /// </summary>
        /// <param name="model">Form Widget Configuration to update.</param>
        /// <returns>Returns updated Form Widget Configuration model.</returns>
        CMSFormWidgetConfigrationModel UpdateFormWidgetConfiguration(CMSFormWidgetConfigrationModel model);
        #endregion

        #region CMS Widget Slider Banner
        /// <summary>
        /// Get the CMS Widget Slider Banner Details.
        /// </summary>
        /// <param name="filters">Filter Collection for Widgets list.</param>
        /// <returns>CMSWidgetSliderBanner details.</returns>
        CMSWidgetConfigurationModel GetCMSWidgetSliderBanner(FilterCollection filters);

        /// <summary>
        /// Save New CMS Widget Slider Banner Details.
        /// </summary>
        /// <param name="model">CMSWidgetConfigurationModel</param>
        /// <returns>CMSWidgetConfigurationModel</returns>
        CMSWidgetConfigurationModel SaveCMSWidgetSliderBanner(CMSWidgetConfigurationModel model);
        #endregion

        #region Link Widget Configuration
        /// <summary>
        /// Create link widget configuration.
        /// </summary>
        /// <param name="model">LinkWidgetConfigurationModel</param>
        /// <returns>Returns LinkWidgetConfigurationModel.</returns>
        LinkWidgetConfigurationModel CreateUpdateLinkWidgetConfiguration(LinkWidgetConfigurationModel model);

        /// <summary>
        /// Get link widget configuration.
        /// </summary>
        /// <param name="expands">expands for link widget configuration.</param>
        /// <param name="filters">Filter Collection for link widget configuration.</param>
        /// <param name="sortCollection">sorts for link widget configuration.</param>
        /// <param name="pageIndex">Index of requesting page</param>
        /// <param name="recordPerPage">Record per page</param>
        /// <returns>list of Link Widget Configuration.</returns>
        LinkWidgetConfigurationListModel LinkWidgetConfigurationList(ExpandCollection expands, FilterCollection filters, SortCollection sortCollection, int pageIndex, int recordPerPage);

        /// <summary>
        /// Delete link widget configuration.
        /// </summary>
        /// <param name="cmsWidgetTitleConfigurationIds">model with ids</param>
        /// <param name="localeId">localeId</param>
        /// <returns>true/false</returns>
        bool DeleteLinkWidgetConfiguration(ParameterModel cmsWidgetTitleConfigurationIds, int localeId);
        #endregion

        #region Category Association.
        /// <summary>
        /// Get list of unassociate categories.
        /// </summary>
        /// <param name="expands">expand for category.</param>
        /// <param name="filters">filter for category.</param>
        /// <param name="sorts">sorts for category</param>
        /// <param name="pageIndex">pageindex.</param>
        /// <param name="pageSize">pagesize.</param>
        /// <returns>UnAssociated category list.</returns>
        CategoryListModel GetUnAssociatedCategoryList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// get associated category list based on cms widgets.
        /// </summary>
        /// <param name="expands">expand for category.</param>
        /// <param name="filters">filter for category.</param>
        /// <param name="sorts">sorts for category.</param>
        /// <param name="pageIndex">pageindex.</param>
        /// <param name="pageSize">pagesize.</param>
        /// <returns>Associated category list.</returns>
        CategoryListModel GetAssociatedCategorylist(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Remove associated categories.
        /// </summary>
        /// <param name="cmsWidgetCategoryId">cmsWidgetCategoryId to delete data.</param>
        /// <returns>true/or false response.</returns>
        bool DeleteCategories(ParameterModel cmsWidgetCategoryId);

        /// <summary>
        /// Associate categories.
        /// </summary>
        /// <param name="parameterModel">model with cms widget ids and publish category ids.</param>
        /// <returns>true/false response</returns>
        bool AssociateCategories(ParameterModelForWidgetCategory parameterModel);

        /// <summary>
        /// Update CMSWidget Category.
        /// </summary>
        /// <param name="categoryModel"></param>
        /// <returns>true/false response</returns>
        CategoryModel UpdateCMSWidgetCategory(CategoryModel categoryModel);
        #endregion

        #region Brand Association.
        /// <summary>
        /// Get list of unassociated brands.
        /// </summary>
        /// <param name="expands">Expand for brand.</param>
        /// <param name="filters">Filter for brand.</param>
        /// <param name="sorts">Sorts for brand</param>
        /// <param name="pageIndex">Pageindex.</param>
        /// <param name="pageSize">Pagesize.</param>
        /// <returns>Unassociated brand list.</returns>
        BrandListModel GetUnAssociatedBrandList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get associated brand list based on cms widgets.
        /// </summary>
        /// <param name="expands">Expand for category.</param>
        /// <param name="filters">Filter for category.</param>
        /// <param name="sorts">Sorts for category.</param>
        /// <param name="pageIndex">Pageindex.</param>
        /// <param name="pageSize">Pagesize.</param>
        /// <returns>Associated brand list.</returns>
        BrandListModel GetAssociatedBrandlist(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Remove associated brands.
        /// </summary>
        /// <param name="cmsWidgetBrandId">Brand Id to delete data.</param>
        /// <returns>true/or false response.</returns>
        bool DeleteBrands(ParameterModel cmsWidgetBrandId);

        /// <summary>
        /// Associate brands.
        /// </summary>
        /// <param name="parameterModel">Model with cms widget ids and brand ids.</param>
        /// <returns>True/False Response.</returns>
        bool AssociateBrands(ParameterModelForWidgetBrand parameterModel);

        /// <summary>
        /// Update CMSWidget Brand.
        /// </summary>
        /// <param name="brandModel"></param>
        /// <returns>true/false response</returns>
        BrandModel UpdateCMSWidgetBrand(BrandModel brandModel);

        #endregion

        #region CMSWidgetProduct
        /// <summary>
        /// Get associated product list .
        /// </summary>
        /// <param name="expands">Expands for associated product.</param>
        /// <param name="filters">Filters for associated product.</param>
        /// <param name="sorts">Sorts for for associated product.</param>
        /// <param name="pageIndex">Index of page.</param>
        /// <param name="pageSize">Size of page.</param>
        /// <returns>Returns list of associated products .</returns>
        CMSWidgetProductListModel GetAssociatedProductList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get unassociated product list .
        /// </summary>
        /// <param name="expands">Expands for associated product.</param>
        /// <param name="filters">Filters for associated product.</param>
        /// <param name="sorts">Sorts for for associated product.</param>
        /// <param name="pageIndex">Index of page.</param>
        /// <param name="pageSize">Size of page.</param>
        /// <returns>Returns list of unassociated products .</returns>
        ProductDetailsListModel GetUnAssociatedProductList(ExpandCollection expands, FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Associate product.
        /// </summary>
        /// <param name="cmsWidgetProductListModel">cmsWidgetProductListModel</param>
        /// <returns>Returns true if product associated successfully else return false.</returns>
        bool AssociateProduct(CMSWidgetProductListModel cmsWidgetProductListModel);

        /// <summary>
        /// Unassociate associated products .
        /// </summary>
        /// <param name="cmsWidgetProductId">ParameterModel contains cmsWidgetProductId to unassociate products .</param>
        /// <returns>Returns true if portals unassociated successfully else return false.</returns>
        bool UnassociateProduct(ParameterModel cmsWidgetProductId);


        /// <summary>
        /// Update CMSWidget Product .
        /// </summary>
        /// <param name="productDetailsModel"></param>
        /// <returns>true/false response</returns>
        ProductDetailsModel UpdateCMSAssociateProduct(ProductDetailsModel productDetailsModel);


        #endregion


        #region Form Widget Email Configuration

        /// <summary>
        /// Get the list of Form Widget Configuration.
        /// </summary>
        /// <param name="expands">Expand Collection for Widget Configuration list.</param>
        /// <param name="filters">Filter Collection for Widget Configuration list.</param>
        /// <param name="sorts">Sort collection of Widget Configuration list.</param>
        /// <returns>List of Form Widget Configuration</returns>
        FormWidgetEmailConfigurationModel GetFormWidgetEmailConfiguration(int cMSContentPagesId);

        /// <summary>
        /// Create Form Widget Configuration.
        /// </summary>
        /// <param name="model"> Form Widget Configuration Model.</param>
        /// <returns>Returns created Text Widget Configuration Model.</returns>
        FormWidgetEmailConfigurationModel CreateFormWidgetEmailConfiguration(FormWidgetEmailConfigurationModel model);

        /// <summary>
        /// Update Form Configuration data.
        /// </summary>
        /// <param name="model">Form Widget Configuration to update.</param>
        /// <returns>Returns updated Form Widget Configuration model.</returns>
        FormWidgetEmailConfigurationModel UpdateFormWidgetEmailConfiguration(FormWidgetEmailConfigurationModel model);

        /// <summary>
        /// Get the search widget configuration.
        /// </summary>
        /// <param name="filters">filter sto get widget configuration</param>
        /// <param name="expands">Expands to get data from related table.</param>
        /// <returns></returns>
        CMSSearchWidgetConfigurationModel GetSearchWidgetConfiguration(FilterCollection filters, ExpandCollection expands);

        /// <summary>
        /// Create the new search widget configuration
        /// </summary>
        /// <param name="cmsSearchWidgetConfigurationModel">model for create search widget.</param>
        /// <returns>model with the created search widget configuration.</returns>
        CMSSearchWidgetConfigurationModel CreateSearchWidgetConfiguration(CMSSearchWidgetConfigurationModel cmsSearchWidgetConfigurationModel);

        /// <summary>
        /// Update search widget configuration.
        /// </summary>
        /// <param name="cmsSearchWidgetConfigurationModel">model for update search widget.</param>
        /// <returns>model with the updated search widget configuration.</returns>
        CMSSearchWidgetConfigurationModel UpdateSearchWidgetConfiguration(CMSSearchWidgetConfigurationModel cmsSearchWidgetConfigurationModel);
        #endregion

        /// <summary>
        /// Save the CMS Widget Content Container Details..
        /// </summary>
        /// <param name="configurationViewModel">CMSContainerWidgetConfigurationModel</param>
        /// <returns>CmsContainerWidgetConfigurationModel</returns>
        CmsContainerWidgetConfigurationModel SaveCmsContainerDetails(CmsContainerWidgetConfigurationModel configurationViewModel);
    }
}
