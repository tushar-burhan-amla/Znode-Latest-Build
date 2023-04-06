using System.Collections.Generic;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.ShoppingCart;
namespace Znode.Engine.Services.Maps
{
    public interface IShoppingCartMap
    {
        //Mapping to model.
        ShoppingCartModel ToModel(ZnodeShoppingCart znodeCart, IImageHelper objImage = null);

        //Mapping to Znode shopping cart model.
        ZnodeShoppingCart ToZnodeShoppingCart(ShoppingCartModel model, UserAddressModel userDetails = null, List<string> expands = null);
    }
}