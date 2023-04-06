using StructureMap;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Entities;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
namespace Znode.Engine.Shipping
{
    public class ZnodeShippingManager : ZnodeBusinessBase, IZnodeShippingManager
    {
        #region private data members
        private ZnodeShoppingCart _shoppingCart;
        private ZnodeGenericCollection<IZnodeShippingsType> _shippingTypes;
        private List<ZnodeShippingBag> _shippingbagList;
        protected readonly IZnodeOrderShippingHelper znodeOrderShippingHelper = null;
        #endregion



        // Throws a NotImplementedException because this class requires a shopping cart to work.
        public ZnodeShippingManager()
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Instantiates all the shipping types and rules that are required for the current shopping cart.
        /// </summary>
        /// <param name="shoppingCart">The current shopping cart.</param>
        public ZnodeShippingManager(ZnodeShoppingCart shoppingCart, bool isAllowAddNewShippingType = false, ZnodeGenericCollection<IZnodeShippingsType> shippingTypes = null, List<ZnodeShippingBag> shippingbagList = null)
        {
            //Assign shopping cart data to instance variable of ZNodeShoppingCart.
            _shoppingCart = shoppingCart;
            if (!isAllowAddNewShippingType)
                _shippingTypes = new ZnodeGenericCollection<IZnodeShippingsType>();
            else
            {
                _shippingTypes = shippingTypes;
                _shippingbagList = shippingbagList;
            }
            znodeOrderShippingHelper = GetService<IZnodeOrderShippingHelper>(new ZnodeNamedParameter("shoppingCart", shoppingCart));

            if(!_shoppingCart.OrderId.HasValue || _shoppingCart.IsShippingRecalculate || isAllowAddNewShippingType || _shoppingCart.IsOldOrder)
            {
                ShippingRuleListModel shippingRules = new ShippingRuleListModel();
                IZnodeShippingHelper shippingHelper = GetService<IZnodeShippingHelper>();
                ShippingModel shippingModel = new ShippingModel();
                if (shoppingCart.Shipping.ShippingID > 0)
                {
                    shippingModel = shippingHelper.GetPortalProfileShipping(shoppingCart.Shipping.ShippingCountryCode, shoppingCart.Shipping.ShippingID, shoppingCart.PortalId, shoppingCart.ProfileId, shoppingCart.UserId);

                    shippingRules.ShippingRuleList = shippingHelper.GetPortalProfileShippingRuleList(shoppingCart.Shipping.ShippingCountryCode, shoppingCart.Shipping.ShippingID, shoppingCart.PortalId, shoppingCart.ProfileId, shoppingCart.UserId);

                    if (((HelperUtility.IsNotNull(shippingModel) && (string.IsNullOrEmpty(shippingModel.ZipCode) || shippingModel.ZipCode.Contains(shoppingCart.Payment.ShippingAddress.PostalCode)) || (shippingRules.ShippingRuleList.Count > 0))))
                    {
                        ZnodeShippingBag shippingBag = BuildShippingBag(shippingModel, shippingRules, shoppingCart.SubTotal);
                        if (isAllowAddNewShippingType)
                        {
                            _shippingbagList.Add(shippingBag);
                        }
                        AddShippingTypes(shippingModel, shippingBag);
                    }

                }
            }
        }

