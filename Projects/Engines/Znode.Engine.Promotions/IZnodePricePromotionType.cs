using Znode.Libraries.ECommerce.Entities;

namespace Znode.Engine.Promotions
{
	/// <summary>
	/// This is the interface for all price promotion types.
	/// </summary>
	public interface IZnodePricePromotionType : IZnodePromotionsType
	{
        /// <summary>
		/// Binds the promotion data to the promotion.
		/// </summary>
		/// <param name="promotionBag">The promotions properties.</param>
		void Bind(ZnodePromotionBag promotionBag);

        /// <summary>
        /// Calculates promotional price.
        /// </summary>
        /// <param name="product">The product to discount.</param>
		/// <param name="currentPrice">The current price of the product.</param>
		/// <returns>Returns the promotional price for the product.</returns>
		decimal PromotionalPrice(ZnodeProductBaseEntity product, decimal currentPrice);

        /// <summary>
		/// Calculates the promotional price.
		/// </summary>
		/// <param name="productId">The ID of the product to discount.</param>
		/// <param name="currentPrice">The current price of the product.</param>
		/// <returns>Returns the promotional price for the product.</returns>
		decimal PromotionalPrice(int productId, decimal currentPrice);
	}
}
