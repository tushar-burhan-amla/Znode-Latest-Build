namespace Znode.Engine.WebStore.ViewModels
{
    public class CouponViewModel : BaseViewModel
    {
        public int PromotionCouponId { get; set; }

        public int PromotionId { get; set; }

        public string Code { get; set; }

        public int? AvailableQuantity { get; set; }

        public string PromotionMessage { get; set; }
        public bool IsAllowedWithOtherCoupons { get; set; }
        public bool CouponApplied { get; set; }
        public bool CouponValid { get; set; }
        public string CouponPromotionType { get; set; }
    }
}