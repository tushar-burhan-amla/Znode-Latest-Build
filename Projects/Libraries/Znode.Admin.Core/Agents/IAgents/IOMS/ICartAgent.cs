using Znode.Engine.Admin.ViewModels;
using Znode.Engine.Api.Models;

namespace Znode.Engine.Admin.Agents
{
    public interface ICartAgent
    {
        /// <summary>
        /// create new cart.
        /// </summary>
        /// <param name="cartItem">CartItemViewModel</param>
        /// <returns>CartViewModel</returns>
        CartViewModel CreateCart(CartItemViewModel cartItem);

        /// <summary>
        /// Get Cart method to check Session or Cookie to get the existing shopping cart.
        /// </summary>
        /// <param name="omsOrderId">OMS order Id</param>
        /// <param name="userId">userId</param>
        /// <returns>Cart View Model</returns>
        CartViewModel GetCart(int omsOrderId = 0, int? userId = 0);

        /// <summary>
        /// Update selected shopping cart item.
        /// </summary>
        /// <param name="createOrderViewModel">CreateOrderViewModel</param>
        /// <param name="guid">Guid for every cart item.</param>
        /// <param name="quantity">Selected quantity.</param>
        /// <param name="productId">Id of product.</param>
        /// <param name="shippingId">Id of Shipping.</param>
        /// <param name="userId">userId</param>
        /// <returns>CartViewModel</returns>
        CartViewModel UpdateCartItem(string guid, decimal quantity, int productId, int shippingId, int? userId = 0);

        /// <summary>
        /// Update the quantity for the selected shopping cart item.
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="quantity"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        CartViewModel UpdateCartItemQuantity(string guid, decimal quantity, int productId, int shippingId, int? userId = 0, bool isQuote = false);

        /// <summary>
        /// Remove all cart line Items.
        /// <param name="orderId">Order Id.</param>
        /// <param name="userId">userId</param>
        /// </summary>
        void RemoveAllCartItems(int orderId = 0, int? userId = 0);

        /// <summary>
        /// Get Cart method to check Session or Cookie to get the existing shopping cart.
        /// </summary>
        /// <returns>ShoppingCartModel</returns>
        ShoppingCartModel GetCartFromCookie();

        /// <summary>
        /// Adds the Coupon Code to the Cart.
        /// </summary>
        /// <param name="couponCode">couponCode of the Cart</param>
        /// <param name="orderId">Order Id.</param>
        /// <param name="userId">userId</param>
        /// <returns>CartViewModel</returns>
        CartViewModel ApplyCoupon(string couponCode, int orderId = 0, int? userId = 0);

        /// <summary>
        /// Removes the applied coupon from the cart.
        /// </summary>
        /// <param name="couponCode">Coupon applied to the cart.</param>
        /// <param name="orderId">Order Id.</param>
        /// <param name="userId">userId</param>
        /// <returns>Returns cart view model.</returns>
        CartViewModel RemoveCoupon(string couponCode, int orderId = 0, int? userId = 0);

        /// <summary>
        /// Apply GiftCard.
        /// </summary>
        /// <param name="giftCard">Giftcard number</param>
        /// <param name="orderId">Order Id.</param>
        /// <param name="userId">userId</param>
        /// ">Giftcard number</param>
        /// <returns>Returns the updated CartViewModel</returns> 
        CartViewModel ApplyGiftCard(string giftCard, int orderId = 0, int? userId = 0);

        /// <summary>
        /// Removes the items from the shopping cart.
        /// </summary>
        /// <param name="guid">GUID of the cart item</param>
        /// <param name="orderId">Order Id.</param>
        /// <param name="userId">userId</param>
        /// <returns>return CartViewModel if the items are removed or not.</returns>
        CartViewModel RemoveShoppingCartItem(string guid, int orderId = 0, int? userId = 0);

        /// <summary>
        /// Apply csr discount.
        /// </summary>
        /// <param name="csrDiscount">csr Discount</param>
        /// <param name="csrDesc">csrDesc</param>
        /// <param name="userId">userId</param>
        /// <returns>CartViewModel</returns>
        CartViewModel ApplyCsrDiscount(decimal csrDiscount, string csrDesc, int? userId);

        /// <summary>
        /// Add multiple product to cart
        /// </summary>
        /// <param name="cartItems">bool for multiple cart item.</param>
        /// <param name="orderId">Order Id.</param>
        /// <param name="userId">userId</param>
        /// <returns>CartViewModel</returns>
        CartViewModel AddProductToCart(bool cartItems, int orderId, int? userId = 0);

        /// <summary>
        /// Remove cart items from cart.
        /// </summary>
        /// <param name="productIds">productIds</param>
        /// <param name="userId">userId</param>
        /// <returns>CartViewModel</returns>
        CartViewModel RemoveItemFromCart(string productIds, int omsOrderId, int? userId = 0);

        /// <summary>
        /// Remove cart from session .
        /// </summary>
        /// <param name="userId">userId</param>

        void RemoveCartSession(int? userId = 0);

