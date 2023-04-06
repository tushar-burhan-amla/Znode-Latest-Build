using System.Collections.Generic;

using Znode.Libraries.ECommerce.Entities;

namespace Znode.Libraries.ECommerce.ShoppingCart
{
    public interface IZnodePortalCart : IZnodeShoppingCart, IZnodeShoppingCartEntities
    {
        int PortalID { get; set; }
        List<IZnodeMultipleAddressCart> AddressCarts
        {
            get;
        }


    }
}
