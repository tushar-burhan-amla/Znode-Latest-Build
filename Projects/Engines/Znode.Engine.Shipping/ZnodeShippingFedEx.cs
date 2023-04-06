using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using Znode.Engine.Api.Models;
using Znode.Engine.Shipping.FedEx;
using Znode.Engine.Shipping.Helper;
using Znode.Libraries.Data;
using Znode.Libraries.ECommerce.Entities;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using Znode.Engine.Promotions;
using RateAvailableServiceWebServiceClient.RateServiceWebReference;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
namespace Znode.Engine.Shipping
{
    public class ZnodeShippingFedEx : ZnodeShippingsType
    {
        public ZnodeShippingFedEx()
        {
            Name = "FedEx";
            Description = "Calculates shipping rates when using FedEx.";

            Controls.Add(ZnodeShippingControl.Profile);
            Controls.Add(ZnodeShippingControl.ServiceCodes);
            Controls.Add(ZnodeShippingControl.HandlingCharge);
            Controls.Add(ZnodeShippingControl.Countries);
        }

        // Calculates shipping rates when using FedEx.
        public override void Calculate()
        {
            ZnodeShippingHelper shippingHelper = new ZnodeShippingHelper();
            decimal itemShippingRate = 0;
            // Instantiate FedEx agent
            FedExAgent fedEx = new FedExAgent();

            string packageType = string.Empty;

            PortalShippingModel portalShippingModel = GetFedExDetails(ref fedEx, out packageType);

            ShippingRateModel model = new ShippingRateModel();


            // Calculate ship separately item.
            if (ShipSeparatelyItems?.Count > 0)
            {
                model = CalculateShipSeparatelyItems(packageType, fedEx, portalShippingModel);
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


                    if (Equals(fedEx.ErrorCode, null))
                        LogsActivity(ShoppingCart);
                    else if (!fedEx.ErrorCode.Equals("0") && (ShipSeparatelyItems.Count > 0 || ShipTogetherItems.Count > 0))
                        LogsActivityShipSeparatelyItems(ShoppingCart, fedEx);
                    else
                    {
                        ShoppingCart.OrderLevelShipping += shoppingCartItem.ShippingCost;
                        ShoppingCart.ApproximateArrival = model.ApproximateArrival;
                    }
                }
            }

            ShoppingCart.Shipping.ShippingHandlingCharge = shippingHelper.GetOrderLevelShippingHandlingCharge(ShippingBag, ShoppingCart);
            ShoppingCart.OrderLevelShipping = 0;
            ShoppingCart.ShippingHandlingCharges = ShoppingCart.Shipping.ShippingHandlingCharge;
        }


