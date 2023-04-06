using System;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Entities;
using Znode.Libraries.Framework.Business;

namespace Znode.Engine.Taxes
{
    public class ZnodeTaxSalesTax : ZnodeTaxesType
    {
        #region Constructors

        public ZnodeTaxSalesTax()
        {
            Name = "Sales Tax";
            Description = "Applies sales tax to the shopping cart.";

            Controls.Add(ZnodeTaxRuleControl.SalesTax);
            Controls.Add(ZnodeTaxRuleControl.VAT);
            Controls.Add(ZnodeTaxRuleControl.GST);
            Controls.Add(ZnodeTaxRuleControl.PST);
            Controls.Add(ZnodeTaxRuleControl.HST);
            Controls.Add(ZnodeTaxRuleControl.Precedence);
            Controls.Add(ZnodeTaxRuleControl.Inclusive);
        }

        #endregion Constructors

        #region Public Method

        // Calculates the sales tax and updates the shopping cart.
        public override void Calculate()
        {
            if (IsValid())
            {
                bool? isAnyTaxApplied = ShoppingCart.ShoppingCartItems?.Find(x => x.IsTaxCalculated == true)?.IsTaxCalculated;
                // Go through each item in the cart
                foreach (ZnodeShoppingCartItem cartItem in ShoppingCart.ShoppingCartItems)
                {
                    if(!Equals(cartItem, null))
                    {
                        decimal extendedPrice = GetProductPrice(cartItem);                             

                        int? groupProductCount = cartItem?.Product?.ZNodeGroupProductCollection?.Count;

                        int? addOnProductCount = cartItem?.Product?.ZNodeAddonsProductCollection?.Count;

                        int? configurableProductCount = cartItem?.Product?.ZNodeConfigurableProductCollection?.Count;

                        //to calculate tax of parent product if product type not equals to Group/Configurable product
                        if (Equals(cartItem.Product.TaxClassID, TaxBag.TaxClassId) && Equals(cartItem.IsTaxCalculated, false) && Equals(Convert.ToInt32(groupProductCount), 0) && Equals(Convert.ToInt32(configurableProductCount), 0))
                        {
                            GetTaxesValue(extendedPrice, cartItem.Product);
                            SetTaxesValueToShoppingCart(cartItem.Product);
                            cartItem.IsTaxCalculated = true;
                            cartItem.TaxRuleId = this.TaxBag.TaxRuleId;
                        }
                        //to calculate tax for the product which dont associated to any tax rule but default tax rule is set for the store.
                        else if (IsDefaultTaxApplicable(cartItem.Product.TaxClassID) && Equals(cartItem.IsTaxCalculated, false) && Equals(Convert.ToInt32(groupProductCount), 0) && Equals(Convert.ToInt32(configurableProductCount), 0))
                        {
                            GetTaxesValue(extendedPrice, cartItem.Product);
                            SetTaxesValueToShoppingCart(cartItem.Product);
                            cartItem.IsTaxCalculated = true;
                            cartItem.TaxRuleId = this.TaxBag.TaxRuleId;
                        }

                        //to calculate tax of all Group product
                        if (groupProductCount > 0)
                            CalculateTaxForGroupProduct(cartItem.Product.ZNodeGroupProductCollection, cartItem);
                                                                       
                        //to calculate tax of all configurable product
                        if (configurableProductCount > 0)
                            CalculateTaxForConfigurableProduct(cartItem.Product.ZNodeConfigurableProductCollection, cartItem);

                        if (Convert.ToInt32(groupProductCount) > 0 || Convert.ToInt32(configurableProductCount) > 0)
                        {
                            cartItem.IsTaxCalculated = true;
                            cartItem.TaxRuleId = this.TaxBag.TaxRuleId;
                        }
                    }
                }
                
                //Add shipping amount in tax list
                if (IsCalculateTaxOnShipping(ShoppingCart.ShippingCost) && !isAnyTaxApplied.GetValueOrDefault())
                {
                    decimal shippingCost = IsCalculateTaxAfterDiscount() ? ShoppingCart.ShippingCost - ShoppingCart.ShippingDiscount : (ShoppingCart.ShippingCost);
                    SetShippingInTaxesValueOfShoppingCart(shippingCost);
                }
            }
        }

        // Process anything that must be done before the order is submitted.
        public override bool PreSubmitOrderProcess()
        {
            AddressModel destinationaddress = ShoppingCart?.Payment?.BillingAddress ?? new AddressModel();

            if (string.IsNullOrEmpty(destinationaddress.CountryName) || string.IsNullOrEmpty(destinationaddress.StateName))
            {
                // set tax rate and error message
                ShoppingCart.TaxRate = 0;
                ShoppingCart.AddErrorMessage = "tax error: invalid destination country or state code.";
                return false;
            }

            return true;
        }

        // Process anything that must be done after the order is submitted.
        public override void PostSubmitOrderProcess(bool isTaxCostUpdated = true)
        {
            // nothing to see here, move along, move along
        }

        #endregion Public Method

        #region Private Method

        // Calculate extended price.
        private decimal GetValue(decimal extendedPrice, decimal value)
            => extendedPrice * (value / 100);

        // Get product final/actual price.
        private decimal GetProductPrice(ZnodeShoppingCartItem cartItem)
        {
            return GetCartItemPrice(cartItem, cartItem.Quantity);
        }

