using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Engine.Admin.Helpers;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Resources;
namespace Znode.Engine.Admin.ViewModels
{
    public class StoreViewModel : BaseViewModel
    {
        public int PortalId { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorStoreCodeRequired)]
        [Display(Name = ZnodeAdmin_Resources.LabelStoreCode, ResourceType = typeof(Admin_Resources))]
        [RegularExpression(@"^[a-zA-Z0-9]*$", ErrorMessageResourceName = ZnodeAdmin_Resources.AlphanumericOnlyWithNoSpaces, ErrorMessageResourceType = typeof(Admin_Resources))]
        [MaxLength(100, ErrorMessageResourceName = ZnodeAdmin_Resources.StoreCodeMaxLength, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string StoreCode { get; set; }

        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorCatalogRequired)]
        public int PublishCatalogId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorCatalogRequired)]
        public string CatalogName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorThemeRequired)]
        public int? CMSThemeId { get; set; }
        public int OrderStatusId { get; set; }

        public string DefaultCurrency { get; set; }
        public string DefaultDimensionUnit { get; set; }
        public string DefaultWeightUnit { get; set; }
        public string ReviewStatusId { get; set; }
        public string ThemeName { get; set; }
        public string UrlEncodedStoreName { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelCustomerReviewStatus, ResourceType = typeof(Admin_Resources))]
        public string ReviewStatus { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelProductReviewStatus, ResourceType = typeof(Admin_Resources))]
        public string ProductReviewStatus { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelOrderStatus, ResourceType = typeof(Admin_Resources))]
        public string OrderStatus { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.Errorlength)]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorStoreNameRequired)]
        [Display(Name = ZnodeAdmin_Resources.LabelStoreName, ResourceType = typeof(Admin_Resources))]
        public string StoreName { get; set; }

        [StringLength(100, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.Errorlength)]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorBrandNameRequired)]
        [Display(Name = ZnodeAdmin_Resources.LabelCompanyName, ResourceType = typeof(Admin_Resources))]
        public string CompanyName { get; set; }

        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorAdministratorEmailRequired)]
        [RegularExpression(AdminConstants.EmailRegularExpression, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ValidEmailAddress)]
        [Display(Name = ZnodeAdmin_Resources.LabelAdminEmail, ResourceType = typeof(Admin_Resources))]
        public string AdministratorEmail { get; set; }

        [RegularExpression(AdminConstants.EmailRegularExpression, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ValidEmailAddress)]
        [Display(Name = ZnodeAdmin_Resources.LabelSalesEmail, ResourceType = typeof(Admin_Resources))]
        public string SalesEmail { get; set; }

        public string DomainUrl { get; set; }
        public string ImageNotAvailablePath { get; set; } = "~/data/default/images/catalog/original/Turnkey/1/Lighthouse.jpg";

        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorCustomerServiceEmailRequired)]
        [RegularExpression(AdminConstants.EmailRegularExpression, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ValidEmailAddress)]
        [Display(Name = ZnodeAdmin_Resources.LabelCustomerServiceEmail, ResourceType = typeof(Admin_Resources))]
        public string CustomerServiceEmail { get; set; }

        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorSalesPhoneNumberRequired)]
        [Display(Name = ZnodeAdmin_Resources.LabelSalesPhoneNumber, ResourceType = typeof(Admin_Resources))]
        public string SalesPhoneNumber { get; set; }

        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorCustomerServicePhoneNumberRequired)]
        [Display(Name = ZnodeAdmin_Resources.LabelCustomerServicePhoneNumber, ResourceType = typeof(Admin_Resources))]
        public string CustomerServicePhoneNumber { get; set; }

        public string Theme { get; set; }
        public string CSS { get; set; }
        public string[] PortalFeatureIds { get; set; }
        public string CopyContentPortalName { get; set; }

        [Display(Name = ZnodeAdmin_Resources.InStockMessage, ResourceType = typeof(Admin_Resources))]
        public string InStockMsg { get; set; }

        [Display(Name = ZnodeAdmin_Resources.OutOfStockMessage, ResourceType = typeof(Admin_Resources))]
        public string OutOfStockMsg { get; set; }

        [Display(Name = ZnodeAdmin_Resources.BackOrderMessage, ResourceType = typeof(Admin_Resources))]
        public string BackOrderMsg { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelSecureSSL, ResourceType = typeof(Admin_Resources))]
        public bool IsEnableSSL { get; set; }

        public bool EnableOrderAlert { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelOrderAmount, ResourceType = typeof(Admin_Resources))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.PleaseEnterOrderAmount)]
        [Range(0.01, 999999.00, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorRangeBetween)]
        public decimal? OrderAmount { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelEmail, ResourceType = typeof(Admin_Resources))]
        [RegularExpression(AdminConstants.MultipleEmailRegEx, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ValidEmailAddress)]
        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ValidEmailAddress)]
        public string Email { get; set; }

        public List<SelectListItem> ThemeList { get; set; }
        public List<SelectListItem> PortalList { get; set; }
        public List<SelectListItem> CSSList { get; set; }
        public List<SelectListItem> CatalogList { get; set; }
        public List<SelectListItem> OrderStatusList { get; set; }
        public List<SelectListItem> CustomerReviewStatusList { get; set; }
        public List<StoreFeatureViewModel> AvailableStoreFeatureList { get; set; }
        public List<StoreFeatureViewModel> SelectedStoreFeatureList { get; set; }
        public int ProfileId { get; set; }
        public int? CopyContentPortalId { get; set; }

        [Required(ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorCSSRequired)]
        public int CMSThemeCSSId { get; set; }
        public bool PersistentCartEnabled { get; set; }

        public bool IsSearchIndexCreated { get; set; }

        public string CSSName { get; set; }
        public string PublishStatus { get; set; }
        public string DefaultCulture { get; set; }
        public List<SelectListItem> UserVerificationTypeCodeList
        {
            get
            {
                return UserRegistartionType.GetUserVerificationType();
            }
        }


        [Display(Name = ZnodeAdmin_Resources.UserVerificationType, ResourceType = typeof(Admin_Resources))]
        public string UserVerificationType { get; set; }
        public UserVerificationTypeEnum UserVerificationTypeCode { get; set; }
    }

    public static class UserRegistartionType
    {
        //Get the list of User Registartion Type.
        public static List<SelectListItem> GetUserVerificationType()
        => new List<SelectListItem>(){
                 new SelectListItem(){Text=Admin_Resources.NoVerificationText,Value=UserVerificationTypeEnum.NoVerificationCode.ToString()},
                 new SelectListItem(){Text=Admin_Resources.EmailVerificationText, Value=UserVerificationTypeEnum.EmailVerificationCode.ToString()} ,
                 new SelectListItem(){Text=Admin_Resources.AdminApprovalText, Value=UserVerificationTypeEnum.AdminApprovalCode.ToString()
                         } };
    }
}