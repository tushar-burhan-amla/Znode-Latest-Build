using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Libraries.Resources;

namespace Znode.Engine.Admin.ViewModels
{
    public class PromotionViewModel : BaseViewModel
    {
        public int PromotionId { get; set; }
        [MaxLength(300, ErrorMessageResourceName = ZnodeAdmin_Resources.Errorlength, ErrorMessageResourceType = typeof(Admin_Resources))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredField, ErrorMessageResourceType = typeof(Admin_Resources))]
        [Display(Name = ZnodeAdmin_Resources.LabelPromotionCode, ResourceType = typeof(Admin_Resources))]
        public string PromoCode { get; set; }

        [MaxLength(100, ErrorMessageResourceName = ZnodeAdmin_Resources.Errorlength, ErrorMessageResourceType = typeof(Admin_Resources))]
        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredField, ErrorMessageResourceType = typeof(Admin_Resources))]
        [Display(Name = ZnodeAdmin_Resources.LabelPromotionName, ResourceType = typeof(Admin_Resources))]
        public string Name { get; set; }

        [MaxLength(100, ErrorMessageResourceName = ZnodeAdmin_Resources.Errorlength, ErrorMessageResourceType = typeof(Admin_Resources))]
        public string Description { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredField, ErrorMessageResourceType = typeof(Admin_Resources))]
        [Display(Name = ZnodeAdmin_Resources.LabelDiscountType, ResourceType = typeof(Admin_Resources))]
        public int? PromotionTypeId { get; set; }

        public string PromotionTypeName { get; set; }
        
        public decimal? Discount { get; set; }

        public DateTime? TodaysDate { get; set; } = DateTime.Today;

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredField, ErrorMessageResourceType = typeof(Admin_Resources))]      
        [Display(Name = ZnodeAdmin_Resources.LabelStartDate, ResourceType = typeof(Admin_Resources))]
        public DateTime? StartDate { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredField, ErrorMessageResourceType = typeof(Admin_Resources))]     
        [Display(Name = ZnodeAdmin_Resources.LabelEndDate, ResourceType = typeof(Admin_Resources))]
        public DateTime? EndDate { get; set; }

      
        [Display(Name = ZnodeAdmin_Resources.LabelMinimumOrderAmount, ResourceType = typeof(Admin_Resources))]
        public decimal? OrderMinimum { get; set; }

        
        [Display(Name = ZnodeAdmin_Resources.LabelMinQuantityCode, ResourceType = typeof(Admin_Resources))]
        public decimal? QuantityMinimum { get; set; }

        public bool IsCouponRequired { get; set; }

        [Required(AllowEmptyStrings = false, ErrorMessageResourceName = ZnodeAdmin_Resources.RequiredField, ErrorMessageResourceType = typeof(Admin_Resources))]
        [Range(1, 999, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorRangeBetween)]
        [Display(Name = ZnodeAdmin_Resources.LabelDisplayOrder, ResourceType = typeof(Admin_Resources))]
        public int DisplayOrder { get; set; } = 99;

        [Display(Name = ZnodeAdmin_Resources.LabelDiscountType, ResourceType = typeof(Admin_Resources))]
        public List<SelectListItem> PromotionTypeList { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelStore, ResourceType = typeof(Admin_Resources))]
        public int PortalId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.TitleProfile, ResourceType = typeof(Admin_Resources))]
        public int ProfileId { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelProduct, ResourceType = typeof(Admin_Resources))]
        public int CallForPricingProductId { get; set; }
       
        [Display(Name = ZnodeAdmin_Resources.LabelReferralProductName, ResourceType = typeof(Admin_Resources))]
        public int? ReferralPublishProductId { get; set; } = 0;

        [Display(Name = ZnodeAdmin_Resources.TitleProfile, ResourceType = typeof(Admin_Resources))]
        public List<int?> ProfileIds { get; set; }

        public bool IsAllowedWithOtherCoupons { get; set; }

        public List<SelectListItem> ProfileList { get; set; }
        public List<SelectListItem> CatalogList { get; set; }
        public List<SelectListItem> CategoryList { get; set; }
        public List<SelectListItem> ProductList { get; set; }
        public CouponListViewModel CouponList { get; set; }

        public int LocaleId { get; set; }

        public bool IsUnique { get; set; }
       
        [Display(Name = ZnodeAdmin_Resources.LabelPromotionProductQuantity, ResourceType = typeof(Admin_Resources))]
        [Range(0.01, 999999.99, ErrorMessageResourceType = typeof(Admin_Resources), ErrorMessageResourceName = ZnodeAdmin_Resources.ErrorDiscountAmountRange)]
        public decimal? PromotionProductQuantity { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelProductToDiscountDiscount, ResourceType = typeof(Admin_Resources))]
        public string ProductName { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelReferralProductName, ResourceType = typeof(Admin_Resources))]
        public string ReferralProductName { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelCallForPrice, ResourceType = typeof(Admin_Resources))]
        public string CallForPriceMessage { get; set; }

        [Display(Name = ZnodeAdmin_Resources.LabelPromotionMessage, ResourceType = typeof(Admin_Resources))]
        public string PromotionMessage { get; set; }

        public bool PortalAllowsMultipleCoupon { get; set; }

        public string AssociatedCatelogIds { get; set; }
        public string AssociatedCategoryIds { get; set; }
        public string AssociatedProductIds { get; set; }
        public string AssociatedBrandIds { get; set; }
        public string StoreName { get; set; }
        public string CustomCouponCode { get; set; }
        public bool IsCustomCoupon { get; set; }
        public bool ShowAllStoreCheckbox { get; set; }
        public string AssociatedShippingIds { get; set; }
    }
}