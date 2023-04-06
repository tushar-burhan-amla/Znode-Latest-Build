using Znode.Libraries.ECommerce.Entities;

namespace Znode.Engine.Promotions
{
    public class ZnodeProductPromotionProductDetails : ZnodeProductPromotionType
    {
        #region Constructor
        public ZnodeProductPromotionProductDetails()
        {
            Name = "Sample Custom Promotion Class";
            Description = "Overrides displayed product details.";
            AvailableForFranchise = false;

            Controls.Add(ZnodePromotionControl.Store);
            Controls.Add(ZnodePromotionControl.Profile);
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Allows you to change product details when displaying the product.
        /// </summary>
        /// <param name="product">The product on which you can change its details.</param>
        public override void ChangeDetails(ZnodeProductBaseEntity product)
        {
            // Check if promotion is already applied
            if (!product.IsPromotionApplied && Equals(product.ProductID, PromotionBag.RequiredProductId))
                // Add whatever logic you want here to override the product details for this promotion. For example, if you
                // want to override the product name and description for the product in a wholesale profile, do the following:
               
                // Ensure it's ignored if applied again
                product.IsPromotionApplied = true;
        } 
        #endregion
    }
}
