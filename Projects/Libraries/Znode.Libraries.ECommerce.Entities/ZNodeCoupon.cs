namespace Znode.Libraries.ECommerce.Entities
{
    /// <summary>
    /// Represents Coupon Information applied on shopping cart.
    /// </summary>
    public class ZnodeCoupon
    {
        /// <summary>
        /// Gets or sets the Znode coupon applied by the user.
        /// </summary>
        public string Coupon { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether the coupon applied
        /// </summary>
        public bool CouponApplied { get; set; } = false;

        /// <summary>
        /// Gets or sets a value indicating whether the coupon valid.
        /// </summary>
        public bool CouponValid { get; set; }

        /// <summary>
        /// Gets or sets the Coupon Message
        /// </summary>
        public string CouponMessage { get; set; }

        /// <summary>
        /// Gets or sets the Coupon Display Order
        /// </summary>
        public int DisplayOrder { get; set; }

        /// <summary>
        /// Gets or sets the Allows Multiple Coupon
        /// </summary>
        public bool AllowsMultipleCoupon { get; set; }

        /// <summary>
        /// Gets or sets the Coupon Is Exist In Order
        /// </summary>
        public bool IsExistInOrder { get; set; }

        /// <summary>
        /// Gets or sets the promotion type of Coupon
        /// </summary>
        public string CouponPromotionType { get; set; }
    }
}