        public override List<ShippingModel> GetEstimateRate(List<ZnodeShippingBag> shippingbagList)
        {
            FedExAgent fedex = new FedEx.FedExAgent();
            string packageType = string.Empty;

            PortalShippingModel portalShippingModel = GetFedExDetails(ref fedex, out packageType);

            RateRequest request = MapShippingDetails(fedex, portalShippingModel);

            foreach (ZnodeShoppingCartItem item in ShipSeparatelyItems)
            {
                ShipTogetherItems.Add(item);
            }
           
            List<ShippingModel> list = fedex.GetFedExEstimateRate(request, Convert.ToDecimal(portalShippingModel.PackageWeightLimit), ShipSeparatelyItems);
            if (ShipTogetherItems?.Count > 0)
            {
                bool isCalculatePromotionForShippingEstimates = ZnodeWebstoreSettings.IsCalculatePromotionForShippingEstimate;
                IZnodeShippingHelper shippingHelper = GetService<IZnodeShippingHelper>();
                foreach (ShippingModel model in list ?? new List<ShippingModel>())
                {
                    ZnodeShippingBag shippingBag = shippingbagList.Where(w => w.ShippingCode == model.ShippingCode)?.FirstOrDefault();
                    if (HelperUtility.IsNotNull(shippingBag))
                    {
                        model.ShippingRate = shippingHelper.GetShipTogetherItemsHandlingCharge(shippingBag, Convert.ToDecimal(model.ShippingRate));
                        shippingBag.ShoppingCart.OrderLevelShipping = model.ShippingRate.GetValueOrDefault();    
                        // Calculate shipping type promotion if isCalculatePromotionForShippingEstimates is true.
                        if (isCalculatePromotionForShippingEstimates && ShoppingCart.IsCalculatePromotionAndCoupon)
                            CalculatePromotionForShippingEstimate(shippingBag?.ShoppingCart);
                        // Calculate shipping handling charges.
                        model.ShippingHandlingCharge= shippingHelper.GetOrderLevelShippingHandlingCharge(ShippingBag, ShoppingCart);

                        if (shippingBag?.ShoppingCart?.Shipping?.ShippingDiscount > 0)
                        {
                            model.ShippingRateWithoutDiscount = model.ShippingRate;
                            model.ShippingRate = model.ShippingRate - shippingBag?.ShoppingCart?.Shipping?.ShippingDiscount;
                        }
                    }
                }
            }
            else
            {
                list.Select(x => x.ShippingRate = 0).ToList();
            }

            return list ?? new List<ShippingModel>();
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

        // Get fedex credentials.
        private FedExAgent GetFedExCredentials(PortalShippingModel portalShippingModel, FedExAgent fedEx)
        {
            // Decrypting FedEx account info
            var encrypt = new ZnodeEncryption();
            fedEx.FedExAccessKey = encrypt.DecryptData(portalShippingModel.FedExProductionKey);
            fedEx.FedExAccountNumber = encrypt.DecryptData(portalShippingModel.FedExAccountNumber);
            fedEx.FedExMeterNumber = encrypt.DecryptData(portalShippingModel.FedExMeterNumber);
            fedEx.FedExSecurityCode = encrypt.DecryptData(portalShippingModel.FedExSecurityCode);
            fedEx.FedExLTLAccountNumber = encrypt.DecryptData(portalShippingModel.FedExLTLAccountNumber);
            fedEx.PackageWeightLimit = Convert.ToDecimal(portalShippingModel.PackageWeightLimit);
            return fedEx;
        }

        // Calculate ship separately item.
        private ShippingRateModel CalculateShipSeparatelyItems(string packageType, FedExAgent fedEx, PortalShippingModel portalShippingModel)
        {
            ShippingRateModel model = new ShippingRateModel();
            decimal itemTotalValue = 0.0m;
            decimal itemShippingRate = 0.0m;
            // Shipping estimate for ship-separately packages
            if (ShipSeparatelyItems?.Count > 0)
            {
                foreach (ZnodeShoppingCartItem separateItem in ShipSeparatelyItems)
                {
                    var singleItemlist = new ZnodeGenericCollection<ZnodeShoppingCartItem>();
                    singleItemlist.Add(separateItem);

                    var separateItemPackage = new ZnodeShippingPackage(singleItemlist);

                    // Add insurance
                    itemTotalValue = separateItemPackage.Value;

                    if (Convert.ToBoolean(portalShippingModel.FedExAddInsurance))
                        fedEx.TotalInsuredValue = itemTotalValue;

                    // Add customs
                    fedEx.TotalCustomsValue = itemTotalValue;

                    fedEx.PackageHeight = Convert.ToString(separateItem.Product.Height);
                    fedEx.PackageWidth = Convert.ToString(separateItem.Product.Width);
                    fedEx.PackageLength = Convert.ToString(separateItem.Product.Length);
                    fedEx.PackageWeight = separateItem.Product.Weight;
                    fedEx.PackageGroupCount = Convert.ToString(separateItem.Quantity);
                    fedEx.PackageCount = "1";
                    fedEx.ItemPackageTypeCode = string.IsNullOrEmpty(separateItem.Product?.PackagingType) ? string.Empty : separateItem.Product?.PackagingType;

                    if (separateItem.Quantity > 0)
                    {
                        model = GetShipSeparatelyItemsShippingRate(fedEx, portalShippingModel);
                        itemShippingRate = model.ShippingRate;
                    }

                    separateItem.ShippingCost = itemShippingRate;
                }
            }

            return model;
        }

        // Calculate ship together item.
        private ShippingRateModel CalculateShipTogetherItems(string packageType, FedExAgent fedEx, PortalShippingModel portalShippingModel)
        {
            ShippingRateModel model = new ShippingRateModel();
            decimal itemShippingRate = 0.0m;

            // Shipping estimate for ship-together package
            if (ShipTogetherItems?.Count > 0)
            {
                var shipTogetherPackage = new ZnodeShippingPackage(ShipTogetherItems);

                model = GetShipToGetherItemRate(fedEx, portalShippingModel);
                itemShippingRate = model.ShippingRate;

                itemShippingRate = GetHandlingCharge(itemShippingRate);
            }
            model.ShippingRate = itemShippingRate;
            return model;
        }

        // Get ship separately item rate.
        private ShippingRateModel GetShipSeparatelyItemsShippingRate(FedExAgent fedEx, PortalShippingModel portalShippingModel)
        {
            ShippingRateModel model = new ShippingRateModel();
            decimal itemShippingRate = 0.0m;

            if (fedEx.WeightUnit.ToUpper().Equals(WeightUnitKgs))
            {
                WeightUnitBase = fedEx.WeightUnit;
                fedEx.PackageWeight = ConvertWeightKgToLbs(fedEx.PackageWeight);
                fedEx.WeightUnit = WeightUnitBase = WeightUnitLbs;
            }
            RateRequest request = MapShippingDetails(fedEx, portalShippingModel);
            model = fedEx.GetShippingRate(request, Convert.ToDecimal(portalShippingModel.PackageWeightLimit));
            itemShippingRate += model.ShippingRate;

            model.ShippingRate = itemShippingRate;
            return model;
        }

        // Get ship together item rate.
        private ShippingRateModel GetShipToGetherItemRate(FedExAgent fedEx, PortalShippingModel portalShippingModel)
        {
            ShippingRateModel model = new ShippingRateModel();
            decimal itemShippingRate = 0.0m;
            RateRequest request = MapShippingDetails(fedEx, portalShippingModel);

            model = fedEx.GetShippingRate(request, Convert.ToDecimal(portalShippingModel.PackageWeightLimit));
            itemShippingRate += model.ShippingRate;

            model.ShippingRate = itemShippingRate;
            return model;
        }

        // Get discount, weightunit,dimension and currency code.
        private FedExAgent GetFedExDiscountWeightUnitDimensionUnitCurrencyCode(PortalShippingModel portalShippingModel, FedExAgent fedEx)
        {
            bool fedExUseDiscountRate = Convert.ToBoolean(portalShippingModel.FedExUseDiscountRate);
            string weightUnit = portalShippingModel?.portalUnitModel?.WeightUnit;
            string dimensionUnit = portalShippingModel?.portalUnitModel?.DimensionUnit;

            // Check to apply FedEx discount rates
            if (fedExUseDiscountRate)
                fedEx.UseDiscountRate = fedExUseDiscountRate;

            // Check weight unit
            if (!string.IsNullOrEmpty(weightUnit))
                fedEx.WeightUnit = weightUnit.TrimEnd(new char[] { 'S' });

            // Check dimension unit
            if (!string.IsNullOrEmpty(dimensionUnit))
                fedEx.DimensionUnit = dimensionUnit;

            // Currency code
            fedEx.CurrencyCode = ZnodeCurrencyManager.CurrencyCode();

            return fedEx;
        }

        // Map package dimensions.
        private FedExAgent MapPackageDimension(ZnodeShippingPackage package, FedExAgent fedEx)
        {
            fedEx.PackageLength = Convert.ToInt32(package.Length).ToString();
            fedEx.PackageHeight = Convert.ToInt32(package.Height).ToString();
            fedEx.PackageWidth = Convert.ToInt32(package.Width).ToString();

            return fedEx;
        }

        // Map shipping destination address.
        private FedExAgent MapShippingDestinationToFedEx(ZnodeShoppingCart shoppingCart, FedExAgent fedEx)
        {
            IZnodeRepository<Znode.Libraries.Data.DataModel.ZnodeShipping> _shippingRepository = new ZnodeRepository<Znode.Libraries.Data.DataModel.ZnodeShipping>();
            IZnodeShippingHelper shippingHelper = GetService<IZnodeShippingHelper>();
            if (HelperUtility.IsNotNull(ShoppingCart?.Payment?.ShippingAddress))
            {
                fedEx.ShipToAddress1 = ShoppingCart.Payment.ShippingAddress.Address1;
                fedEx.ShipToAddress2 = ShoppingCart.Payment.ShippingAddress.Address2;
                fedEx.ShipToCity = ShoppingCart.Payment.ShippingAddress.CityName;
                fedEx.ShipToZipCode = ShoppingCart.Payment.ShippingAddress.PostalCode;
                fedEx.ShipToStateCode = shippingHelper.GetStateCode(ShoppingCart.Payment.ShippingAddress.StateName);
                fedEx.ShipToCountryCode = HelperUtility.IsNotNull(ShoppingCart.Payment.ShippingAddress.CountryName) ? ShoppingCart.Payment.ShippingAddress.CountryName : _shippingRepository.Table.FirstOrDefault(x => x.ShippingId == shoppingCart.Shipping.ShippingID)?.DestinationCountryCode;
            }
            return fedEx;
        }

        // Map shipping origin address.
        private FedExAgent MapShippingOriginToFedEx(PortalShippingModel portalShippingModel, FedExAgent fedEx)
        {
            IZnodeShippingHelper shippingHelper = GetService<IZnodeShippingHelper>();

            // Service type
            fedEx.FedExServiceType = ShippingBag.ShippingCode;
            fedEx.PackageTypeCode = portalShippingModel.FedExPackagingType;
            fedEx.DropOffType = portalShippingModel.FedExDropoffType;
            // Set portal default ware house address or origin address on flag basis.
            AddressModel portalWareHouseAddressModel = shippingHelper.GetPortalShippingAddress(portalShippingModel.PortalId);
            // Shipping origin
            fedEx.ShipperAddress1 = portalWareHouseAddressModel.Address1;
            fedEx.ShipperAddress2 = portalWareHouseAddressModel.Address2;
            fedEx.ShipperCity = portalWareHouseAddressModel.CityName;
            fedEx.ShipperZipCode = portalWareHouseAddressModel.PostalCode;
            fedEx.ShipperStateCode = shippingHelper.GetStateCode(string.IsNullOrEmpty(portalWareHouseAddressModel.StateName) ? portalWareHouseAddressModel.StateCode : portalWareHouseAddressModel.StateName);
            fedEx.ShipperCountryCode = portalWareHouseAddressModel.CountryName;

            return fedEx;
        }

        // Los activity for ship separately item.
        private void LogsActivityShipSeparatelyItems(ZnodeShoppingCart shoppingCart, FedExAgent fedEx)
        {
            ShoppingCart.Shipping.ResponseCode = fedEx.ErrorCode;
            ShoppingCart.Shipping.ResponseMessage = GenericShippingErrorMessage();
            ShoppingCart.AddErrorMessage = GenericShippingErrorMessage();
            ZnodeLogging.LogActivity((int)ZnodeLogging.ErrorNum.GeneralError, "Shipping error: " + fedEx.ErrorCode + " " + fedEx.ErrorDescription);
        }

        // Log activity.
        private void LogsActivity(ZnodeShoppingCart shoppingCart)
        {
            ShoppingCart.Shipping.ResponseCode = "-1";
            ShoppingCart.AddErrorMessage = "Shipping error: Invalid option selected.";
            ShoppingCart.Shipping.ResponseMessage = GenericShippingErrorMessage();
        }

        private RateRequest SetPackageLineItem(FedExAgent fedex, ZnodeGenericCollection<ZnodeShoppingCartItem> shipTogetherItems, bool isInsurance)
        {
            RateRequest request = new RateRequest();
            int count = 0;
            int packageLineItem = 1;
            int packageCount = 0;
            decimal packagePrice = 0.0M;
            request.RequestedShipment = new RequestedShipment();
            request.RequestedShipment.RequestedPackageLineItems = new RequestedPackageLineItem[ShipTogetherItems.Count];
            foreach (ZnodeShoppingCartItem cartItem in ShipTogetherItems)
            {
                if (cartItem.Quantity > 0)
                {
                    request.RequestedShipment.RequestedPackageLineItems[count] = new RequestedPackageLineItem();

                    // Set the package sequence number
                    request.RequestedShipment.RequestedPackageLineItems[count].SequenceNumber = packageLineItem.ToString();
                    request.RequestedShipment.RequestedPackageLineItems[count].GroupPackageCount = Convert.ToString((int)cartItem.Quantity);
                    decimal totalWeight = cartItem.Quantity * cartItem.Product.Weight;

                    //Set Packaging Type
                    request.RequestedShipment.RequestedPackageLineItems[count].PhysicalPackaging = string.IsNullOrEmpty(cartItem.Product?.PackagingType) ? PhysicalPackagingType.BOX : (PhysicalPackagingType)Enum.Parse(typeof(PhysicalPackagingType), cartItem.Product?.PackagingType, true);
                    request.RequestedShipment.RequestedPackageLineItems[count].PhysicalPackagingSpecified = true;

                    // Set the package weight 
                    request.RequestedShipment.RequestedPackageLineItems[count].Weight = new Weight();
                    request.RequestedShipment.RequestedPackageLineItems[count].Weight.Value = totalWeight;
                    request.RequestedShipment.RequestedPackageLineItems[count].Weight.ValueSpecified = true;
                    request.RequestedShipment.RequestedPackageLineItems[count].Weight.Units = (WeightUnits)Enum.Parse(typeof(WeightUnits), fedex.WeightUnit);
                    request.RequestedShipment.RequestedPackageLineItems[count].Weight.UnitsSpecified = true;

                    // Set the package dimensions
                    request.RequestedShipment.RequestedPackageLineItems[count].Dimensions = new Dimensions();
                    request.RequestedShipment.RequestedPackageLineItems[count].Dimensions.Length = Convert.ToString((int)cartItem.Product.Length);
                    request.RequestedShipment.RequestedPackageLineItems[count].Dimensions.Width = Convert.ToString((int)cartItem.Product.Width);
                    request.RequestedShipment.RequestedPackageLineItems[count].Dimensions.Height = Convert.ToString((int)cartItem.Product.Height);
                    request.RequestedShipment.RequestedPackageLineItems[count].Dimensions.Units = (LinearUnits)Enum.Parse(typeof(LinearUnits), fedex.DimensionUnit);
                    request.RequestedShipment.RequestedPackageLineItems[count].Dimensions.UnitsSpecified = true;
                    count++;
                    packageLineItem++;

                    packageCount = packageCount + (int)cartItem.Quantity;
                    packagePrice = packagePrice + cartItem.Product.FinalPrice;
                }
            }

            if (isInsurance)
                fedex.TotalInsuredValue = packagePrice;

            request.RequestedShipment.PackageCount = packageCount.ToString();
            return request;
        }

        private RateRequest SetFrightPackageLineItem(FedExAgent fedex, ZnodeGenericCollection<ZnodeShoppingCartItem> shipTogetherItems, bool isInsurance)
        {
            RateRequest request = new RateRequest();
            int count = 0;
            int packageLineItem = 1;
            int packageCount = 0;
            request.RequestedShipment = new RequestedShipment();
            request.RequestedShipment.FreightShipmentDetail = new FreightShipmentDetail();
            request.RequestedShipment.FreightShipmentDetail.LineItems = new FreightShipmentLineItem[ShipTogetherItems.Count];
            foreach (ZnodeShoppingCartItem cartItem in ShipTogetherItems)
            {
                if (cartItem.Quantity > 0)
                {
                    request.RequestedShipment.FreightShipmentDetail.LineItems[count] = new FreightShipmentLineItem();

                    // Set the package sequence number
                    request.RequestedShipment.FreightShipmentDetail.LineItems[count].FreightClass = FreightClassType.CLASS_100;
                    request.RequestedShipment.FreightShipmentDetail.LineItems[count].FreightClassSpecified = true;

                    //Set Packaging Type
                    request.RequestedShipment.FreightShipmentDetail.LineItems[count].Packaging = string.IsNullOrEmpty(cartItem.Product?.PackagingType) ? PhysicalPackagingType.BOX : (PhysicalPackagingType)Enum.Parse(typeof(PhysicalPackagingType), cartItem.Product?.PackagingType, true);
                    request.RequestedShipment.FreightShipmentDetail.LineItems[count].PackagingSpecified = true;
                    request.RequestedShipment.FreightShipmentDetail.LineItems[count].Description = "Freight line item description";

                    // Set the package weight 
                    request.RequestedShipment.FreightShipmentDetail.LineItems[count].Weight = new Weight();
                    request.RequestedShipment.FreightShipmentDetail.LineItems[count].Weight.Value = (decimal)(cartItem.Product.Weight * cartItem.Quantity);
                    request.RequestedShipment.FreightShipmentDetail.LineItems[count].Weight.ValueSpecified = true;
                    request.RequestedShipment.FreightShipmentDetail.LineItems[count].Weight.Units = (WeightUnits)Enum.Parse(typeof(WeightUnits), fedex.WeightUnit);
                    request.RequestedShipment.FreightShipmentDetail.LineItems[count].Weight.UnitsSpecified = true;

                    // Set the package dimensions
                    request.RequestedShipment.FreightShipmentDetail.LineItems[count].Dimensions = new Dimensions();
                    request.RequestedShipment.FreightShipmentDetail.LineItems[count].Dimensions.Length = Convert.ToString((int)cartItem.Product.Length);
                    request.RequestedShipment.FreightShipmentDetail.LineItems[count].Dimensions.Width = Convert.ToString((int)cartItem.Product.Width);
                    request.RequestedShipment.FreightShipmentDetail.LineItems[count].Dimensions.Height = Convert.ToString((int)cartItem.Product.Height);
                    request.RequestedShipment.FreightShipmentDetail.LineItems[count].Dimensions.Units = (LinearUnits)Enum.Parse(typeof(LinearUnits), fedex.DimensionUnit);
                    request.RequestedShipment.FreightShipmentDetail.LineItems[count].Dimensions.UnitsSpecified = true;
                    count++;
                    packageLineItem++;

                    packageCount = packageCount + (int)cartItem.Quantity;
                }
            }

            request.RequestedShipment.FreightShipmentDetail.TotalHandlingUnits = packageCount.ToString();
            request.RequestedShipment.PackageCount = count.ToString();
            return request;
        }

        private PortalShippingModel GetFedExDetails(ref FedExAgent fedEx, out string packageType)
        {
            IZnodeShippingHelper shippingHelper = GetService<IZnodeShippingHelper>();
            PortalShippingModel portalShippingModel = shippingHelper.GetPortalShipping(Convert.ToInt32(ShoppingCart.PortalId),GetPublishStateId(ShoppingCart.PublishStateId));

            packageType = portalShippingModel.FedExPackagingType;

            if (string.IsNullOrEmpty(portalShippingModel.FedExAccountNumber)
                || string.IsNullOrEmpty(portalShippingModel.FedExMeterNumber)
                || string.IsNullOrEmpty(portalShippingModel.FedExProductionKey)
                || string.IsNullOrEmpty(portalShippingModel.FedExMeterNumber)
                 || string.IsNullOrEmpty(portalShippingModel.FedExSecurityCode)
                || Equals(portalShippingModel.PackageWeightLimit, 0.0M) || (fedEx.IsFreightServiceType(ShippingBag.ShippingCode) && string.IsNullOrEmpty(portalShippingModel.FedExLTLAccountNumber))
                )
            {
                ShoppingCart = shippingHelper.SetShippingErrorMessage(ShoppingCart);
                ShoppingCart.Shipping.ResponseMessage = GenericShippingErrorMessage();
            }

            try
            {
                // Get fedex credentials.
                fedEx = GetFedExCredentials(portalShippingModel, fedEx);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("There was an error decrypting FedEx account information.", "Shipping", TraceLevel.Error, ex);
            }

            // Shipping origin properties.
            fedEx = MapShippingOriginToFedEx(portalShippingModel, fedEx);

            // Destination properties.
            fedEx = MapShippingDestinationToFedEx(ShoppingCart, fedEx);


            // Ship-to residence
            fedEx.ShipToAddressIsResidential = ShoppingCart?.Payment?.ShippingAddress?.AccountId > 0;

            fedEx = GetFedExDiscountWeightUnitDimensionUnitCurrencyCode(portalShippingModel, fedEx);

            // Split the items that ship separately from the items that ship together
            SplitShipSeparatelyFromShipTogether();
            return portalShippingModel;
        }

        private RateRequest MapShippingDetails(FedExAgent fedEx, PortalShippingModel portalShippingModel)
         {
            var shipTogetherPackage = new ZnodeShippingPackage(ShipTogetherItems);

            //Split Weight if Weight is greater than 150 for fedEx
            List<decimal> splWeight = new List<decimal>();
            if (fedEx.WeightUnit.ToUpper().Equals(WeightUnitKgs))
            {
                WeightUnitBase = fedEx.WeightUnit;
                fedEx.PackageWeight = ConvertWeightKgToLbs(fedEx.PackageWeight);
                fedEx.WeightUnit = WeightUnitBase = WeightUnitLbs;
            }

            RateRequest request = new RateRequest();
            if (fedEx.IsFreightServiceType(fedEx.FedExServiceType))
                request = SetFrightPackageLineItem(fedEx, ShipTogetherItems, portalShippingModel.FedExAddInsurance.GetValueOrDefault(false));
            else
                request = SetPackageLineItem(fedEx, ShipTogetherItems, portalShippingModel.FedExAddInsurance.GetValueOrDefault(false));
            return request;
        }

        private decimal GetHandlingCharge(decimal itemShippingRate)
        {
            IZnodeShippingHelper shippingHelper = GetService<IZnodeShippingHelper>();

            itemShippingRate = shippingHelper.GetShipTogetherItemsHandlingCharge(ShippingBag, itemShippingRate);
            return itemShippingRate;
        }

        // Calculate shipping type promotion if isCalculatePromotionForShippingEstimates is true.
        protected virtual void CalculatePromotionForShippingEstimate(ZnodeShoppingCart znodeShoppingCart)
        {
            ZnodeLogging.LogMessage("Execution Started:", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);
            IZnodeCartPromotionManager cartPromoManager = GetService<IZnodeCartPromotionManager>(new ZnodeNamedParameter("shoppingCart", znodeShoppingCart), new ZnodeNamedParameter("profileId", znodeShoppingCart.ProfileId));

            if (cartPromoManager.CartPromotionCache.Any(x => x.PromotionType.ClassName == ZnodeConstant.PercentOffShipping
            || x.PromotionType.ClassName == ZnodeConstant.PercentOffShippingWithCarrier || x.PromotionType.ClassName == ZnodeConstant.AmountOffShipping
            || x.PromotionType.ClassName == ZnodeConstant.AmountOffShippingWithCarrier))
                cartPromoManager.Calculate();
            ZnodeLogging.LogMessage("Execution Ended:", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);

        }
    }
}
