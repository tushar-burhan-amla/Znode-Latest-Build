using System;
using System.Collections.Generic;
using Znode.Engine.Api.Models;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
using Znode.Libraries.ECommerce.Entities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Shipping
{
    public class ZnodeShippingCustomWeight : ZnodeBusinessBase
    {
        /// <summary>
        /// Calculates the custom shipping rate based on weight.
        /// </summary>
        /// <param name="shoppingCart">The current shopping cart.</param>
        /// <param name="shippingBag">The shipping data for the custom shipping type.</param>
        public void Calculate(ZnodeShoppingCart shoppingCart, ZnodeShippingBag shippingBag)
        {
            ShippingRuleListModel shippingRules = new ShippingRuleListModel();
            IZnodeShippingHelper shippingHelper = GetService<IZnodeShippingHelper>();

            shippingRules.ShippingRuleList = GetRateBasedOnWeightRuleList(shoppingCart);

            // Determine weight-based shipping rate for each item
            foreach (ZnodeShoppingCartItem cartItem in shippingBag?.ShoppingCart?.ShoppingCartItems ?? new List<ZnodeShoppingCartItem>())
            {
                decimal itemShippingCost = 0;
                decimal totalItemWeightQuantity = 0;
                bool isRuleApplied = false;

                string shippingRuleTypeCode = cartItem.Product.ShippingRuleTypeCode;
                bool getsFreeShipping = cartItem.Product.FreeShippingInd;
                decimal quantity = cartItem.Quantity;

                if (Equals(shippingRuleTypeCode?.ToLower(), shippingHelper.GetShippingRuleTypesEnumValue(Convert.ToString(ZnodeShippingRuleTypes.RateBasedOnWeight)).ToLower()) && (!getsFreeShipping))
                    totalItemWeightQuantity = cartItem.Product.Weight * quantity;

                if (totalItemWeightQuantity > 0)
                {
                    isRuleApplied = ApplyRule(shippingRules.ShippingRuleList, totalItemWeightQuantity, out itemShippingCost);



                    if (isRuleApplied)
                        cartItem.Product.ShippingCost = itemShippingCost;

                }

                // Reset if rule is applied
                isRuleApplied = true;

                bool applyHandlingCharge = false;

                // Get weight-based rate for group product.
                cartItem.Product.ShippingCost = cartItem?.Product?.ZNodeGroupProductCollection?.Count > 0 ? CalculateProductShipping(out itemShippingCost, shippingBag, cartItem?.Product?.ZNodeGroupProductCollection, shoppingCart) : cartItem.Product.ShippingCost;
                
                // Get weight-based rate for addons product.
                foreach (ZnodeProductBaseEntity productBaseEntity in cartItem?.Product?.ZNodeAddonsProductCollection ?? new ZnodeGenericCollection<ZnodeProductBaseEntity>())
                    cartItem.Product.ShippingCost = GetShippingForAddOnWeightBasedProduct(productBaseEntity, itemShippingCost, out applyHandlingCharge, isRuleApplied, shoppingCart);
            }
        }

        // Calculate shipping for group, configure, addons, weight-based product.
        private decimal GetShippingForAddOnWeightBasedProduct(ZnodeProductBaseEntity productBaseEntity, decimal itemShippingCost, out bool applyHandlingCharge, bool isRuleApplied, ZnodeShoppingCart shoppingCart)
        {
            ShippingRuleListModel shippingRules = new ShippingRuleListModel();
            IZnodeShippingHelper shippingHelper = GetService<IZnodeShippingHelper>();
            shippingRules.ShippingRuleList = GetRateBasedOnWeightRuleList(shoppingCart);

            bool getsFreeShipping = productBaseEntity.FreeShippingInd;
            string shippingRuleTypeCode = productBaseEntity.ShippingRuleTypeCode;
            applyHandlingCharge = false;

            if (Equals(shippingRuleTypeCode?.ToLower(), shippingHelper.GetShippingRuleTypesEnumValue(Convert.ToString(ZnodeShippingRuleTypes.RateBasedOnWeight)).ToLower()) && !getsFreeShipping)
            {
                decimal addOnItemWeightQuantity = productBaseEntity.Weight * productBaseEntity.SelectedQuantity;

                isRuleApplied &= ApplyRule(shippingRules.ShippingRuleList, addOnItemWeightQuantity, out itemShippingCost);

                applyHandlingCharge |= isRuleApplied;

                if (isRuleApplied)
                    productBaseEntity.ShippingCost = itemShippingCost;
            }
            return productBaseEntity.ShippingCost;
        }

        // ShippingRule dependency on data access
        private bool ApplyRule(List<ShippingRuleModel> shippingRules, decimal itemWeightQuantity, out decimal itemShippingCost)
        {
            bool isRuleApplied = false;
            itemShippingCost = 0;

            foreach (ShippingRuleModel rule in shippingRules)
            {
                if (itemWeightQuantity >= rule.LowerLimit && itemWeightQuantity <= rule.UpperLimit)
                {
                    itemShippingCost += rule.BaseCost + (rule.PerItemCost * itemWeightQuantity);
                    isRuleApplied = true;
                }
            }
            return isRuleApplied;
        }

        // Get shipping rule list for rate based on weight.
        private List<ShippingRuleModel> GetRateBasedOnWeightRuleList(ZnodeShoppingCart shoppingCart)
        {
            IZnodeShippingHelper shippingHelper = GetService<IZnodeShippingHelper>();
            return shippingHelper.GetShippingRuleList(shippingHelper.GetShippingRuleTypesEnumValue(Convert.ToString(ZnodeShippingRuleTypes.RateBasedOnWeight)), string.IsNullOrEmpty(shoppingCart.Shipping.ShippingCountryCode) ? string.Empty : shoppingCart.Shipping.ShippingCountryCode, Convert.ToInt32(shoppingCart.PortalId), shoppingCart.Shipping.ShippingID);
        }

        // Calculate group product shipping.
        private decimal CalculateProductShipping(out decimal itemShippingCost, ZnodeShippingBag shippingBag, ZnodeGenericCollection<ZnodeProductBaseEntity> productCollection, ZnodeShoppingCart shoppingCart)
        {
            itemShippingCost = 0;

            ShippingRuleListModel shippingRules = new ShippingRuleListModel();
            IZnodeShippingHelper shippingHelper = GetService<IZnodeShippingHelper>();
            shippingRules.ShippingRuleList = GetRateBasedOnWeightRuleList(shoppingCart);

            foreach (ZnodeProductBaseEntity groupItem in productCollection ?? new ZnodeGenericCollection<ZnodeProductBaseEntity>())
            {
                decimal productShippingCost = 0;
                decimal totalItemWeightQuantity = 0;
                bool isRuleApplied = false;

                string shippingRuleTypeCode = groupItem.ShippingRuleTypeCode;
                bool getsFreeShipping = groupItem.FreeShippingInd;
                decimal quantity = groupItem.SelectedQuantity;

                if (Equals(shippingRuleTypeCode?.ToLower(), shippingHelper.GetShippingRuleTypesEnumValue(Convert.ToString(ZnodeShippingRuleTypes.RateBasedOnWeight)).ToLower()) && (!getsFreeShipping))
                    totalItemWeightQuantity = groupItem.Weight * quantity;

                if (totalItemWeightQuantity > 0)
                {
                    isRuleApplied = ApplyRule(shippingRules.ShippingRuleList, totalItemWeightQuantity, out productShippingCost);

                    if (isRuleApplied && groupItem.ShipSeparately)
                        groupItem.ShippingCost = shippingHelper.GetItemHandlingCharge(productShippingCost, shippingBag);


                    else if (isRuleApplied)
                        groupItem.ShippingCost = productShippingCost;

                    itemShippingCost += productShippingCost;
                }
            }
            return itemShippingCost;
        }
    }
}
