using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using Znode.Engine.Api.Models;
using Znode.Engine.Promotions;
using Znode.Engine.Shipping.Ups;
using Znode.Libraries.ECommerce.Entities;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
namespace Znode.Engine.Shipping
{
    public class ZnodeShippingUps : ZnodeShippingsType, IZnodeShippingUps
    {
        public ZnodeShippingUps()
        {
            Name = "UPS";
            Description = "Calculates shipping rates when using UPS.";

            Controls.Add(ZnodeShippingControl.Profile);
            Controls.Add(ZnodeShippingControl.ServiceCodes);
            Controls.Add(ZnodeShippingControl.HandlingCharge);
        }

        // Calculates shipping rates when using UPS.
        public override void Calculate()
        {
            decimal itemShippingRate = 0;
            IZnodeShippingHelper shippingHelper = GetService<IZnodeShippingHelper>();
            PortalShippingModel portalShippingModel = shippingHelper.GetPortalShipping(Convert.ToInt32(ShoppingCart.PortalId),GetPublishStateId(ShoppingCart.PublishStateId));
            // Instantiate UPS agent
            UpsAgent ups = new UpsAgent();
            try
            {
                GetUPSCredentialsSetting(portalShippingModel, ups);
            }
            catch (Exception ex)
            {
                ZnodeLogging.LogMessage("There was an error decrypting UPS account information.", "Shipping", TraceLevel.Error, ex);
            }

            bool isUPSFreight = IsUPSFreight();


            if (string.IsNullOrEmpty(portalShippingModel.UpsUsername)
              || string.IsNullOrEmpty(portalShippingModel.UpsPassword)
              || string.IsNullOrEmpty(portalShippingModel.UpsKey)
              || Equals(portalShippingModel.PackageWeightLimit, 0.0M) || (isUPSFreight && (string.IsNullOrEmpty(portalShippingModel.LTLUpsUserName)
                               || string.IsNullOrEmpty(portalShippingModel.LTLUpsPassword)
                               || string.IsNullOrEmpty(portalShippingModel.LTLUpsAccessLicenseNumber)
                               || string.IsNullOrEmpty(portalShippingModel.LTLUpsAccountNumber))))
            {

                ShoppingCart = shippingHelper.SetShippingErrorMessage(ShoppingCart);
                ShoppingCart.Shipping.ResponseMessage = GenericShippingErrorMessage();
            }


            ups = isUPSFreight ? GeneralSettingForLTL(portalShippingModel, ups) : GeneralSetting(portalShippingModel, ups);
            HandleSpecialShippingAddress(ups);
            // Split the items that ship separately from the items that ship together
            SplitShipSeparatelyFromShipTogether();

            ShippingRateModel model = new ShippingRateModel();


            // Calculate ship items seperately.
            if (ShipSeparatelyItems?.Count > 0)
            {
                model = CalculateShipSeparatelyItems(ShipSeparatelyItems, ups);
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

            ShoppingCart.ApproximateArrival = model.ApproximateArrival;
            ShoppingCart.Shipping.ShippingHandlingCharge = shippingHelper.GetOrderLevelShippingHandlingCharge(ShippingBag, ShoppingCart);
            ShoppingCart.OrderLevelShipping = 0;
            ShoppingCart.ShippingHandlingCharges = ShoppingCart.Shipping.ShippingHandlingCharge;
        }

        public virtual void GetUPSCredentialsSetting(PortalShippingModel portalShippingModel, UpsAgent ups)
        {
            ZnodeEncryption encrypt = new ZnodeEncryption();

            ups.UpsUserId = encrypt.DecryptData(portalShippingModel.UpsUsername);
            ups.UpsPassword = encrypt.DecryptData(portalShippingModel.UpsPassword);
            ups.UpsKey = encrypt.DecryptData(portalShippingModel.UpsKey);
            ups.LTLUpsUserName = encrypt.DecryptData(portalShippingModel.LTLUpsUserName);
            ups.LTLUpsPassword = encrypt.DecryptData(portalShippingModel.LTLUpsPassword);
            ups.LTLUpsAccessLicenseNumber = encrypt.DecryptData(portalShippingModel.LTLUpsAccessLicenseNumber);
            ups.LTLUpsAccountNumber = encrypt.DecryptData(portalShippingModel.LTLUpsAccountNumber);
            ups.PackageWeightLimit = Convert.ToDecimal(portalShippingModel.PackageWeightLimit);
            ups.PackageTypeCode = portalShippingModel.UPSPackagingType;
            ups.PickupType = portalShippingModel.UPSDropoffType;
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

        // General setting for ups.
        protected virtual UpsAgent GeneralSetting(PortalShippingModel portalShippingModel, UpsAgent ups)
        {
            // General settings  
            IZnodeShippingHelper shippingHelper = GetService<IZnodeShippingHelper>();
            // Set portal default ware house address or origin address on flag basis.
            AddressModel portalWareHouseAddressModel = shippingHelper.GetPortalShippingAddress(portalShippingModel.PortalId);
            ups.ShipperZipCode = portalWareHouseAddressModel.PostalCode;
            ups.ShipperCountryCode = portalWareHouseAddressModel.CountryName;
            ups.ShipperCityName = portalWareHouseAddressModel.CityName;
            if (HelperUtility.IsNotNull(ShoppingCart.Payment.ShippingAddress))
            {
                ups.ShipToZipCode = ShoppingCart.Payment.ShippingAddress.PostalCode;
                ups.ShipToCountryCode = ShoppingCart.Payment.ShippingAddress.CountryName;
                ups.ShipToCity = ShoppingCart.Payment.ShippingAddress.CityName;
                ups.ShipStateProvinceCode = shippingHelper.GetStateCode(ShoppingCart.Payment.ShippingAddress.StateName);
            }

            ups.UpsServiceCode = ShippingBag.ShippingCode;
            ups.ShipToAddressType = "Commercial";

            string weightUnit = portalShippingModel?.portalUnitModel?.WeightUnit;
            if (!string.IsNullOrEmpty(weightUnit))
                ups.WeightUnit = weightUnit.ToUpper().Equals("KGS") ? weightUnit.TrimEnd(new char[] { 'S' }) : weightUnit;

            return ups;
        }

        // Calculate ship together items.
        protected virtual ShippingRateModel CalculateShipTogetherItems(UpsAgent ups)
        {
            ShippingRateModel model = new ShippingRateModel();
            decimal itemShippingRate = 0.0m;
            // Shipping estimate for ship-together package
            if (ShipTogetherItems.Count > 0)
            {

                // Map ship demenssion
                ups = MapShipItemsDimensions(ups);

                // Calculate shiping rate together.
                model = GetItemShippingRateShipTogether(ups);
                itemShippingRate = model.ShippingRate;

                IZnodeShippingHelper shippingHelper = GetService<IZnodeShippingHelper>();

                // Calculate ship together handling charge.
                itemShippingRate = shippingHelper.GetShipTogetherItemsHandlingCharge(ShippingBag, itemShippingRate);

                // Logs activity for ship together items.
                LogsShipTogetherItems(ShoppingCart, ups);
            }

            model.ShippingRate = itemShippingRate;
            return model;
        }

        // Calculate ship rate item for ship together.
        protected virtual ShippingRateModel GetItemShippingRateShipTogether(UpsAgent ups)
        {
            ShippingRateModel model = new ShippingRateModel();
            decimal itemShippingRate = 0.0m;
            bool isUPSFreight = IsUPSFreight();

            if (isUPSFreight)
            {
                model = ups.GetLTLShippingRate();
            }
            else if (ups.PackageWeight > 150)
            {
                decimal packageWeightCount = Math.Ceiling(ups.PackageWeight / ups.PackageWeightLimit);
                decimal packageWeight = Math.Ceiling(ups.PackageWeight / packageWeightCount);
                for (int i = 0; i < packageWeightCount; i++)
                {
                    ups.PackageWeight = packageWeight;
                    model = ups.GetShippingRate();
                    itemShippingRate += model.ShippingRate;
                }
            }
            else
            {

                model = ups.GetShippingRate();
                itemShippingRate += model.ShippingRate;
            }

            model.ShippingRate = itemShippingRate;
            return model;
        }

        // Calculate ship separately items.
        protected virtual ShippingRateModel CalculateShipSeparatelyItems(ZnodeGenericCollection<ZnodeShoppingCartItem> shipSeparatelyItems, UpsAgent ups)
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

                    SetWeightUnit(ups);

                    // Calculate shipping item separately.
                    model = GetItemShippingRateSeparately(ups, separateItem);
                    itemShippingRate = model.ShippingRate;


                    separateItem.ShippingCost = itemShippingRate;
                    // Logs activity for ship separately item.
                    LogsShipSeparatelyItems(ShoppingCart, ups, separateItem);

                }
            }

