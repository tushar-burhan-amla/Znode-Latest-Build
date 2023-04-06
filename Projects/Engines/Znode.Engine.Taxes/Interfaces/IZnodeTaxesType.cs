using System.Collections.Generic;
using Znode.Libraries.ECommerce.Entities;

namespace Znode.Engine.Taxes.Interfaces
{   /// <summary>
    ///This Is The root Interfaces for All Tax Type
    /// </summary>

    public interface IZnodeTaxesType : IZnodeProviderType
    {     
        int Precedence { get; set; }


        /// <summary>
        /// This is Collection of all The Tax rule
        /// </summary>
        List<ZnodeTaxRuleControl> Controls { get; }

        /// <summary>
        /// Binds the shopping cart and tax data to the tax rule.
        /// </summary>
        /// <param name="shoppingCart">The current shopping cart.</param>
        /// <param name="taxBag">The tax properties.</param>
        void Bind(ZnodeShoppingCart shoppingCart, ZnodeTaxBag taxBag);

        /// <summary>
        /// Calculates the tax and updates the shopping cart.
        /// </summary>
        void Calculate();

        /// <summary>
        /// Process anything that must be done before the order is submitted.
        ///</summary>
        /// <returns>True if everything is good for submitting the order; otherwise, false.</returns>
        bool PreSubmitOrderProcess();

        /// <summary>
        /// Process anything that must be done after the order is submitted.
        /// </summary>
        void PostSubmitOrderProcess(bool isTaxCostUpdated = true);

    }
}
