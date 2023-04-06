namespace Znode.Engine.Promotions
{
	/// <summary>
	/// This is the interface for all product promotion types.
	/// </summary>
	public interface IZnodeProductPromotionType : IZnodePromotionsType
	{
        /// <summary>
		/// Binds the promotion data to the promotion.
		/// </summary>
		/// <param name="promotionBag">The promotions properties.</param>
		void Bind(ZnodePromotionBag promotionBag);
	}
}
