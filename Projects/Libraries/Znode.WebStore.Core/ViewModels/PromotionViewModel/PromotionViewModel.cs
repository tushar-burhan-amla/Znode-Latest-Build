using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using Znode.Libraries.Resources;

namespace Znode.Engine.WebStore.ViewModels
{
    public class PromotionViewModel : BaseViewModel
    {
        public int PromotionId { get; set; }
        [MaxLength(100, ErrorMessageResourceName = ZnodeAdmin_Resources.Errorlength, ErrorMessageResourceType = typeof(Admin_Resources))]
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
        public string AssociatedShippingIds { get; set; }
    }
}