        /// <summary>
        /// Remove all cart of user from session.
        /// <param name="userId">userId</param>
        /// </summary>
        /// <returns>CartViewModel</returns>
        CartViewModel RemoveAllCart(int? userId = 0);

        /// <summary>
        /// Add new item to cart
        /// </summary>
        /// <param name="cartItem"></param>
        /// <returns>cartItem</returns>
        AddToCartViewModel AddToCartProduct(AddToCartViewModel cartItem, bool isQuote = false);

        /// <summary>
        /// Get Cart method to check Session or Cookie to get the existing shopping cart.
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="portalId"></param>
        /// <param name="omsOrderId"></param>
        /// <returns></returns>
        CartViewModel GetShoppingCart(int userId, int portalId, int omsOrderId = 0, bool isQuote = false);

        /// <summary>
        /// Calculate the shopping cart
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="portalId"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        CartViewModel CalculateShoppingCart(int userId, int portalId, int orderId = 0, bool isQuote = false);        

        /// <summary>
        /// Remove shopping cart item
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="orderId"></param>
        /// <param name="userId"></param>
        /// <param name="portalId"></param>
        /// <returns></returns>
        bool RemoveCartItem(string guid, int orderId = 0, int userId = 0, int portalId = 0, bool isQuote = false);

        /// <summary>
        /// Get Shopping cart from session and will Remove all shopping cart items
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="portalId"></param>
        /// <param name="orderId"></param>
        /// <returns>CartViewModel</returns>
        CartViewModel RemoveAllShoppingCartItems(int userId, int portalId, int orderId = 0, bool isQuote = false);

        /// <summary>
        ///  Remove all shopping cart items
        /// </summary>
        /// <param name="cart"></param>
        /// <param name="orderId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        bool DeleteAllCartItems(ShoppingCartModel cart, int orderId = 0, int userId = 0, bool isQuote = false);
        
        /// <summary>
        /// Get Cart count of ShoppingCart.
        /// </summary>
        /// <returns>Returns count of cart items.</returns>
        decimal GetCartCount(int portalId, int userId);

        /// <summary>
        /// Get Calculated shopping cart
        /// </summary>
        /// <param name="userId"></param>
        /// <param name="portalId"></param>
        /// <param name="orderId"></param>
        /// <returns></returns>
        CartViewModel GetCalculatedShoppingCart(int userId, int portalId, int orderId = 0);

        /// <summary>
        /// Remove key from dictionary.
        /// </summary>
        /// <param name="orderModel">order model.</param>
        /// <param name="key">key to remove</param>
        /// <param name="isFromLineItem">flag whether to remove line item. </param>
        void RemoveKeyFromDictionary(OrderModel orderModel, string key, bool isFromLineItem = false);

        /// <summary>
        /// Get shopping cart from session.
        /// </summary>
        /// <param name="orderId">order id</param>
        /// <param name="userId">user id</param>
        /// <returns>shopping cart model.</returns>
        ShoppingCartModel GetShoppingCartFromSession(int orderId, int? userId = 0);

        /// <summary>
        /// get order line history.
        /// </summary>
        /// <param name="orderModel">order model.</param>
        /// <param name="key">key</param>
        /// <param name="lineHistory">line history</param>
        void OrderLineHistory(OrderModel orderModel, string key, OrderLineItemHistoryModel lineHistory);

        /// <summary>
        /// Get order model from session.
        /// </summary>
        /// <param name="orderId">oms order id.</param>
        /// <returns>order model</returns>
        OrderModel GetOrderModelFromSession(int orderId);

        /// <summary>
        /// Save model model in session.
        /// </summary>
        /// <param name="orderId">order id</param>
        /// <param name="orderModel">order model.</param>
        void SaveOrderModelInSession(int orderId, OrderModel orderModel);

        /// <summary>
        /// Order history
        /// </summary>
        /// <param name="orderModel">order model.</param>
        /// <param name="settingType">setting type.</param>
        /// <param name="oldValue">old value.</param>
        /// <param name="newValue">new value.</param>
        void OrderHistory(OrderModel orderModel, string settingType, string oldValue, string newValue = "");
      
        /// <summary>
        /// Update the quantity for the selected shopping cart item.
        /// </summary>
        /// <param name="guid"></param>
        /// <param name="quantity"></param>
        /// <param name="productId"></param>
        /// <returns></returns>
        CartViewModel UpdateCartItemPrice(string guid, decimal quantity, int productId, int shippingId, int? userId = 0);

        /// <summary>
        /// Apply voucher
        /// </summary>
        /// <param name="voucherNumber"> voucher Code</param>
        /// <param name="orderId">Order Id</param>
        /// <param name="userId">User ID</param>
        /// <returns>CartViewModel</returns>
        CartViewModel ApplyVoucher(string voucherNumber, int orderId = 0, int? userId = 0);

        /// <summary>
        /// Remove Voucher
        /// </summary>
        /// <param name="voucherCode">Voucher NUmber</param>
        /// <param name="orderId"> Order ID</param>
        /// <param name="userId">User ID</param>
        /// <returns>CartViewModel</returns>
        CartViewModel RemoveVoucher(string voucherCode, int orderId = 0, int? userId = 0);
    }
}