using System;
using System.Collections.Generic;
using System.Xml.Serialization;
using Znode.Engine.Api.Models;
using Znode.Libraries.ECommerce.Utilities;
using Znode.Libraries.Framework.Business;

namespace Znode.Libraries.ECommerce.Entities
{
    // Represents a product in the catalog
    [Serializable()]
    [XmlRoot("ZNodeProduct")]
    public class ZnodeProductBaseEntity : ZnodeBusinessBase
    {
        #region Private Variables
        protected int _ProductID = 0;
        protected int _PortalID = 0;
        protected string _Name = string.Empty;
        protected string _ProductNum = string.Empty;
        protected int _ProductTypeID;
        protected decimal _RetailPrice;
        protected string _ImageFile;
        protected decimal _Weight;
        protected bool _IsActive;
        protected decimal? _MaxQty;
        protected decimal? _MinQty;
        protected int _DisplayOrder;
        protected string _CallMessage = string.Empty;
        protected string _ShippingRuleTypeCode;
        protected decimal? _SalePrice;
        protected decimal _DiscountAmount;
        protected decimal _OrderDiscountAmount;
        protected string _downloadLink = string.Empty;
        protected string _addOnDescription = string.Empty;
        protected bool _TrackInventoryInd;
        protected bool _AllowBackOrder;
        protected decimal _QuantityOnHand;
        protected string _sku = String.Empty;
        protected decimal _shippingCost;
        protected decimal _height;
        protected decimal _width;
        protected decimal _length;
        protected string _shortDescription = string.Empty;
        protected string _GiftCardDescription = string.Empty;
        protected bool _freeShippingInd = false;
        protected int _TaxClassID = 0;
        protected bool _shipSeparately = false;
        protected string _Guid = string.Empty;
        protected string _BackOrderMsg = string.Empty;
        protected string _seoURL = string.Empty;
        protected bool _CallForPricing;
        protected string _Description = string.Empty;
        protected string _allowedTerritories = string.Empty;
        protected bool _RecurringBillingInd = false;
        protected bool _RecurringBillingInstallmentInd = false;
        protected string _RecurringBillingPeriod = "";
        protected string _RecurringBillingFrequency = "";
        protected int _RecurringBillingTotalCycles = 0;
        protected decimal _RecurringBillingInitialAmount = 0;

        protected decimal _GST = 0;
        protected decimal _HST = 0;
        protected decimal _PST = 0;
        protected decimal _VAT = 0;
        protected decimal _ImportDuty = 0;
        protected decimal _SalesTax = 0;

        protected decimal _ShippingRate;

        protected bool isPromotionApplied = false;

        protected string _InventoryTracking = string.Empty;
        protected string _AddonGroupName = string.Empty;
        protected decimal _SelectedQuantity;
        protected AddressModel _ShippingAddress;
        protected string _BrandCode = string.Empty;
        protected bool _IsPriceExist;
        protected string _VendorCode;
        protected List<OrderAttributeModel> _attributes = new List<OrderAttributeModel>();
        protected int[] _productCategoryIds;
        protected decimal? _CustomPrice;
        //Property used for Shipping USPS
        protected string _Container;
        protected string _Size;
        protected string _ProductKey;
        //Property used for FedEx Shipping.
        protected string _packagingType;
        protected int? _omsSavedCartLineItemId;
        protected decimal _CSRDiscountAmount;
        protected decimal _PromotionalPrice;
        protected List<OrderDiscountModel> _OrdersDiscount;
        #endregion

        #region protected MemberObjects      
        protected ZnodeGenericCollection<ZnodeProductTierEntity> _tieredPriceCollection = new ZnodeGenericCollection<ZnodeProductTierEntity>();
        private ZnodeGenericCollection<ZnodeProductBaseEntity> _addonsProductCollection = new ZnodeGenericCollection<ZnodeProductBaseEntity>();
        private ZnodeGenericCollection<ZnodeProductBaseEntity> _bundleProductCollection = new ZnodeGenericCollection<ZnodeProductBaseEntity>();
        private ZnodeGenericCollection<ZnodeProductBaseEntity> _configurableProductCollection = new ZnodeGenericCollection<ZnodeProductBaseEntity>();
        private ZnodeGenericCollection<ZnodeProductBaseEntity> _groupProductCollection = new ZnodeGenericCollection<ZnodeProductBaseEntity>();
        #endregion

        #region Constructors

