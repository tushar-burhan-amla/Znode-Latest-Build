using System.Collections.Generic;
using Znode.Engine.Api.Models;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Promotions
{
    /// <summary>
    /// Property bag of settings used by the promotions. 
    /// </summary>
	public class ZnodePromotionBag : ZnodeBusinessBase
    {
        public int PromotionId { get; set; }
        public int? PortalId { get; set; }
        public int? ProfileId { get; set; }
        public decimal Discount { get; set; }
        public decimal MinimumOrderAmount { get; set; }
        public int? RequiredBrandId { get; set; }
        public decimal RequiredBrandMinimumQuantity { get; set; }
        public int? RequiredCatalogId { get; set; }
        public decimal RequiredCatalogMinimumQuantity { get; set; }
        public int? RequiredCategoryId { get; set; }
        public decimal RequiredCategoryMinimumQuantity { get; set; }
        public int? RequiredProductId { get; set; }
        public decimal RequiredProductMinimumQuantity { get; set; }
        public int? DiscountedProductId { get; set; }
        public decimal DiscountedProductQuantity { get; set; }
        public bool IsCouponAllowedWithOtherCoupons { get; set; }
        public bool IsUnique { get; set; }
        public string PromoCode { get; set; }
        public string PromotionMessage { get; set; }
        public List<ProductModel> AssociatedProducts { get; set; }
        public List<CouponModel> Coupons { get; set; }
        public string PromotionName { get; set; }
        public int? PromotionTypeId { get; set; }
    }
}