            return model;
        }

        // Calculate shipping rate separately.
        protected virtual ShippingRateModel GetItemShippingRateSeparately(UpsAgent ups, ZnodeShoppingCartItem separateItem)
        {
            ShippingRateModel model = new ShippingRateModel();
            decimal itemShippingRate = 0.0m;
            bool isUPSFreight = IsUPSFreight();
           
            ups.PackageWeight = (separateItem.Product.Weight * separateItem.Quantity);
            ups.PackageHeight = separateItem.Product.Height;
            ups.PackageWidth = separateItem.Product.Width;
            ups.PackageLength = separateItem.Product.Length;

            model = isUPSFreight ? ups.GetLTLShippingRate() : ups.GetShippingRate();
            // Get shipping rate for each single item and multiply with quantity to total charges of order line item
            itemShippingRate += (model.ShippingRate);

            return model;
        }

        // Map ship items dimensions.
        protected virtual UpsAgent MapShipItemsDimensions(UpsAgent ups)
        {
            decimal totalWeight = 0M;
            foreach (ZnodeShoppingCartItem cartItem in ShipTogetherItems)
            {
                totalWeight = totalWeight + (cartItem.Product.Weight * cartItem.Quantity);
                ups.PackageHeight = cartItem.Product.Height;
                ups.PackageWidth = cartItem.Product.Width;
                ups.PackageWeight = cartItem.Product.Weight;
                ups.PackageLength = cartItem.Product.Length;
            }
            ups.PackageWeight = totalWeight;
            SetWeightUnit(ups);
            return ups;
        }