        // Get Variant product final/actual price.
        private decimal GetVariantProductPrice(ZnodeShoppingCartItem cartItem)
        {
            decimal quantity = cartItem.Quantity;
            if (cartItem?.Product?.ZNodeGroupProductCollection?.Count > 0)
                quantity = cartItem.Product.ZNodeGroupProductCollection[0].SelectedQuantity;
            decimal extendedPriceAfterDiscount = (cartItem.PromotionalPrice * quantity) - cartItem.DiscountAmount;
            return IsCalculateTaxAfterDiscount() ? (extendedPriceAfterDiscount <= 0 ? 0 : extendedPriceAfterDiscount)
                                               : (cartItem.UnitPrice * quantity);
        }

        // Get Group product final/actual price.
        private decimal GetGroupProductPrice(ZnodeProductBaseEntity groupItem, ZnodeShoppingCartItem cartItem)
        {
            decimal extendedPriceAfterDiscount = (cartItem.PromotionalPrice * groupItem.SelectedQuantity) - cartItem.DiscountAmount;
            if (IsCalculateTaxAfterDiscount())
                return extendedPriceAfterDiscount <= 0 ? 0 : extendedPriceAfterDiscount;
            return (cartItem.UnitPrice * groupItem.SelectedQuantity);
        }

        // Set GST, HST, PST, VAT, SalesTax to product.
        private void GetTaxesValue(decimal extendedPrice, ZnodeProductBaseEntity product)
        {
            product.GST += GetValue(extendedPrice, TaxBag.GST);
            product.HST += GetValue(extendedPrice, TaxBag.HST);
            product.PST += GetValue(extendedPrice, TaxBag.PST);
            product.VAT += GetValue(extendedPrice, TaxBag.VAT);
            product.SalesTax += GetValue(extendedPrice, TaxBag.SalesTax);
        }

        // Set GST, HST, PST, VAT, SalesTax to shopping cart for product.
        private void SetTaxesValueToShoppingCart(ZnodeProductBaseEntity product)
        {
            ShoppingCart.GST += product.GST;
            ShoppingCart.HST += product.HST;
            ShoppingCart.PST += product.PST;
            ShoppingCart.VAT += product.VAT;
            ShoppingCart.SalesTax += product.SalesTax;
        }

        // Set GST, HST, PST, VAT, SalesTax to shipping in shopping cart for product.
        private void SetShippingInTaxesValueOfShoppingCart(decimal shippingCost)
        {
            ShoppingCart.GST += GetValue(shippingCost, TaxBag.GST);
            ShoppingCart.HST += GetValue(shippingCost, TaxBag.HST);
            ShoppingCart.PST += GetValue(shippingCost, TaxBag.PST);
            ShoppingCart.VAT += GetValue(shippingCost, TaxBag.VAT);
            ShoppingCart.SalesTax += GetValue(shippingCost, TaxBag.SalesTax);

            SetTaxOnShipping(shippingCost);
        }

        private void SetTaxOnShipping(decimal shippingCost)
        {
            ShoppingCart.TaxOnShipping += (GetValue(shippingCost, TaxBag.GST)
                                           + GetValue(shippingCost, TaxBag.HST)
                                           + GetValue(shippingCost, TaxBag.PST)
                                           + GetValue(shippingCost, TaxBag.VAT)
                                           + GetValue(shippingCost, TaxBag.SalesTax));
        }

        // Calculate tax for group product.
        private void CalculateTaxForAddonProduct(ZnodeGenericCollection<ZnodeProductBaseEntity> productCollection, ZnodeShoppingCartItem cartItem)
        {
            foreach (ZnodeProductBaseEntity productItem in productCollection)
            {
                decimal extendedPrice = GetVariantProductPrice(cartItem);
                ApplyTax(productItem, extendedPrice);
            }
        }

        // Calculate tax for group product.
        private void CalculateTaxForGroupProduct(ZnodeGenericCollection<ZnodeProductBaseEntity> productCollection, ZnodeShoppingCartItem cartItem)
        {
            foreach (ZnodeProductBaseEntity productItem in productCollection)
            {
                decimal extendedPrice = GetProductPrice(cartItem);
                ApplyTax(productItem, extendedPrice);
            }
        }

        // Calculate tax for Configurable product.
        private void CalculateTaxForConfigurableProduct(ZnodeGenericCollection<ZnodeProductBaseEntity> productCollection, ZnodeShoppingCartItem cartItem)
        {
            foreach (ZnodeProductBaseEntity productItem in productCollection)
            {
                decimal extendedPrice = GetProductPrice(cartItem);

                if (extendedPrice == 0)
                    extendedPrice = GetVariantProductPrice(cartItem);

                ApplyTax(productItem, extendedPrice);
            }           
        }

        //to apply tax to product
        private void ApplyTax(ZnodeProductBaseEntity productItem, decimal extendedPrice)
        {
            bool taxCalculated = false;
            if ((Equals(productItem.TaxClassID, TaxBag.TaxClassId) && !productItem.TaxCalculated) || (!taxCalculated && IsDefaultTaxApplicable(productItem.TaxClassID) && !productItem.TaxCalculated))
            {
                GetTaxesValue(extendedPrice, productItem);
                SetTaxesValueToShoppingCart(productItem);
                productItem.TaxCalculated = true;
            }
        }

        //to check store's default tax applicable for the current product
        private bool IsDefaultTaxApplicable(int productTaxClassId)
        {
            return TaxBag.IsDefault == true && (productTaxClassId == 0 || !TaxBag.AssociatedTaxRuleIds.Contains(productTaxClassId.ToString()));
        }

        #endregion Private Method
    }
}