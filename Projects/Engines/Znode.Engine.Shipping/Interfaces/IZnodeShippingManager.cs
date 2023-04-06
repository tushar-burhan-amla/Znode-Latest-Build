using System.Collections.Generic;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Entities;

namespace Znode.Engine.Shipping
{
    public interface IZnodeShippingManager
    {
        /// <summary>
        /// Calculates the shipping cost and updates the shopping cart.
        /// </summary>
        void Calculate();

        /// <summary>
        /// Process anything that must be done before the order is submitted.
        /// </summary>
        /// <returns>True if everything is good for submitting the order; otherwise, false.</returns>
        bool PreSubmitOrderProcess();

        /// <summary>
        /// Process anything that must be done after the order is submitted.
        /// </summary>
        /// <returns>True if everything is good for submitting the order; otherwise, false.</returns>
        void PostSubmitOrderProcess();

        /// <summary>
        /// Create and return instance for Shipping classes
        /// </summary>
        T GetShippingTypeInstance<T>(string className) where T : class;

        List<ShippingModel> GetShippingEstimateRate(ZnodeShoppingCart znodeShoppingCart, ShoppingCartModel cartModel, string countryCode, List<ZnodeShippingBag> shippingbagList);
    }
}