        protected virtual void SetWeightUnit(UpsAgent ups)
        {
            //convert weight in LBS
            if (ups.WeightUnit.ToUpper().Equals(WeightUnitKgs))
            {
                WeightUnitBase = ups.WeightUnit;
                ups.PackageWeight = ConvertWeightKgToLbs(ups.PackageWeight);
                ups.WeightUnit = "LBS";
                WeightUnitBase = WeightUnitLbs;
            }
        }

        // Logs activity ship together items.
        protected virtual void LogsShipTogetherItems(ZnodeShoppingCart shoppingCart, UpsAgent ups)
        {
            if (!Equals(ups.ErrorCode, "0") && !string.IsNullOrEmpty(ups.ErrorCode))
            {
                ShoppingCart.Shipping.ResponseCode = ups.ErrorCode;
                ShoppingCart.Shipping.ResponseMessage = GenericShippingErrorMessage();
                ShoppingCart.AddErrorMessage = GenericShippingErrorMessage();
                ZnodeLogging.LogActivity((int)ZnodeLogging.ErrorNum.GeneralError, "Shipping error: " + ups.ErrorCode + " " + ups.ErrorDescription);
            }
        }

        // Logs activity ship separately items.
        protected virtual void LogsShipSeparatelyItems(ZnodeShoppingCart shoppingCart, UpsAgent ups, ZnodeShoppingCartItem separateItem)
        {
            if (!Equals(ups.ErrorCode, null) && !Equals(ups.ErrorCode, "0"))
            {
                ShoppingCart.Shipping.ResponseCode = ups.ErrorCode;
                ShoppingCart.AddErrorMessage = GenericShippingErrorMessage();
                ZnodeLogging.LogActivity((int)ZnodeLogging.ErrorNum.GeneralError,
                    $"Shipping error for {separateItem.Product.Name}: {ups.ErrorCode} {ups.ErrorDescription}");
                ZnodeLogging.LogActivity((int)ZnodeLogging.ErrorNum.GeneralError, $"Shipping error for {separateItem.Product.Name} : {ups.ErrorCode} {ups.ErrorDescription}");
            }
        }

        // Check for LTL shipping.
        protected virtual bool IsUPSFreight()
        {
            IZnodeShippingHelper shippingHelper = GetService<IZnodeShippingHelper>();
            List<string> serviceType = new List<string>() { shippingHelper.GetUPSLTLServiceCodeEnumValue(Convert.ToString(UPSLTLServiceCode.FreightLTL)), shippingHelper.GetUPSLTLServiceCodeEnumValue(Convert.ToString(UPSLTLServiceCode.FreightLTLGuaranteed)), shippingHelper.GetUPSLTLServiceCodeEnumValue(Convert.ToString(UPSLTLServiceCode.FreightLTLUrgent)) };
            return serviceType.Contains(ShippingBag.ShippingCode);
        }

