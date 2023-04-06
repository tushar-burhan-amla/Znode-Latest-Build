using System;
using System.Xml.Serialization;
using Znode.Engine.Api.Models;
using Znode.Engine.Promotions;
using Znode.Engine.Taxes;
using Znode.Libraries.ECommerce.Entities;
using Znode.Libraries.ECommerce.Utilities;

namespace Znode.Libraries.ECommerce.ShoppingCart
{
    /// <summary>
    /// Represents a product
    /// </summary>    
    [Serializable()]
    public class ZnodeProductBase : ZnodeProductBaseEntity, IZnodeProductBase
    {
        #region Private variables
        private int? shoppingCartQuantity;
        #endregion

        #region Constructors

        /// <summary>
        /// Initializes a new instance of the ZNodeProductBase class.
        /// </summary>
        public ZnodeProductBase()
        {
        }

        /// <summary>
        /// Initializes a new instance of the ZNodeProductBase class. With ZNodeProduct parameter.
        /// </summary>
        /// <param name="product">ZNodeProduct object</param>
        public ZnodeProductBase(ZnodeProduct product, AddressModel shippingdAddress, decimal? unitPrice = null)
        {
            // General Settings
            _Name = product._Name;
            _PortalID = product._PortalID;
            _ProductID = product._ProductID;
            _ProductNum = product._ProductNum;
            _ProductTypeID = product._ProductTypeID;
            _ImageFile = product._ImageFile;
            _IsActive = product._IsActive;
            _MaxQty = product._MaxQty;
            _MinQty = product._MinQty;
            _shortDescription = product._shortDescription;
            _sku = product._sku;
            _addOnDescription = product._addOnDescription;
            _DisplayOrder = product._DisplayOrder;
            _downloadLink = product._downloadLink;
            _Guid = product._Guid;
            _seoURL = product._seoURL;
            // Collection Properties           
            _tieredPriceCollection = product._tieredPriceCollection;
            // Inventory Settings
            _QuantityOnHand = product._QuantityOnHand;
            _AllowBackOrder = product._AllowBackOrder;
            _TrackInventoryInd = product._TrackInventoryInd;
            _BackOrderMsg = product._BackOrderMsg;
            _CallMessage = product._CallMessage;

            // Shipping & tax
            _shippingCost = product._shippingCost;
            _ShippingRuleTypeCode = product._ShippingRuleTypeCode;
            _shipSeparately = product._shipSeparately;
            shoppingCartQuantity = product.shoppingCartQuantity;
            _freeShippingInd = product._freeShippingInd;
            _TaxClassID = product._TaxClassID;
            _ShippingRate = product.ShippingRate;
            _RetailPrice = product.RetailPrice;
            _SalePrice = product.SalePrice;
            _CallForPricing = product._CallForPricing;
            _CallMessage = product._CallMessage;
            // Product Dimensions
            _height = product._height;
            _length = product._length;
            _width = product._width;
            _Weight = product._Weight;

            // Recurring Billing
            _RecurringBillingInd = product._RecurringBillingInd;
            _RecurringBillingTotalCycles = product._RecurringBillingTotalCycles;
            _RecurringBillingPeriod = product._RecurringBillingPeriod;
            _RecurringBillingFrequency = product._RecurringBillingFrequency;
            _RecurringBillingInstallmentInd = product._RecurringBillingInstallmentInd;
            _RecurringBillingInitialAmount = product._RecurringBillingInitialAmount;

            // Bundle Product Collection
            ZNodeBundleProductCollection = product.ZNodeBundleProductCollection;
            isPromotionApplied = product.IsPromotionApplied;
            _ShippingAddress = shippingdAddress;
            _InventoryTracking = product.InventoryTracking;
            _BrandCode = product.BrandCode;
            _IsPriceExist = product.IsPriceExist;
            _VendorCode = product.VendorCode;
            _attributes = product.Attributes;
            _productCategoryIds = product.ProductCategoryIds;
            _allowedTerritories = product.AllowedTerritories;
            _CustomPrice = HelperUtility.IsNotNull(unitPrice) ? unitPrice : null;
        }

        #endregion

        #region Public Properties      


        // Gets or sets the retail price. Will return the SKU override retail price if one exists.        
        [XmlIgnore()]
        public new decimal RetailPrice
        {
            get
            {
                decimal retailPrice = _RetailPrice;

                ZnodeInclusiveTax salesTax = new ZnodeInclusiveTax();

                retailPrice = salesTax.GetInclusivePrice(_TaxClassID, retailPrice, AddressToShip, _ShippingAddress);

                return retailPrice;
            }

            set
            {
                _RetailPrice = value;
            }
        }

        // Gets or sets the product sale price as a decimal value
        [XmlIgnore()]
        public new decimal? SalePrice
        {
            get
            {
                decimal? salePrice = _SalePrice;

                if (salePrice.HasValue)
                {
                    ZnodeInclusiveTax salesTax = new ZnodeInclusiveTax();

                    salePrice = salesTax.GetInclusivePrice(_TaxClassID, salePrice.Value, AddressToShip, _ShippingAddress);

                    return salePrice;
                }

                return _SalePrice;
            }

            set
            {
                _SalePrice = value;
            }
        }

        // Gets the final calculated price for a product which includes inclusive tax
        [XmlIgnore()]
        public new decimal FinalPrice
        {
            get
            {
                decimal finalPrice = ProductPrice;

                ZnodeInclusiveTax inclusiveTax = new ZnodeInclusiveTax();

                finalPrice = inclusiveTax.GetInclusivePrice(_TaxClassID, finalPrice, AddressToShip, _ShippingAddress);

                return finalPrice;
            }
        }

        // Gets the final calculated price for a product without inclusive tax
        [XmlIgnore()]
        public new decimal ProductPrice
        {
            get
            {
                ZnodePricePromotionManager pricePromoManager = new ZnodePricePromotionManager();
                return pricePromoManager.PromotionalPrice(this);
            }
        }

        // Gets or sets the retail price. Will return the SKU override retail price if one exists.        
        [XmlIgnore()]
        public override decimal? CustomPrice
        {
            get
            {
                return _CustomPrice;
            }

            set
            {
                _CustomPrice = value;
            }
        }

        #endregion

        #region Static Create      

        /// <summary>
        /// Apply Promotion Product if any.
        /// </summary>
        public void ApplyPromotion()
        {
            ZnodeProductPromotionManager productPromoManager = new ZnodeProductPromotionManager();
            productPromoManager?.ChangeDetails(this);
        }

        #endregion
    }
}