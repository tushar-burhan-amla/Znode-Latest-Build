using Znode.Libraries.ECommerce.Entities;

namespace Znode.Engine.Promotions
{
    public interface IZnodeOrderDiscountHelper
    {
        /// <summary>
        /// Calculates all Coupons and Promotion discount on for particular order data.
        /// </summary>
        void OrderCalculate();


        /// <summary>
        /// This function is used to check the Applied promotion/coupon is valid or not.
        /// </summary>
        /// <param name="cartItem">ZnodeShoppingCartItem</param>
        /// <param name="discountCode">PromotionCode/CouponCode</param>
        /// <returns></returns>
        bool IsApplicablePromotion(ZnodeShoppingCartItem cartItem, string discountCode);

        /// <summary>
        /// This function is used to check the condition that sub total of order is greater than the sum of the orderdiscount, line item discount and CSR discount
        /// </summary>
        /// <returns>Returns true if condition is correct</returns>
        bool IsDiscountApplicable();

        /// <summary>
        /// This function is used to check the condition that shipping of order is greater than the sum of the shippingdiscount
        /// </summary>
        /// <returns>Returns true if condition is correct</returns>
        bool IsShippingDiscountApplicable();
        
    }
}
