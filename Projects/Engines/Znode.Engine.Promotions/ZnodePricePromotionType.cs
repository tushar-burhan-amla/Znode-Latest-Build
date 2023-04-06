using Znode.Libraries.ECommerce.Entities;

namespace Znode.Engine.Promotions
{
    /// <summary>
	/// Provides the base implementation for all price promotion types. Handles price changes of products before they are put into the shopping cart.
    /// </summary>
	public abstract class ZnodePricePromotionType : ZnodePromotionsType, IZnodePricePromotionType
    {
        public ZnodePromotionBag PromotionBag { get; set; }

        /// <summary>
        /// Binds the promotion data to the promotion.
        /// </summary>
        /// <param name="promotionBag">The promotions properties.</param>
        public virtual void Bind(ZnodePromotionBag promotionBag) => PromotionBag = promotionBag;

        /// <summary>
        /// Calculates the product price to display after the promotion is applied.
        /// </summary>
        /// <param name="product">The product on which the promotional price is to be used.</param>
        /// <param name="currentPrice">The current price of the product.</param>
        /// <remarks>Normally promotions will be calculated on the product's base price. The current price is included just in case you need to calculate promotions based on other promotions that have already been applied.</remarks>
        /// <returns>The promotional price.</returns>
        public virtual decimal PromotionalPrice(ZnodeProductBaseEntity product, decimal currentPrice)
            => PromotionalPrice(product.ProductID, currentPrice);

        /// <summary>
        /// Calculates the product price to display after the promotion is applied.
        /// </summary>
        /// <param name="productId">The ID of the product on which the promotional price is to be used.</param>
        /// <param name="currentPrice">The current price of the product.</param>
        /// <remarks>Normally promotions will be calculated on the product's base price. The current price is included just in case you need to calculate promotions based on other promotions that have already been applied.</remarks>
        /// <returns>The promotional price.</returns>
        public virtual decimal PromotionalPrice(int productId, decimal currentPrice) => currentPrice;
    }
}
