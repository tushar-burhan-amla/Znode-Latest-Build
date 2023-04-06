using System;
using System.Collections.Generic;
using System.Configuration;
using Znode.Engine.Api.Models;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
using Znode.Engine.Shipping.Usps;
using Znode.Libraries.ECommerce.Entities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Shipping
{
    public class ZnodeShippingUsps : ZnodeShippingsType
    {
        public ZnodeShippingUsps()
        {
            Name = "USPS";
            Description = "Calculates shipping rates when using the United States Postal Service.";

            Controls.Add(ZnodeShippingControl.Profile);
            Controls.Add(ZnodeShippingControl.DisplayName);
            Controls.Add(ZnodeShippingControl.InternalCode);
            Controls.Add(ZnodeShippingControl.HandlingCharge);
            Controls.Add(ZnodeShippingControl.Countries);
        }

        // Calculates shipping rates when using the United States Postal Service.
        public override void Calculate()
        {
            decimal itemShippingRate = 0;

            // Instantiate the USPS agent
            UspsAgent usps = new UspsAgent();

            IZnodeShippingHelper shippingHelper = GetService<IZnodeShippingHelper>();
            PortalShippingModel portalShippingModel = shippingHelper.GetPortalShipping(Convert.ToInt32(ShoppingCart.PortalId),GetPublishStateId(ShoppingCart.PublishStateId));

            if (string.IsNullOrEmpty(portalShippingModel.USPSShippingAPIURL) || string.IsNullOrEmpty(portalShippingModel.USPSWebToolsUserID) || Equals(portalShippingModel.PackageWeightLimit, 0.0M))
            {
                ShoppingCart = shippingHelper.SetShippingErrorMessage(ShoppingCart);
                ShoppingCart.Shipping.ResponseMessage = GenericShippingErrorMessage();
            }

            string weightUnit = portalShippingModel.portalUnitModel.WeightUnit;

            // Get usps settings.
            usps = GetUspsSettings(usps, portalShippingModel);

            // Split the items that ship separately from the items that ship together
            SplitShipSeparatelyFromShipTogether();

            ShippingRateModel model = new ShippingRateModel();

            // Check weight unit            
            if (!string.IsNullOrEmpty(weightUnit))
                weightUnit = weightUnit.TrimEnd(new char[] { 'S' });

            //get package weight limit
            decimal PackageWeightLimit = Convert.ToDecimal(portalShippingModel.PackageWeightLimit);


            // Calculate ship rate items separately 
            if (ShipSeparatelyItems?.Count > 0)
            {
                model = CalculateShipSeparatelyItems(usps, PackageWeightLimit, weightUnit);
                foreach (ZnodeShoppingCartItem shoppingCartItem in ShoppingCart.ShoppingCartItems)
                {
                    foreach (ZnodeShoppingCartItem shipSeparatelyItems in ShipSeparatelyItems)
                    {
                        if (Equals(shoppingCartItem.Product.SKU, shipSeparatelyItems.Product.SKU))
                            shoppingCartItem.ShippingCost = shipSeparatelyItems.ShippingCost;

                        //For group product collection 
                        ShipSeparatelyForGroupAddonConfigureProduct(shoppingCartItem.Product.ZNodeGroupProductCollection, shipSeparatelyItems, shoppingCartItem);
                        //For Addon product collection
                        ShipSeparatelyForGroupAddonConfigureProduct(shoppingCartItem.Product.ZNodeAddonsProductCollection, shipSeparatelyItems, shoppingCartItem);
                        //For Bundle product collection
                        ShipSeparatelyForGroupAddonConfigureProduct(shoppingCartItem.Product.ZNodeBundleProductCollection, shipSeparatelyItems, shoppingCartItem);
                        //For Configure product collection
                        ShipSeparatelyForGroupAddonConfigureProduct(shoppingCartItem.Product.ZNodeConfigurableProductCollection, shipSeparatelyItems, shoppingCartItem);

                    }
                    ShoppingCart.OrderLevelShipping += shoppingCartItem.ShippingCost;

                }
            }
            
            //Order Level Handling charges.
            ShoppingCart.Shipping.ShippingHandlingCharge = shippingHelper.GetOrderLevelShippingHandlingCharge(ShippingBag, ShoppingCart);
            ShoppingCart.OrderLevelShipping = 0;
            ShoppingCart.ShippingHandlingCharges = ShoppingCart.Shipping.ShippingHandlingCharge;
        }

        private UspsAgent GetUspsSettings(UspsAgent usps, PortalShippingModel portalShippingModel)
        {
            // General settings
            usps.ShippingAddress = new AddressModel();
            IZnodeShippingHelper shippingHelper = GetService<IZnodeShippingHelper>();
            // Set portal default ware house address or origin address on flag basis.
            AddressModel portalWareHouseAddressModel = shippingHelper.GetPortalShippingAddress(portalShippingModel.PortalId);
            usps.OriginZipCode = portalWareHouseAddressModel.PostalCode;
            usps.PostageDeliveryUnitZip5 = ShoppingCart?.Payment?.ShippingAddress?.PostalCode;
            usps.ShippingAddress = ShoppingCart?.Payment?.ShippingAddress;
            usps.UspsShippingApiUrl = portalShippingModel.USPSShippingAPIURL;
            usps.UspsWebToolsUserId = portalShippingModel.USPSWebToolsUserID;
            usps.PackageWeightLimit = Convert.ToDecimal(portalShippingModel.PackageWeightLimit);
            usps.Country = shippingHelper.GetCountryByCountryCode(ShoppingCart?.Payment?.ShippingAddress?.CountryName);
            usps.ServiceType = ShippingBag?.ShippingCode;
            usps.PublishStateId = ShoppingCart.PublishStateId;
            return usps;
        }

        /// <summary>
        /// Checks the response code before the order is submitted.
        /// </summary>
        /// <returns>True if the response code is 0; otherwise, false.</returns>
        public override bool PreSubmitOrderProcess()
        {
            if (!Equals(ShoppingCart.Shipping.ResponseCode, "0"))
            {
                ShoppingCart.AddErrorMessage = GenericShippingErrorMessage();
                ZnodeLogging.LogActivity((int)ZnodeLogging.ErrorNum.GeneralError, "Shipping error in PreSubmitOrderProcess: " + ShoppingCart.Shipping.ResponseCode + " " + ShoppingCart.Shipping.ResponseMessage);
                return false;
            }
            return true;
        }

        // Calculate ship item separately.
        private ShippingRateModel CalculateShipSeparatelyItems(UspsAgent usps, decimal packageWeightLimitlbs, string weightUnit)
        {
            ShippingRateModel model = new ShippingRateModel();
            decimal itemShippingRate = 0.0m;
            // Shipping estimate for ship-separately packages
            if (ShipSeparatelyItems?.Count > 0)
            {
                foreach (ZnodeShoppingCartItem separateItem in ShipSeparatelyItems)
                {
                    ZnodeGenericCollection<ZnodeShoppingCartItem> singleItemlist = new ZnodeGenericCollection<ZnodeShoppingCartItem>();
                    singleItemlist.Add(separateItem);

                    ZnodeShippingPackage separateItemPackage = new ZnodeShippingPackage(singleItemlist, false, false);

                    usps.Sku = separateItem.Product.SKU;
                    usps.Container = separateItem.Product.Container;
                    usps.Size = separateItem.Product.Size;

                    //convert weight in LBS                    
                    if ((weightUnit?.ToUpper().Equals(WeightUnitKgs)).GetValueOrDefault())
                    {
                        WeightUnitBase = weightUnit;
                        separateItemPackage.Weight = ConvertWeightKgToLbs(separateItemPackage.Weight);
                        weightUnit = WeightUnitBase = WeightUnitLbs;
                    }

                    // Get weight in pounds.
                    usps.WeightInPounds = GetWeightInPounds(separateItemPackage);

                    // Get weight in ounces.
                    usps.WeightInOunces = GetWeightInOunces(separateItemPackage, usps);

                    // Calculate shipping rate.
                    model = GetSeparateItemShippingRate(usps, packageWeightLimitlbs, separateItem);
                    itemShippingRate = model.ShippingRate;



                    separateItem.ShippingCost = itemShippingRate;
                    // Logs activity.
                    LogsActivityShipSeparatelyItemsUsps(ShoppingCart, usps, separateItem);

                }
            }

            return model;
        }

        // Calculate ship item together.
        private ShippingRateModel CalculateShipTogetherItems(UspsAgent usps, decimal packageWeightLimitlbs, string weightUnit)
        {
            ShippingRateModel model = new ShippingRateModel();
            decimal itemShippingRate = 0.0m;
            // Shipping estimate for ship-together package
            if (ShipTogetherItems?.Count > 0)
            {
                var shipTogetherPackage = new ZnodeShippingPackage(ShipTogetherItems, false, true);

                usps.Sku = ShipTogetherItems[0].Product.SKU;

                usps.Container = ShipTogetherItems[0].Product.Container ?? ConfigurationManager.AppSettings["ShippingContainer"];
                usps.Size = ShipTogetherItems[0].Product.Size ?? ConfigurationManager.AppSettings["ShippingPackageSizeFromRequest"];
                usps.PackageHeight = ShipTogetherItems[0].Product.Height;
                usps.PackageLength = ShipTogetherItems[0].Product.Length;
                usps.PackageWidth = ShipTogetherItems[0].Product.Width;

                //convert weight in LBS               
                if ((weightUnit?.ToUpper().Equals(WeightUnitKgs)).GetValueOrDefault())
                {
                    WeightUnitBase = weightUnit;
                    shipTogetherPackage.Weight = ConvertWeightKgToLbs(shipTogetherPackage.Weight);
                    weightUnit = WeightUnitBase = WeightUnitLbs;
                }

                // Get weight in pounds.
                usps.WeightInPounds = GetWeightInPounds(shipTogetherPackage);

                // Get weight in ounces.
                usps.WeightInOunces = GetWeightInOunces(shipTogetherPackage, usps);

                // Calculate shipping rate.
                model = GetShipTogetherItemShippingRate(packageWeightLimitlbs, usps);
                itemShippingRate = model.ShippingRate;

                IZnodeShippingHelper shippingHelper = GetService<IZnodeShippingHelper>();

                // Calculate shipping handling charge.
                itemShippingRate = shippingHelper.GetShipTogetherItemsHandlingCharge(ShippingBag, itemShippingRate);

                // Logs activity for ship together items.
                LogsActivityShipTogetherItemsUsps(usps, ShoppingCart);

            }

            model.ShippingRate = itemShippingRate;
            return model;
        }

        // Get separate item shipping rate.
        private ShippingRateModel GetSeparateItemShippingRate(UspsAgent usps, decimal packageWeightLimitlbs, ZnodeShoppingCartItem separateItem)
        {
            ShippingRateModel model = new ShippingRateModel();
            decimal itemShippingRate = 0.0m;
            //Split Weight if Weight is greater than 70 lbs
            List<decimal> spWeight = new List<decimal>();

            if (Convert.ToDecimal(usps.WeightInOunces) > packageWeightLimitlbs)
                spWeight = usps.SplitPackageWeight(Convert.ToDecimal(usps.WeightInOunces), packageWeightLimitlbs);

            if (spWeight.Count > 0)
            {
                for (int listItemCount = 0; listItemCount < spWeight.Count; listItemCount++)
                {
                    usps.WeightInPounds = spWeight[listItemCount].ToString();
                    model = usps.CalculateShippingRate();
                    itemShippingRate = model.ShippingRate;

                    // Get shipping rate for each single item and multiply with quantity to total charges of order line item
                    itemShippingRate += itemShippingRate * separateItem.Quantity;
                }
            }
            else
            {
                model = usps.CalculateShippingRate();

                // Get shipping rate for each single item and multiply with quantity to total charges of order line item
                itemShippingRate += (model.ShippingRate * separateItem.Quantity);
            }

            model.ShippingRate = itemShippingRate;
            return model;
        }

        // Get ship together item shipping rate
        private ShippingRateModel GetShipTogetherItemShippingRate(decimal packageWeightLimitlbs, UspsAgent usps)
        {
            ShippingRateModel model = new ShippingRateModel();
            decimal itemShippingRate = 0.0m;
            //Split Weight if Weight is greater than 70lbs
            List<decimal> spWeight = new List<decimal>();

            if (Convert.ToDecimal(usps.WeightInPounds) > packageWeightLimitlbs)
                spWeight = usps.SplitPackageWeight(Convert.ToDecimal(usps.WeightInPounds), packageWeightLimitlbs);

            if (spWeight?.Count > 0)
            {
                for (int listItemCount = 0; listItemCount < spWeight.Count; listItemCount++)
                {
                    usps.WeightInPounds = spWeight[listItemCount].ToString();

                    // Get shipping rate for each item
                    model = usps.CalculateShippingRate();
                    itemShippingRate += model.ShippingRate;
                }
            }
            else
            {
                // Get shipping rate for each item
                model = usps.CalculateShippingRate();
                itemShippingRate += model.ShippingRate;
            }

            model.ShippingRate = itemShippingRate;
            return model;
        }

        // Get weight in ounces.
        private string GetWeightInOunces(ZnodeShippingPackage itemPackage, UspsAgent usps)
        {
            decimal remainder = itemPackage.Weight - Convert.ToDecimal(usps.WeightInPounds);
            return (remainder * 16).ToString();
        }

        // Get weight in pounds.
        private string GetWeightInPounds(ZnodeShippingPackage itemPackage)
        {
            string[] weightArray = itemPackage.Weight.ToString("N2").Split('.');
            return weightArray[0];
        }

        // Log activity for ship together items usps.
        private void LogsActivityShipTogetherItemsUsps(UspsAgent usps, ZnodeShoppingCart shoppingCart)
        {
            if (!Equals(usps.ErrorCode, "0"))
            {
                ShoppingCart.Shipping.ResponseCode = usps.ErrorCode;
                ShoppingCart.Shipping.ResponseMessage = GenericShippingErrorMessage();
                ShoppingCart.AddErrorMessage = GenericShippingErrorMessage();
                ZnodeLogging.LogActivity((int)ZnodeLogging.ErrorNum.GeneralError, "Shipping error: " + usps.ErrorCode + " " + usps.ErrorDescription);
            }
        }

        // Log activity for ship ship separately items usps.
        private void LogsActivityShipSeparatelyItemsUsps(ZnodeShoppingCart shoppingCart, UspsAgent usps, ZnodeShoppingCartItem separateItem)
        {
            if (!Equals(usps.ErrorCode, "0"))
            {
                ShoppingCart.Shipping.ResponseCode = usps.ErrorCode;
                ShoppingCart.Shipping.ResponseMessage = GenericShippingErrorMessage();
                ShoppingCart.AddErrorMessage = GenericShippingErrorMessage();
                ZnodeLogging.LogActivity((int)ZnodeLogging.ErrorNum.GeneralError,
                    $"Shipping error for {separateItem.Product.Name}: {usps.ErrorCode} {usps.ErrorDescription}");
                ZnodeLogging.LogActivity((int)ZnodeLogging.ErrorNum.GeneralError, $"Shipping error for {separateItem.Product.Name} : {usps.ErrorCode} {usps.ErrorDescription}");
            }
        }
    }
}