        public ZnodeProductBaseEntity()
        {

        }

        public virtual ZnodeProductBaseEntity Clone()
        {
            var copiedItem = MemberwiseClone() as ZnodeProductBaseEntity;
            return copiedItem;
        }
        #endregion

        #region Public Properties
        // Gets or sets the product description
        [XmlElement()]
        public string Description
        {
            get { return _Description; }
            set { _Description = value; }
        }

        // Retrieves the GUID(unique value) for this product. 
        [XmlIgnore()]
        public string GUID
        {
            get { return _Guid; }
            set { _Guid = value; }
        }

        // Collection of bundle product association 
        [XmlElement("ZNodeAddonsProduct")]
        public ZnodeGenericCollection<ZnodeProductBaseEntity> ZNodeAddonsProductCollection
        {
            get { return _addonsProductCollection; }
            set { _addonsProductCollection = value; }
        }

        // Collection of bundle product association 
        [XmlElement("ZNodeBundleProduct")]
        public ZnodeGenericCollection<ZnodeProductBaseEntity> ZNodeBundleProductCollection
        {
            get { return _bundleProductCollection; }
            set { _bundleProductCollection = value; }
        }

        // Collection of configurable product association 
        [XmlElement("ZNodeConfigurableProduct")]
        public ZnodeGenericCollection<ZnodeProductBaseEntity> ZNodeConfigurableProductCollection
        {
            get { return _configurableProductCollection; }
            set { _configurableProductCollection = value; }
        }

        // Collection of group product association 
        [XmlElement("ZNodeGroupProduct")]
        public ZnodeGenericCollection<ZnodeProductBaseEntity> ZNodeGroupProductCollection
        {
            get { return _groupProductCollection; }
            set { _groupProductCollection = value; }
        }

        public List<OrderAttributeModel> Attributes
        {
            get { return _attributes; }
            set { _attributes = value; }
        }

        // Gets or sets the ProductID
        [XmlElement()]
        public int ProductID
        {
            get
            {
                return _ProductID;
            }
            set
            {
                _ProductID = value;
            }
        }

        // Gets or sets the site portal id
        [XmlElement()]
        public int PortalID
        {
            get
            {
                return _PortalID;
            }
            set
            {
                _PortalID = value;
            }
        }

        // Gets or sets the product Name
        [XmlElement()]
        public string Name
        {
            get
            {
                return _Name;
            }
            set
            {
                _Name = value;
            }
        }

        // Gets or sets the product shortdescription
        [XmlElement()]
        public string ShortDescription
        {
            get
            {
                return _shortDescription;
            }
            set
            {
                _shortDescription = value;
            }
        }

        // Gets or sets the retail price. Will return the SKU override retail price if one exists.        
        [XmlElement()]
        public decimal RetailPrice
        {
            get
            {
                decimal retailPrice = _RetailPrice;
                return retailPrice;
            }
            set
            {
                _RetailPrice = value;
            }
        }

        // Gets or sets the discount amount applied to this line item.
        public decimal DiscountAmount
        {
            get
            {
                return _DiscountAmount;
            }
            set
            {
                _DiscountAmount = value;
            }
        }

        // Gets or sets the Order level discount amount applied to this line item.
        public decimal OrderDiscountAmount
        {
            get
            {
                return _OrderDiscountAmount;
            }
            set
            {
                _OrderDiscountAmount = value;
            }
        }

        // Gets or sets the CSR level discount amount applied to this line item.
        public decimal CSRDiscountAmount
        {
            get
            {
                return _CSRDiscountAmount;
            }
            set
            {
                _CSRDiscountAmount = value;
            }
        }

        // Gets or sets the discount description applied to this line item.
        public List<OrderDiscountModel> OrdersDiscount
        {
            get
            {
                return _OrdersDiscount;
            }
            set
            {
                _OrdersDiscount = value;
            }
        }

        // Gets or sets the tiered prices related with this product
        [XmlElement("ZNodeProductTier")]
        public ZnodeGenericCollection<ZnodeProductTierEntity> ZNodeTieredPriceCollection
        {
            get
            {
                return _tieredPriceCollection;
            }
            set
            {
                _tieredPriceCollection = value;
            }
        }

        // Gets or sets the product sale price as a decimal value
        [XmlElement()]
        public virtual decimal? SalePrice
        {
            get
            {
                return _SalePrice;
            }
            set
            {
                _SalePrice = value;
            }
        }
        

