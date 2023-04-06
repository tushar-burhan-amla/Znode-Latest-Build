using System.Collections.Generic;

using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.ShoppingCart;

namespace Znode.Engine.Services.Maps
{
    public interface IZnodeShippingCartMap
    {
        //Perform mapping to shipping ZnodeShoppingCart model for shipping.
        ZnodeShoppingCart ToZnodeShippingShoppingCart(ShoppingCartModel model, List<string> expands = null);
    }
}