        // Calculates the shipping cost and updates the shopping cart.
        public virtual void Calculate()
        {
            _shoppingCart.OrderLevelShipping = 0;
            _shoppingCart.Shipping.ShippingHandlingCharge = 0;
            _shoppingCart.Shipping.ResponseCode = "0";
            _shoppingCart.Shipping.ResponseMessage = String.Empty;
            ResetShippingCostForCartItemsAndAddOns();
            
            if (_shoppingCart.OrderId > 0 && !_shoppingCart.IsCallLiveShipping && !_shoppingCart.IsShippingRecalculate)
                znodeOrderShippingHelper.OrderCalculate();
            else
            {
                CalculateEachShippingType();
                if(_shoppingCart.IsOldOrder)
                {
                    znodeOrderShippingHelper.SetOrderLinePerQuantityShippingCost();
                    znodeOrderShippingHelper.SetLineItemShipping();
                }              
            }


            //Calculate run time discount in case shipping change.
            if(_shoppingCart.OrderId > 0 && _shoppingCart.IsShippingRecalculate)
                znodeOrderShippingHelper.SetOrderLinePerQuantityShippingCost();

        }
        /// <summary>
        /// Process anything that must be done before the order is submitted.
        /// </summary>
        /// <returns>True if everything is good for submitting the order; otherwise, false.</returns>
        public virtual bool PreSubmitOrderProcess()
        {
            bool allPreConditionsOk = true;

            foreach (ZnodeShippingsType shippingType in _shippingTypes)
                // Make sure all pre-conditions are good before letting the customer check out
                allPreConditionsOk &= shippingType.PreSubmitOrderProcess();

            return allPreConditionsOk;
        }

        // Process anything that must be done after the order is submitted.
        public virtual void PostSubmitOrderProcess()
        {
            foreach (ZnodeShippingsType shippingType in _shippingTypes)
                shippingType.PostSubmitOrderProcess();
        }

        // Caches all available shipping types in the application cache.
        [Obsolete("This method is not in use now, as removed caching for shipping types")]
        public static void CacheAvailableShippingTypes()
        {
            if (Equals(HttpRuntime.Cache["ShippingTypesCache"], null))
            {
                var shippingTypes = GetAvailableShippingTypes();
                if (HelperUtility.IsNotNull(shippingTypes))
                    HttpRuntime.Cache["ShippingTypesCache"] = shippingTypes;
            }
        }

        /// <summary>
        /// Gets all available shipping types from the application cache.
        /// </summary>
        /// <returns>A list of the available shipping types.</returns>
        public static List<IZnodeShippingsType> GetAvailableShippingTypes()
        {
            ObjectFactory.Configure(scanner => scanner.Scan(x =>
            {
                x.AssembliesFromApplicationBaseDirectory(
                    assembly => assembly.FullName.Contains("Znode.") || assembly.FullName.Contains(!string.IsNullOrEmpty(ZnodeApiSettings.CustomAssemblyLookupPrefix) ? Convert.ToString(ZnodeApiSettings.CustomAssemblyLookupPrefix) : string.Empty));
                x.AddAllTypesOf<IZnodeShippingsType>();
            }));

            // Only cache shipping types that have a ClassName and Name; this helps avoid showing base classes in some of the dropdown lists
            var shippingTypes = ObjectFactory.GetAllInstances<IZnodeShippingsType>().Where(x => !String.IsNullOrEmpty(x.ClassName) && !String.IsNullOrEmpty(x.Name)).ToList();
            if (!Equals(shippingTypes, null))
                shippingTypes.Sort((shippingTypeA, shippingTypeB) => string.CompareOrdinal(shippingTypeA.Name, shippingTypeB.Name));
            else
                shippingTypes = new List<IZnodeShippingsType>();
            return shippingTypes;
        }

        private void ResetShippingCostForCartItemsAndAddOns()
        {
            foreach (ZnodeShoppingCartItem cartItem in _shoppingCart.ShoppingCartItems)
            {
                // Reset each line item shipping cost
                cartItem.ShippingCost = 0;
                if (HelperUtility.IsNotNull(cartItem.Product))
                {
                    cartItem.Product.ShippingCost = 0;

                    foreach (ZnodeProductBaseEntity addOn in cartItem.Product.ZNodeAddonsProductCollection)
                    {
                        addOn.ShippingCost = 0;
                    }

                    foreach (ZnodeProductBaseEntity group in cartItem.Product.ZNodeGroupProductCollection)
                        group.ShippingCost = 0;                    
                }
            }
        }