        // Gets the tiered Price for this product for the quantity 1.
        // If no tiered pricing applied, it will return 0
        public virtual decimal TieredPrice
        {
            get
            {
                decimal tieredPrice = 0;

                // Check product tiers list
                if (_tieredPriceCollection?.Count > 0)
                {
                    foreach (ZnodeProductTierEntity productTieredPrice in _tieredPriceCollection)
                    {
                        if (productTieredPrice.TierQuantity >= 1)
                        {
                            if (SelectedQuantity >= productTieredPrice.MinQuantity && SelectedQuantity < productTieredPrice.MaxQuantity)
                            {
                                tieredPrice = productTieredPrice.Price;
                                break;
                            }
                        }
                    }
                }

                // Get Product tiered price if one exists, otherwise 0
                return tieredPrice;
            }
        }

        // Gets the final calculated price for a product which includes inclusive tax
        [XmlIgnore()]
        public decimal FinalPrice
        {
            get { return ProductPrice; }
        }

        // Gets the final calculated price for a product without inclusive tax
        [XmlIgnore()]
        public virtual decimal ProductPrice
        {
            get
            {
                decimal basePrice = _RetailPrice;

                decimal? salePrice = _SalePrice;

                if (salePrice.HasValue)
                    basePrice = salePrice.Value;

                decimal tieredPrice = TieredPrice;

                return tieredPrice > 0 ? tieredPrice : basePrice;
            }
        }

        // Gets or sets the image file name for this product. Will return the SKU picture override if one exists.
        [XmlElement()]
        public string ImageFile
        {
            get
            {
                return _ImageFile;
            }

            set { _ImageFile = value; }
        }

        // Gets or sets a value indicating whether the product is active.
        [XmlElement(ElementName = "ActiveInd")]
        public bool IsActive
        {
            get { return _IsActive; }
            set { _IsActive = value; }
        }

        // Gets or sets the display order for this product
        [XmlElement()]
        public int DisplayOrder
        {
            get { return _DisplayOrder; }
            set { _DisplayOrder = value; }
        }

        // Sets or retrieves the ShippingRuleTypeID for this product
        [XmlElement()]
        public string ShippingRuleTypeCode
        {
            get { return _ShippingRuleTypeCode; }
            set { _ShippingRuleTypeCode = value; }
        }

        // Gets or sets the TrackInventoryInd property for this product
        [XmlElement()]
        public bool TrackInventoryInd
        {
            get { return _TrackInventoryInd; }
            set { _TrackInventoryInd = value; }
        }

        // Gets or sets the AllowBackOrder property for this product
        [XmlElement()]
        public bool AllowBackOrder
        {
            get { return _AllowBackOrder; }
            set { _AllowBackOrder = value; }
        }

        // Gets or sets the InventoryTracking for this product
        [XmlElement()]
        public string InventoryTracking
        {
            get { return _InventoryTracking; }
            set { _InventoryTracking = value; }
        }

        // Gets or sets the AddonGroupName for this product
        [XmlElement()]
        public string AddonGroupName
        {
            get { return _AddonGroupName; }
            set { _AddonGroupName = value; }
        }

        // Gets or sets the BrandCode for this product
        [XmlElement()]
        public string BrandCode
        {
            get { return _BrandCode; }
            set { _BrandCode = value; }
        }

        // Gets or sets the SKU for this product
        // It will return the SKU override if one exists.
        [XmlElement()]
        public string SKU
        {
            get
            {
                return _sku;
            }
            set
            {
                _sku = value;
            }
        }

        // Gets or sets the QuantityOnHand for this product
        // It will return the SKU Quantity Available if one exists
        [XmlElement()]
        public decimal QuantityOnHand
        {
            get
            {
                return _QuantityOnHand;
            }
            set
            {
                _QuantityOnHand = value;
            }
        }

        // Gets or sets the SelectedQuantity for this product
        [XmlElement()]
        public decimal SelectedQuantity
        {
            get { return _SelectedQuantity; }
            set { _SelectedQuantity = value; }
        }

        // Gets or sets the MaxQuantity       
        [XmlElement()]
        public decimal? MinQty
        {
            get { return _MinQty; }
            set { _MinQty = value; }
        }

        // Gets or sets the MaxQuantity       
        [XmlElement()]
        public virtual decimal? MaxQty
        {
            get
            {
                return _MaxQty;
            }
            set
            {
                _MaxQty = value;
            }
        }

