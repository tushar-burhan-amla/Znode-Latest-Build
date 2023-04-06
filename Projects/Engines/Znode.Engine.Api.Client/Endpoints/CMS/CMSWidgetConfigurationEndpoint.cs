namespace Znode.Engine.Api.Client.Endpoints
{
    public class CMSWidgetConfigurationEndpoint : BaseEndpoint
    {
        //Get CMS Text Widget Configuration List Endpoint
        public static string TextWidgetConfigurationList() => $"{ApiRoot}/textwidgetconfiguration/list";

        //Get CMS Text Widget Configuration Endpoint
        public static string GetTextWidgetConfiguration(int textWidgetConfigurationId) => $"{ApiRoot}/textwidgetconfiguration/{textWidgetConfigurationId}";

        //Create CMS Text Widget Configuration Endpoint
        public static string CreateTextWidgetConfiguration() => $"{ApiRoot}/textwidgetconfiguration/create";

        //Update CMS Text Widget Configuration Endpoint
        public static string UpdateTextWidgetConfiguration() => $"{ApiRoot}/textwidgetconfiguration/update";

        //Save and update media details
        public static string SaveAndUpdateMediaWidgetConfiguration() => $"{ApiRoot}/mediawidgetconfiguration/saveandupdate";

        //Remove Widget details
        public static string RemoveWidgetDataFromContentPageConfiguration() => $"{ApiRoot}/cmswidgetConfiguration/removewidgetdatafromcontentpageconfiguration";

        #region CMS Form Widget Configration 

        //Get CMS Form Widget Configuration List Endpoint
        public static string FormWidgetConfigurationList() => $"{ApiRoot}/formwidgetconfiguration/list";

        //Create CMS Form Widget Configuration Endpoint
        public static string CreateFormWidgetConfiguration() => $"{ApiRoot}/formwidgetconfiguration/create";

        //Update CMS Form Widget Configuration Endpoint
        public static string UpdateFormWidgetConfiguration() => $"{ApiRoot}/formwidgetconfiguration/update";

        //Get CMS Form Widget Email Configuration List Endpoint
        public static string GetFormWidgetEmailConfiguration(int cmsContentPagesId) => $"{ApiRoot}/formwidgetemailconfiguration/getformwidgetemailconfiguration/{cmsContentPagesId}";

        //Create CMS Form Widget Email Configuration Endpoint
        public static string CreateFormWidgetEmailConfiguration() => $"{ApiRoot}/formwidgetemailconfiguration/create";

        //Update CMS Form Widget Email Configuration Endpoint
        public static string UpdateFormWidgetEmailConfiguration() => $"{ApiRoot}/formwidgetemailconfiguration/update";
        #endregion

        #region CMS Widget Slider Banner.
        //Get the CMS Widget Slider Banner Endpoint
        public static string GetCMSWidgetSliderBanner() => $"{ApiRoot}/websitewidgetsliderbanner/getcmswidgetsliderbanner";

        //Save CMS Widget Slider Banner Endpoint
        public static string SaveCMSWidgetSliderBanner() => $"{ApiRoot}/websitewidgetsliderbanner/savecmswidgetsliderbanner";
        #endregion

        #region Link Widget Configuration.
        //Create Link Widget Configuration Endpoint.
        public static string CreateUpdateLinkWidgetConfiguration() => $"{ApiRoot}/linkwidgetconfiguration/create";

        //Get Link Widget Configuration List Endpoint.
        public static string LinkWidgetConfigurationList() => $"{ApiRoot}/linkwidgetconfiguration/list";

        //Delete Link Widget Configuration Endpoint.
        public static string DeleteLinkWidgetConfiguration(int localeId) => $"{ApiRoot}/linkwidgetconfiguration/delete/{localeId}";
        #endregion

        #region Category Association
        //Get list of unassociate categories
        public static string GetUnassociatedCategoryList() => $"{ApiRoot}/cmswidgetcategory/getunassociatedcategory";

        //get associated category list
        public static string GetAssociatedCategorylist() => $"{ApiRoot}/cmswidgetcategory/getassociatedcategory";

        //Remove associated categories
        public static string DeleteCategories() => $"{ApiRoot}/cmswidgetcategory/deletecategories";

        //Associate categories.
        public static string AssociateCategories() => $"{ApiRoot}/cmswidgetcategory/associatecategories";

        //Update CMS Widget Category.
        public static string UpdateCMSWidgetCategory() => $"{ApiRoot}/cmswidgetcategory/updatecmswidgetcategory";
        #endregion

        #region Brand Association
        //Get list of unassociate brands.
        public static string GetUnassociatedBrandList() => $"{ApiRoot}/cmswidgetbrand/getunassociatedbrand";

        //Get associated brand list.
        public static string GetAssociatedBrandList() => $"{ApiRoot}/cmswidgetbrand/getassociatedbrand";

        //Remove associated brands.
        public static string DeleteBrands() => $"{ApiRoot}/cmswidgetbrand/deletebrands";

        //Associate brands.
        public static string AssociateBrands() => $"{ApiRoot}/cmswidgetbrand/associatebrands";

        //Update CMS Widget Brand.
        public static string UpdateCMSWidgetBrand() => $"{ApiRoot}/cmswidgetbrand/updatecmswidgetbrand";
        #endregion

        #region CMSWidgetProduct
        //CMSWidgetProduct List endpoint.
        public static string GetCMSOfferPageProductList() => $"{ApiRoot}/cmswidgetproduct/associatedproduct/list";

        //Get UnAssociated CMSWidgetProduct list.
        public static string GetUnAssociatedProductList() => $"{ApiRoot}/cmswidgetproduct/unassociatedproduct/list";

        //Associate product.
        public static string AssociateProduct() => $"{ApiRoot}/cmswidgetproduct/associateproduct";

        //unassociate product.
        public static string UnAssociateProduct() => $"{ApiRoot}/cmswidgetproduct/unassociateproduct";

        //Get Search widget configuration.
        public static string GetSearchWidgetConfiguration() => $"{ApiRoot}/getsearchwidgetconfiguration";

        //Create the search widget configuration.
        public static string CreateSearchWidgetConfiguration() => $"{ApiRoot}/searchwidgetconfiguration/create";

        //Update the search widget configuration.
        public static string UpdateSearchWidgetConfiguration() => $"{ApiRoot}/searchwidgetconfiguration/update";


        //Update CMS Widget Product.
        public static string UpdateCMSAssociateProduct() => $"{ApiRoot}/cmsassociateproduct/updatecmsassociateproduct";


        #endregion

        //Save the CMS Widget Content Container Details.
        public static string SaveCmsContainerDetails() => $"{ApiRoot}/websitewidgetsliderbanner/savecmscontainerdetails";

    }
}
