using Znode.Libraries.ECommerce.Entities;

namespace Znode.Engine.Promotions
{
    public class ZnodeProductPromotionCallForPricing : ZnodeProductPromotionType
    {
        #region Constructor
        public ZnodeProductPromotionCallForPricing()
        {
            Name = "Call For Pricing";
            Description = "Shows a \"call for price\" message.";
            AvailableForFranchise = false;

            Controls.Add(ZnodePromotionControl.Store);
            Controls.Add(ZnodePromotionControl.Profile);
            Controls.Add(ZnodePromotionControl.CallForPriceProduct);
            Controls.Add(ZnodePromotionControl.CallForPriceSku);
            Controls.Add(ZnodePromotionControl.CallForPriceMessage);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Shows a "call for pricing" message when displaying the product.
        /// </summary>
        /// <param name="product">The product that will show the "call for pricing" message.</param>
        public override void ChangeDetails(ZnodeProductBaseEntity product)
        {
            // Check if promotion is already applied
            if (!product.IsPromotionApplied && Equals(product.ProductID, PromotionBag.RequiredProductId))
            {
                if (PromotionBag.DiscountedProductId > 0 && Equals(PromotionBag.DiscountedProductId, product.SKU))
                {
                   
                    product.CallForPricing = true;
                    product.IsPromotionApplied = true;
                }
            }
        }
        #endregion
    }
}
