using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Libraries.ECommerce.Entities
{
    // Represents a product items in the shopping cart
    [Serializable()]
    public class ZnodeShoppingCartItem : ZnodeBusinessBase
    {
        public const string ProfileCache = "ProfileCache";

        #region Private Member Variables
        private string _Guid = string.Empty;
        protected decimal _ExtendedPrice = 0;
        private decimal _ShippingCost = 0;
        private string _PromoDescription = string.Empty;

        #endregion

        #region Constructors      
        // Initializes a new instance of the ZNodeShoppingCartItem class. Create GUID value for the shopping cart item
        public ZnodeShoppingCartItem()
        {
            // Create GUID
            _Guid = System.Guid.NewGuid().ToString();
            OrderShipments = new List<ZnodeOrderShipment>();
            IsTaxCalculated = false;
        }
        #endregion

        #region Public Properties

        //Get or Set Order Shipments
        public List<ZnodeOrderShipment> OrderShipments { get; set; }

        //Get or Set Shipping Address
        public AddressModel ShippingAddress { get; set; }

        // Gets the database shopping cart line item ID for this cart item.        
        public int LineItemId { get; set; }

        // Gets the GUID (unique value) for this Shopping cart item. 
        [XmlElement()]
        public virtual string GUID
        {
            get
            {
                return _Guid;
            }
        }
        // Gets or sets the product object for this cart item
        public ZnodeProductBaseEntity Product { get; set; }

        //Get and set Personalise Value list
        public Dictionary<string, object> PersonaliseValuesList { get; set; }

        public List<PersonaliseValueModel> PersonaliseValuesDetail { get; set; }

        // Gets the unit price of this line item.
        public virtual decimal UnitPrice
        {
            get
            {
                if (HelperUtility.IsNotNull(Product?.CustomPrice) && Product?.CustomPrice > 0)
                    return Product.CustomPrice.GetValueOrDefault();

                decimal basePrice = this.TieredPricing;

                //to get group product price 
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

                //to get configurable product price if parent product dont have price
                if (basePrice.Equals(0) && Product.ZNodeConfigurableProductCollection.Count > 0)
                    basePrice += Product.ConfigurableProductPrice;

                decimal unitPrice = basePrice + (!IsAllowAddOnQuantity ? Product.AddOnPrice: 0);

                return unitPrice;
            }
        }

        public virtual bool IsAllowAddOnQuantity { get => Convert.ToBoolean(ZnodeApiSettings.IsAllowAddOnQuantity);}


        // Gets or sets the product extended price
        public decimal ExtendedPrice
        {
            get
            {
                if (this.Product?.ZNodeGroupProductCollection?.Count > 0)
                    this._ExtendedPrice = this.UnitPrice * GetGroupProductQuantity(this.Product?.ZNodeGroupProductCollection) + (IsAllowAddOnQuantity ? Convert.ToDecimal(Product?.ExtendedAddOnPrice):0);
                else
                    this._ExtendedPrice = this.UnitPrice * this.Quantity + (IsAllowAddOnQuantity ? Convert.ToDecimal(Product?.ExtendedAddOnPrice):0);
                return this._ExtendedPrice;
            }

            set
            {
                this._ExtendedPrice = value;
            }
        }

        //To after getting Tier Pricing data it will get uncomment.
        // Gets the product tiered pricing. Returns the tiered Price for this product based on the quantity.
        // If no tiered pricing applied, it will return the product base price.
        public virtual decimal TieredPricing
        {
            get
            {
                ZnodeGenericCollection<ZnodeProductTierEntity> productTiers = Product.ZNodeTieredPriceCollection;
                decimal quantity = Quantity;
                decimal finalPrice = Product?.FinalPrice ?? 0;

                //Check product tiers list
                if (productTiers?.Count > 0)
                {
                    //Bind tier pricing data.
                    finalPrice = GetTierPrice(productTiers, quantity, finalPrice);
                }

                // Get Product tiered price if one exists, otherwise unit price
                return finalPrice;
            }
        }

        // Gets or sets the quantity of products for this line item.
        public virtual decimal Quantity { get; set; }

        // Gets or sets the shipping cost for this line item.
        public virtual decimal ShippingCost
        {
            get
            {
                decimal shipCost = 0;

                //to add all addOn product shippingcost
                foreach (ZnodeProductBaseEntity addOn in Product.ZNodeAddonsProductCollection)
                {
                    shipCost += addOn.ShippingCost;
                }

                //to add all configurable product shippingcost
                foreach (ZnodeProductBaseEntity config in Product.ZNodeConfigurableProductCollection)
                {
                    shipCost += config.ShippingCost;
                }

                //to add all group product shippingcost
                foreach (ZnodeProductBaseEntity group in Product.ZNodeGroupProductCollection)
                {
                    shipCost += group.ShippingCost;
                }

                return _ShippingCost + shipCost + Product.ShippingCost;
            }
            set
            {
                _ShippingCost = value;
            }
        }

        // Point where tax cost is being calculated.
        // Gets the tax cost for this line item.
        public virtual decimal TaxCost
        {
            get
            {
                decimal taxGST = 0;
                decimal taxPST = 0;
                decimal taxHST = 0;
                decimal taxVAT = 0;
                decimal taxSalesTax = 0;
                decimal taxCost = 0;

                if (HelperUtility.IsNotNull(Product))
                {
                    taxCost = Product.VAT + Product.GST + Product.HST + Product.PST + Product.SalesTax;

                    //to add all addOn product taxes
                    foreach (ZnodeProductBaseEntity addOn in Product.ZNodeAddonsProductCollection)
                    {
                        taxGST += addOn.GST;
                        taxPST += addOn.PST;
                        taxHST += addOn.HST;
                        taxVAT += addOn.VAT;
                        taxSalesTax += addOn.SalesTax;
                    }

                    //to add all configurable product taxes
                    foreach (ZnodeProductBaseEntity config in Product.ZNodeConfigurableProductCollection)
                    {
                        taxGST += config.GST;
                        taxPST += config.PST;
                        taxHST += config.HST;
                        taxVAT += config.VAT;
                        taxSalesTax += config.SalesTax;
                    }

                    //to add all group product taxes
                    foreach (ZnodeProductBaseEntity group in Product.ZNodeGroupProductCollection)
                    {
                        taxGST += group.GST;
                        taxPST += group.PST;
                        taxHST += group.HST;
                        taxVAT += group.VAT;
                        taxSalesTax += group.SalesTax;
                    }
                }
                return taxCost + taxGST + taxHST + taxPST + taxVAT + taxSalesTax;
            }
        }

        // Gets the discount amount applied to this line item.
        public virtual decimal DiscountAmount
        {
            get
            {
                decimal discountAmount = 0;
                if (HelperUtility.IsNotNull(Product))
                {
                    discountAmount += Product.DiscountAmount * Quantity;

                    //Loop through the selected addOns for this product
                    foreach (ZnodeProductBaseEntity addon in Product.ZNodeAddonsProductCollection)
                    {
                        discountAmount += addon.DiscountAmount * Quantity;
                    }
                }

                foreach (ZnodeProductBaseEntity config in Product.ZNodeConfigurableProductCollection)
                {
                    discountAmount += config.DiscountAmount * Quantity;
                }

                foreach (ZnodeProductBaseEntity group in Product.ZNodeGroupProductCollection)
                {
                    discountAmount += group.DiscountAmount * group.SelectedQuantity;
                }

                return discountAmount;
            }
        }

        // Gets the discount amount applied to OrderLevel.
        public virtual decimal OrderDiscountAmount
        {
            get
            {
                decimal orderDiscountAmount = 0;
                if (HelperUtility.IsNotNull(Product))
                {
                    orderDiscountAmount += Product.OrderDiscountAmount;

                    //Loop through the selected addOns for this product
                    foreach (ZnodeProductBaseEntity addon in Product.ZNodeAddonsProductCollection)
                    {
                        orderDiscountAmount += addon.OrderDiscountAmount;
                    }
                    foreach (ZnodeProductBaseEntity config in Product.ZNodeConfigurableProductCollection)
                    {
                        orderDiscountAmount += config.OrderDiscountAmount;
                    }

                    foreach (ZnodeProductBaseEntity group in Product.ZNodeGroupProductCollection)
                    {
                        orderDiscountAmount += group.OrderDiscountAmount;
                    }
                }

                return orderDiscountAmount;
            }
        }


        // Gets the discount amount applied to OrderLevel.
        public virtual decimal CSRDiscountAmount
        {
            get
            {
                decimal csrDiscountAmount = 0;
                if (HelperUtility.IsNotNull(Product))
                {
                    csrDiscountAmount += Product.CSRDiscountAmount;

                    //Loop through the selected addOns for this product
                    foreach (ZnodeProductBaseEntity addon in Product.ZNodeAddonsProductCollection)
                    {
                        csrDiscountAmount += addon.CSRDiscountAmount;
                    }

                    foreach (ZnodeProductBaseEntity config in Product.ZNodeConfigurableProductCollection)
                    {
                        csrDiscountAmount += config.CSRDiscountAmount;
                    }

                    foreach (ZnodeProductBaseEntity group in Product.ZNodeGroupProductCollection)
                    {
                        csrDiscountAmount += group.CSRDiscountAmount;
                    }
                }              

                return csrDiscountAmount;
            }
        }

        // Gets or sets the price of product after applying promotions on tiered price
        public virtual decimal PromotionalPrice { get; set; }

        // Gets or sets the discount amount applied to the extended price of this line item.
        public virtual decimal ExtendedPriceDiscount { get; set; }

        //Gets or sets the product description.
        public virtual string Description { get; set; }

        // Gets the promotion description.
        public virtual string PromoDescription { get { return _PromoDescription; } }

        // Gets or sets a value indicating whether is tax already calculated for the current shopping cart.
        public bool IsTaxCalculated { get; set; }

        //Gets the Image Name.
        public string Image { get; set; }

        // Gets Product Code.
        public string ProductCode { get; set; }

        // Gets Product Type.
        public string ProductType { get; set; }

        public virtual ZnodeShoppingCartItem Clone()
        {
            var copiedItem = MemberwiseClone() as ZnodeShoppingCartItem;
            copiedItem.Product = Product.Clone();
            return copiedItem;
        }

        // Get profile model on basis of user id from cache.
        public virtual int GetProfileCache()
        {
            int portalId = HelperUtility.GetPortalId();
            string cacheName = GetLoginUserId() > 0 ? $"ProfileCache_{ GetLoginUserId() }_{portalId}" : $"ProfileCache_{portalId}";

            List<ProfileModel> profileList = (List<ProfileModel>)HttpContext.Current.Cache[cacheName];

            //Null check for profile list and return default profile.
            if (profileList?.Count > 0)
                return profileList.Where(x => x.IsDefault.GetValueOrDefault())?.FirstOrDefault()?.ProfileId ?? 0;

            return 0;
        }

        //Get tier price.
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

        //to get group product quantity 
        public virtual decimal GetGroupProductQuantity(ZnodeGenericCollection<ZnodeProductBaseEntity> groupProduct)
        {
            decimal quantity = 0;
            if (groupProduct?.Count > 0)
            {
                foreach (ZnodeProductBaseEntity group in groupProduct)
                {
                    quantity += group.SelectedQuantity;
                }
            }
            return quantity;
        }

        public int OmsQuoteId { get; set; }
        public int ParentProductId { get; set; }
        public int OmsQuoteLineItemId { get; set; }
        public int OmsOrderLineItemId { get; set; }
        public int? ParentOmsQuoteLineItemId { get; set; }
        public int? OrderLineItemRelationshipTypeId { get; set; }
        public int Sequence { get; set; }
        public int GroupSequence { get; set; }
        public string SKU { get; set; }
        public string CustomText { get; set; }
        public string CartAddOnDetails { get; set; }
        public bool InsufficientQuantity { get; set; }
        public string UOM { get; set; }
        public string TrackingNumber { get; set; }
        //To get the order status of line item
        public int OrderStatusId { get; set; }
        public string OrderStatus { get; set; }
        public string TaxTransactionNumber { get; set; }
        public int TaxRuleId { get; set; }
        public bool IsEditStatus { get; set; }
        public bool IsActive { get; set; }
        public int? OmsOrderId { get; set; }
        public bool IsItemStateChanged { get; set; }
        public bool IsSendEmail { get; set; }
        public bool IsAllowedTerritories { get; set; } = true;
        public string AutoAddonSKUs { get; set; }
        public int OmsSavedCartLineItemId { get; set; }
        public string Custom1 { get; set; }
        public string Custom2 { get; set; }
        public string Custom3 { get; set; }
        public string Custom4 { get; set; }
        public string Custom5 { get; set; }

        public string GroupId { get; set; }

        public bool IsShippingReturn { get; set; }
        public decimal? PartialRefundAmount { get; set; }
        public int ParentOmsSavedCartLineItemId { get; set; }
        public string ParentProductSKU { get; set; }

        public int CreatedBy { get; set; }
        public int ModifiedBy { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime ModifiedDate { get; set; }

        public virtual decimal PerQuantityLineItemDiscount 
        {
            get
            {
                decimal lineDiscountAmount = 0;
                if (HelperUtility.IsNotNull(Product))
                {
                    lineDiscountAmount = Product.DiscountAmount;
                    foreach (ZnodeProductBaseEntity addon in Product.ZNodeAddonsProductCollection)
                    {
                        lineDiscountAmount += addon.DiscountAmount;
                    }

                    foreach (ZnodeProductBaseEntity config in Product.ZNodeConfigurableProductCollection)
                    {
                        lineDiscountAmount += config.DiscountAmount;
                    }

                    foreach (ZnodeProductBaseEntity group in Product.ZNodeGroupProductCollection)
                    {
                        lineDiscountAmount += group.DiscountAmount;
                    }
                    lineDiscountAmount = lineDiscountAmount > 0 ? lineDiscountAmount / Quantity : 0;
                }
                return lineDiscountAmount;
            }
        }
        public int? ParentOmsOrderLineItemsId { get; set; }

        public virtual decimal PerQuantityCSRDiscount
        {
            get
            {
                decimal lineItemCSRDiscount = 0;
                if (HelperUtility.IsNotNull(Product))
                {
                    lineItemCSRDiscount = Product.CSRDiscountAmount;
                    //Loop through the selected addOns for this product
                    foreach (ZnodeProductBaseEntity addon in Product.ZNodeAddonsProductCollection)
                    {
                        lineItemCSRDiscount += addon.CSRDiscountAmount;
                    }

                    foreach (ZnodeProductBaseEntity config in Product.ZNodeConfigurableProductCollection)
                    {
                        lineItemCSRDiscount += config.CSRDiscountAmount;
                    }

                    foreach (ZnodeProductBaseEntity group in Product.ZNodeGroupProductCollection)
                    {
                        lineItemCSRDiscount += group.CSRDiscountAmount;
                    }
                    lineItemCSRDiscount = lineItemCSRDiscount > 0 ? lineItemCSRDiscount / Quantity : 0;
                }
                return lineItemCSRDiscount;
            }
        }


        public virtual decimal PerQuantityOrderLevelDiscountOnLineItem
        {
            get
            {
                decimal lineItemOrderDiscount = 0;
                if (HelperUtility.IsNotNull(Product))
                {
                    lineItemOrderDiscount = Product.OrderDiscountAmount;

                    //Loop through the selected addOns for this product
                    foreach (ZnodeProductBaseEntity addon in Product.ZNodeAddonsProductCollection)
                    {
                        lineItemOrderDiscount += addon.OrderDiscountAmount;
                    }

                    foreach (ZnodeProductBaseEntity config in Product.ZNodeConfigurableProductCollection)
                    {
                        lineItemOrderDiscount += config.OrderDiscountAmount;
                    }

                    foreach (ZnodeProductBaseEntity group in Product.ZNodeGroupProductCollection)
                    {
                        lineItemOrderDiscount += group.OrderDiscountAmount;
                    }
                    lineItemOrderDiscount = lineItemOrderDiscount > 0 ? lineItemOrderDiscount / Quantity : 0;
                }
                return lineItemOrderDiscount;
            }
        }

        public virtual decimal PerQuantityShippingCost { get; set; }
        public virtual decimal PerQuantityShippingDiscount { get; set; }
        public DateTime? ShipDate { get; set; }
        public decimal PerQuantityVoucherAmount { get; set; }
        public string InventoryTracking { get; set; }

        public virtual decimal InitialPrice { get; set; }
        public virtual bool IsPriceEdit { get; set; }
        public virtual decimal InitialShippingCost { get; set; }
        public virtual decimal CustomShippingCost { get; set; }
        #endregion

        #region Private
        // Ger user Id from current request headers.
        protected virtual int GetLoginUserId()
        {
            const string headerUserId = "Znode-UserId";
            int userId = 0;
            var headers = HttpContext.Current.Request.Headers;
            int.TryParse(headers[headerUserId], out userId);
            return userId;
        }

        #endregion
    }
}