        // Get setting for UPS LTL.
        protected virtual UpsAgent GeneralSettingForLTL(PortalShippingModel portalShippingModel, UpsAgent ups)
        {
            // General settings  
            IZnodeShippingHelper shippingHelper = GetService<IZnodeShippingHelper>();

            // Set portal default ware house address or origin address on flag basis.
            AddressModel portalWareHouseAddressModel = shippingHelper.GetPortalShippingAddress(portalShippingModel.PortalId);

            // Below field get vary or change(customize) as per customer requirement.
            ups.LTLRequestOption = new string[] { "1" };

            ups.LTLShipFromAddressLines = new string[] { portalWareHouseAddressModel.Address1, portalWareHouseAddressModel.Address2 };
            ups.LTLShipFromCity = portalWareHouseAddressModel.CityName;
            ups.LTLStateProvinceCode = shippingHelper.GetStateCode(string.IsNullOrEmpty(portalWareHouseAddressModel.StateName) ? portalWareHouseAddressModel.StateCode : portalWareHouseAddressModel.StateName);
            ups.ShipperZipCode = portalWareHouseAddressModel.PostalCode;
            ups.LTLCountryCode = portalWareHouseAddressModel.CountryName;
            ups.LTLShipFromName = string.IsNullOrEmpty(portalWareHouseAddressModel.WarehouseName) ? portalWareHouseAddressModel.CityName : portalWareHouseAddressModel.WarehouseName;
            ups.LTLShipFromPhoneNumber = portalWareHouseAddressModel.PhoneNumber;

            if (HelperUtility.IsNotNull(ShoppingCart?.Payment?.ShippingAddress))
            {
                ups.LTLShipToAddressLines = new string[] { ShoppingCart.Payment.ShippingAddress.Address1, ShoppingCart.Payment.ShippingAddress.Address2 };
                ups.ShipToCity = ShoppingCart.Payment.ShippingAddress.CityName;
                ups.ShipStateProvinceCode = shippingHelper.GetStateCode(ShoppingCart.Payment.ShippingAddress.StateName);
                ups.ShipToZipCode = ShoppingCart.Payment.ShippingAddress.PostalCode;
                ups.ShipToCountryCode = ShoppingCart.Payment.ShippingAddress.CountryName;
                ups.LTLShipToName = (!string.IsNullOrEmpty(ShoppingCart.Payment.ShippingAddress.FirstName) ? ShoppingCart.Payment.ShippingAddress.FirstName : "") + " " + (!string.IsNullOrEmpty(ShoppingCart.Payment.ShippingAddress.LastName) ? ShoppingCart.Payment.ShippingAddress.LastName : "");
                ups.LTLShipToPhoneNumber = ShoppingCart.Payment.ShippingAddress.PhoneNumber;
            }

            if (HelperUtility.IsNotNull(ShoppingCart?.Payment?.BillingAddress))
            {
                ups.LTLPayerName = (!string.IsNullOrEmpty(ShoppingCart.Payment.BillingAddress.FirstName) ? ShoppingCart.Payment.BillingAddress.FirstName : "") + " " + (!string.IsNullOrEmpty(ShoppingCart.Payment.BillingAddress.LastName) ? ShoppingCart.Payment.BillingAddress.LastName : "");
                ups.LTLPayerPhoneNumber = ShoppingCart.Payment.BillingAddress.PhoneNumber;
                ups.LTLPayerAddressLines = new string[] { ShoppingCart.Payment.BillingAddress.Address1, ShoppingCart.Payment.BillingAddress.Address2 };
                ups.LTLPayerCity = ShoppingCart.Payment.BillingAddress.CityName;
                ups.LTLPayerStateProvinceCode = shippingHelper.GetStateCode(ShoppingCart.Payment.BillingAddress.StateName);
                ups.LTLPayerPostalCode = ShoppingCart.Payment.BillingAddress.PostalCode;
                ups.LTLPayerCountryCode = ShoppingCart.Payment.BillingAddress.CountryName;
            }

            // Below option will be customize as per requirement.
            ups.LTLPayerUPSAccountNumber = !string.IsNullOrEmpty(ups.LTLUpsAccountNumber) ? ups.LTLUpsAccountNumber : "25Y552";

            // Below option will be customize as per requirement.
            ups.LTLShipBillOptionCode = "10";
            ups.LTLShipBillOptionDescription = "PREPAID";

            ups.UpsServiceCode = ShippingBag.ShippingCode;

            // Below option will be customize as per requirement.
            ups.LTLCommodityNumberOfPieces = "20";

            // Below option will be customize as per requirement.
            ups.LTLNMFCCommodityPrimeCode = "132680";
            ups.LTLNMFCCommoditySubCode = "02";

            // Below option will be customize as per requirement.
            ups.LTLCommodityFreightClass = "77.5";

            // Below option will be customize as per requirement.
            // 7   Bag            31  Bale
            // 8   Barrel         32  Basket
            // 33  Bin            34  Box
            // 35  Bunch          10  Bundle
            // 36  Cabinet        11  Can
            // 37  Carboy         38  Carrier
            // 39  Carton         40  Case
            // 54  Cask           41  Container
            // 14  Crate          15  Cylinder
            // 16  Drum           42  Loose
            // 99  Other          43  Package
            // 44  Pail           18  Pallet
            // 45  Pieces         46  Pipe Line
            // 53  Rack           47  Reel
            // 20  Roll           48  Skid
            // 19  Spool          49  Tank 
            // 3   Tube           50  Unit 
            // 51  Van Pack       52  Wrapped
            ups.LTLPackagingTypeCode = "BAG";
            ups.LTLPackagingTypeDescription = "BAG";

            string weightUnit = portalShippingModel.portalUnitModel.WeightUnit;
            if (weightUnit.Length > 0)
            {
                ups.WeightUnit = weightUnit.ToUpper().Equals("KGS") ? weightUnit.TrimEnd(new char[] { 'S' }) : weightUnit;
                ups.LTLUnitOfMeasurementDescription = "";
            }

            ups.LTLCommodityValueCurrencyCode = ShoppingCart.CurrencyCode;
            // Below option will be customize as per requirement.
            ups.LTLCommodityValueMonetaryValue = "100";

            // Below option will be customize as per requirement.
            ups.LTLCommodityDescription = "LCD TVS";

            // Below option will be customize as per requirement.
            ups.LTLHandlingUnitQuantity = "1";

            // Below option will be customize as per requirement.
            // PAL Pallet(s)
            // SKD Skid(s)
            // TOT Tote(s)
            // CAR Carboy(s)
            // OTH Other
            // LSE Loose
            ups.LTLHandlingUnitTypeCode = "SKD";
            ups.LTLHandlingUnitTypeDescription = "SKID";

            return ups;
        }

