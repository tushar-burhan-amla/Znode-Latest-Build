using System.Collections.Generic;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Entities;

namespace Znode.Engine.Promotions
{
    /// <summary>
    /// This is the interface for all shopping cart promotion types.
    /// </summary>
    public interface IZnodeCartPromotionType : IZnodePromotionsType
    {
        /// <summary>
        /// Binds the shopping cart and promotion data to the promotion.
        /// </summary>
        /// <param name="shoppingCart">The current shopping cart.</param>
        /// <param name="promotionBag">The promotion properties.</param>
        /// <param name="couponIndex">Index of the coupon.</param>
        void Bind(ZnodeShoppingCart shoppingCart, ZnodePromotionBag promotionBag);

        /// <summary>
        /// Calculates cart promotions.
        /// </summary>
        /// <param name="couponIndex">Index of the coupon applied.</param>
        void Calculate(int? couponIndex, List<PromotionModel> allPromotions);

        /// <summary>
        /// Process anything that must be done before the order is submitted.
        /// </summary>
        /// <returns>True if everything is good for submitting the order; otherwise, false.</returns>
        bool PreSubmitOrderProcess();

        /// <summary>
        /// Process anything that must be done after the order is submitted.
        /// </summary>
        void PostSubmitOrderProcess();

        /// <summary>
        /// Checks if a coupon is available for a promotion.
        /// </summary>
        bool IsPromoCouponAvailable { get; set; }
    }
}
