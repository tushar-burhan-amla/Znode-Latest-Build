using System.Collections.Generic;

namespace Znode.Engine.Api.Models
{
    public class WebStorePortalModel : PortalModel
    {
        public int PortalThemeId { get; set; }
        public int CSSId { get; set; }
        public int? Duration { get; set; }
        public string CSSName { get; set; }
        public string WebsiteLogo { get; set; }
        public string WebsiteTitle { get; set; }
        public string WebsiteDescription { get; set; }
        public string FaviconImage { get; set; }
        public string LogoPath { get; set; }
        public List<LocaleModel> PortalLocales { get; set; }
        public string CurrencySymbol { get; set; }
        public string CurrencySuffix { get; set; }
        public string CurrencyName { get; set; }
        public string CurrencyCode { get; set; }
        public string InStockMessage { get; set; }
        public string OutOfStockMessage { get; set; }
        public string BackOrderMessage { get; set; }
        public bool EnableCompare { get; set; }
        public bool EnableAddressValidation { get; set; }
        public bool IsFullPageCacheActive { get; set; }
        public int VersionId { get; set; }
        public int WebstoreVersionId { get; set; }
        public bool EnableApprovalManagement { get; set; }
        public GlobalAttributeEntityDetailsModel GlobalAttributes { get; set; }
        public string CultureCode { get; set; }
        public List<PortalSortSettingModel> SortList;
        public List<PortalPageSettingModel> PageList;
        public string DefaultRobotTag { get; set; }
        public string DynamicStyle { get; set; }
        public RecommendationSettingModel RecommendationSetting { get; set; }
        public string ImageLargeUrl { get; set; }
        public string ImageMediumUrl { get; set; }
        public string ImageSmallUrl { get; set; }
        public string ImageCrossSellUrl { get; set; }
        public string ImageThumbnailUrl { get; set; }
        public string ImageSmallThumbnailUrl { get; set; }
        public string TrackingPixelScriptCode { get; set; }
        public int? PortalProfileCatalogId { get; set; }
        //below property is representing the 'Saveforlater' store setting.
        public bool EnableSaveForLater { get; set; }
        public bool IsSmsProviderEnabled { get; set; }
        public bool IsProductInheritanceEnabled { get; set; }
        public bool IsAddToCartOptionForProductSlidersEnabled { get; set; }
    }
}
