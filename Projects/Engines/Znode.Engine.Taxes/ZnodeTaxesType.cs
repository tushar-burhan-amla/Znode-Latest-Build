using System;
using System.Collections.Generic;
using System.Linq;
using Znode.Engine.Api.Models;
using Znode.Engine.Promotions;
using Znode.Engine.Taxes.Interfaces;
using Znode.Libraries.Data;
using Znode.Libraries.Data.DataModel;
using Znode.Libraries.ECommerce.Entities;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
namespace Znode.Engine.Taxes
{
    // This is the base class for all tax types.
    public class ZnodeTaxesType : IZnodeTaxesType
    {
        #region Member variables
        private string _className;
        private List<ZnodeTaxRuleControl> _controls;
        #endregion


        #region public Properties
        public virtual string ClassName
        {
            get
            {
                if (String.IsNullOrEmpty(_className))
                {
                    _className = GetType().Name;
                }

                return _className;
            }
        }

        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public virtual int Precedence { get; set; }
        public virtual ZnodeShoppingCart ShoppingCart { get; set; }
        public virtual ZnodeTaxBag TaxBag { get; set; }

        public virtual List<ZnodeTaxRuleControl> Controls
        {
            get { return _controls ?? (_controls = new List<ZnodeTaxRuleControl>()); }
        }

        #endregion

        // Binds the shopping cart and tax data to the tax rule.
        public virtual void Bind(ZnodeShoppingCart shoppingCart, ZnodeTaxBag taxBag)
        {
            ShoppingCart = shoppingCart;
            TaxBag = taxBag;
        }

        // Calculates the tax and updates the shopping cart.
        public virtual void Calculate() { }

        //CCH full order return request ,compensates the transaction for returns or losses.
        public virtual void PartialReturnOrderLineItem() { }


        // returnsTrue if everything is good for submitting the order; otherwise, false.
        public virtual bool PreSubmitOrderProcess() => true;

        // Process anything that must be done after the order is submitted.  
        public virtual void PostSubmitOrderProcess(bool isTaxCostUpdated = true)
        {
            // Most taxes don't need any further processing after the order is submitted
        }

        // Process anything that must be done after the order is submitted.  
        public virtual void CancelOrderRequest(ShoppingCartModel shoppingCartModel)
        {
            // Most taxes don't need any further processing after the order is submitted
        }

        // This Method Indicates whether or not the tax rule is valid and should be applied to the shopping cart.
        public virtual bool IsValid()
        {
            bool isValid = false;

            AddressModel destinationAddress = ShoppingCart.Payment.ShippingAddress;

            //Check if tax rule applies to all countries
            if (Equals(TaxBag.DestinationCountryCode, null) || string.Equals(destinationAddress.CountryName, TaxBag.DestinationCountryCode, StringComparison.OrdinalIgnoreCase))
            {
                // Check if tax rule applies to all states
                if (Equals(TaxBag?.DestinationStateCode, null))
                    isValid = true;
                else if (string.Equals(TaxBag.DestinationStateCode, destinationAddress.StateCode, StringComparison.OrdinalIgnoreCase))
                {
                    // Check if tax rule applies to specific state
                    if (Equals(TaxBag?.CountyFIPS, null))
                        isValid = true;
                    else
                    {
                        IZnodeTaxHelper taxHelper = GetService<IZnodeTaxHelper>();
                        TaxRuleModel taxRuleModel = new TaxRuleModel() { CountyFIPS = TaxBag.CountyFIPS };
                        List<CityModel> cityList = taxHelper.GetTaxRuleByCountryFIPSCityStateCodePostalCode(destinationAddress, taxRuleModel);
                        if (!Equals(cityList?.Count, 0))
                            isValid = true;
                    }
                }
            }

            return isValid;
        }

        // call this method to cancel the tax request for 3rd party integrations using the transaction number
        public virtual bool CancelTaxRequest(string taxTransactionNumber, int? portalId) => false;

        // call this method in order to fulfill the tax requirement 
        public virtual bool TaxFulfillment(string taxTransactionNumber) => false;

        //Returns order line item.
        public virtual void ReturnOrderLineItem(ShoppingCartModel orderModel) { }

