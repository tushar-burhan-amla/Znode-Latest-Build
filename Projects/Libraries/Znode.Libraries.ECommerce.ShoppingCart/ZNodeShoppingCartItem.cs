using System;
using System.Collections.Generic;
using Znode.Engine.Api.Models;
using Znode.Engine.Promotions;
using Znode.Engine.Taxes;
using Znode.Libraries.ECommerce.Entities;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Libraries.ECommerce.ShoppingCart
{
    /// <summary>
    /// Represents a product items in the shopping cart
    /// </summary>
    [Serializable()]
    public class ZnodeShoppingCartItem : Entities.ZnodeShoppingCartItem
    {
        public ZnodeShoppingCartItem(AddressModel shippingAddress)
        {
            ExternalId = Guid.NewGuid().ToString();
            this.ShippingAddress = shippingAddress;
        }

        #region Public Properties

        // Gets or sets the product object for this cart item
        public new ZnodeProductBase Product
        {
            get { return (ZnodeProductBase)base.Product; }
            set { base.Product = value; }
        }

        // Gets the unit price of this line item.
        public new decimal UnitPrice
        {
            get
            {
                decimal basePrice = 0;
                if (HelperUtility.IsNotNull(Product))
                {
                    if (HelperUtility.IsNotNull(Product.CustomPrice) && Product.CustomPrice > 0)
                        return Product.CustomPrice.GetValueOrDefault();

                    ZnodePricePromotionManager pricePromoManager = new ZnodePricePromotionManager();
                    basePrice = pricePromoManager.PromotionalPrice(Product, TieredPricing, OmsOrderId.GetValueOrDefault());//Once all promotions are done then remove first line and uncomment this line

                    //to get group product price in group product parent product price is not required
                    if (Product.ZNodeGroupProductCollection.Count > 0)
                    {
                        ZnodeGenericCollection<ZnodeProductTierEntity> productTiers = Product.ZNodeTieredPriceCollection;

                        //Check product tiers list
                        if (productTiers?.Count > 0)
                        {
                            //Bind tier pricing data.
                            basePrice = GetTierPrice(productTiers, Product.ZNodeGroupProductCollection[0].SelectedQuantity, basePrice);
                        }
                        else
                            basePrice = Product.GroupProductPrice;
                    }

                    if (basePrice > 0)
                        //Calculate sales tax on discounted price.
                        basePrice = new ZnodeInclusiveTax().GetInclusivePrice(Product.TaxClassID, basePrice, Product.AddressToShip, ShippingAddress);

                    basePrice = basePrice + (!IsAllowAddOnQuantity? Product.AddOnPrice : 0 );

                    //to get configurable product price if parent product dont have price
                    if (basePrice.Equals(0) && Product.ZNodeConfigurableProductCollection.Count > 0)
                        basePrice += Product.ConfigurableProductPrice;
                }
                return basePrice;
            }
        }

        // Gets or sets the product extended price
        public new decimal ExtendedPrice
        {
            get
            {
                if (HelperUtility.IsNotNull(Product))
                {
                    if (Product?.ZNodeGroupProductCollection?.Count > 0)
                        _ExtendedPrice = UnitPrice * GetGroupProductQuantity(Product?.ZNodeGroupProductCollection) + (IsAllowAddOnQuantity ? Convert.ToDecimal(Product?.ExtendedAddOnPrice):0);
                    else
                        _ExtendedPrice = (UnitPrice * Quantity) + (IsAllowAddOnQuantity ? Convert.ToDecimal(Product?.ExtendedAddOnPrice):0);
                }
                return Math.Round( _ExtendedPrice, ZnodeConstant.PriceConstant, MidpointRounding.AwayFromZero);
            }

            set
            {
                _ExtendedPrice = value;
            }
        }

        //Unique identifier for external integration.
        public string ExternalId { get; set; }
        public bool ShipSeperately { get; set; }
        public int ShippingOptionId { get; set; }
        public decimal? CustomUnitPrice { get; set; }
        public decimal? PartialRefundAmount { get; set; }
        public Dictionary<string, decimal> AdditionalCost { get; set; }
        public List<AssociatedProductModel> AssociatedAddOnProducts { get; set; }
        public decimal? ProductLevelTax { get; set; }
        public int? PaymentStatusId { get; set; }
        public string InventoryTracking { get; set; }
        #endregion

        public new ZnodeShoppingCartItem Clone()
        {
            var copiedItem = MemberwiseClone() as ZnodeShoppingCartItem;
            return copiedItem;
        }
    }
}
