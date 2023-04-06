using System;

namespace Znode.Libraries.ECommerce.ShoppingCart
{
    public interface IZnodeMultipleAddressCart : IZnodeShoppingCart
    {
        Guid AddressCartID { get; set; }

        int AddressID { get; set; }

        int OrderShipmentID { get; set; }

    }
}
