using System;
using System.Xml.Serialization;

namespace Znode.Libraries.ECommerce.Entities
{
    /// Represents a bundle product item
    [Serializable()]
    [XmlRoot("ZNodeProductType")]
    public class ZnodeProductTypeEntity : ZnodeProductBaseEntity
    {
        #region Constructors
        // Initializes a new instance of the ZNodeBundleProductEntity class
        public ZnodeProductTypeEntity()
        {
        }

        // Initializes a new instance of the ZNodeBundleProductEntity class
        public ZnodeProductTypeEntity(ZnodeProductBaseEntity product)
        {
            // General Settings
            _Name = product.Name;
            _PortalID = product.PortalID;
            _ProductID = product.ProductID;
            _ImageFile = product.ImageFile;
            _IsActive = product.IsActive;
            _MaxQty = product.MaxQty;
            _MinQty = product.MinQty;
            _shortDescription = product.ShortDescription;
            _sku = product.SKU;
            _DisplayOrder = product.DisplayOrder;
            _downloadLink = product.DownloadLink;

            _Guid = product.GUID;
            _seoURL = product.SEOURL;

            // Collection Properties			
            _tieredPriceCollection = product.ZNodeTieredPriceCollection;

            // Inventory Settings
            _QuantityOnHand = product.QuantityOnHand;
            _AllowBackOrder = product.AllowBackOrder;
            _TrackInventoryInd = product.TrackInventoryInd;
            _BackOrderMsg = product.BackOrderMsg;

            // Shipping & tax
            _shippingCost = product.ShippingCost;
            _ShippingRuleTypeCode = product.ShippingRuleTypeCode;
            _shipSeparately = product.ShipSeparately;

            _freeShippingInd = product.FreeShippingInd;
            _TaxClassID = product.TaxClassID;
            _ShippingRate = product.ShippingRate;
            _RetailPrice = product.RetailPrice;
            _SalePrice = product.SalePrice;

            // Product Dimensions
            _height = product.Height;
            _length = product.Length;
            _width = product.Width;
            _Weight = product.Weight;

            // Recurring Billing
            _RecurringBillingInd = product.RecurringBillingInd;
            _RecurringBillingTotalCycles = product.RecurringBillingTotalCycles;
            _RecurringBillingPeriod = product.RecurringBillingPeriod;
            _RecurringBillingFrequency = product.RecurringBillingFrequency;
            _RecurringBillingInstallmentInd = product.RecurringBillingInstallmentInd;
            _RecurringBillingInitialAmount = product.RecurringBillingInitialAmount;

            // Bundle Product Collection
            ZNodeBundleProductCollection = product.ZNodeBundleProductCollection;

            isPromotionApplied = product.IsPromotionApplied;
            _AddonGroupName = product.AddonGroupName;
            _SelectedQuantity = product.SelectedQuantity;
            _InventoryTracking = product.InventoryTracking;
            _BrandCode = product.BrandCode;
            _Description = product.Description;
            _IsPriceExist = product.IsPriceExist;
            _attributes = product.Attributes;
            _productCategoryIds = product.ProductCategoryIds;
            _allowedTerritories = product.AllowedTerritories;
            _Container = product.Container;
            _Size = product.Size;
            _packagingType = product.PackagingType;
            _omsSavedCartLineItemId = product.OmsSavedCartLineItemId;
            _OrdersDiscount = product.OrdersDiscount;
        }
        #endregion
    }
}
