using System.Linq;
using Znode.Libraries.ECommerce.Entities;

namespace Znode.Engine.Promotions
{
    public class ZnodePricePromotionPercentOffProduct : ZnodePricePromotionType
    {
        #region Constructor
        public ZnodePricePromotionPercentOffProduct()
        {
            Name = "Percent Off Displayed Product Price";
            Description = "Displays a discounted price for the product based on the percent off.";
            AvailableForFranchise = false;

            Controls.Add(ZnodePromotionControl.Store);
            Controls.Add(ZnodePromotionControl.Profile);
            Controls.Add(ZnodePromotionControl.DiscountPercent);
            Controls.Add(ZnodePromotionControl.RequiredProduct);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Calculates the percent off the product price based on the promotion discount.
        /// </summary>
        /// <param name="product">The product to discount.</param>
        /// <param name="currentPrice">The current price of the product.</param>
        /// <returns>Returns the promotional price for the product.</returns>
        public override decimal PromotionalPrice(ZnodeProductBaseEntity product, decimal currentPrice) => PromotionalPrice(product.ProductID, currentPrice);

        /// <summary>
        /// Calculates the percent off the product price based on the promotion discount.
        /// </summary>
        /// <param name="productId">The ID of the product to discount.</param>
        /// <param name="currentPrice">The current price of the product.</param>
        /// <returns>Returns the promotional price for the product.</returns>
        public override decimal PromotionalPrice(int productId, decimal currentPrice)
        {
            decimal discountedPrice = currentPrice;

            decimal discount = GetProductDiscountAmount(productId);

            if (discount > 0)
                discountedPrice -= currentPrice * (discount / 100);

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