        public override List<ShippingModel> GetEstimateRate(List<ZnodeShippingBag> shippingbagList)
        {
            SplitShipSeparatelyFromShipTogether();
            UpsAgent ups = new UpsAgent();
            IZnodeShippingHelper shippingHelper = GetService<IZnodeShippingHelper>();
            PortalShippingModel portalShippingModel = shippingHelper.GetPortalShipping(Convert.ToInt32(ShoppingCart.PortalId), ShoppingCart.PublishStateId);
            GetUPSCredentialsSetting(portalShippingModel, ups);
            ups = GeneralSetting(portalShippingModel, ups);
            HandleSpecialShippingAddress(ups);
            List<ShippingModel> list = null;

            //The UPS  letter envelope packaging not support multi packaging so calculating the shipping rate for each line item. Link : https://www.ups.com/in/en/help-center/sri/glo-mlt-pc.page
            if (ups.PackageTypeCode == "01")
            {
                list = GetUPSEstimateShippingRateForLetterPackging(ups);
            }
            else
            {
                MapShipItemsDimensionsForEstimateRate(ups);
                list = ups.GetUPSEstimateRate();
            }

            bool isCalculatePromotionForShippingEstimates = ZnodeWebstoreSettings.IsCalculatePromotionForShippingEstimate;

            foreach (ShippingModel model in list ?? new List<ShippingModel>())
            {
                ZnodeShippingBag shippingBag = shippingbagList.FirstOrDefault(w => w.ShippingCode == model.ShippingCode);
                if (HelperUtility.IsNotNull(shippingBag))
                {                   
                    shippingBag.ShoppingCart.OrderLevelShipping = model.ShippingRate.GetValueOrDefault();
                    model.ShippingRate = shippingHelper.GetShipTogetherItemsHandlingCharge(shippingBag, Convert.ToDecimal(model.ShippingRate));

                    // Calculate shipping type promotion if isCalculatePromotionForShippingEstimates is true.
                    if (isCalculatePromotionForShippingEstimates && ShoppingCart.IsCalculatePromotionAndCoupon)
                        CalculatePromotionForShippingEstimate(shippingBag?.ShoppingCart);
                    // Calculate shipping handling charges.
                    model.ShippingHandlingCharge = shippingHelper.GetOrderLevelShippingHandlingCharge(ShippingBag, ShoppingCart);

                    if (shippingBag?.ShoppingCart?.Shipping?.ShippingDiscount > 0)
                    {
                        model.ShippingRateWithoutDiscount = model.ShippingRate;
                        model.ShippingRate = model.ShippingRate - shippingBag?.ShoppingCart?.Shipping?.ShippingDiscount;                            
                    }
                }
            }

            return list ?? new List<ShippingModel>();
        }

