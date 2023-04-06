using System;
using System.Collections.Generic;
using Znode.Engine.Api.Models;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
using Znode.Libraries.ECommerce.Entities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Shipping
{
    public class ZnodeShippingCustomQuantity : ZnodeBusinessBase
    {
        /// <summary>
        /// Calculates the custom shipping rate based on quantity.
        /// </summary>
        /// <param name="shoppingCart">The current shopping cart.</param>
        /// <param name="shippingBag">The shipping data for the custom shipping type.</param>
        public void Calculate(ZnodeShoppingCart shoppingCart, ZnodeShippingBag shippingBag)
        {
            ShippingRuleListModel shippingRules = new ShippingRuleListModel();
            IZnodeShippingHelper shippingHelper = GetService<IZnodeShippingHelper>();

            shippingRules.ShippingRuleList = GetRateBasedOnQuantityRuleList(shoppingCart);

            // Determine quantity-based shipping rate for each item
            foreach (ZnodeShoppingCartItem cartItem in shippingBag?.ShoppingCart?.ShoppingCartItems ?? new List<ZnodeShoppingCartItem>())
            {
                decimal itemShippingCost = 0;
                decimal totalItemQuantity = 0;
                bool isRuleApplied = false;

                string shippingRuleTypeCode = Convert.ToString(cartItem.Product.ShippingRuleTypeCode);
                bool getsFreeShipping = cartItem.Product.FreeShippingInd;
                decimal quantity = cartItem.Quantity;

                if (Equals(shippingRuleTypeCode?.ToLower(), shippingHelper.GetShippingRuleTypesEnumValue(Convert.ToString(ZnodeShippingRuleTypes.RateBasedOnQuantity)).ToLower()) && (!getsFreeShipping))
                    totalItemQuantity += quantity;

                if (totalItemQuantity > 0)
                {
                    isRuleApplied = ApplyRule(shippingRules.ShippingRuleList, totalItemQuantity, out itemShippingCost);

                    if (isRuleApplied && cartItem.Product.ShipSeparately)
                        cartItem.Product.ShippingCost = shippingHelper.GetItemHandlingCharge(itemShippingCost, shippingBag);

                    else if (isRuleApplied)
                        cartItem.Product.ShippingCost = itemShippingCost;

                }

                // Reset if rule is applied
                isRuleApplied = true;

                bool applyHandlingCharge = false;

                // Get shipping rate on quantity based for group product               
                foreach (ZnodeProductBaseEntity productBaseEntity in cartItem.Product?.ZNodeGroupProductCollection ?? new ZnodeGenericCollection<ZnodeProductBaseEntity>())
                    productBaseEntity.ShippingCost = CalculateProductShipping(shippingBag, cartItem?.Product?.ZNodeGroupProductCollection, shoppingCart, out itemShippingCost);

                //Comment below line for fixed issue ZPD-1690
                // Get shipping rate on quantity based for configurable product
                //cartItem.Product.ShippingCost = cartItem?.Product?.ZNodeConfigurableProductCollection?.Count > 0 ? CalculateProductShipping(shippingBag, cartItem?.Product?.ZNodeGroupProductCollection, shoppingCart, out itemShippingCost) : cartItem.Product.ShippingCost;

                // Get shipping rate on quantity based for addons product
                foreach (ZnodeProductBaseEntity productBaseEntity in cartItem?.Product?.ZNodeAddonsProductCollection ?? new ZnodeGenericCollection<ZnodeProductBaseEntity>())
                    cartItem.Product.ShippingCost = GetShippingForAddOnForQuantityBasedProduct(productBaseEntity, itemShippingCost, out applyHandlingCharge, isRuleApplied, shoppingCart);
            }
        }

        // Calculate shipping for group, configure, addons, for quantity based product.
        private decimal GetShippingForAddOnForQuantityBasedProduct(ZnodeProductBaseEntity productBaseEntity, decimal itemShippingCost, out bool applyHandlingCharge, bool isRuleApplied, ZnodeShoppingCart shoppingCart)
        {
            ShippingRuleListModel shippingRules = new ShippingRuleListModel();
            IZnodeShippingHelper shippingHelper = GetService<IZnodeShippingHelper>();

            shippingRules.ShippingRuleList = GetRateBasedOnQuantityRuleList(shoppingCart);

            bool getsFreeShipping = productBaseEntity.FreeShippingInd;
            string shippingRuleTypeCode = Convert.ToString(productBaseEntity.ShippingRuleTypeCode);
            applyHandlingCharge = false;

            if (Equals(shippingRuleTypeCode?.ToLower(), shippingHelper.GetShippingRuleTypesEnumValue(Convert.ToString(ZnodeShippingRuleTypes.RateBasedOnQuantity)).ToLower()) && !getsFreeShipping)
            {
                isRuleApplied &= ApplyRule(shippingRules.ShippingRuleList, productBaseEntity.SelectedQuantity, out itemShippingCost);

                applyHandlingCharge |= isRuleApplied;

                if (isRuleApplied)
                    productBaseEntity.ShippingCost = itemShippingCost;
            }

            return productBaseEntity.ShippingCost;
        }

        // Apply rule.
        private bool ApplyRule(List<ShippingRuleModel> shippingRules, decimal itemQuantity, out decimal itemShippingCost)
        {
            bool isRuleApplied = false;
            itemShippingCost = 0;

            foreach (ShippingRuleModel rule in shippingRules)
            {
                if (itemQuantity >= rule.LowerLimit && itemQuantity <= rule.UpperLimit)
                {
                    itemShippingCost += rule.BaseCost + (rule.PerItemCost * itemQuantity);
                    isRuleApplied = true;
                }
            }
            return isRuleApplied;
        }

        // Get Get shipping rule list  for rate based on quantity.
        private List<ShippingRuleModel> GetRateBasedOnQuantityRuleList(ZnodeShoppingCart shoppingCart)
        {
            IZnodeShippingHelper shippingHelper = GetService<IZnodeShippingHelper>();
            return shippingHelper.GetShippingRuleList(shippingHelper.GetShippingRuleTypesEnumValue(Convert.ToString(ZnodeShippingRuleTypes.RateBasedOnQuantity)), string.IsNullOrEmpty(shoppingCart.Shipping.ShippingCountryCode) ? string.Empty : shoppingCart.Shipping.ShippingCountryCode, Convert.ToInt32(shoppingCart.PortalId), shoppingCart.Shipping.ShippingID);
        }

        // Calculate shipping rate for group product.
        private decimal CalculateProductShipping(ZnodeShippingBag shippingBag, ZnodeGenericCollection<ZnodeProductBaseEntity> productCollection, ZnodeShoppingCart shoppingCart, out decimal itemShippingCost)
        {
            itemShippingCost = 0;

            ShippingRuleListModel shippingRules = new ShippingRuleListModel();
            IZnodeShippingHelper shippingHelper = GetService<IZnodeShippingHelper>();

            shippingRules.ShippingRuleList = GetRateBasedOnQuantityRuleList(shoppingCart);

            foreach (ZnodeProductBaseEntity productItem in productCollection ?? new ZnodeGenericCollection<ZnodeProductBaseEntity>())
            {
                decimal productShippingCost = 0;
                decimal totalItemQuantity = 0;
                bool isRuleApplied = false;

                string shippingRuleTypeCode = Convert.ToString(productItem.ShippingRuleTypeCode);
                bool getsFreeShipping = productItem.FreeShippingInd;
                decimal quantity = productItem.SelectedQuantity;

                if (Equals(shippingRuleTypeCode?.ToLower(), shippingHelper.GetShippingRuleTypesEnumValue(Convert.ToString(ZnodeShippingRuleTypes.RateBasedOnQuantity)).ToLower()) && (!getsFreeShipping))
                    totalItemQuantity += quantity;

                if (totalItemQuantity > 0)
                {
                    isRuleApplied = ApplyRule(shippingRules.ShippingRuleList, totalItemQuantity, out productShippingCost);

                     if (isRuleApplied)
                        productItem.ShippingCost = productShippingCost;

                    itemShippingCost += productShippingCost;
                }
            }
            return itemShippingCost;
        }



    }
}
