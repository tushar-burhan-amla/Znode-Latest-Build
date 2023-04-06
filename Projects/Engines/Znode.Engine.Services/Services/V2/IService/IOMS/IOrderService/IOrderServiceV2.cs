using System.Collections.Specialized;
using Znode.Engine.Api.Models;
using Znode.Engine.Api.Models.V2;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Engine.Services
{
    public interface IOrderServiceV2 : IOrderService
    {
        /// <summary>
        /// Get the list of all orders.
        /// </summary>
        /// <param name="expands">Expand Collection.</param>
        /// <param name="filters">Filter collection.</param>
        /// <param name="sorts">Sort Collection.</param>
        /// <param name="Page">paging parameters.</param>
        /// <returns>OrdersList Model.</returns>
        OrdersListModelV2 GetOrderListV2(NameValueCollection expands, FilterCollection filters, NameValueCollection sorts, NameValueCollection Page);

        /// <summary>
        /// Create new order 
        /// </summary>
        /// <param name="model">Create Order Model V2</param>
        /// <returns>OrderModel</returns>
        OrderResponseModelV2 CreateOrderV2(CreateOrderModelV2 model);

        /// <summary>
        /// Get User details for shopping cart
        /// </summary>
        /// <param name="userId">userId</param>
        /// <returns>UserModel</returns>
        UserModel GetUserDetails(int userId);

        /// <summary>
        /// Get Shopping for new order
        /// </summary>
        /// <param name="model">CreateOrderModelV2</param>
        /// <returns>ShoppingCartModel</returns>
        ShoppingCartModel GetShoppingCart(CreateOrderModelV2 model);

        /// <summary>
        /// Get shipping details for order 
        /// </summary>
        /// <param name="shippingOptionId">shippingOptionId</param>
        /// <returns>OrderShippingModel</returns>
        OrderShippingModel GetShipping(int shippingOptionId);

        /// <summary>
        /// Get billing shipping address for order
        /// </summary>
        /// <param name="userId">userId</param>
        /// <param name="addressId">addressId</param>
        /// <param name="isBilling">isBilling</param>
        /// <returns></returns>
        AddressModel GetAddress(int userId, int addressId, bool isBilling);

        /// <summary>
        /// Get billing shipping address for order
        /// </summary>
        /// <param name="userId">userId</param>
        /// <param name="isBilling">isBilling</param>
        /// <returns>AddressModel</returns>
        AddressModel GetAddress(int userId, bool isBilling = true);

        /// <summary>
        /// Update the payment status for the specific Order
        /// </summary>
        /// <param name="omsOrderId">omsOrderId</param>
        /// <param name="paymentStatusId">paymentStatusId</param>
        /// <returns>Returns true if updated</returns>
        bool UpdatePaymentStatusV2(int omsOrderId, int paymentStatusId);

        /// <summary>
        /// Get the order details by order number.
        /// </summary>
        /// <param name="orderNumber">Order number</param>
        /// <param name="filters">Filters</param>
        /// <param name="expands">Expands</param>
        /// <returns></returns>
        OrderModel GetOrderByOrderNumber(string orderNumber, FilterCollection filters, NameValueCollection expands);
    }
}
