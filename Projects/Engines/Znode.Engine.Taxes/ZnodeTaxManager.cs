using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web;
using StructureMap;
using Znode.Engine.Api.Models;
using Znode.Engine.Taxes.Interfaces;
using Znode.Libraries.ECommerce.Entities;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;
using static Znode.Libraries.ECommerce.Utilities.ZnodeDependencyResolver;
namespace Znode.Engine.Taxes
{
    // Helps in Manage taxes.
    public class ZnodeTaxManager : ZnodeBusinessBase,IZnodeTaxManager
    {
        #region Private Member Variables
        private const string TaxTypesCache = "TaxTypesCache";
        private ZnodeShoppingCart _shoppingCart;
        private ZnodeGenericCollection<IZnodeTaxesType> _taxes;
        private readonly IZnodeTaxHelper taxHelper;
        #endregion
        #region Constructors
        public ZnodeTaxManager()
        {
            // Throws a NotImplementedException because this class requires a shopping cart to work.
            throw new NotImplementedException();
        }

        public ZnodeTaxManager(ZnodeShoppingCart shoppingCart)
        {
            taxHelper = GetService<IZnodeTaxHelper>();
            _shoppingCart = shoppingCart;
            BindTaxRules();
        }

        public ZnodeTaxManager(ShoppingCartModel shoppingCartModel)
        {
            taxHelper = GetService<IZnodeTaxHelper>();
            BindShoppingCartModel(shoppingCartModel);
            BindTaxRules();
        }
        #endregion

        //Get the list
        public virtual List<TaxRuleModel> TaxRules
        {
            get
            {
                //Added for SiteAdmin CreateOrder Without Customer Selected
                if (HelperUtility.IsNull(_shoppingCart?.Payment?.ShippingAddress))
                {
                    _shoppingCart = new ZnodeShoppingCart();
                    _shoppingCart.Payment = new ZnodePayment();
                    _shoppingCart.Payment.ShippingAddress = new AddressModel();
                }

                AddressModel shippingAddress = _shoppingCart?.Payment?.ShippingAddress ?? new AddressModel();

                List<TaxRuleModel> taxRuleList = null;
                if (HelperUtility.IsNotNull(_shoppingCart))
                {
                    int? _userId = HelperUtility.IsNull(_shoppingCart.OrderId) ? _shoppingCart.UserId : GetGuestUserId(_shoppingCart.UserId, _shoppingCart.OrderId);
                    taxRuleList = taxHelper.GetTaxRuleListByPortalId(shippingAddress, Convert.ToInt32(_shoppingCart.PortalId), _shoppingCart.ProfileId, _userId, _shoppingCart.OrderId);
                }

                return taxRuleList ?? new List<TaxRuleModel>();
            }
        }

        // Calculates the sales tax for the shopping cart and its items. 
        public virtual void Calculate(ZnodeShoppingCart shoppingCart)
        {
            // Reset values  
            shoppingCart.OrderLevelTaxes = shoppingCart.GST = shoppingCart.HST = shoppingCart.PST = 0;
            shoppingCart.VAT = shoppingCart.SalesTax = shoppingCart.TaxRate = 0;

            // Go through each item in the cart
            foreach (ZnodeShoppingCartItem cartItem in shoppingCart.ShoppingCartItems)
            {
                cartItem.IsTaxCalculated = false;

                cartItem.Product.GST = cartItem.Product.HST = cartItem.Product.PST = cartItem.Product.VAT = cartItem.Product.SalesTax = 0;

                ResetValue(cartItem.Product.ZNodeGroupProductCollection);
                ResetValue(cartItem.Product.ZNodeAddonsProductCollection);
                ResetValue(cartItem.Product.ZNodeConfigurableProductCollection);
                if (shoppingCart.IsQuoteOrder)
                    cartItem.Product.PromotionalPrice = cartItem.Product.FinalPrice;
            }

            foreach (ZnodeTaxesType tax in _taxes)
                tax.Calculate();
        }

        // Process anything that must be done before the order is submitted.
        public virtual bool PreSubmitOrderProcess(ZnodeShoppingCart shoppingCart)
        {
            bool allPreConditionsOk = true;

            foreach (ZnodeTaxesType tax in _taxes)
                // Make sure all pre-conditions are good before letting the customer check out
                allPreConditionsOk &= tax.PreSubmitOrderProcess();

            return allPreConditionsOk;
        }

        // Process anything that must be done after the order is submitted.
        public virtual void PostSubmitOrderProcess(ZnodeShoppingCart shoppingCart, bool isTaxCostUpdated = true)
        {
            foreach (ZnodeTaxesType tax in _taxes)
                tax.PostSubmitOrderProcess(isTaxCostUpdated);
        }

        // Process anything that must be done after the order is submitted.
        public virtual void CancelOrderRequest(ShoppingCartModel shoppingCartModel)
        {
            foreach (ZnodeTaxesType tax in _taxes)
                tax.CancelOrderRequest(shoppingCartModel);
        }

        [Obsolete("This method is not in use now, as removed caching for taxes types")]
        // Caches all available tax types in the application cache.
        public static void CacheAvailableTaxTypes()
        {
            if (!HelperUtility.IsNull(HttpRuntime.Cache[TaxTypesCache])) return;
            var taxTypes = GetAvailableTaxTypes();
            if (HelperUtility.IsNotNull(taxTypes))
                HttpRuntime.Cache[TaxTypesCache] = taxTypes;
        }

