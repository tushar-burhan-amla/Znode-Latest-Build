using Znode.Libraries.ECommerce.Entities;

namespace Znode.Engine.Promotions
{
    /// <summary>
	/// Provides the base implementation for all product promotion types. Allows you to change product details when displaying a product.
    /// </summary>
	public abstract class ZnodeProductPromotionType : ZnodePromotionsType, IZnodeProductPromotionType
    {
        #region Public Properties
        public ZnodePromotionBag PromotionBag { get; set; }
        #endregion

        #region Public Methods
        /// <summary>
        /// Binds the promotion data to the promotion.
        /// </summary>
        /// <param name="promotionBag">The promotions properties.</param>
        public virtual void Bind(ZnodePromotionBag promotionBag) => PromotionBag = promotionBag;

        /// <summary>
        /// Use this to change product details.
        /// </summary>
        /// <param name="product">The product on which the details can be changed.</param>
        public virtual void ChangeDetails(ZnodeProductBaseEntity product)
        {
        } 
        #endregion
    }
}
