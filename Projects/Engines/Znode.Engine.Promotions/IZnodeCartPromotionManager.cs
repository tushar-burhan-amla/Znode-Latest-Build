using System.Collections.Generic;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Promotions
{
    public interface IZnodeCartPromotionManager
    {
        /// <summary>
        /// Calculates the promotion and updates the shopping cart.
        /// </summary>
        void Calculate();

        //to clear all promotions and coupons applied to shopping cart
        void ClearAllPromotionsAndCoupons();

        /// <summary>
        /// Process anything that must be done before the order is submitted.
        /// </summary>
        /// <returns>True if everything is good for submitting the order; otherwise, false.</returns>
        bool PreSubmitOrderProcess();

        /// <summary>
        /// Process anything that must be done after the order is submitted.
        /// </summary>
        void PostSubmitOrderProcess();

        List<PromotionModel> CartPromotionCache
        {
            get;
        }
        /// <summary>
        /// Set Promotional Price of each cart item from Shopping Cart.
        /// </summary>
        void SetPromotionalPrice();
    }
}