        //Get the currency.
        public virtual string GetCulture(int currentStore)
        {
            IZnodeRepository<ZnodeCurrency> znodeCurrency = new ZnodeRepository<ZnodeCurrency>();
            IZnodeRepository<ZnodeCulture> znodeCulture = new ZnodeRepository<ZnodeCulture>();
            IZnodeRepository<ZnodePortalUnit> znodePortalUnit = new ZnodeRepository<ZnodePortalUnit>();
            //all client side transactions
            string culture = (from _znodePortalUnit in znodePortalUnit.Table
                              join _znodeCurrency in znodeCurrency.Table on _znodePortalUnit.CurrencyId equals _znodeCurrency.CurrencyId
                              join _znodeCulture in znodeCulture.Table on _znodePortalUnit.CultureId equals _znodeCulture.CultureId
                              where _znodePortalUnit.PortalId == currentStore
                              select
                                  _znodeCulture.CultureCode //TODO-U323
                        )?.FirstOrDefault();
            return culture;
        }

        public virtual string GetCurrencyCode(int currentStore)
        {
            IZnodeRepository<ZnodeCurrency> znodeCurrency = new ZnodeRepository<ZnodeCurrency>();
            IZnodeRepository<ZnodePortalUnit> znodePortalUnit = new ZnodeRepository<ZnodePortalUnit>();

            string currencyCode = (from _znodePortalUnit in znodePortalUnit.Table
                                   join _znodeCurrency in znodeCurrency.Table on _znodePortalUnit.CurrencyId equals _znodeCurrency.CurrencyId
                                   where _znodePortalUnit.PortalId == currentStore
                                   select _znodeCurrency.CurrencyCode
                        )?.FirstOrDefault();
            return currencyCode;
        }

        protected virtual bool IsCalculateTaxAfterDiscount()
        {
            if (HelperUtility.IsNotNull(ShoppingCart.OrderId) && ShoppingCart.OrderId > 0 && HelperUtility.IsNotNull(ShoppingCart.IsCalculateTaxAfterDiscount))               
                return ShoppingCart.IsCalculateTaxAfterDiscount.GetValueOrDefault();
            else
            {
                ShoppingCart.IsCalculateTaxAfterDiscount = DefaultGlobalConfigHelper.IsCalculateTaxAfterDiscount;
                return DefaultGlobalConfigHelper.IsCalculateTaxAfterDiscount;
            }
        }

        protected virtual bool IsCalculateTaxAfterDiscount(ZnodeShoppingCart shoppingCart)
        {
            if (HelperUtility.IsNotNull(shoppingCart.OrderId) && shoppingCart.OrderId > 0 && HelperUtility.IsNotNull(shoppingCart.IsCalculateTaxAfterDiscount))
                return shoppingCart.IsCalculateTaxAfterDiscount.GetValueOrDefault();
            else
            {
                shoppingCart.IsCalculateTaxAfterDiscount = DefaultGlobalConfigHelper.IsCalculateTaxAfterDiscount;
                return DefaultGlobalConfigHelper.IsCalculateTaxAfterDiscount;
            }
        }

        protected virtual bool IsPricesInclusiveOfTaxes(ZnodeShoppingCart shoppingCart)
        {
            if (HelperUtility.IsNotNull(shoppingCart.OrderId) && shoppingCart.OrderId > 0 && HelperUtility.IsNotNull(shoppingCart.IsPricesInclusiveOfTaxes))
                return shoppingCart.IsPricesInclusiveOfTaxes.GetValueOrDefault();
            else
            {
                IZnodeRepository<ZnodeTaxPortal> _taxPortalRepository = new ZnodeRepository<ZnodeTaxPortal>();
                shoppingCart.IsPricesInclusiveOfTaxes = Convert.ToBoolean(_taxPortalRepository.Table.FirstOrDefault(x => x.PortalId == shoppingCart.PortalId)?.AvataxIsTaxIncluded);
                return shoppingCart.IsPricesInclusiveOfTaxes.GetValueOrDefault();
            }
        }
        
        //Get the line item discount.
        protected virtual decimal GetLineItemDiscount(ZnodeShoppingCartItem cartItem)
        {
            return (cartItem.DiscountAmount + cartItem.ExtendedPriceDiscount) + (cartItem.PerQuantityOrderLevelDiscountOnLineItem * cartItem.Quantity) + (cartItem.PerQuantityCSRDiscount * cartItem.Quantity);
        }

        protected virtual decimal GetReturnLineItemDiscount(ReturnOrderLineItemModel cartItem)
        {
            return (cartItem.PerQuantityLineItemDiscount * cartItem.Quantity) + (cartItem.PerQuantityOrderLevelDiscountOnLineItem * cartItem.Quantity) + (cartItem.PerQuantityCSRDiscount * cartItem.Quantity);
        }

        protected virtual bool IsCalculateTaxOnShipping(decimal shippingCost, bool shippingTaxInd)
        {
            bool isCalculateTaxOnShipping = false;
            if (shippingCost > 0)
            {
                isCalculateTaxOnShipping = Convert.ToBoolean(shippingTaxInd);
            }
            return isCalculateTaxOnShipping;
        }