        // Gets or sets the product height         
        [XmlElement()]
        public decimal Height
        {
            get { return _height; }
            set { _height = value; }
        }

        // Gets or sets the product width         
        [XmlElement()]
        public decimal Width
        {
            get { return _width; }
            set { _width = value; }
        }

        // Gets or sets the product length         
        [XmlElement()]
        public decimal Length
        {
            get { return _length; }
            set { _length = value; }
        }

        // Gets or sets the weight of this product 
        [XmlElement()]
        public decimal Weight
        {
            get
            {
                return _Weight;
            }
            set
            {
                _Weight = value;
            }
        }

        //Gets or sets the download Link
        [XmlElement()]
        public string DownloadLink
        {
            get { return _downloadLink; }
            set { _downloadLink = value; }
        }

        //Gets or sets the free shipping property
        [XmlElement()]
        public bool FreeShippingInd
        {
            get { return _freeShippingInd; }
            set { _freeShippingInd = value; }
        }

        //Gets or sets the ship separately boolean property
        [XmlElement()]
        public bool ShipSeparately
        {
            get { return _shipSeparately; }
            set { _shipSeparately = value; }
        }

        //Gets or sets the tax exempt property
        [XmlElement()]
        public int TaxClassID
        {
            get { return _TaxClassID; }
            set { _TaxClassID = value; }
        }

        // Gets or sets the SEO url.
        [XmlElement()]
        public string SEOURL
        {
            get { return _seoURL; }
            set { _seoURL = value; }
        }

        // Gets or sets a value indicating whether the CallForPricing is enabled.
        [XmlElement()]
        public bool CallForPricing
        {
            get { return _CallForPricing; }
            set { _CallForPricing = value; }
        }

        // Gets or Sets the billing enable property
        [XmlElement()]
        public bool RecurringBillingInd
        {
            get { return _RecurringBillingInd; }
            set { _RecurringBillingInd = value; }
        }

        // Gets or Sets the installmentInd  property
        [XmlElement()]
        public bool RecurringBillingInstallmentInd
        {
            get { return _RecurringBillingInstallmentInd; }
            set { _RecurringBillingInstallmentInd = value; }
        }

        // Gets or Sets the billing period (days or months), in association with the frequency.
        [XmlElement()]
        public string RecurringBillingPeriod
        {
            get
            {
                return _RecurringBillingPeriod;
            }
            set { _RecurringBillingPeriod = value; }
        }

        // Gets or Sets the billing frequency, in association with the Period.
        [XmlElement()]
        public string RecurringBillingFrequency
        {
            get
            {
                return _RecurringBillingFrequency;
            }
            set { _RecurringBillingFrequency = value; }
        }

        // Gets or sets the number of billing occurrences or payments for the subscription
        [XmlElement()]
        public int RecurringBillingTotalCycles
        {
            get
            {
                return _RecurringBillingTotalCycles;
            }
            set { _RecurringBillingTotalCycles = value; }
        }

        // Gets or sets the initial amount to be charged during subscription creation,
        [XmlElement()]
        public decimal RecurringBillingInitialAmount
        {
            get { return _RecurringBillingInitialAmount; }
            set { _RecurringBillingInitialAmount = value; }
        }

        [XmlElement()]
        public decimal ShippingRate
        {
            get { return _ShippingRate; }
            set { _ShippingRate = value; }
        }

        // Gets or sets Promotion Applied or not.
        [XmlIgnore()]
        public bool IsPromotionApplied
        {
            get { return isPromotionApplied; }
            set { isPromotionApplied = value; }
        }

        [XmlElement()]
        public bool IsPriceExist
        {
            get { return _IsPriceExist; }
            set { _IsPriceExist = value; }
        }

        // Gets or sets the Product Vendor Code
        [XmlElement()]
        public string VendorCode
        {
            get { return _VendorCode; }
            set { _VendorCode = value; }
        }

        // Gets or sets the Product Vendor Code
        [XmlElement()]
        public int[] ProductCategoryIds
        {
            get { return _productCategoryIds; }
            set { _productCategoryIds = value; }
        }

        //Gets or sets the ship separately boolean property
        [XmlElement()]
        public string AllowedTerritories
        {
            get { return _allowedTerritories; }
            set { _allowedTerritories = value; }
        }

        public bool TaxCalculated { get; set; }
        #endregion

