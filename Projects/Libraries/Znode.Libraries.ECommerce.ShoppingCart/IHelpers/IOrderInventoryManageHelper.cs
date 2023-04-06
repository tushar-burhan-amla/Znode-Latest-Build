
using System.Collections.Generic;
using Znode.Engine.Api.Models;
using Znode.Libraries.Framework.Business;

namespace Znode.Libraries.ECommerce.ShoppingCart
{
    public interface IOrderInventoryManageHelper
    {
        // Set backordering for the shopping cart items
        void SetBackOrderingForShoppingCart(ZnodeGenericCollection<ZnodeShoppingCartItem> ShoppingCartItems);

        // Set backordering for the shopping cart items
        void SetBackOrderingForShoppingCart(List<ShoppingCartItemModel> shoppingCartItems, OrderModel model);
    }
}
