using System.Collections.Generic;
using Znode.Engine.Api.Models;
using Znode.Libraries.Admin;
using Znode.Libraries.ECommerce.ShoppingCart;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IShoppingCartService
    {
        /// <summary>
        /// Get cart details by cookie.
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
        /// <param name="portalId">portal id</param>
        /// <param name="shoppingCart">ShoppingCartModel</param>
        /// <returns>ShoppingCart Model</returns>
        ShoppingCartModel CreateCart(ShoppingCartModel shoppingCart);

        /// <summary>
        /// Creates a new shopping cart and saves it to the database.
        /// </summary>
        /// <param name="shoppingCart">AddToCartModel</param>
        /// <returns>AddToCart Model</returns>
        AddToCartModel AddToCartProduct(AddToCartModel shoppingCart);

        /// <summary>
        /// Performs calculations for a shopping cart.
        /// </summary>
        /// <param name="shoppingCartModel">ShoppingCartModel</param>
        /// <returns>ShoppingCart Model</returns>
        ShoppingCartModel Calculate(ShoppingCartModel shoppingCartModel);

        /// <summary>
        /// Remove all saved cart items.
        /// </summary>
        /// <param name="userId">user id</param>
        /// <param name="cookieMappingId">cookie mapping id</param>
        /// <returns>return true if all saved cart item delete.</returns>
        bool RemoveSavedCartItems(int? userId, int? cookieMappingId, int? portalId);

        /// <summary>
        /// Get OmsLineItem Detail by omsOrderId
        /// </summary>
        /// <param name="omsOrderId">omsOrderId</param>
        /// <returns>List of OmsLineItems</returns>
        OrderLineItemDataListModel GetOrderLineItemDetails(int omsOrderId);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="zipCode"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        ShippingListModel GetShippingEstimates(string zipCode, ShoppingCartModel model);

        /// <summary>
        ///  //To check product inventory 
        /// </summary>
        /// <param name="shoppingCartItem">shoppingCartItem</param>
        /// <param name="portalId">portalId</param>
        /// <param name="cartParameterModel">cartParameterModel</param>
        void CheckCartlineItemInventory(ShoppingCartItemModel shoppingCartItem, int portalId, CartParameterModel cartParameterModel);

        /// <summary>
        /// Get shopping cart details.
        /// </summary>
        /// <param name="cartParameterModel">CartParameterModel</param>
        /// <param name="cartModel">ShoppingCartModel</param>
        /// <returns>ShoppingCartModel</returns>
        ShoppingCartModel GetShoppingCartDetails(CartParameterModel cartParameterModel, ShoppingCartModel cartModel = null);


        /// <summary>
        /// Bind child line items
        /// </summary>
        /// <param name="shoppingCartItems">shoppingCartItems</param>
        /// <param name="orderHelper">orderHelper</param>
        /// <param name="parentItem">parentItem</param>
        /// <param name="item">item</param>
        void BindChildLineItem(List<ShoppingCartItemModel> shoppingCartItems, IZnodeOrderHelper orderHelper, ShoppingCartItemModel parentItem, ShoppingCartItemModel item);

        /// <summary>
        /// Remove Saved Cart Line Item by omsSavedCartLineItemId
        /// </summary>
        /// <param name="omsSavedCartLineItemId"></param>
        /// <returns>return true if saved cart item deleted.</returns>
        bool RemoveSavedCartLineItem(int omsSavedCartLineItemId);

        /// <summary>
        /// Merge cart after login
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        bool MergeGuestUsersCart(FilterCollection filters);

        /// <summary>
        /// Get cart by Order Id
        /// </summary>
        /// <param name="cartParameterModel"></param>
        /// <returns></returns>
        ShoppingCartModel GetCartByOrderId(CartParameterModel cartParameterModel);

        /// <summary>
        /// Sets shipping state code
        /// </summary>
        /// <param name="shoppingCartModel">ShoppingCartModel</param>
        /// <returns></returns>
        void SetShippingStateCode(ShoppingCartModel shoppingCartModel);

        /// <summary>
        /// Get cart and map to checkout
        /// </summary>
        /// <param name="UserAddressModel">user</param>
        /// <param name="ShoppingCartModel">shoppingCart</param>   
        /// <returns></returns>
        IZnodeCheckout GetCartAndMapToCheckout(UserAddressModel user, ShoppingCartModel shoppingCart);

        /// <summary>
        /// Check bag lineItem inventory.
        /// </summary>
        /// <param name="shoppingCartModel"></param>
        /// <param name="cartParameterModel"></param>
        void CheckBaglineItemInventory(ShoppingCartModel shoppingCartModel, CartParameterModel cartParameterModel);

        /// <summary>
        /// Get Publish Product
        /// </summary>
        /// <param name="shoppingCart"></param>
        /// <returns></returns>
        PublishProductModel GetPublishProduct(AddToCartModel shoppingCart);

       
        /// <summary>
        /// if any item from cart is out of stock so it return total no of item which is "out stock item".
        /// </summary>
        /// <param name="shoppingCartItem"></param>
        /// <param name="skus"></param>
        /// <param name="selectedQuantity"></param>
        /// <param name="inventoryList"></param>
        /// <param name="insufficientQuantity"></param>
        /// <param name="products"></param>
        /// <returns></returns>
        int IsItemOutOfStock(ShoppingCartItemModel shoppingCartItem, List<string> skus, decimal selectedQuantity, List<InventorySKUModel> inventoryList, int insufficientQuantity, PublishedProductEntityModel products);

        /// <summary>
        /// GetShoppingCartItems
        /// </summary>
        /// <param name="cartModel"></param>
        /// <returns></returns>
        List<ShoppingCartItemModel> GetShoppingCartItems(ShoppingCartModel cartModel);

        /// <summary>
        /// Validate all the SKUs 
        /// </summary>
        /// <param name="shoppingCart"></param>
        void ValidateSKUDetails(AddToCartModel shoppingCart);

        /// <summary>
        /// Get the list of SKUs
        /// </summary>
        /// <param name="shoppingCart"></param>
        /// <returns></returns>
        List<string> GetAssociatedSKUs(AddToCartModel shoppingCart);

        /// <summary>
        /// To get ShoppingCart by quoteId
        /// </summary>
        /// <param name="cartParameterModel"></param>
        /// <returns></returns>
        ShoppingCartModel LoadCartForQuote(CartParameterModel cartParameterModel);
    }
}