        //Calculate shipping types.
        private void CalculateEachShippingType()
        {
            //Calculate all shipping types.
            foreach (ZnodeShippingsType shippingType in _shippingTypes ?? new ZnodeGenericCollection<IZnodeShippingsType>())
            {
                try
                {
                    shippingType.ResetShippingItems();
                    shippingType.Calculate();
                }
                catch (Exception ex)
                {
                    //Log exception if occur.
                    ZnodeLogging.LogMessage("Error while calculating shipping type: " + shippingType.Name, "Shipping", TraceLevel.Error, ex);

                }
            }
        }

        //Assign shipping data to ZnodeShippingBag which is used by shipping types for calculations.
        private ZnodeShippingBag BuildShippingBag(ShippingModel shipping, ShippingRuleListModel shippingRuleList, decimal subTotal)
        {
            ZnodeShippingBag shippingBag = new ZnodeShippingBag();

            shippingBag.ShoppingCart = _shoppingCart;
            shippingBag.ShippingCode = shipping?.ShippingCode;
            shippingBag.HandlingCharge = shipping.HandlingCharge;
            shippingBag.ShippingRules = new ShippingRuleListModel();
            shippingBag.ShippingRules.ShippingRuleList = shippingRuleList.ShippingRuleList;
            shippingBag.SubTotal = subTotal;
            shippingBag.HandlingBasedOn = shipping?.HandlingChargeBasedOn ?? string.Empty;

            return shippingBag;
        }

        //Get Shipping types from assembly and add to local instance variable.
        private void AddShippingTypes(ShippingModel shipping, ZnodeShippingBag shippingBag)
        {
            try
            {
                IZnodeShippingsType type = GetShippingTypeInstance<IZnodeShippingsType>(shipping.ClassName);

                type.Bind(_shoppingCart, shippingBag);
                _shippingTypes.Add(type);
            }
            catch (Exception ex)
            {
                //Log exception if occur.
                ZnodeLogging.LogMessage("Error while instantiating shipping type: " + shipping.ClassName, "Shipping", TraceLevel.Error, ex);
            }
        }

        //Create and return instance for Shipping classes
        public virtual T GetShippingTypeInstance<T>(string className) where T : class
        {
            if (!string.IsNullOrEmpty(className))
                return (T)GetKeyedService<T>(className);
            return null;
        }

        private void AllowedTerritoriesForGroupConfigureAddonsProduct(ZnodeShoppingCart shoppingCart, ZnodeShoppingCartItem item)
        {
            foreach (ZnodeProductBaseEntity productBaseEntity in item.Product.ZNodeGroupProductCollection)
            {
                item.IsAllowedTerritories = !string.IsNullOrEmpty(productBaseEntity.AllowedTerritories) ? productBaseEntity.AllowedTerritories.Split(',').ToList().Contains(shoppingCart.Shipping.ShippingCountryCode) : true;
            }
        }

        public virtual List<ShippingModel> GetShippingEstimateRate(ZnodeShoppingCart znodeShoppingCart, ShoppingCartModel cartModel, string countryCode, List<ZnodeShippingBag> shippingbagList)
        {
            List<ShippingModel> listWithRates = new List<ShippingModel>();

            List<string> shippingName = new List<string>();
            foreach (ZnodeShippingsType shippingType in _shippingTypes ?? new ZnodeGenericCollection<IZnodeShippingsType>())
            {
                if (!shippingName.Contains(shippingType.Name))
                {
                    shippingType.ResetShippingItems();
                    shippingName.Add(shippingType.Name);


                    switch (shippingType?.Name?.ToLower())
                    {
                        case "ups":
                            listWithRates.AddRange(shippingType.GetEstimateRate(shippingbagList));
                            break;
                        case "usps":
                            listWithRates.AddRange(shippingType.GetEstimateRate(shippingbagList));
                            break;
                        case "fedex":
                            listWithRates.AddRange(shippingType.GetEstimateRate(shippingbagList));
                            break;
                        default:
                            break;
                    }

                }
            }

            return listWithRates ?? new List<ShippingModel>();
        }
    }
}
