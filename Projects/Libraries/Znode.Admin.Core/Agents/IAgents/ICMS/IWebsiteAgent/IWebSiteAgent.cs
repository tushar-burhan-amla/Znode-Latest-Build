using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Client.Sorts;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Admin.Agents
{
    public interface IWebSiteAgent
    {
        /// <summary>
        /// Get the Portal List, for which the Themes are assigned.
        /// </summary>
        /// <param name="filters">Filters for Portal List.</param>
        /// <param name="sorts">Sorts for Portal List.</param>
        /// <param name="pageIndex">Start page index of Portal List.</param>
        /// <param name="recordPerPage">Record per page of Portal List.</param>
        /// <returns>Return the List of Portals.</returns>
        StoreListViewModel GetPortalList(FilterCollection filters, SortCollection sorts, int? pageIndex, int? pageSize);

        /// <summary>
        /// Get Web Site Logo Details by Portal Id.
        /// </summary>
        /// <param name="portalId">Id for the Portal</param>
        /// <returns>Return Portal Logo Details in WebSiteLogoViewModel Format.</returns>
        WebSiteLogoViewModel GetWebSiteLogoDetails(int portalId);

        /// <summary>
        /// Save the WebSite Logo Details
        /// </summary>
        /// <param name="model">Model of type WebSiteLogoViewModel</param>
        /// <returns>return true or false</returns>
        bool SaveWebSiteLogo(WebSiteLogoViewModel model);

        /// <summary>
        /// Get Portal Associated Widgets.
        /// </summary>
        /// <param name="cmsMappingId">cmsMappingId</param>
        /// <param name="typeOfMapping">typeOfMapping</param>
        /// <param name="templatePath">templatePath</param>
        /// <param name="displayName">displayName</param>
        /// <param name="fileName">fileName</param>
        /// <returns>Return CMSWidgetsListViewModel.</returns>
        CMSWidgetsListViewModel GetPortalAssociatedWidgets(int cmsMappingId, string typeOfMapping, string templatePath, string displayName, string fileName);

        /// <summary>
        /// Sets filter.
        /// </summary>
        /// <param name="filters">Filter collection.</param>
        /// <param name="Id">Filter Id.</param>
        /// <param name="filterKey">Filter Key.</param>
        void SetFilter(FilterCollection filters, int id, string filterKey);

        /// <summary>
        /// Publish CMS configuration
        /// </summary>
        /// <param name="portalId">PortalId</param>
        /// <returns>Return True/False</returns>
        bool Publish(int portalId, out string errorMessage,string targetPublishState = null, string publishContent = null);
        
        #region Link Widget Configuration
        /// <summary>
        /// Save link widget configuration.
        /// </summary>
        /// <param name="viewmodel"></param>
        /// <returns>view model link widget configuration data</returns>
        LinkWidgetDataViewModel CreateUpdateLinkWidgetConfiguration(LinkWidgetDataViewModel viewmodel, out string message);

        /// <summary>
        /// Get Link Widget Configuration List.
        /// </summary>
        /// <param name="model">filtercollection data model</param>
        /// <param name="mappingId">mappping Id</param>
        /// <param name="widgetId">widget id</param>
        /// <param name="widgetKey">widget key</param>
        /// <param name="mappingType">mapping Type</param>
        /// <param name="displayName">displayName</param>
        /// <returns>LinkWidgetConfigurationListViewModel</returns>
        LinkWidgetDataViewModel GetLinkWidgetConfigurationList(FilterCollectionDataModel model, int mappingId, int widgetId, string widgetKey, string mappingType,int localeId);

        /// <summary>
        /// Delete link widget configuration by Id.
        /// </summary>
        /// <param name="cmsWidgetTitleConfigurationIds">ids of link widget configuration.</param>
        /// <param name="localeId">localeId</param>
        /// <param name="message"></param>
        /// <returns>true/false</returns>
        bool DeleteLinkWidgetConfiguration(string cmsWidgetTitleConfigurationIds,int localeId, out string message);

        /// <summary>
        /// Map paramters to link widget configuration view model properties.
        /// </summary>
        /// <param name="cmsMappingId">cmsMappingId</param>
        /// <param name="cmsWidgetsId">cmsWidgetsId</param>
        /// <param name="widgetKey">widgetKey</param>
        /// <param name="typeOfMapping">typeOfMapping</param>
        /// <param name="displayName">displayName</param>
        /// <param name="widgetName">widgetName</param>
        /// <returns>Return the list of widget Configuration</returns>
        LinkWidgetConfigurationViewModel MapParametersOfLinkWidget(int cmsMappingId, int cmsWidgetsId, string widgetKey, string typeOfMapping, string displayName, string widgetName);

        /// <summary>
        /// Get the Link Widget Configuration Details based on the widgetConfiguration Id.
        /// </summary>
        /// <param name="widgetConfigurationId">widgetConfigurationId</param>
        /// <returns>Return the Widget configuration details.</returns>
        LinkWidgetConfigurationViewModel GetLinkWidgetConfigurationDetailById(int widgetConfigurationId, int localeId);
        #endregion

        #region Portal Product Page
        /// <summary>
        /// Get Product types and PDP templates.
        /// </summary>
        /// <param name="portalProductPageViewModel">PortalProductPageViewModel</param>
        /// <returns>Product types and PDP templates in portalProductPageViewModel.</returns>
        PortalProductPageViewModel GetPortalProductPage(PortalProductPageViewModel portalProductPageViewModel);

        /// <summary>
        /// Update PDP templates of portal product page.
        /// </summary>        
        /// <param name="portalProductPageViewModel">PortalProductPageViewModel</param>
        /// <returns>Returns true if updated successfully else return false.</returns>
        bool UpdatePortalProductPage(PortalProductPageViewModel portalProductPageViewModel);
        #endregion

        #region Category Association
        /// <summary>
        /// Get associated categories based on widgets.
        /// </summary>
        /// <param name="cmsWidgetsId">cmsWidgetsId</param>
        /// <returns>list model of category</returns>
        CategoryListViewModel GetAssociatedCategoryList(int cmsWidgetsId, int cmsMappingId, string widgetKey, string typeOFMapping, string widgetName, string displayName, FilterCollectionDataModel model);

        /// <summary>
        /// Get unassociated categories.
        /// </summary>
        /// <param name="cmsWidgetsId">cmsWidgetsId</param>
        /// <returns>list model of category</returns>
        CategoryListViewModel GetUnAssociatedCategoryList(int cmsWidgetsId, int cmsMappingId, string widgetKey, string typeOFMapping, string widgetName, string displayName, FilterCollectionDataModel model);

        /// <summary>
        ///Remove associated categories.
        /// </summary>
        /// <param name="cmsWidgetCategoryId">offer page ids.</param>
        /// <returns>return true or false</returns>
        bool RemoveAssociatedCategories(string cmsWidgetCategoryId);

        /// <summary>
        ///Associate categories.
        /// </summary>
        /// <param name="cmsWidgetsId">cmsWidgetsId</param>
        /// <param name="publishCategoryId"> mutiple categrories id.</param>
        /// <returns></returns>
        bool AssociateCategories(int cmsWidgetsId, string categoryCodes, int cmsMappingId, string widgetKey, string typeOFMapping);


        /// <summary>
        /// Update CMS Widget Category.
        /// </summary>
        /// <param name="categoryViewModel"></param>
        /// <returns>true or false</returns>
        bool EditCMSWidgetCategory(CategoryViewModel categoryViewModel);
        #endregion

        #region Brand Association
        /// <summary>
        /// Get associated brands based on widgets.
        /// </summary>
        /// <param name="cmsWidgetsId">cmsWidgetsId</param>
        /// <returns>Returns list model of brand.</returns>
        BrandListViewModel GetAssociatedBrandList(int cmsWidgetsId, int cmsMappingId, string widgetKey, string typeOFMapping, string widgetName, string displayName, FilterCollectionDataModel model);

        /// <summary>
        /// Get unassociated brands.
        /// </summary>
        /// <param name="cmsWidgetsId">cmsWidgetsId</param>
        /// <returns>Returns list model of brand.</returns>
        BrandListViewModel GetUnAssociatedBrandList(int cmsWidgetsId, int cmsMappingId, string widgetKey, string typeOFMapping, string widgetName, string displayName, FilterCollectionDataModel model);

        /// <summary>
        ///Remove associated brands.
        /// </summary>
        /// <param name="cmsWidgetBrandId">Brand Ids.</param>
        /// <returns>Returns true if associated brand removed successfully else return false.</returns>
        bool RemoveAssociatedBrands(string cmsWidgetBrandId);

        /// <summary>
        ///Associate brands.
        /// </summary>
        /// <param name="cmsWidgetsId">cmsWidgetsId</param>
        /// <param name="brandId"> Mutiple brand id.</param>
        /// <returns>Returns true if brand associated successfully else return false.</returns>
        bool AssociateBrands(int cmsWidgetsId, string brandId, int cmsMappingId, string widgetKey, string typeOFMapping);

       /// <summary>
       /// Edit brands details
       /// </summary>
       /// <param name="brandViewModel"></param>
       /// <returns></returns>
        bool EditCMSWidgetBrand(BrandViewModel brandViewModel);

        #endregion

        #region Form Widget Configuration

        /// <summary>
        /// Get Form Widget Configuration Details.
        /// </summary>
        /// <param name="mappingId">Id for the Seleted Portal/Content Page</param>
        /// <param name="widgetId">Id for the Seleted widget</param>
        /// <param name="widgetKey">Key for the Seleted Widget</param>
        /// <param name="mappingType">Type of the WidgetMapping</param>
        /// <returns>Return Configured Form Widget Configuration Details in CMSFormWidgetConfigurationViewModel format.</returns>
        CMSFormWidgetConfigurationViewModel GetFormWidgetConfiguration(int mappingId, int widgetId, string widgetKey, string mappingType, int localeId);

        /// <summary>
        /// Save the Form Widget Configuration Details
        /// </summary>
        /// <param name="viewModel">Model of type CMSFormWidgetConfigurationViewModel</param>
        /// <returns>return Form Widget Configuration Model</returns>
        CMSFormWidgetConfigurationViewModel SaveFormWidgetConfiguration(CMSFormWidgetConfigurationViewModel viewModel);
        #endregion

        /// <summary>
        /// Get Text Widget Configuration Details.
        /// </summary>
        /// <param name="mappingId">Id for the Seleted Portal/Content Page</param>
        /// <param name="widgetId">Id for the Seleted widget</param>
        /// <param name="widgetKey">Key for the Seleted Widget</param>
        /// <param name="mappingType">Type of the WidgetMapping</param>
        /// <returns>Return Configured Text Widget Configuration Details in CMSTextWidgetConfigurationViewModel format.</returns>
        CMSTextWidgetConfigurationViewModel GetTextWidgetConfiguration(int mappingId, int widgetId, string widgetKey, string mappingType, int localeId);

        /// <summary>
        /// Save and Update Media Details
        /// </summary>
        /// <param name="mappingId">mappingId</param>
        /// <param name="widgetId">widgetId</param>
        /// <param name="widgetKey">widgetKey</param>
        /// <param name="mappingType">mappingType</param>
        /// <param name="MediaId">MediaId</param>
        /// <param name="CMSMediaConfigurationId">CMSMediaConfigurationId</param>
        /// <returns></returns>
        CMSMediaWidgetConfigurationViewModel SaveAndUpdateMediaWidgetConfiguration(int mappingId, int widgetId, string widgetKey, string mappingType, int MediaId);

        /// <summary>
        /// Remove Widget Configuration Data
        /// </summary>
        /// <param name="mappingId">mappingId</param>
        /// <param name="widgetKey">widgetKey</param>
        /// <param name="widgetCode">widgetCode</param>
        /// <param name="message">message</param>
        /// <returns>true/false</returns>
        bool RemoveWidgetDataFromContentPage(int mappingId, string widgetKey, string widgetCode, out string message);

        /// <summary>
        /// Save the Text Widget Configuration Details
        /// </summary>
        /// <param name="model">Model of type CMSTextWidgetConfigurationViewModel</param>
        /// <returns>return Text Widget Configuration Model</returns>
        CMSTextWidgetConfigurationViewModel SaveTextWidgetConfiguration(CMSTextWidgetConfigurationViewModel model);

        #region WebSite Widget Configuration
        #region CMS Widget Slider Banner 
        /// <summary>
        /// Get Banner Slider Details.
        /// </summary>
        /// <param name="cmsMappingId">cmsMappingId for the Seleted Portal</param>
        /// <param name="cmsWidgetsId">cmsWidgetsId for the Seleted Widget</param>
        /// <param name="widgetsKey">Key of the Seleted Widget</param>
        /// <param name="typeOFMapping">typeOFMapping of the Seleted Widget</param>
        /// <returns>Return Configured Slider Banner Details in CMSWidgetConfigurationViewModel format.</returns>
        CMSWidgetConfigurationViewModel GetCMSWidgetSliderBanner(int cmsMappingId, int cmsWidgetsId, string widgetKey, string typeOFMapping,int? localeId);

        /// <summary>
        /// Save the CMS Widget Slider Banner Details.
        /// </summary>
        /// <param name="model">Model of type CMSWidgetConfigurationViewModel</param>
        /// <returns>return Widger Configuration Model</returns>
        CMSWidgetConfigurationViewModel SaveCMSWidgetSliderBanner(CMSWidgetConfigurationViewModel model);

        /// <summary>
        /// Get Form Widget Email Configuration
        /// </summary>
        /// <param name="mappingId"></param>
        /// <param name="localeId"></param>
        /// <returns></returns>
        FormWidgetEmailConfigurationViewModel GetFormWidgetEmailConfiguration(int cMSContentPagesId, int widgetId, string widgetKey, string mappingType , int localeId);


        /// <summary>
        /// Save the Form Widget Configuration Details
        /// </summary>
        /// <param name="viewModel">Model of type CMSFormWidgetConfigurationViewModel</param>
        /// <returns>return Form Widget Configuration Model</returns>
        FormWidgetEmailConfigurationViewModel SaveFormWidgetEmailConfiguration(FormWidgetEmailConfigurationViewModel viewModel);

        #endregion
        #endregion

        #region CMSWidgetProduct 
        /// <summary>
        /// Get associated product list .
        /// </summary>
        /// <param name="filters">Filters for associated product.</param>
        /// <param name="sorts">Sorts for for associated product.</param>
        /// <param name="pageIndex">Index of page.</param>
        /// <param name="pageSize">Size of page.</param>
        /// <returns>Returns list of associated products .</returns>
        CMSWidgetProductListViewModel GetAssociatedProductList(FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Get unassociated product list .
        /// </summary>
        /// <param name="filters">Filters for associated product.</param>
        /// <param name="sorts">Sorts for for associated product.</param>
        /// <param name="pageIndex">Index of page.</param>
        /// <param name="recordPerPage">Records per page.</param>
        /// <returns>Returns list of unassociated products .</returns>
        ProductDetailsListViewModel GetUnAssociatedProductList(FilterCollection filters = null, SortCollection sorts = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Associate product .
        /// </summary>
        /// <param name="cmsWidgetsId">cmsWidgetsId</param>
        /// <param name="cmsMappingId">cmsMappingId.</param>
        /// <param name="widgetKey">widgetKey .</param>
        /// <param name="typeOfMapping">typeOfMapping.</param>
        /// <param name="SKUs">SKU's to be associated.</param>
        /// <returns>Returns true if product associated successfully else return false.</returns>
        bool AssociateProduct(int cmsWidgetsId, int cmsMappingId, string widgetKey, string typeOfMapping, string SKUs);

        /// <summary>
        /// Unassociate associated products . 
        /// </summary>
        /// <param name="cmsWidgetProductId">cmsWidgetProductId to unassociate products.</param>
        /// <returns>Returns true if portals unassociated successfully else return false.</returns>
        bool UnassociateProduct(string cmsWidgetProductId);

        /// <summary>
        /// Edit Product details
        /// </summary>
        /// <param name="productDetailsViewModel"></param>
        /// <returns></returns>
        bool EditCMSAssociateProduct(ProductDetailsViewModel productDetailsViewModel);
        /// <summary>
        /// Sets filter for cmsWidgetsId, widgetsKey,cmsMappingId,typeOfMapping.
        /// </summary>
        /// <param name="filters">Filter collection.</param>
        /// <param name="inventoryListId">InventoryListId.</param>
        void SetFilters(FilterCollection filters, int cmsWidgetsId, string widgetsKey, int cmsMappingId, string typeOfMapping, int localeId = 0);

        /// <summary>
        /// Get the widget id by its code.
        /// </summary>
        /// <param name="widgetCode">Widget Code.</param>
        /// <returns>Returns Widget Id.</returns>
        int GetWidgetIdByCode(string widgetCode);

       
        #endregion

        #region Search Widget Configuration
        /// <summary>
        /// Get Text Widget Configuration Details.
        /// </summary>
        /// <param name="mappingId">Id for the Seleted Portal/Content Page</param>
        /// <param name="widgetId">Id for the Seleted widget</param>
        /// <param name="widgetKey">Key for the Seleted Widget</param>
        /// <param name="mappingType">Type of the WidgetMapping</param>
        /// <returns>Return Configured Text Widget Configuration Details in CMSTextWidgetConfigurationViewModel format.</returns>
        CMSSearchWidgetConfigurationViewModel GetSearchWidgetConfiguration(int mappingId, int widgetId, string widgetKey, string mappingType, int localeId);

        /// <summary>
        /// Save the Text Widget Configuration Details
        /// </summary>
        /// <param name="model">Model of type CMSTextWidgetConfigurationViewModel</param>
        /// <returns>return Text Widget Configuration Model</returns>
        CMSSearchWidgetConfigurationViewModel SaveSearchWidgetConfiguration(CMSSearchWidgetConfigurationViewModel model);
        #endregion

        /// <summary>
        /// Return the Preview feature enable or disabled.
        /// </summary>
        /// <returns></returns>
        bool IsPreviewGloballyEnabled();

        /// <summary>
        /// Check need to publish in preview or not
        /// </summary>
        /// <param name="typeOfMapping">typeOfMapping</param>
        /// <returns>Return true if its from cms page</returns>
        bool IsPreviewPerform(string typeOfMapping);

        /// <summary>
        /// List of Content Containers
        /// </summary>
        /// <param name="filters">filters</param>
        /// <param name="sortCollection">sortCollection</param>
        /// <param name="pageIndex">pageIndex</param>
        /// <param name="recordPerPage">recordPerPage</param>
        /// <returns>ContentContainerListViewModel model</returns>
        ContentContainerListViewModel ContainerList(FilterCollection filters = null, SortCollection sortCollection = null, int? pageIndex = null, int? recordPerPage = null);

        /// <summary>
        /// Save the CMS Widget Content Container Details.
        /// </summary>
        /// <param name="configurationViewModel">Model of type CmsContainerWidgetConfigurationViewModel</param>
        /// <returns>return Widget Configuration Model</returns>
        CmsContainerWidgetConfigurationViewModel SaveCmsContainerDetails(CmsContainerWidgetConfigurationViewModel configurationViewModel);

    }
}