using System;

namespace Znode.Engine.Shipping
{
    public class ZnodeShippingCustom : ZnodeShippingsType
    {
        public ZnodeShippingCustom()
        {
            Name = "Custom";
            Description = "Calculates custom shipping rates.";

            Controls.Add(ZnodeShippingControl.Profile);
            Controls.Add(ZnodeShippingControl.DisplayName);
            Controls.Add(ZnodeShippingControl.InternalCode);
            Controls.Add(ZnodeShippingControl.HandlingCharge);
            Controls.Add(ZnodeShippingControl.Countries);
        }

        // Calculates custom shipping rates.
        public override void Calculate()
        {
            var flatRateShipping = new ZnodeShippingCustomFlatRate();
            flatRateShipping.Calculate(ShoppingCart, ShippingBag);

            var quantityBasedShipping = new ZnodeShippingCustomQuantity();
            quantityBasedShipping.Calculate(ShoppingCart, ShippingBag);

            var weightBasedShipping = new ZnodeShippingCustomWeight();
            weightBasedShipping.Calculate(ShoppingCart, ShippingBag);

            var fixedRateShipping = new ZnodeShippingCustomFixedRate();
            fixedRateShipping.Calculate(ShippingBag);

            // Apply handling charge.            
            ShoppingCart.Shipping.ShippingHandlingCharge = GetShippingHandlingCharge();
            ShoppingCart.ShippingHandlingCharges = ShoppingCart.Shipping.ShippingHandlingCharge;
        }

        // Calculate shipping handling charge
        private decimal GetShippingHandlingCharge()
        {
            decimal shippingHandlingCharge = 0.0m;

            switch ((ZnodeShippingHandlingChargesBasedON)Enum.Parse(typeof(ZnodeShippingHandlingChargesBasedON), ShippingBag.HandlingBasedOn))
            {
                case ZnodeShippingHandlingChargesBasedON.SubTotal:
                    shippingHandlingCharge = GetShippingHandlingChargeBasedOnSubTotal();
                    break;
                case ZnodeShippingHandlingChargesBasedON.Shipping:
                    shippingHandlingCharge = GetShippingHandlingChargeBasedOnPercentage();
                    break;
                case ZnodeShippingHandlingChargesBasedON.Amount:
                    shippingHandlingCharge = ShippingBag.HandlingCharge;
                    break;
                default:
                    break;
            }
            return shippingHandlingCharge;
        }

        // Get shipping handling charge based on sub total.
        private decimal GetShippingHandlingChargeBasedOnPercentage()
            => ShippingBag.CalculateShippingHandlingChargeInPercentage(ShoppingCart.ShippingCost, ShippingBag.HandlingCharge);

        // Get shipping handling charge based on percentage.
        private decimal GetShippingHandlingChargeBasedOnSubTotal()
            => ShippingBag.CalculateShippingHandlingChargeInPercentage(ShippingBag.SubTotal, ShippingBag.HandlingCharge);
    }
}
