using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.V2;

namespace Znode.Engine.Services
{
    public interface IShoppingCartServiceV2 : IShoppingCartService
    {
        /// <summary>
        /// Creates a new shopping cart and saves it to the database.
        /// </summary>        
        /// <param name="cartModel">ShoppingCartModel</param>
        /// <returns>ShoppingCart Model</returns>
        ShoppingCartModelV2 CreateCart(ShoppingCartModelV2 cartModel);

        /// <summary>
        /// Remove items from cart
        /// </summary>
        /// <param name="model">RemoveCartItemModelV2</param>
        /// <returns>True/False</returns>
        ShoppingCartModelV2 RemoveSavedCartItems(RemoveCartItemModelV2 model);

        /// <summary>
        /// Calculate shopping cart
        /// </summary>
        /// <param name="model">ShoppingCartCalculateRequestModelV2</param>
        /// <returns>ShoppingCartCalculateResponseModelV2</returns>
        ShoppingCartModelV2 CalculateV2(ShoppingCartCalculateRequestModelV2 model);

        /// <summary>
        /// Get shopping cart
        /// </summary>
        /// <param name="model">CartParameterModel</param>
        /// <returns>ShoppingCartCalculateResponseModelV2</returns>
        ShoppingCartModelV2 GetShoppingCartV2(CartParameterModel model);
    }
}
