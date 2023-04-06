namespace Znode.Engine.Api.Models
{
    public class CouponModel : BaseModel
    {
        public int PromotionCouponId { get; set; }
        public int PromotionId { get; set; }
        public string PromotionMessage { get; set; }
        public string Code { get; set; }
        public int InitialQuantity { get; set; }
        public int AvailableQuantity { get; set; }
        public bool IsEnableUrl { get; set; }
        public bool IsActive { get; set; }
        public bool CouponApplied { get; set; }
        public bool CouponValid { get; set; }
        public string CustomCouponCode { get; set; }
        public bool IsCustomCoupon { get; set; }
        public bool IsExistInOrder { get; set; }
        public string CouponPromotionType { get; set; }
    }
}
