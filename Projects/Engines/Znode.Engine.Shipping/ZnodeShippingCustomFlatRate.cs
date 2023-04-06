using System;
using System.Collections.Generic;
using Znode.Engine.Api.Models;
using static Znode.Libraries.ECommerce.Utilities.HelperUtility;
using Znode.Libraries.ECommerce.Entities;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;

namespace Znode.Engine.Shipping
{
    public class ZnodeShippingCustomFlatRate : ZnodeBusinessBase
    {
        /// <summary>
        /// Calculates the custom fixed shipping rate.
        /// </summary>
        /// <param name="shoppingCart">The current shopping cart.</param>
        /// <param name="shippingBag">The shipping data for the custom shipping type.</param>
        public void Calculate(ZnodeShoppingCart shoppingCart, ZnodeShippingBag shippingBag)
        {
            ShippingRuleListModel shippingRules = new ShippingRuleListModel();
            IZnodeShippingHelper shippingHelper = GetService<IZnodeShippingHelper>();

            shippingRules.ShippingRuleList = GetFlatRatePerItemShippingRuleList(shoppingCart);

            bool isHandlingChargeApplied = false;
            //Determine flat shipping rate for each item
            foreach (ZnodeShoppingCartItem cartItem in shippingBag?.ShoppingCart?.ShoppingCartItems ?? new List<ZnodeShoppingCartItem>())
            {
                decimal itemShippingCost = 0;
                decimal totalItemQuantity = 0;
                bool isRuleApplied = false;
              
                if (IsNotNull(cartItem))
                {
                    string shippingRuleTypeCode = Convert.ToString(cartItem.Product?.ShippingRuleTypeCode);
                    bool getsFreeShipping = cartItem.Product.FreeShippingInd;
                    decimal quantity = cartItem.Quantity;

                    if (Equals(shippingRuleTypeCode?.ToLower(), shippingHelper.GetShippingRuleTypesEnumValue(Convert.ToString(ZnodeShippingRuleTypes.FlatRatePerItem)).ToLower()) && (!getsFreeShipping))
                        totalItemQuantity += quantity;

                    if (totalItemQuantity > 0)
                    {
                        isRuleApplied = ApplyRule(shippingRules.ShippingRuleList, totalItemQuantity, out itemShippingCost);

                        if (isRuleApplied && cartItem.Product.ShipSeparately &&!isHandlingChargeApplied)
                        {
                            cartItem.Product.ShippingCost = shippingHelper.GetItemHandlingCharge(itemShippingCost, shippingBag);
                            isHandlingChargeApplied = true;
                        }
                        else if (isRuleApplied)
                            cartItem.Product.ShippingCost = itemShippingCost;

                    }

                    // Reset if rule is applied
                    isRuleApplied = true;

                    bool applyHandlingCharge = false;

                    // Get flat rate for group product.
                    foreach (ZnodeProductBaseEntity productBaseEntity in cartItem.Product?.ZNodeGroupProductCollection ?? new ZnodeGenericCollection<ZnodeProductBaseEntity>())
                        productBaseEntity.ShippingCost = CalculateProductShipping(cartItem.Product?.ZNodeGroupProductCollection, out itemShippingCost, shippingBag, shoppingCart);

                    // Get flat rate for addons product
                    foreach (ZnodeProductBaseEntity productBaseEntity in cartItem.Product?.ZNodeAddonsProductCollection ?? new ZnodeGenericCollection<ZnodeProductBaseEntity>())
                        productBaseEntity.ShippingCost = GetShippingForAddOnForFlatRateBasedProduct(productBaseEntity, itemShippingCost, out applyHandlingCharge, isRuleApplied, shoppingCart);

                }
            }
            //Handling charges will be applied at the order level.
            shoppingCart.Shipping.ShippingHandlingCharge = shippingHelper.GetOrderLevelShippingHandlingCharge(shippingBag, shoppingCart);
        }

        // Calculate shipping for group,configure and addon flat rate based product.
        private decimal GetShippingForAddOnForFlatRateBasedProduct(ZnodeProductBaseEntity productBaseEntity, decimal itemShippingCost, out bool applyHandlingCharge, bool isRuleApplied, ZnodeShoppingCart shoppingCart)
        {
            ShippingRuleListModel shippingRules = new ShippingRuleListModel();
            IZnodeShippingHelper shippingHelper = GetService<IZnodeShippingHelper>();
            shippingRules.ShippingRuleList = GetFlatRatePerItemShippingRuleList(shoppingCart);
            applyHandlingCharge = false;
            bool getsFreeShipping = productBaseEntity.FreeShippingInd;

            string shippingRuleTypeCode = Convert.ToString(productBaseEntity.ShippingRuleTypeCode);

            if (Equals(shippingRuleTypeCode?.ToLower(), shippingHelper.GetShippingRuleTypesEnumValue(Convert.ToString(ZnodeShippingRuleTypes.FlatRatePerItem)).ToLower()) && !getsFreeShipping)
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
                itemShippingCost += rule.BaseCost + (rule.PerItemCost * itemQuantity);
                isRuleApplied = true;
            }
            return isRuleApplied;
        }

        // Get shipping rule list for flat rate per item.
        private List<ShippingRuleModel> GetFlatRatePerItemShippingRuleList(ZnodeShoppingCart shoppingCart)
        {
            IZnodeShippingHelper shippingHelper = GetService<IZnodeShippingHelper>();
            return shippingHelper.GetShippingRuleList(shippingHelper.GetShippingRuleTypesEnumValue(Convert.ToString(ZnodeShippingRuleTypes.FlatRatePerItem)), string.IsNullOrEmpty(shoppingCart.Shipping.ShippingCountryCode) ? string.Empty : shoppingCart.Shipping.ShippingCountryCode, Convert.ToInt32(shoppingCart.PortalId), shoppingCart.Shipping.ShippingID);
        }

        // Calculate shipping for group product.
        private decimal CalculateProductShipping(ZnodeGenericCollection<ZnodeProductBaseEntity> productCollection, out decimal itemShippingCost, ZnodeShippingBag shippingBag, ZnodeShoppingCart shoppingCart)
        {
            itemShippingCost = 0;
            ShippingRuleListModel shippingRules = new ShippingRuleListModel();
            IZnodeShippingHelper shippingHelper = GetService<IZnodeShippingHelper>();

            shippingRules.ShippingRuleList = GetFlatRatePerItemShippingRuleList(shoppingCart);

            foreach (ZnodeProductBaseEntity productItem in productCollection ?? new ZnodeGenericCollection<ZnodeProductBaseEntity>())
            {
                decimal productShippingCost = 0;
                decimal totalItemQuantity = 0;
                bool isRuleApplied = false;

                string shippingRuleTypeCode = Convert.ToString(productItem.ShippingRuleTypeCode);
                bool getsFreeShipping = productItem.FreeShippingInd;
                decimal quantity = productItem.SelectedQuantity;

                if (Equals(shippingRuleTypeCode?.ToLower(), shippingHelper.GetShippingRuleTypesEnumValue(Convert.ToString(ZnodeShippingRuleTypes.FlatRatePerItem)).ToLower()) && (!getsFreeShipping))
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
