using System;
using System.Text;
using Znode.Engine.Promotions;
using Znode.Engine.Shipping;
using Znode.Engine.Taxes;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
using ZnodeEntityType = Znode.Libraries.ECommerce.Entities;
namespace Znode.Libraries.ECommerce.ShoppingCart
{
    [Serializable()]
    public class ZnodeMultipleAddressCart : ZnodeShoppingCart, IZnodeMultipleAddressCart
    {
        public Guid AddressCartID { get; set; }

        public int AddressID { get; set; }

        public int OrderShipmentID { get; set; }

        public ZnodeMultipleAddressCart() : base()
        {
            AddressCartID = Guid.NewGuid();
        }

        public ZnodeMultipleAddressCart(int addressID, ZnodeShippings shipping, ZnodeGenericCollection<ZnodeShoppingCartItem> shoppingCartItems) : base()
        {
            AddressCartID = Guid.NewGuid();
            this.AddressID = addressID;
            this.Shipping = shipping;
            this.ShoppingCartItems = shoppingCartItems;
        }

        /// <summary>
        /// Calculates final pricing, shipping and taxes in the cart.
        /// </summary>
        public override void Calculate()
        {
            // Clear previous messages
            _ErrorMessage = new StringBuilder();

            if (!IsQuoteToOrder)
            {
                // ShippingRules
                IZnodeShippingManager shipping = GetService<IZnodeShippingManager>(new ZnodeTypedParameter(typeof(ZnodeEntityType.ZnodeShoppingCart), this));
                shipping.Calculate();
            }

            IZnodeCartPromotionManager cartPromoManager = GetService<IZnodeCartPromotionManager>(new ZnodeNamedParameter("shoppingCart", this), new ZnodeNamedParameter("profileId", null));
            cartPromoManager.Calculate();

            IZnodeTaxManager taxRules = GetService<IZnodeTaxManager>(new ZnodeTypedParameter(typeof(ZnodeEntityType.ZnodeShoppingCart), this));
            taxRules.Calculate(this);
        }
    }
}
