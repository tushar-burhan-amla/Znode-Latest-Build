
using Znode.Engine.Api.Models;

namespace Znode.Engine.Api.Cache
{
    public interface IShoppingCartCache
    {
        /// <summary>
        /// To get shopping cart by user/cookie mapping id.
        /// </summary>
        /// <param name="routeUri"></param>
        /// <param name="cartParameterModel"></param>
        /// <returns></returns>
        ShoppingCartModel GetShoppingCart(string routeUri, CartParameterModel cartParameterModel);

        /// <summary>
        /// To get shopping cart count
        /// </summary>
        /// <param name="routeUri"></param>
        /// <param name="cartParameterModel"></param>
        /// <returns></returns>
       string GetCartCount(string routeUri, CartParameterModel cartParameterModel);
        /// <summary>
        /// Creates a new shopping cart and saves it to the database.
        /// </summary>
        /// <param name="routeUri"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        ShoppingCartModel CreateCart(string routeUri, ShoppingCartModel model);

        /// <summary>
        /// Creates a new cart and saves it to the database.
        /// </summary>
        /// <param name="routeUri"></param>
        /// <param name="model">AddToCartModel</param>
        /// <returns></returns>
        AddToCartModel AddToCartProduct(string routeUri, AddToCartModel model);

        /// <summary>
        /// Performs calculations for a shopping cart.
        /// </summary>
        /// <param name="routeUri"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        ShoppingCartModel Calculate(string routeUri, ShoppingCartModel model);

        /// <summary>
        /// 
        /// </summary>
        /// <param name="zipCode"></param>
        /// <param name="routeUri"></param>
        /// <param name="model"></param>
        /// <returns></returns>
        ShippingListModel GetShippingEstimates(string zipCode, string routeUri, ShoppingCartModel model);

        /// <summary>
        /// Get OmsLineItem Detail by omsOrderId
        /// </summary>
        /// <param name="omsOrderId">omsOrderId</param>
        /// <returns>List of OmsLineItems</returns>
        OrderLineItemDataListModel GetOrderLineItemDetails(int omsOrderId);

        /// <summary>
        /// Merge cart after login
        /// </summary>
        /// <param name="routeUri"></param>
        /// <returns></returns>
        bool MergeGuestUsersCart(string routeUri);
    }
}