        //Return order line item.
        public virtual void ReturnOrderLineItem(ShoppingCartModel orderModel)
        {
            foreach (ZnodeTaxesType tax in _taxes)
                tax.ReturnOrderLineItem(orderModel);
        }

        // Gets all available tax types from the application cache.
        public static List<IZnodeTaxesType> GetAvailableTaxTypes()
        {
            //Code Added for get all current instance of all request. 
            ObjectFactory.Configure(scanner => scanner.Scan(x =>
                {
                    x.AssembliesFromApplicationBaseDirectory(
                        assembly => assembly.FullName.Contains("Znode.") || assembly.FullName.Contains(!string.IsNullOrEmpty(ZnodeApiSettings.CustomAssemblyLookupPrefix) ? Convert.ToString(ZnodeApiSettings.CustomAssemblyLookupPrefix) : string.Empty));
                    x.AddAllTypesOf<IZnodeTaxesType>();
                }));
            // Only cache tax types that have a ClassName and Name; this helps avoid showing base classes in some of the dropdown lists
            var list = ObjectFactory.GetAllInstances<IZnodeTaxesType>().Where(x => !String.IsNullOrEmpty(x.ClassName) && !String.IsNullOrEmpty(x.Name))?.ToList();
            if (!Equals(list, null))
                list.Sort((taxA, taxB) => String.CompareOrdinal(taxA.Name, taxB.Name));
            else
                list = new List<IZnodeTaxesType>();
            return list;
        }

        //Get ZnodeTaxBag to calculate tax rates.
        protected virtual ZnodeTaxBag BuildTaxBag(TaxRuleModel rule)
            => new ZnodeTaxBag
            {
                DestinationCountryCode = string.IsNullOrEmpty(rule.DestinationCountryCode) ? null : rule.DestinationCountryCode,
                DestinationStateCode = string.IsNullOrEmpty(rule.DestinationStateCode) ? null : rule.DestinationStateCode,
                CountyFIPS = string.IsNullOrEmpty(rule.CountyFIPS) ? null : rule.CountyFIPS,
                SalesTax = rule.SalesTax.GetValueOrDefault(),
                ShippingTaxInd = rule.TaxShipping,
                VAT = rule.VAT.GetValueOrDefault(),
                TaxClassId = rule.TaxClassId.GetValueOrDefault(),
                GST = rule.GST.GetValueOrDefault(),
                PST = rule.PST.GetValueOrDefault(),
                HST = rule.HST.GetValueOrDefault(),
                Custom1 = Convert.ToString(rule.Custom1),
                InclusiveInd = rule.InclusiveInd,
                IsDefault = rule.IsDefault,
                TaxRuleId = rule.TaxRuleId,
                AssociatedTaxRuleIds = GetAssociatedTaxRuleIds()
            };

        //Add available tax class to generic tax class collection.
        protected virtual void AddTaxTypes(TaxRuleModel rule, ZnodeTaxBag taxBag)
        {
            try
            {
                IZnodeTaxesType type = GetTaxTypeInstance<IZnodeTaxesType>(rule.ClassName);

                type.Bind(_shoppingCart, taxBag);
                _taxes.Add(type);
            }
            catch (Exception ex)
            {
                //Log exception if occur.
                ZnodeLogging.LogMessage("Error while instantiating tax type: " + rule.ClassName, string.Empty, TraceLevel.Error, ex);
            }
        }

        //Create and return instance for tax type classes
        public virtual T GetTaxTypeInstance<T>(string className) where T : class
        {
            if (!string.IsNullOrEmpty(className))
                return (T)GetKeyedService<T>(className);
            return null;
        }

        // Reset values.
        protected virtual void ResetValue(ZnodeGenericCollection<ZnodeProductBaseEntity> productCollection)
        {
            foreach (ZnodeProductBaseEntity productItem in productCollection)
            {
                productItem.GST = productItem.HST = productItem.PST = productItem.VAT = productItem.SalesTax = 0;
                productItem.TaxCalculated = false;
            }
        }

        //to get all tax rule id associated to store
        protected virtual string GetAssociatedTaxRuleIds()
         => String.Join(",", TaxRules.Select(i => i.TaxClassId).ToList());

        //to get guest user id
        protected virtual int? GetGuestUserId(int? userId, int? orderId)
        {
            if (HelperUtility.IsNotNull(orderId) && orderId > 0 && taxHelper.IsGuestUser(userId.GetValueOrDefault()))
                return 0;
            return userId;
        }

        //Binds the shopping cart.
        protected virtual void BindShoppingCartModel(ShoppingCartModel shoppingCart)
        {
            _shoppingCart = new ZnodeShoppingCart();
            _shoppingCart.Payment = new ZnodePayment();
            _shoppingCart.Payment.ShippingAddress = shoppingCart.ShippingAddress;
            _shoppingCart.OrderId = shoppingCart.OmsOrderId;
            _shoppingCart.UserId = shoppingCart.UserId;
            _shoppingCart.PortalId = shoppingCart.PortalId;
            _shoppingCart.ProfileId = shoppingCart.ProfileId;
        }

        //Binds the tax rules.
        protected virtual void BindTaxRules()
        {
            _taxes = new ZnodeGenericCollection<IZnodeTaxesType>();

            List<TaxRuleModel> taxRules = TaxRules;
            if (taxRules?.Count > 0)
            {
                //Apply sorting based on precedence 
                taxRules.OrderBy(x => x.Precedence);

                // Loop through tax rules and apply to cart based on precedence
                foreach (TaxRuleModel rule in taxRules)
                    AddTaxTypes(rule, BuildTaxBag(rule));
            }
        }
    }
}

