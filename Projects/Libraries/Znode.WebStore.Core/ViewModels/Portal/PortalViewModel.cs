
using System.Collections.Generic;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.WebStore.ViewModels
{
    public class PortalViewModel : BaseViewModel
    {
        public int PortalId { get; set; }
        public int LocaleId { get; set; }
        public int ProfileId { get; set; }
        public int PublishCatalogId { get; set; }
        public int Duration { get; set; }
        public string Name { get; set; }
        public string Css { get; set; }
        public string Theme { get; set; }
        public string ParentTheme { get; set; }
        public string WebsiteLogo { get; set; }
        public string WebsiteTitle { get; set; }
        public string WebsiteDescription { get; set; }
        public string FaviconImage { get; set; }
        public string CustomerServiceEmail { get; set; }
        public string CustomerServicePhoneNumber { get; set; }
        public string SiteWideBottomJavascript { get; set; }
        public string SiteWideTopJavascript { get; set; }
        public string OrderReceiptAffiliateJavascript { get; set; }
        public string SiteWideAnalyticsJavascript { get; set; }
        public string MediaServerUrl { get; set; }
        public string ProductCompareType { get; set; } = "Global Level Compare";
        public bool IsEnableSinglePageCheckout { get; set; }
        public bool IsAllowMultipleCoupon { get; set; }
        public bool PersistentCartEnabled { get; set; }
        public string MediaServerThumbnailUrl { get; set; }
        public string CurrencyCode { get; set; }
        public string CurrencySymbol { get; set; }
        public string CurrencySuffix { get; set; }
        public string InStockMessage { get; set; }
        public string OutOfStockMessage { get; set; }
        public string BackOrderMessage { get; set; }
        public bool EnableCompare { get; set; }
        public bool EnableAddressValidation { get; set; }
        public Dictionary<string, bool> PortalFeatureValues { get; set; }
        public List<LocaleModel> Locales { get; set; }
        public bool IsEnabledTagManager { get; set; }
        public string ContainerId { get; set; }
        public string AnalyticsIdForAddToCart { get; set; }
        public string AnalyticsIdForRemoveFromCart { get; set; }
        public string AnalyticsUId { get; set; }
        public bool AnalyticsIsActive { get; set; }
        public bool IsFullPageCacheActive { get; set; }
        public int WebstoreVersionId { get; set; }
        public bool EnableApprovalManagement { get; set; }
        public List<PortalSortSettingModel> SortList;
        public List<PortalPageSettingModel> PageList;
        public ZnodePublishStatesEnum PublishState { get; set; }
        public GlobalAttributeEntityDetailsModel GlobalAttributes { get; set; }
        public string DefaultRobotTag { get; set; }
        public string DynamicStyle { get; set; }
        public RecommendationSettingModel RecommendationSetting { get; set; }

        public PortalViewModel()
        {
            Locales = new List<LocaleModel>();
        }
        public string CultureCode { get; set; }
        public UserVerificationTypeEnum UserVerificationTypeCode { get; set; }
        public string ImageLargeUrl { get; set; }
        public string ImageMediumUrl { get; set; }
        public string ImageSmallUrl { get; set; }
        public string ImageCrossSellUrl { get; set; }
        public string ImageThumbnailUrl { get; set; }
        public string ImageSmallThumbnailUrl { get; set; }
        public string TrackingPixelScriptCode { get; set; }
        public bool EnableEnhancedEcommerce { get; set; }
        public int? PortalProfileCatalogId { get; set; }
        public int DefaultOrderStateID { get; set; }
        //below property is representing the 'Saveforlater' store setting.
        public bool EnableSaveForLater { get; set; }

        //below property is representing the 'Klaviyo' store setting.
        public bool IsKlaviyoEnable { get; set; }
        public bool IsSmsProviderEnabled { get; set; }
        public bool IsProductInheritanceEnabled { get; set; }
        public bool IsAddToCartOptionForProductSlidersEnabled { get; set; }
        public string ApplicationType { get; set; }

    }
}