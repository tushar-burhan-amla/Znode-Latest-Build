using System.Collections.Generic;
using Znode.Libraries.ECommerce.Entities;

namespace Znode.Engine.Shipping
{
    /// <summary>
    /// This is the root interface for all shipping types.
    /// </summary>
    public interface IZnodeShippingsType : IZnodeProviderType
    {
        List<ZnodeShippingControl> Controls { get; }

        void Bind(ZnodeShoppingCart shoppingCart, ZnodeShippingBag shippingBag);
        void Calculate();
        bool PreSubmitOrderProcess();
        void PostSubmitOrderProcess();
    }
}
