using System;
using System.Collections.Generic;
using Znode.Engine.Api.Models;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
using Znode.Libraries.ECommerce.Entities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Shipping
{
    public class ZnodeShippingCustomFixedRate : ZnodeBusinessBase
    {
        /// <summary>
        /// Calculates the custom fixed shipping rate.
        /// </summary>
        /// <param name="shippingBag">The shipping data for the custom shipping type.</param>
        public void Calculate(ZnodeShippingBag shippingBag)
        {
            ShippingRuleListModel shippingRules = new ShippingRuleListModel();
            IZnodeShippingHelper shippingHelper = GetService<IZnodeShippingHelper>();

            shippingRules.ShippingRuleList = GetFixedRatePerItemShippingRuleList(shippingBag);

            // Determine fixed shipping rate for each item
            foreach (ZnodeShoppingCartItem cartItem in shippingBag?.ShoppingCart?.ShoppingCartItems ?? new List<ZnodeShoppingCartItem>())
            {
                string shippingRuleTypeCode = Convert.ToString(cartItem.Product.ShippingRuleTypeCode);
                decimal itemShippingCost = 0;

                if (Equals(shippingRuleTypeCode?.ToLower(), shippingHelper.GetShippingRuleTypesEnumValue(Convert.ToString(ZnodeShippingRuleTypes.FixedRatePerItem)).ToLower()) && (!cartItem.Product.FreeShippingInd))
                {
                    foreach (ShippingRuleModel rule in shippingRules.ShippingRuleList)
                        itemShippingCost += rule.BaseCost;


                    itemShippingCost += cartItem.Product.ShippingRate;

                    cartItem.Product.ShippingCost = itemShippingCost;

                }
                // Reset if rule is applied
                bool isRuleApplied = true;

                bool applyHandlingCharge = false;

                // Get fixed rate for group product.
                cartItem.Product.ShippingCost = cartItem?.Product?.ZNodeGroupProductCollection?.Count > 0 ? CalculateProductShipping(cartItem?.Product?.ZNodeGroupProductCollection, out itemShippingCost, shippingBag) : cartItem.Product.ShippingCost;

                // Get fixed rate for addons product.
                foreach (ZnodeProductBaseEntity productBaseEntity in cartItem?.Product?.ZNodeAddonsProductCollection ?? new ZnodeGenericCollection<ZnodeProductBaseEntity>())
                    cartItem.Product.ShippingCost = GetShippingForAddOnForFixedRateBasedProduct(productBaseEntity, itemShippingCost, out applyHandlingCharge, isRuleApplied, shippingBag);
            }
        }

        // Calculate shipping for group, configure, addons, for fixed rate based.
        private decimal GetShippingForAddOnForFixedRateBasedProduct(ZnodeProductBaseEntity productBaseEntity, decimal itemShippingCost, out bool applyHandlingCharge, bool isRuleApplied, ZnodeShippingBag shippingBag)
        {
            ShippingRuleListModel shippingRules = new ShippingRuleListModel();
            IZnodeShippingHelper shippingHelper = GetService<IZnodeShippingHelper>();

            shippingRules.ShippingRuleList = GetFixedRatePerItemShippingRuleList(shippingBag);

            bool getsFreeShipping = productBaseEntity.FreeShippingInd;
            string shippingRuleTypeCode = Convert.ToString(productBaseEntity.ShippingRuleTypeCode);
            applyHandlingCharge = false;

            if (Equals(shippingRuleTypeCode?.ToLower(), shippingHelper.GetShippingRuleTypesEnumValue(Convert.ToString(ZnodeShippingRuleTypes.FixedRatePerItem)).ToLower())
                && !getsFreeShipping)
            {
                isRuleApplied &= ApplyRule(shippingRules?.ShippingRuleList, out itemShippingCost);

                applyHandlingCharge |= isRuleApplied;

                if (isRuleApplied)
                    productBaseEntity.ShippingCost = itemShippingCost;

            }
            return productBaseEntity.ShippingCost;
        }

        private decimal GetShippingHandlingChargeForPercentage(ZnodeShippingBag shippingBag)
            => shippingBag.CalculateShippingHandlingChargeInPercentage(shippingBag.ShoppingCart.OrderLevelShipping, shippingBag.HandlingCharge);

        private decimal GetShippingHandlingChargeForSubTotal(ZnodeShippingBag shippingBag)
            => shippingBag.CalculateShippingHandlingChargeInPercentage(shippingBag.SubTotal, shippingBag.HandlingCharge);

        // Apply rule fixed rate per item.
        private bool ApplyRule(List<ShippingRuleModel> shippingRules, out decimal itemShippingCost)
        {
            bool isRuleApplied = false;
            itemShippingCost = 0;

            foreach (ShippingRuleModel rule in shippingRules)
            {
                itemShippingCost += rule.BaseCost;
                isRuleApplied = true;
            }

            return isRuleApplied;
        }

        // Get shipping rule list for fixed rate per item.
        private List<ShippingRuleModel> GetFixedRatePerItemShippingRuleList(ZnodeShippingBag shippingBag)
        {
            IZnodeShippingHelper shippingHelper = GetService<IZnodeShippingHelper>();
            return shippingHelper.GetShippingRuleList(shippingHelper.GetShippingRuleTypesEnumValue(Convert.ToString(ZnodeShippingRuleTypes.FixedRatePerItem)), string.IsNullOrEmpty(shippingBag.ShoppingCart.Shipping.ShippingCountryCode) ? string.Empty :
                shippingBag.ShoppingCart.Shipping.ShippingCountryCode, Convert.ToInt32(shippingBag.ShoppingCart.PortalId), shippingBag.ShoppingCart.Shipping.ShippingID);
        }

        // Calculate group product shipping.
        private decimal CalculateProductShipping(ZnodeGenericCollection<ZnodeProductBaseEntity> productCollection, out decimal itemShippingCost, ZnodeShippingBag shippingBag)
        {
            ShippingRuleListModel shippingRules = new ShippingRuleListModel();
            IZnodeShippingHelper shippingHelper = GetService<IZnodeShippingHelper>();
            itemShippingCost = 0;

            shippingRules.ShippingRuleList = GetFixedRatePerItemShippingRuleList(shippingBag);

            foreach (ZnodeProductBaseEntity groupItem in productCollection ?? new ZnodeGenericCollection<ZnodeProductBaseEntity>())
            {
                string shippingRuleTypeCode = Convert.ToString(groupItem.ShippingRuleTypeCode);

                if (Equals(shippingRuleTypeCode?.ToLower(), shippingHelper.GetShippingRuleTypesEnumValue(Convert.ToString(ZnodeShippingRuleTypes.FixedRatePerItem)).ToLower()) && (!groupItem.FreeShippingInd))
                {
                    foreach (ShippingRuleModel rule in shippingRules.ShippingRuleList)
                        itemShippingCost += rule.BaseCost;

                    itemShippingCost += groupItem.ShippingRate;

                }
            }
            return itemShippingCost;
        }

        private void GetFixedRateHandlingCharge(ZnodeShippingBag shippingBag, bool applyHandlingCharge = false)
        {
            switch ((ZnodeShippingHandlingChargesBasedON)Enum.Parse(typeof(ZnodeShippingHandlingChargesBasedON), shippingBag.HandlingBasedOn))
            {
                case ZnodeShippingHandlingChargesBasedON.SubTotal:
                    shippingBag.ShoppingCart.OrderLevelShipping += GetShippingHandlingChargeForSubTotal(shippingBag);
                    break;
                case ZnodeShippingHandlingChargesBasedON.Shipping:
                    shippingBag.ShoppingCart.OrderLevelShipping += GetShippingHandlingChargeForPercentage(shippingBag);
                    break;
                case ZnodeShippingHandlingChargesBasedON.Amount:
                    shippingBag.ShoppingCart.OrderLevelShipping += shippingBag.HandlingCharge;
                    break;
                default:
                    break;
            }
        }
    }
}
