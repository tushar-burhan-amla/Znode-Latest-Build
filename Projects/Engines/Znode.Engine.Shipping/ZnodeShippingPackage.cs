using System;
using System.Collections.Generic;
using Znode.Libraries.ECommerce.Entities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Shipping
{
    /// <summary>
    /// Calculates the size of a package from a list of products. 
    /// </summary>    
    public class ZnodeShippingPackage : ZnodeBusinessBase
    {
        public decimal Width { get; set; }
        public decimal Height { get; set; }
        public decimal Length { get; set; }
        public decimal Weight { get; set; }
        public decimal Value { get; set; }
        public bool UseGrossValue { get; set; }
        public bool RoundWeightToNextIncrement { get; set; }
        public ZnodeShoppingCartItem ShoppingCartItem { get; set; }
        public ZnodeGenericCollection<ZnodeShoppingCartItem> ShoppingCartItems { get; set; }
        public List<ZnodeProductBaseEntity> Products { get; set; }

        public ZnodeShippingPackage(ZnodeGenericCollection<ZnodeShoppingCartItem> shoppingCartItems) : this(shoppingCartItems, false, false)
        {
        }

        public ZnodeShippingPackage(ZnodeGenericCollection<ZnodeShoppingCartItem> shoppingCartItems, bool roundWeightToNextIncrement, bool useGrossValue)
        {
            ShoppingCartItem = new ZnodeShoppingCartItem();
            ShoppingCartItems = shoppingCartItems;
            Products = new List<ZnodeProductBaseEntity>();
            RoundWeightToNextIncrement = roundWeightToNextIncrement;
            UseGrossValue = useGrossValue;
            EstimateShipmentPackageForShoppingCartItems();
        }

        public ZnodeShippingPackage(List<ZnodeProductBaseEntity> products)
        {
            ShoppingCartItem = new ZnodeShoppingCartItem();
            ShoppingCartItems = new ZnodeGenericCollection<ZnodeShoppingCartItem>();
            Products = products;
            RoundWeightToNextIncrement = true;
            UseGrossValue = false;
            EstimateShipmentPackageForProductsAndAddOns();
        }

        private void EstimatePackageDimensions(decimal weight, decimal height, decimal width, decimal length, decimal price, ref decimal quantity, ref decimal totalVolume, ref decimal totalWeight, ref decimal totalValue, decimal itemWeight, bool getsFreeShipping)
        {
            if (!getsFreeShipping)
            {
                // For fedex we need to calculate volume.
                // Get dimensions
                totalVolume += (height * width * length) * quantity;

                //Check to round the weight up to the next pound              

                itemWeight = RoundWeightToNextIncrement ? itemWeight + Math.Round(weight + 0.51m, 0) : itemWeight + weight;

                if (UseGrossValue)
                {
                    totalValue += price * quantity;
                    itemWeight *= quantity;
                }
                else
                    totalValue += price;

                totalWeight += (itemWeight * quantity);
            }
        }

        /// <summary>
        /// Estimates the total package size and weight when given list of products and addons.
        /// </summary>
        private void EstimateShipmentPackageForProductsAndAddOns()
        {
            // We are going to get a very approximate package size by taking the total volume of all items in
            // the cart and then getting the cube root of that volume. This will give us a minimum package size.
            // NOTE: This will underestimate the total package volume since packages will rarely be cubes!

            decimal totalVolume = 0;
            decimal totalWeight = 0;
            decimal totalValue = 0;

            foreach (var p in Products)
            {
                // Quantity is set to 1 because we're sending in a product row for every single item in the order
                decimal quantity = 1;

                if (p.Weight > 0 && p.Height > 0 && p.Width > 0 && p.Length > 0 && p.RetailPrice > 0)
                {
                    decimal itemWeight = 0;
                    EstimatePackageDimensions(p.Weight, p.Height, p.Width, p.Length, p.RetailPrice, ref quantity, ref totalVolume, ref totalWeight, ref totalValue, itemWeight, false);
                }
            }

            // This method use for get dimension of cube of total volume.             
            GetDimensionOfCubicalVolume(totalVolume, totalWeight, totalValue);
        }

        // Approximate dimensions by taking the cube root of the total volume.
        private void GetDimensionOfCubicalVolume(decimal totalVolume, decimal totalWeight, decimal totalValue)
        {
            // Approximate dimensions by taking the cube root of the total volume.
            var dimension = (decimal)Math.Pow(Convert.ToDouble(totalVolume), 1.0 / 3.0);
            dimension = Math.Round(dimension);
            Height = dimension;
            Length = dimension;
            Width = dimension;
            Weight = totalWeight;
            Value = totalValue; // Total product and addon value
        }

        // Estimates the total package size and weight for items in the shopping cart.
        private void EstimateShipmentPackageForShoppingCartItems()
        {
            // We are going to get a very approximate package size by taking the total volume of all items in
            // the cart and then getting the cube root of that volume. This will give us a minimum package size.
            // NOTE: This will underestimate the total package volume since packages will rarely be cubes!
            decimal totalVolume = 0;
            decimal totalWeight = 0;
            decimal totalValue = 0;

            foreach (ZnodeShoppingCartItem cartItem in ShoppingCartItems)
            {
                decimal quantity = cartItem.Quantity;
                decimal itemWeight = 0;
                EstimatePackageDimensions(cartItem.Product.Weight, cartItem.Product.Height, cartItem.Product.Width, cartItem.Product.Length, cartItem.TieredPricing, ref quantity, ref totalVolume, ref totalWeight, ref totalValue, itemWeight, cartItem.Product.FreeShippingInd);
            }

            // This method use for get dimension of cube of total volume.
            GetDimensionOfCubicalVolume(totalVolume, totalWeight, totalValue);

        }
    }
}
