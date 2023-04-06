using System.Collections.Generic;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Entities;

namespace Znode.Engine.Taxes
{
    public interface IZnodeTaxManager
    {
        /// <summary>
        /// Calculates the sales tax for the shopping cart and its items. 
        /// </summary>
        void Calculate(ZnodeShoppingCart shoppingCart);

        /// <summary>
        /// Process anything that must be done before the order is submitted.
        /// </summary>
        bool PreSubmitOrderProcess(ZnodeShoppingCart shoppingCart);

        /// <summary>
        /// Process anything that must be done after the order is submitted.
        /// </summary>
        void PostSubmitOrderProcess(ZnodeShoppingCart shoppingCart, bool isTaxCostUpdated = true);

        void CancelOrderRequest(ShoppingCartModel shoppingCartModel);

        /// <summary>
        /// Return order line item.
        /// </summary>
        void ReturnOrderLineItem(ShoppingCartModel orderModel);

        /// <summary>
        /// Create and return instance for tax type classes
        /// </summary>
        T GetTaxTypeInstance<T>(string className) where T : class;

        /// <summary>
        /// Get the list
        /// </summary>
        List<TaxRuleModel> TaxRules { get; }
    }
}