        /// <summary>
        /// This method is written to handle special cases of shipping address for UPS shipping method only.
        /// </summary>
        /// <param type="UpsAgent" name="ups"></param>
        /// <returns>It returns the updated object of UpsAgent with correct shipping address.</returns>
        protected virtual UpsAgent HandleSpecialShippingAddress(UpsAgent ups)
        {
            ZnodeLogging.LogMessage("Updating address according to UPS shipping rules.", ZnodeLogging.Components.Shipping.ToString(), TraceLevel.Info);
            //The following condition checks and alters the address according to UPS rules
            if (ups.ShipToCountryCode == "US" && ups.ShipStateProvinceCode == "PR")
            {
                ups.ShipToCountryCode = "PR";
            }
            return ups;
        }
        public virtual void MapShipItemsDimensionsForEstimateRate(UpsAgent ups)
        {
            StringBuilder packageXml = new StringBuilder();
            foreach (ZnodeShoppingCartItem cartItem in ShipSeparatelyItems)
            {
                SetPackageDetails(ups, packageXml, cartItem);
                ups.PackageXml = packageXml.ToString();
            }
            SetWeightUnit(ups);
        }

        // Get Estimated shipping rate for 
        public virtual List<ShippingModel> GetUPSEstimateShippingRateForLetterPackging(UpsAgent ups)
        {
            StringBuilder packageXml = new StringBuilder();
            List<ShippingModel> shippingRatelist = new List<ShippingModel>();
            foreach (ZnodeShoppingCartItem cartItem in ShipSeparatelyItems)
            {
                SetPackageDetails(ups, packageXml, cartItem);
                ups.PackageXml = ups.GetPackageXML().ToString();
                MapShippingRateListModel(shippingRatelist, ups.GetUPSEstimateRate());
            }
            return shippingRatelist;
        }

        //Map calculated rates for each shipping service code in ShippingRateList model. 
        public virtual List<ShippingModel> MapShippingRateListModel(List<ShippingModel> ShippingRateList, List<ShippingModel> calculatedList)
        {
            if(calculatedList?.Count > 0)
            {
                foreach(ShippingModel serviceRate in calculatedList)
                {
                    if(ShippingRateList?.Count > 0 && ShippingRateList.Any(x => x.ShippingCode == serviceRate.ShippingCode))
                    {
                        ShippingRateList.Where(w => w.ShippingCode == serviceRate.ShippingCode).ToList().ForEach(s => s.ShippingRate = (s.ShippingRate + serviceRate.ShippingRate));
                    }
                    else
                    {
                        ShippingRateList?.Add(
                                              new ShippingModel
                                              {
                                                  ShippingCode = serviceRate.ShippingCode,
                                                  ShippingRate = serviceRate.ShippingRate,
                                                  EstimateDate = serviceRate.EstimateDate
                                              });
                    }
                }
            }
            return ShippingRateList;
        }

        // Set package Xml for line items.
        protected virtual void SetPackageDetails(UpsAgent ups, StringBuilder packageXml, ZnodeShoppingCartItem cartItem)
        {
            ups.PackageWeight = (cartItem.Product.Weight * cartItem.Quantity);
            ups.PackageHeight = cartItem.Product.Height;
            ups.PackageWidth = cartItem.Product.Width;
            if (ups.PackageWeight > 0)
            {
                packageXml.Append(ups.GetPackageXML());
            }
        }

        // Calculate shipping type promotion if isCalculatePromotionForShippingEstimates is true.
        private void CalculatePromotionForShippingEstimate(ZnodeShoppingCart znodeShoppingCart)
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
