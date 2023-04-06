using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Znode.Libraries.Resources;

namespace Znode.Engine.Api.Models
{
    public class PromotionModel : BaseModel
    {
        public int PromotionId { get; set; }
        [Required(ErrorMessageResourceName = ZnodeApi_Resources.RequiredField, ErrorMessageResourceType = typeof(Api_Resources))]
        [MaxLength(300, ErrorMessageResourceName = ZnodeApi_Resources.Errorlength, ErrorMessageResourceType = typeof(Api_Resources))]
        public string PromoCode { get; set; }

        [Required(ErrorMessageResourceName = ZnodeApi_Resources.RequiredField, ErrorMessageResourceType = typeof(Api_Resources))]
        [MaxLength(300, ErrorMessageResourceName = ZnodeApi_Resources.Errorlength, ErrorMessageResourceType = typeof(Api_Resources))]
        public string Name { get; set; }

        public string Description { get; set; }

        [Required(ErrorMessageResourceName = ZnodeApi_Resources.RequiredField, ErrorMessageResourceType = typeof(Api_Resources))]
        public int? PromotionTypeId { get; set; }

        public string PromotionTypeName { get; set; }
        public decimal? Discount { get; set; }

        [Required(ErrorMessageResourceName = ZnodeApi_Resources.RequiredField, ErrorMessageResourceType = typeof(Api_Resources))]
        public DateTime? StartDate { get; set; }

        [Required(ErrorMessageResourceName = ZnodeApi_Resources.RequiredField, ErrorMessageResourceType = typeof(Api_Resources))]
        public DateTime? EndDate { get; set; }

        public decimal? OrderMinimum { get; set; }
        public decimal? QuantityMinimum { get; set; }
        public bool? IsCouponRequired { get; set; }

        [Required(ErrorMessageResourceName = ZnodeApi_Resources.RequiredField, ErrorMessageResourceType = typeof(Api_Resources))]
        [Range(1, int.MaxValue, ErrorMessageResourceType = typeof(Api_Resources), ErrorMessageResourceName = ZnodeApi_Resources.RangeGreaterThan0)]
        public int? DisplayOrder { get; set; }

        public bool IsUnique { get; set; }

        public int? PortalId { get; set; }
        public int? ProfileId { get; set; }
        public List<int?> ProfileIds { get; set; }
        public decimal? PromotionProductQuantity { get; set; }
        public int? ReferralPublishProductId { get; set; }
        public string ProductName { get; set; }
        public string ReferralProductName { get; set; }
        public string Code { get; set; }
        public CouponListModel CouponList { get; set; }
        public string PromotionMessage { get; set; }
        public PromotionTypeModel PromotionType { get; set; }
        public bool PortalAllowsMultipleCoupon { get; set; }
        public bool IsAllowedWithOtherCoupons { get; set; }
        public string BrandCode { get; set; }

        public string AssociatedCatelogIds { get; set; }
        public string AssociatedCategoryIds { get; set; }
        public string AssociatedProductIds { get; set; }
        public string AssociatedBrandIds { get; set; }

        public string StoreName { get; set; }
        public string AssociatedShippingIds { get; set; }
        public bool IsAllowWithOtherPromotionsAndCoupons { get; set; } 

        public string GetClassName()
        {
            return PromotionType?.ClassName;
        }


        public string ClassType { get; set; }
        public bool IsActive { get; set; }
        public string ClassName { get; set; }
        public bool IsUsedInOrder { get; set; }
    }
}