        #region Public Instance Properties - Related to Shopping Cart

        // Returns the discounted final price of the product
        // This method calculates the addon price, product price which includes promotions discount.
        // If Tiered price exists, then it will override the product price
        public virtual decimal AddOnPrice
        {
            get
            {
                decimal addOnRetailPriceTotal = 0;

                #region Calculate AddOn additional price without promotions discount
                // Loop through the selected addOns for this product
                foreach (ZnodeProductBaseEntity AddOns in this.ZNodeAddonsProductCollection)
                {
                    // calculate additional price
                    addOnRetailPriceTotal += AddOns.FinalPrice;
                }
                #endregion

                return addOnRetailPriceTotal;
            }
        }

        /// <summary>
        /// This method calculates the extended addon price 
        /// </summary>
        public decimal ExtendedAddOnPrice
        {
            get
            {
                decimal addOnRetailPriceTotal = 0;

                // Loop through the selected addOns for this product
                foreach (ZnodeProductBaseEntity AddOns in this.ZNodeAddonsProductCollection)
                    // calculate add-ons extended price
                    addOnRetailPriceTotal += AddOns.FinalPrice * AddOns.SelectedQuantity;

                return addOnRetailPriceTotal;
            }
        }

        // Returns the discounted final price of the product
        // This method calculates the configurable price, product price which includes promotions discount.
        // If Tiered price exists, then it will override the product price
        public virtual decimal ConfigurableProductPrice
        {
            get
            {
                decimal configurableProductPrice = 0;

                foreach (ZnodeProductBaseEntity configurableItem in this.ZNodeConfigurableProductCollection)
                    // calculate configurable product price
                    configurableProductPrice += configurableItem.FinalPrice;

                return configurableProductPrice;
            }
        }

        // Returns the discounted final price of the product
        // This method calculates the group price, product price which includes promotions discount.
        // If Tiered price exists, then it will override the product price
        public virtual decimal GroupProductPrice
        {
            get
            {
                decimal groupProductPrice = 0;

                foreach (ZnodeProductBaseEntity groupItem in this.ZNodeGroupProductCollection)
                    // calculate group product price
                    groupProductPrice += groupItem.FinalPrice;

                return groupProductPrice;
            }
        }

        // Returns the total shopping cart description for this item
        public virtual string ShoppingCartDescription
        {
            get
            {
                string stockMessage = string.Empty;
                string productstockMessage = string.Empty;
                string msg = string.Empty;

                // Allow Back Order
                if (string.Equals(InventoryTracking, ZnodeConstant.AllowBackOrdering, StringComparison.OrdinalIgnoreCase) && QuantityOnHand <= 0 && !string.IsNullOrEmpty(_BackOrderMsg))
                    //Set back order message
                    stockMessage = $"Note : {_BackOrderMsg}";

                msg = stockMessage + " " + (string.IsNullOrEmpty(GiftCardDescription) ? string.Empty : GiftCardDescription + "<br/>");

                foreach (ZnodeProductBaseEntity bundleProduct in ZNodeBundleProductCollection)
                {
                    if (string.Equals(bundleProduct.InventoryTracking, ZnodeConstant.AllowBackOrdering, StringComparison.OrdinalIgnoreCase) && bundleProduct.QuantityOnHand <= 0 && !string.IsNullOrEmpty(bundleProduct.BackOrderMsg))
                        //Set back order message
                        productstockMessage = $"Note : { bundleProduct.BackOrderMsg}";

                    if (!string.IsNullOrEmpty(productstockMessage))
                        productstockMessage += "<br/>";
                    msg += $"Qty: {bundleProduct.SelectedQuantity } | { bundleProduct.SKU } - { bundleProduct.Name } <br/>{  productstockMessage }";
                }

                foreach (ZnodeProductBaseEntity configurableProduct in ZNodeConfigurableProductCollection)
                {
                    if (string.Equals(configurableProduct.InventoryTracking, ZnodeConstant.AllowBackOrdering, StringComparison.OrdinalIgnoreCase) && configurableProduct.QuantityOnHand <= 0 && !string.IsNullOrEmpty(configurableProduct.BackOrderMsg))
                        //Set back order message
                        productstockMessage = $"Note : { configurableProduct.BackOrderMsg}";

                    msg += (string.IsNullOrEmpty(configurableProduct.Description)?string.Empty : $"{configurableProduct.Description} <br/>") ;
                }

                foreach (ZnodeProductBaseEntity groupProduct in ZNodeGroupProductCollection)
                {
                    if (string.Equals(groupProduct.InventoryTracking, ZnodeConstant.AllowBackOrdering, StringComparison.OrdinalIgnoreCase) && groupProduct.QuantityOnHand <= 0 && !string.IsNullOrEmpty(groupProduct.BackOrderMsg))
                        //Set back order message
                        productstockMessage = $"Note : { groupProduct.BackOrderMsg}";
                }

                foreach (ZnodeProductBaseEntity addonProduct in ZNodeAddonsProductCollection)
                {
                    if (string.Equals(addonProduct.InventoryTracking, ZnodeConstant.AllowBackOrdering, StringComparison.OrdinalIgnoreCase) && addonProduct.QuantityOnHand <= 0 && !string.IsNullOrEmpty(addonProduct.BackOrderMsg))
                        //Set back order message
                        productstockMessage = $"Note : { addonProduct.BackOrderMsg}";

                    msg += $"{ addonProduct.AddonGroupName } : { addonProduct.Name } <br />";
                }

                return msg;
            }
        }

