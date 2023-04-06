using System.Linq;
using Znode.Libraries.ECommerce.Entities;

namespace Znode.Engine.Promotions
{
    public class ZnodePricePromotionAmountOffProduct : ZnodePricePromotionType
    {
        #region Constructor
        public ZnodePricePromotionAmountOffProduct()
        {
            Name = "Amount Off Displayed Product Price";
            Description = "Displays a discounted price for the product based on the amount off.";
            AvailableForFranchise = false;

            Controls.Add(ZnodePromotionControl.Store);
            Controls.Add(ZnodePromotionControl.Profile);
            Controls.Add(ZnodePromotionControl.DiscountAmount);
            Controls.Add(ZnodePromotionControl.RequiredProduct);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Calculates the amount off the product price based on the promotion discount.
        /// </summary>
        /// <param name="product">The product to discount.</param>
        /// <param name="currentPrice">The current price of the product.</param>
        /// <returns>Returns the promotional price for the product.</returns>
        public override decimal PromotionalPrice(ZnodeProductBaseEntity product, decimal currentPrice)
            => PromotionalPrice(product.ProductID, currentPrice);

        /// <summary>
        /// Calculates the amount off the product price based on the promotion discount.
        /// </summary>
        /// <param name="productId">The ID of the product to discount.</param>
        /// <param name="currentPrice">The current price of the product.</param>
        /// <returns>Returns the promotional price for the product.</returns>
        public override decimal PromotionalPrice(int productId, decimal currentPrice)
        {
            decimal discountedPrice = currentPrice;

            discountedPrice -= GetProductDiscountAmount(productId);

            if (discountedPrice < 0)
                discountedPrice = 0;

            return discountedPrice;
        }
        #endregion

        #region Private Method

        //to get product discount amount
        private decimal GetProductDiscountAmount(int productId)
        {
            if (PromotionBag?.AssociatedProducts?.Count > 0)
            {
                return (from promotion in PromotionBag.AssociatedProducts
                        where promotion.ProductId == productId
                        select PromotionBag.Discount
                         ).FirstOrDefault();
            }
            return 0;
        }
        #endregion
    }
}
