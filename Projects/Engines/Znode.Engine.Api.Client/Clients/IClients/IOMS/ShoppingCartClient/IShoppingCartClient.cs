using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Api.Client
{
    public interface IShoppingCartClient : IBaseClient
    {
        /// <summary>
        /// Gets a shopping cart for a cookie.
        /// </summary>
        /// <param name="cartParameterModel">Parameter model to get cart details.</param>
        /// <returns>ShoppingCart Model</returns>
        ShoppingCartModel GetShoppingCart(CartParameterModel cartParameterModel);

        /// <summary>
        /// Get shopping cart count 
        /// </summary>
        /// <param name="cartParameterModel">Parameter model to get cart details.</param>
        /// <returns>count</returns>
        string GetCartCount(CartParameterModel cartParameterModel);

        /// <summary>
        /// Creates a new shopping cart and saves it to the database.
        /// </summary>
        /// <param name="shoppingCartModel">The model of the shopping cart.</param>
        /// <returns>ShoppingCart Model</returns>
        ShoppingCartModel CreateCart(ShoppingCartModel shoppingCartModel);

        /// <summary>
        /// Creates a new cart and saves it to the database.
        /// </summary>
        /// <param name="addToCartModel">The model of the shopping cart.</param>
        /// <returns>AddToCart Model</returns>
        AddToCartModel AddToCartProduct(AddToCartModel addToCartModel);

        /// <summary>
        /// Performs calculations for a shopping cart.
        /// </summary>
        /// <param name="shoppingCartModel">The model of the shopping cart.</param>
        /// <returns>ShoppingCart Model</returns>
        ShoppingCartModel Calculate(ShoppingCartModel shoppingCartModel);

        /// <summary>
        /// Remove all saved cart item by user id and cookie mapping id.
        /// </summary>
        /// <param name="userId">User id</param>
        /// <param name="cookieMappingId">Cookie mapping id</param>
        /// <returns>Return true if all saved cart item delete.</returns>
        bool RemoveAllCartItem(CartParameterModel cartParameterModel);

        /// <summary>
        /// Get the Shippung Estimates
        /// </summary>
        /// <param name="shoppingCartModel"></param>
        /// <returns></returns>
        ShippingListModel GetShippingEstimates(string zipCode, ShoppingCartModel shoppingCartModel);

        /// <summary>
        /// Get OmsLineItem Detail by omsOrderId
        /// </summary>
        /// <param name="omsOrderId">omsOrderId</param>
        /// <returns>List of OmsLineItems</returns>
        OrderLineItemDataListModel GetOmsLineItemDetails(int omsOrderId);

        /// <summary>
        /// Remove Saved Cart Line Item by omsSavedCartLineItemId
        /// </summary>
        /// <param name="omsSavedCartLineItemId"></param>
        /// <returns>return true if saved cart item deleted.</returns>
        bool RemoveCartLineItem(int omsSavedCartLineItemId);

        /// <summary>
        /// Merge cart after login
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        bool MergeGuestUsersCart(FilterCollection filters);
    }
}