        // Stores metadata on the gift card.(attribute selection, etc)
        // This metadata is displayed on the shopping cart list
        // This property is set explicitly by the UI and is not retrieved automatically
        [XmlElement()]
        public string GiftCardDescription
        {
            get { return _GiftCardDescription; }
            set { _GiftCardDescription = value; }
        }

        // Gets or Sets the gift card number.
        [XmlElement()]
        public string GiftCardNumber { get; set; } = string.Empty;

        // Gets or sets the BackOrder Message
        [XmlElement()]
        public string BackOrderMsg
        {
            get { return _BackOrderMsg; }
            set { _BackOrderMsg = value; }
        }

        // Gets or Sets of shipping cost
        [XmlIgnore()]
        public decimal ShippingCost
        {
            get { return _shippingCost; }
            set { _shippingCost = value; }
        }

        // Gets or Sets of HST
        [XmlIgnore()]
        public decimal HST
        {
            get { return _HST; }
            set { _HST = value; }
        }

        // Gets or Sets of GST
        [XmlIgnore()]
        public decimal GST
        {
            get { return _GST; }
            set { _GST = value; }
        }

        // Gets or Sets of PST
        [XmlIgnore()]
        public decimal PST
        {
            get { return _PST; }
            set { _PST = value; }
        }

        [XmlIgnore()]
        public decimal VAT
        {
            get { return _VAT; }
            set { _VAT = value; }
        }

        [XmlIgnore()]
        public decimal ImportDuty
        {
            get { return _ImportDuty; }
            set { _ImportDuty = value; }
        }

        [XmlIgnore()]
        public decimal SalesTax
        {
            get { return _SalesTax; }
            set { _SalesTax = value; }
        }

        //todo : Dependency on DataAccess(Need to be done using EF)
        [XmlIgnore()]
        public AddressModel AddressToShip { get; set; }

        // Gets or sets the SKU for this product
        // It will return the SKU override if one exists.
        [XmlElement()]
        public string Container
        {
            get
            {
                return _Container;
            }
            set
            {
                _Container = value;
            }
        }

        [XmlElement()]
        public string PackagingType
        {
            get
            {
                return _packagingType;
            }
            set
            {
                _packagingType = value;
            }
        }

        // Gets or sets the SKU for this product
        // It will return the SKU override if one exists.
        [XmlElement()]
        public string Size
        {
            get
            {
                return _Size;
            }
            set
            {
                _Size = value;
            }
        }

        [XmlElement()]
        public string DownloadableProductKey
        {
            get
            {
                return _ProductKey;
            }
            set
            {
                _ProductKey = value;
            }
        }

        [XmlIgnore()]
        public int? OmsSavedCartLineItemId
        {
            get
            {
                return _omsSavedCartLineItemId;
            }
            set
            {
                _omsSavedCartLineItemId = value;
            }
        }

        // Gets or sets the retail price. Will return the SKU override retail price if one exists.        
        [XmlIgnore()]
        public virtual decimal? CustomPrice
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

        // Gets or sets the Promotional price. Will return the Promotional price if one exists.        
        [XmlIgnore()]
        public virtual decimal PromotionalPrice
        {
            get
            {
                return _PromotionalPrice;
            }

            set
            {
                _PromotionalPrice = value;
            }
        }
        #endregion

    }
}
