namespace Znode.Libraries.ECommerce.ShoppingCart
{
    /// <summary>
    /// Represents a product
    /// </summary>    

    public interface IZnodeProductBase 
    {
        #region Static Create      

        /// <summary>
        /// Apply Promotion Product if any.
        /// </summary>
        void ApplyPromotion();
        #endregion
    }
}