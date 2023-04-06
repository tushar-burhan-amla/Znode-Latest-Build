using System.ComponentModel.DataAnnotations;

using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.Helpers
{
    #region View Mode Types

    public enum ViewModeTypes
    {
        Detail = 1,
        Tile = 2,
        List = 3,
    }

    #endregion View Mode Types

    #region Znode Entities

    public enum ZnodeEntities
    {
        ZnodeMediaAttributeFamily,
        ZnodeMediaAttributeGroup,
        ZnodeMediaAttribute,
        ZnodeMedia,
        ZnodePIMAttributeFamily,
        ZnodePimCategory,
        ZnodePimProduct
    }

    #endregion Znode Entities

    public enum GridListType
    {
        ZnodeMediaAttributeFamily,
        ZnodeMediaAttributeGroup,
        View_GetMediaPathDetail,
        RoleList,
        ZnodeAccount,
        ZnodeMediaUser,
        ZnodeTaxRuleType,
        ZnodeCustomerAccount,
        ZnodeLocale,
        ZnodeCurrency,
        ZnodeCountry,
        ZnodeProviderEngine,
        ZnodeSupplierType,
        ZnodeShippingType,
        ZnodePromotionType,
        ZnodeTaxClass,
        ZnodePayment,
        ZnodePortalProfile,
        ZnodeDomain,
        ZnodeAdminAPIDomain,
        ZnodeStore,
        ZnodeShipping,
        ZnodeProfile,
        ZnodeSupplier,
        vw_ZNodeApplicationSetting,
        ZnodeEmailTemplate,
        ZnodeMenu,
        ZnodePimAttributeFamily,
        ZnodePimAttributeGroup,
        ZnodePimAssignedAttributes,
        View_ManageProductList,
        View_ManageProductTypeAssociationList,
        AssociatedAttributes,
        ZnodePimCustomField,
        ZnodeCategoryAttributeFamily,
        ZnodeCategoryAttributeGroup,
        ZnodeCategoryAssignedAttributes,
        ZnodePimCatalog,
        ZnodePimCategory,
        View_PimCategoryDetail,
        ZnodeCategoryAttribute,
        ZnodePimAttribute,
        View_PimProductImage,
        View_ManageProductAddonList,
        ZnodePriceList,
        ZnodePrice,
        ZnodeInventory,
        ZnodePimInventory,
        ZnodeInventoryList,
        ZnodePriceTier,
        ZnodePriceListPortal,
        PriceListAccount,
        ZnodePortal,
        UnAssociatedCustomers,
        ZnodePriceListProfile,
        UnAssociatedProfiles,
        ZnodeWarehouse,
        BasePriceManagement,
        ProfileBasePriceManagement,
        UnAssociatedCategoryProducts,
        AssociatedCategoryProducts,
        View_ImportPriceList,
        ZnodeWarehouseInventory,
        UnassociatedInventory,
        ZnodeInventoryWarehouse,
        UnassociatedWarehouse,
        UnAssociatedPriceList,
        View_ManageProductTypeList,
        View_ManageProductTypeList_GroupProduct,
        UnassociatedProducts,
        UnassociatedProductsDynamic,
        UnassociatedAddonList,
        View_ManageLinkProductList,
        ZnodePreviewImportPrice,
        ZnodePreviewImportInventory,
        ZnodeShippingSKU,
        ZnodeDepartment,
        ZnodeNote,
        ZnodeUser,
        ZnodePortalCatalog,
        ZnodeImportInventory,
        View_GetAccountListWithAddress,
        ZnodeSubAccount,
        ZnodeTaxClassSKU,
        ZnodeAccountPermission,
        ZnodeCMSTheme,
        ZnodeTaxRule,
        AccountPriceList,
        UnassociatedAccounts,
        AssociatedStoreListForCMSTheme,
        ZnodeCMSSlider,
        CustomerReviewList,
        ZnodeCMSContentPages,
        View_GetCMSSliderBannerPath,
        View_GetCMSWidgetTitleConfiguration,
        ZnodeCMSWidgetTitleConfiguration,
        View_GetPimAddonGroups,
        ZnodeShippingRule,
        ZnodeAccountsCustomer,
        StaticPageList,
        View_GetManageMessageList,
        AccountAddressList,
        ManageMessageListForWebSiteConfiguration,
        ZnodeCMSOfferPageCategory,
        ZnodeTaxClassPortal,
        AssociateCategoryToSpecialOffer,
        ZnodeShippingPortal,
        AssociatedCMSOfferPageProduct,
        UnAssociatedCMSOfferPageProduct,
        ZnodeCMSUrlRedirect,
        UnassociateAddonGroupList,
        SEOProductsDetails,
        SEOCategoryDetails,
        SEOContentPages,
        ZnodePromotion,
        ZNodePortalAddress,
        ZnodeCatalogAssociateCategory,
        ZnodeMediaAttribute,
        ZnodeGiftCard,
        ZnodeCMSThemeCSS,
        ZnodeCMSTemplate,
        WebSiteConfigurationStoreList,
        ZnodeCMSWidgetProduct,
        ZnodeCMSWidgetCategory,
        ZnodeShippingCountry,
        ZnodePortalLocale,
        ZnodeERPConfigurator,
        ZnodeOrder,
        ZnodeERPTaskScheduler,
        View_ZnodeTouchPointConfiguration,
        ZnodeOrderCustomer,
        ZnodeUserProfile,
        ZnodeUserAddress,
        CustomerNotes,
        ZnodeUserUnAssociatedProfiles,
        ZnodeTouchPointConfiguration,
        ZnodeOrderProductList,
        View_ZnodeTouchPointPopUp,
        ZnodeGetCatalogAssociatedProduct,
        UnAssociatedCategoriesToCatalog,
        UnAssociatedProductsToCatalog,
        ZnodePortalCountry,
        ZnodeCaseRequest,
        ZnodeServiceRequestNote,
        UnAssociatedPortalCountry,
        ZnodeRmaReasonForReturn,
        ZnodeRmaRequestStatus,
        ZnodeRmaRequestList,
        ZnodeHighlightProduct,
        ZnodeHighlight,
        AssociatedPriceListToAccount,
        SKUList,
        ZnodeSearchGlobalProductBoost,
        ZnodeSearchGlobalProductCategoryBoost,
        ZnodeSearchDocumentMapping,
        PublishedProductList,
        TouchPointSchedulerHistory,
        HighlightUnAssociatedProduct,
        View_ZnodeTouchPointSchedulerHistory,
        AssociatedPriceListToCustomer,
        AssociatedProductList,
        ZnodeBrandDetails,
        ZnodePimVendor,
        AssociatedBrandProductList,
        AssociatedShippingProductList,
        AssociatedVendorProductList,
        ZnodeImportLog,
        ZnodeImportLogDetails,
        AssociatedTaxClassProductList,
        PublishedCategoryList,
        PublishedCatelogList,
        AssociatedCatelogList,
        AssociatedCategoryList,
        ZnodeOmsQuote,
        ZnodeAccountUsersAddress,
        ZnodeSearchIndexMonitor,
        AssociatedProductsList,
        AccountUserOrderList,
        ZnodeImportProcessLog,
        ZnodeSearchIndexServerStatus,
        ZnodeImportProcessLogStatus,
        ZnodeOmsReferralCommission,
        CustomerOrderHistory,
        View_CustomerReferralCommissionDetail,
        View_GetProfileCatalog,
        UnAssociatedProfileCatalogList,
        ZnodeOmsOrderLineItems,
        ZnodeGetProfileCatalogAssociatedProduct,
        ZnodeCustomerPaymentList,
        ZnodeProfileAssociatedCatalogList,
        View_AccountProfileList,
        UnAssociatedProfilesForAccount,
        ZnodeCaseRequestHistory,
        ZnodeRmaRequest,
        AccountUserAddress,
        ZnodePromotionBrandDetails,
        ZnodePromotionAssociatedBrandDetails,
        ZnodeDynamicReport,
        ZnodePublishCatalogLog,
        ZnodePortalTaxClassList,
        ZnodePortalTaxClassAssociatedList,
        AssociatedPaymentListToPortal,
        UnassociatedPaymentListToPortal,
        ZnodeAssociatedShippingListToProfile,
        ZnodeUnAssociatedShippingList,
        ZnodeAssociatedShippingListToPortal,
        AssociatedPaymentListToProfile,
        View_ManageProductTypeListForToBeAssociated,
        ZnodeStoreCatalog,
        ZnodeGiftCardCustomer,
        View_UnassignZnodeTouchPointConfiguration,
        ZnodeUserAccountList,
        ZnodeUserPortalList,
        ZnodeThemeRevision,
        ZnodeCMSWidgetBrand,
        AssociateBrandToSpecialOffer,
        ZnodePublishPortalLog,
        ZnodeOmsHistory,
        ZnodePimAddonGroupProduct,
        ZnodeOrderReturnLineItemList,
        ZnodeProductFeed,
        ZnodePromotionAssociatedShippingDetails,
        ZnodePromotionShippingDetails,
        ZnodeCMSContentPageList,
        ZnodeBlogNewsList,
        ZnodeBlogNewsCommentList,
        ZnodeSearchSynonymsList,
        ZnodeSearchKeywordsRedirectList,
        ZnodeGlobalAttribute,
        ZnodeGlobalAttributeGroupList,
        ZnodeGlobalAssignedAttributes,
        ZnodePimDownloadableProductKey,
        ZnodeDomainList,
        ZnodeSearchProfile,
        ZnodeSearchProfileAttribute,
        SKUListForPrice,
        ZnodeSearchAttributes,
        ZnodeUnAssociatedSearchAttributes,
        ZnodeSearchProfileTriggers,
        ZnodeSearchPortalProfile,
        ZnodeSearchProfilePortal,
        ZnodeFormBuilderList,
        ZnodeFormSubmissionList,
        ZnodePimDownloadableProductKeyForInventory,
        ZnodeFormWidgetEmailTemplate,
        ZnodeStorePortal,
        AssociateBrandPortal,
        ZnodeLogMessage,
        ZnodeIntegrationLogMessage,
        ZnodeEventLogMessage,
        ZnodeDatabaseLogMessage,
        ProductInventoryWarehouseAssociation,
        ProductCatagoryAssociation,
        AssociatedCategoriesToProduct,
        UnAssociatedCategoriesToProduct,
        ZnodeOmsPendingPayment,
        ZnodeSearchCatalogRule,
        ZnodePublishHistory,
        ZnodePublishStateApplicationTypeMapping,
        ZnodeProfileDefaultCatalogList,
        AssociatedSortListToPortal,
        UnassociatedSortListToPortal,
        AssociatedPageListToPortal,
        UnassociatedPageListToPortal,
        ZnodeStoreExperience,
        ZnodeGuestAccount,
        ZnodeImersonationActivityLog,
        ZnodeSalesReps,
        ZnodeAssociatedSalesRep,
        ZnodePortalBrandAssociatedList,
        ZnodePortalBrandList,
        SearchNoResultsFoundReport,
        SearchTopKeywordsReport,
        ZnodeAccountUnAssociatedCustomer,
        ZnodeCloudflareDomainList,
        ZnodeCmsPageSearchIndexServerStatus,
        ZnodeCmsPageSearchIndexMonitor,
        ZnodeOmsRequestQuote,
        ZnodeReturn,
        ZnodeUserVoucherList,
        ZnodeVoucherHistory,
        ZnodeGlobalAttributeFamilyList,
        ZnodeParentAccountList,
        ZnodeCMSContentContainer,
        ZnodeCMSWidgetTemplate,
        ZnodeGuestUserAddress,
        ZnodeGuestUserProfile,
        View_ManageProductTypeList_BundleProduct,
        ZnodeOmsFailedOrderPayments,
        ZnodeCMSAssociatedVariant,
        ZnodeCMSContainerWidget,
        ZnodeExportProcessLog,
        ZnodeImportTemplate
    }

    #region Export File Types

    /// <summary>
    /// The default file types for export.
    /// </summary>
    public enum FileTypes
    {
        [Display(Name = "Microsoft Excel (.xls)")]
        Excel = 1,

        [Display(Name = "Delimited File Format")]
        CSV = 2
    }

    #endregion Export File Types

    public enum UnAssociatedProductListType
    {
        Addon,
        Link,
        AssociatedProducts
    }

    public enum AddonType
    {
        [Display(Name = ZnodeAdmin_Resources.CheckBox, ResourceType = typeof(Admin_Resources))]
        CheckBox = 2,

        [Display(Name = ZnodeAdmin_Resources.DropDown, ResourceType = typeof(Admin_Resources))]
        DropDown = 4,

        [Display(Name = ZnodeAdmin_Resources.RadioButton, ResourceType = typeof(Admin_Resources))]
        RadioButton = 1,
    }

    public enum RequiredType
    {
        [Display(Name = ZnodeAdmin_Resources.Required, ResourceType = typeof(Admin_Resources))]
        Required,

        [Display(Name = ZnodeAdmin_Resources.Optional, ResourceType = typeof(Admin_Resources))]
        Optional,

        [Display(Name = ZnodeAdmin_Resources.Auto, ResourceType = typeof(Admin_Resources))]
        Auto
    }

    public enum SEOType
    {
        Product = 1,
        Category = 2,
        ContentPage = 3
    }

    public enum AssetType
    {
        Layout,
        Header,
        Footer,
        ColorSkins,
        Home,
        Category,
        PDP
    }

    // Enum to select blog/news type.
    public enum BlogNewsType
    {
        [Display(Name = ZnodeAdmin_Resources.Blog, ResourceType = typeof(Admin_Resources))]
        Blog,

        [Display(Name = ZnodeAdmin_Resources.News, ResourceType = typeof(Admin_Resources))]
        News,
    }

    #region Product Feed Enum

    // Enum for Frequently change in Product Feed
    public enum Frequency
    {
        Daily = 0,
        Always = 1,
        Hourly = 2,
        Weekly = 3,
        Monthly = 4,
        Yearly = 5,
        Never = 6
    }

    // Enum for Priority in Product Feed
    public enum Priority
    {
        First = 100,
        Second = 101,
        Third = 102,
        Fourth = 103,
        Fifth = 104,
        Sixth = 105,
        Seventh = 106,
        Eighth = 107,
        Ninth = 108,
        Tenth = 109,
        Eleventh = 200
    }

    // Enum for XML Site Map in Product Feed
    public enum XMLSiteMap
    {
        [Display(Name = "Xml Site Map")]
        XmlSiteMap = 1,

        [Display(Name = "Google Product Feed")]
        Google = 2,

        [Display(Name = "Bing Product Feed")]
        Bing = 3,
        [Display(Name = "Xml Product Feed")]
        Xml = 4
    }

    // Enum for Last Modification Date in Product Feed
    public enum LastModification
    {
        [Display(Name = "None")]
        None = 1,

        [Display(Name = "Use the database update date")]
        DatabaseDate = 2,

        [Display(Name = "Use date / time of this update")]
        CustomDate = 3
    }

    // Enum for XML Site Map Type in Product Feed
    public enum XMLSiteMapType
    {
        [Display(Name = "Category")]
        Category = 1,

        [Display(Name = "Content Pages")]
        ContentPages = 2
    }

    #region Store Unit Enum

    //Enum for dimension unit.
    public enum DimensionUnit
    {
        CM,
        IN
    }

    #endregion Store Unit Enum

    #endregion Product Feed Enum

    public enum CurrentEnvironmentEnum
    {
        Prod,
        Qa,
        Uat
    }

    #region Code Field Enum

    public enum CodeFieldService
    {
        PortalService,
        CatalogService,
        StoreLocatorService,
        IsCodeExists,
        AccountService,
        SearchService
    }
    #endregion

    #region Date Time Range Enum
    public enum DateTimeRange
    {
        [Display(Name = ZnodeAdmin_Resources.AllLogs, ResourceType = typeof(Admin_Resources))]
        All_Logs = -1,
        [Display(Name = ZnodeAdmin_Resources.LastHour, ResourceType = typeof(Admin_Resources))]
        Last_Hour = 1,
        [Display(Name = ZnodeAdmin_Resources.LastDay, ResourceType = typeof(Admin_Resources))]
        Last_Day = 24,
        [Display(Name = ZnodeAdmin_Resources.LastSevenDays, ResourceType = typeof(Admin_Resources))]
        Last_7_Days = 168,
        [Display(Name = ZnodeAdmin_Resources.LastThirtyDays, ResourceType = typeof(Admin_Resources))]
        Last_30_Days = 720,
        [Display(Name = ZnodeAdmin_Resources.CustomRange, ResourceType = typeof(Admin_Resources))]
        CustomRange = 0,
        [Display(Name = ZnodeAdmin_Resources.AllOrders, ResourceType = typeof(Admin_Resources))]
        All_Orders= -11,
        [Display(Name = ZnodeAdmin_Resources.AllReturns, ResourceType = typeof(Admin_Resources))]
        All_Returns= -12,
        [Display(Name = ZnodeAdmin_Resources.AllFailedOrderTransactions, ResourceType = typeof(Admin_Resources))]
        All_Transactions = -13
    }
    #endregion

    #region Robot Tag Options Enum
    public enum RobotTag
    {
        [Display(Name = ZnodeAdmin_Resources.LabelRobotTagNone, ResourceType = typeof(Admin_Resources))]
        None,
        [Display(Name = ZnodeAdmin_Resources.LabelRobotTagINDEXFOLLOW, ResourceType = typeof(Admin_Resources))]
        INDEX_FOLLOW,
        [Display(Name = ZnodeAdmin_Resources.LabelRobotTagNOINDEXNOFOLLOW, ResourceType = typeof(Admin_Resources))]
        NOINDEX_NOFOLLOW,
        [Display(Name = ZnodeAdmin_Resources.LabelRobotTagNOINDEXFOLLOW, ResourceType = typeof(Admin_Resources))]
        NOINDEX_FOLLOW,
        [Display(Name = ZnodeAdmin_Resources.LabelRobotTagINDEXNOFOLLOW, ResourceType = typeof(Admin_Resources))]
        INDEX_NOFOLLOW
    }
    #endregion

    #region EntityType

    public enum EntityType
    {
        Store = 1,
        User = 2,
        Account = 3 ,
        Container = 4
    }
    #endregion
}