        protected virtual bool IsCalculateTaxOnShipping(decimal shippingCost)
        {
            bool isCalculateTaxOnShipping = false;
            if (shippingCost > 0)
            {
                isCalculateTaxOnShipping = Convert.ToBoolean(this.TaxBag?.ShippingTaxInd);
            }
            return isCalculateTaxOnShipping;
        }

        protected virtual decimal GetCartItemPrice(ZnodeShoppingCartItem cartItem, decimal cartQuantity, ZnodeShoppingCart shoppingCart = null)
        {
            decimal unitPrice = GetDisplayedPrice(cartItem);
            decimal extendedPriceAfterDiscount = (unitPrice * cartQuantity) - GetLineItemDiscount(cartItem);
            if (HelperUtility.IsNotNull(shoppingCart))
                return IsCalculateTaxAfterDiscount(shoppingCart) && !shoppingCart.IsQuoteOrder ? (extendedPriceAfterDiscount <= 0 ? 0 : extendedPriceAfterDiscount) : (unitPrice * cartQuantity);
            else
                return IsCalculateTaxAfterDiscount() ? (extendedPriceAfterDiscount <= 0 ? 0 : extendedPriceAfterDiscount): (unitPrice * cartQuantity);
        }

        protected virtual decimal GetReturnCartItemPrice(ReturnOrderLineItemModel cartItem, decimal cartQuantity)
        {
            decimal extendedPriceAfterDiscount = (cartItem.UnitPrice * cartQuantity) - GetReturnLineItemDiscount(cartItem);
            return IsCalculateTaxAfterDiscount() ? (extendedPriceAfterDiscount <= 0 ? 0 : extendedPriceAfterDiscount) : (cartItem.UnitPrice * cartQuantity);
        }

        protected virtual decimal GetReturnShippingCost(decimal shippingCost, decimal shippingDiscount)
        {
            decimal extendedShippingCost = 0;
            if (shippingCost > 0)
            {
                extendedShippingCost= IsCalculateTaxAfterDiscount() ? shippingCost - shippingDiscount : shippingCost;
            }
            return extendedShippingCost;
        }

        //Get displayed price which is inclusive of product displayed price promotion.
        protected virtual decimal GetDisplayedPrice(ZnodeShoppingCartItem cartItem)
        {
            ZnodePricePromotionManager pricePromoManager = new ZnodePricePromotionManager();
            decimal basePrice = pricePromoManager.PromotionalPrice(cartItem.Product, cartItem.TieredPricing);
            if (cartItem.Product.ZNodeGroupProductCollection.Count > 0)
            {
                ZnodeGenericCollection<ZnodeProductTierEntity> productTiers = cartItem.Product.ZNodeTieredPriceCollection;

                //Check product tiers list
                if (productTiers?.Count > 0)
                {
                    //Bind tier pricing data.
                    basePrice = GetTierPrice(productTiers, cartItem.Product.ZNodeGroupProductCollection[0].SelectedQuantity, basePrice);
                }
                else
                    basePrice = cartItem.Product.GroupProductPrice;
            }

            if (basePrice > 0)
                //Calculate sales tax on discounted price.
                basePrice = new ZnodeInclusiveTax().GetInclusivePrice(cartItem.Product.TaxClassID, basePrice, cartItem.Product.AddressToShip, cartItem.ShippingAddress);

            basePrice = basePrice + (!IsAllowAddOnQuantity ? cartItem.Product.AddOnPrice : 0);

            //to get configurable product price if parent product dont have price
            if (basePrice.Equals(0) && cartItem.Product.ZNodeConfigurableProductCollection.Count > 0)
                basePrice += cartItem.Product.ConfigurableProductPrice;
        
            return basePrice > 0 ? basePrice : cartItem.UnitPrice;
        }

        public virtual decimal GetTierPrice(ZnodeGenericCollection<ZnodeProductTierEntity> productTiers, decimal quantity, decimal finalPrice)
        {
            foreach (ZnodeProductTierEntity productTieredPrice in productTiers)
            {
                //check if tier quantity is valid or not.
                if (quantity >= productTieredPrice.MinQuantity && quantity < productTieredPrice.MaxQuantity)
                {
                    finalPrice = productTieredPrice.Price;
                    break;
                }
            }
            return finalPrice;
        }

        public virtual bool IsAllowAddOnQuantity { get => Convert.ToBoolean(ZnodeApiSettings.IsAllowAddOnQuantity); }

    }
}
