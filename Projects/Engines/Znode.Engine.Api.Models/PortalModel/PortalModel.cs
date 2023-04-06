using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;

namespace Znode.Engine.Api.Models
{
    public class PortalModel : BaseModel
    {
        public int PortalId { get; set; }
        public string StoreCode { get; set; }
        public int? PublishCatalogId { get; set; }
        public int? LocaleId { get; set; }
        public int? CMSThemeId { get; set; }
        public int? CMSParentThemeId { get; set; }
        public int OrderStatusId { get; set; }

        public string DefaultCurrency { get; set; }
        public string DefaultDimensionUnit { get; set; }
        public string DefaultWeightUnit { get; set; }
        public int? DefaultOrderStateID { get; set; }
        public string ReviewStatus { get; set; }
        public string ThemeName { get; set; }
        public string ParentThemeName { get; set; }
        public string ProductReviewStatus { get; set; }
        public string OrderStatus { get; set; }
        public string DefaultCulture { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.Errorlength)]
        public string StoreName { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.Errorlength)]
        public string CompanyName { get; set; }
        public string AdministratorEmail { get; set; }
        public string SalesEmail { get; set; }
        public string DomainUrl { get; set; }
        public string CustomerServiceEmail { get; set; }
        public string ImageNotAvailablePath { get; set; }
        public string SalesPhoneNumber { get; set; }
        public string MediaServerUrl { get; set; }
        public string MediaServerThumbnailUrl { get; set; }
        public string CustomerServicePhoneNumber { get; set; }
        public string[] PortalFeatureIds { get; set; }
        public bool IsEnableSSL { get; set; }
        public int CMSThemeCSSId { get; set; }
        public int ProfileId { get; set; }
        public int? CopyContentPortalId { get; set; }
        public string CopyContentPortalName { get; set; }
        public string InStockMsg { get; set; }
        public string OutOfStockMsg { get; set; }
        public string BackOrderMsg { get; set; }

        public decimal? OrderAmount { get; set; }
        public string Email { get; set; }

        public List<PortalFeatureModel> SelectedPortalFeatures { get; set; }
        public List<PortalFeatureModel> AvailablePortalFeatures { get; set; }

        public Dictionary<string, bool> PortalFeatureValues { get; set; }

        public bool IsSearchIndexCreated { get; set; } = true;

        public string CatalogName { get; set; }
        public string PublishStatus { get; set; }
        public string CSSName { get; set; }

        public bool IsEnabledTagManager { get; set; }
        public string ContainerId { get; set; }
        public string AnalyticsIdForAddToCart { get; set; }
        public string AnalyticsIdForRemoveFromCart { get; set; }
        public string StoreLogo { get; set; }
        public string AnalyticsUId { get; set; }
        public bool AnalyticsIsActive { get; set; }
        public string Code { get; set; }
        public ZnodePublishStatesEnum PublishState { get; set; }
        public UserVerificationTypeEnum UserVerificationTypeCode { get; set; }
        public bool EnableEnhancedEcommerce { get; set; }

        //below property is representing the 'Klaviyo' store setting.
        public bool IsKlaviyoEnable { get; set; }
    }
}
