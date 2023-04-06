using Znode.Libraries.ECommerce.Entities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Shipping
{
    public class ZnodeOrderShippingHelper : IZnodeOrderShippingHelper
    {
        #region Variables
        protected ZnodeShoppingCart _shoppingCart;
        #endregion

        #region Constructor
        public ZnodeOrderShippingHelper(ZnodeShoppingCart shoppingCart)
        {
            _shoppingCart = shoppingCart;
        }
        #endregion

        #region Public Methods
        /// <summary>
        /// Set shipping for all line item in the cart.
        /// </summary>
        public virtual void OrderCalculate()
        {
            SetLineItemShipping();
        }

        // Set Line item Shipping cost
        public virtual void SetLineItemShipping()
        {
            _shoppingCart.Shipping.ShippingHandlingCharge = _shoppingCart.ShippingHandlingCharges;
            foreach (ZnodeShoppingCartItem cartItem in _shoppingCart.ShoppingCartItems)
            {
                SetShippinginCartItem(cartItem);
            }
        }

        // To set shipping in shopping cart item.
        protected virtual void SetShippinginCartItem(ZnodeShoppingCartItem cartItem)
        {
            if (cartItem.Product.ZNodeGroupProductCollection.Count == 0 && cartItem.Product.ZNodeConfigurableProductCollection.Count == 0)
                cartItem.Product.ShippingCost = cartItem.PerQuantityShippingCost * cartItem.Quantity;

            if (cartItem.Product.ZNodeGroupProductCollection.Count > 0)
                SetShippingInChildLineItem(cartItem.Product.ZNodeGroupProductCollection, cartItem);

            if (cartItem.Product.ZNodeConfigurableProductCollection.Count > 0)
                SetShippingInChildLineItem(cartItem.Product.ZNodeConfigurableProductCollection, cartItem);

        }

        public virtual void SetShippingInChildLineItem(ZnodeGenericCollection<ZnodeProductBaseEntity> productCollection, ZnodeShoppingCartItem cartItem)
        {
            if (productCollection?.Count > 0)
                foreach (ZnodeProductBaseEntity item in productCollection)
                {
                    item.ShippingCost = cartItem.PerQuantityShippingCost * cartItem.Quantity;
                }
        }

        // To set line item shipping cost.
        public virtual void SetOrderLinePerQuantityShippingCost()
        {
            _shoppingCart.Shipping.ShippingHandlingCharge = _shoppingCart.ShippingHandlingCharges;
            foreach (ZnodeShoppingCartItem cartItem in _shoppingCart.ShoppingCartItems)
            {
                if (_shoppingCart.IsOldOrder && cartItem.PerQuantityShippingCost > 0)
                {
                    SetShippinginCartItem(cartItem);
                    // ShippingHandlingCharges and ShippingDiscount gets reset in case of line item shipping edit.
                    _shoppingCart.ShippingHandlingCharges = 0;
                    _shoppingCart.Shipping.ShippingDiscount = 0;
                }
                else
                {
                    cartItem.PerQuantityShippingCost = cartItem.ShippingCost / cartItem.Quantity;
                }
            }
        }

        #endregion
    }
}
